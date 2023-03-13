import { type ActionArgs } from "@remix-run/node";
import { z } from "zod";
import { resourceClient } from "~/grpc.server";
import { type CreateResourceResponse } from "~/protos/main";

export async function action({ request }: ActionArgs) {
  const form = await request.formData();
  const { url } = await z.object({
    url: z.string().url(),
  }).parseAsync(Object.fromEntries(form))

  const resource = await new Promise<CreateResourceResponse>(
    (resolve, reject) => {
      resourceClient.create(
        { url },
        (err, response) => {
          if (err) {
            reject(err);
            return;
          }
          resolve(response);
        }
      );
    }
  );
  console.log("new resource", resource);
  return resource;
}
