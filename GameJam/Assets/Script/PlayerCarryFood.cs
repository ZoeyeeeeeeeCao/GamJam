using UnityEngine;

public class PlayerCarryFood : MonoBehaviour
{
    [Header("玩家手上食物挂载点")]
    public Transform holdPoint;

    [Header("交互设置")]
    public KeyCode interactKey = KeyCode.E;
    public string foodTriggerTag = "FoodTrigger";   // 食物交互区的Tag
    public string tableTriggerTag = "TableTrigger"; // 桌子交互区的Tag

    [Header("下毒桌设置")]
    public string potionTableTag = "PotionTable";    // 下毒桌触发区的 Tag
    private PotionTable potionInRange;               // 当前范围内的下毒桌

    [Header("丢在地上参数（仍然用 E 触发）")]
    public float dropForwardOffset = 0.5f;     // 丢出时在玩家前方多远
    public float dropUpOffset = 0.5f;          // 丢出时在玩家上方多高
    public float throwForwardForce = 1.2f;     // 往前推力
    public float throwUpForce = 0.6f;          // 往上推力

    private FoodItem heldFood;           // 目前拿着的食物
    private FoodItem foodInRange;        // 脚下可拾取食物
    private TableServeArea tableInRange; // 脚下可放置的桌子
    public bool IsCarryingFood => heldFood != null;

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
            {
                // 手上没有食物 → 尝试捡起
                TryPickupFood();
            }
            else
            {
                // 手上有食物 → 放桌子 / 下毒桌 / 丢在地上
                TryPlaceFoodOrDrop();
            }
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
    // ② 放桌子 / 下毒桌 / 丢地上（统一在这里判断）
    // =============================
    private void TryPlaceFoodOrDrop()
    {
        if (heldFood == null) return;

        // ① 先恢复玩家与这份食物的碰撞（不管最终放哪）
        RestoreCollisionWithHeldFood();

        // ② 如果有下毒桌在范围内，优先放到下毒桌
        if (potionInRange != null)
        {
            potionInRange.PlaceFood(heldFood);

            // 下毒桌逻辑内部会自己处理刚体/位置
            heldFood = null;
            heldFoodColliders = null;
            Debug.Log("把食物放到下毒桌。");
            return;
        }

        // ③ 否则，如果有普通桌子，则放到普通桌
        if (tableInRange != null)
        {
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

            Debug.Log("把食物放到普通桌子上。");
            return;
        }

        // ④ 两种桌子都没有 → 丢在地上
        DropFoodOnGround();
    }

    // =============================
    // ③ 丢在地上（仍然用 E）
    // =============================
    private void DropFoodOnGround()
    {
        if (heldFood == null) return;

        heldFood.transform.SetParent(null);

        Rigidbody rb = heldFood.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            Transform t = transform;

            // 丢出的起始位置：玩家前方 + 上方一点
            Vector3 dropPos =
                t.position +
                t.forward * dropForwardOffset +
                Vector3.up * dropUpOffset;

            rb.position = dropPos;

            // 清空当前速度
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // 给一点往前 + 往上的推力
            Vector3 force =
                t.forward * throwForwardForce +
                Vector3.up * throwUpForce;

            rb.AddForce(force, ForceMode.Impulse);
        }

        Debug.Log("把食物扔在地上：" + heldFood.name);

        heldFood = null;
        heldFoodColliders = null;
    }

    // =============================
    // ④ 恢复玩家与手上食物的碰撞
    // =============================
    private void RestoreCollisionWithHeldFood()
    {
        if (playerColliders == null || heldFoodColliders == null) return;

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

    // =============================
    // ⑤ 触发区检测
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
        else if (other.CompareTag(potionTableTag))
        {
            var potionTable = other.GetComponentInParent<PotionTable>();
            if (potionTable != null)
            {
                potionInRange = potionTable;
                Debug.Log("进入下毒桌交互区：" + potionTable.name);
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
        else if (other.CompareTag(potionTableTag))
        {
            var potionTable = other.GetComponentInParent<PotionTable>();
            if (potionTable != null && potionTable == potionInRange)
            {
                potionInRange = null;
                Debug.Log("离开下毒桌交互区");
            }
        }
    }
}
