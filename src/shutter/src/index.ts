import { Server, ServerCredentials } from '@grpc/grpc-js'
import { PingService, PingServer } from './protos/main';

const server = new Server();

server.addService(PingService, {
    ping: (call, callback) => {
        if (call.request.message !== "Ping")
            return callback({ message: "Invalid message, expected 'Ping'", name: "Invalid" })
        return callback(null, { message: "Pong" })
    }
} as PingServer)

server.bindAsync('0.0.0.0:5555', ServerCredentials.createInsecure(), (err, port) => {
    if (err) throw err;
    console.log(`Server listening on port ${port}`)
    server.start();
})