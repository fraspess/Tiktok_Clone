# WebSocket API (SignalR)

**Hub URL:** `/hubs/chat`  
**Auth:** JWT passed via `accessTokenFactory` (appended as `?access_token=`)

---

## Client → Server (invoke)

### SendMessage
Відправити повідомлення в чат.
| Parameter | Type | Description |
|---|---|---|
| conversationId | Guid | Target conversation |
| content | string | Message text |

```js
connection.invoke("SendMessage", "conversation-guid", "message-text")
```

### MarkAsDelivered
Покажіть що повідомлення дошло до користувача
| Parameter | Type | Description |
|---|---|---|
| messageId | Guid | Delivered message |

```js
connection.invoke("MarkAsDelivered", "message-guid")
```

### MarkAsRead
Покажіть що користувач прочитав повідомлення.
| Parameter | Type | Description |
|---|---|---|
| messageId | Guid | Read message |

```js
connection.invoke("MarkAsRead", "message-guid")
```

---

## Server → Client (on)

### ReceivedMessage
Тригериться коли приходить вхідне повідомлення.
```js
connection.on("ReceivedMessage", (message) => {
    
})
```

### ReceivedPendingMessages
Тригериться коли користувач підключається і в нього є вхідні повідомлення які не були доставлені.
```js
connection.on("ReceivedPendingMessages", (messages) => {
    
})
```

---
