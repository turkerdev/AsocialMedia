import { pgEnum, pgTable, primaryKey, text, timestamp, uuid } from 'drizzle-orm/pg-core';

export const asset_status = pgEnum('asset_status', ['in_bucket', 'not_in_bucket', 'in_use'])

export const assets = pgTable('assets', {
    id: uuid('id').primaryKey().defaultRandom(),
    url: text('url').notNull(),
    description: text('description'),
    createdAt: timestamp('created_at').notNull().defaultNow(),
    status: asset_status('status').notNull().default('not_in_bucket'),
    // TODO: add other fields
    // starttime, endtime, etc
});

export const social_platform = pgEnum('social_platform', ['youtube'])

export const social_accounts = pgTable('social_accounts', {
    platform: social_platform('platform').notNull(),
    platformId: text('platform_id').notNull(),
    accessToken: text('access_token').notNull(),
    refreshToken: text('refresh_token').notNull(),
}, (t) => {
    return {
        id: primaryKey(t.platform, t.platformId),
    }
})