# Plasma Siege TODO List

## Current Status
- ✅ **Project Planning**: Comprehensive 8-week development plan created
- ✅ **Blockchain**: PLASMA token and NFT collection deployed on Solana devnet
- 🆘 **MCP SERVERS**: Must be set up first (HIGHEST PRIORITY)
- 🚧 **Phase 1**: Foundation & Infrastructure (Week 1 - IN PROGRESS)
- ⏳ **Phases 2-7**: Core systems, economy, NFTs, polish (Weeks 2-8)

## 🎯 8-Week Development Plan

### Phase 1: Foundation & Infrastructure (Week 1) 🚧
**Goal**: Set up development environment and basic ship physics

#### 1.1 MCP Server Setup (HIGHEST PRIORITY) ✅
- [x] Run `/home/benton/projects/plasmasiegeprj/setup-all-mcp.sh`
- [x] Test with `/home/benton/projects/plasmasiegeprj/test-mcp-setup.sh`
- [ ] Restart Cursor to load MCP configuration
- [ ] Verify Unity MCP can create scripts
- [ ] Verify N8N MCP can create workflows

#### 1.2 Unity Project Configuration ❌
- [ ] Configure Unity 6000.1.9f1 LTS project settings
- [ ] Set Fixed Timestep to 0.02 (50Hz physics)
- [ ] Configure Input Manager with all axes
- [ ] Set up layers: Ship, Boundary, Plasma, Pickup, Projectile
- [ ] Configure tags: Player, Ship, PlasmaOrb, Boundary, Pickup
- [ ] Import existing scripts from Assets folder

#### 1.3 Complete Stage 1: Ship Physics ❌
- [ ] Create Scout ship prefab with Rigidbody
- [ ] Attach and test ShipController.cs
- [ ] Implement 6DOF movement (WASD + Mouse)
- [ ] Create arena with boundaries
- [ ] Test HolographicShipTester UI
- [ ] Ensure 60 FPS performance

#### 1.4 N8N Docker Setup ❌
- [ ] Install Docker if needed
- [ ] Run n8n container with configuration
- [ ] Set up .env file with credentials
- [ ] Import workflow templates
- [ ] Test webhook endpoints with cURL
- [ ] Verify Supabase connection

### Phase 2: Core Game Systems (Weeks 2-3) ⏳
**Goal**: Implement ship tiers, combat, and plasma orb mechanics

#### 2.1 Ship Tier System ❌
- [ ] Create Fighter ship prefab (200 shield, 60 speed, 2 weapons)
- [ ] Create Destroyer ship prefab (300 shield, 40 speed, 3 weapons)
- [ ] Implement ship stats and differences
- [ ] Add ship purchasing with PLASMA tokens
- [ ] Create ship selection UI
- [ ] Test all three ship types

#### 2.2 Combat & Weapons ❌
- [ ] Implement laser weapons (hitscan)
- [ ] Add missile projectiles
- [ ] Create object pooling system
- [ ] Add damage and shield systems
- [ ] Implement weapon visual effects
- [ ] Test combat mechanics

#### 2.3 Plasma Orb & Spline System ❌
- [ ] Create spline path system
- [ ] Implement plasma orb physics
- [ ] Add force application from weapons
- [ ] Create goal scoring mechanics
- [ ] Add visual effects for orb states
- [ ] Test orb movement along spline

### Phase 3: Token Economy Integration (Week 4) ⏳
**Goal**: Connect PLASMA tokens and implement economy features

#### 3.1 PLASMA Transaction System ❌
- [ ] Connect to Solana devnet
- [ ] Implement wallet integration
- [ ] Add token balance display
- [ ] Create transaction manager
- [ ] Test token transfers
- [ ] Add error handling

#### 3.2 Economy Features ❌
- [ ] Implement entry fee system (10 PLASMA)
- [ ] Add win rewards (20 PLASMA)
- [ ] Create token pickup mechanics (1-5 PLASMA)
- [ ] Implement ship purchase flow
- [ ] Add transaction confirmations
- [ ] Test full economy loop

#### 3.3 Upgrade System ❌
- [ ] Create 9 upgrade slots per ship
- [ ] Implement fuel upgrades (3 tiers)
- [ ] Add shield upgrades (3 tiers)
- [ ] Create weapon upgrades (3 tiers)
- [ ] Build upgrade shop UI
- [ ] Add visual upgrade indicators

