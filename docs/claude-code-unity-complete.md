# Claude Code Instructions: Complete Plasma Siege Unity Implementation

## Project Overview
**Project Name:** Plasma Siege  
**Unity Version:** 6000.1.9f1 LTS  
**Project Location:** `/home/benton/projects/plasmasiegeprj/plasmasiegeUnity/`  
**MCP Server Location:** `/home/benton/projects/plasmasiegeprj/tools/unity-mcp/`  
**MCP Config:** `/home/benton/.cursor/mcp.json`  
**Platform:** Pop!_OS 22.04 LTS (Linux)

## Game Specifications Summary
- **Core Gameplay:** Push plasma orb along dynamic spline path into opponent's goal
- **Ship System:** 3 tiers (Scout/Fighter/Destroyer) with NFT minting at level 10
- **Token Economy:** PLASMA tokens for entry fees, rewards, and upgrades
- **Target Performance:** 60 FPS, <30MB build, <10s load time
- **Physics:** Deterministic at 50Hz fixed timestep

## Current Issues & Implementation Stages

### Stage 0: Foundation & MCP Setup ✅
1. **MCP Connection:** Unity MCP server connection to Claude Code/Cursor
2. **Basic Movement:** Ship controller not responding to WASD/mouse input
3. **Physics Setup:** Rigidbody-based movement needs configuration

### Stage 1: Core Ship System (Priority)
Implement 3-tier ship system with proper stats and movement

### Stage 2: Plasma Orb & Spline System
Dynamic orb path with arena editor

### Stage 3: PLASMA Token Integration
Entry fees, rewards, and upgrade economy

### Stage 4: NFT Ship Progression
XP system and level 10 minting capability

## Detailed Implementation Instructions

### 1. Fix MCP Connection & Basic Movement

**MCP Configuration Check:**
```json
{
  "mcpServers": {
    "unity-local": {
      "command": "uv",
      "args": [
        "run",
        "--directory",
        "/home/benton/projects/plasmasiegeprj/tools/unity-mcp/src",
        "server.py"
      ],
      "env": {
        "UNITY_PROJECT_PATH": "/home/benton/projects/plasmasiegeprj/plasmasiegeUnity",
        "MCP_WEBSOCKET_PORT": "5010"
      }
    }
  }
}
```

**Required Input Axes Configuration:**
```
- Horizontal (A/D, Left/Right Arrow)
- Vertical (W/S, Up/Down Arrow)  
- Horizontal2 (Mouse X for yaw)
- Vertical2 (Mouse Y for pitch)
- Roll (Q/E)
- Lift (Space/Left Shift)
- Boost (Left Ctrl)
- Fire1 (Left Mouse - primary laser)
- Fire2 (Right Mouse - missiles)
```

### 2. Ship Tier Implementation

**Create ShipBase.cs:**
```csharp
using UnityEngine;

public abstract class ShipBase : MonoBehaviour 
{
    [Header("Ship Stats")]
    public ShipTier tier;
    public float maxShield;
    public float currentShield;
    public float maxSpeed;
    public int weaponSlots;
    
    [Header("Progression")]
    public int level = 1;
    public int experience = 0;
    public bool isNFT = false;
    public string nftMintAddress;
    
    [Header("Upgrades")]
    public UpgradeSlot[] fuelUpgrades = new UpgradeSlot[3];
    public UpgradeSlot[] shieldUpgrades = new UpgradeSlot[3];
    public UpgradeSlot[] weaponUpgrades = new UpgradeSlot[3];
    
    protected Rigidbody rb;
    
    public enum ShipTier { Scout, Fighter, Destroyer }
    
    protected virtual void Start() 
    {
        rb = GetComponent<Rigidbody>();
        ConfigurePhysics();
        InitializeStats();
    }
    
    protected void ConfigurePhysics() 
    {
        rb.useGravity = false;
        rb.drag = 0.5f;
        rb.angularDrag = 2f;
        rb.mass = GetShipMass();
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
    
    protected abstract void InitializeStats();
    protected abstract float GetShipMass();
    
    public void AddExperience(int amount) 
    {
        experience += amount;
        int newLevel = CalculateLevel(experience);
        if (newLevel > level && newLevel <= 10) 
        {
            level = newLevel;
            OnLevelUp();
        }
    }
    
    private int CalculateLevel(int exp) 
    {
        return Mathf.FloorToInt(Mathf.Sqrt(exp / 100f)) + 1;
    }
    
    protected virtual void OnLevelUp() 
    {
        // Visual effects, stat bonuses
        Debug.Log($"Ship leveled up to {level}!");
    }
    
    public bool CanMintNFT() 
    {
        return level >= 10 && !isNFT;
    }
}
```

**Create specific ship classes:**

```csharp
// ScoutShip.cs
public class ScoutShip : ShipBase 
{
    protected override void InitializeStats() 
    {
        tier = ShipTier.Scout;
        maxShield = 100f;
        currentShield = maxShield;
        maxSpeed = 80f;
        weaponSlots = 1;
    }
    
    protected override float GetShipMass() => 0.8f;
}

// FighterShip.cs  
public class FighterShip : ShipBase 
{
    protected override void InitializeStats() 
    {
        tier = ShipTier.Fighter;
        maxShield = 200f;
        currentShield = maxShield;
        maxSpeed = 60f;
        weaponSlots = 2;
    }
    
    protected override float GetShipMass() => 1.2f;
}

// DestroyerShip.cs
public class DestroyerShip : ShipBase 
{
    protected override void InitializeStats() 
    {
        tier = ShipTier.Destroyer;
        maxShield = 300f;
        currentShield = maxShield;
        maxSpeed = 40f;
        weaponSlots = 3;
    }
    
    protected override float GetShipMass() => 2.0f;
}
```

