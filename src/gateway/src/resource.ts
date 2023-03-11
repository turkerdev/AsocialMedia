import db from "./db";
import { ResourceServer } from "./protos/main";
import amqp from 'amqplib'
import { env } from "./env";
import * as grpc from '@grpc/grpc-js'
import { z } from "zod";
import { GetObjectCommand } from "@aws-sdk/client-s3";
import s3 from "./s3";
import * as google from '@googleapis/youtube'

export const resource_Impl: ResourceServer = {
    new: async (call, callback) => {
        const body = await z.object({
            url: z.string().url()
        }).safeParseAsync(call.request)

        if (!body.success) {
            callback({ code: grpc.status.INVALID_ARGUMENT }, null)
            return
        }

        const resource = await db.resource.create({ data: { url: body.data.url } })
        console.log("New resource created: ", resource)
        callback(null, { id: resource.id });
    },
    download: async (call, callback) => {
        const resource = await db.resource.findUnique({ where: { id: call.request.id } })
        if (!resource) {
            callback({ code: grpc.status.NOT_FOUND }, null)
            return
        }

        const connection = await amqp.connect(env.RABBITMQ_URL)
        const channel = await connection.createChannel()

        const payload = {
            id: resource.id,
            url: resource.url
        }

        channel.sendToQueue('dl', Buffer.from(JSON.stringify(payload)))
        await channel.close();
        await connection.close();
        callback(null, null)
    },
    upload: async (call, callback) => {
        const resource = await db.resource.findUnique({ where: { id: call.request.resourceId } })
        if (!resource) {
            callback({ code: grpc.status.NOT_FOUND }, null)
            return
        }

        const command = new GetObjectCommand({
            Bucket: "app-bucket",
            Key: resource.id
        })
        const { ContentLength: fileSize } = await s3.send(command)

        if (!fileSize) {
            callback({ code: grpc.status.DATA_LOSS }, null)
            return
        }

        const maxChunkSize = Math.min(1024 * 1024 * 90, fileSize);
        const parts: {
            from: number;
            to: number;
            chunkSize: number;
            total: number;
        }[] = []

        // This might be dangerous for large files
        // It may exceed the maximum safe number
        for (let from = 0; from < fileSize; from += maxChunkSize) {
            const chunkSize = Math.min(maxChunkSize, fileSize - from);
            const to = Math.min(fileSize, from + chunkSize) - 1;
            parts.push({
                from,
                to,
                chunkSize,
                total: fileSize,
            });
        }

        const auth = new google.auth.OAuth2({
            clientId: env.GOOGLE_CLIENT_ID,
            clientSecret: env.GOOGLE_CLIENT_SECRET,
        })

        //TODO: For now, just upload to the first channel
        const channel = await db.channel.findUnique({ where: { id: call.request.channelId[0] } })
        if (!channel) {
            callback({ code: grpc.status.NOT_FOUND }, null)
            return
        }

        auth.setCredentials({
            access_token: channel.access_token,
            refresh_token: channel.refresh_token
        })

        await auth.getAccessToken()

        const youtube = google.youtube({ version: 'v3', auth: auth })
        const video = await youtube.videos.insert(
            {
                uploadType: "resumable",
                part: [],
            },
            {
                rootUrl: "https://www.googleapis.com/upload/"
            }
        )

        const headers = new Headers(video.headers)
        const location = headers.get('Location')

        if (!location) {
            callback({ code: grpc.status.FAILED_PRECONDITION }, null)
        }

        for await (const part of parts) {
            const response = await fetch(`${env.UPLOAD_URL}/chunk`, {
                method: "POST",
                body: JSON.stringify({
                    key: resource.id,
                    token: auth.credentials.access_token,
                    url: location,
                    chunksize: part.chunkSize,
                    from: part.from,
                    to: part.to,
                    total: part.total
                })
            })
            console.log(`${response.status}: ${await response.text()}`)
            if (![200, 308].includes(response.status)) {
                callback({ code: grpc.status.INTERNAL }, null)
                return;
            }
        }

        callback(null, null)
    }
}