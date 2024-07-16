using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    // 移動したいシーンの名前
    public string SceneName;

    // ボタンが押されたときに呼び出されるメソッド
    public void SwitchScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}