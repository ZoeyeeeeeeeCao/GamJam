using UnityEngine;
using TMPro;   // 记得引用 TextMeshPro
using System.Collections;

public class TutorialLevelManager : MonoBehaviour
{
    public enum Step
    {
        None,
        Task1_Move,
        Task2_MoveFoodToKitchen,
        Task3_ApproachCustomer,
        Task4_OrderDesk,
        Task5_PickupFood,
        Task6_DropFood,
        Task7_PickupAgain,
        Task8_DoRecipe,
        Task9_PickupAfterRecipe,
        Task10_PlaceAtTable,
        Done
    }

    [Header("玩家相关")]
    public Transform player;              // 拖 Player
    private PlayerCarryFood playerCarry;  // 运行时自动找
    private Vector3 moveStartPos;

    [Header("教程 UI")]
    public GameObject taskPanel;          // TutorialPanel
    public TextMeshProUGUI taskText;      // Panel 里的 TextMeshPro

    [Header("Task2：搬走 FoodToKitchen")]
    public GameObject foodToKitchen;      // 场景中那块要搬走的食物或箱子

    [Header("生成点（空物体）")]
    public Transform spawnPoint1;         // 用于 Task3 的顾客 & Task4/10 的桌子/点单台
    public Transform spawnPoint2;         // 用于 Task8 的毒药台

    [Header("教程用 Prefab")]
    public GameObject customerWithTablePrefab;  // 顾客+桌子
    public GameObject orderDeskPrefab;          // 点单台
    public GameObject potionTablePrefab;        // 毒药台（内含 PotionUI）
    public GameObject serveTablePrefab;         // 最后上菜用的桌子

  

    // 运行时实例引用
    private GameObject currentCustomerTable;
    private CustomerOrderUI currentCustomerOrderUI;

    private GameObject currentOrderDesk;
    private GameObject currentPotionTable;
    private GameObject currentServeTable;

    // 毒药 UI
    private PotionUI potionUI;
    private bool potionPanelWasOpen = false;

    // 当前步骤
    public Step currentStep = Step.None;
    private bool started = false;

    private float task10Timer = 0f;

