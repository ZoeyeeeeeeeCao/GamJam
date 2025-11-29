using System.Collections;
using UnityEngine;

public class TableServeArea : MonoBehaviour
{
    [Header("桌子中央放菜的位置")]
    public Transform platePoint;

    private CustomerWaitArea waitArea;

    private void Awake()
    {
        waitArea = GetComponentInParent<CustomerWaitArea>();
    }

    public void PlaceFood(FoodItem food)
    {
        if (food == null) return;

        // 把食物放到桌子的摆盘点
        food.transform.SetParent(null);
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
        Debug.Log("上菜时 FoodItem.foodPrefabId = " + food.foodPrefabId?.name);

        // 🔴 核心：从等待区域拿到当前顾客
        var customerOrderUI = (waitArea != null) ? waitArea.CurrentCustomerUI : null;
        if (customerOrderUI != null)
        {
            // ⭐⭐ 这里一定要传 prefabId，而不是 food.gameObject ⭐⭐
            var reaction = customerOrderUI.EvaluateFood(food.foodPrefabId);

            switch (reaction)
            {
                case CustomerReactionType.Reaction1:
                    Debug.Log("Reaction1：上对菜，顾客开心 😊"); //happy
                    break;
                case CustomerReactionType.Reaction2:
                    Debug.Log("Reaction2：下毒菜，顾客倒地 😵"); //died
                    break;
                case CustomerReactionType.Reaction3:
                    Debug.Log("Reaction3：上错菜，顾客生气 😡"); //angry
                    break;
                default:
                    Debug.Log("顾客还没点单 / 无效上菜");
                    break;
            }

            customerOrderUI.PlayReaction(reaction);
        }
        else
        {
            Debug.LogWarning("这张桌子当前没有顾客，或 CustomerWaitArea 没检测到顾客。");
        }

        // After evaluating reaction, add this:
        StartCoroutine(DestroyFoodAfterSeconds(food, 3f)); // food disappears after 3 seconds

    }

    private IEnumerator DestroyFoodAfterSeconds(FoodItem food, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (food != null)
            Destroy(food.gameObject);
    }


}
