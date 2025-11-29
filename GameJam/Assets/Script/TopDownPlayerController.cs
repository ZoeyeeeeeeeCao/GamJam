using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TopDownPlayerController : MonoBehaviour
{
    [Header("移动参数")]
    public float moveSpeed = 5f;          // 移动速度

    [Header("相机")]
    public Transform cameraTransform;     // 用来确定屏幕方向（一般拖 Main Camera）

    private Rigidbody rb;
    private Vector3 inputDirection;

    // NEW: Animator 引用
    private Animator animator;

    private PlayerCarryFood carryFood; // NEW

    [Header("音效")]
    public AudioSource audioSource;   // 用来播放脚步声的 AudioSource（Loop ON）
    public AudioClip footstepClip;    // 脚步声循环音效

    private bool wasMoving = false;   // 上一帧是否在移动


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // 不希望刚体自己乱旋转
        rb.constraints = RigidbodyConstraints.FreezeRotationX
                       | RigidbodyConstraints.FreezeRotationZ;

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        // 在子物体中找到 Animator（模型一般在子物体上）
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
        {
            Debug.LogWarning("TopDownPlayerController: No Animator found in children.");
        }

        carryFood = GetComponent<PlayerCarryFood>();

        // ⭐ 初始化脚步声 AudioSource
        if (audioSource != null && footstepClip != null)
        {
            audioSource.clip = footstepClip;
            audioSource.loop = true;         // 循环播放
            audioSource.playOnAwake = false; // 不要一开始就自己播
        }
    }

    private void Update()
    {
        HandleMovementInput();
        HandleMouseRotation();
        UpdateAnimation(); // NEW: 根据输入更新动画
        HandleFootstepSound(); // NEW: 根据是否在移动控制脚步声 Play/Stop
    }

    private void HandleMovementInput()
    {
        // 获取输入（WSAD / 方向键）
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D, 左/右
        float vertical = Input.GetAxisRaw("Vertical");   // W/S, 前/后

        Vector3 rawInput = new Vector3(horizontal, 0f, vertical);

        // 没有输入就直接清零
        if (rawInput.sqrMagnitude < 0.001f)
        {
            inputDirection = Vector3.zero;
            return;
        }

        // 根据相机方向来算移动方向（保证和屏幕方向一致）
        if (cameraTransform != null)
        {
            Vector3 camForward = cameraTransform.forward;
            Vector3 camRight = cameraTransform.right;

            // 只要水平分量，y = 0，避免上下倾斜影响
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // 把输入转换到世界空间：前后用 camForward，左右用 camRight
            Vector3 moveDir = camForward * rawInput.z + camRight * rawInput.x;

            // 防止斜着走更快，归一化
            if (moveDir.magnitude > 1f)
                moveDir.Normalize();

            inputDirection = moveDir;
        }
        else
        {
            // 没有设置相机就按世界坐标走（Z 前 X 右）
            inputDirection = rawInput.normalized;
        }
    }

    /// <summary>
    /// 用鼠标控制玩家朝向，绕 Y 轴
    /// </summary>
    private void HandleMouseRotation()
    {
        Camera cam = Camera.main;
        if (cam == null && cameraTransform != null)
            cam = cameraTransform.GetComponent<Camera>();
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // 在玩家当前高度的平面上求交点
        Plane plane = new Plane(Vector3.up, new Vector3(0f, transform.position.y, 0f));

        if (plane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            Vector3 lookDir = hitPoint - transform.position;
            lookDir.y = 0f; // 只在水平面旋转

            if (lookDir.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(lookDir, Vector3.up);
                // 用刚体旋转会更稳定
                rb.MoveRotation(Quaternion.Slerp(rb.rotation, targetRot, 10f * Time.deltaTime));
            }
        }
    }

    private void FixedUpdate()
    {
        // 物理更新中移动
        if (inputDirection.sqrMagnitude > 0.001f)
        {
            Vector3 targetPosition = rb.position + inputDirection * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(targetPosition);
        }
    }

    // NEW: 根据是否有移动输入，切换 Idle/Walk 动画
    private void UpdateAnimation()
    {
        if (animator == null) return;

        bool isMoving = inputDirection.sqrMagnitude > 0.001f;
        animator.SetBool("IsMoving", isMoving);

        bool isCarrying = carryFood != null && carryFood.IsCarryingFood;
        animator.SetBool("IsCarrying", isCarrying);
    }

    // NEW: 专门控制脚步声 Play / Stop
    private void HandleFootstepSound()
    {
        if (audioSource == null || footstepClip == null)
            return;

        bool isMoving = inputDirection.sqrMagnitude > 0.001f;

        if (isMoving && !wasMoving)
        {
            // 刚开始移动 → 播放脚步循环
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else if (!isMoving && wasMoving)
        {
            // 刚刚停止移动 → 立刻掐断脚步声
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        wasMoving = isMoving;
    }
}
