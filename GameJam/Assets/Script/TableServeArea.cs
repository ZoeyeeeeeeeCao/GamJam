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

        var rb = food.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        Debug.Log("把食物放在桌子上：" + food.name);

        // 从等待区域拿到当前这桌顾客
        CustomerOrderUI customerOrderUI = (waitArea != null) ? waitArea.CurrentCustomerUI : null;

        if (customerOrderUI != null)
        {
            var reaction = customerOrderUI.EvaluateFood(food.foodPrefabId);

            switch (reaction)
            {
                case CustomerReactionType.Reaction1:
                    Debug.Log("Reaction1：上对菜，顾客开心 😊");
                    break;

                case CustomerReactionType.Reaction2:
                    Debug.Log("Reaction2：下毒菜，顾客倒地 😵");
                    break;

                case CustomerReactionType.Reaction3:
                    Debug.Log("Reaction3：上错菜，顾客生气 😡");
                    break;

                default:
                    Debug.Log("顾客还没点单 / 无效上菜");
                    break;
            }
        }
        else
        {
            Debug.LogWarning("这张桌子当前没有顾客，或 CustomerWaitArea 没检测到顾客。");
        }
    }
}
