import grpc, { Server, ServerCredentials } from '@grpc/grpc-js'
import { ShutterService, TTSService } from './protos/main';
import { Shutter_ServiceImpl } from './services/shutterService';
import { TTS_ServiceImpl } from './services/ttsService';

const server = new Server();

server.addService(ShutterService, Shutter_ServiceImpl)
server.addService(TTSService, TTS_ServiceImpl)

server.bindAsync('0.0.0.0:5555', ServerCredentials.createInsecure(), (err, port) => {
    if (err) throw err;
    console.log(`Server listening on port ${port}`)
    server.start();
})