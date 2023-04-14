import { auth } from "@googleapis/youtube";
import { redirect } from "@remix-run/node";
import { env } from "~/env.server";

export async function loader() {
  const oauth = new auth.OAuth2({
    clientId: env.GOOGLE_CLIENT_ID,
    clientSecret: env.GOOGLE_CLIENT_SECRET,
    redirectUri: env.GOOGLE_REDIRECT_URI,
  });

  const url = oauth.generateAuthUrl({
    scope: [
      "https://www.googleapis.com/auth/youtube.upload",
      "https://www.googleapis.com/auth/youtube",
    ],
    access_type: "offline",
    prompt: "consent",
  });

  return redirect(url);
}
