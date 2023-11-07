import { BrowserService, BrowserServer } from '../protos/browser';
import { chromium } from 'playwright'
import { randomUUID } from 'crypto';
import { PutObjectCommand } from '@aws-sdk/client-s3'
import s3 from '../s3'

const implementation: BrowserServer = {
    screenshot: async ({ request }, callback) => {
        const browser = await chromium.launch();
        const page = await browser.newPage();

        await page.goto(request.url);

        const filename = randomUUID() + ".png";
        const screenshot = request.selector
            ? await page.locator(request.selector).screenshot()
            : await page.screenshot();

        const texts = await Promise.all(
            request.textSelectors.map(async ({ selectorName, selector }) => {
                const text = await page.locator(selector).textContent() || ""
                return {
                    selectorName,
                    text
                };
            })
        )

        const put = new PutObjectCommand({
            Bucket: "app",
            Key: filename,
            Body: screenshot,
            ContentType: "image/png",
        })

        await s3.send(put);

        callback(null, {
            filename,
            texts
        })

        await page.close();
    }
};

export default {
    service: BrowserService,
    implementation
}