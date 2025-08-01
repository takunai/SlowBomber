using System.Collections;
using UnityEngine;

public class EnemyMoveBase : MonoBehaviour
{
    [Header("=== 移動・行動設定 ===")]
    [SerializeField] protected float moveSpeed = 1.0f;
    [SerializeField] protected float actionInterval = 2.0f;
    [SerializeField] protected GameObject attackPoint;
    [SerializeField] protected GameObject attackPrefab;
    [SerializeField] protected float actionStartTime;
    [SerializeField] protected float attackLifetime = 0.5f;

    protected Transform player;
    protected bool isInEngageRange = false;
    protected float actionTimer = 0.0f;
    protected Rigidbody rb;

    private bool shouldChase = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Rigidbody が見つかりません。Enemy に Rigidbody をアタッチしてください！");
        }
    }

    private void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Playerオブジェクトが見つかりません");
        }
    }

    private void Update()
    {
        if (player == null) return;

        float deltaX = Mathf.Abs(player.position.x - transform.position.x);
        float deltaZ = Mathf.Abs(player.position.z - transform.position.z);

        bool isWithinXZRange = (deltaX <= 5.0f) || (deltaZ <= 5.0f);

        if (isWithinXZRange)
        {
            shouldChase = true;

            if (!isInEngageRange)
            {
                Debug.Log("行動開始");
                isInEngageRange = true;
                actionTimer = 0.0f;
            }

            actionTimer += Time.deltaTime;
            if (actionTimer >= actionInterval)
            {
                StartCoroutine(PerformAction());
                actionTimer = 0.0f;
            }
        }
        else
        {
            if (isInEngageRange)
            {
                Debug.Log("行動停止");
                isInEngageRange = false;
                actionTimer = 0.0f;
            }

            shouldChase = false;
        }
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        if (shouldChase)
        {
            rb.isKinematic = false;

            Vector3 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;

            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * moveSpeed);
        }
        else
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true; // 物理演算を止めるので絶対止まる
        }
    }

    protected virtual IEnumerator PerformAction()
    {
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);
        yield return new WaitForSeconds(actionStartTime);
        Debug.Log("行動を実行しました");
    }

    public Vector3 GetFacingDirection()
    {
        return transform.forward;
    }
}