# Plasma Siege – Technical Implementation Guide (technical-implementation.md)

## 0. Purpose
This document is the single hand-off package for any engineer joining the **Plasma Siege** project on **Linux (Pop!_OS)**. It explains how to set up the complete toolchain (Unity + MCP + Claude Code, Supabase, Solana, n8n), implement the PLASMA token economy from day one, build NFT-enabled spaceships, and create a browser-based competitive game with Web2/Web3 hybrid authentication. Each development stage includes copy-paste Claude Code prompts, SQL schemas, and workflow configurations.

**Scope**: PLASMA token-powered browser game with entry fees/rewards, NFT spaceships with upgrades, dynamic arena editor, Web2/Web3 onboarding, deterministic physics, and automated backend operations.

## 1. Prerequisites

| Tool | Version | Notes |
|------|---------|--------|
| Pop!_OS | 22.04 LTS | Ubuntu-based, NVIDIA-friendly |
| Unity Hub Linux | Latest | Native Linux version |
| Unity Editor | 2023.3 LTS | With WebGL module |
| Unity MCP Bridge | Latest git | `https://github.com/justinpbarnett/unity-mcp.git` |
| Claude Desktop | Latest | With MCP support |
| Node.js | 20 LTS | Via nvm (not apt) |
| Python | 3.10+ | System python3 |
| Solana CLI | 1.18+ | For token deployment |
| Supabase CLI | 1.136+ | Cloud, not local |
| Docker | Latest | For n8n only |
| Git + Git LFS | Latest | LFS for assets |

## 2. Linux Workstation Setup

### 2.1 System Preparation
```bash
# Update Pop!_OS
sudo apt update && sudo apt upgrade -y

# Install development essentials
sudo apt install -y build-essential git curl wget \
  libssl-dev libffi-dev python3-dev python3-pip \
  libgconf-2-4 libglu1 libnss3 libxss1 libasound2 \
  libxtst6 libgtk-3-0 libgbm-dev libvulkan1 vulkan-utils

# Install Git LFS
sudo apt install git-lfs
git lfs install
```

### 2.2 Unity Installation
```bash
# Download Unity Hub for Linux
wget -qO - https://hub.unity3d.com/linux/keys/public | sudo apt-key add -
sudo sh -c 'echo "deb https://hub.unity3d.com/linux/repos/deb stable main" > /etc/apt/sources.list.d/unityhub.list'
sudo apt update
sudo apt install unityhub

# Launch Unity Hub
unityhub &

# Install Unity 2023.3 LTS with WebGL module via Hub GUI
```

### 2.3 Node.js via NVM
```bash
# Install nvm
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.0/install.sh | bash
source ~/.bashrc

# Install Node.js
nvm install 20
nvm use 20
nvm alias default 20
```

### 2.4 Solana CLI & Token Setup
```bash
# Install Solana
sh -c "$(curl -sSfL https://release.solana.com/stable/install)"
export PATH="$HOME/.local/share/solana/install/active_release/bin:$PATH"

# Configure for devnet
solana config set --url https://api.devnet.solana.com

# Create development wallet
solana-keygen new --outfile ~/.config/solana/devnet.json
solana config set --keypair ~/.config/solana/devnet.json

# Get devnet SOL
solana airdrop 2

# Create PLASMA token
spl-token create-token --decimals 8
# Save this token mint address as PLASMA_TOKEN_MINT

# Create NFT collection for spaceships
# We'll use Metaplex for ship NFTs
npm install -g @metaplex-foundation/sugar-cli
```

### 2.5 Unity MCP Server
```bash
# Create project directory
mkdir -p ~/Development/3DOrbShooter
cd ~/Development/3DOrbShooter

# Clone MCP server
git clone https://github.com/justinpbarnett/unity-mcp.git tools/unity-mcp-server

# Set up Python environment
cd tools/unity-mcp-server
python3 -m venv venv
source venv/bin/activate
pip install -r requirements.txt

# Configure Claude Desktop
mkdir -p ~/.config/claude
cat > ~/.config/claude/claude_desktop_config.json << 'EOF'
{
  "mcpServers": {
    "unity-mcp": {
      "command": "python3",
      "args": ["/home/$USER/Development/3DOrbShooter/tools/unity-mcp-server/server.py"],
      "env": {
        "UNITY_PROJECT_PATH": "/home/$USER/Development/3DOrbShooter/UnityProject",
        "MCP_WEBSOCKET_PORT": "5010"
      }
    }
  }
}
EOF
```

