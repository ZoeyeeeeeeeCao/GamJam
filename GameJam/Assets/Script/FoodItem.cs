using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [Header("这个食物对应的“菜品Prefab ID”")]
    public GameObject foodPrefabId;
    // 这里建议直接拖“对应的原始 prefab”（比如 NormalBurgerPrefab / PoisonBurgerPrefab）
}
