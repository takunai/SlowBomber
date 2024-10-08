using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SoundPlay2 : MonoBehaviour
{
    void Start()
    {
        // 現在のシーン名を取得
        Scene currentScene = SceneManager.GetActiveScene();

        // StartSceneの場合にのみBGMを再生
        if (currentScene.name == "StartScene")
        {
            SoundManager.Instance.PlayBGM(BGMSoundData.BGM.Title);
        }

        // シーンがロードされたときのイベントを登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        // シーンロード時のイベントを解除
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // GameSceneに移動した場合、BGMを停止
        if (scene.name == "GameScene")
        {
            SoundManager.Instance.StopBGM();
        }
    }
}