### 2.6 Supabase Cloud Setup
```bash
# Install Supabase CLI
npm install -g supabase

# Login to Supabase Cloud (create account first)
supabase login

# Create new project on supabase.com
# Copy project ID and anon key to .env
```

### 2.7 n8n Docker Setup
```bash
# Install Docker on Pop!_OS
sudo apt install docker.io docker-compose
sudo usermod -aG docker $USER
# Logout and login for group change

# Run n8n
docker run -d --name n8n \
  -p 5678:5678 \
  -e N8N_BASIC_AUTH_ACTIVE=true \
  -e N8N_BASIC_AUTH_USER=admin \
  -e N8N_BASIC_AUTH_PASSWORD=devpass \
  -v ~/.n8n:/home/node/.n8n \
  n8nio/n8n

# Access at http://localhost:5678
```

## 3. Project Structure
```
~/Development/PlasmaSiege/
├── UnityProject/
│   ├── Assets/
│   │   ├── _Game/
│   │   │   ├── Scripts/
│   │   │   │   ├── Core/
│   │   │   │   ├── Blockchain/
│   │   │   │   ├── Ships/         # NEW: Ship NFT system
│   │   │   │   ├── Arena/
│   │   │   │   ├── Economy/
│   │   │   │   └── UI/
│   │   │   ├── Prefabs/
│   │   │   │   ├── Ships/         # 3 tiers of ships
│   │   │   │   └── Upgrades/      # Fuel, Shield, Weapon mods
│   │   │   └── ScriptableObjects/
│   │   │       ├── ShipConfigs/   # Ship tier definitions
│   │   │       └── UpgradeConfigs/
│   │   └── Plugins/
│   │       └── WebGL/
├── contracts/              # Solana programs & NFT metadata
├── supabase/              # Database schemas
├── web-app/               # React frontend
├── workflows/             # n8n exports
├── tools/
│   └── unity-mcp-server/
└── .env
```

## 4. Environment Configuration
```bash
# Create .env file
cat > ~/Development/PlasmaSiege/.env << 'EOF'
# Supabase
NEXT_PUBLIC_SUPABASE_URL=https://xxxxx.supabase.co
NEXT_PUBLIC_SUPABASE_ANON_KEY=eyJhbGc...
SUPABASE_SERVICE_ROLE_KEY=eyJhbGc...

# Solana
NEXT_PUBLIC_SOLANA_NETWORK=devnet
NEXT_PUBLIC_PLASMA_TOKEN_MINT=YOUR_PLASMA_TOKEN_MINT_ADDRESS
NEXT_PUBLIC_SHIP_COLLECTION_ADDRESS=YOUR_NFT_COLLECTION_ADDRESS
TREASURY_WALLET_PRIVATE_KEY=YOUR_TREASURY_KEY

# n8n
N8N_WEBHOOK_URL=http://localhost:5678/webhook/match-complete

# Unity paths
UNITY_PROJECT_PATH=/home/$USER/Development/PlasmaSiege/UnityProject
EOF
```

## 5. Stage-by-Stage Implementation

### Stage 0: PLASMA Token & NFT Setup (Day 1)

#### Deploy PLASMA Token
```bash
# Create PLASMA token
spl-token create-token --decimals 8
# Save the token mint address as PLASMA_TOKEN_MINT

# Create token account
spl-token create-account <PLASMA_TOKEN_MINT>

# Mint initial supply (1 billion PLASMA)
spl-token mint <PLASMA_TOKEN_MINT> 1000000000

# Verify
spl-token supply <PLASMA_TOKEN_MINT>
```

#### Ship NFT Collection Setup
```bash
# Create NFT collection metadata
cat > ~/Development/PlasmaSiege/contracts/ship-collection.json << 'EOF'
{
  "name": "Plasma Siege Ships",
  "symbol": "PSS",
  "description": "Upgradeable spaceship NFTs for Plasma Siege",
  "image": "https://arweave.net/your-collection-image",
  "attributes": [
    {"trait_type": "Collection", "value": "Plasma Siege Ships"}
  ],
  "properties": {
    "files": [{"uri": "https://arweave.net/your-collection-image", "type": "image/png"}],
    "category": "image"
  }
}
EOF
```

