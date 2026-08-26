# GitSwap

> **Troque entre contas Git com uma interface gráfica simples, sem usar o terminal.**

O GitSwap é uma aplicação desktop que permite gerenciar e alternar entre diferentes identidades Git (user.name e user.email) de forma simples e rápida, sem precisar decorar comandos ou abrir o terminal.

---

## Versão

**v1.1.0** — Versão atual

---

## Funcionalidades

- **Gerenciar perfis Git** — crie, edite e remova perfis com nome, usuário e email
- **Alternar conta com um clique** — mude o user.name e user.email instantaneamente
- **Conta global** — aplica `git config --global` para todos os repositórios
- **Conta local** — aplica `git config --local` para uma pasta específica
- **Badge Local/Global** — cada perfil exibe indicativo visual do tipo de conta
- **Conta global ativa** — identifica automaticamente qual perfil corresponde a conta global do git
- **Preview de comandos** — visualiza os comandos git antes de aplicar
- **Pasta opcional** — qualquer pasta é aceita, sem validação de repositório
- **Diálogo de adição** — formulário completo para criar novo perfil
- **Diálogo de edição** — altere os dados de um perfil existente (campos já preenchidos)
- **Conta ativa** — exibe qual conta está ativa no momento
- **Barra de status** — mostra conta ativa, escopo e contagem de perfis
- **Confirmação de exclusão** — pede confirmação antes de remover perfil
- **Barra de menus** — menu Arquivo (Adicionar, Exportar, Importar, Sair) e Ajuda (Ajuda do GitSwap, Configuração do Git, Sobre)
- **Janela de Ajuda** — guia completo de uso com navegação por tópicos no estilo Microsoft
- **Clonar perfil** — duplica um perfil existente para criar variações rápidas
- **Importar/Exportar perfis** — salve e carregue perfis em JSON para transferir entre computadores
- **Busca de perfis** — filtre perfis por nome, usuário ou email em tempo real
- **Reordenar perfis** — mova perfis para cima ou para baixo na lista
- **Atalhos de teclado** — Ctrl+N (adicionar), Ctrl+E (exportar), Ctrl+I (importar), Ctrl+H (ajuda)
- **Visualizar config Git** — veja toda a configuração do Git em uma janela dedicada
- **Diálogo Sobre** — informações do app e dados do sistema
- **Armazenamento local** — perfis salvos em JSON, sem backend ou banco de dados
- **Multiplataforma** — Windows, macOS e Linux
- **Seguro** — não armazena senhas, tokens, chaves SSH ou credenciais
- **Criptografia** — armazenamento com criptografia AES-256 opcional
- **Validação** — proteção contra path traversal e sanitização de entrada
- **Apoio** — botão "Cafézin pro dev" para apoiar o projeto
- **100% em português** — interface e mensagens em português com acentos corretos

---

## Changelog

### v1.1.0 — 26/08/2026

| # | Funcionalidade | Descrição |
|---|----------------|-----------|
| 1 | Capitalização de nomes | Primeira letra do nome do perfil e do user.name do Git sempre maiúscula |
| 2 | Menu nativo do SO | Menu agora segue o estilo nativo do sistema operacional |
| 3 | Nome na barra de menu | Aplicação exibe "GitSwap" na barra de menu (macOS e outros SOs) |

### v1.0.0 — 25/08/2026

Versão inicial do GitSwap com todas as funcionalidades e segurança:

| # | Funcionalidade | Descrição |
|---|----------------|-----------|
| 1 | Gerenciamento de perfis | Criar, editar, clonar e remover perfis Git |
| 2 | Alternância de contas | Troque user.name e user.email com um clique |
| 3 | Conta global e local | Aplicação global ou por repositório |
| 4 | Busca e filtros | Busca de perfis por nome, usuário ou email em tempo real |
| 5 | Importar/Exportar | Salve e carregue perfis em JSON |
| 6 | Janela de Ajuda | Guia completo de uso com navegação por tópicos |
| 7 | Visualizar config Git | Veja toda a configuração do Git |
| 8 | Atalhos de teclado | Ctrl+N, Ctrl+E, Ctrl+I, Ctrl+H |
| 9 | Multiplataforma | Windows, macOS e Linux |
| 10 | Interface em português | 100% da interface traduzida |
| 11 | Criptografia | Armazenamento com criptografia AES-256 |
| 12 | Validação de segurança | Proteção contra path traversal e sanitização |
| 13 | Validação de entrada | Verificação de email e dados obrigatórios |
| 14 | Doação | Botão "Cafézin pro dev" para apoiar o projeto |

---

## Pre-requisitos

