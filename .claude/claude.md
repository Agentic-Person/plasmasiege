# Plasma Siege - Claude Code Project Configuration

## Claude Rules
1. First think through the problem, read the codebase for relevant files, and write a plan to tasks/todo.md.
2. The plan should have a list of todo items that you can check off as you complete them
3. Before you begin working, check in with me and I will verify the plan.
4. Then, begin working on the todo items, marking them as complete as you go.
5. Please explain every step of the way just give me a high level explanation of what changes you made
6. Make every task and code change you do as simple as possible. We want to avoid making any massive or complex changes. Every change should impact as little code as possible. Everything is about simplicity.
7. Finally, add a review section to the todo.md file with a summary of the changes you made and any other relevant information.

## Security prompt:

Please check through all the code you just wrote and make sure it follows security best practices. make sure there are no sensitive information in the front and and there are no vulnerabilities that can be exploited

## Learning from Claude prompt:

Please explain the functionality and code you just built out in detail. Walk me through what you changed and how it works. Act like you’re a senior engineer teaching me code and create a task folder that will keep track of every task you build



## Project Overview
This is a Unity 3D competitive space arena game with PLASMA token economy and NFT ship system. Claude Code should reference the documentation files in `/docs/` for all implementation details.

## Critical Project Files
**ALWAYS consult these documents before making changes:**
- `/docs/plasma-siege-prd-final.md` - Product requirements and specifications
- `/docs/plasma-siege-tech-implementation-linux.md` - Technical implementation guide
- `/docs/plasma-siege-gdd-updated.md` - Game design specifications (v3.0)
- `/docs/claude-code-unity-instructions.md` - Step-by-step implementation guide
- `/docs/blockchain-setup-notes.md` - Solana deployment information

## Project Structure
```
/home/benton/projects/plasmasiegeprj/
├── plasmasiegeUnity/          # Unity project (Unity 6000.1.9f1 LTS)
│   ├── Assets/
│   │   ├── Scripts/
│   │   │   ├── Core/          # GameManager, PLASMAManager
│   │   │   ├── Ships/         # ShipBase, ShipController, tiers
│   │   │   ├── Arena/         # ArenaEditor, PlasmaOrb, Spline
│   │   │   ├── Economy/       # TokenPickup, UpgradeSystem
│   │   │   ├── AI/           # AIShipController
│   │   │   └── UI/           # Menus, HUD
│   │   ├── Prefabs/
│   │   │   ├── Ships/        # Scout, Fighter, Destroyer prefabs
│   │   │   ├── Weapons/      # Lasers, missiles
│   │   │   └── Pickups/      # Token pickups
│   │   └── ScriptableObjects/
│   │       ├── ShipConfigs/
│   │       └── UpgradeConfigs/
├── tools/
│   └── unity-mcp/            # MCP server
├── docs/                     # All documentation files
├── .claude/                  # This configuration
└── .env                      # Environment variables
```

## Development Priorities (8-Week Plan)
**Current Phase**: Phase 1 - Foundation & Infrastructure (Week 1)

### Phase 1: Foundation (Week 1) - IN PROGRESS 🚧
1. ❌ MCP server setup (Unity & N8N) - HIGHEST PRIORITY
2. ❌ Unity project configuration (6000.1.9f1 LTS)
3. ❌ Basic ship physics implementation
4. ❌ N8N Docker deployment

### Phase 2: Core Systems (Weeks 2-3) ⏳
1. Ship tier system (Scout/Fighter/Destroyer)
2. Combat mechanics (lasers, missiles)
3. Plasma orb & spline physics

### Phase 3: Token Economy (Week 4) ⏳
1. PLASMA transaction system
2. Economy features (entry fees, rewards)
3. Upgrade system (9 slots per ship)

### Phase 4: NFT & Progression (Week 5) ⏳
1. Ship leveling (XP system)
2. NFT preparation (level 10 minting)

### Phase 5: Arena & AI (Week 6) ⏳
1. Dynamic arena editor
2. AI opponent system

### Phase 6: Backend Integration (Week 7) ⏳
1. Supabase setup
2. N8N workflow automation

### Phase 7: Polish & Optimization (Week 8) ⏳
1. WebGL optimization (<30MB)
2. Visual & audio polish

## Key Technical Constraints
- **Unity Version**: 6000.1.9f1 LTS
- **Platform**: Pop!_OS 22.04 LTS (Linux)
- **Physics**: Deterministic at 50Hz fixed timestep (0.02)
- **Performance**: Must maintain 60 FPS on GTX 1060
- **Build Size**: < 30MB compressed WebGL
- **Token**: PLASMA (8 decimals) on Solana devnet

## Blockchain Configuration
- **Network**: Solana Devnet
- **RPC URL**: https://api.devnet.solana.com
- **PLASMA Token Mint**: `3UDziHJzxc7yLthFFdXYwRTPYvGD5i5UW7EtcTndwuA7`
- **NFT Collection Mint**: `AjK1Zqpd4FTupZN9yD6ETGwRUWKFAeJES2Zki7HWTezj`
- **Treasury Wallet**: `4VHP4LxZ9pxYSYxPashQjLknLWNMY8LUj6J8dH86jYeR`
- **Keypair Path**: `/home/benton/.config/solana/devnet.json`
- **Token Supply**: 1,000,000,000 PLASMA (initial)

