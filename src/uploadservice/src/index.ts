import { GetObjectCommand, S3Client } from '@aws-sdk/client-s3'
import { z } from 'zod'

addEventListener("fetch", (event) => {
	event.respondWith(handleRequest(event));
})

async function handleRequest(e: FetchEvent): Promise<Response> {
	const { pathname } = new URL(e.request.url);
	if (pathname === "/chunk") {
		return await handleChunk(e);
	}
	return new Response("Not found", { status: 404 });
}

async function handleChunk(e: FetchEvent): Promise<Response> {
	const env = await z.object({
		BUCKET_URL: z.string(),
		BUCKET_KEY: z.string(),
		BUCKET_SECRET: z.string()
	}).parseAsync({
		BUCKET_URL,
		BUCKET_KEY,
		BUCKET_SECRET
	})

	const json = await e.request.json();
	const request = await z.object({
		key: z.string(),
		token: z.string(),
		url: z.string(),
		chunksize: z.number(),
		from: z.number(),
		to: z.number(),
		total: z.number()
	}).parseAsync(json)

	const s3 = new S3Client({
		endpoint: env.BUCKET_URL,
		credentials: {
			accessKeyId: env.BUCKET_KEY,
			secretAccessKey: env.BUCKET_SECRET,
		},
		forcePathStyle: true,
		region: "auto"
	});

	const fileCmd = new GetObjectCommand({
		Bucket: "app-bucket",
		Key: request.key,
		Range: `bytes=${request.from}-${request.to}`
	})

	const file = await s3.send(fileCmd)

	if (file.Body instanceof ReadableStream) {
		const headers = new Headers();
		headers.append('Content-Type', 'video/*')
		headers.append('Authorization', `Bearer ${request.token}`)
		headers.append('Content-Length', `${request.chunksize}`)
		headers.append('Content-Range', `bytes ${request.from}-${request.to}/${request.total}`)

		console.log(`Starting to upload. bytes ${request.from}-${request.to}/${request.total}`)
		const upload = await fetch(request.url, { headers, method: 'PUT', body: file.Body })

		console.log(`Upload status: ${upload.status}`)
		if (upload.status === 200 || upload.status === 308) {
			return new Response(undefined, { status: upload.status })
		}

		console.log(await upload.clone().text())
		throw new Error("Upload failed")
	}

	return new Response("Not implemented", { status: 501 })
}
