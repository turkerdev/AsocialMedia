import { ApiClient } from '@twurple/api';
import { AppTokenAuthProvider } from '@twurple/auth';
import { env } from '~/env.server';

const authProvider = new AppTokenAuthProvider(
    env.TWITCH_CLIENT_ID,
    env.TWITCH_CLIENT_SECRET
);

const twitchApiClient = new ApiClient({ authProvider });

export const categories = {
    IRL: "509658",
    LoL: "21779",
    GTAV: "32982",
    TFT: "513143",
    VALORANT: "516575",
    APEX: "511224",
} as const;

export const mostWatchedClipsToday = (category: string) => mostWatchedClips(category, daysAgo(1))

function daysAgo(day: number) {
    const date = new Date();
    date.setDate(date.getDate() - day);
    return date;
}

async function mostWatchedClips(category: string, fromDate: Date, toDate = new Date()) {
    const from = fromDate.toISOString();
    const to = toDate.toISOString();

    const clips = await twitchApiClient.clips.getClipsForGame(category, {
        startDate: from,
        endDate: to,
    })
    return clips.data;
}