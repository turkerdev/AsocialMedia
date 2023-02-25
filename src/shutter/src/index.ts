import { Server, ServerCredentials } from '@grpc/grpc-js'
import { ShutterService } from './protos/main';
import { ShutterServiceImpl } from './services/shutterService';

const server = new Server();

server.addService(ShutterService, ShutterServiceImpl)

server.bindAsync('0.0.0.0:5555', ServerCredentials.createInsecure(), (err, port) => {
    if (err) throw err;
    console.log(`Server listening on port ${port}`)
    server.start();
})