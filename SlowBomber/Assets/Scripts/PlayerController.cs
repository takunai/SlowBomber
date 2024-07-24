using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; // シーン管理のための名前空間

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10.0f; // 移動速度
    public float lookSpeed = 2.0f; // 視点移動速度
    public string targetSceneName = "GoalScene"; // 移動先のシーン名

    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 100;
        Cursor.lockState = CursorLockMode.Locked; // マウスカーソルをロック
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーの移動
        float moveX = 0;
        float moveZ = 0;
        if (Input.GetKey(KeyCode.A))
        {
            moveX -= moveSpeed * Time.deltaTime; // 左に移動
        }
        if (Input.GetKey(KeyCode.D))
        {
            moveX += moveSpeed * Time.deltaTime; // 右に移動
        }
        if (Input.GetKey(KeyCode.W))
        {
            moveZ += moveSpeed * Time.deltaTime; // 前に移動
        }
        if (Input.GetKey(KeyCode.S))
        {
            moveZ -= moveSpeed * Time.deltaTime; // 後ろに移動
        }
        transform.Translate(moveX, 0, moveZ);

        // 視点の回転
        float lookX = Input.GetAxis("Mouse X") * lookSpeed;
        float looky = Input.GetAxis("Mouse Y") * lookSpeed;


        transform.Rotate(0, lookX, 0);
        transform.Rotate(0, looky, 0);
        

        // カメラの上下回転を別のGameObject（例：カメラ自体）に適用する場合
    }

    // 特定のオブジェクトに触れたときの処理
    void OnTriggerEnter(Collider other)
    {
        // オブジェクトのタグが "TargetObject" であるか確認
        if (other.CompareTag("TargetObject"))
        {
            // シーンを移動
            SceneManager.LoadScene(targetSceneName);
        }
    }
}