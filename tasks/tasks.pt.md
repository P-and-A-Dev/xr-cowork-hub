# 🧩 **Lista Completa de Tarefas (ordenada logicamente)**

---

## **1️⃣ Setup & Fundações**

### 🔧 Setup do Projeto

* [x] Criar repositório Git
* [x] Configurar Unity + XR Interaction Toolkit
* [ ] Configurar URP + cenas
* [ ] Integrar Firebase Unity SDK
* [ ] Integrar Agora SDK

### 🔥 Estrutura Firebase

* [x] Criar projeto Firebase
* [ ] Adicionar google-services.json ao Unity
* [ ] Criar coleções Firestore: rooms, participants, panels
* [ ] Ativar Auth Anónima
* [ ] Ativar Storage (PDFs, imagens)

### 🎤 Agora Voice

* [ ] Criar projeto Agora
* [ ] Adicionar APP ID no Unity
* [ ] Testar entrada/saída no canal de áudio

---

## **2️⃣ Funcionalidades Básicas do Utilizador**

### 🧭 MR Passthrough

* [ ] Ativar passthrough (Meta XR)
* [ ] Testar colocação simples de um cubo
* [ ] Pipeline de ancoragem de painéis

### 🧊 Avatares / Presença

* [ ] ParticipantManager (listener Firestore)
* [ ] Avatar/orbe simples por utilizador
* [ ] Estados: online / typing

---

## **3️⃣ Painéis Espaciais (Core)**

### 📦 Sistema de Painéis

* [ ] PanelManager + Factory
* [ ] PanelBase.cs
* [ ] Sync de Transform (posição/rotação/escala)
* [ ] CRUD: criar / atualizar / apagar painel

### 📒 Tipos de Painéis

* [ ] Notepad multi-utilizador
* [ ] Lista To-Do
* [ ] PDF viewer
* [ ] Image viewer
* [ ] Painel de screenshot/captura
* [ ] Sticky notes
* [ ] Pomodoro
* [ ] Mini chat (opcional)
* [ ] Mind map (opcional)

---

## **4️⃣ Multi-utilizador (Sync)**

### 🔄 Sincronização Firestore

* [ ] Sync dos dados dos painéis
* [ ] Sync das transformações
* [ ] Sync dos campos dos participantes
* [ ] Otimização com batching/debounce

---

## **5️⃣ Voice Groups + Focus Bubbles**

### 🔊 Lógica de VoiceGroupId

* [ ] Adicionar campo `voiceGroupId` aos participantes
* [ ] Unity: mute/unmute conforme grupo
* [ ] Ghost visual quando user ≠ groupId

### 🔇 UI/Fluxo da Focus Bubble

* [ ] Selecionar 1+ utilizadores
* [ ] Gerar newGroupId
* [ ] Atualizar Firestore
* [ ] Sair da bubble (groupId = 0)

---

## **6️⃣ Private 3D Bubble Spaces**

### 🏰 Ambiente Privado

* [ ] Prefab BubbleSpace (dome/cubo/sala)
* [ ] Spawn à volta do utilizador
* [ ] Desativação visual do resto

### 🔊 Áudio isolado (reuse groupId)

* [ ] Agora: canal = groupId

### 📂 Painéis Privados

* [ ] Invisíveis para a sala principal
* [ ] visibility = "bubble"
* [ ] bubbleGroupId = currentGroupId

### 🔄 Sync do Estado Bubble

* [ ] Campo Firestore `inBubbleSpace`
* [ ] Spawn/despawn automático

---

## **7️⃣ Consola Web (React/Next.js)**

### 🌐 Upload PC → XR

* [ ] Página de upload
* [ ] Upload Firebase Storage
* [ ] Webhook: criar painel no Firestore
* [ ] Lista de ficheiros

---

## **8️⃣ Polish & UI**

### 🎨 UX & Conforto

* [ ] Shader ghost mais suave
* [ ] Animações dos painéis
* [ ] Avatares melhores
* [ ] Dicas & onboarding

### 🚀 Otimizações

* [ ] Batching Firestore
* [ ] Cache dos dados
* [ ] Limpeza / limites

---

## **9️⃣ Testes & Release**

* [ ] Teste real com 2 utilizadores
* [ ] Stress test dos painéis
* [ ] Teste das private bubble spaces
* [ ] Build APK Quest
* [ ] Upload Meta Quest Developer Hub