### Phase 4: NFT & Progression (Week 5) ⏳
**Goal**: Add ship leveling and NFT preparation

#### 4.1 Ship Leveling ❌
- [ ] Implement XP system
- [ ] Add level progression (1-10)
- [ ] Create experience UI
- [ ] Add level-up effects
- [ ] Store progression data
- [ ] Test XP calculations

#### 4.2 NFT Preparation ❌
- [ ] Create metadata generation
- [ ] Add level 10 unlock check
- [ ] Build minting UI
- [ ] Implement 100 PLASMA minting cost
- [ ] Connect to NFT collection
- [ ] Test metadata creation

### Phase 5: Arena & AI (Week 6) ⏳
**Goal**: Dynamic arenas and intelligent opponents

#### 5.1 Dynamic Arena Editor ❌
- [ ] Create in-game spline editor
- [ ] Add obstacle placement
- [ ] Implement arena size adjustment
- [ ] Create save/load system
- [ ] Add preset configurations
- [ ] Test arena variations

#### 5.2 AI Opponent ❌
- [ ] Create AI state machine
- [ ] Implement behavior states
- [ ] Add difficulty scaling
- [ ] Create token collection logic
- [ ] Test AI with all ship types
- [ ] Balance AI difficulty

### Phase 6: Backend Integration (Week 7) ⏳
**Goal**: Connect all systems to backend

#### 6.1 Supabase Setup ❌
- [ ] Implement database schema
- [ ] Create authentication system
- [ ] Add player stats tracking
- [ ] Store match history
- [ ] Set up RLS policies
- [ ] Test data persistence

#### 6.2 N8N Workflows ❌
- [ ] Connect Unity to N8N webhooks
- [ ] Implement match result processing
- [ ] Add token distribution automation
- [ ] Create NFT minting queue
- [ ] Set up daily rewards
- [ ] Test all workflows

### Phase 7: Polish & Optimization (Week 8) ⏳
**Goal**: Optimize for WebGL and add polish

#### 7.1 WebGL Optimization ❌
- [ ] Optimize for <30MB build size
- [ ] Ensure 60 FPS on GTX 1060
- [ ] Implement LOD system
- [ ] Optimize shaders
- [ ] Add loading screens
- [ ] Test on multiple browsers

#### 7.2 Visual & Audio Polish ❌
- [ ] Add particle effects
- [ ] Implement UI animations
- [ ] Create sound effects
- [ ] Add background music
- [ ] Polish visual feedback
- [ ] Final performance pass

## Immediate Tasks (High Priority) - Detailed

### 0. MCP SERVERS SETUP (HIGHEST PRIORITY)
**Purpose**: Enable Unity and N8N integration with Claude for automated development

- [ ] **Run Complete MCP Setup**
  ```bash
  cd /home/benton/projects/plasmasiegeprj/plasmasiege
  ./setup-all-mcp.sh
  ```
  This installs:
  - Unity MCP Server (Python-based)
  - N8N MCP Server (npm package)
  - All dependencies and virtual environments
  - Startup and testing scripts

- [ ] **Start All Services**
  ```bash
  /home/benton/projects/plasmasiegeprj/start-all-services.sh
  ```
  This starts:
  - N8N Docker container at http://localhost:5678
  - Sets up MCP server connections
  - Creates all necessary directories

- [ ] **Restart Cursor (CRITICAL)**
  - Close Cursor completely (all windows)
  - Reopen Cursor
  - MCP servers should auto-connect
  - Look for MCP indicators in Cursor

- [ ] **Verify MCP Setup**
  ```bash
  /home/benton/projects/plasmasiegeprj/test-mcp-setup.sh
  ```
  This checks:
  - Unity MCP server files exist
  - N8N MCP package installed
  - Python virtual environment working
  - MCP configuration valid

- [ ] **Test MCP Integration**
  - Try asking Claude to create a Unity script
  - Try asking Claude to create an N8N workflow
  - Verify Unity project path is accessible
  - Check N8N webhook connectivity

### 1. Complete Stage 1 Implementation
**Purpose**: Finalize the foundation for all future development

- [ ] **Create Unity Project First**
  - Create new Unity 6000.1.9f1 LTS project named "PlasmaSiege"
  - Use "Universal 3D" template (includes Universal Render Pipeline)
  - Enable legacy Input Manager in Player Settings
  - Import all scripts from Assets/Scripts/ directory
  - Set up folder structure to match existing layout

