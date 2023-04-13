DO $$ BEGIN
 CREATE TYPE "asset_status" AS ENUM('in_bucket', 'not_in_bucket', 'in_use');
EXCEPTION
 WHEN duplicate_object THEN null;
END $$;

ALTER TABLE "assets" ADD COLUMN "status" "asset_status" DEFAULT 'not_in_bucket' NOT NULL;