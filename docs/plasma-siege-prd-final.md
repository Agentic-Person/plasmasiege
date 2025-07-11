# Plasma Siege — Product Requirements Document (PRD)

## 1. Product Overview

**Plasma Siege** is a competitive, browser-based, physics-driven space arena game built in Unity 3D. Players pilot upgradeable spaceships in zero-gravity combat, battling to push an energy orb along a dynamic spline into the opponent's goal. The game features a **Web2/Web3 hybrid onboarding system** where traditional gamers can sign up with email and receive free starter PLASMA tokens, while crypto-native players can connect their wallets directly. Ships start as in-game items and can be minted as NFTs upon reaching level 10, creating a progression system that rewards skill and investment.

## 2. Goals & Success Criteria

- **Deliver a WebGL browser-playable MVP** with deterministic physics, dynamic arena editor, and engaging solo vs. AI gameplay
- **Seamless Web2/Web3 integration**: Email signup with auto-wallet creation OR direct wallet connection for existing crypto users
- **NFT Ship System**: 3 tiers of ships that become mintable NFTs at level 10, with modular upgrade slots
- **PLASMA Token Economy**: All in-game actions (entry fees, ship purchases, upgrades, rewards) powered by PLASMA tokens
- **Dynamic Arena System**: In-game editor for testing various spline configurations and obstacle layouts
- **Performance**: 60 FPS on mid-range hardware (GTX 1060), <10s load time, ≤1% replay desync
- **Fair Competition**: Deterministic physics and on-chain validation ensure transparent, verifiable matches

## 3. Core Features & Requirements

### 3.1 Tokenomics & Blockchain

#### PLASMA Token
- **Symbol**: PLASMA
- **Network**: Solana (Devnet → Mainnet)
- **Decimals**: 8
- **Initial Supply**: 1,000,000,000 PLASMA
- **Distribution**:
  - New User Bonus: 1,000 PLASMA (free starter tokens)
  - Treasury: 40% (rewards, development)
  - Liquidity: 30% (DEX provision)
  - Team: 20% (vested over 2 years)
  - Marketing: 10%

#### Token Economy
- **Match Entry**: 10 PLASMA for ranked matches (practice mode free)
- **Win Rewards**: 20 PLASMA (2x multiplier on entry fee)
- **Token Pickups**: 1-5 PLASMA scattered in arena during matches
- **Ship Costs**:
  - Scout: Free (starter ship)
  - Fighter: 500 PLASMA
  - Destroyer: 2,000 PLASMA
- **Upgrade Costs**:
  - Fuel Boosters: 50/100/200 PLASMA (Tier 1/2/3)
  - Shield Modules: 75/150/300 PLASMA
  - Weapon Systems: 100/200/400 PLASMA
- **NFT Minting**: 100 PLASMA to mint ship as NFT (requires level 10)

### 3.2 Ship System & NFTs

#### Ship Tiers
1. **Scout Ship**
   - Stats: 100 shield, 80 speed, 1 weapon slot
   - Cost: Free (starter ship)
   - Agile but fragile

2. **Fighter Ship**
   - Stats: 200 shield, 60 speed, 2 weapon slots
   - Cost: 500 PLASMA
   - Balanced offense and defense

3. **Destroyer Ship**
   - Stats: 300 shield, 40 speed, 3 weapon slots
   - Cost: 2,000 PLASMA
   - Tank class with heavy firepower

#### Ship Progression
- Ships gain XP from matches (100 XP per match, 200 for wins)
- Level 1-10 progression (1,000 XP per level)
- At level 10, ships can be minted as NFTs
- NFT metadata includes: ship type, level, equipped upgrades, match history

#### Upgrade System
- Each ship has 9 upgrade slots (3 fuel, 3 shield, 3 weapon)
- Upgrades are modular and interchangeable
- Visual changes reflect equipped upgrades
- Upgrades persist when ship is minted as NFT

### 3.3 Gameplay & Arena

#### Core Mechanics
- **Movement**: 6-DoF arcade spaceflight with boost, drift, and stabilization
- **Combat**: Primary lasers (hitscan) and secondary missiles (projectile)
- **Objective**: Push the plasma orb along the spline into opponent's goal
- **Resources**: Fuel (boost), shields (defense), ammo (offense)
- **Match Format**: First to 3 goals or highest score after 5 minutes

#### Dynamic Arena System
- **Arena Editor GUI** (in-game tool):
  - Spline control: 3-8 adjustable points
  - Arena size: 300³ to 500³ units
  - Obstacle density: 0-50 asteroids
  - Environmental hazards toggle
  - Save/load arena configurations
