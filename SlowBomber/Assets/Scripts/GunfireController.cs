using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace BigRookGames.Weapons
{
    public class GunfireController : MonoBehaviour
    {
        // --- オーディオ ---
        public AudioClip GunShotClip;
        public AudioClip ReloadClip;
        public AudioSource source;
        public AudioSource reloadSource;
        public Vector2 audioPitch = new Vector2(.9f, 1.1f);

        // --- マズルフラッシュ ---
        public GameObject muzzlePrefab;
        public GameObject muzzlePosition;

        // --- 設定 ---
        public float shotDelay = .5f;
        public BoolReactiveProperty rotate = new BoolReactiveProperty(true);
        public float rotationSpeed = .25f;

        // --- スコープ ---
        public GameObject scope;
        public BoolReactiveProperty scopeActive = new BoolReactiveProperty(true);

        // --- 弾 ---
        [Tooltip("武器が発射されるたびにインスタンス化する弾のゲームオブジェクト。")]
        public GameObject projectilePrefab;
        [Tooltip("発射時に非表示にするオブジェクト（例：見た目のロケット）")]
        public GameObject projectileToDisableOnFire;

        // --- タイミング ---
        private float timeLastFired;

        // --- 弾速 ---
        public float initialProjectileSpeed = 10f;
        public float projectileSpeedIncrement = 2f;
        private float currentProjectileSpeed;

        private void Start()
        {
            // 初期設定
            if (source != null) source.clip = GunShotClip;
            currentProjectileSpeed = initialProjectileSpeed;
            timeLastFired = 0f;

            // --- 入力処理（右クリックで発射） ---
            this.UpdateAsObservable()
                .Where(_ => Input.GetMouseButtonDown(1))
                .Where(_ => Time.time >= timeLastFired + shotDelay)
                .Subscribe(_ => FireWeapon())
                .AddTo(this);

            // --- 武器の回転処理 ---
            this.UpdateAsObservable()
                .Where(_ => rotate.Value)
                .Subscribe(_ =>
                {
                    transform.localEulerAngles = new Vector3(
                        transform.localEulerAngles.x,
                        transform.localEulerAngles.y + rotationSpeed,
                        transform.localEulerAngles.z
                    );
                })
                .AddTo(this);

            // --- スコープの状態を監視して反映 ---
            scopeActive
                .DistinctUntilChanged()
                .Subscribe(active =>
                {
                    if (scope != null) scope.SetActive(active);
                })
                .AddTo(this);
        }

        /// <summary>
        /// 武器を発射する処理
        /// </summary>
        private void FireWeapon()
        {
            timeLastFired = Time.time;

            // マズルフラッシュを生成
            if (muzzlePrefab && muzzlePosition)
            {
                Instantiate(muzzlePrefab, muzzlePosition.transform);
            }

            // 弾の発射
            if (projectilePrefab != null)
            {
                GameObject newProjectile = Instantiate(projectilePrefab, muzzlePosition.transform.position, muzzlePosition.transform.rotation);
                Rigidbody rb = newProjectile.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.velocity = muzzlePosition.transform.forward * currentProjectileSpeed;
                }

                // 弾の威力設定
                var projectileScript = newProjectile.GetComponent<Projectile>();
                if (projectileScript != null)
                {
                    projectileScript.damage = 10f;
                }
            }

            // ロケットの見た目を非表示に
            if (projectileToDisableOnFire != null)
            {
                projectileToDisableOnFire.SetActive(false);
                Observable.Timer(System.TimeSpan.FromSeconds(3))
                          .Subscribe(_ => ReEnableDisabledProjectile())
                          .AddTo(this);
            }

            // オーディオ処理
            if (source != null)
            {
                if (source.transform.IsChildOf(transform))
                {
                    source.Play();
                }
                else
                {
                    AudioSource newAS = Instantiate(source);
                    if (newAS != null && newAS.outputAudioMixerGroup?.audioMixer != null)
                    {
                        float pitch = Random.Range(audioPitch.x, audioPitch.y);
                        newAS.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", pitch);
                        newAS.pitch = pitch;
                        newAS.PlayOneShot(GunShotClip);
                        Destroy(newAS.gameObject, 4f);
                    }
                }
            }
        }

        /// <summary>
        /// 発射後に非表示にしたオブジェクトを再表示
        /// </summary>
        private void ReEnableDisabledProjectile()
        {
            if (reloadSource != null) reloadSource.Play();
            if (projectileToDisableOnFire != null) projectileToDisableOnFire.SetActive(true);
        }

        /// <summary>
        /// 敵を倒したときに呼び出して弾速を強化
        /// </summary>
        public void OnEnemyKilled()
        {
            currentProjectileSpeed += projectileSpeedIncrement;
        }
    }
}
