using UnityEngine;

public class PlayerCarryFood : MonoBehaviour
{
    [Header("玩家手上食物挂载点")]
    public Transform holdPoint;

    [Header("交互设置")]
    public KeyCode interactKey = KeyCode.E;
    public string foodTriggerTag = "FoodTrigger";   // 食物交互区的Tag
    public string tableTriggerTag = "TableTrigger"; // 桌子交互区的Tag

    private FoodItem heldFood;           // 目前拿着的食物
    private FoodItem foodInRange;        // 脚下可拾取食物
    private TableServeArea tableInRange; // 脚下可放置的桌子

    // ⭐ 玩家自身所有 Collider（包括子物体）
    private Collider[] playerColliders;
    // ⭐ 当前拿在手上的这一份食物的 Collider
    private Collider[] heldFoodColliders;

    private void Awake()
    {
        // 记录玩家上所有的 Collider，后面忽略碰撞会用到
        playerColliders = GetComponentsInChildren<Collider>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (heldFood == null)
                TryPickupFood();
            else
                TryPlaceFoodOnTable();
        }
    }

    // =============================
    // ① 拾取食物
    // =============================
    private void TryPickupFood()
    {
        if (foodInRange == null) return;

        heldFood = foodInRange;
        foodInRange = null;

        // 把食物挂到手上
        heldFood.transform.SetParent(holdPoint);
        heldFood.transform.localPosition = Vector3.zero;
        heldFood.transform.localRotation = Quaternion.identity;

        // 禁用物理（不被力推动）
        Rigidbody rb = heldFood.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // 记录这份食物上的所有 Collider
        heldFoodColliders = heldFood.GetComponentsInChildren<Collider>();

        // ⭐ 忽略“玩家 Collider”与“这份食物 Collider”的碰撞
        if (playerColliders != null && heldFoodColliders != null)
        {
            foreach (var pc in playerColliders)
            {
                if (pc == null || pc.isTrigger) continue;

                foreach (var fc in heldFoodColliders)
                {
                    if (fc == null || fc.isTrigger) continue;

                    Physics.IgnoreCollision(pc, fc, true);
                }
            }
        }

        Debug.Log("拿起食物：" + heldFood.name);
    }

    // =============================
    // ② 放下食物（放到桌子上）
    // =============================
    private void TryPlaceFoodOnTable()
    {
        if (heldFood == null || tableInRange == null) return;

        // 先恢复玩家与这份食物的碰撞
        if (playerColliders != null && heldFoodColliders != null)
        {
            foreach (var pc in playerColliders)
            {
                if (pc == null || pc.isTrigger) continue;

                foreach (var fc in heldFoodColliders)
                {
                    if (fc == null || fc.isTrigger) continue;

                    Physics.IgnoreCollision(pc, fc, false);
                }
            }
        }

        // 把食物交给桌子处理（摆到桌面中心 + 判定顾客反应）
        tableInRange.PlaceFood(heldFood);

        // 恢复刚体物理
        Rigidbody rb = heldFood.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        heldFood.transform.SetParent(null);
        heldFood = null;
        heldFoodColliders = null;

        Debug.Log("放下食物");
    }

    // =============================
    // ③ 触发区检测
    // =============================
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(foodTriggerTag))
        {
            var food = other.GetComponentInParent<FoodItem>();
            if (food != null)
            {
                foodInRange = food;
                Debug.Log("进入食物交互区：" + food.name);
            }
        }
        else if (other.CompareTag(tableTriggerTag))
        {
            var table = other.GetComponentInParent<TableServeArea>();
            if (table != null)
            {
                tableInRange = table;
                Debug.Log("进入桌子交互区：" + table.name);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(foodTriggerTag))
        {
            var food = other.GetComponentInParent<FoodItem>();
            if (food != null && food == foodInRange)
            {
                foodInRange = null;
                Debug.Log("离开食物交互区");
            }
        }
        else if (other.CompareTag(tableTriggerTag))
        {
            var table = other.GetComponentInParent<TableServeArea>();
            if (table != null && table == tableInRange)
            {
                tableInRange = null;
                Debug.Log("离开桌子交互区");
            }
        }
    }
}
