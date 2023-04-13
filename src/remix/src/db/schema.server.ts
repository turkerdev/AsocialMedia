import { pgEnum, pgTable, text, timestamp, uuid } from 'drizzle-orm/pg-core';

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