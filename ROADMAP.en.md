# 📅 Implementation Plan - XR Cowork Hub

## 📊 Status Report (Done vs To Do)

| Feature | Current Status | Comments |
| :--- | :--- | :--- |
| **Project Setup** | ✅ Done | Unity, Git, Packages installed. |
| **Firebase Auth** | 🟡 Partial | Basic `FirebaseInit` script (Anon login). No user state management. |
| **Agora Voice** | 🟡 Partial | Basic `AgoraInit` script (Join channel). No mute/group management. |
| **Passthrough** | ❌ To Do | Not configured. |
| **Panel System** | ❌ To Do | No spawn, movement, or sync logic. |
| **Multiplayer Sync** | ❌ To Do | No `ParticipantManager` or position sync. |
| **Focus Bubbles** | ❌ To Do | Core feature (Audio + Visual) missing. |
| **Private 3D Spaces** | ❌ To Do | "Premium" feature missing. |
| **Web Console** | ❌ To Do | Code nonexistent. |

---

## 👥 Proposed Distribution (2 Developers)

To maximize efficiency, I propose separating tasks into two axes: **System/Backend** (Dev A) and **XR Experience/Visuals** (Dev B).

### 🔧 Dev A: "System & Network Architect"
*Focus: Firebase, Agora, Data Synchronization, Pure Logic.*

1.  **Participant Management (Firestore)**: Create `ParticipantManager`. Handle Online/Offline state, store `voiceGroupId`.
2.  **Sync System (Network)**: Code logic to synchronize position/rotation of objects (Panels, Avatars) via Firestore.
3.  **Audio Logic (Agora)**: Implement channel/mute switching based on `voiceGroupId`.
4.  **Web Console (React)**: Create web interface to upload files to Firebase Storage.

### 🥽 Dev B: "XR & Interaction Specialist"
*Focus: Unity, UI, Spatial Interactions, Shaders.*

1.  **Passthrough & Player Setup**: Configure XR rig, Passthrough, and hands.
2.  **Panel System (Interaction)**: Create Panel Prefabs (Note, PDF), make them "grabbable" and resizable.
3.  **Bubble Visuals**: Create shaders for "Ghost" effect (transparency of other users) and visual bubble dome.
4.  **Private 3D Spaces**: Create 3D environment that appears when entering a private bubble.

---

## 🗓️ Roadmap by Phases

### Phase 1: Foundations (Days 1-2)
*   **Dev A**: `ParticipantManager` (see who is online) + Firestore structure (`rooms/`).
*   **Dev B**: Passthrough Scene Setup + Simple Avatar Prefab (sphere following head).
*   **Goal**: See each other in the room (even as spheres) and see connection logs.

### Phase 2: Panels & Sync (Days 3-5)
*   **Dev A**: `PanelManager` (Sync positions/content Firestore).
*   **Dev B**: Panel Prefabs (Unity UI) + Interaction (Grab/Resize).
*   **Goal**: Create a sticky note, write on it, and the other player sees it move and update.

### Phase 3: Focus Bubbles (Days 6-8)
*   **Dev A**: `voiceGroupId` logic + Agora dynamic Mute/Unmute.
*   **Dev B**: "Ghost" Shader + UI to create/join a bubble.
*   **Goal**: Click a button, get isolated audio/visually from others.

### Phase 4: Private 3D Spaces & Web (Days 9-10)
*   **Dev A**: Web Console (Upload PDF).
*   **Dev B**: 3D Environment "Private Room" + Spawn logic.
*   **Goal**: Complete "Premium" feature and real document import.

---

## ⚠️ Watch Points
*   **Firestore Costs**: Watch out for update loops that are too fast (position sync). Need to optimize (send position only if changed > 1cm, or max 10 times/sec).
*   **Merge Conflicts**: Work on different Prefabs as much as possible. Dev A on Manager Scripts, Dev B on UI/Visual Prefabs.
