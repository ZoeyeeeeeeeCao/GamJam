using UnityEngine;

public class PotionTable : MonoBehaviour
{
    [Header("食物在桌子上的摆放点")]
    public Transform workPoint;  // 把食物放在哪个位置

    [HideInInspector]
    public FoodItem currentFood; // 当前在桌子上的那份食物

    /// <summary>
    /// 玩家把食物放到这个桌子上时调用
    /// </summary>
    public void PlaceFood(FoodItem food)
    {
        if (food == null) return;

        currentFood = food;

        // 解除父子关系（不再挂在玩家手上）
        food.transform.SetParent(null);

        // 把食物移到工作台中心
        if (workPoint != null)
        {
            food.transform.position = workPoint.position;
            food.transform.rotation = workPoint.rotation;
        }

        // 恢复物理（放在桌子上应该受重力）
        var rb = food.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Debug.Log("把食物放到调制桌：" + food.name);

        // 打开 UI 面板，让玩家点 ABC
        PotionUI.Instance.OpenPanel(this);
    }

    /// <summary>
    /// 当玩家完成 ABC 点选后，由 UI 回调，success 表示顺序是否正确
    /// </summary>
    public void ApplyResult(bool success)
    {
        if (currentFood == null)
            return;

        GameObject resultPrefab;

        if (success)
        {
            Debug.Log("配方成功！变成正确版本食物。");
            resultPrefab = currentFood.correctResultPrefab;
        }
        else
        {
            Debug.Log("配方失败，变成下毒食物！");
            resultPrefab = currentFood.poisonResultPrefab;
        }

        if (resultPrefab == null)
        {
            Debug.LogWarning("结果 prefab 没有设置，检查 FoodItem 上的 correctResultPrefab / poisonResultPrefab");
            return;
        }

        // 替换成新食物
        FoodItem newFood = ReplaceFood(currentFood, resultPrefab);
        currentFood = newFood;
    }

    private FoodItem ReplaceFood(FoodItem oldFood, GameObject prefab)
    {
        Vector3 pos = oldFood.transform.position;
        Quaternion rot = oldFood.transform.rotation;

        GameObject newObj = Instantiate(prefab, pos, rot);
        Destroy(oldFood.gameObject);

        return newObj.GetComponent<FoodItem>();
    }
}
