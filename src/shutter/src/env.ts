import { z } from 'zod'

const schema = z.object({
    BUCKET_URL: z.string(),
    BUCKET_KEY: z.string(),
    BUCKET_SECRET: z.string(),
    TTS_KEY: z.string(),
    TTS_REGION: z.string(),
})

export const env = schema.parse(process.env)