- **Procedural Generation**: Seed-based for competitive fairness
- **Arena Presets**: Community-created and official maps

#### Token Integration
- All matches require PLASMA entry (except practice mode)
- Token pickups spawn during matches at deterministic locations
- Destroyed ships drop 50% of collected tokens
- Perfect match bonus: +5 PLASMA

### 3.4 User Onboarding

#### Web2 Path (Email/Social)
1. Sign up with email or Google/Discord
2. Automatic custodial wallet creation (via Supabase)
3. Receive 1,000 PLASMA starter tokens
4. Play immediately without crypto knowledge
5. Optional: Export wallet to self-custody later

#### Web3 Path (Wallet Connect)
1. Connect existing wallet (Phantom, Solflare, Backpack)
2. Sign message for authentication
3. Link to game profile
4. Use existing PLASMA balance
5. Full control of tokens and NFTs

### 3.5 Backend & Infrastructure

#### Supabase Services
- **Authentication**: Email/social login + wallet signatures
- **Database**: Player profiles, ship data, match history
- **Wallet Management**: Encrypted key storage for custodial wallets
- **Real-time**: Live leaderboards and match updates

#### Blockchain Integration
- **Solana RPC**: Helius for reliable devnet/mainnet access
- **Token Operations**: Transfer, balance checks, transaction history
- **NFT Minting**: Metaplex standard for ship NFTs
- **Match Validation**: Checksums stored on-chain

#### Automation (n8n)
- New user wallet creation and token distribution
- Match completion and reward distribution
- NFT minting queue processing
- Tournament bracket management
- Analytics and suspicious activity monitoring

## 4. Technical Requirements

### 4.1 Technology Stack
- **Game Engine**: Unity 2023.3 LTS (URP, WebGL 2.0)
- **Development**: Claude Code via Unity MCP Server
- **Frontend**: React 18 + TypeScript + Tailwind CSS
- **Backend**: Supabase (PostgreSQL + Edge Functions)
- **Blockchain**: Solana + Thirdweb Unity SDK
- **CDN**: Cloudflare R2
- **Hosting**: Vercel (React) + CDN (Unity WebGL)

### 4.2 Performance Targets
- **Frame Rate**: 60 FPS stable on GTX 1060
- **Load Time**: <10 seconds on 50 Mbps connection
- **Build Size**: <30MB compressed
- **Memory Usage**: <1.5GB in browser
- **Network Latency**: <100ms for gameplay
- **Transaction Time**: <5 seconds for blockchain operations

### 4.3 Platform Support
- **Browsers**: Chrome 90+, Firefox 88+, Edge 90+, Safari 15+
- **Operating Systems**: Windows 10+, macOS 11+, Linux (Ubuntu 20.04+)
- **Development OS**: Pop!_OS 22.04 LTS (primary), cross-platform compatible
- **Mobile**: Not supported in MVP (future roadmap)

## 5. Security & Compliance

### 5.1 Game Security
- **Deterministic Physics**: All matches can be replayed and verified
- **Server Validation**: Critical game state verified server-side
- **Anti-Cheat**: Input rate limiting, physics bounds checking
- **Replay System**: Full match recording for dispute resolution

### 5.2 Blockchain Security
- **Wallet Security**: Custodial keys encrypted with Supabase Vault
- **Transaction Validation**: All token operations require signatures
- **Smart Contract Audits**: Before mainnet deployment
- **Rate Limiting**: Max 10 transactions per minute per wallet

### 5.3 Data Protection
- **User Privacy**: GDPR-compliant data handling
- **Secure Communications**: HTTPS/WSS only
- **Access Control**: Row-level security on all user data
- **Audit Logs**: All token and NFT movements tracked

## 6. Development Roadmap

### Phase 0: Foundation (Week 1)
- [ ] Deploy PLASMA token on Solana devnet
- [ ] Set up Unity project with MCP integration
- [ ] Implement Web2/Web3 authentication
- [ ] Create basic ship movement and physics

### Phase 1: Core Gameplay (Week 2-3)
- [ ] Implement 3 ship tiers with unique stats
- [ ] Create upgrade system with 9 slots per ship
- [ ] Build dynamic arena editor
- [ ] Add token pickups and combat mechanics

### Phase 2: Economy Integration (Week 4)
- [ ] Integrate PLASMA token transactions
- [ ] Implement ship purchasing system
- [ ] Add upgrade shop with token costs
- [ ] Create match entry and reward flow

### Phase 3: NFT System (Week 5)
- [ ] Ship progression and XP system
- [ ] NFT minting at level 10
- [ ] Metadata generation and storage
- [ ] Wallet integration for NFT display

