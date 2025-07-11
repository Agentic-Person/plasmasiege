using UnityEngine;

public class CreateCube : MonoBehaviour 
{
    void Start() 
    {
        Debug.Log("STARTING CUBE CREATION");
        
        // Create cube
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = "RedCube";
        cube.transform.position = new Vector3(0, 0, 5);
        cube.transform.localScale = new Vector3(3, 3, 3);
        
        // Make it bright red
        Material mat = cube.GetComponent<Renderer>().material;
        mat.color = Color.red;
        
        Debug.Log("RED CUBE CREATED - YOU SHOULD SEE IT NOW");
    }
}