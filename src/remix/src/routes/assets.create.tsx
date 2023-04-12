import { type ActionArgs } from "@remix-run/node";
import { z } from "zod";
import { db } from "~/db/db.server";
import { assets } from "~/db/schema.server";

export async function action({ request }: ActionArgs) {
  const form = await request.formData();
  const body = await z
    .object({
      url: z.string().url(),
      description: z.string().nullable(),
    })
    .parseAsync(Object.fromEntries(form));

  const [newAsset] = await db
    .insert(assets)
    .values({
      url: body.url,
      description: body.description,
    })
    .returning({ id: assets.id })
    .execute();

  return newAsset;
}
