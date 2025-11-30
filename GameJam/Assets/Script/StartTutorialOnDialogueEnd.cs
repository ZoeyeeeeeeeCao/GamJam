using UnityEngine;

public class StartTutorialOnDialogueEnd : MonoBehaviour
{
    [Header("拖 TutorialLevelManager 进来（或自动寻找）")]
    public TutorialLevelManager tutorial;

    private void Awake()
    {
        // 如果你忘了在 Inspector 里拖，也尝试自动找一下
        if (tutorial == null)
        {
            tutorial = FindAnyObjectByType<TutorialLevelManager>();
            if (tutorial == null)
            {
                Debug.LogError("[StartTutorialOnDialogueEnd] 场景里找不到 TutorialLevelManager！");
            }
            else
            {
                Debug.Log("[StartTutorialOnDialogueEnd] 自动找到 TutorialLevelManager。");

            }
        }
    }

    // 给 DialogueManager 或对话结束事件调用
    public void BeginTutorial()
    {
        Debug.Log("[StartTutorialOnDialogueEnd] BeginTutorial 被调用。");

        if (tutorial == null)
        {
            Debug.LogError("[StartTutorialOnDialogueEnd] tutorial 为 null，不能启动教程。");
            return;
        }

        BGMManager.Instance.FadeToBGM2(1.2f);
        tutorial.StartTutorial();
    }
}

