import { z } from 'zod'

const schema = z.object({
    GRPC_RESOURCE_HOST: z.string(),
})

export const env = schema.parse(process.env)