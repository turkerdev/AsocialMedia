import { S3Client } from "@aws-sdk/client-s3";
import env from "./env";

const s3 = new S3Client({
    endpoint: env.STORAGE_URL,
    credentials: {
        accessKeyId: env.STORAGE_KEY,
        secretAccessKey: env.STORAGE_SECRET,
    },
    forcePathStyle: true,
    region: "auto"
});

export default s3