- [ ] **Configure Unity Project Settings**
  - Set Fixed Timestep to 0.02 (Edit > Project Settings > Time)
  - Configure layers: Ship, Boundary, Plasma, Pickup, Projectile
  - Configure tags: Player, Ship, PlasmaOrb, Boundary, Pickup
  - Set up Input Manager axes as per InputAxisSetup.cs
  - Configure WebGL build settings

- [ ] **Create Game Scene**
  - Create new scene "GameArena.unity"
  - Add GameObject "GameManager" with ArenaSetup.cs
  - Let ArenaSetup create arena boundaries and lighting
  - Add UI Canvas for GameUIManager

- [ ] **Create Ship Prefab**
  - Create basic ship GameObject (use cube for now)
  - Add Rigidbody component
  - Attach ShipController.cs
  - Configure ship physics settings
  - Save as prefab "ScoutShip.prefab"

- [ ] **Test HolographicShipTester.cs in Unity**
  - Attach HolographicShipTester to UI GameObject
  - Ensure GameUIManager singleton is initialized first
  - Verify circular speed gauge responds to drag input
  - Check boost energy bar fills/depletes correctly
  - Confirm rotation indicator spins based on turn rate

- [ ] **Verify ship physics with new holographic UI**
  - Test WASD movement with visual speed gauge feedback
  - Confirm boost mechanic depletes fuel bar in real-time
  - Verify turn rate adjustment affects ship rotation speed
  - Ensure "Reset Ship" button returns ship to origin (0,0,0)
  - Check that physics values update ship behavior immediately

- [ ] **Ensure 60 FPS with visual effects**
  - Monitor FPS counter in holographic UI
  - Profile any performance issues with Unity Profiler
  - Optimize holographic shader effects if needed
  - Test with multiple UI panels open simultaneously
  - Verify no memory leaks from UI animations

- [ ] **Test boundary collision and bounce mechanics**
  - Fly ship into all 6 arena boundaries (top, bottom, 4 sides)
  - Verify bounce direction is correct (reflection angle)
  - Check bounce force feels natural
  - Ensure ship doesn't clip through boundaries at high speed
  - Test corner collisions for proper behavior

### 2. MCP Servers Setup (NEW - PRIORITY)
**Purpose**: Enable Unity and N8N integration with Claude

- [ ] **Set up ALL MCP servers**
  ```bash
  cd /home/benton/projects/plasmasiegeprj/plasmasiege
  ./setup-all-mcp.sh
  ```

- [ ] **Start all services**
  ```bash
  /home/benton/projects/plasmasiegeprj/start-all-services.sh
  ```

- [ ] **Restart Cursor to load MCP configuration**
  - Close Cursor completely
  - Reopen Cursor 
  - MCP servers should auto-connect

- [ ] **Test MCP setup**
  ```bash
  /home/benton/projects/plasmasiegeprj/test-mcp-setup.sh
  ```

### 3. N8N Docker Setup
**Purpose**: Enable automated backend workflows for token economy

- [ ] **Install Docker on development machine** (handled by MCP setup)
  ```bash
  # For Pop!_OS / Ubuntu:
  sudo apt update
  sudo apt install docker.io docker-compose
  sudo usermod -aG docker $USER
  # Log out and back in for group change
  ```

- [ ] **Run n8n container with provided configuration**
  ```bash
  # Use the exact command from N8N_WORKFLOW_DOCUMENTATION.md
  docker run -d \
    --name n8n-plasmasiege \
    --restart unless-stopped \
    -p 5678:5678 \
    -e N8N_BASIC_AUTH_ACTIVE=true \
    -e N8N_BASIC_AUTH_USER=admin \
    -e N8N_BASIC_AUTH_PASSWORD=plasmasiege2024 \
    -e N8N_ENCRYPTION_KEY=$(openssl rand -hex 32) \
    -v ~/.n8n:/home/node/.n8n \
    n8nio/n8n
  ```

- [ ] **Set up environment variables (.env file)**
  - Copy template from N8N_WORKFLOW_DOCUMENTATION.md
  - Fill in your Supabase project URL and keys
  - Add Solana devnet configuration
  - Set webhook secret for Unity communication
  - Place .env in ~/.n8n/ directory

- [ ] **Test webhook endpoints with cURL**
  - Access n8n at http://localhost:5678
  - Login with admin/plasmasiege2024
  - Import workflow templates from documentation
  - Test each webhook with provided cURL examples
  - Verify responses return success status