    private void Start()
    {
        // 找玩家携带脚本
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) player = p.transform;
        }

        playerCarry = FindFirstObjectByType<PlayerCarryFood>();

        // 一开始教程 panel 不显示
        if (taskPanel != null)
            taskPanel.SetActive(false);

        if (taskText != null)
            taskText.text = "";
    }

    /// <summary>
    /// 这个函数让“对话结束”的脚本来调用。
    /// 比如你的 DialogueManager 在对话结束时：tutorialManager.StartTutorial();
    /// </summary>
    public void StartTutorial()
    {
        if (started) return;
        started = true;

        if (taskPanel != null)
            taskPanel.SetActive(true);

        GoToStep(Step.Task1_Move);
    }

    private void Update()
    {
        if (!started) return;

        switch (currentStep)
        {
            case Step.Task1_Move:
                UpdateTask1();
                break;

            case Step.Task2_MoveFoodToKitchen:
                UpdateTask2();
                break;

            case Step.Task3_ApproachCustomer:
                UpdateTask3();
                break;

            case Step.Task4_OrderDesk:
                UpdateTask4();
                break;

            case Step.Task5_PickupFood:
                UpdateTask5();
                break;

            case Step.Task6_DropFood:
                UpdateTask6();
                break;

            case Step.Task7_PickupAgain:
                UpdateTask7();
                break;

            case Step.Task8_DoRecipe:
                UpdateTask8();
                break;

            case Step.Task9_PickupAfterRecipe:
                UpdateTask9();
                break;

            case Step.Task10_PlaceAtTable:
                UpdateTask10();
                break;

            case Step.Done:
                // 可以在这里考虑自动 Load 下一关
                break;
        }
    }

    //==============================
    // 步骤切换 & 进入时的初始化
    //==============================
    private void GoToStep(Step newStep)
    {
        currentStep = newStep;
        UpdateTaskText();
        OnEnterStep(newStep);
        Debug.Log("[Tutorial] Enter Step: " + newStep);
    }

    private void OnEnterStep(Step step)
    {
        switch (step)
        {
            case Step.Task1_Move:
                if (player != null)
                    moveStartPos = player.position;
                break;

            case Step.Task3_ApproachCustomer:
                SpawnCustomerAndTable();
                break;

            case Step.Task4_OrderDesk:
                SpawnOrderDesk();
                break;

            case Step.Task8_DoRecipe:
                SpawnPotionTable();
                break;

            case Step.Task10_PlaceAtTable:
                SpawnServeTable();
                task10Timer = 0f;         // ⭐ 开始计时
                break;



            case Step.Done:
                EndTutorial();
                break;
        }
    }

    private void UpdateTaskText()
    {
        if (taskText == null) return;

        switch (currentStep)
        {
            case Step.Task1_Move:
                taskText.text = "Task 1:\nUse WASD to move and the mouse to look around.";
                break;

            case Step.Task2_MoveFoodToKitchen:
                taskText.text = "Task 2:\nPress E to move the food pile into the kitchen.";
                break;

            case Step.Task3_ApproachCustomer:
                taskText.text = "Task 3:\nApproach the customer and wait for them place an order.";
                break;

            case Step.Task4_OrderDesk:
                taskText.text = "Task 4:\nGo to the order counter and choose the food";
                break;

            case Step.Task5_PickupFood:
                taskText.text = "Task 5:\nWalk up to the food and press E to pick it up";
                break;

            case Step.Task6_DropFood:
                taskText.text = "Task 6:\nPress E again to drop the food on the floor";
                break;

            case Step.Task7_PickupAgain:
                taskText.text = "Task 7:\nPress E to pick the food up one more time.";
                break;

            case Step.Task8_DoRecipe:
                taskText.text = "Task 8:\nPress E to use the potion table and click on the potions in the correct order.\nThe correct order will be shown at the start of each round";
                break;

            case Step.Task9_PickupAfterRecipe:
                taskText.text = "Task 9:\nPress E to pick up the cooked / poisoned food from the potion table.";
                break;

            case Step.Task10_PlaceAtTable:
                taskText.text = "Task 10:\nWalk to the customer table and press E to place the food.";
                break;

            case Step.Done:
                taskText.text = "Tutorial complete!";
                break;

            default:
                taskText.text = "";
                break;
        }
    }

    //==============================
    // Task1: 移动一次
    //==============================
    private void UpdateTask1()
    {
        if (player == null) return;

        float dist = Vector3.Distance(player.position, moveStartPos);
        if (dist > 0.1f)
        {
            GoToStep(Step.Task2_MoveFoodToKitchen);
        }
    }

    //==============================
    // Task2: FoodToKitchen 被 Destroy/隐藏
    //==============================
    private void UpdateTask2()
    {
        if (foodToKitchen == null || !foodToKitchen.activeSelf)
        {
            GoToStep(Step.Task3_ApproachCustomer);
        }
    }

    //==============================
    // Task3: 顾客点单完成（用 CustomerOrderUI.completed）
    //==============================
    private void SpawnCustomerAndTable()
    {
        if (customerWithTablePrefab == null || spawnPoint1 == null)
        {
            Debug.LogWarning("Tutorial: customerWithTablePrefab 或 spawnPoint1 没设置。");
            return;
        }

        if (currentCustomerTable != null)
            Destroy(currentCustomerTable);

        currentCustomerTable = Instantiate(
            customerWithTablePrefab,
            spawnPoint1.position,
            spawnPoint1.rotation
        );

        currentCustomerOrderUI = currentCustomerTable.GetComponentInChildren<CustomerOrderUI>();
        if (currentCustomerOrderUI == null)
        {
            Debug.LogWarning("Tutorial: 在顾客 prefab 里找不到 CustomerOrderUI，请检查结构。");
        }
    }

    private void UpdateTask3()
    {
        if (currentCustomerOrderUI == null) return;

        // 顾客点单完成（finalImage 出现）
        if (currentCustomerOrderUI.completed)
        {
            // 防止重复进入
            if (currentCustomerTable != null)
            {
                StartCoroutine(DelayedDestroyCustomerAndNextStep());
            }

            // 防止多次触发
            currentCustomerOrderUI = null;
        }
    }


    //==============================
    // Task4: 点单台（这里简单用按一次 E 作为完成条件）
    //==============================
    private void SpawnOrderDesk()
    {
        if (orderDeskPrefab == null || spawnPoint1 == null)
        {
            Debug.LogWarning("Tutorial: orderDeskPrefab 或 spawnPoint1 没设置。");
            return;
        }

        if (currentOrderDesk != null)
            Destroy(currentOrderDesk);

        currentOrderDesk = Instantiate(
            orderDeskPrefab,
            spawnPoint1.position,
            spawnPoint1.rotation
        );
    }

    private void UpdateTask4()
    {
        // 简化做法：本步骤中，只要玩家按一次 E，就认为已经学会“靠近点单台按 E”
        if (Input.GetKeyDown(KeyCode.E))
        {
            GoToStep(Step.Task5_PickupFood);
        }
    }

    //==============================
    // Task5: 拾起食物（用已有的 PlayerCarryFood.IsCarryingFood）
    //==============================
    private void UpdateTask5()
    {
        if (playerCarry == null) return;

        if (playerCarry.IsCarryingFood)
        {
            // 玩家已经把食物拿在手上了 → 可以删掉教程点单台
            if (currentOrderDesk != null)
            {
                Destroy(currentOrderDesk);
                currentOrderDesk = null;
            }

            GoToStep(Step.Task6_DropFood);
        }
    }

    //==============================
    // Task6: 扔掉 / 放下食物
    //==============================
    private void UpdateTask6()
    {
        if (playerCarry == null) return;

        if (!playerCarry.IsCarryingFood)
        {
            GoToStep(Step.Task7_PickupAgain);
        }
    }

    //==============================
    // Task7: 再次拾取
    //==============================
    private void UpdateTask7()
    {
        if (playerCarry == null) return;

        if (playerCarry.IsCarryingFood)
        {
            GoToStep(Step.Task8_DoRecipe);
        }
    }

    //==============================
    // Task8: 生成毒药台 + 观察 PotionUI 开关
    //==============================
    private void SpawnPotionTable()
    {
        if (potionTablePrefab == null || spawnPoint2 == null)
        {
            Debug.LogWarning("Tutorial: potionTablePrefab 或 spawnPoint2 没设置。");
            return;
        }

        if (currentPotionTable != null)
            Destroy(currentPotionTable);

        currentPotionTable = Instantiate(
            potionTablePrefab,
            spawnPoint2.position,
            spawnPoint2.rotation
        );

        // ⭐ 在这个实例上找 PotionUI（哪怕默认是隐藏的，也用 GetComponentInChildren(true)）
        potionUI = currentPotionTable.GetComponentInChildren<PotionUI>(true);

        if (potionUI == null)
        {
            Debug.LogWarning("Tutorial: 在毒药台 prefab 里找不到 PotionUI。");
        }
        else
        {
            potionPanelWasOpen = false;
        }
    }

    private void UpdateTask8()
    {
        if (potionUI == null) return;

        // 面板激活时认为“开始操作”
        if (potionUI.gameObject.activeSelf)
        {
            potionPanelWasOpen = true;
        }
        // 如果曾经开过，现在关了 → 认为玩家完成了这一步
        else if (potionPanelWasOpen && !potionUI.gameObject.activeSelf)
        {
            GoToStep(Step.Task9_PickupAfterRecipe);
        }
    }

    //==============================
    // Task9: 玩家拿起处理好的食物，同时销毁毒药台
    //==============================
    private void UpdateTask9()
    {
        if (playerCarry == null) return;

        if (playerCarry.IsCarryingFood)
        {
            if (currentPotionTable != null)
            {
                Destroy(currentPotionTable);
                currentPotionTable = null;
                potionUI = null;
            }

            GoToStep(Step.Task10_PlaceAtTable);
        }
    }

    //==============================
    // Task10: 生成上菜桌子，等它被销毁
    //==============================
    private void SpawnServeTable()
    {
        if (serveTablePrefab == null || spawnPoint1 == null)
        {
            Debug.LogWarning("Tutorial: serveTablePrefab 或 spawnPoint1 没设置。");
            return;
        }

        if (currentServeTable != null)
            Destroy(currentServeTable);

        currentServeTable = Instantiate(
            serveTablePrefab,
            spawnPoint1.position,
            spawnPoint1.rotation
        );
    }

    private void UpdateTask10()
    {
        task10Timer += Time.deltaTime;

        if (task10Timer >= 4f)
        {
            Debug.Log("[Tutorial] Task10 自动完成（4秒计时）。");
            GoToStep(Step.Done);
        }
    }

    //==============================
    // 教程结束
    //==============================
    private void EndTutorial()
    {
        Debug.Log("[Tutorial] All tasks finished.");

        if (taskPanel != null)
            taskPanel.SetActive(false);

        // 这里以后可以加 LoadScene / 解锁正式关卡 等
    }
    private IEnumerator DelayedDestroyCustomerAndNextStep()
    {
        // ✨ 等待 3 秒
        yield return new WaitForSeconds(3f);

        // ✨ 销毁顾客 + 桌子
        if (currentCustomerTable != null)
        {
            Destroy(currentCustomerTable);
            currentCustomerTable = null;
        }

        // ✨ 进入 Task4
        GoToStep(Step.Task4_OrderDesk);
    }

}
