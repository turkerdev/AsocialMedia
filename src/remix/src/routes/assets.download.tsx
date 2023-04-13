import { type ActionArgs } from "@remix-run/node";
import amqp from "amqplib";
import { eq } from "drizzle-orm/expressions";
import { z } from "zod";
import { db } from "~/db/db.server";
import { assets } from "~/db/schema.server";
import { env } from "~/env.server";

export async function action({ request }: ActionArgs) {
  const form = await request.formData();
  const body = await z
    .object({
      id: z.string(),
    })
    .parseAsync(Object.fromEntries(form));

  const [asset] = await db
    .update(assets)
    .set({ status: "in_use" })
    .where(eq(assets.id, body.id))
    .returning()
    .execute();

  const connection = await amqp.connect(env.RABBITMQ_URL);
  const channel = await connection.createChannel();

  const payload = {
    Id: asset.id,
    Url: asset.url,
  };

  channel.sendToQueue("video.download", Buffer.from(JSON.stringify(payload)));
  await channel.close();
  await connection.close();

  return null;
}
