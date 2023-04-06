import { type ActionArgs } from "@remix-run/node";
import { eq } from "drizzle-orm/expressions";
import { db } from "~/db/db.server";
import { assets } from "~/db/schema/assets.server";

export async function action({ request }: ActionArgs) {
  const form = await request.formData();
  const body = Object.fromEntries(form);

  // TODO: Add validation

  await db.delete(assets).where(eq(assets.id, body["id"].toString())).execute();

  return null;
}