### 3. Enhanced Ship Controller with Token Integration

**Update ShipController.cs:**
```csharp
using UnityEngine;
using System.Collections.Generic;

public class ShipController : MonoBehaviour 
{
    [Header("Movement Settings")]
    public float thrustPower = 10f;
    public float strafePower = 8f;
    public float liftPower = 8f;
    public float rotationSpeed = 2f;
    public float boostMultiplier = 2f;
    
    [Header("Combat")]
    public GameObject laserPrefab;
    public GameObject missilePrefab;
    public Transform[] weaponMounts;
    public float fireRate = 0.2f;
    
    [Header("Resources")]
    public float maxFuel = 100f;
    public float currentFuel = 100f;
    public float fuelRegenRate = 20f;
    public float boostFuelCost = 30f;
    
    [Header("Token Collection")]
    public int collectedTokens = 0;
    private List<int> tokenPickupValues = new List<int>();
    
    private ShipBase shipBase;
    private Rigidbody rb;
    private float nextFireTime;
    private bool isBoosting;
    
    void Start() 
    {
        shipBase = GetComponent<ShipBase>();
        rb = GetComponent<Rigidbody>();
        currentFuel = maxFuel;
        
        // Apply upgrade modifiers
        ApplyUpgrades();
    }
    
    void Update() 
    {
        HandleWeapons();
        UpdateFuel();
        
        // Debug info
        if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            Debug.Log($"Tokens: {collectedTokens}, Fuel: {currentFuel:F1}, Shield: {shipBase.currentShield:F1}");
        }
    }
    
    void FixedUpdate() 
    {
        HandleMovement();
        HandleRotation();
        ClampVelocity();
    }
    
    void HandleMovement() 
    {
        // Get input
        float thrust = Input.GetAxis("Vertical");
        float strafe = Input.GetAxis("Horizontal");
        float lift = 0f;
        
        if (Input.GetKey(KeyCode.Space)) lift = 1f;
        if (Input.GetKey(KeyCode.LeftShift)) lift = -1f;
        
        // Check boost
        isBoosting = Input.GetKey(KeyCode.LeftControl) && currentFuel > 0;
        float boostMod = isBoosting ? boostMultiplier : 1f;
        
        // Apply forces
        Vector3 moveForce = transform.forward * thrust * thrustPower * boostMod;
        moveForce += transform.right * strafe * strafePower * boostMod;
        moveForce += transform.up * lift * liftPower * boostMod;
        
        rb.AddForce(moveForce);
    }
    
    void HandleRotation() 
    {
        float yaw = Input.GetAxis("Mouse X") * rotationSpeed;
        float pitch = -Input.GetAxis("Mouse Y") * rotationSpeed;
        float roll = -Input.GetAxis("Roll") * rotationSpeed * 2f;
        
        Vector3 torque = new Vector3(pitch, yaw, roll);
        rb.AddRelativeTorque(torque);
    }
    
    void HandleWeapons() 
    {
        if (Input.GetButton("Fire1") && Time.time > nextFireTime) 
        {
            FirePrimary();
            nextFireTime = Time.time + fireRate;
        }
        
        if (Input.GetButtonDown("Fire2")) 
        {
            FireSecondary();
        }
    }
    
    void FirePrimary() 
    {
        foreach (Transform mount in weaponMounts) 
        {
            if (mount.gameObject.activeSelf) 
            {
                GameObject laser = Instantiate(laserPrefab, mount.position, mount.rotation);
                // Apply damage multiplier from upgrades
            }
        }
    }
    
    void FireSecondary() 
    {
        // Similar for missiles
    }
    
    void UpdateFuel() 
    {
        if (isBoosting) 
        {
            currentFuel -= boostFuelCost * Time.deltaTime;
            currentFuel = Mathf.Max(0, currentFuel);
        }
        else 
        {
            currentFuel += fuelRegenRate * Time.deltaTime;
            currentFuel = Mathf.Min(maxFuel, currentFuel);
        }
    }
    
    void ClampVelocity() 
    {
        float maxVel = shipBase.maxSpeed * (isBoosting ? boostMultiplier : 1f);
        if (rb.velocity.magnitude > maxVel) 
        {
            rb.velocity = rb.velocity.normalized * maxVel;
        }
    }
    
    void ApplyUpgrades() 
    {
        // Apply fuel upgrades
        foreach (var upgrade in shipBase.fuelUpgrades) 
        {
            if (upgrade != null && upgrade.tier > 0) 
            {
                maxFuel *= (1f + upgrade.GetBonus());
            }
        }
        
        // Apply other upgrades...
    }
    
    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("TokenPickup")) 
        {
            TokenPickup token = other.GetComponent<TokenPickup>();
            collectedTokens += token.value;
            tokenPickupValues.Add(token.value);
            Destroy(other.gameObject);
            
            // Visual/audio feedback
            Debug.Log($"Collected {token.value} PLASMA! Total: {collectedTokens}");
        }
    }
    
    public void TakeDamage(float damage) 
    {
        shipBase.currentShield -= damage;
        if (shipBase.currentShield <= 0) 
        {
            OnShipDestroyed();
        }
    }
    
    void OnShipDestroyed() 
    {
        // Drop 50% of collected tokens
        int droppedTokens = collectedTokens / 2;
        // Spawn token pickups at current position
        
        Debug.Log($"Ship destroyed! Dropping {droppedTokens} PLASMA");
    }
}
```

