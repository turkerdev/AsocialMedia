import { type ActionArgs } from "@remix-run/node";
import { db } from "~/db/db.server";
import { assets } from "~/db/schema/assets.server";

export async function action({ request }: ActionArgs) {
  const form = await request.formData();
  const body = Object.fromEntries(form);

  // TODO: Add validation

  const [newAsset] = await db
    .insert(assets)
    .values({
      url: body["url"].toString(),
      description: body["description"].toString(),
    })
    .returning({ id: assets.id })
    .execute();

  return newAsset;
}