## Ship Specifications
| Ship Type | Shield | Speed | Weapon Slots | Cost |
|-----------|--------|-------|--------------|------|
| Scout     | 100    | 80    | 1           | Free |
| Fighter   | 200    | 60    | 2           | 500 PLASMA |
| Destroyer | 300    | 40    | 3           | 2000 PLASMA |

## Token Economy
- **Starter Bonus**: 1,000 PLASMA
- **Match Entry**: 10 PLASMA
- **Win Reward**: 20 PLASMA
- **Token Pickups**: 1-5 PLASMA
- **NFT Minting**: 100 PLASMA (at ship level 10)

## Current Implementation Status

### Completed ✅
- [x] PLASMA token deployed on Solana devnet
- [x] NFT collection created
- [x] Comprehensive documentation
- [x] 8-week development plan
- [x] Executive summary created

### In Progress 🚧
- [ ] MCP Unity connection setup
- [ ] Unity project configuration

### Pending ⏳
- [ ] Basic ship movement (WASD + Mouse)
- [ ] Ship tier system (3 types)
- [ ] Plasma orb mechanics
- [ ] Token pickups and economy
- [ ] Arena editor
- [ ] AI opponent
- [ ] Ship upgrades (9 slots)
- [ ] XP/Level system (1-10)
- [ ] WebGL optimization

## Code Standards
1. **Physics**: All movement in FixedUpdate() using AddForce/AddTorque
2. **Pooling**: Object pools for all projectiles and effects
3. **Determinism**: No random values in physics calculations
4. **Performance**: Profile regularly, maintain <2KB GC per frame
5. **Naming**: PascalCase for classes, camelCase for variables

## Testing Requirements
Before marking any feature complete:
1. Test in Play mode at 60 FPS
2. Verify deterministic physics (replay must match)
3. Check memory allocation in Profiler
4. Test WebGL build performance
5. Validate token transactions

## Common Commands
```bash
# Start MCP server
cd /home/benton/projects/plasmasiegeprj/tools/unity-mcp
uv run server.py

# Build WebGL
/home/benton/projects/plasmasiegeprj/build-plasma-siege.sh

# Run tests
cd /home/benton/projects/plasmasiegeprj/plasmasiegeUnity
/opt/Unity/Editor/Unity -runTests -projectPath .

# Solana Commands
solana config set --url https://api.devnet.solana.com
solana airdrop 2  # Get test SOL
spl-token balance 3UDziHJzxc7yLthFFdXYwRTPYvGD5i5UW7EtcTndwuA7  # Check PLASMA balance
spl-token accounts  # List all token accounts
```

## Environment Variables
```bash
NEXT_PUBLIC_PLASMA_TOKEN_MINT=3UDziHJzxc7yLthFFdXYwRTPYvGD5i5UW7EtcTndwuA7
NEXT_PUBLIC_SHIP_COLLECTION_ADDRESS=AjK1Zqpd4FTupZN9yD6ETGwRUWKFAeJES2Zki7HWTezj
NEXT_PUBLIC_SOLANA_NETWORK=devnet
TREASURY_WALLET_ADDRESS=4VHP4LxZ9pxYSYxPashQjLknLWNMY8LUj6J8dH86jYeR
```

## Error Handling
If you encounter issues:
1. Check Unity Console for errors
2. Verify all required components are attached
3. Ensure physics settings match requirements
4. Confirm Input Manager has all axes configured
5. Check `/docs/claude-code-unity-instructions.md` troubleshooting section

## Important Notes
- **ALWAYS** check the documentation files before implementing features
- **NEVER** use transform manipulation for ship movement (use physics)
- **ALWAYS** test changes in Play mode before committing
- **MAINTAIN** 60 FPS performance target at all times
- Ship progression leads to NFT minting - this is core to the game economy

## Next Session Checklist
When starting a new Claude Code session:
1. Review this file and current implementation status
2. Check which stage we're on in the development priorities
3. Read relevant sections from documentation files
4. Test current build state before making changes
5. Update implementation status checkboxes as features are completed

## Recent Changes
- 2024-01-XX: Deployed PLASMA token on Solana devnet
- 2024-01-XX: Created ship NFT collection mint
- 2024-01-XX: Set up project structure and documentation
- 2025-07-07: Created comprehensive 8-week development plan
- 2025-07-07: Updated TODO.md with phase-based structure
- 2025-07-07: Created executive summary document
- 2025-07-07: Reorganized project priorities and milestones

## Recent Decisions
- Adopted 8-week phased development approach
- MCP server setup is highest priority before any Unity work
- Focus on iterative development with weekly milestones
- Ship NFT minting will occur at level 10 (not immediate)
- Web2 onboarding with email + free tokens is critical for adoption
