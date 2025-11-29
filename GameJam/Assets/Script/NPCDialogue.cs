using UnityEngine;

public class NPCDialogue : MonoBehaviour
{
    [Header("NPC 基本信息")]
    public string npcName = "老板";
    public Sprite npcPortrait;

    [Header("这一段完整对话（按顺序，勾 isPlayer 决定谁说）")]
    public DialogueLine[] lines;

    [Header("是否只触发一次（教程建议开）")]
    public bool triggerOnlyOnce = true;

    private DialogueManager dialogueManager;
    private bool triggered = false;

    void Start()
    {
        // 新版 Unity 推荐写法
        dialogueManager = FindFirstObjectByType<DialogueManager>();

        if (dialogueManager == null)
        {
            Debug.LogWarning("NPCDialogue: 场景里找不到 DialogueManager！");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (triggerOnlyOnce && triggered) return;
        if (!other.CompareTag("Player")) return;

        triggered = true;

        if (dialogueManager != null)
        {
            dialogueManager.StartDialogue(npcName, npcPortrait, lines);
        }
    }
}
