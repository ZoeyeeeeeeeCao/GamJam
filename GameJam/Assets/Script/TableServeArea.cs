using UnityEngine;

public class TableServeArea : MonoBehaviour
{
    [Header("桌子中央放菜的位置")]
    public Transform platePoint;

    private CustomerWaitArea waitArea;  // 同一张桌子的等待区域

    private void Awake()
    {
        // TableServeArea 和 CustomerWaitArea 在同一个桌子Root上时：
        waitArea = GetComponentInParent<CustomerWaitArea>();
    }

    public void PlaceFood(FoodItem food)
    {
        if (food == null) return;

        // 脱离玩家手
        food.transform.SetParent(null);

        // 传送到桌子中央
        if (platePoint != null)
        {
            food.transform.position = platePoint.position;
            food.transform.rotation = platePoint.rotation;
        }

        // 恢复刚体物理
        var rb = food.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Debug.Log("把食物放在桌子上：" + food.name);

        // 从等待区域拿到当前这桌顾客
        CustomerOrderUI customerOrderUI =
            (waitArea != null) ? waitArea.CurrentCustomerUI : null;

        if (customerOrderUI != null)
        {
            // 1. 判断这道菜对不对
            CustomerReactionType reaction =
                customerOrderUI.EvaluateFood(food.foodPrefabId);

            // 2. 让顾客自己执行对应反应
            customerOrderUI.PlayReaction(reaction);
        }
        else
        {
            Debug.LogWarning("这张桌子当前没有顾客，或 CustomerWaitArea 没检测到顾客。");
        }
    }
}
