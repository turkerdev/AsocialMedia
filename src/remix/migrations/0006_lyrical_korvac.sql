DO $$ BEGIN
 CREATE TYPE "social_platform" AS ENUM('youtube');
EXCEPTION
 WHEN duplicate_object THEN null;
END $$;

CREATE TABLE IF NOT EXISTS "social_accounts" (
	"platform" social_platform NOT NULL,
	"platform_id" text NOT NULL,
	"access_token" text NOT NULL,
	"refresh_token" text NOT NULL
);
ALTER TABLE "social_accounts" ADD CONSTRAINT "social_accounts_platform_platform_id" PRIMARY KEY("platform","platform_id");