#### Claude Code Prompt 0 - Project Setup with NFT Ships
```
## Purpose
Create Unity project with PLASMA token economy and NFT spaceship system from the start.

## Technical Constraints
- Unity 2023.3 LTS with URP for WebGL
- Fixed timestep 0.02 (50Hz) for deterministic physics
- 3 ship tiers: Scout (common), Fighter (rare), Destroyer (legendary)
- Ships are mintable as NFTs after reaching certain levels
- Upgrade system: Fuel Boosters, Shield Modules, Weapon Systems

## Development Guidelines
- Ships start as in-game items, become NFTs at level 10
- Each ship has upgrade slots (3 fuel, 3 shield, 3 weapon)
- NFT metadata includes ship stats and upgrade configuration
- Support both Web2 (database ships) and Web3 (NFT ships)

### Tasks
1. Configure Unity project:
   - Import Thirdweb SDK for NFT support
   - Set up ship prefab structure (3 tiers)
   - Create upgrade slot system

2. Create ship scripts:
   - ShipBase.cs (abstract class for all ships)
   - ShipNFTManager.cs (handles minting/metadata)
   - ShipUpgradeSystem.cs (modular upgrades)

3. ScriptableObjects:
   - ShipConfig (Scout, Fighter, Destroyer)
   - UpgradeConfig (Fuel, Shield, Weapon variants)

4. NFT Integration:
   - Metadata generator for on-chain storage
   - Visual differences for each tier
   - Upgrade visualization system

5. UI Updates:
   - Ship selection screen
   - Upgrade slots interface
   - "Mint as NFT" button (unlocks at level 10)

Return implementation focusing on modularity.
```

### Stage 1: Ship System & Core Gameplay (Day 2-3)

#### Claude Code Prompt 1 - NFT Ships with PLASMA Integration
```
## Purpose
Implement 3-tier ship system with NFT minting capability and PLASMA token integration.

## Technical Constraints
- Scout Ship: 100 shield, 80 speed, 1 weapon slot (costs 0 PLASMA - starter)
- Fighter Ship: 200 shield, 60 speed, 2 weapon slots (costs 500 PLASMA)
- Destroyer Ship: 300 shield, 40 speed, 3 weapon slots (costs 2000 PLASMA)
- Ships become mintable NFTs at level 10
- Upgrade slots: 3 each for fuel, shield, weapons

## Development Guidelines
- Ships stored in database until minted
- NFT metadata includes: tier, level, equipped upgrades
- Visual customization based on upgrades
- Seamless transition from database to NFT

### Tasks
1. Ship Classes:
   - ScoutShip.cs (inherits ShipBase)
   - FighterShip.cs (more armor, slower)
   - DestroyerShip.cs (tank class)

2. NFT System:
   - ShipNFTMetadata.cs (generates JSON)
   - MintingManager.cs (handles blockchain tx)
   - NFTShipLoader.cs (loads ships from wallet)

3. Upgrade Implementation:
   - FuelBooster.cs (increases max fuel by 25/50/75%)
   - ShieldModule.cs (increases shields by 50/100/150)
   - WeaponUpgrade.cs (damage multiplier 1.5x/2x/2.5x)

4. Ship Progression:
   - Experience system (XP from matches)
   - Level 1-10 progression
   - "Mint NFT" unlocks at level 10

5. Database Schema:
   - player_ships table
   - ship_upgrades table
   - ship_experience table

Return complete implementation with NFT metadata structure.
```

### Stage 2: Dynamic Arena System (Day 4)

#### Claude Code Prompt 2 - Arena Editor with Token Pickups
```
## Purpose
Create dynamic arena editor GUI and token pickup system.

## Technical Constraints
- Unity Splines for path generation
- 3-8 control points adjustable in runtime
- Deterministic token spawn positions
- Arena configs saved as ScriptableObjects

## Development Guidelines
- In-game editor for testing
- Visual spline preview
- Token pickups worth 1-5 ORB

### Tasks
1. ArenaEditor.cs:
   - Runtime GUI for spline editing
   - Arena size slider (300-500 units)
   - Obstacle density control
   - Save/load configurations

2. SplineOrbPath.cs:
   - Dynamic spline generation
   - Path validation (no intersections)
   - Visual preview mode

3. TokenPickupSystem.cs:
   - Spawn 5-10 tokens per match
   - Value based on arena difficulty
   - Collection effects and sound

4. ArenaConfig.asset:
   - Scriptable object template
   - Preset configurations

Linux note: Use $HOME paths for saved configs.
```

