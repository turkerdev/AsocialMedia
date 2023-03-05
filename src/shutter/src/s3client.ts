import { BucketAlreadyOwnedByYou, CreateBucketCommand, S3Client } from "@aws-sdk/client-s3";
import { env } from "./env";

export const s3 = new S3Client({
    endpoint: env.BUCKET_URL,
    credentials: {
        accessKeyId: env.BUCKET_KEY,
        secretAccessKey: env.BUCKET_SECRET,
    },
    forcePathStyle: true,
    region: "auto"
});

s3.send(new CreateBucketCommand({ Bucket: "app-bucket" })).then(() => {
    console.log("Bucket 'app-bucket' created");
}).catch((err) => {
    if (err instanceof BucketAlreadyOwnedByYou) {
        console.log("Bucket 'app-bucket' already exists");
    }
    else {
        throw err
    }
});