| Requisito | Windows | macOS | Linux |
|-----------|---------|-------|-------|
| [.NET 10 SDK](https://dotnet.microsoft.com/download) | Sim | Sim | Sim |
| [Git](https://git-scm.com/downloads) | Sim | Sim (via Xcode CLI Tools) | Sim |

### Verificar instalação

```bash
# Verificar .NET
dotnet --version

# Verificar Git
git --version
```

---

## Como baixar e executar

### 1. Clonar o repositório

```bash
git clone https://github.com/wanfranklin/gitswap.git
cd gitswap
```

### 2. Restaurar e compilar

```bash
dotnet restore
dotnet build
```

### 3. Executar

```bash
dotnet run --project src/GitSwap
```

### 4. Limpar cache e executar

```bash
dotnet clean && dotnet run --project src/GitSwap
```

A janela do GitSwap será aberta automaticamente.

---

## Como usar

### Adicionar uma conta

1. Clique em **+ Adicionar conta** ou vá em **Arquivo > Adicionar conta**
2. Preencha:
   - **Nome do perfil** — nome descritivo (ex: Trabalho, Pessoal)
   - **Nome do usuário Git** — nome que aparecerá nos commits
   - **Email** — email que aparecerá nos commits
   - **Pasta do repositório** (opcional) — para aplicar apenas em um repositório
3. Clique em **Salvar**

### Editar uma conta

1. Clique no botão **Editar** no perfil desejado
2. Altere os dados necessários
3. Clique em **Salvar**

### Alternar de conta

1. Clique no botão **Usar** no perfil desejado
2. A conta será ativada automaticamente
3. Se tiver pasta de repositório, aplica localmente; senão, aplica globalmente

### Remover uma conta

1. Clique no botão **Excluir** no perfil desejado
2. Confirme a exclusão no diálogo

### Verificar a conta ativa

A seção **CONTA ATIVA** no topo da janela mostra o nome e email configurados no Git.

---

## Onde os dados são salvos

| Sistema Operacional | Caminho |
|---------------------|---------|
| **Windows** | `%APPDATA%\GitSwap\profiles.json` |
| **macOS** | `~/.config/GitSwap/profiles.json` |
| **Linux** | `~/.config/GitSwap/profiles.json` |

---

## Estrutura do projeto

```
gitswap/
├── src/GitSwap/
│   ├── Assets/
│   │   ├── icon.ico
│   │   ├── icon.png
│   │   └── logo.png
│   ├── Models/
│   │   └── GitProfile.cs
│   ├── Platform/macOS/
│   │   ├── Info.plist
│   │   └── icon.icns
│   ├── Services/
│   │   ├── CryptoService.cs
│   │   ├── GitService.cs
│   │   └── ProfileStorageService.cs
│   ├── ViewModels/
│   │   ├── MainWindowViewModel.cs
│   │   └── AddProfileDialogViewModel.cs
│   ├── Views/
│   │   ├── MainWindow.axaml / .cs
│   │   ├── AddProfileDialog.axaml / .cs
│   │   ├── EditProfileDialog.axaml / .cs
│   │   └── AboutDialog.axaml / .cs
│   ├── App.axaml / .cs
│   └── Program.cs
├── tests/GitSwap.Tests/
│   ├── GitProfileTests.cs
│   ├── JsonSerializationTests.cs
│   └── ProfileStorageServiceTests.cs
├── GitSwap.slnx
├── .gitignore
├── LICENSE
└── README.md
```

---

## Executar testes

```bash
dotnet test GitSwap.slnx
```

---

## Stack técnica

| Componente | Tecnologia |
|------------|------------|
| Linguagem | C# |
| Runtime | .NET 10 |
| UI | Avalonia UI 12.1.1 |
| Padrão | MVVM |
| MVVM Toolkit | CommunityToolkit.Mvvm 8.4.2 |
| Serialização | System.Text.Json |
| Criptografia | AES-256 (System.Security.Cryptography) |
| Testes | xUnit 2.9.3 |
| Git | Executável local via Process |

---

## O que o GitSwap NÃO faz

- Não gerencia repositórios
- Não faz clone, push, pull ou commit
- Não gerencia branches
- Não se conecta ao GitHub, GitLab ou Bitbucket
- Não gerencia SSH ou chaves de acesso
- Não armazena senhas ou tokens
- Não possui backend ou banco de dados
- Não possui telemetria ou rastreamento

---

## Como baixar e executar

Acesse a página de releases e baixe a versão para o seu sistema operacional:

| Sistema Operacional | Arquivo |
|---------------------|---------|
| **Windows** | `GitSwap-win-x64.zip` |
| **macOS Intel** | `GitSwap-osx-x64.zip` |
| **macOS Apple Silicon** | `GitSwap-osx-arm64.zip` |
| **Linux** | `GitSwap-linux-x64.zip` |

### Windows

1. Baixe o arquivo `GitSwap-win-x64.zip`
2. Extraia o ZIP em uma pasta de sua preferência
3. Execute `GitSwap.exe`
4. Se o Windows bloquear, clique com o botão direito > Propriedades > Desbloquear

### macOS

1. Baixe o arquivo correspondente ao seu processador (Intel ou Apple Silicon)
2. Extraia o ZIP e arraste o `GitSwap.app` para a pasta Aplicativos
3. Na primeira execução, clique com o botão direito > Abrir
4. Se o macOS bloquear, vá em Ajustes > Privacidade e clique em Abrir mesmo assim

### Linux

1. Baixe o arquivo `GitSwap-linux-x64.zip`
2. Extraia o ZIP em uma pasta de sua preferência
3. Torne o executável executável: `chmod +x GitSwap`
4. Execute: `./GitSwap`

### Requisito

O Git precisa estar instalado no computador. O GitSwap utiliza os comandos git para ler e alterar a configuração.

---

## Licença

Autor: Wanfranklin Alves

Este projeto está licenciado sob a [GNU General Public License v3.0](LICENSE).

Veja o arquivo [LICENSE](LICENSE) para mais detalhes.