### Stage 3: PLASMA Economy & Ship Upgrades (Day 5)

#### Claude Code Prompt 3 - Entry Fees and Ship Upgrade Shop
```
## Purpose
Implement PLASMA token economy with ship purchases and upgrade system.

## Technical Constraints
- Entry fee: 10 PLASMA for ranked matches
- Ship prices: Scout (free), Fighter (500 PLASMA), Destroyer (2000 PLASMA)
- Upgrade costs: 
  - Fuel Boosters: 50/100/200 PLASMA (tier 1/2/3)
  - Shield Modules: 75/150/300 PLASMA
  - Weapon Systems: 100/200/400 PLASMA
- Ships must be level 10 to mint as NFT (costs 100 PLASMA gas)

## Development Guidelines
- All transactions in PLASMA tokens
- Clear cost/benefit display
- Ships retain upgrades when minted as NFT
- NFT ships can be imported from wallet

### Tasks
1. ShipShopUI.cs:
   - Display 3 ship tiers with stats
   - Show PLASMA costs
   - "Locked" state for insufficient balance
   - Preview ship models

2. UpgradeShopUI.cs:
   - Grid layout for each upgrade type
   - Visual representation of upgrade effects
   - Compatibility check (ship tier restrictions)
   - Total cost calculator

3. PLASMATransactionManager.cs:
   - Purchase validation
   - Transaction queuing
   - Rollback on failure
   - Receipt generation

4. NFTMintingUI.cs:
   - Show eligible ships (level 10+)
   - Minting cost: 100 PLASMA
   - Metadata preview
   - Transaction status

5. Database Updates:
   - Log all PLASMA transactions
   - Track ship ownership
   - Store upgrade configurations

Return complete shop implementation with NFT minting flow.
```

### Stage 4: Combat System (Day 6)

#### Claude Code Prompt 4 - Token-Aware Combat
```
## Purpose
Implement combat system where damage and rewards involve tokens.

## Technical Constraints
- Object pooling for zero GC
- Deterministic projectile physics
- Token drops on ship destruction
- Linux performance optimization

## Development Guidelines
- Pool all projectiles and effects
- Clear visual feedback for token events
- Balanced risk/reward mechanics

### Tasks
1. ProjectileSystem.cs:
   - Object pool (200 lasers, 50 missiles)
   - Damage affects orb momentum
   - Hit registration with effects

2. CombatRewards.cs:
   - Destroyed ships drop 50% tokens
   - Perfect kill bonus (5 ORB)
   - Damage dealt tracking

3. WeaponUpgrades.cs:
   - Apply purchased upgrades
   - Visual weapon differences
   - Damage scaling

4. Performance monitoring:
   - GC allocation tracking
   - Frame time analysis

Target: < 2KB GC allocation per frame.
```

### Stage 5: AI Opponent (Day 7)

#### Claude Code Prompt 5 - AI with Economic Behavior
```
## Purpose
Create AI opponent that collects tokens and makes upgrade decisions.

## Technical Constraints
- Deterministic AI for replay validation
- Difficulty affects token collection priority
- AI must use same economy rules as players

## Development Guidelines
- Behavior tree or state machine
- Visible AI decision making
- Fair but challenging

### Tasks
1. AIController.cs:
   - States: Collect, Attack, Defend, Score
   - Token collection priority
   - Upgrade decision logic

2. DifficultyScaler.cs:
   - Accuracy: 30-70%
   - Reaction time: 1.0-0.3s
   - Token awareness: low-high

3. AITokenManager.cs:
   - Track AI token balance
   - Spend on upgrades between rounds
   - Drop tokens when destroyed

Linux optimization: Use Job System for AI calculations.
```

### Stage 6: Backend Integration (Day 8)

