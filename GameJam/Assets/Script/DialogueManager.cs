using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class DialogueLine
{
    public bool isPlayer;          // true = 玩家说话，false = NPC 说话
    [TextArea(2, 5)]
    public string text;
}

public class DialogueManager : MonoBehaviour
{
    [Header("UI 引用")]
    public GameObject dialoguePanel;
    public Image portraitImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI contentText;

    [Header("两边的角色信息")]
    public string playerName = "You";
    public Sprite playerPortrait;

    // 当前对话数据
    private DialogueLine[] lines;
    private int index;

    private string npcName;
    private Sprite npcPortrait;

    private bool isActive = false;

    void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    /// <summary>
    /// 由 NPC 调用，开始一整段对话
    /// </summary>
    public void StartDialogue(string _npcName, Sprite _npcPortrait, DialogueLine[] _lines)
    {
        if (_lines == null || _lines.Length == 0) return;

        npcName = _npcName;
        npcPortrait = _npcPortrait;
        lines = _lines;
        index = 0;
        isActive = true;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(true);

        ShowCurrentLine();
    }

    /// <summary>
    /// 按按钮或按键：下一句
    /// </summary>
    public void OnClickNext()
    {
        if (!isActive || lines == null) return;

        index++;

        if (index >= lines.Length)
        {
            EndDialogue();
        }
        else
        {
            ShowCurrentLine();
        }
    }

    private void Update()
    {
        // 想要的话也可以用 Space 继续
        if (isActive && Input.GetKeyDown(KeyCode.Space))
        {
            OnClickNext();
        }
    }

    private void ShowCurrentLine()
    {
        if (lines == null || index < 0 || index >= lines.Length) return;

        DialogueLine line = lines[index];

        if (line.isPlayer)
        {
            if (nameText != null) nameText.text = playerName;
            if (portraitImage != null && playerPortrait != null)
                portraitImage.sprite = playerPortrait;
        }
        else
        {
            if (nameText != null) nameText.text = npcName;
            if (portraitImage != null && npcPortrait != null)
                portraitImage.sprite = npcPortrait;
        }

        if (contentText != null)
            contentText.text = line.text;
    }

    private void EndDialogue()
    {
        isActive = false;
        lines = null;

        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }
}
