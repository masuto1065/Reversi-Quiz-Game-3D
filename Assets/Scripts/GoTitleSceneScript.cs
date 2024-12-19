using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoTitleSceneScript : MonoBehaviour
{
    [Header("フェード")] public FadeImageScript fade;

    private bool _firstPush = false;
    private bool _goNextScene = false;

    /* ゲーム終了後にタイトル画面移動ボタンを押したときにフェードアウトを開始する関数 */
    /* 引数：なし                                                                     */
    /* 戻り値：なし                                                                   */
    public void PressGoTitleScene(){
        if(!_firstPush){
            fade.StartFadeOut();
            _firstPush = true;
        }
    }

    private void Update()
    {
        // フェードアウト後、TitleSceneに移動
        if(!_goNextScene && fade.IsFadeOutComplete())
        {
            SceneManager.LoadScene("TitleScene");
            _goNextScene = true;
        }
    }
}
