# Plasma Siege – Game Design Document (gamedesign.md)

## Executive Summary
Plasma Siege is a competitive, browser-based, physics-driven space arena game built in Unity 3D. Players pilot upgradeable spaceships in zero-gravity combat, battling to push an energy orb along a dynamic spline into the opponent's goal. The game features a **Web2/Web3 hybrid onboarding system** where traditional gamers can sign up with email and receive free starter PLASMA tokens, while crypto-native players can connect their wallets directly. Ships start as in-game items and can be minted as NFTs upon reaching level 10, creating a progression system that rewards skill and investment. Unity Model Context Protocol (MCP) plus Claude Code accelerate development throughout.

## 1. Vision & Target Audience
Fast, skill-based space combat meets competitive token economy with NFT ship progression. The game targets three audiences:
- **Casual Gamers**: Can start playing immediately with email signup, receiving 1,000 PLASMA starter tokens to learn the game
- **Competitive Players**: Can invest tokens to upgrade ships, unlock new tiers, and compete for larger reward pools
- **Crypto Enthusiasts**: Full Web3 features with self-custody wallets, NFT ship minting, and on-chain validation

## 2. Core Game Loop
1. **Entry**: Choose practice mode (free) or ranked match (10 PLASMA entry fee)
2. **Ship Selection**: Pick from Scout (starter), Fighter (500 PLASMA), or Destroyer (2,000 PLASMA)
3. **Spawn**: Enter arena with ship-specific stats and equipped upgrades
4. **Collect**: Gather token pickups (1-5 PLASMA) scattered in the arena
5. **Combat**: Use lasers and missiles to control the plasma orb along the spline
6. **Score**: Force orb through opponent's goal to score points
7. **Win**: First to 3 goals or highest score after 5 minutes wins and earns 20 PLASMA
8. **Progress**: Ships gain XP (100 per match, 200 for wins) and can be minted as NFTs at level 10

## 3. Ship System & NFTs

### 3.1 Ship Tiers
1. **Scout Ship**
   - Stats: 100 shield, 80 speed, 1 weapon slot
   - Cost: Free (starter ship)
   - Role: Agile interceptor, ideal for orb control
   - Strengths: High mobility, quick repositioning
   - Weaknesses: Low durability, limited firepower

2. **Fighter Ship**
   - Stats: 200 shield, 60 speed, 2 weapon slots
   - Cost: 500 PLASMA
   - Role: Balanced all-rounder
   - Strengths: Versatile loadout options, good survivability
   - Weaknesses: No standout advantages

3. **Destroyer Ship**
   - Stats: 300 shield, 40 speed, 3 weapon slots
   - Cost: 2,000 PLASMA
   - Role: Heavy assault tank
   - Strengths: Maximum firepower, high durability
   - Weaknesses: Slow movement, poor orb control

### 3.2 Ship Progression & NFT System
- **Experience System**: Ships earn XP from matches (100 base, 200 for wins)
- **Level Progression**: 10 levels total, requiring 1,000 XP per level
- **NFT Minting**: At level 10, ships can be minted as NFTs for 100 PLASMA
- **NFT Metadata**: Includes ship type, level, equipped upgrades, match history, and unique visual traits
- **Benefits of NFT Ships**:
  - Tradeable on secondary markets
  - Preserved upgrade configurations
  - Access to exclusive tournaments
  - Visual customization options
  - Legacy match statistics

### 3.3 Upgrade System
Each ship has 9 modular upgrade slots:
- **3 Fuel Slots**: Boost capacity and regeneration
- **3 Shield Slots**: Defense and regeneration rate
- **3 Weapon Slots**: Damage output and special abilities

**Upgrade Tiers & Costs**:
- **Tier 1**: 50/75/100 PLASMA (Fuel/Shield/Weapon)
- **Tier 2**: 100/150/200 PLASMA
- **Tier 3**: 200/300/400 PLASMA

Upgrades are visually represented on the ship model and persist when minted as NFT.

## 4. Gameplay Systems

### 4.1 Ship Flight & Combat
- **Movement**: 6-DoF arcade spaceflight with boost, drift, and stabilization
- **Primary Weapon**: Hitscan lasers with heat management
- **Secondary Weapon**: Projectile missiles with limited ammo
- **Collision**: Ships can ram the orb and each other for tactical plays
- **Physics**: Deterministic simulation at 50Hz for replay validation

### 4.2 Dynamic Arena System
- **Arena Editor GUI** (in-game tool):
  - Spline control: 3-8 adjustable points for orb path
  - Arena size: 300³ to 500³ units
  - Obstacle density: 0-50 asteroids with physics
  - Environmental hazards toggle (post-MVP)
  - Save/load arena configurations
  - Community sharing features