### 4. Plasma Orb & Spline System

**Create PlasmaOrb.cs:**
```csharp
using UnityEngine;
using UnityEngine.Splines;

public class PlasmaOrb : MonoBehaviour 
{
    [Header("Movement")]
    public SplineContainer splineContainer;
    public float currentPosition = 0.5f; // 0-1 along spline
    public float velocity = 0f;
    public float drag = 0.98f;
    public float maxVelocity = 0.2f;
    
    [Header("Visuals")]
    public Renderer orbRenderer;
    public ParticleSystem trailParticles;
    public Gradient neutralGradient;
    public Gradient team1Gradient;
    public Gradient team2Gradient;
    
    private Material orbMaterial;
    
    void Start() 
    {
        orbMaterial = orbRenderer.material;
        UpdatePosition();
    }
    
    void FixedUpdate() 
    {
        // Apply velocity
        currentPosition += velocity * Time.fixedDeltaTime;
        
        // Apply drag
        velocity *= drag;
        
        // Clamp position
        currentPosition = Mathf.Clamp01(currentPosition);
        
        // Check for goals
        if (currentPosition <= 0.05f) 
        {
            OnGoalScored(2); // Team 2 scores
        }
        else if (currentPosition >= 0.95f) 
        {
            OnGoalScored(1); // Team 1 scores
        }
        
        UpdatePosition();
        UpdateVisuals();
    }
    
    void UpdatePosition() 
    {
        if (splineContainer != null) 
        {
            Vector3 position = splineContainer.EvaluatePosition(currentPosition);
            Vector3 tangent = splineContainer.EvaluateTangent(currentPosition);
            
            transform.position = position;
            transform.rotation = Quaternion.LookRotation(tangent);
        }
    }
    
    void UpdateVisuals() 
    {
        // Color based on velocity direction
        Color targetColor = Color.white;
        var gradient = neutralGradient;
        
        if (velocity > 0.01f) 
        {
            gradient = team2Gradient;
        }
        else if (velocity < -0.01f) 
        {
            gradient = team1Gradient;
        }
        
        float intensity = Mathf.Abs(velocity) / maxVelocity;
        targetColor = gradient.Evaluate(intensity);
        
        orbMaterial.SetColor("_EmissionColor", targetColor * 2f);
        
        // Particle effects
        var main = trailParticles.main;
        main.startColor = targetColor;
        main.startSpeed = 5f + Mathf.Abs(velocity) * 20f;
    }
    
    public void ApplyImpulse(Vector3 force, Vector3 hitPoint) 
    {
        // Project force onto spline tangent
        Vector3 tangent = splineContainer.EvaluateTangent(currentPosition);
        float impulse = Vector3.Dot(force.normalized, tangent) * force.magnitude;
        
        // Scale based on hit angle
        float hitAngle = Vector3.Angle(force, tangent);
        float angleFactor = Mathf.Cos(hitAngle * Mathf.Deg2Rad);
        
        velocity += impulse * angleFactor * 0.001f;
        velocity = Mathf.Clamp(velocity, -maxVelocity, maxVelocity);
    }
    
    void OnGoalScored(int teamNumber) 
    {
        Debug.Log($"GOAL! Team {teamNumber} scores!");
        // Reset orb to center
        currentPosition = 0.5f;
        velocity = 0f;
        
        // Trigger match events
        GameManager.Instance.OnGoalScored(teamNumber);
    }
}
```

### 5. Dynamic Arena Editor

