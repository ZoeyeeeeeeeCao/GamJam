using UnityEngine;
using UnityEngine.UI;

public class FoodCookingTask : MonoBehaviour
{
    [Header("Food Data")]
    public string foodType;
    public float cookingTime = 5f;

    [Header("UI Elements")]
    public Image radialFill;
    public Image foodIcon;
    public GameObject uiRoot;

    private float timer = 0f;
    private bool isCooking = false;
    private ChefStation chefStation;

    public void BeginCooking(ChefStation station)
    {
        chefStation = station;
        timer = 0f;
        isCooking = true;

        uiRoot.SetActive(true);
        radialFill.fillAmount = 0f;
    }

    private void Update()
    {
        if (!isCooking) return;

        timer += Time.deltaTime;
        float progress = timer / cookingTime;
        radialFill.fillAmount = progress;

        if (progress >= 1f)
        {
            isCooking = false;
            uiRoot.SetActive(false);

            chefStation.OnCookingFinished(foodType, this);
            Destroy(gameObject);
        }
    }
}