#### Claude Code Prompt 6 - Supabase & Match Validation
```
## Purpose
Connect all systems to Supabase backend with match validation.

## Technical Constraints
- Real-time leaderboards
- Match result verification
- Token transaction logging
- Linux-compatible networking

## Development Guidelines
- Async/await patterns
- Retry with exponential backoff
- Offline queue for transactions

### Tasks
1. SupabaseManager.cs:
   - Authentication flow
   - Real-time subscriptions
   - Transaction logging

2. MatchValidator.cs:
   - Record all game events
   - Generate deterministic checksum
   - Submit results to backend

3. LeaderboardUI.cs:
   - Real-time updates
   - Token-based rankings
   - Personal stats

4. NetworkOptimization.cs:
   - Connection pooling
   - Request batching
   - Compression

Linux: Use native sockets for better performance.
```

### Stage 7: Polish & Optimization (Day 9-10)

#### Claude Code Prompt 7 - Linux Performance & WebGL Build
```
## Purpose
Optimize for WebGL deployment on Linux build machines.

## Technical Constraints
- Build size < 30MB compressed
- 60 FPS on GTX 1060
- Fast initial load
- Linux CI/CD pipeline

## Development Guidelines
- Aggressive optimization
- Asset bundling
- Texture compression

### Tasks
1. BuildOptimization.cs:
   - Strip unused code
   - Optimize shaders
   - Compress textures (ASTC)

2. LinuxBuildScript.sh:
   - Automated build process
   - Asset validation
   - Size reporting

3. PerformanceMonitor.cs:
   - FPS tracking
   - Memory usage
   - Network latency

4. WebGLTemplate:
   - Loading progress
   - Mobile detection
   - Wallet integration

Return build configuration files.
```

## 6. Complete Database Schema