**Create ArenaEditor.cs:**
```csharp
using UnityEngine;
using UnityEngine.Splines;
using System.Collections.Generic;

public class ArenaEditor : MonoBehaviour 
{
    [Header("Arena Settings")]
    public Vector3 arenaSize = new Vector3(400, 400, 400);
    public GameObject arenaBoundsPrefab;
    
    [Header("Spline Editor")]
    public SplineContainer splineContainer;
    public int splinePoints = 5;
    public float splineComplexity = 50f;
    
    [Header("Obstacles")]
    public GameObject asteroidPrefab;
    public int asteroidCount = 20;
    public float minAsteroidSize = 5f;
    public float maxAsteroidSize = 20f;
    
    [Header("Token Spawns")]
    public GameObject tokenPickupPrefab;
    public int tokenSpawnCount = 10;
    public int[] tokenValues = { 1, 1, 1, 2, 2, 3, 5 };
    
    private List<GameObject> spawnedObjects = new List<GameObject>();
    
    void Start() 
    {
        if (Application.isPlaying) 
        {
            GenerateArena();
        }
    }
    
    public void GenerateArena() 
    {
        ClearArena();
        CreateArenaBounds();
        GenerateSpline();
        SpawnAsteroids();
        SpawnTokenPickups();
    }
    
    void CreateArenaBounds() 
    {
        // Create 6 walls for arena cube
        CreateWall(Vector3.up * arenaSize.y/2, Vector3.up, arenaSize.x, arenaSize.z);
        CreateWall(Vector3.down * arenaSize.y/2, Vector3.down, arenaSize.x, arenaSize.z);
        CreateWall(Vector3.right * arenaSize.x/2, Vector3.right, arenaSize.z, arenaSize.y);
        CreateWall(Vector3.left * arenaSize.x/2, Vector3.left, arenaSize.z, arenaSize.y);
        CreateWall(Vector3.forward * arenaSize.z/2, Vector3.forward, arenaSize.x, arenaSize.y);
        CreateWall(Vector3.back * arenaSize.z/2, Vector3.back, arenaSize.x, arenaSize.y);
    }
    
    void CreateWall(Vector3 position, Vector3 normal, float width, float height) 
    {
        GameObject wall = Instantiate(arenaBoundsPrefab, position, Quaternion.LookRotation(normal));
        wall.transform.localScale = new Vector3(width, height, 1);
        spawnedObjects.Add(wall);
    }
    
    void GenerateSpline() 
    {
        if (splineContainer == null) return;
        
        var spline = splineContainer.Spline;
        spline.Clear();
        
        // Start and end points
        Vector3 startPos = new Vector3(-arenaSize.x * 0.4f, 0, 0);
        Vector3 endPos = new Vector3(arenaSize.x * 0.4f, 0, 0);
        
        spline.Add(new BezierKnot(startPos));
        
        // Add intermediate points
        for (int i = 1; i < splinePoints - 1; i++) 
        {
            float t = (float)i / (splinePoints - 1);
            Vector3 basePos = Vector3.Lerp(startPos, endPos, t);
            
            // Add variation
            Vector3 offset = new Vector3(
                Random.Range(-splineComplexity, splineComplexity),
                Random.Range(-splineComplexity, splineComplexity),
                Random.Range(-splineComplexity, splineComplexity)
            );
            
            spline.Add(new BezierKnot(basePos + offset));
        }
        
        spline.Add(new BezierKnot(endPos));
        
        // Set to auto-smooth
        for (int i = 0; i < spline.Count; i++) 
        {
            spline.SetTangentMode(i, TangentMode.AutoSmooth);
        }
    }
    
    void SpawnAsteroids() 
    {
        for (int i = 0; i < asteroidCount; i++) 
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-arenaSize.x/2, arenaSize.x/2),
                Random.Range(-arenaSize.y/2, arenaSize.y/2),
                Random.Range(-arenaSize.z/2, arenaSize.z/2)
            );
            
            // Don't spawn too close to spline
            float distToSpline = GetDistanceToSpline(randomPos);
            if (distToSpline < 30f) continue;
            
            GameObject asteroid = Instantiate(asteroidPrefab, randomPos, Random.rotation);
            float scale = Random.Range(minAsteroidSize, maxAsteroidSize);
            asteroid.transform.localScale = Vector3.one * scale;
            
            // Add slow rotation
            asteroid.GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * 0.5f;
            
            spawnedObjects.Add(asteroid);
        }
    }
    
    void SpawnTokenPickups() 
    {
        for (int i = 0; i < tokenSpawnCount; i++) 
        {
            Vector3 randomPos = new Vector3(
                Random.Range(-arenaSize.x/3, arenaSize.x/3),
                Random.Range(-arenaSize.y/3, arenaSize.y/3),
                Random.Range(-arenaSize.z/3, arenaSize.z/3)
            );
            
            GameObject token = Instantiate(tokenPickupPrefab, randomPos, Quaternion.identity);
            TokenPickup pickup = token.GetComponent<TokenPickup>();
            pickup.value = tokenValues[Random.Range(0, tokenValues.Length)];
            
            spawnedObjects.Add(token);
        }
    }
    
    float GetDistanceToSpline(Vector3 position) 
    {
        // Simple approximation - check distance to several points on spline
        float minDist = float.MaxValue;
        for (float t = 0; t <= 1f; t += 0.1f) 
        {
            Vector3 splinePoint = splineContainer.EvaluatePosition(t);
            float dist = Vector3.Distance(position, splinePoint);
            minDist = Mathf.Min(minDist, dist);
        }
        return minDist;
    }
    
    void ClearArena() 
    {
        foreach (var obj in spawnedObjects) 
        {
            DestroyImmediate(obj);
        }
        spawnedObjects.Clear();
    }
    
    public void SaveArenaConfig(string configName) 
    {
        ArenaConfig config = ScriptableObject.CreateInstance<ArenaConfig>();
        config.arenaSize = arenaSize;
        config.splinePoints = splinePoints;
        config.asteroidCount = asteroidCount;
        config.tokenSpawnCount = tokenSpawnCount;
        // Save spline data...
        
        #if UNITY_EDITOR
        UnityEditor.AssetDatabase.CreateAsset(config, $"Assets/ArenaConfigs/{configName}.asset");
        #endif
    }
}
```

### 6. Token & Economy System

