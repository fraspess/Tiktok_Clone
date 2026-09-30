// Run with the Vite server available and PLAYWRIGHT_MODULE pointing to playwright/index.mjs.
import assert from "node:assert/strict";
const {chromium} = await import(process.env.PLAYWRIGHT_MODULE || "playwright");
const browser = await chromium.launch({headless: true});
const baseUrl = process.env.FEATURE_TEST_URL || "http://127.0.0.1:5175";
const token = `e30.${Buffer.from(JSON.stringify({sub: "viewer", exp: 4102444800})).toString("base64url")}.test`;
const viewer = {id: "viewer", username: "viewer", avatar: ""};
const actor = {id: "creator", username: "creator", avatar: ""};
const envelope = data => ({data, success: true});
const paged = (items, page = 1, total = items.length) => ({items, metadata: {
    currentPage: page, pageSize: 20, totalCount: total, totalPages: Math.ceil(total / 20),
    hasNext: page * 20 < total, hasPrevious: page > 1,
}});
async function waitUntil(predicate) {
    for (let attempt = 0; attempt < 100; attempt++) {
        if (predicate()) return;
        await new Promise(resolve => setTimeout(resolve, 50));
    }
    assert(predicate(), "Expected async state was not reached");
}

try {
    for (const viewport of [{width: 1280, height: 800}, {width: 390, height: 844}]) {
        const context = await browser.newContext({viewport, locale: "en-US"});
        await context.addInitScript(() => localStorage.setItem("i18nextLng", "en"));
        const page = await context.newPage();
        const errors = [];
        page.on("pageerror", error => errors.push(error.message));
        let items = Array.from({length: 25}, (_, i) => ({
            id: `notification-${i}`, recipientId: viewer.id, actorId: actor.id, actor,
            type: i < 2 ? "NewDMMessage" : "YourVideoLiked", conversationId: `conversation-${i}`,
            videoShortId: "video-test", resourceId: `resource-${i}`, readAt: null,
            createdAt: new Date(Date.now() - i * 60000).toISOString(),
        }));
        let failList = false;
        let notificationSocket;
        const requestedPages = [];
        const openedConversations = [];
        await page.routeWebSocket("**/hubs/**", socket => {
            socket.onMessage(message => {
                for (const part of message.toString().split("\x1e").filter(Boolean)) {
                    const payload = JSON.parse(part);
                    if (payload.protocol) {
                        socket.send("{}\x1e");
                        if (socket.url().includes("/notification")) notificationSocket = socket;
                    }
                }
            });
        });
        await page.route("**/hubs/**/negotiate?*", route => route.fulfill({json: {
            negotiateVersion: 1, connectionId: "test", connectionToken: "test",
            availableTransports: [{transport: "WebSockets", transferFormats: ["Text"]}],
        }}));
        await page.route("**/api/**", async route => {
            const url = new URL(route.request().url());
            const path = url.pathname;
            let data;
            if (path.endsWith("/users/refresh")) data = {accessToken: token};
            else if (path.endsWith("/users/me")) data = viewer;
            else if (path.endsWith("/notifications/unread-count")) data = items.filter(n => !n.readAt).length;
            else if (path.endsWith("/notifications/read-all")) {
                items = items.map(n => ({...n, readAt: new Date().toISOString()}));
                return route.fulfill({status: 204});
            } else if (/\/notifications\/[^/]+\/read$/.test(path)) {
                const id = path.split("/").at(-2);
                items = items.map(n => n.id === id ? {...n, readAt: new Date().toISOString()} : n);
                return route.fulfill({status: 204});
            } else if (path.endsWith("/notifications")) {
                if (failList) return route.fulfill({status: 500, json: {message: "Test failure"}});
                const number = Number(url.searchParams.get("pageNumber"));
                requestedPages.push(number);
                data = paged(items.slice((number - 1) * 20, number * 20), number, items.length);
            } else if (/\/conversations\/conversation-/.test(path)) {
                const id = path.split("/").at(-1);
                data = {id, participants: [viewer, actor]};
            } else if (path.endsWith("/conversations/messages")) {
                openedConversations.push(url.searchParams.get("conversationId"));
                data = paged([]);
            } else if (path.endsWith("/conversations")) {
                data = paged([{id: "conversation-0", participants: [viewer, actor]}]);
            } else data = paged([]);
            return route.fulfill({json: envelope(data)});
        });
        await page.goto(`${baseUrl}/search`);
        const bell = page.getByRole("button", {name: /^Notifications/});
        await page.getByRole("button", {name: "Notifications (25 unread)", exact: true}).waitFor();
        await bell.click();
        const panel = page.getByRole("dialog", {name: "Notifications"});
        await panel.getByText("sent you a message").first().waitFor();
        const bounds = await panel.boundingBox();
        assert.equal(Math.round(bounds.height), 440);
        assert.equal(Math.round(bounds.width), 360);
        assert(bounds.x >= 0 && bounds.x + bounds.width <= viewport.width);
        const scroll = page.getByTestId("notification-scroll");
        assert(await scroll.evaluate(el => el.scrollHeight > el.clientHeight));
        await scroll.evaluate(el => {el.scrollTop = el.scrollHeight;});
        await page.waitForFunction(() => document.querySelectorAll('[data-testid="notification-scroll"] li').length === 25);
        assert(requestedPages.includes(2));
        assert.equal(Math.round((await panel.boundingBox()).height), 440);
        await page.screenshot({path: `/tmp/notifications-${viewport.width}.png`});
        await page.keyboard.press("Escape");
        await panel.waitFor({state: "hidden"});
        await page.waitForFunction(() => document.activeElement?.getAttribute("aria-label")?.startsWith("Notifications"));

        await bell.click();
        await panel.getByRole("button", {name: "Mark all read"}).click();
        await page.waitForFunction(() => document.querySelector('button[aria-label="Notifications"]') !== null);
        assert(items.every(n => n.readAt));
        await page.keyboard.press("Escape");

        await waitUntil(() => notificationSocket);
        items[0].readAt = null;
        notificationSocket.send(JSON.stringify({type: 1, target: "ReceiveNotification", arguments: [items[0]]}) + "\x1e");
        await page.getByRole("button", {name: "Notifications (1 unread)", exact: true}).waitFor();
        await bell.click();
        await panel.getByRole("button").filter({hasText: "sent you a message"}).first().click();
        await page.waitForURL("**/messages");
        await panel.waitFor({state: "hidden"});
        await waitUntil(() => openedConversations.includes("conversation-0"));
        assert(items[0].readAt);
        await bell.click();
        await panel.getByRole("button").filter({hasText: "sent you a message"}).nth(1).click();
        await panel.waitFor({state: "hidden"});
        await waitUntil(() => openedConversations.includes("conversation-1"));

        items = [];
        await bell.click();
        await panel.getByText(/You're all caught up/).waitFor();
        await page.keyboard.press("Escape");
        failList = true;
        await bell.click();
        await panel.getByRole("alert").waitFor();
        failList = false;
        await panel.getByRole("button", {name: "Try again"}).click();
        await panel.getByText(/You're all caught up/).waitFor();
        assert.deepEqual(errors, []);
        console.log(`PASS ${viewport.width}px: fixed dropdown, scroll paging, read state, live event, conversation navigation, empty/error/retry`);
        await context.close();
    }
} finally {
    await browser.close();
}
