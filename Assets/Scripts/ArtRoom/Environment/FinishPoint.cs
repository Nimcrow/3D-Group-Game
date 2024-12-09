using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishPoint : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private BoxCollider boxCollider;

    private void Awake()
    {
        // Get references to MeshRenderer and BoxCollider
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();

        // Ensure the finish point is initially disabled
        if (meshRenderer != null) meshRenderer.enabled = false;
        if (boxCollider != null) boxCollider.enabled = false;
    }

    public void ActivateFinishPoint()
    {
        // Enable MeshRenderer and BoxCollider when called
        if (meshRenderer != null) meshRenderer.enabled = true;
        if (boxCollider != null) boxCollider.enabled = true;

        Debug.Log("Finish point activated!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LoadHubRoom();
        }
    }

    private void LoadHubRoom()
    {
        SceneManager.LoadScene(0); // Always load HubRoom, assuming it's at index 0
    }
}
