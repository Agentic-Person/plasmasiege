using UnityEngine;

public class FlyingShipFixed : MonoBehaviour 
{
    [Header("Movement")]
    public float thrustPower = 15f;
    public float rotationSpeed = 2f;
    public float maxSpeed = 20f;
    
    [Header("Controls")]
    public bool invertPitch = false;
    public bool invertYaw = false;
    public float maxRollAngle = 45f;
    public float mouseConstraintRadius = 200f;
    
    [Header("Camera")]
    public float cameraDistance = 10f;
    public float cameraHeight = 3f;
    public float cameraSmooth = 2f;
    
    private Rigidbody rb;
    private GameObject ship;
    private Camera cam;
    private float currentRoll = 0f;
    private Vector2 screenCenter;
    
    void Start() 
    {
        Debug.Log("CREATING IMPROVED FLYING SHIP");
        
        // Create ship
        ship = GameObject.CreatePrimitive(PrimitiveType.Cube);
        ship.name = "FlyingShip";
        ship.transform.position = Vector3.zero;
        ship.transform.localScale = new Vector3(2, 0.5f, 3);
        
        // Add physics
        rb = ship.AddComponent<Rigidbody>();
        rb.useGravity = false;
        rb.linearDamping = 2f;  // More damping for better control
        rb.angularDamping = 5f;
        rb.mass = 1f;
        
        // Make it cyan
        Material mat = ship.GetComponent<Renderer>().material;
        mat.color = Color.cyan;
        
        // Get camera reference
        cam = Camera.main;
        
        // Initialize screen center for mouse constraint
        screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        
        Debug.Log("SHIP READY - W/S=Forward/Back, A/D=Roll, Mouse=Look, Space/Shift=Up/Down");
    }
    
    void Update() 
    {
        if (rb == null || ship == null) return;
        
        HandleMovement();
        HandleRotation();
        UpdateCamera();
        
        // Debug info
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log($"Speed: {rb.linearVelocity.magnitude:F1} m/s | Position: {ship.transform.position}");
        }
    }
    
    void HandleMovement()
    {
        Vector3 force = Vector3.zero;
        
        // Forward/back
        if (Input.GetKey(KeyCode.W)) force += ship.transform.forward;
        if (Input.GetKey(KeyCode.S)) force -= ship.transform.forward;
        
        // Up/down
        if (Input.GetKey(KeyCode.Space)) force += ship.transform.up;
        if (Input.GetKey(KeyCode.LeftShift)) force -= ship.transform.up;
        
        // Apply force
        rb.AddForce(force * thrustPower);
        
        // Speed limiting
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }
    
    void HandleRotation()
    {
        // Constrain mouse to a circular area around screen center
        Vector2 mousePos = Input.mousePosition;
        Vector2 deltaFromCenter = mousePos - screenCenter;
        
        // If mouse is outside the constraint radius, clamp it
        if (deltaFromCenter.magnitude > mouseConstraintRadius)
        {
            deltaFromCenter = deltaFromCenter.normalized * mouseConstraintRadius;
            Vector2 clampedMousePos = screenCenter + deltaFromCenter;
            
            // Force mouse position back to constrained area
            Cursor.lockState = CursorLockMode.Locked;
        }
        
        // Mouse look with constrained movement
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;
        
        // Apply inversion if enabled
        if (invertYaw) mouseX = -mouseX;
        if (invertPitch) mouseY = -mouseY;
        
        ship.transform.Rotate(-mouseY, mouseX, 0, Space.Self);
        
        // Roll with A/D keys with limiting
        float rollInput = 0f;
        if (Input.GetKey(KeyCode.A)) rollInput = 1f;
        if (Input.GetKey(KeyCode.D)) rollInput = -1f;
        
        if (rollInput != 0f)
        {
            float targetRoll = currentRoll + rollInput * rotationSpeed * 50 * Time.deltaTime;
            targetRoll = Mathf.Clamp(targetRoll, -maxRollAngle, maxRollAngle);
            
            float rollDelta = targetRoll - currentRoll;
            ship.transform.Rotate(0, 0, rollDelta);
            currentRoll = targetRoll;
        }
        else
        {
            // Auto-level roll when no input (optional)
            if (Mathf.Abs(currentRoll) > 0.1f)
            {
                float levelSpeed = 30f; // degrees per second
                float rollDelta = -currentRoll * levelSpeed * Time.deltaTime;
                rollDelta = Mathf.Clamp(rollDelta, -Mathf.Abs(currentRoll), Mathf.Abs(currentRoll));
                
                ship.transform.Rotate(0, 0, rollDelta);
                currentRoll += rollDelta;
                
                if (Mathf.Abs(currentRoll) < 0.1f) currentRoll = 0f;
            }
        }
        
        // Mouse buttons disabled for GUI interaction
        // TODO: Add weapon firing later when GUI testing is complete
    }
    
    void UpdateCamera()
    {
        if (cam == null || ship == null) return;
        
        // Camera follows behind and above ship
        Vector3 targetPos = ship.transform.position - ship.transform.forward * cameraDistance + ship.transform.up * cameraHeight;
        
        // Smooth movement
        cam.transform.position = Vector3.Lerp(cam.transform.position, targetPos, cameraSmooth * Time.deltaTime);
        
        // Look at ship
        cam.transform.LookAt(ship.transform);
    }
}