import { S3Client } from "@aws-sdk/client-s3";
import { env } from "./env";

const s3 = new S3Client({
    endpoint: env.BUCKET_URL,
    credentials: {
        accessKeyId: env.BUCKET_KEY,
        secretAccessKey: env.BUCKET_SECRET,
    },
    forcePathStyle: true,
    region: "auto"
});

export default s3