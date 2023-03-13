import { credentials } from "@grpc/grpc-js";
import { env } from "~/env.server";
import { ResourceClient } from "~/protos/main";

export const resourceClient = new ResourceClient(
    env.GRPC_RESOURCE_HOST,
    credentials.createInsecure()
);