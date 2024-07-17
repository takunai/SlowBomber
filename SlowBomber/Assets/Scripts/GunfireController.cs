using UnityEngine;

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
        public bool rotate = true;
        public float rotationSpeed = .25f;

        // --- オプション ---
        public GameObject scope;
        public bool scopeActive = true;
        private bool lastScopeState;

        // --- 弾 ---
        [Tooltip("武器が発射されるたびにインスタンス化する弾のゲームオブジェクト。")]
        public GameObject projectilePrefab;
        [Tooltip("時々、発射時にメッシュを無効にする必要がある場合があります。例えば、ロケットが発射されると、" +
            "新しいロケットがインスタンス化され、ロケットランチャーに取り付けられた見えるロケットは無効にされます。")]
        public GameObject projectileToDisableOnFire;

        // --- タイミング ---
        [SerializeField] private float timeLastFired;

        private void Start()
        {
            if (source != null) source.clip = GunShotClip;
            timeLastFired = 0;
            lastScopeState = scopeActive;
        }

        private void Update()
        {
            // --- 回転が有効な場合、シーン内で武器を回転させる ---
            if (rotate)
            {
                transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y
                                                                        + rotationSpeed, transform.localEulerAngles.z);
            }

            // --- スコープの状態を更新する ---
            if (scope && lastScopeState != scopeActive)
            {
                lastScopeState = scopeActive;
                scope.SetActive(scopeActive);
            }

            // --- スペースキーが押された場合、武器を発射する ---
            if (Input.GetKeyDown(KeyCode.Space) && (timeLastFired + shotDelay) <= Time.time)
            {
                FireWeapon();
            }
        }

        /// <summary>
        /// マズルフラッシュのインスタンスを作成します。
        /// 複数のショットが同じオーディオソースに重ならないように、オーディオソースのインスタンスも作成します。
        /// この関数内に弾のコードを挿入します。
        /// </summary>
        public void FireWeapon()
        {
            // --- 武器が発射された時間を記録する ---
            timeLastFired = Time.time;

            // --- マズルフラッシュを生成する ---
            var flash = Instantiate(muzzlePrefab, muzzlePosition.transform);

            // --- 弾オブジェクトを発射する ---
            if (projectilePrefab != null)
            {
                GameObject newProjectile = Instantiate(projectilePrefab, muzzlePosition.transform.position, muzzlePosition.transform.rotation);
            }

            // --- 必要に応じてゲームオブジェクトを無効にする ---
            if (projectileToDisableOnFire != null)
            {
                projectileToDisableOnFire.SetActive(false);
                Invoke("ReEnableDisabledProjectile", 3);
            }

            // --- オーディオを処理する ---
            if (source != null)
            {
                // --- オーディオソースが武器にアタッチされていない場合、各ショットが個別のオーディオソースを持つようにする ---
                if (source.transform.IsChildOf(transform))
                {
                    source.Play();
                }
                else
                {
                    // --- オーディオ用のプレハブをインスタンス化し、数秒後に削除する ---
                    AudioSource newAS = Instantiate(source);
                    if (newAS != null && newAS.outputAudioMixerGroup != null && newAS.outputAudioMixerGroup.audioMixer != null)
                    {
                        // --- ショットの繰り返しに変化を与えるため、ピッチを変更する ---
                        newAS.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", Random.Range(audioPitch.x, audioPitch.y));
                        newAS.pitch = Random.Range(audioPitch.x, audioPitch.y);

                        // --- 銃声を再生する ---
                        newAS.PlayOneShot(GunShotClip);

                        // --- 数秒後に削除する。テストスクリプトのみ。プロジェクトで使用する場合はオブジェクトプールの使用を推奨します ---
                        Destroy(newAS.gameObject, 4);
                    }
                }
            }

            // --- 武器から弾やヒットスキャンを発射するためのカスタムコードをここに挿入する ---

        }

        private void ReEnableDisabledProjectile()
        {
            reloadSource.Play();
            projectileToDisableOnFire.SetActive(true);
        }
    }
}