```sql
-- Enable extensions
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";
CREATE EXTENSION IF NOT EXISTS "pgcrypto";

-- Users table (Web2 and Web3)
CREATE TABLE users (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  email TEXT UNIQUE,
  auth_type TEXT NOT NULL CHECK (auth_type IN ('email', 'social', 'wallet', 'hybrid')),
  display_name TEXT,
  avatar_url TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Wallet management
CREATE TABLE user_wallets (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id UUID REFERENCES users(id) ON DELETE CASCADE,
  wallet_address TEXT UNIQUE NOT NULL,
  wallet_type TEXT NOT NULL CHECK (wallet_type IN ('custodial', 'external')),
  is_primary BOOLEAN DEFAULT TRUE,
  encrypted_private_key TEXT, -- For custodial only
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Player statistics
CREATE TABLE player_stats (
  user_id UUID PRIMARY KEY REFERENCES users(id),
  wallet_address TEXT NOT NULL,
  plasma_balance DECIMAL(20, 8) DEFAULT 1000, -- Starter PLASMA tokens
  lifetime_earned DECIMAL(20, 8) DEFAULT 0,
  lifetime_spent DECIMAL(20, 8) DEFAULT 0,
  total_matches INTEGER DEFAULT 0,
  matches_won INTEGER DEFAULT 0,
  current_tier INTEGER DEFAULT 1,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  updated_at TIMESTAMPTZ DEFAULT NOW()
);

-- Player ships (before NFT minting)
CREATE TABLE player_ships (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  user_id UUID REFERENCES users(id),
  ship_type TEXT NOT NULL CHECK (ship_type IN ('scout', 'fighter', 'destroyer')),
  ship_name TEXT,
  level INTEGER DEFAULT 1,
  experience INTEGER DEFAULT 0,
  is_nft BOOLEAN DEFAULT FALSE,
  nft_mint_address TEXT,
  nft_metadata JSONB,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Ship upgrades
CREATE TABLE ship_upgrades (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  ship_id UUID REFERENCES player_ships(id),
  upgrade_type TEXT NOT NULL CHECK (upgrade_type IN ('fuel', 'shield', 'weapon')),
  upgrade_tier INTEGER CHECK (upgrade_tier BETWEEN 1 AND 3),
  slot_position INTEGER CHECK (slot_position BETWEEN 1 AND 3),
  plasma_cost DECIMAL(20, 8) NOT NULL,
  purchased_at TIMESTAMPTZ DEFAULT NOW()
);

-- PLASMA token transactions
CREATE TABLE plasma_transactions (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  from_wallet TEXT,
  to_wallet TEXT,
  amount DECIMAL(20, 8) NOT NULL,
  transaction_type TEXT NOT NULL CHECK (transaction_type IN 
    ('entry_fee', 'win_reward', 'ship_purchase', 'upgrade_purchase', 
     'nft_mint', 'pickup', 'starter_bonus')),
  reference_id UUID, -- Can reference match_id, ship_id, etc.
  blockchain_signature TEXT,
  status TEXT DEFAULT 'pending' CHECK (status IN ('pending', 'confirmed', 'failed')),
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Match results
CREATE TABLE match_results (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  player_id UUID REFERENCES users(id),
  player_ship_id UUID REFERENCES player_ships(id),
  opponent_id UUID REFERENCES users(id),
  opponent_ship_id UUID REFERENCES player_ships(id),
  arena_config_id UUID,
  entry_fee DECIMAL(20, 8) DEFAULT 10, -- 10 PLASMA
  winner_id UUID NOT NULL,
  winner_reward DECIMAL(20, 8) DEFAULT 20, -- 20 PLASMA
  plasma_collected DECIMAL(20, 8) DEFAULT 0,
  match_checksum TEXT NOT NULL,
  replay_data JSONB,
  match_duration INTEGER NOT NULL,
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- NFT minting queue
CREATE TABLE nft_mint_queue (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  ship_id UUID REFERENCES player_ships(id),
  user_id UUID REFERENCES users(id),
  metadata JSONB NOT NULL,
  mint_cost DECIMAL(20, 8) DEFAULT 100, -- 100 PLASMA for minting
  status TEXT DEFAULT 'pending' CHECK (status IN ('pending', 'minting', 'completed', 'failed')),
  mint_address TEXT,
  transaction_signature TEXT,
  created_at TIMESTAMPTZ DEFAULT NOW(),
  completed_at TIMESTAMPTZ
);

-- Ship experience tracking
CREATE TABLE ship_match_history (
  id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
  ship_id UUID REFERENCES player_ships(id),
  match_id UUID REFERENCES match_results(id),
  experience_gained INTEGER NOT NULL,
  damage_dealt INTEGER,
  damage_taken INTEGER,
  plasma_collected DECIMAL(20, 8),
  created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Indexes for performance
CREATE INDEX idx_wallet_address ON user_wallets(wallet_address);
CREATE INDEX idx_player_ships_user ON player_ships(user_id);
CREATE INDEX idx_ship_upgrades_ship ON ship_upgrades(ship_id);
CREATE INDEX idx_plasma_tx_wallet ON plasma_transactions(from_wallet, to_wallet);
CREATE INDEX idx_nft_ships ON player_ships(is_nft, nft_mint_address);

-- Functions for ship progression
CREATE OR REPLACE FUNCTION calculate_ship_level(exp INTEGER)
RETURNS INTEGER AS $
BEGIN
  -- Level formula: level = floor(sqrt(exp / 100)) + 1
  -- Level 10 requires 10,000 XP
  RETURN FLOOR(SQRT(exp::FLOAT / 100)) + 1;
END;
$ LANGUAGE plpgsql;

-- Trigger to update ship level
CREATE OR REPLACE FUNCTION update_ship_level()
RETURNS TRIGGER AS $
BEGIN
  NEW.level = calculate_ship_level(NEW.experience);
  RETURN NEW;
END;
$ LANGUAGE plpgsql;

CREATE TRIGGER ship_level_update
BEFORE UPDATE OF experience ON player_ships
FOR EACH ROW
EXECUTE FUNCTION update_ship_level();

-- Row Level Security
ALTER TABLE users ENABLE ROW LEVEL SECURITY;
ALTER TABLE user_wallets ENABLE ROW LEVEL SECURITY;
ALTER TABLE player_stats ENABLE ROW LEVEL SECURITY;
ALTER TABLE player_ships ENABLE ROW LEVEL SECURITY;
ALTER TABLE ship_upgrades ENABLE ROW LEVEL SECURITY;

-- RLS Policies
CREATE POLICY users_self ON users 
  FOR ALL USING (auth.uid() = id);

CREATE POLICY ships_owner ON player_ships 
  FOR ALL USING (auth.uid() = user_id);

CREATE POLICY upgrades_owner ON ship_upgrades 
  FOR ALL USING (EXISTS (
    SELECT 1 FROM player_ships 
    WHERE player_ships.id = ship_upgrades.ship_id 
    AND player_ships.user_id = auth.uid()
  ));
```

## 7. n8n Workflows

