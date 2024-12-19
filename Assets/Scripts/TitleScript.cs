using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScript : MonoBehaviour
{
    [Header("フェード")] public FadeImageScript fade;

    // スタートボタン関連
    private bool _firstPush = false;
    private bool _goNextScene = false;

    // 遊び方関連
    public GameObject HowToPlayPanel;  // 遊び方の背景

    // 設定ボタン関連
    public GameObject SettingPanel;  // 設定画面の背景
    private BGMControllerScript _bgmControllerScript;

    void Start()
    {
        _bgmControllerScript = FindObjectOfType<BGMControllerScript>();
        _bgmControllerScript.PlayBGM();
    }

    void Update()
    {
        // フェードアウト後、ReversiSceneに移動
        if(!_goNextScene && fade.IsFadeOutComplete())
        {
            SceneManager.LoadScene("ReversiScene");
            _goNextScene = true;
        }
    }

    /* スタートボタンを押したときにフェードアウトを開始する関数 */
    public void PressStart(){
        if(!_firstPush){
            fade.StartFadeOut();
            _firstPush = true;
        }
    }

    /* 設定画面を表示する関数 */
    public void ShowSettingPanel()
    {
        SettingPanel.SetActive(true);
    }

    /* 遊び方を表示する関数 */
    public void ShowHowToPlayPanel()
    {
        HowToPlayPanel.SetActive(true);
    }
}