import { Link, useLoaderData } from "@remix-run/react";
import { db } from "~/db/db.server";
import { social_accounts } from "~/db/schema.server";

export async function loader() {
  const accounts = await db
    .select({
      platform: social_accounts.platform,
      platformId: social_accounts.platformId,
    })
    .from(social_accounts)
    .execute();

  return { accounts };
}

export default function Component() {
  const data = useLoaderData<typeof loader>();

  return (
    <>
      <h1>YouTube</h1>
      <Link to="/oauth/youtube">Login</Link>
      {data.accounts
        .filter((x) => x.platform === "youtube")
        .map((account) => (
          <>
            <p>{account.platformId}</p>
          </>
        ))}
      <hr />
    </>
  );
}
