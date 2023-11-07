import { z } from 'zod'

const schema = z.object({
    DATABASE_URL: z.string().min(1),
    RABBITMQ_URL: z.string().min(1),
    BUCKET_URL: z.string().min(1),
    BUCKET_KEY: z.string().min(1),
    BUCKET_SECRET: z.string().min(1),
    UPLOAD_URL: z.string().url(),
    GOOGLE_CLIENT_ID: z.string().min(1),
    GOOGLE_CLIENT_SECRET: z.string().min(1),
})

export const env = schema.parse(process.env)