using UnityEngine;

/// <summary>
/// Scout Ship - Fast and agile starter ship
/// Stats: 100 shield, 80 speed, 1 weapon slot
/// </summary>
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