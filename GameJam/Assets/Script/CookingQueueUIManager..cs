using UnityEngine;

public class CookingQueueUIManager : MonoBehaviour
{
    public static CookingQueueUIManager Instance;

    public Transform uiParent; // set to CookingQueueUI
    public GameObject cookingTaskUIPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public Transform CreateTaskUI(string foodType, Sprite icon)
    {
        GameObject ui = Instantiate(cookingTaskUIPrefab, uiParent);

        // Set icon
        ui.transform.Find("Icon").GetComponent<UnityEngine.UI.Image>().sprite = icon;

        return ui.transform;
    }
}
