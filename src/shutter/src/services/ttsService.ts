import { PutObjectCommand } from "@aws-sdk/client-s3";
import * as sdk from "microsoft-cognitiveservices-speech-sdk";
import { randomUUID } from "node:crypto";
import { env } from "../env";
import { TTSServer } from "../protos/main";
import { s3 } from "../s3client";

export const TTS_ServiceImpl: TTSServer = {
    speak: (call, callback) => {
        const speechConfig = sdk.SpeechConfig.fromSubscription(env.TTS_KEY, env.TTS_REGION);
        speechConfig.speechSynthesisVoiceName = call.request.voice;

        const synthesizer = new sdk.SpeechSynthesizer(speechConfig);

        synthesizer.speakTextAsync(call.request.text,
            async (res) => {
                synthesizer.close();
                if (res.reason === sdk.ResultReason.SynthesizingAudioCompleted) {
                    console.log("synthesis finished.");
                    const key = randomUUID() + ".wav";

                    await s3.send(new PutObjectCommand({
                        Bucket: "app-bucket",
                        Key: key,
                        Body: Buffer.from(res.audioData),
                        ContentType: "audio/wav",
                    }))
                    return callback(null, { key })
                }
                console.error("Speech synthesis cancelled, " + res.errorDetails +
                    "\nDid you set the speech resource key and region values?");
            },
            (err) => {
                synthesizer.close();
                console.trace("err - " + err);
            })
    }
}