- **Procedural Generation**: Seed-based for competitive fairness
- **Arena Presets**: Official competitive layouts and community creations

### 4.3 Plasma Orb Mechanics
- **Movement**: Restricted to dynamic spline path
- **Physics**: Impulse forces projected onto path tangent
- **Visual States**: 
  - Neutral: White glow
  - Moving toward goal: Team-colored trail
  - High velocity: Intense particle effects
- **Tactical Elements**: Orb momentum carries between hits

### 4.4 PLASMA Token Economy
- **Initial Supply**: 1,000,000,000 PLASMA
- **New User Bonus**: 1,000 PLASMA (free starter tokens)
- **Match Economics**:
  - Entry Fee: 10 PLASMA for ranked matches
  - Win Reward: 20 PLASMA (2x multiplier)
  - Token Pickups: 1-5 PLASMA per pickup
  - Perfect Match Bonus: +5 PLASMA
  - Destroyed ships drop 50% of collected tokens
- **Ship Costs**:
  - Scout: Free
  - Fighter: 500 PLASMA
  - Destroyer: 2,000 PLASMA
- **NFT Minting**: 100 PLASMA fee at level 10

### 4.5 Resources & Pickups
- **PLASMA Tokens**: Instant currency when collected
- **Fuel Cells**: Restore boost capacity
- **Shield Batteries**: Repair and overcharge shields
- **Weapon Cores**: Temporary damage multipliers
- **Spawn Logic**: Deterministic positions based on match seed

### 4.6 Web2/Web3 Hybrid System
- **Web2 Path**:
  - Sign up with email or Google/Discord
  - Automatic custodial wallet creation
  - 1,000 PLASMA starter bonus
  - Play immediately without crypto knowledge
  - Optional wallet export later
- **Web3 Path**:
  - Connect existing wallet (Phantom, Solflare, Backpack)
  - Sign message for authentication
  - Use existing PLASMA balance
  - Full control of tokens and NFTs
  - Direct blockchain interactions

## 5. Game Modes

### 5.1 Practice Mode (Free)
- No entry fees or token rewards
- Test ship configurations and upgrades
- Experiment with arena editor
- AI opponents with adjustable difficulty
- Full access to all features except rewards

### 5.2 Ranked Matches (10 PLASMA Entry)
- Competitive token stakes
- Skill-based matchmaking
- Full reward potential
- Stats tracking and leaderboards
- Replay system for validation

### 5.3 Arena Workshop
- Create custom arena configurations
- Test with AI opponents
- Share designs with community
- Vote on favorites for official rotation
- Earn recognition for popular designs

### 5.4 Future Modes
- **PvP Multiplayer**: Real-time battles
- **Tournaments**: Scheduled events with prize pools
- **Ship Racing**: Speed circuits on custom splines
- **Survival Arena**: Wave-based PvE challenges
- **DAO Governance**: Token holder voting on features

## 6. AI & Procedural Features

### 6.1 AI Opponent System
- **Behavior States**: 
  - Collect tokens strategically
  - Control orb position
  - Engage in combat
  - Defend goal area
- **Difficulty Scaling**:
  - Accuracy: 30-70%
  - Reaction time: 1.0-0.3 seconds
  - Token awareness: low to high
  - Ship tier preference by difficulty
- **Ship Usage**: AI uses all three ship types with appropriate strategies

### 6.2 Procedural Arena Elements
- **Spline Generation**: Varied paths with consistent endpoints
- **Asteroid Fields**: Physics-enabled obstacles
- **Token Placement**: Balanced distribution algorithms
- **Difficulty Progression**: Complexity increases with player skill

## 7. Technology Stack
- **Unity 2023.3 LTS**: WebGL build with URP
- **Claude Code + MCP**: AI-assisted development
- **React 18 + TypeScript**: Frontend wrapper
- **Supabase**: Auth, database, custodial wallets
- **Solana + Thirdweb**: Blockchain integration
- **Cloudflare R2**: CDN for assets
- **n8n**: Automated workflows
- **Metaplex**: NFT standard for ships

## 8. Art & Audio Direction

### 8.1 Visual Style
- **Ships**: Distinct silhouettes for each tier with modular upgrade visuals
- **Arena**: Sci-fi aesthetic with holographic boundaries
- **Plasma Orb**: Pulsating energy sphere with team-colored effects
- **UI**: Clean interface with prominent PLASMA balance display
- **NFT Ships**: Special visual effects and customization options

### 8.2 Audio Design
- **Dynamic Soundtrack**: Intensity scales with match action
- **Ship Audio**: Unique engine sounds per tier
- **Combat SFX**: Impactful weapons with spatial audio
- **Token Feedback**: Satisfying collection sounds
- **UI Audio**: Clear feedback for transactions

