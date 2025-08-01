using UnityEngine;

    public class EnemyController : MonoBehaviour
    {
        public int hp;
        /// <summary>
        /// Animator animator;
        /// </summary>
       void Start()
        {
            ///animator = GetComponent<Animator>
        }

        public void OnDamage()
        {
           ///hp -=1;
            ///animator.SetTrigger("IsHurt");
            ///if (hp <= 0)
            {
                ///Die();
            }
        }
        void Die()
        {
            ///hp = 0;
            ///animator.SetTrigger("Die");
        }
    }