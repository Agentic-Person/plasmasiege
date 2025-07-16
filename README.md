# Plasma Siege

A 3D spaceship combat game built in Unity with blockchain integration, featuring PLASMA token economy and NFT ship progression.

## 🚀 Project Overview

**Plasma Siege** is an innovative 3D space combat game where players control spaceships in arena battles, earning PLASMA tokens through gameplay. The game features a unique token economy, NFT-based ship progression, and competitive multiplayer combat.

### Key Features
- **6DOF Spaceship Physics**: Full 3D movement and combat
- **Token Economy**: Earn and spend PLASMA tokens through gameplay
- **NFT Ship Progression**: Three ship tiers (Scout, Fighter, Destroyer) with upgrades
- **Arena Combat**: Structured battle environments with objectives
- **Blockchain Integration**: Solana-based token and NFT systems

## 📊 Current Status (Week 1 of 8)

### ✅ Completed
- **Project Planning**: Comprehensive 8-week development roadmap
- **MCP Servers**: Unity MCP server working and connected
- **Ship Physics**: 6DOF movement system functional
- **Arena Scripts**: Boundary system and environment generation
- **Ship Assets**: Striker ship model imported with textures

### 🔶 In Progress - Critical Issues Found
- **Arena System**: Scripts created but needs integration and scaling
- **Mouse Controls**: Critical issue preventing proper ship control

### 🚨 Critical Issues to Fix (Next Session)
1. **Mouse Control Broken**: Cursor must leave Unity window to turn ship
2. **Arena Too Small**: Current arena needs to be 5x larger (500x300x500)
3. **Ship Model**: Need to implement imported Striker.fbx for better orientation
4. **Arena Integration**: Scripts created but not added to Unity scene

## 🛠️ Technology Stack

### Game Development
- **Unity 6000.1.9f1 LTS**: Game engine
- **C#**: Primary programming language
- **6DOF Physics**: Custom spaceship movement system

### Blockchain Integration
- **Solana**: Blockchain platform
- **PLASMA Token**: In-game currency
- **Metaplex**: NFT framework for ship progression

### Development Tools
- **MCP Servers**: Unity automation and workflow integration
- **N8N**: Workflow automation
- **Supabase**: Backend database
- **Git**: Version control

## 🎮 Game Design

### Ship System
- **Scout**: Fast, agile, basic weapons (50 PLASMA)
- **Fighter**: Balanced stats, better weapons (100 PLASMA) 
- **Destroyer**: Heavy, powerful, slow (200 PLASMA)

### Token Economy
- **Entry Fee**: 10 PLASMA per match
- **Win Reward**: 20 PLASMA
- **Token Pickups**: 1-5 PLASMA during matches
- **Upgrades**: 9 upgrade slots per ship (fuel, shields, weapons)

### Arena Combat
- **Objective-Based**: Control plasma orbs, score goals
- **Environmental Hazards**: Asteroids and obstacles
- **Spatial Awareness**: Clear up/down/orientation markers

## 📁 Project Structure

```
plasmasiegeprj/
├── plasmasiegeUnity/          # Unity project
│   ├── Assets/
│   │   ├── FlyingShipFixed.cs      # Ship physics controller
│   │   ├── ArenaGenerator.cs       # Arena environment system
│   │   ├── ArenaInfoGUI.cs         # Arena information display
│   │   ├── Striker.fbx             # Ship 3D model
│   │   └── Striker_Orange.png      # Ship texture
├── tools/
│   ├── unity-mcp/             # Unity MCP server
│   └── n8n-mcp/              # N8N automation server
├── docs/                      # Comprehensive documentation
├── tasks/                     # Detailed task logs
└── TODO.md                    # Current development roadmap
```

## 🚀 Getting Started

### Prerequisites
- Unity 6000.1.9f1 LTS
- Python 3.10+ (for MCP servers)
- Git

### Quick Setup
1. **Clone the repository**:
   ```bash
   git clone https://github.com/Agentic-Person/plasmasiege.git
   cd plasmasiegeprj
   ```

2. **Start MCP servers**:
   ```bash
   ./start-all-services.sh
   ```

3. **Open Unity project**:
   - Open Unity Hub
   - Add project: `plasmasiegeprj/plasmasiegeUnity`
   - Open project

### Current Testing
- **Scene**: `shiptester_001.unity`
- **Ship Controls**: WASD + Mouse (mouse control issue exists)
- **Arena**: Scripts exist but need integration

## 🐛 Known Issues

### Critical (Blocking Development)
1. **Mouse Control**: Cursor must leave Unity window to turn ship properly
2. **Arena Scale**: Too small for proper flight testing

### Secondary
3. **Ship Model**: Need to implement Striker.fbx for visual orientation
4. **Arena Integration**: Scripts created but not in scene

## 📋 Development Roadmap

### Phase 1: Foundation (Week 1) - 🔶 IN PROGRESS
- ✅ MCP server setup
- 🔶 Ship physics and arena (critical issues found)
- ⏳ Mouse control fix
- ⏳ Arena integration

### Phase 2: Core Systems (Weeks 2-3)
- Ship tier implementation
- Combat and weapons
- Plasma orb mechanics

### Phase 3: Token Economy (Week 4)
- PLASMA token integration
- Transaction system
- Economy features

### Phases 4-7: Features & Polish (Weeks 5-8)
- NFT progression
- Arena AI
- Backend integration
- Final polish

## 📝 Documentation

- **[Tasks](tasks/README.md)**: Detailed task logs and progress
- **[TODO](TODO.md)**: Current development priorities
- **[Docs](docs/)**: Comprehensive project documentation

## 🤝 Contributing

This is an active development project. Current focus is on resolving critical control issues and arena integration.

## 📄 License

[License information to be added]

---

**Repository**: https://github.com/Agentic-Person/plasmasiege  
**Last Updated**: January 20, 2025  
**Status**: Week 1 Development - Critical Issues Phase 