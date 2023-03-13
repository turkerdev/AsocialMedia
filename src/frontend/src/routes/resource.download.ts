import { type ActionArgs } from "@remix-run/node";
import { z } from "zod";
import { resourceClient } from "~/grpc.server";
import { type DownloadResourceResponse } from "~/protos/main";

export async function action({ request }: ActionArgs) {
  const form = await request.formData();
  const { id } = await z.object({
    id: z.string().uuid(),
  }).parseAsync(Object.fromEntries(form))

  console.log("downloading resource", id);

  await new Promise<DownloadResourceResponse>(
    (resolve, reject) => {
      resourceClient.download({ id: id.toString() }, (err, response) => {
        if (err) {
          reject(err);
          return;
        }
        resolve(response);
      });
    }
  );

  return null;
}
