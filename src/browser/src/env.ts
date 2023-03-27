import { z } from 'zod'

const schema = z.object({
    STORAGE_URL: z.string(),
    STORAGE_KEY: z.string(),
    STORAGE_SECRET: z.string(),
})

const env = schema.parse(process.env)

export default env