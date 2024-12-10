using UnityEditor.Search;
using UnityEngine;

public class MapDanceMove : MonoBehaviour
{
    public GameObject[] mappedObjects = new GameObject[6]; 

    public Material incorrectMaterial;
    public Material correctMaterial;

    void Update()
    {
        for (int i = 0; i < 6; i++)
        {
            // Check if the number key (1-6) is pressed
            if (Input.GetKeyDown((KeyCode)(KeyCode.Alpha1 + i))) // maps 1 -> 6 to keys 1 to 6
            {
                Debug.Log("You pressed number: " + (i + 1));
                
                // Change the material for the selected banana man (based on the index)
                ChangeMaterial(i);
            }
        }
    }

    public void ChangeMaterial(int objectIndex)
    {
        if (objectIndex < 0 || objectIndex >= mappedObjects.Length) return; // Ensure the index is within range

        // Get the main object
        GameObject selectedObject = mappedObjects[objectIndex];

        // Iterate over all child objects of the selected GameObject
        Renderer[] renderers = selectedObject.GetComponentsInChildren<Renderer>();

        // Loop through all the renderers in the children
        foreach (Renderer renderer in renderers)
        {
            // Change the material of each child renderer
            renderer.material = incorrectMaterial;
        }
    }

    public void ResetMaterial()
    {  
        int objectIndex = 0;
        while (objectIndex < 6)
        {
            GameObject selectedObject = mappedObjects[objectIndex];

            Renderer[] renderers = selectedObject.GetComponentsInChildren<Renderer>();

            foreach (Renderer renderer in renderers)
            {
                renderer.material = correctMaterial;
            }
        }
    } 
       
}      
      
       
       
      
       
       
       
       
       
       