import * as grpc from '@grpc/grpc-js'
import SvcBrowser from './services/browser'

const server = new grpc.Server();
server.addService(SvcBrowser.service, SvcBrowser.implementation)

server.bindAsync('0.0.0.0:4054',
    grpc.ServerCredentials.createInsecure(),
    (err, port) => {
        if (err) throw err;
        console.log(`Server listening on port ${port}`)
        server.start();
    }
)