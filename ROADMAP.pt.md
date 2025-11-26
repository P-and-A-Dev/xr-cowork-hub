# 📅 Plano de Implementação - XR Cowork Hub

## 📊 Relatório de Status (Feito vs A Fazer)

| Funcionalidade | Status Atual | Comentários |
| :--- | :--- | :--- |
| **Setup do Projeto** | ✅ Feito | Unity, Git, Pacotes instalados. |
| **Auth Firebase** | 🟡 Parcial | Script `FirebaseInit` básico (Login anônimo). Sem gerenciamento de estado do usuário. |
| **Voz Agora** | 🟡 Parcial | Script `AgoraInit` básico (Entrar no canal). Sem gerenciamento de mute/grupos. |
| **Passthrough** | ❌ A Fazer | Não configurado. |
| **Sistema de Painéis** | ❌ A Fazer | Sem lógica de spawn, movimento ou sincronização. |
| **Sincronização Multiplayer** | ❌ A Fazer | Sem `ParticipantManager` ou sincronização de posição. |
| **Focus Bubbles** | ❌ A Fazer | Funcionalidade principal (Áudio + Visual) ausente. |
| **Espaços 3D Privados** | ❌ A Fazer | Funcionalidade "Premium" ausente. |
| **Console Web** | ❌ A Fazer | Código inexistente. |

---

## 👥 Proposta de Distribuição (2 Desenvolvedores)

Para maximizar a eficiência, proponho separar as tarefas em dois eixos: **Sistema/Backend** (Dev A) e **Experiência XR/Visual** (Dev B).

### 🔧 Dev A: "Arquiteto de Sistema e Rede"
*Foco: Firebase, Agora, Sincronização de Dados, Lógica Pura.*

1.  **Gerenciamento de Participantes (Firestore)**: Criar `ParticipantManager`. Gerenciar estado Online/Offline, armazenar `voiceGroupId`.
2.  **Sistema de Sincronização (Rede)**: Codificar lógica para sincronizar posição/rotação de objetos (Painéis, Avatares) via Firestore.
3.  **Lógica de Áudio (Agora)**: Implementar troca de canal/mute baseado no `voiceGroupId`.
4.  **Console Web (React)**: Criar interface web para upload de arquivos para o Firebase Storage.

### 🥽 Dev B: "Especialista XR e Interações"
*Foco: Unity, UI, Interações Espaciais, Shaders.*

1.  **Setup Passthrough e Jogador**: Configurar rig XR, Passthrough e mãos.
2.  **Sistema de Painéis (Interação)**: Criar Prefabs de Painéis (Nota, PDF), torná-los "agarráveis" e redimensionáveis.
3.  **Visuais das Bolhas**: Criar shaders para efeito "Fantasma" (transparência de outros usuários) e domo visual da bolha.
4.  **Espaços 3D Privados**: Criar ambiente 3D que aparece ao entrar em uma bolha privada.

---

## 🗓️ Roadmap por Fases

### Fase 1: Fundações (Dias 1-2)
*   **Dev A**: `ParticipantManager` (ver quem está online) + Estrutura Firestore (`rooms/`).
*   **Dev B**: Setup Cena Passthrough + Prefab Avatar Simples (esfera seguindo a cabeça).
*   **Objetivo**: Ver um ao outro na sala (mesmo como esferas) e ver logs de conexão.

### Fase 2: Painéis e Sincronização (Dias 3-5)
*   **Dev A**: `PanelManager` (Sync posições/conteúdo Firestore).
*   **Dev B**: Prefabs de Painéis (Unity UI) + Interação (Agarrar/Redimensionar).
*   **Objetivo**: Criar um post-it, escrever nele, e o outro jogador ver ele se mover e atualizar.

### Fase 3: Focus Bubbles (Dias 6-8)
*   **Dev A**: Lógica `voiceGroupId` + Mute/Unmute dinâmico Agora.
*   **Dev B**: Shader "Fantasma" + UI para criar/entrar em uma bolha.
*   **Objetivo**: Clicar em um botão, ficar isolado áudio/visualmente dos outros.

### Fase 4: Espaços 3D Privados e Web (Dias 9-10)
*   **Dev A**: Console Web (Upload PDF).
*   **Dev B**: Ambiente 3D "Sala Privada" + Lógica de spawn.
*   **Objetivo**: Funcionalidade "Premium" completa e importação de documentos reais.

---

## ⚠️ Pontos de Atenção
*   **Custos Firestore**: Cuidado com loops de atualização muito rápidos (sync de posição). Necessário otimizar (enviar posição apenas se mudar > 1cm, ou max 10 vezes/seg).
*   **Conflitos de Merge**: Trabalhem em Prefabs diferentes o máximo possível. Dev A nos Scripts Managers, Dev B nos Prefabs UI/Visuais.
