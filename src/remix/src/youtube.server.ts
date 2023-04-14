import { auth } from "@googleapis/oauth2";
import { youtube } from "@googleapis/youtube";
import { z } from "zod";
import { env } from "~/env.server";

export async function getChannelDetails(accessToken: string, refreshToken: string) {
    const oauth = new auth.OAuth2({
        clientId: env.GOOGLE_CLIENT_ID,
        clientSecret: env.GOOGLE_CLIENT_SECRET,
        redirectUri: env.GOOGLE_REDIRECT_URI,
    })

    oauth.setCredentials({
        access_token: accessToken,
        refresh_token: refreshToken,
    })

    await oauth.refreshAccessToken()

    const youtubeService = youtube({ version: 'v3', auth: oauth })

    const details = await youtubeService.channels.list({
        part: ["id", "snippet"],
        mine: true,
    })

    const schema = z.object({
        id: z.string(),
        snippet: z.object({
            title: z.string(),
            thumbnails: z.object({
                default: z.object({
                    url: z.string(),
                }),
            }),
        }),
    }).transform(val => ({
        id: val.id,
        name: val.snippet.title,
        image: val.snippet.thumbnails.default.url,
    }))

    return await schema.parseAsync(details.data.items?.[0])
}