import { type LoaderArgs } from "@remix-run/node";
import { resourceClient } from "~/grpc.server";
import { type ListResourceResponse } from "~/protos/main";

export async function loader(_: LoaderArgs) {
  const resources = await new Promise<ListResourceResponse>(
    (resolve, reject) => {
      resourceClient.list({}, (err, response) => {
        if (err) {
          reject(err);
          return;
        }
        resolve(response);
      });
    }
  );
  return resources;
}
