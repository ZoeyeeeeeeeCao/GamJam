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
    }

    private void Update()
    {
        // 获取输入（WSAD / 方向键）
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D, 左/右
        float vertical = Input.GetAxisRaw("Vertical");     // W/S, 上/下

        // 把输入先存起来，在 FixedUpdate 里用
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

            // 只要水平分量（y=0），避免上下倾斜影响
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // 把输入转换到世界空间：纵向用前后，横向用左右
            Vector3 moveDir = camForward * rawInput.z + camRight * rawInput.x;

            // 防止斜着跑更快，归一化
            if (moveDir.magnitude > 1f)
                moveDir.Normalize();

            inputDirection = moveDir;
        }
        else
        {
            // 没设置相机就直接按世界坐标走（Z 前 X 右）
            inputDirection = rawInput.normalized;
        }

        // 让角色面对移动方向（可选）
        if (inputDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(inputDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRot,
                10f * Time.deltaTime
            );
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
}
