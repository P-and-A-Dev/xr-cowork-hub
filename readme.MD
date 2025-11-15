# 🧭 XR Cowork Hub

### **Mixed Reality Coworking Platform – Meta Quest / Unity / Firebase / Agora**

XR Cowork Hub is a **Mixed Reality coworking experience** for Meta Quest where multiple users can work together in the same real room using Passthrough, spatial XR panels, collaborative tools, and instant private “Focus Bubbles” powered by real-time voice isolation.

Designed for the **Meta Start Developer Competition**, the project focuses on:

* 🧠 Collaboration
* 🔊 Real-time voice groups
* 📄 Shared panels (Notes, To-Do, PDFs, Images, Timer / Pomodoro, Sticky Post-its, Web Bookmark)
* 🏗 Zero-backend architecture (100% serverless)
* ⚡ Smooth MR user experience

---

# 📦 Features

### 🧭 **Passthrough MR Room**

* See your real environment using Quest Passthrough.
* XR panels anchored in real space.
* Persistent workspace layout.

### 📂 **XR Spatial Panels**

* Kanban Board
* Sticky Notes Wall
* NotePad (multi-user text editor)
* PDF Viewer
* Pomodoro Timer
* To-Do List
* Screen-share Snapshots
* AI Summary Panel
* Image Viewer
* Whiteboard Blocks
* Shared Calendar
* Website Preview Panel
* Bookmark List
* World Clock Panel
* Mind Map Panel
* Mini Chat Panel

`Panels are movable / resizable / rotatable`

### 🧑‍🤝‍🧑 **Multi-user Real-Time Collaboration**

* Firestore sync for:

  * Panels
  * Panel edits
  * Panel transforms
  * Room participants
* Live presence indicators:

  * Online
  * Typing
  * Focus bubble group

### 🔇 **Focus Bubbles (Private Audio + Visual Isolation)**

* Create private subgroups instantly
* No room change required
* Others become:

  * Muted
  * Ghost (faded / desaturated)
* Ultra-simple implementation using `voiceGroupId`

### 🗄 **File Upload via Web Console**

* Upload PDF / image files from PC browser
* Stored in Firebase Storage
* Instantly appear as panels in MR room

---

# 🧱 Tech Stack

## 🎮 **Client – Unity**

* Unity 6000 / 2023 LTS / 2022 LTS
* **URP (Universal Render Pipeline)** – recommended
* Meta XR SDK / OpenXR
* XR Interaction Toolkit (2.x)
* C# Modular architecture:

  * `RoomManager`
  * `PanelManager`
  * `ParticipantManager`
  * `FocusBubbleManager`
  * `FirestoreService`
  * `AgoraVoiceManager`
* World-space UI (TextMeshPro)
* Smooth interpolation for avatars

## 🔥 **Backend (Serverless – Firebase)**

* Firestore (real-time state)
* Storage (PDFs, images, snapshots)
* Realtime Database (optional):

  * High-frequency avatar movement (10–20 FPS)
* Firebase Hosting (optional UI tools)
* Firebase Auth (Anonymous)

## 🎤 **Voice**

* **Agora.io** (recommended for hackathon)

  * Multi-channel audio
  * Switch voice groups based on Firestore state
* Alternative: Vivox (UGS)

## 🌐 **Web Console**

* React or Next.js
* Firebase Web SDK
* Upload PDF / images
* Manage file metadata

---

# 🗃 Firestore Data Model (Final)

## **rooms**

`rooms/{roomId}`

```jsonc
{
  "name": "Main Room",
  "createdAt": "timestamp"
}
```

---

## **participants**

`rooms/{roomId}/participants/{userId}`

```jsonc
{
  "displayName": "Antonin",
  "voiceGroupId": 0,          // 0 = global, >0 = private bubble
  "isOnline": true,
  "lastSeen": "timestamp",
  
  // Optional UX:
  "typing": false,
  "avatarColor": "#87CEFA",
  "device": "quest3"
}
```

---

## **panels**

`rooms/{roomId}/panels/{panelId}`

```jsonc
{
  "type": "note",             // note | tasks | pdf | image | snapshot
  "title": "Pitch Notes",
  "content": "text...",
  
  "fileId": "file_abc",
  "fileUrl": "https://...",

  "createdBy": "user_123",
  "updatedAt": "timestamp",

  "position": { "x": 1.2, "y": 1.5, "z": -0.8 },
  "rotation": { "x": 0, "y": 45, "z": 0 },

  "visibility": "room"        // room | owner | bubble
}
```

---

## **files**

`files/{fileId}`

```jsonc
{
  "roomId": "main-room",
  "type": "pdf",
  "name": "Slides.pdf",
  "url": "https://...",
  "uploadedBy": "antonin",
  "createdAt": "timestamp"
}
```

---

## **realtime presence**

`presence/{roomId}/{userId}`

```json
{
  "pos": { "x": 0, "y": 1.6, "z": 0 },
  "rot": { "x": 0, "y": 90, "z": 0 },
  "timestamp": 1731680892000
}
```

---

# 🎤 Focus Bubbles – Technical Design

### ✔ Only one field: `voiceGroupId`

No extra `bubbles` collection is needed.

### Creating a bubble:

```csharp
long newGroupId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
await participantRef.UpdateAsync("voiceGroupId", newGroupId);
```

### Leaving bubble:

```csharp
await participantRef.UpdateAsync("voiceGroupId", 0);
```

### Unity logic:

```csharp
bool SameGroup(Participant a, Participant b)
    => a.voiceGroupId == b.voiceGroupId;

avatar.SetGhost(!SameGroup(other, me));
audio.Mute(other)   if (!SameGroup(other, me));
audio.Unmute(other) if ( SameGroup(other, me));
```

---

# 🧩 Unity Architecture

```
/Assets
  /Scripts
    /Core
    /Managers
      RoomManager.cs
      PanelManager.cs
      ParticipantManager.cs
      FocusBubbleManager.cs
      FirestoreService.cs
      AgoraVoiceManager.cs
    /UI
    /XR
  /Prefabs
    /Panels
    /Avatars
  /Scenes
    MainScene.unity
```

---

# 🚀 Getting Started (Developers)

### 1. Clone the repository

```bash
git clone https://github.com/P-and-A-Dev/xr-cowork-hub.git
```

### 2. Open Unity project

Open `unity-client/` in Unity Hub.

### 3. Install required packages

* XR Interaction Toolkit
* OpenXR
* Meta XR SDK
* Firebase Unity SDK
* Agora Unity SDK

### 4. Configure Firebase

* Add `google-services.json` to Unity project
* Enable Firestore + Storage

### 5. Configure Agora

* Add SDK
* Insert APP ID in `AgoraVoiceManager.cs`

### 6. Run Scene

Open `MainScene.unity`
Press **Play** (works without a headset thanks to XR Device Simulator).

---

# 🔮 Roadmap

### **MVP (Hackathon)**

* Notes / To-Do / PDF panels / Sticky Notes Wall
* Focus bubbles
* Multi-user sync
* Audio routing
* Ghost visuals
* Passthrough room
* Web console for file uploads

### **Future**

* Multi-room support
* AI assistant on panels
* Spatial anchors saving
* Live screen sharing
* Gesture-based interactions

---

# 📄 License

Licensed under the Apache License, Version 2.0

---

# 🧑‍💻 Authors

**Antonin Do Souto**

**Pietro Giacomelli**
