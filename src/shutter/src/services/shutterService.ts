import { PutObjectCommand } from '@aws-sdk/client-s3';
import { chromium } from 'playwright';
import { ShutterServer } from '../protos/main';
import { s3 } from '../s3client';
import { randomUUID } from 'node:crypto'

export const Shutter_ServiceImpl: ShutterServer = {
    screenshot: async ({ request }, callback) => {
        console.log(request)
        const browser = await chromium.launch();
        const context = await browser.newContext();
        const page = await context.newPage();

        await page.goto(request.url);

        let text = undefined;
        if (request.textSelector) {
            text = await page.locator(request.textSelector).textContent() || undefined;
        }
        const screenshot = await page.locator(request.selector).screenshot();
        const key = randomUUID() + ".png";

        await s3.send(new PutObjectCommand({
            Bucket: "shutter",
            Key: key,
            Body: screenshot,
            ContentType: "image/png",
        }))

        await browser.close();
        await context.close();
        return callback(null, { key, text })
    }
}