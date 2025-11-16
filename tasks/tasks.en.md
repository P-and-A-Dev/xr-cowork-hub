# 🧩 **Full Tasklist (ordered by logical development)**

---

## **1️⃣ Setup & Foundations**

### 🔧 Project Setup

* [x] Create Git repo
* [x] Setup Unity + XR Interaction Toolkit
* [ ] Configure URP + scenes
* [ ] Integrate Firebase Unity SDK
* [ ] Integrate Agora SDK

### 🔥 Firebase Structure

* [x] Create Firebase project
* [ ] Add google-services.json to Unity
* [ ] Create Firestore collections: rooms, participants, panels
* [ ] Setup Anonymous Auth
* [ ] Setup Storage (PDFs, images)

### 🎤 Agora Voice

* [ ] Create Agora project
* [ ] Add APP ID to Unity
* [ ] Test join/leave audio channel

---

## **2️⃣ Basic User Features**

### 🧭 MR Passthrough

* [ ] Setup passthrough mode (Meta XR)
* [ ] Test simple cube placement
* [ ] Panel anchoring pipeline

### 🧊 Avatars / Presence

* [ ] ParticipantManager (Firestore listener)
* [ ] Simple avatar/orb per user
* [ ] Status: online / typing

---

## **3️⃣ Spatial Panels (Core)**

### 📦 Panel System

* [ ] PanelManager + Factory
* [ ] PanelBase.cs
* [ ] Transform Sync (position/rotation/scale)
* [ ] CRUD: create / update / delete panel

### 📒 Panel Types

* [ ] Multi-user Notepad
* [ ] To-Do list
* [ ] PDF viewer
* [ ] Image viewer
* [ ] Screenshot/snapshot panel
* [ ] Sticky notes
* [ ] Pomodoro
* [ ] Mini chat (optional)
* [ ] Mind map (optional)

---

## **4️⃣ Multi-user Sync**

### 🔄 Firestore Realtime Sync

* [ ] Sync panel data
* [ ] Sync panel transforms
* [ ] Sync participant fields
* [ ] Optimized batching/debounce

---

## **5️⃣ Voice Groups + Focus Bubbles**

### 🔊 VoiceGroupId Logic

* [ ] Add `voiceGroupId` field to participants
* [ ] Unity: mute/unmute based on group
* [ ] Ghost visual when user ≠ groupId

### 🔇 Focus Bubble UI/Flow

* [ ] Select 1+ users
* [ ] Generate newGroupId
* [ ] Update Firestore for selected users
* [ ] Manage bubble exit (groupId = 0)

---

## **6️⃣ Private 3D Bubble Spaces**

### 🏰 Private Environment

* [ ] BubbleSpace prefab (dome/cube/room)
* [ ] Spawn around user
* [ ] Hide the rest visually

### 🔊 Isolated Audio (reuse groupId)

* [ ] Agora: channel = groupId

### 📂 Bubble Panels

* [ ] Panels invisible to main room
* [ ] visibility = "bubble"
* [ ] bubbleGroupId = currentGroupId

### 🔄 Sync Bubble State

* [ ] Firestore field `inBubbleSpace`
* [ ] Auto spawn/destroy according to state

---

## **7️⃣ Web Console (React/Next.js)**

### 🌐 Uploads PC → XR

* [ ] Upload page
* [ ] Firebase Storage upload
* [ ] Client webhook: create panel in Firestore
* [ ] File list preview

---

## **8️⃣ Polish & UI**

### 🎨 UX & Comfort

* [ ] Better ghost shader
* [ ] Panel animations
* [ ] Better avatar visuals
* [ ] Hints & onboarding

### 🚀 Optimizations

* [ ] Firestore write batching
* [ ] Panel data caching
* [ ] Cleanup / limits

---

## **9️⃣ Tests & Release**

* [ ] Real 2-user test
* [ ] Panel stress test
* [ ] Private bubble spaces test
* [ ] Build Quest APK
* [ ] Upload to Meta Quest Developer Hub
