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
		chunksize: z.string(),
		from: z.string(),
		to: z.string(),
		total: z.string()
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
		headers.append('Content-Length', request.chunksize)
		headers.append('Content-Range', `bytes ${request.from}-${request.to}/${request.total}`)
		const upload = fetch(request.url, { headers, method: 'PUT', body: file.Body })
		e.waitUntil(upload.then(async res =>
			console.log({
				response: res.status,
				body: await res.text(),
				headers: Object.fromEntries(res.headers)
			})))

		return new Response("OK", { status: 200 })
	}

	return new Response("Not implemented", { status: 501 })
}
