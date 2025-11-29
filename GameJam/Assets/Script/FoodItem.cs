using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [Header("唯一食物ID（用于给顾客判断）")]
    public GameObject foodPrefabId;          // 👉 一定是 Project 里的 prefab

    [Header("正常处理后的成品（正确版本）")]
    public GameObject correctResultPrefab;   // 半成品用，成品可以留空

    [Header("错误处理后的成品（下毒版本）")]
    public GameObject poisonResultPrefab;    // 半成品用，成品可以留空

    [Header("正确的处理顺序（例如 ABC）")]
    public string[] correctOrder;            // 半成品用，成品可以留空

    [HideInInspector]
    public WindowCounter ownerWindow; 


}