## 9. User Experience Flow

### 9.1 First-Time User Journey
1. **Landing**: Choose Web2 (email) or Web3 (wallet) path
2. **Onboarding**: Receive 1,000 PLASMA starter tokens
3. **Tutorial**: Learn controls with Scout ship
4. **First Match**: Try practice mode (free)
5. **Progression**: Enter ranked match, earn XP
6. **Investment**: Purchase Fighter ship or upgrades

### 9.2 Returning Player Flow
1. **Quick Login**: Saved credentials or wallet
2. **Hangar**: View ships and upgrade options
3. **Shop**: Browse available upgrades
4. **Matchmaking**: Queue for appropriate tier
5. **Post-Match**: Check XP progress, consider NFT minting

## 10. Competitive Features

### 10.1 Ranking System
- **Ship-Specific Leaderboards**: Best pilots per ship tier
- **Overall Rankings**: Total PLASMA earned
- **Win Streaks**: Consecutive victory tracking
- **Arena Records**: Fastest goal times
- **NFT Showcase**: Display minted ships

### 10.2 Match Validation
- **Deterministic Replays**: Frame-perfect recreation
- **Checksum Verification**: On-chain validation
- **Dispute Resolution**: Community replay review
- **Anti-Cheat**: Input validation and physics bounds

## 11. Development Milestones

### Phase 0: Foundation (Week 1)
- [x] Deploy PLASMA token on Solana devnet
- [x] Unity project with MCP integration
- [x] Web2/Web3 authentication system
- [x] Basic ship movement physics

### Phase 1: Core Gameplay (Weeks 2-3)
- [ ] Implement 3 ship tiers with unique stats
- [ ] Create modular upgrade system
- [ ] Build dynamic arena editor
- [ ] Add token pickup mechanics

### Phase 2: Economy Integration (Week 4)
- [ ] PLASMA transaction system
- [ ] Ship purchasing flow
- [ ] Upgrade shop interface
- [ ] Match entry/reward distribution

### Phase 3: NFT System (Week 5)
- [ ] Ship XP and progression
- [ ] Level 10 NFT minting
- [ ] Metadata generation
- [ ] Wallet NFT display

### Phase 4: Polish & Beta (Weeks 6-8)
- [ ] AI opponent implementation
- [ ] Performance optimization
- [ ] Visual effects and audio
- [ ] Community beta testing

## 12. Success Metrics

### 12.1 Technical KPIs
- Frame rate: >95% at 60 FPS
- Load time: <10 seconds
- Transaction success: >99%
- Build size: <30MB compressed

### 12.2 Engagement KPIs
- D1 retention: >60%
- D7 retention: >40%
- D30 retention: >25%
- Avg session: >20 minutes
- Ship upgrade rate: >60%
- NFT minting rate: >30% of eligible

### 12.3 Economic KPIs
- Token velocity: >10% daily
- Healthy sink/faucet: ±5% monthly
- Ship tier distribution: 50/35/15%
- Average player investment: >500 PLASMA

## 13. Risk Mitigation

### 13.1 Technical Risks
- **WebGL Performance**: LOD system, aggressive culling
- **Blockchain Congestion**: Queue system, optimistic UI
- **Browser Compatibility**: Extensive testing matrix

### 13.2 Economic Risks
- **Token Inflation**: Multiple sinks, burning mechanisms
- **Bot Prevention**: CAPTCHA, rate limiting
- **Market Balance**: Dynamic pricing adjustments

### 13.3 Adoption Risks
- **Complexity**: Clear tutorials, tooltips
- **Entry Barrier**: Generous starter tokens
- **Learning Curve**: AI practice partners

## 14. Future Roadmap

### Post-Launch Features
1. **Real-time PvP**: WebRTC multiplayer
2. **Tournament System**: Scheduled competitions
3. **Ship Marketplace**: P2P NFT trading
4. **Custom Arenas**: Revenue sharing for creators
5. **Mobile Support**: Native iOS/Android
6. **DAO Governance**: Community decisions
7. **Staking Rewards**: Passive PLASMA income
8. **Cross-Game Ships**: Partner integrations

## 15. Glossary
- **PLASMA**: The game's native token (8 decimals)
- **Ship Tiers**: Scout, Fighter, Destroyer
- **Custodial Wallet**: Supabase-managed for Web2 users
- **MCP**: Model Context Protocol for Claude integration
- **Deterministic Physics**: Reproducible for fairness
- **Spline**: Dynamic curved path for plasma orb

---

*Plasma Siege Game Design Document v3.0 — Updated with NFT Ships and Refined Token Economy*