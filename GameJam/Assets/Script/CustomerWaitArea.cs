using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CustomerWaitArea : MonoBehaviour
{
    [Header("引用")]
    public Transform customer;
    public Image loadingCircle;
    public Image[] finalIcons;

    [Header("设置")]
    public float waitTime = 10f;
    public string playerTag = "Player";
    public string customerTag = "Customer";

    private bool playerInside = false;
    private bool customerInside = false; // ⭐ costumer 是否在区域内
    private bool isCounting = false;
    private bool completed = false;
    private Coroutine waitCoroutine;

    private void Start()
    {
        if (loadingCircle != null)
        {
            loadingCircle.fillAmount = 0f;
            loadingCircle.gameObject.SetActive(false);
        }

        HideAllFinalIcons();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 玩家进入
        if (other.CompareTag(playerTag) && !completed)
        {
            playerInside = true;

            // 只有当 customer 也在场时才开始计时
            if (customerInside && !isCounting)
            {
                waitCoroutine = StartCoroutine(WaitAndShowIcon());
            }
        }
        // Customer 进入
        else if (other.CompareTag(customerTag) && !completed)
        {
            customerInside = true;

            // 如果玩家已经在圈里，也可以马上开始计时
            if (playerInside && !isCounting)
            {
                waitCoroutine = StartCoroutine(WaitAndShowIcon());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 玩家离开
        if (other.CompareTag(playerTag))
        {
            playerInside = false;
            StopCountingIfNeeded();
        }
        // Customer 离开
        else if (other.CompareTag(customerTag))
        {
            customerInside = false;
            StopCountingIfNeeded();
        }
    }

    private void StopCountingIfNeeded()
    {
        // 只要还没完成，就打断计时并重置 UI
        if (waitCoroutine != null && !completed)
        {
            StopCoroutine(waitCoroutine);
            waitCoroutine = null;
            isCounting = false;
            ResetUI();
        }
    }

    private IEnumerator WaitAndShowIcon()
    {
        isCounting = true;

        if (loadingCircle != null)
        {
            loadingCircle.gameObject.SetActive(true);
            loadingCircle.fillAmount = 0f;
        }

        HideAllFinalIcons();

        float timer = 0f;

        while (timer < waitTime)
        {
            // 中途只要 player 或 customer 有一方不在，直接退出
            if (!playerInside || !customerInside)
            {
                isCounting = false;
                yield break;
            }

            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / waitTime);

            if (loadingCircle != null)
            {
                loadingCircle.fillAmount = progress;
            }

            yield return null;
        }

        // 计时结束
        if (loadingCircle != null)
        {
            loadingCircle.gameObject.SetActive(false);
        }

        ShowRandomFinalIcon();

        completed = true;   // 只想触发一次
        isCounting = false;
    }

    private void ResetUI()
    {
        if (loadingCircle != null)
        {
            loadingCircle.fillAmount = 0f;
            loadingCircle.gameObject.SetActive(false);
        }

        if (!completed)
        {
            HideAllFinalIcons();
        }
    }

    private void HideAllFinalIcons()
    {
        if (finalIcons == null) return;

        foreach (var icon in finalIcons)
        {
            if (icon != null)
            {
                icon.gameObject.SetActive(false);
            }
        }
    }

    private void ShowRandomFinalIcon()
    {
        if (finalIcons == null || finalIcons.Length == 0) return;

        HideAllFinalIcons();

        int index = Random.Range(0, finalIcons.Length);
        var chosen = finalIcons[index];

        if (chosen != null)
        {
            chosen.gameObject.SetActive(true);
        }
    }
}
