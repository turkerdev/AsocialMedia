import { auth } from "@googleapis/oauth2";
import { redirect, type LoaderArgs } from "@remix-run/node";
import { z } from "zod";
import { db } from "~/db/db.server";
import { social_accounts } from "~/db/schema.server";
import { env } from "~/env.server";
import { getChannelDetails } from "~/youtube.server";

export async function loader({ request }: LoaderArgs) {
  const url = new URL(request.url);
  const { code } = await z
    .object({
      code: z.string(),
    })
    .parseAsync(Object.fromEntries(url.searchParams));

  const oauth = new auth.OAuth2({
    clientId: env.GOOGLE_CLIENT_ID,
    clientSecret: env.GOOGLE_CLIENT_SECRET,
    redirectUri: env.GOOGLE_REDIRECT_URI,
  });

  const { tokens } = await oauth.getToken(code);

  const { accessToken, refreshToken } = await z
    .object({
      accessToken: z.string(),
      refreshToken: z.string(),
    })
    .parseAsync({
      accessToken: tokens.access_token,
      refreshToken: tokens.refresh_token,
    });

  const { id } = await getChannelDetails(accessToken, refreshToken);

  await db
    .insert(social_accounts)
    .values({
      platform: "youtube",
      platformId: id,
      accessToken: accessToken,
      refreshToken: refreshToken,
    })
    .onConflictDoUpdate({
      target: [social_accounts.platform, social_accounts.platformId],
      set: {
        accessToken: accessToken,
        refreshToken: refreshToken,
      },
    })
    .execute();

  return redirect("/accounts");
}
