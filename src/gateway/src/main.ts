import './env';
import { Server, ServerCredentials } from '@grpc/grpc-js'
import { resource_Impl } from './resource';
import { ResourceService } from './protos/main'

const server = new Server();

server.addService(ResourceService, resource_Impl)

server.bindAsync('0.0.0.0:6543', ServerCredentials.createInsecure(), (err, port) => {
    if (err) throw err;
    console.log(`Server listening on port ${port}`)
    server.start();
})