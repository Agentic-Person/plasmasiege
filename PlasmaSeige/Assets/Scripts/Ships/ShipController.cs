using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 6-DoF Ship Controller for Plasma Siege
/// Handles movement, boost, and basic combat
/// </summary>
public class ShipController : MonoBehaviour 
{
    [Header("Movement Settings")]
    public float thrustPower = 15f;
    public float strafePower = 12f;
    public float liftPower = 12f;
    public float rotationSpeed = 3f;
    public float boostMultiplier = 2f;
    
    [Header("Combat")]
    public Transform[] weaponMounts;
    public float fireRate = 0.2f;
    
    [Header("Resources")]
    public float maxFuel = 100f;
    public float currentFuel = 100f;
    public float fuelRegenRate = 20f;
    public float boostFuelCost = 30f;
    
    [Header("Token Collection")]
    public int collectedTokens = 0;
    
    private ShipBase shipBase;
    private Rigidbody rb;
    private float nextFireTime;
    private bool isBoosting;
    
    void Start() 
    {
        shipBase = GetComponent<ShipBase>();
        rb = GetComponent<Rigidbody>();
        currentFuel = maxFuel;
        
        Debug.Log("Ship Controller initialized - Use WASD to move, Mouse to look, Ctrl to boost");
        Debug.Log("Controls: WASD=Move, Mouse=Look, Space/Shift=Up/Down, Ctrl=Boost, Mouse1=Fire");
    }
    
    void Update() 
    {
        HandleWeapons();
        UpdateFuel();
        
        // Debug info
        if (Input.GetKeyDown(KeyCode.Tab)) 
        {
            Debug.Log($"Tokens: {collectedTokens}, Fuel: {currentFuel:F1}, Shield: {shipBase.currentShield:F1}, Level: {shipBase.level}");
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
        float thrust = Input.GetKey(KeyCode.W) ? 1f : Input.GetKey(KeyCode.S) ? -1f : 0f;
        float strafe = Input.GetKey(KeyCode.D) ? 1f : Input.GetKey(KeyCode.A) ? -1f : 0f;
        float lift = Input.GetKey(KeyCode.Space) ? 1f : Input.GetKey(KeyCode.LeftShift) ? -1f : 0f;
        
        // Check boost
        isBoosting = Input.GetKey(KeyCode.LeftControl) && currentFuel > 0;
        float boostMod = isBoosting ? boostMultiplier : 1f;
        
        // Apply forces in local space
        Vector3 moveForce = Vector3.zero;
        moveForce += transform.forward * thrust * thrustPower * boostMod;
        moveForce += transform.right * strafe * strafePower * boostMod;
        moveForce += transform.up * lift * liftPower * boostMod;
        
        rb.AddForce(moveForce);
    }
    
    void HandleRotation() 
    {
        // Mouse look
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
        
        // Manual roll with Q/E
        float roll = 0f;
        if (Input.GetKey(KeyCode.Q)) roll = -1f;
        if (Input.GetKey(KeyCode.E)) roll = 1f;
        roll *= rotationSpeed * 2f;
        
        Vector3 torque = new Vector3(-mouseY, mouseX, roll);
        rb.AddTorque(torque);
    }
    
    void HandleWeapons() 
    {
        if (Input.GetMouseButton(0) && Time.time > nextFireTime) 
        {
            FirePrimary();
            nextFireTime = Time.time + fireRate;
        }
    }
    
    void FirePrimary() 
    {
        // Simple laser effect for now
        foreach (Transform mount in weaponMounts) 
        {
            if (mount != null) 
            {
                Debug.DrawRay(mount.position, mount.forward * 100f, Color.cyan, 0.1f);
                Debug.Log("Laser fired!");
            }
        }
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
    
    void OnTriggerEnter(Collider other) 
    {
        if (other.CompareTag("TokenPickup")) 
        {
            TokenPickup token = other.GetComponent<TokenPickup>();
            if (token != null)
            {
                collectedTokens += token.value;
                Debug.Log($"Collected {token.value} PLASMA! Total: {collectedTokens}");
                Destroy(other.gameObject);
            }
        }
    }
    
    public void TakeDamage(float damage) 
    {
        shipBase.currentShield -= damage;
        Debug.Log($"Ship took {damage} damage! Shield: {shipBase.currentShield:F1}");
        
        if (shipBase.currentShield <= 0) 
        {
            OnShipDestroyed();
        }
    }
    
    void OnShipDestroyed() 
    {
        Debug.Log("Ship destroyed! Respawning...");
        // Reset position and shields
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        shipBase.currentShield = shipBase.maxShield;
        
        // Drop 50% of tokens
        int droppedTokens = collectedTokens / 2;
        collectedTokens -= droppedTokens;
        Debug.Log($"Dropped {droppedTokens} PLASMA tokens");
    }
}