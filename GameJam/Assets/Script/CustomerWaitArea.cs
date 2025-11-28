using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CustomerWaitArea : MonoBehaviour
{
    [Header("引用")]
    public Transform customer;         // costumer 物体（头顶UI跟着它）
    public Image loadingCircle;        // 圆形加载条（Radial）
    public Image finalIcon;            // 10秒后出现的图片

    [Header("设置")]
    public float waitTime = 10f;       // 等待时间（秒）
    public string playerTag = "Player";

    private bool playerInside = false; // 玩家是否在区域内
    private bool isCounting = false;   // 是否正在计时
    private bool completed = false;    // 是否已经完成过一次
    private Coroutine waitCoroutine;

    private void Start()
    {
        // 初始状态：隐藏进度条和最终图片
        if (loadingCircle != null)
        {
            loadingCircle.fillAmount = 0f;
            loadingCircle.gameObject.SetActive(false);
        }
        if (finalIcon != null)
        {
            finalIcon.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag) && !completed)
        {
            playerInside = true;

            if (!isCounting)
            {
                waitCoroutine = StartCoroutine(WaitAndShowIcon());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            playerInside = false;

            // 玩家离开就中断计时并重置UI
            if (waitCoroutine != null && !completed)
            {
                StopCoroutine(waitCoroutine);
                isCounting = false;
            }

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

        if (finalIcon != null)
        {
            finalIcon.gameObject.SetActive(false);
        }

        float timer = 0f;

        while (timer < waitTime)
        {
            // 如果玩家中途离开，退出
            if (!playerInside)
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

        if (finalIcon != null)
        {
            finalIcon.gameObject.SetActive(true);
        }

        completed = true;   // 如果只想触发一次
        isCounting = false;
    }

    private void ResetUI()
    {
        if (loadingCircle != null)
        {
            loadingCircle.fillAmount = 0f;
            loadingCircle.gameObject.SetActive(false);
        }

        if (!completed && finalIcon != null)
        {
            finalIcon.gameObject.SetActive(false);
        }
    }
}

