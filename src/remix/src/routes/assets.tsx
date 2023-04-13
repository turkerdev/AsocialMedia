import { useFetcher, useLoaderData } from "@remix-run/react";
import { Fragment } from "react";
import { db } from "~/db/db.server";
import { assets } from "~/db/schema.server";

export const loader = async () => {
  // TODO: Use streaming and pagination
  const allAssets = await db.select().from(assets);

  return {
    assets: allAssets,
  };
};

export default function Index() {
  const data = useLoaderData<typeof loader>();
  const fetcher = useFetcher();

  return (
    <div>
      <center>
        <fetcher.Form action="create" method="POST">
          <input type="url" name="url" placeholder="URL" required />
          <input type="text" name="description" placeholder="Description" />
          <button type="submit">Submit</button>
        </fetcher.Form>
      </center>
      <br />
      <center>
        {data.assets.map((asset) => (
          <Fragment key={asset.id}>
            <hr />
            <div>{JSON.stringify(asset)}</div>

            <fetcher.Form action="delete" method="POST">
              <input type="hidden" name="id" value={asset.id} />
              <button type="submit">Delete</button>
            </fetcher.Form>

            <fetcher.Form action="download" method="POST">
              <input type="hidden" name="id" value={asset.id} />
              <button type="submit">Download</button>
            </fetcher.Form>
          </Fragment>
        ))}
      </center>
    </div>
  );
}
