using UnityEngine;
using UnityEngine.UI;

public class CookingTaskUI : MonoBehaviour
{
    [Header("UI")]
    public Image iconImage;
    public Image radialFill;   // Image with Fill Method = Radial360

    private ChefStation chefStation;
    private FoodType foodType;
    private float duration;
    private float timer;
    private bool isCooking;

    public void Init(ChefStation station, FoodType type, float time, Sprite iconSprite)
    {
        chefStation = station;
        foodType = type;
        duration = time;

        if (iconImage != null)
            iconImage.sprite = iconSprite;

        if (radialFill != null)
            radialFill.fillAmount = 0f;

        timer = 0f;
        isCooking = true;
        gameObject.SetActive(true);
    }

    private void Update()
    {
        if (!isCooking) return;

        timer += Time.deltaTime;
        float progress = Mathf.Clamp01(timer / duration);

        if (radialFill != null)
            radialFill.fillAmount = progress;

        if (progress >= 1f)
        {
            isCooking = false;
            // Tell chef we're done
            chefStation.NotifyTaskFinished(this, foodType);
            Destroy(gameObject);
        }
    }
}
