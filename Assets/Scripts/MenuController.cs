using UnityEngine;
using UnityEngine.SceneManagement; // For scene management
using UnityEngine.UI; // For button functionality

public class MenuController : MonoBehaviour
{
    public Button startButton; // Reference to the Start button
    public Button quitButton;  // Reference to the Quit button

    void Start()
    {
        // Ensure buttons are set up correctly in the Unity inspector
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitButtonClicked);
        }
    }

    // This function is called when the Start button is clicked
    void OnStartButtonClicked()
    {
        // Load the "HubRoom" scene
        SceneManager.LoadScene("HubRoom");
    }

    // This function is called when the Quit button is clicked
    void OnQuitButtonClicked()
    {
        // Quit the application
        Application.Quit();

        // In the Unity Editor, Application.Quit does not actually close the editor
        // So, if you're testing in the editor, you can add this line to stop the playmode:
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}