**Create TokenPickup.cs:**
```csharp
using UnityEngine;

public class TokenPickup : MonoBehaviour 
{
    public int value = 1;
    public float rotationSpeed = 90f;
    public float bobAmplitude = 0.5f;
    public float bobFrequency = 1f;
    
    private Vector3 startPos;
    private ParticleSystem particles;
    
    void Start() 
    {
        startPos = transform.position;
        particles = GetComponentInChildren<ParticleSystem>();
        
        // Set visual based on value
        transform.localScale = Vector3.one * (0.5f + value * 0.1f);
        
        // Color based on value
        Color tokenColor = value switch 
        {
            1 => Color.white,
            2 => Color.cyan,
            3 => Color.yellow,
            5 => Color.magenta,
            _ => Color.white
        };
        
        GetComponent<Renderer>().material.SetColor("_EmissionColor", tokenColor * 2f);
    }
    
    void Update() 
    {
        // Rotation
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        
        // Bobbing
        float newY = startPos.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}
```

**Create PLASMAManager.cs:**
```csharp
using UnityEngine;
using System;

public class PLASMAManager : MonoBehaviour 
{
    public static PLASMAManager Instance;
    
    [Header("Economy Settings")]
    public int entryFee = 10;
    public int winReward = 20;
    public int perfectMatchBonus = 5;
    public float dropPercentage = 0.5f;
    
    [Header("Player Balance")]
    public int playerBalance = 1000; // Starter tokens
    
    public event Action<int> OnBalanceChanged;
    
    void Awake() 
    {
        if (Instance == null) 
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else 
        {
            Destroy(gameObject);
        }
    }
    
    public bool CanAffordEntry() 
    {
        return playerBalance >= entryFee;
    }
    
    public void DeductEntryFee() 
    {
        if (CanAffordEntry()) 
        {
            playerBalance -= entryFee;
            OnBalanceChanged?.Invoke(playerBalance);
            Debug.Log($"Entry fee deducted. Balance: {playerBalance} PLASMA");
        }
    }
    
    public void AwardMatchReward(bool won, int tokensCollected, bool perfectMatch) 
    {
        int reward = tokensCollected;
        
        if (won) 
        {
            reward += winReward;
            if (perfectMatch) 
            {
                reward += perfectMatchBonus;
            }
        }
        
        playerBalance += reward;
        OnBalanceChanged?.Invoke(playerBalance);
        
        Debug.Log($"Match complete! Reward: {reward} PLASMA. New balance: {playerBalance}");
    }
    
    public bool PurchaseShip(ShipBase.ShipTier tier) 
    {
        int cost = GetShipCost(tier);
        
        if (playerBalance >= cost) 
        {
            playerBalance -= cost;
            OnBalanceChanged?.Invoke(playerBalance);
            Debug.Log($"Purchased {tier} ship for {cost} PLASMA");
            return true;
        }
        
        return false;
    }
    
    public bool PurchaseUpgrade(UpgradeType type, int tier) 
    {
        int cost = GetUpgradeCost(type, tier);
        
        if (playerBalance >= cost) 
        {
            playerBalance -= cost;
            OnBalanceChanged?.Invoke(playerBalance);
            Debug.Log($"Purchased {type} upgrade tier {tier} for {cost} PLASMA");
            return true;
        }
        
        return false;
    }
    
    int GetShipCost(ShipBase.ShipTier tier) 
    {
        return tier switch 
        {
            ShipBase.ShipTier.Scout => 0,
            ShipBase.ShipTier.Fighter => 500,
            ShipBase.ShipTier.Destroyer => 2000,
            _ => 0
        };
    }
    
    int GetUpgradeCost(UpgradeType type, int tier) 
    {
        return type switch 
        {
            UpgradeType.Fuel => tier * 50,
            UpgradeType.Shield => tier * 75,
            UpgradeType.Weapon => tier * 100,
            _ => 0
        };
    }
}

public enum UpgradeType 
{
    Fuel,
    Shield,
    Weapon
}
```

### 7. Game Manager & Match Flow

