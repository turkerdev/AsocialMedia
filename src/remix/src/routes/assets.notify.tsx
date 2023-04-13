import { type ActionArgs } from "@remix-run/node";
import { eq } from "drizzle-orm/expressions";
import { z } from "zod";
import { db } from "~/db/db.server";
import { assets } from "~/db/schema.server";

export async function action({ request }: ActionArgs) {
  const form = await request.formData();
  const body = await z
    .object({
      id: z.string(),
      message: z.string(),
    })
    .parseAsync(Object.fromEntries(form));

  if (body.message === "downloadComplete") {
    await db
      .update(assets)
      .set({ status: "in_bucket" })
      .where(eq(assets.id, body.id))
      .execute();
  }

  return null;
}
