import { GetObjectCommand, S3Client } from '@aws-sdk/client-s3'
import { z } from 'zod'

addEventListener("fetch", (event) => {
	event.respondWith(handleRequest(event.request));
})

async function handleRequest(request: Request): Promise<Response> {
	const { pathname } = new URL(request.url);
	if (pathname === "/upload") {
		return await handleUpload(request);
	}
	return new Response("Not found", { status: 404 });
}

async function handleUpload(request: Request): Promise<Response> {
	const env = await z.object({
		BUCKET_URL: z.string(),
		BUCKET_KEY: z.string(),
		BUCKET_SECRET: z.string()
	}).parseAsync({
		BUCKET_URL,
		BUCKET_KEY,
		BUCKET_SECRET
	})

	const json = await request.json();
	const { key, token, url } = await z.object({
		key: z.string(),
		token: z.string(),
		url: z.string()
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

	const command = new GetObjectCommand({
		Bucket: "app-bucket",
		Key: key
	})
	const file = await s3.send(command)

	if (!file.Body) {
		return new Response("No file found", { status: 404 })
	}

	const headers = new Headers();
	headers.append('Content-Type', file.ContentType || 'video/mp4')
	headers.append('Authorization', `Bearer ${token}`)

	await fetch(url,
		{
			headers,
			method: 'PUT',
			body: file.Body.transformToWebStream()
		})

	return new Response("OK", { status: 200 })
}
