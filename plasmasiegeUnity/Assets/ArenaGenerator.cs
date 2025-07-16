using UnityEngine;
using System.Collections.Generic;

public class ArenaGenerator : MonoBehaviour 
{
    [Header("Arena Settings")]
    public Vector3 arenaSize = new Vector3(100, 60, 100);
    public bool showBounds = true;
    public bool showGridLines = true;
    public bool showDirectionalMarkers = true;
    
    [Header("Environmental Objects")]
    public int asteroidCount = 15;
    public float minAsteroidSize = 1f;
    public float maxAsteroidSize = 4f;
    
    [Header("Reference Objects")]
    public int gridSpacing = 20;
    public bool showFloor = true;
    public bool showCeiling = true;
    
    [Header("Arena Controls")]
    public KeyCode regenerateKey = KeyCode.R;
    public bool autoGenerate = true;
    
    private List<GameObject> arenaObjects = new List<GameObject>();
    private GameObject arenaParent;
    
    void Start()
    {
        Debug.Log("CREATING ARENA ENVIRONMENT");
        
        if (autoGenerate)
        {
            GenerateArena();
        }
        
        Debug.Log($"ARENA READY - Size: {arenaSize} | Press {regenerateKey} to regenerate");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(regenerateKey))
        {
            RegenerateArena();
        }
    }
    
    public void GenerateArena()
    {
        ClearArena();
        CreateArenaParent();
        
        if (showBounds) CreateArenaBounds();
        if (showGridLines) CreateGridSystem();
        if (showDirectionalMarkers) CreateDirectionalMarkers();
        if (showFloor) CreateFloorCeiling();
        CreateAsteroids();
        CreateTestObjects();
        
        Debug.Log($"Arena generated with {arenaObjects.Count} objects");
    }
    
    public void RegenerateArena()
    {
        Debug.Log("Regenerating arena...");
        GenerateArena();
    }
    
    void CreateArenaParent()
    {
        arenaParent = new GameObject("Arena");
        arenaParent.transform.position = Vector3.zero;
    }
    
    void CreateArenaBounds()
    {
        // Create 6 boundary walls (invisible barriers)
        float halfX = arenaSize.x / 2f;
        float halfY = arenaSize.y / 2f;
        float halfZ = arenaSize.z / 2f;
        
        // Wall positions and scales
        Vector3[] positions = {
            new Vector3(halfX, 0, 0),     // Right wall
            new Vector3(-halfX, 0, 0),    // Left wall
            new Vector3(0, halfY, 0),     // Top wall
            new Vector3(0, -halfY, 0),    // Bottom wall
            new Vector3(0, 0, halfZ),     // Front wall
            new Vector3(0, 0, -halfZ)     // Back wall
        };
        
        Vector3[] scales = {
            new Vector3(1, arenaSize.y, arenaSize.z),     // Right wall
            new Vector3(1, arenaSize.y, arenaSize.z),     // Left wall
            new Vector3(arenaSize.x, 1, arenaSize.z),     // Top wall
            new Vector3(arenaSize.x, 1, arenaSize.z),     // Bottom wall
            new Vector3(arenaSize.x, arenaSize.y, 1),     // Front wall
            new Vector3(arenaSize.x, arenaSize.y, 1)      // Back wall
        };
        
        Color[] colors = { Color.red, Color.green, Color.blue, Color.yellow, Color.magenta, Color.cyan };
        
        for (int i = 0; i < positions.Length; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = $"BoundaryWall_{i}";
            wall.transform.position = positions[i];
            wall.transform.localScale = scales[i];
            wall.transform.SetParent(arenaParent.transform);
            
            // Make walls semi-transparent
            Material mat = wall.GetComponent<Renderer>().material;
            mat.color = new Color(colors[i].r, colors[i].g, colors[i].b, 0.2f);
            
            arenaObjects.Add(wall);
        }
    }
    
    void CreateGridSystem()
    {
        // Create reference grid lines
        float halfX = arenaSize.x / 2f;
        float halfY = arenaSize.y / 2f;
        float halfZ = arenaSize.z / 2f;
        
        // Vertical grid lines (X direction)
        for (int x = -Mathf.FloorToInt(halfX / gridSpacing); x <= Mathf.FloorToInt(halfX / gridSpacing); x++)
        {
            for (int z = -Mathf.FloorToInt(halfZ / gridSpacing); z <= Mathf.FloorToInt(halfZ / gridSpacing); z++)
            {
                if (x == 0 && z == 0) continue; // Skip center
                
                Vector3 pos = new Vector3(x * gridSpacing, 0, z * gridSpacing);
                if (IsInsideArena(pos))
                {
                    GameObject gridLine = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                    gridLine.name = $"GridLine_{x}_{z}";
                    gridLine.transform.position = pos;
                    gridLine.transform.localScale = new Vector3(0.2f, halfY, 0.2f);
                    gridLine.transform.SetParent(arenaParent.transform);
                    
                    Material mat = gridLine.GetComponent<Renderer>().material;
                    mat.color = new Color(0.5f, 0.5f, 0.5f, 0.6f);
                    
                    arenaObjects.Add(gridLine);
                }
            }
        }
    }
    
    void CreateDirectionalMarkers()
    {
        // Create UP marker (green sphere at top center)
        GameObject upMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        upMarker.name = "UP_Marker";
        upMarker.transform.position = new Vector3(0, arenaSize.y / 2f - 5, 0);
        upMarker.transform.localScale = Vector3.one * 3f;
        upMarker.transform.SetParent(arenaParent.transform);
        
        Material upMat = upMarker.GetComponent<Renderer>().material;
        upMat.color = Color.green;
        
        arenaObjects.Add(upMarker);
        
        // Create DOWN marker (red sphere at bottom center)
        GameObject downMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        downMarker.name = "DOWN_Marker";
        downMarker.transform.position = new Vector3(0, -arenaSize.y / 2f + 5, 0);
        downMarker.transform.localScale = Vector3.one * 3f;
        downMarker.transform.SetParent(arenaParent.transform);
        
        Material downMat = downMarker.GetComponent<Renderer>().material;
        downMat.color = Color.red;
        
        arenaObjects.Add(downMarker);
        
        // Create cardinal direction markers
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };
        Color[] dirColors = { Color.blue, Color.yellow, Color.magenta, Color.cyan };
        string[] dirNames = { "NORTH", "SOUTH", "EAST", "WEST" };
        
        for (int i = 0; i < directions.Length; i++)
        {
            Vector3 pos = directions[i] * (arenaSize.x / 2f - 8);
            pos.y = 0;
            
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = $"{dirNames[i]}_Marker";
            marker.transform.position = pos;
            marker.transform.localScale = new Vector3(2, 8, 2);
            marker.transform.SetParent(arenaParent.transform);
            
            Material mat = marker.GetComponent<Renderer>().material;
            mat.color = dirColors[i];
            
            arenaObjects.Add(marker);
        }
    }
    
    void CreateFloorCeiling()
    {
        if (showFloor)
        {
            // Create floor plane
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Plane);
            floor.name = "Arena_Floor";
            floor.transform.position = new Vector3(0, -arenaSize.y / 2f, 0);
            floor.transform.localScale = new Vector3(arenaSize.x / 10f, 1, arenaSize.z / 10f);
            floor.transform.SetParent(arenaParent.transform);
            
            Material floorMat = floor.GetComponent<Renderer>().material;
            floorMat.color = new Color(0.3f, 0.3f, 0.3f, 0.8f);
            
            arenaObjects.Add(floor);
        }
        
        if (showCeiling)
        {
            // Create ceiling plane
            GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ceiling.name = "Arena_Ceiling";
            ceiling.transform.position = new Vector3(0, arenaSize.y / 2f, 0);
            ceiling.transform.rotation = Quaternion.Euler(180, 0, 0);
            ceiling.transform.localScale = new Vector3(arenaSize.x / 10f, 1, arenaSize.z / 10f);
            ceiling.transform.SetParent(arenaParent.transform);
            
            Material ceilingMat = ceiling.GetComponent<Renderer>().material;
            ceilingMat.color = new Color(0.2f, 0.2f, 0.4f, 0.6f);
            
            arenaObjects.Add(ceiling);
        }
    }
    
    void CreateAsteroids()
    {
        for (int i = 0; i < asteroidCount; i++)
        {
            // Random position within arena bounds (but not too close to center)
            Vector3 pos;
            int attempts = 0;
            do 
            {
                pos = new Vector3(
                    Random.Range(-arenaSize.x / 2f + 5, arenaSize.x / 2f - 5),
                    Random.Range(-arenaSize.y / 2f + 5, arenaSize.y / 2f - 5),
                    Random.Range(-arenaSize.z / 2f + 5, arenaSize.z / 2f - 5)
                );
                attempts++;
            } 
            while (Vector3.Distance(pos, Vector3.zero) < 15f && attempts < 50);
            
            GameObject asteroid = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            asteroid.name = $"Asteroid_{i}";
            asteroid.transform.position = pos;
            
            // Random size and rotation
            float size = Random.Range(minAsteroidSize, maxAsteroidSize);
            asteroid.transform.localScale = Vector3.one * size;
            asteroid.transform.rotation = Random.rotation;
            asteroid.transform.SetParent(arenaParent.transform);
            
            // Random gray color
            Material mat = asteroid.GetComponent<Renderer>().material;
            float grayValue = Random.Range(0.2f, 0.8f);
            mat.color = new Color(grayValue, grayValue, grayValue);
            
            // Add Rigidbody for potential collisions
            Rigidbody rb = asteroid.AddComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
            
            arenaObjects.Add(asteroid);
        }
    }
    
    void CreateTestObjects()
    {
        // Create a test tunnel/ring to fly through
        Vector3 tunnelPos = new Vector3(arenaSize.x / 3f, 0, 0);
        
        for (int i = 0; i < 8; i++)
        {
            float angle = i * 45f * Mathf.Deg2Rad;
            Vector3 ringPos = tunnelPos + new Vector3(0, Mathf.Cos(angle) * 12f, Mathf.Sin(angle) * 12f);
            
            GameObject ringSegment = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ringSegment.name = $"TestRing_{i}";
            ringSegment.transform.position = ringPos;
            ringSegment.transform.localScale = new Vector3(3, 2, 2);
            ringSegment.transform.SetParent(arenaParent.transform);
            
            Material mat = ringSegment.GetComponent<Renderer>().material;
            mat.color = Color.white;
            
            arenaObjects.Add(ringSegment);
        }
        
        // Create some target practice objects
        for (int i = 0; i < 5; i++)
        {
            Vector3 targetPos = new Vector3(
                Random.Range(-arenaSize.x / 3f, arenaSize.x / 3f),
                Random.Range(-arenaSize.y / 3f, arenaSize.y / 3f),
                arenaSize.z / 2f - 10
            );
            
            GameObject target = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            target.name = $"Target_{i}";
            target.transform.position = targetPos;
            target.transform.localScale = new Vector3(3, 0.5f, 3);
            target.transform.rotation = Quaternion.Euler(90, 0, 0);
            target.transform.SetParent(arenaParent.transform);
            
            Material mat = target.GetComponent<Renderer>().material;
            mat.color = new Color(1f, 0.5f, 0f); // Orange
            
            arenaObjects.Add(target);
        }
    }
    
    bool IsInsideArena(Vector3 position)
    {
        return Mathf.Abs(position.x) < arenaSize.x / 2f &&
               Mathf.Abs(position.y) < arenaSize.y / 2f &&
               Mathf.Abs(position.z) < arenaSize.z / 2f;
    }
    
    void ClearArena()
    {
        foreach (GameObject obj in arenaObjects)
        {
            if (obj != null)
            {
                DestroyImmediate(obj);
            }
        }
        arenaObjects.Clear();
        
        if (arenaParent != null)
        {
            DestroyImmediate(arenaParent);
        }
    }
    
    void OnDestroy()
    {
        ClearArena();
    }
} 