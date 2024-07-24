using UnityEngine;

namespace BigRookGames.Weapons
{
    public class Projectile : MonoBehaviour
    {
        public float damage = 10f; // 弾の威力を10に設定

        private void OnCollisionEnter(Collision collision)
        {
            EnemyController enemy = collision.gameObject.GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // 弾を破壊する
            Destroy(gameObject);
        }
    }
}