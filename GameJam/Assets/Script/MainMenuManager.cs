using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Names")]
    public string tutorialSceneName = "TutorialScene";  // change to your real scene name

    // Called by Start Button
    public void StartGame()
    {
        SceneManager.LoadScene(tutorialSceneName);
    }

    // Called by Exit Button
    public void ExitGame()
    {
        Debug.Log("Quit game");

        // Quit in build
        Application.Quit();

        // Quit inside Unity Editor
#if UNITY_EDITOR
       // UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
