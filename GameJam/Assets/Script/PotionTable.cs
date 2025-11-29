using UnityEngine;

public class PotionTable : MonoBehaviour
{
    [Header("食物在桌子上的摆放点")]
    public Transform workPoint;  // 把食物放在哪个位置

    [Header("成品旋转角度（可在 Inspector 自己调节）")]
    public Vector3 resultRotationOffset;   // ⭐ 新增：旋转角度偏移

    [HideInInspector]
    public FoodItem currentFood; // 当前在桌子上的那份食物

    public void PlaceFood(FoodItem food)
    {
        if (food == null) return;

        currentFood = food;
        food.transform.SetParent(null);

        if (workPoint != null)
        {
            food.transform.position = workPoint.position;
            food.transform.rotation = workPoint.rotation;
        }

        var rb = food.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Debug.Log("把食物放到调制桌：" + food.name);

        PotionUI.Instance.OpenPanel(this);
    }

    public void ApplyResult(bool success)
    {
        if (currentFood == null)
            return;

        GameObject resultPrefab = success ?
            currentFood.correctResultPrefab :
            currentFood.poisonResultPrefab;

        if (resultPrefab == null)
        {
            Debug.LogWarning("结果 prefab 没有设置！");
            return;
        }

        FoodItem newFood = ReplaceFood(currentFood, resultPrefab);
        currentFood = newFood;
    }

    private FoodItem ReplaceFood(FoodItem oldFood, GameObject prefab)
    {
        Vector3 pos = oldFood.transform.position;
        Quaternion rot = oldFood.transform.rotation;

        // ⭐ 实例化新食物
        GameObject newObj = Instantiate(prefab, pos, rot);

        // ⭐⭐ 应用可调节的旋转偏移
        newObj.transform.rotation *= Quaternion.Euler(resultRotationOffset);

        // 添加或获取 FoodItem
        FoodItem newItem = newObj.GetComponent<FoodItem>();
        if (newItem == null)
            newItem = newObj.AddComponent<FoodItem>();

        // 设置 foodPrefabId 为预制体资产（避免 Clone 问题）
        newItem.foodPrefabId = prefab;

        // 删除旧的半成品
        Destroy(oldFood.gameObject);

        return newItem;
    }
}

