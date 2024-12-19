using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeImageScript : MonoBehaviour
{
    [Header("最初からフェードインが完了しているかどうか")] public bool firstFadeInComp;

    private Image _fadeImage = null;    // シーン転換用画像
    private int _frameCount = 0;        // フレームカウント
    private float _timer = 0.0f;        // タイマー
    private bool _fadeIn = false;       // フェードインしたか
    private bool _fadeOut = false;      // フェードアウトしたか
    private bool _compFadeIn = false;   // フェードインが完了したか
    private bool _compFadeOut = false;  // フェードアウトが完了したか

    // Start is called before the first frame update
    void Start()
    {
        _fadeImage = GetComponent<Image>();

        if(firstFadeInComp)
        {
            FadeInComplete();
        }
        else
        {
            StartFadeIn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // シーン移行時の処理の重さでTime.deltaTimeが大きくなってしまうので2フレーム待つ
        if(_frameCount > 2)
        {
            if(_fadeIn)
            {
                FadeIn();
            }
            else if(_fadeOut)
            {
                FadeOut();
            }
        }
        ++_frameCount;
    }

    /* フェードインを開始する関数 */
    /* 引数：なし                 */
    /* 戻り値：なし               */
    public void StartFadeIn()
    {
        if(_fadeIn || _fadeOut)
        {
            return;
        }
        _fadeIn = true;
        _compFadeIn = false;
        _timer = 0.0f;
        _fadeImage.color = new Color(1, 1, 1, 1);
        _fadeImage.fillAmount = 1;
        _fadeImage.raycastTarget = true;
    }

    /* フェードインが完了したか調べる関数 */
    /* 引数：なし                         */
    /* 戻り値：フェードインが完了したか   */
    public bool IsFadeInComplete()
    {
        return _compFadeIn;
    }

    /* フェードアウトを開始する関数 */
    /* 引数：なし                   */
    /* 戻り値：なし                 */
    public void StartFadeOut()
    {
        if(_fadeIn || _fadeOut)
        {
            return;
        }
        _fadeOut = true;
        _compFadeOut = false;
        _timer = 0.0f;
        _fadeImage.color = new Color(1, 1, 1, 0);
        _fadeImage.fillAmount = 0;
        _fadeImage.raycastTarget = true;
    }

    /* フェードアウトが完了したか調べる関数 */
    /* 引数：なし                           */
    /* 戻り値：フェードアウトが完了したか   */
    public bool IsFadeOutComplete()
    {
        return _compFadeOut;
    }

    /* フェードイン中の処理を行う関数 */
    /* 引数：なし                     */
    /* 戻り値：なし                   */
    private void FadeIn()
    {
        if(_timer < 1.0f)
        {
            _fadeImage.color = new Color(1, 1, 1, 1 - _timer);
            _fadeImage.fillAmount = 1 - _timer;
        }
        else
        {
            FadeInComplete();
        }
        _timer += Time.deltaTime;
    }

    /* フェードイン完了後の処理を行う関数 */
    /* 引数：なし                         */
    /* 戻り値：なし                       */
    private void FadeInComplete()
    {
        _fadeImage.color = new Color(1, 1, 1, 0);
        _fadeImage.fillAmount = 0;
        _fadeImage.raycastTarget = false;
        _timer = 0.0f;
        _fadeIn = false;
        _compFadeIn = true;
    }

    /* フェードアウト中の処理を行う関数 */
    /* 引数：なし                       */
    /* 戻り値：なし                     */
    private void FadeOut()
    {
        if(_timer < 1.0f)
        {
            _fadeImage.color = new Color(1, 1, 1, _timer);
            _fadeImage.fillAmount = _timer;
        }
        else
        {
            FadeOutComplete();
        }
        _timer += Time.deltaTime;
    }

    /* フェードアウト完了後の処理を行う関数 */
    /* 引数：なし                           */
    /* 戻り値：なし                         */
    private void FadeOutComplete()
    {
        _fadeImage.color = new Color(1, 1, 1, 1);
        _fadeImage.fillAmount = 1;
        _fadeImage.raycastTarget = false;
        _timer = 0.0f;
        _fadeOut = false;
        _compFadeOut = true;
    }
}