- [ ] **Verify Supabase connection from n8n**
  - Create credentials in n8n for Supabase
  - Test connection with a simple query
  - Ensure RLS policies allow service role access
  - Check that tables exist (or create them)

### 3. Unity-N8N Integration (After MCP Setup)
**Purpose**: Connect game events to backend automation

- [ ] **Use MCP to implement N8NWebhookManager.cs in Unity**
  - Ask Claude (with Unity MCP) to create script in Unity project
  - Automatic integration with Blockchain folder
  - MCP handles singleton pattern correctly
  - Set webhook base URL to http://localhost:5678/webhook/
  - Configure webhook secret to match n8n .env

- [ ] **Use MCP to create mock match data for testing**
  - Ask Claude to build test scene with Unity MCP
  - MCP creates GameObjects and components directly
  - Generate realistic match data:
    * Match ID (use System.Guid.NewGuid())
    * Winner/Loser IDs and wallet addresses
    * Plasma collected (random 10-50)
    * Match checksum (simple hash)

- [ ] **Test match-complete webhook via MCP**
  - Use Unity MCP to run test scenarios
  - MCP monitors Unity console automatically
  - Check n8n execution history for received webhook
  - Verify workflow processes successfully
  - Confirm data saved to Supabase (if connected)

- [ ] **Use N8N MCP to verify token distribution workflow**
  - N8N MCP can create/modify workflows directly
  - Test workflow execution from Claude
  - Monitor token calculations in real-time
  - Check winner receives 20 PLASMA reward
  - Verify loser pays 10 PLASMA entry fee

- [ ] **MCP-assisted retry logic implementation**
  - Unity MCP implements retry patterns
  - N8N MCP configures error handling workflows
  - Automatic exponential backoff (2s, 4s, 8s)
  - Maximum 3 retry attempts
  - Queue failed webhooks for later retry

## Key Milestones

- **Week 1**: MCP setup, Unity configuration, basic ship physics
- **Week 2-3**: Ship tiers, combat system, plasma orb mechanics
- **Week 4**: PLASMA token integration, economy features
- **Week 5**: Ship progression, NFT preparation
- **Week 6**: Dynamic arena editor, AI opponent
- **Week 7**: Backend integration, automated workflows
- **Week 8**: WebGL optimization, final polish

## Success Criteria

### Technical Performance
- [ ] 60 FPS stable on GTX 1060
- [ ] < 30MB compressed WebGL build
- [ ] < 10 second initial load time
- [ ] < 2KB GC allocation per frame
- [ ] 50Hz deterministic physics

### Game Features
- [ ] 3 playable ship tiers with unique stats
- [ ] Full PLASMA token economy working
- [ ] Ships can reach level 10 and become NFT-ready
- [ ] Dynamic arena editor functional
- [ ] AI opponent provides appropriate challenge

### Integration
- [ ] MCP servers connected and functional
- [ ] N8N workflows processing match results
- [ ] Solana devnet transactions working
- [ ] Web2/Web3 authentication complete

## Known Issues
- [ ] MCP servers not yet configured
- [ ] Unity project needs initial setup
- [ ] Missing Input Manager configuration
- [ ] No Supabase schema implemented yet

## Quick Commands Reference
```bash
# MCP Setup
cd /home/benton/projects/plasmasiegeprj/plasmasiege
./setup-all-mcp.sh

# Start Services
/home/benton/projects/plasmasiegeprj/start-all-services.sh

# Test MCP
/home/benton/projects/plasmasiegeprj/test-mcp-setup.sh

# Check PLASMA Balance
spl-token balance 3UDziHJzxc7yLthFFdXYwRTPYvGD5i5UW7EtcTndwuA7
```

---

**Last Updated:** 2025-07-07
**Current Phase:** Phase 1 - Foundation & Infrastructure (Week 1)
**Next Milestone:** Complete MCP setup and basic ship physics

## Quick Links
- [Executive Summary](docs/executive-summary.md)
- [Game Design Doc](docs/plasma-siege-gdd-updated.md)
- [Product Requirements](docs/plasma-siege-prd-final.md)
- [Technical Implementation](docs/plasma-seige-tech-implementation-linux.md)
- [Unity Instructions](docs/claude-code-unity-complete.md)
- [N8N Documentation](N8N_WORKFLOW_DOCUMENTATION.md)
- [Claude Config](.claude/claude.md)