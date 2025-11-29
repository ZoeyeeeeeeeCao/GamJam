using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [Header("唯一食物ID（用于给顾客判断）")]
    public GameObject foodPrefabId;

    [Header("正常处理后的成品（正确版本）")]
    public GameObject correctResultPrefab;

    [Header("错误处理后的成品（下毒版本）")]
    public GameObject poisonResultPrefab;

    [Header("正确的处理顺序（例如 ABC）")]
    public string[] correctOrder;
}