**Create GameManager.cs:**
```csharp
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour 
{
    public static GameManager Instance;
    
    [Header("Match Settings")]
    public int goalsToWin = 3;
    public float matchTimeLimit = 300f; // 5 minutes
    public bool isPracticeMode = false;
    
    [Header("Game State")]
    public int playerScore = 0;
    public int aiScore = 0;
    public float matchTime = 0f;
    public bool matchActive = false;
    
    [Header("References")]
    public GameObject playerShipPrefab;
    public GameObject aiShipPrefab;
    public Transform playerSpawn;
    public Transform aiSpawn;
    public PlasmaOrb plasmaOrb;
    
    [Header("UI")]
    public Text scoreText;
    public Text timerText;
    public Text plasmaBalanceText;
    
    private GameObject playerShip;
    private GameObject aiShip;
    private int playerTokensCollected = 0;
    
    void Awake() 
    {
        Instance = this;
    }
    
    void Start() 
    {
        StartCoroutine(StartMatch());
    }
    
    IEnumerator StartMatch() 
    {
        // Check entry fee
        if (!isPracticeMode && !PLASMAManager.Instance.CanAffordEntry()) 
        {
            Debug.Log("Insufficient PLASMA for entry!");
            yield break;
        }
        
        if (!isPracticeMode) 
        {
            PLASMAManager.Instance.DeductEntryFee();
        }
        
        // Spawn ships
        playerShip = Instantiate(playerShipPrefab, playerSpawn.position, playerSpawn.rotation);
        aiShip = Instantiate(aiShipPrefab, aiSpawn.position, aiSpawn.rotation);
        
        // Initialize orb
        plasmaOrb.currentPosition = 0.5f;
        plasmaOrb.velocity = 0f;
        
        // Start match
        matchActive = true;
        matchTime = 0f;
        
        yield return null;
    }
    
    void Update() 
    {
        if (!matchActive) return;
        
        // Update timer
        matchTime += Time.deltaTime;
        UpdateUI();
        
        // Check win conditions
        if (playerScore >= goalsToWin || aiScore >= goalsToWin || matchTime >= matchTimeLimit) 
        {
            EndMatch();
        }
    }
    
    public void OnGoalScored(int team) 
    {
        if (team == 1) 
        {
            playerScore++;
        }
        else 
        {
            aiScore++;
        }
        
        // Reset positions
        StartCoroutine(ResetAfterGoal());
    }
    
    IEnumerator ResetAfterGoal() 
    {
        matchActive = false;
        
        // Show goal celebration
        yield return new WaitForSeconds(2f);
        
        // Reset ship positions
        playerShip.transform.position = playerSpawn.position;
        playerShip.transform.rotation = playerSpawn.rotation;
        playerShip.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        aiShip.transform.position = aiSpawn.position;
        aiShip.transform.rotation = aiSpawn.rotation;
        aiShip.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        matchActive = true;
    }
    
    void EndMatch() 
    {
        matchActive = false;
        
        bool playerWon = playerScore > aiScore;
        bool perfectMatch = aiScore == 0 && playerScore >= goalsToWin;
        
        // Get tokens collected from player ship
        if (playerShip != null) 
        {
            var controller = playerShip.GetComponent<ShipController>();
            playerTokensCollected = controller.collectedTokens;
        }
        
        // Award rewards
        if (!isPracticeMode) 
        {
            PLASMAManager.Instance.AwardMatchReward(playerWon, playerTokensCollected, perfectMatch);
        }
        
        // Add XP to ship
        var shipBase = playerShip.GetComponent<ShipBase>();
        shipBase.AddExperience(playerWon ? 200 : 100);
        
        Debug.Log($"Match ended! Player: {playerScore}, AI: {aiScore}");
        
        // Return to menu or restart
        StartCoroutine(ReturnToMenu());
    }
    
    IEnumerator ReturnToMenu() 
    {
        yield return new WaitForSeconds(3f);
        // Load menu scene or show results screen
    }
    
    void UpdateUI() 
    {
        scoreText.text = $"Player: {playerScore} - AI: {aiScore}";
        
        float remainingTime = Mathf.Max(0, matchTimeLimit - matchTime);
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = $"{minutes:00}:{seconds:00}";
        
        plasmaBalanceText.text = $"{PLASMAManager.Instance.playerBalance} PLASMA";
    }
}
```

### 8. AI Opponent Implementation

**Create AIShipController.cs:**
```csharp
using UnityEngine;

public class AIShipController : MonoBehaviour 
{
    [Header("AI Settings")]
    public float difficulty = 0.5f; // 0-1
    public float reactionTime = 0.5f;
    public float accuracy = 0.7f;
    
    [Header("Behavior Weights")]
    public float collectTokenWeight = 0.3f;
    public float controlOrbWeight = 0.5f;
    public float combatWeight = 0.2f;
    
    private enum AIState { CollectTokens, ControlOrb, Combat, Defend }
    private AIState currentState = AIState.ControlOrb;
    
    private ShipController shipController;
    private PlasmaOrb orb;
    private float nextDecisionTime;
    private Transform currentTarget;
    
    void Start() 
    {
        shipController = GetComponent<ShipController>();
        orb = FindObjectOfType<PlasmaOrb>();
        
        // Scale parameters by difficulty
        reactionTime = Mathf.Lerp(1f, 0.3f, difficulty);
        accuracy = Mathf.Lerp(0.3f, 0.9f, difficulty);
    }
    
    void Update() 
    {
        if (Time.time > nextDecisionTime) 
        {
            MakeDecision();
            nextDecisionTime = Time.time + reactionTime;
        }
        
        ExecuteBehavior();
    }
    
    void MakeDecision() 
    {
        // Evaluate situation
        float orbPosition = orb.currentPosition;
        float orbVelocity = orb.velocity;
        bool orbMovingTowardsOurGoal = orbPosition < 0.5f && orbVelocity < 0;
        
        // Find nearest token
        GameObject nearestToken = FindNearestToken();
        float tokenDistance = nearestToken ? Vector3.Distance(transform.position, nearestToken.transform.position) : float.MaxValue;
        
        // State weights based on situation
        float tokenPriority = tokenDistance < 50f ? collectTokenWeight * 2f : collectTokenWeight;
        float orbPriority = orbMovingTowardsOurGoal ? controlOrbWeight * 3f : controlOrbWeight;
        float combatPriority = Vector3.Distance(transform.position, GameManager.Instance.playerShip.transform.position) < 100f ? combatWeight : 0f;
        
        // Choose state
        float total = tokenPriority + orbPriority + combatPriority;
        float random = Random.Range(0f, total);
        
        if (random < tokenPriority) 
        {
            currentState = AIState.CollectTokens;
            currentTarget = nearestToken?.transform;
        }
        else if (random < tokenPriority + orbPriority) 
        {
            currentState = AIState.ControlOrb;
            currentTarget = orb.transform;
        }
        else 
        {
            currentState = AIState.Combat;
            currentTarget = GameManager.Instance.playerShip.transform;
        }
    }
    
    void ExecuteBehavior() 
    {
        if (currentTarget == null) return;
        
        Vector3 targetDirection = (currentTarget.position - transform.position).normalized;
        float targetDistance = Vector3.Distance(transform.position, currentTarget.position);
        
        // Movement
        Vector3 moveInput = targetDirection;
        
        // Add inaccuracy
        moveInput += Random.insideUnitSphere * (1f - accuracy) * 0.5f;
        moveInput.Normalize();
        
        // Simulate input
        SimulateMovementInput(moveInput);
        
        // Combat behavior
        if (currentState == AIState.Combat || (currentState == AIState.ControlOrb && targetDistance < 50f)) 
        {
            SimulateCombatInput(targetDirection);
        }
    }
    
    void SimulateMovementInput(Vector3 direction) 
    {
        // Convert world direction to local space
        Vector3 localDir = transform.InverseTransformDirection(direction);
        
        // Simulate WASD input
        float thrust = Mathf.Clamp(localDir.z, -1f, 1f);
        float strafe = Mathf.Clamp(localDir.x, -1f, 1f);
        float lift = Mathf.Clamp(localDir.y, -1f, 1f);
        
        // Apply to ship controller (would need to modify ShipController to accept AI input)
        // For now, directly apply forces
        var rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * thrust * shipController.thrustPower);
        rb.AddForce(transform.right * strafe * shipController.strafePower);
        rb.AddForce(transform.up * lift * shipController.liftPower);
        
        // Rotation
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRotation, Time.deltaTime * 2f));
    }
    
    void SimulateCombatInput(Vector3 targetDirection) 
    {
        // Check if target is in front
        float angle = Vector3.Angle(transform.forward, targetDirection);
        
        if (angle < 30f * accuracy) 
        {
            // Fire primary weapon
            shipController.SendMessage("FirePrimary", SendMessageOptions.DontRequireReceiver);
        }
    }
    
    GameObject FindNearestToken() 
    {
        GameObject[] tokens = GameObject.FindGameObjectsWithTag("TokenPickup");
        GameObject nearest = null;
        float minDistance = float.MaxValue;
        
        foreach (var token in tokens) 
        {
            float dist = Vector3.Distance(transform.position, token.transform.position);
            if (dist < minDistance) 
            {
                minDistance = dist;
                nearest = token;
            }
        }
        
        return nearest;
    }
}
```