### Ship NFT Minting Workflow
```json
{
  "name": "mint-ship-nft",
  "nodes": [
    {
      "parameters": {
        "path": "/webhook/mint-ship",
        "methods": ["POST"]
      },
      "name": "Webhook",
      "type": "n8n-nodes-base.webhook",
      "position": [250, 300]
    },
    {
      "parameters": {
        "operation": "get",
        "table": "player_ships",
        "id": "={{$json[\"shipId\"]}}"
      },
      "name": "Get Ship Data",
      "type": "n8n-nodes-base.supabase",
      "position": [450, 300]
    },
    {
      "parameters": {
        "functionName": "generate-nft-metadata",
        "jsonParameters": {
          "shipType": "={{$json[\"ship_type\"]}}",
          "level": "={{$json[\"level\"]}}",
          "upgrades": "={{$json[\"upgrades\"]}}"
        }
      },
      "name": "Generate Metadata",
      "type": "n8n-nodes-base.function",
      "position": [650, 300]
    },
    {
      "parameters": {
        "method": "POST",
        "url": "https://api.metaplex.com/mint",
        "authentication": "predefinedCredentialType",
        "nodeCredentialType": "metaplexApi",
        "jsonParameters": true,
        "options": {
          "bodyContentType": "json"
        },
        "bodyParametersJson": {
          "metadata": "={{$json[\"metadata\"]}}",
          "collection": "{{$env[\"SHIP_COLLECTION_ADDRESS\"]}}"
        }
      },
      "name": "Mint NFT",
      "type": "n8n-nodes-base.httpRequest",
      "position": [850, 300]
    }
  ]
}
```

### PLASMA Token Distribution Workflow
```json
{
  "name": "distribute-plasma-rewards",
  "nodes": [
    {
      "parameters": {
        "path": "/webhook/match-complete",
        "methods": ["POST"]
      },
      "name": "Webhook",
      "type": "n8n-nodes-base.webhook",
      "position": [250, 300]
    },
    {
      "parameters": {
        "operation": "update",
        "table": "match_results",
        "columns": {
          "winner_id": "={{$json[\"winnerId\"]}}",
          "plasma_collected": "={{$json[\"plasmaCollected\"]}}",
          "match_checksum": "={{$json[\"checksum\"]}}"
        }
      },
      "name": "Update Match",
      "type": "n8n-nodes-base.supabase",
      "position": [450, 300]
    },
    {
      "parameters": {
        "operation": "executeQuery",
        "query": "UPDATE player_stats SET plasma_balance = plasma_balance + $1 WHERE user_id = $2",
        "queryParams": "20,{{$json[\"winnerId\"]}}"
      },
      "name": "Credit Winner",
      "type": "n8n-nodes-base.supabase",
      "position": [650, 300]
    },
    {
      "parameters": {
        "method": "POST",
        "url": "https://api.devnet.solana.com",
        "jsonParameters": true,
        "bodyParametersJson": {
          "jsonrpc": "2.0",
          "method": "sendTransaction",
          "params": {
            "instruction": "transfer",
            "from": "{{$env[\"TREASURY_WALLET\"]}}",
            "to": "={{$json[\"winnerWallet\"]}}",
            "amount": 20000000000,
            "mint": "{{$env[\"PLASMA_TOKEN_MINT\"]}}"
          }
        }
      },
      "name": "Transfer PLASMA",
      "type": "n8n-nodes-base.httpRequest",
      "position": [850, 300]
    }
  ]
}
```

## 8. Linux Build Script

```bash
#!/bin/bash
# build-plasma-siege.sh

set -e

PROJECT_PATH="/home/$USER/Development/PlasmaSiege/UnityProject"
BUILD_PATH="/home/$USER/Development/PlasmaSiege/builds/webgl"
UNITY_PATH="/home/$USER/Unity/Hub/Editor/2023.3.16f1/Editor/Unity"

echo "Building Plasma Siege WebGL on Linux..."

# Clean previous build
rm -rf "$BUILD_PATH"
mkdir -p "$BUILD_PATH"

# Run Unity build
"$UNITY_PATH" \
  -batchmode \
  -quit \
  -logFile build.log \
  -projectPath "$PROJECT_PATH" \
  -buildTarget WebGL \
  -executeMethod BuildScript.BuildWebGL \
  -buildOutput "$BUILD_PATH"

# Check build size
SIZE=$(du -sh "$BUILD_PATH" | cut -f1)
echo "Build complete! Size: $SIZE"

# Compress for deployment
cd "$BUILD_PATH"
tar -czf ../PlasmaSiege-$(date +%Y%m%d-%H%M%S).tar.gz .

echo "Plasma Siege build packaged and ready for deployment!"
```

