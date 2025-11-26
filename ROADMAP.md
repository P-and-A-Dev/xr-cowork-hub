# 📅 Plan d'Implémentation - XR Cowork Hub

## 📊 État des Lieux (Ce qui est fait vs Reste à faire)

| Feature | État Actuel | Commentaires |
| :--- | :--- | :--- |
| **Setup Projet** | ✅ Fait | Unity, Git, Packages installés. |
| **Auth Firebase** | 🟡 Partiel | Script `FirebaseInit` basique (Anon login). Pas de gestion d'état utilisateur. |
| **Voice Agora** | 🟡 Partiel | Script `AgoraInit` basique (Join channel). Pas de gestion de mute/groupes. |
| **Passthrough** | ❌ À faire | Pas configuré. |
| **Système de Panneaux** | ❌ À faire | Aucune logique de spawn, déplacement ou sync. |
| **Sync Multi-joueurs** | ❌ À faire | Pas de `ParticipantManager` ni de sync de position. |
| **Focus Bubbles** | ❌ À faire | Cœur du projet (Audio + Visuel) manquant. |
| **Private 3D Spaces** | ❌ À faire | Feature "Premium" manquante. |
| **Web Console** | ❌ À faire | Code inexistant. |

---

## 👥 Proposition de Répartition (2 Développeurs)

Pour maximiser l'efficacité, je propose de séparer les tâches en deux axes : **Système/Backend** (Dev A) et **Expérience XR/Visuel** (Dev B).

### 🔧 Dev A : "Architecte Système & Réseau"
*Focus : Firebase, Agora, Synchronisation des données, Logique pure.*

1.  **Gestion des Participants (Firestore)** : Créer `ParticipantManager`. Gérer l'état Online/Offline, stocker `voiceGroupId`.
2.  **Système de Sync (Network)** : Coder la logique pour synchroniser la position/rotation des objets (Panneaux, Avatars) via Firestore.
3.  **Logique Audio (Agora)** : Implémenter le changement de channel/mute basé sur le `voiceGroupId`.
4.  **Web Console (React)** : Créer l'interface web pour uploader des fichiers vers Firebase Storage.

### 🥽 Dev B : "Spécialiste XR & Interactions"
*Focus : Unity, UI, Interactions Spatiales, Shaders.*

1.  **Setup Passthrough & Player** : Configurer le rig XR, le Passthrough, et les mains.
2.  **Système de Panneaux (Interaction)** : Créer les Prefabs des panneaux (Note, PDF), les rendre "grabables" et redimensionnables.
3.  **Visuels des Bulles** : Créer les shaders pour l'effet "Ghost" (transparence des autres utilisateurs) et le dôme visuel des bulles.
4.  **Private 3D Spaces** : Créer l'environnement 3D qui apparaît quand on entre dans une bulle privée.

---

## 🗓️ Roadmap par Phases

### Phase 1 : Les Fondations (Jours 1-2)
*   **Dev A** : `ParticipantManager` (voir qui est en ligne) + Structure Firestore (`rooms/`).
*   **Dev B** : Setup Scène Passthrough + Prefab Avatar simple (sphère qui suit la tête).
*   **Objectif** : Se voir dans la room (même sous forme de sphère) et voir les logs de connexion.

### Phase 2 : Panneaux & Sync (Jours 3-5)
*   **Dev A** : `PanelManager` (Sync des positions/contenu Firestore).
*   **Dev B** : Prefabs des Panneaux (UI Unity) + Interaction (Grab/Resize).
*   **Objectif** : Créer un post-it, écrire dessus, et l'autre joueur le voit bouger et se mettre à jour.

### Phase 3 : Focus Bubbles (Jours 6-8)
*   **Dev A** : Logique `voiceGroupId` + Agora Mute/Unmute dynamique.
*   **Dev B** : Shader "Ghost" + UI pour créer/rejoindre une bulle.
*   **Objectif** : On clique sur un bouton, on est isolé audio/visuel des autres.

### Phase 4 : Private 3D Spaces & Web (Jours 9-10)
*   **Dev A** : Web Console (Upload PDF).
*   **Dev B** : Environnement 3D "Private Room" + Logique d'apparition.
*   **Objectif** : Feature complète "Premium" et import de documents réels.

---

## ⚠️ Points de Vigilance
*   **Firestore Costs** : Attention aux boucles de mise à jour trop rapides (sync de position). Il faudra optimiser (envoyer la position seulement si elle change > 1cm, ou max 10 fois/sec).
*   **Merge Conflicts** : Travaillez sur des Prefabs différents autant que possible. Dev A sur les Scripts Managers, Dev B sur les Prefabs UI/Visuels.