### 9. Required ScriptableObjects

**Create UpgradeConfig.cs:**
```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeConfig", menuName = "PlasmaSiege/UpgradeConfig")]
public class UpgradeConfig : ScriptableObject 
{
    public string upgradeName;
    public UpgradeType type;
    public int tier; // 1-3
    public int plasmaCost;
    public float bonusPercentage;
    public Sprite icon;
    public string description;
    
    public float GetBonus() 
    {
        return bonusPercentage / 100f;
    }
}

[System.Serializable]
public class UpgradeSlot 
{
    public UpgradeConfig upgrade;
    public int tier;
    
    public float GetBonus() 
    {
        return upgrade != null ? upgrade.GetBonus() : 0f;
    }
}
```

**Create ArenaConfig.cs:**
```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "ArenaConfig", menuName = "PlasmaSiege/ArenaConfig")]
public class ArenaConfig : ScriptableObject 
{
    public string arenaName;
    public Vector3 arenaSize = new Vector3(400, 400, 400);
    public int splinePoints = 5;
    public AnimationCurve splineShape;
    public int asteroidCount = 20;
    public int tokenSpawnCount = 10;
    public float difficulty = 1f;
    
    [TextArea(3, 5)]
    public string description;
}
```

### 10. Performance Optimization for WebGL

**Create PerformanceManager.cs:**
```csharp
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PerformanceManager : MonoBehaviour 
{
    [Header("Quality Settings")]
    public bool autoAdjustQuality = true;
    public float targetFrameRate = 60f;
    public float adjustmentThreshold = 5f;
    
    [Header("Current Stats")]
    public float currentFPS;
    public int currentQualityLevel;
    
    private float deltaTime;
    private float adjustmentCooldown = 2f;
    private float lastAdjustmentTime;
    
    void Start() 
    {
        // WebGL specific settings
        Application.targetFrameRate = 60;
        
        // Optimize for WebGL
        QualitySettings.vSyncCount = 0;
        QualitySettings.shadows = ShadowQuality.HardOnly;
        QualitySettings.shadowResolution = ShadowResolution.Low;
        
        // Reduce texture quality for WebGL
        QualitySettings.globalTextureMipmapLimit = 1;
        
        // Get URP asset and optimize
        var urpAsset = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
        if (urpAsset != null) 
        {
            urpAsset.shadowDistance = 100f;
            urpAsset.renderScale = 0.9f;
        }
    }
    
    void Update() 
    {
        // Calculate FPS
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        currentFPS = 1.0f / deltaTime;
        
        // Auto adjust quality
        if (autoAdjustQuality && Time.time > lastAdjustmentTime + adjustmentCooldown) 
        {
            if (currentFPS < targetFrameRate - adjustmentThreshold && currentQualityLevel > 0) 
            {
                currentQualityLevel--;
                QualitySettings.SetQualityLevel(currentQualityLevel);
                lastAdjustmentTime = Time.time;
                Debug.Log($"Lowered quality to level {currentQualityLevel}");
            }
            else if (currentFPS > targetFrameRate + adjustmentThreshold && currentQualityLevel < QualitySettings.names.Length - 1) 
            {
                currentQualityLevel++;
                QualitySettings.SetQualityLevel(currentQualityLevel);
                lastAdjustmentTime = Time.time;
                Debug.Log($"Raised quality to level {currentQualityLevel}");
            }
        }
    }
}
```