### Phase 4: Polish & Beta (Week 6-7)
- [ ] AI opponent with difficulty scaling
- [ ] Performance optimization for WebGL
- [ ] UI/UX polish and effects
- [ ] Beta testing with community

### Phase 5: Launch Preparation (Week 8)
- [ ] Security audit completion
- [ ] Load testing and optimization
- [ ] Marketing website and materials
- [ ] Mainnet deployment preparation

## 7. Success Metrics

### 7.1 Technical KPIs
- Frame rate stability: >95% at 60 FPS
- Load time: <10 seconds for 90% of users
- Crash rate: <0.1% per session
- Transaction success: >99% completion rate

### 7.2 Engagement KPIs
- Day 1 retention: >60%
- Day 7 retention: >40%
- Day 30 retention: >25%
- Average session length: >20 minutes
- Matches per day per user: >5

### 7.3 Economic KPIs
- Token velocity: >10% daily circulation
- Ship upgrade rate: >60% of players
- NFT minting rate: >30% of eligible ships
- Token sink/faucet balance: ±5% monthly

## 8. Risks & Mitigation

### 8.1 Technical Risks
- **WebGL Performance**: Aggressive optimization, LOD systems
- **Blockchain Congestion**: Transaction queuing, optimistic UI
- **Browser Compatibility**: Extensive testing, polyfills

### 8.2 Economic Risks
- **Token Inflation**: Careful sink design, burning mechanisms
- **Bot Farming**: Anti-cheat, rate limiting, human verification
- **Market Manipulation**: Transaction limits, monitoring systems

### 8.3 Adoption Risks
- **Web3 Complexity**: Seamless Web2 onboarding option
- **High Entry Barrier**: Generous starter tokens (1,000 PLASMA)
- **Learning Curve**: Comprehensive tutorial, practice mode

## 9. Future Roadmap (Post-MVP)

### Planned Features
1. **Multiplayer PvP**: Real-time battles with matchmaking
2. **Tournament System**: Scheduled events with large prize pools
3. **Ship Marketplace**: P2P trading of NFT ships
4. **Custom Arenas**: Player-created maps with revenue sharing
5. **Mobile Support**: iOS and Android native apps
6. **DAO Governance**: Token holders vote on game updates
7. **Staking Rewards**: Earn PLASMA by staking ships
8. **Cross-Game Utility**: Ships usable in partner games

## 10. Deliverables

### 10.1 Game Assets
- Unity project with all source code and assets
- 3 fully implemented ship types with upgrade systems
- Dynamic arena editor with save/load functionality
- AI opponent with configurable difficulty

### 10.2 Web Infrastructure
- React frontend with wallet integration
- Supabase backend with complete schema
- n8n automation workflows
- Deployment scripts and CI/CD pipeline

### 10.3 Blockchain Components
- PLASMA token smart contract
- Ship NFT collection contract
- Transaction management system
- On-chain validation mechanisms

### 10.4 Documentation
- Technical implementation guide
- Game design document
- API documentation
- Player onboarding guide
- Token economics whitepaper

## 11. Acceptance Criteria

### 11.1 Gameplay
- [ ] All 3 ship types playable with distinct characteristics
- [ ] Upgrade system affects gameplay measurably
- [ ] Arena editor creates valid, playable configurations
- [ ] AI provides appropriate challenge across difficulty levels

### 11.2 Economy
- [ ] PLASMA tokens transfer correctly for all transactions
- [ ] Entry fees and rewards distribute properly
- [ ] Ship purchases and upgrades persist across sessions
- [ ] NFT minting succeeds for level 10 ships

### 11.3 Technical
- [ ] 60 FPS maintained during combat with effects
- [ ] Deterministic replays match 100% of the time
- [ ] Web2 and Web3 login paths both functional
- [ ] All transactions complete within 5 seconds

### 11.4 Security
- [ ] No token duplication exploits
- [ ] Replay validation catches modified results
- [ ] Rate limiting prevents spam attacks
- [ ] Custodial wallets remain secure

## 12. Conclusion

Plasma Siege represents a new paradigm in Web3 gaming: accessible to traditional gamers while offering deep blockchain integration for crypto enthusiasts. By combining fast-paced space combat with a robust token economy and NFT progression system, we create a game where skill and investment both matter. The Web2/Web3 hybrid approach ensures the widest possible audience while the ship NFT system provides long-term value and collectibility.

**This PRD, combined with the technical implementation guide and game design document, provides a complete blueprint for building, launching, and scaling Plasma Siege into a successful Web3 gaming ecosystem.**

---

*Plasma Siege PRD v1.0 - Ready for Development*