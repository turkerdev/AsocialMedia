const { drizzle } = require("drizzle-orm/node-postgres/index");
const { migrate } = require("drizzle-orm/node-postgres/migrator");
const { Pool } = require("pg");

run();

async function run() {
  const url = process.env.DATABASE_URL;
  const client = new Pool({
    connectionString: url,
  });

  const db = drizzle(client);

  await migrate(db, { migrationsFolder: "migrations" });
  console.log("Migrations complete");
}
