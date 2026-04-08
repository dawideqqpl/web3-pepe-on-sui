# PEPE on SUI

Unity 2D arcade prototype inspired by Flappy Bird, extended with PlayFab backend features and optional Web3 / Telegram login flows.

## Demo

Gameplay video:

https://github.com/user-attachments/assets/074b3542-0e85-4f12-bc57-41d63b257c2a

Fallback link: [Watch the demo video](https://github.com/user-attachments/assets/074b3542-0e85-4f12-bc57-41d63b257c2a)

## Stack

- Unity `2022.3.22f1`
- C#
- PlayFab SDK
- Web3Unity / ChainSafe integration
- TextMeshPro
- OPS Anti-Cheat

## Main Scene

The current gameplay scene enabled in build settings is:

- `Assets/Assets/Scenes/Flappy Bird.unity`

Additional login scenes are available in:

- `Assets/Web3Unity/Scenes/WebLogin.unity`
- `Assets/Web3Unity/Scenes/WalletLogin.unity`

## Features

- Flappy Bird style obstacle gameplay
- Score and bonus point system
- PlayFab login, leaderboard, currency, and CloudScript integration
- Optional wallet-based Web3 flow
- Telegram data bridge for account/display name setup
- Token price lookup and holder bonus thresholds

## How To Open

1. Clone the repository.
2. Open the project in Unity Hub using Unity `2022.3.22f1`.
3. Let Unity reimport packages and generate local project files.
4. Open `Assets/Assets/Scenes/Flappy Bird.unity`.
5. Press Play.

## Local Configuration

This public repository is sanitized for GitHub. Real local values should not be committed.

The project reads runtime config from:

- `Assets/Resources/ProjectConfigData.asset`
- or, if present, `Assets/Resources/ProjectConfigDataLocal.asset`

`ProjectConfigDataLocal.asset` is ignored by git and should be used for machine-specific or private local setup.

Recommended local workflow:

1. Duplicate `Assets/Resources/ProjectConfigData.asset`.
2. Rename the copy to `ProjectConfigDataLocal.asset`.
3. Fill in your local values there if needed.

Current client-safe config fields:

- `ProjectId`
- `ChainId`
- `Chain`
- `Network`
- `Rpc`
- `PlayFabTitleId`
- `DexScreenerPairUrl`

## Public Repo Notes

- No server-side secrets should be stored in this project.
- `DeveloperSecretKey` is intentionally not committed.
- Local build output, caches, and archives are excluded via `.gitignore`.
- The checked-in config contains placeholders where appropriate for public sharing.

## Repository Structure

- `Assets/Scripts` - custom gameplay / service scripts
- `Assets/Assets/Scripts` - core gameplay loop scripts
- `Assets/Resources` - shared runtime config and resources
- `Assets/Web3Unity` - wallet / blockchain integration
- `Assets/PlayFabSDK` - PlayFab SDK

## Notes

This repository is presented as a portfolio / recruiter-facing code sample, so the focus is on keeping the project runnable while avoiding committing private environment data or unnecessary generated files.
