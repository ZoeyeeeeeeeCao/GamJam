using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PotionUI : MonoBehaviour
{
    public static PotionUI Instance;

    [Header("三个按钮")]
    public Button btnA;
    public Button btnB;
    public Button btnC;

    private PotionTable currentTable;
    private List<string> input = new List<string>();

    private void Awake()
    {
        Instance = this;

        // 绑定按钮点击事件
        if (btnA != null) btnA.onClick.AddListener(() => Click("A"));
        if (btnB != null) btnB.onClick.AddListener(() => Click("B"));
        if (btnC != null) btnC.onClick.AddListener(() => Click("C"));

        gameObject.SetActive(false); // 初始隐藏
    }

    /// <summary>
    /// 被 PotionTable 调用，打开面板
    /// </summary>
    public void OpenPanel(PotionTable table)
    {
        currentTable = table;
        input.Clear();
        gameObject.SetActive(true);
    }

    private void Click(string s)
    {
        if (currentTable == null || currentTable.currentFood == null)
        {
            Debug.LogWarning("没有当前食物或桌子，点击无效");
            return;
        }

        input.Add(s);
        var correctOrder = currentTable.currentFood.correctOrder;

        if (correctOrder == null || correctOrder.Length == 0)
        {
            Debug.LogWarning("当前食物没有设置 correctOrder");
            return;
        }

        // 输入长度达到配方长度时，进行判断
        if (input.Count == correctOrder.Length)
        {
            bool success = true;
            for (int i = 0; i < input.Count; i++)
            {
                if (input[i] != correctOrder[i])
                {
                    success = false;
                    break;
                }
            }

            // 通知桌子应用结果（生成正确版或下毒版）
            currentTable.ApplyResult(success);

            // 关闭面板
            gameObject.SetActive(false);
        }
    }
}
