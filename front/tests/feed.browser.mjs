// PLAYWRIGHT_MODULE=/path/to/playwright/index.mjs node tests/feed.browser.mjs
import assert from 'node:assert/strict';
const {chromium} = await import(process.env.PLAYWRIGHT_MODULE || 'playwright');
const browser = await chromium.launch({headless: true});
const baseURL = process.env.FEED_TEST_URL || 'http://127.0.0.1:5173';
const pixel = Buffer.from('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/x8AAwMCAO+aX1sAAAAASUVORK5CYII=', 'base64');
const avatar = version => ({small: `/avatars/${version}.png`, medium: `/avatars/${version}.png`, large: `/avatars/${version}.png`});
const profile = {id: 'user-1', username: 'tester', description: '', isOwnProfile: true, followersCount: 0, followingCount: 0, avatar: avatar('old')};
const videos = [1, 2, 3].map(n => ({id: `video-${n}`, videoUrl: '/fake.mp4', description: 'A video', author: {id: n === 2 ? 'other' : profile.id, username: n === 2 ? 'other' : profile.username, avatar: avatar(n === 2 ? 'other' : 'old')}, likeCount: 0, commentsCount: 0, favoriteCount: 0}));
try {
    const context = await browser.newContext({viewport: {width: 390, height: 667}, hasTouch: true, isMobile: true, locale: 'en'});
    const page = await context.newPage();
    let updated = false;
    await page.route('**/avatars/*.png', route => route.fulfill({contentType: 'image/png', body: pixel}));
    await page.route('**/api/**', async route => {
        const url = new URL(route.request().url());
        let data = null;
        if (url.pathname.endsWith('/refresh')) data = {accessToken: 'test-token'};
        else if (url.pathname === '/api/users' && route.request().method() === 'PATCH') updated = true;
        else if (['/api/users/me', '/api/users/tester'].includes(url.pathname)) data = {...profile, avatar: avatar(updated ? 'new' : 'old')};
        else if (url.pathname.includes('/videos/') || url.pathname.includes('/notifications')) data = {items: videos, metadata: {hasNext: false}};
        await route.fulfill({json: {data}});
    });
    await page.goto(baseURL);
    await page.locator('.video-card').first().waitFor();
    await page.locator('.video-actions a').first().click();
    await page.getByRole('button', {name: 'Edit profile', exact: true}).click();
    await page.locator('input[type=file]').setInputFiles({name: 'avatar.png', mimeType: 'image/png', buffer: pixel});
    await page.getByRole('button', {name: 'Save', exact: true}).click();
    await page.getByRole('dialog').waitFor({state: 'hidden'});
    assert(updated, 'avatar upload was submitted');
    await page.waitForFunction(() => document.querySelector('button[aria-haspopup=menu] img')?.getAttribute('src') === '/avatars/new.png');
    await page.getByRole('link', {name: /Back to feed/i}).click();
    await page.waitForFunction(() => document.querySelector('.video-actions img')?.getAttribute('src') === '/avatars/new.png');
    assert.equal(await page.locator('.video-actions img').nth(1).getAttribute('src'), '/avatars/other.png');
    console.log('PASS avatar upload updates topbar and cached video author; other author is unchanged');

    for (const [width, height] of [[320, 568], [390, 667], [568, 320], [667, 375], [844, 390], [1024, 600], [1440, 900]]) {
        await page.setViewportSize({width, height});
        await page.waitForTimeout(150);
        const feed = page.locator('.video-card').first().locator('..');
        await feed.evaluate(el => el.scrollTo({top: 0, behavior: 'instant'}));
        await page.waitForTimeout(150);
        const bounds = await page.locator('.video-card').first().boundingBox();
        const actions = await page.locator('.video-actions').first().boundingBox();
        assert(actions.y >= bounds.y && actions.y + actions.height <= bounds.y + bounds.height, `actions fit at ${width}x${height}`);
        assert(actions.x + actions.width <= width, 'actions fit horizontally');
        assert.equal(await page.evaluate(() => document.documentElement.scrollWidth > innerWidth), false);
        // A real touch gesture beginning over the action column must advance the feed.
        const cdp = await context.newCDPSession(page);
        const x = actions.x + actions.width / 2;
        const start = Math.min(actions.y + actions.height - 20, bounds.height - 20);
        await cdp.send('Input.dispatchTouchEvent', {type: 'touchStart', touchPoints: [{x, y: start}]});
        for (let i = 1; i <= 8; i++) {
            await cdp.send('Input.dispatchTouchEvent', {type: 'touchMove', touchPoints: [{x, y: start - i * Math.min(bounds.height * 0.75 / 8, (start - 10) / 8)}]});
            await page.waitForTimeout(25);
        }
        await cdp.send('Input.dispatchTouchEvent', {type: 'touchEnd', touchPoints: []});
        await page.waitForTimeout(800);
        assert(await feed.evaluate(el => el.scrollTop) > 0, `swipe over actions advances feed at ${width}x${height}`);
        await cdp.detach();
        console.log(`PASS layout and touch scrolling ${width}x${height}`);
    }
} finally {
    await browser.close();
}
