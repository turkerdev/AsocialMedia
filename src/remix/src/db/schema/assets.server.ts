import { pgTable, text, timestamp, uuid } from 'drizzle-orm/pg-core';

export const assets = pgTable('assets', {
    id: uuid('id').primaryKey().defaultRandom(),
    url: text('url').notNull(),
    description: text('description'),
    createdAt: timestamp('created_at').notNull().defaultNow(),
    // TODO: add other fields
    // starttime, endtime, etc
});