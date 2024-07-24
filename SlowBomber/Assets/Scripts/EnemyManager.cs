using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float health = 1f; // 敵の体力を1に設定

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 0.1秒後に敵を破壊する
        Destroy(gameObject, 0.1f);
    }
}