## Testing Procedures

### 1. MCP Connection Test
```csharp
// Test script to verify MCP is working
public class MCPTest : MonoBehaviour 
{
    void Start() 
    {
        Debug.Log("MCP Test: Creating test cube...");
        GameObject testCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        testCube.transform.position = Vector3.up * 5f;
        testCube.name = "MCP_Test_Cube";
    }
}
```

### 2. Ship Movement Test
```csharp
public class MovementDebugger : MonoBehaviour 
{
    void OnGUI() 
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"Thrust: {Input.GetAxis("Vertical")}");
        GUI.Label(new Rect(10, 30, 300, 20), $"Strafe: {Input.GetAxis("Horizontal")}");
        GUI.Label(new Rect(10, 50, 300, 20), $"Mouse X: {Input.GetAxis("Mouse X")}");
        GUI.Label(new Rect(10, 70, 300, 20), $"Mouse Y: {Input.GetAxis("Mouse Y")}");
        GUI.Label(new Rect(10, 90, 300, 20), $"FPS: {1f/Time.deltaTime:F1}");
    }
}
```

### 3. Token Economy Test
- Start with 1000 PLASMA
- Enter practice match (no fee)
- Collect 5 tokens (varying values)
- Win match (+20 PLASMA)
- Purchase Fighter ship (-500 PLASMA)
- Verify balance updates correctly

## Build & Deployment

### Linux Build Script
```bash
#!/bin/bash
# build-plasma-siege.sh

UNITY_PATH="/opt/Unity/Editor/Unity"
PROJECT_PATH="/home/benton/projects/plasmasiegeprj/plasmasiegeUnity"
BUILD_PATH="$PROJECT_PATH/Builds/WebGL"

# Clean previous build
rm -rf "$BUILD_PATH"

# Build WebGL
"$UNITY_PATH" \
  -batchmode \
  -quit \
  -projectPath "$PROJECT_PATH" \
  -buildTarget WebGL \
  -executeMethod BuildScript.BuildWebGL \
  -logFile build.log

echo "Build complete! Check $BUILD_PATH"
```

### Build Settings Script
```csharp
using UnityEditor;
using UnityEngine;

public class BuildScript 
{
    [MenuItem("Build/WebGL Production")]
    public static void BuildWebGL() 
    {
        PlayerSettings.WebGL.linkerTarget = WebGLLinkerTarget.Wasm;
        PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Gzip;
        PlayerSettings.WebGL.exceptionSupport = WebGLExceptionSupport.None;
        PlayerSettings.WebGL.memorySize = 512;
        
        BuildPlayerOptions options = new BuildPlayerOptions 
        {
            scenes = new[] { "Assets/Scenes/MainMenu.unity", "Assets/Scenes/Arena.unity" },
            locationPathName = "Builds/WebGL",
            target = BuildTarget.WebGL,
            options = BuildOptions.None
        };
        
        BuildPipeline.BuildPlayer(options);
    }
}
```

## Troubleshooting Guide

### Common Issues

1. **MCP Not Connecting**
   - Check `uv` is installed: `pip install uv`
   - Verify path in mcp.json is correct
   - Try manual server start: `cd /tools/unity-mcp && uv run server.py`

2. **Ship Not Moving**
   - Verify Input Manager has all required axes
   - Check Rigidbody constraints aren't locking movement
   - Ensure ShipController script is attached and enabled

3. **WebGL Performance Issues**
   - Reduce shadow quality in URP settings
   - Lower render scale to 0.8
   - Disable post-processing effects
   - Use object pooling for all projectiles

4. **Token Collection Not Working**
   - Verify "TokenPickup" tag exists
   - Check collider triggers are enabled
   - Ensure both ship and tokens have colliders

## Success Metrics Checklist

- [ ] **MCP Integration**: Can create/modify GameObjects via Claude Code
- [ ] **Ship Movement**: All 6DOF controls working smoothly
- [ ] **Ship Tiers**: 3 distinct ships with correct stats
- [ ] **Token Economy**: Entry fees, rewards, and pickups functional
- [ ] **Plasma Orb**: Moves along spline, responds to impacts
- [ ] **Arena Editor**: Can adjust spline and spawn configuration
- [ ] **AI Opponent**: Makes intelligent decisions, uses all mechanics
- [ ] **Performance**: Maintains 60 FPS on GTX 1060
- [ ] **WebGL Build**: Under 30MB, loads in <10 seconds
- [ ] **Upgrade System**: 9 slots per ship, visual changes
- [ ] **NFT Ready**: Level/XP system tracks to level 10
- [ ] **Match Flow**: Complete game loop with win conditions

## Next Steps After Core Implementation

1. **Supabase Integration**: Connect authentication and wallet system
2. **Blockchain Connection**: Implement Thirdweb SDK for Solana
3. **NFT Minting UI**: Create interface for level 10 ships
4. **Multiplayer Foundation**: Prepare networking architecture
5. **Polish Pass**: Particle effects, audio, UI animations

---

*Claude Code Unity Instructions v2.0 - Complete Implementation Guide*