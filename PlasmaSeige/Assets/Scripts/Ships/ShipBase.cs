using UnityEngine;

/// <summary>
/// Base class for all ship types in Plasma Siege
/// Implements core stats, progression, and upgrade system
/// </summary>
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
        Debug.Log($"{tier} ship initialized with {maxShield} shield and {maxSpeed} speed");
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
        return Mathf.Min(10, Mathf.FloorToInt(exp / 1000f) + 1);
    }
    
    protected virtual void OnLevelUp() 
    {
        Debug.Log($"Ship leveled up to {level}!");
        if (level == 10)
        {
            Debug.Log("Ship can now be minted as NFT!");
        }
    }
    
    public bool CanMintNFT() 
    {
        return level >= 10 && !isNFT;
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