## 9. Performance Targets

| Metric | Target | Measurement |
|--------|--------|-------------|
| FPS | 60 | Unity Profiler |
| GC Alloc/Frame | < 2KB | Profiler Memory |
| Initial Load | < 10s | Browser DevTools |
| Build Size | < 30MB | Compressed |
| Token Transaction | < 5s | End-to-end |
| Memory Usage | < 1.5GB | Task Manager |

## 10. CI/CD Pipeline (GitHub Actions)

```yaml
name: Build-Deploy-Linux

on:
  push:
    branches: [main, develop]

env:
  UNITY_LICENSE: ${{ secrets.UNITY_LICENSE }}

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with:
          lfs: true
      
      - name: Cache Library
        uses: actions/cache@v3
        with:
          path: UnityProject/Library
          key: Library-Linux-${{ hashFiles('UnityProject/Assets/**', 'UnityProject/Packages/**') }}
      
      - name: Run Tests
        uses: game-ci/unity-test-runner@v4
        with:
          projectPath: UnityProject
          unityVersion: 2023.3.16f1
          testMode: PlayMode

  build:
    needs: test
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
        with:
          lfs: true
      
      - name: Build WebGL
        uses: game-ci/unity-builder@v4
        with:
          projectPath: UnityProject
          unityVersion: 2023.3.16f1
          targetPlatform: WebGL
          buildsPath: builds
      
      - name: Upload to R2
        env:
          R2_ACCOUNT_ID: ${{ secrets.R2_ACCOUNT_ID }}
          R2_ACCESS_KEY: ${{ secrets.R2_ACCESS_KEY }}
          R2_SECRET_KEY: ${{ secrets.R2_SECRET_KEY }}
        run: |
          # Install rclone
          curl https://rclone.org/install.sh | sudo bash
          
          # Configure R2
          rclone config create r2 s3 \
            provider=Cloudflare \
            access_key_id=$R2_ACCESS_KEY \
            secret_access_key=$R2_SECRET_KEY \
            endpoint=https://${R2_ACCOUNT_ID}.r2.cloudflarestorage.com
          
          # Upload build
          rclone copy builds/WebGL r2:3dorbshooter/${{ github.sha }}

  deploy:
    needs: build
    runs-on: ubuntu-latest
    steps:
      - name: Deploy to Vercel
        run: |
          npm i -g vercel
          vercel --prod --token=${{ secrets.VERCEL_TOKEN }}
```

## 11. Development Tips for Linux

### Unity on Linux Specifics
1. **File Paths**: Always use `Path.Combine()` for cross-platform compatibility
2. **Case Sensitivity**: Linux filesystems are case-sensitive
3. **Permissions**: May need to `chmod +x` build scripts
4. **Graphics**: Ensure Vulkan drivers are updated

### Performance Optimization
```bash
# Before starting Unity
export UNITY_FORCE_VULKAN=1
export MESA_GL_VERSION_OVERRIDE=4.5

# Monitor performance
htop
nvidia-smi -l 1  # If using NVIDIA
```

### Troubleshooting
- **Unity crashes**: Check `~/.config/unity3d/Unity/Editor.log`
- **Build fails**: Ensure all Linux dependencies installed
- **Slow performance**: Disable compositor during development

## 12. Launch Checklist

- [ ] PLASMA token deployed on Solana devnet
- [ ] Ship NFT collection created
- [ ] Supabase schema migrated with ship tables
- [ ] n8n workflows configured for NFT minting
- [ ] Unity project builds without errors
- [ ] 3 ship tiers implemented and playable
- [ ] Ship upgrade system working
- [ ] NFT minting at level 10 functional
- [ ] WebGL build under 30MB
- [ ] 60 FPS with all ship types
- [ ] PLASMA transactions working
- [ ] Web2 signup creates wallet
- [ ] Web3 wallet connects properly
- [ ] Arena editor saves/loads configs
- [ ] AI uses all ship types effectively
- [ ] Leaderboards show ship statistics
- [ ] CI/CD pipeline green

---

*Plasma Siege Technical Implementation Guide v3.0 - Linux/Web3/NFT Edition*