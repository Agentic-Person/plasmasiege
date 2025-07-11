# Plasma Siege - Executive Summary

## Project Vision
Plasma Siege is a competitive, browser-based space arena game that bridges Web2 and Web3 gaming. Players pilot upgradeable spaceships in zero-gravity combat, battling to push an energy orb into the opponent's goal while collecting PLASMA tokens. The game features a unique progression system where ships can be minted as NFTs upon reaching level 10.

## Core Game Concept
- **Genre**: Physics-driven space combat arena
- **Platform**: WebGL (browser-based)
- **Players**: 1v1 (vs AI initially, PvP planned)
- **Objective**: Push plasma orb along dynamic spline path into opponent's goal
- **Match Duration**: 5 minutes or first to 3 goals

## Technology Stack
- **Game Engine**: Unity 6000.1.9f1 LTS with URP
- **Blockchain**: Solana (devnet → mainnet)
- **Backend**: Supabase (auth, database)
- **Automation**: n8n workflows
- **Development**: Claude Code with MCP integration
- **Platform**: Pop!_OS 22.04 LTS (Linux)

## Token Economy Overview

### PLASMA Token
- **Symbol**: PLASMA (8 decimals)
- **Network**: Solana
- **Initial Supply**: 1,000,000,000 PLASMA
- **Token Mint**: `3UDziHJzxc7yLthFFdXYwRTPYvGD5i5UW7EtcTndwuA7` (devnet)

### Economy Flow
- **New Player Bonus**: 1,000 PLASMA tokens
- **Match Entry Fee**: 10 PLASMA
- **Win Reward**: 20 PLASMA
- **Token Pickups**: 1-5 PLASMA per pickup
- **Ship Costs**:
  - Scout: Free (starter ship)
  - Fighter: 500 PLASMA
  - Destroyer: 2,000 PLASMA
- **NFT Minting**: 100 PLASMA (at level 10)

## Ship System

### Three Tiers
| Ship Type | Shield | Speed | Weapon Slots | Cost | Role |
|-----------|--------|-------|--------------|------|------|
| Scout | 100 | 80 | 1 | Free | Agile interceptor |
| Fighter | 200 | 60 | 2 | 500 PLASMA | Balanced all-rounder |
| Destroyer | 300 | 40 | 3 | 2,000 PLASMA | Heavy assault tank |

### Progression & NFTs
- Ships earn XP from matches (100 base, 200 for wins)
- 10 levels total (1,000 XP per level)
- At level 10, ships can be minted as NFTs
- NFTs preserve upgrades and match history
- Tradeable on secondary markets

### Upgrade System
Each ship has 9 modular upgrade slots:
- **3 Fuel Slots**: Boost capacity and regeneration
- **3 Shield Slots**: Defense and regeneration
- **3 Weapon Slots**: Damage output and abilities

## Development Timeline (8 Weeks)

### Phase 1: Foundation (Week 1) - CURRENT
- MCP server setup for Unity/n8n integration
- Unity project configuration
- Basic ship physics implementation
- N8N Docker deployment

### Phase 2: Core Systems (Weeks 2-3)
- 3-tier ship system
- Combat mechanics
- Plasma orb & spline physics

### Phase 3: Token Economy (Week 4)
- Solana integration
- PLASMA transactions
- Upgrade system

### Phase 4: NFT & Progression (Week 5)
- XP/leveling system
- NFT metadata generation
- Minting interface

### Phase 5: Arena & AI (Week 6)
- Dynamic arena editor
- AI opponent system
- Difficulty scaling

### Phase 6: Backend (Week 7)
- Supabase integration
- N8N workflow automation
- Data persistence

### Phase 7: Polish (Week 8)
- WebGL optimization
- Visual effects
- Performance tuning

## Current Status
- ✅ PLASMA token deployed on Solana devnet
- ✅ NFT collection created
- ✅ Comprehensive documentation complete
- ✅ Unity project started
- 🚧 MCP servers need configuration (highest priority)
- ⏳ Basic ship physics in development

## Key Performance Targets
- **Frame Rate**: 60 FPS on GTX 1060
- **Build Size**: < 30MB compressed
- **Load Time**: < 10 seconds
- **Physics**: Deterministic at 50Hz
- **Memory**: < 2KB GC per frame

## Unique Selling Points
1. **Hybrid Onboarding**: Email signup with free tokens OR direct wallet connection
2. **Ship Evolution**: In-game items become valuable NFTs through gameplay
3. **Dynamic Arenas**: Player-created spline paths for endless variety
4. **Skill-Based Economy**: Rewards tied to performance, not just participation
5. **Browser-Based**: No downloads, instant accessibility

## Success Metrics
- **Technical**: 60 FPS, <30MB build, deterministic physics
- **Engagement**: >20min average session, >40% D7 retention
- **Economic**: Balanced token flow, >30% NFT minting rate
- **Adoption**: Seamless Web2 onboarding, clear Web3 benefits

## Risk Mitigation
- **Performance**: Aggressive optimization, LOD systems
- **Blockchain**: Transaction queuing, optimistic UI
- **Economy**: Multiple token sinks, dynamic balancing
- **Adoption**: Free starter tokens, optional Web3 features

## Future Roadmap (Post-MVP)
- Real-time multiplayer PvP
- Tournament system with prize pools
- Ship marketplace for NFT trading
- Mobile app versions
- DAO governance for game updates
- Cross-game ship utility

## Investment Highlights
- **Low Barrier**: Browser-based, free-to-start
- **Clear Progression**: Ships → NFTs creates value
- **Proven Team**: Experienced in Web3 gaming
- **Technical Excellence**: Unity + Solana best practices
- **Market Timing**: Web3 gaming adoption accelerating

---

*Plasma Siege combines the instant accessibility of browser gaming with the economic opportunities of Web3, creating a competitive arena where skill translates directly into token rewards and NFT ownership.*