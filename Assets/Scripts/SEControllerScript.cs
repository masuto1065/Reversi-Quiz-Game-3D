using UnityEngine;

public class SEControllerScript : MonoBehaviour
{
    [SerializeField] private AudioClip _clickSE;           // クリック時のSE
    [SerializeField] private AudioClip _questionSE;        // クイズ出題時のSE
    [SerializeField] private AudioClip _correctSE;         // クイズ正解時のSE
    [SerializeField] private AudioClip _incorrectSE;       // クイズ不正解時のSE
    [SerializeField] private AudioClip _winnerSE;          // 最終結果表示時の勝利SE
    [SerializeField] private AudioClip _drawSE;            // 最終結果表示時の引き分けSE
    [SerializeField] private AudioClip _textAppearanceSE;  // テキストが表示時のSE
    [SerializeField] private AudioClip _reversiStoneSE;    // リバーシの石が置かれる音
    private AudioSource _seAudioSource;

    // Start is called before the first frame update
    void Start()
    {
        // AudioSourceコンポーネントを取得
        _seAudioSource = GetComponent<AudioSource>();
    }

    /* クリック時のSEを鳴らす */
    public void ClickSE()
    {
        _seAudioSource.PlayOneShot(_clickSE);
    }

    /* クイズ出題時のSEを鳴らす */
    public void QuestionSE()
    {
        _seAudioSource.PlayOneShot(_questionSE);
    }

    /* クイズ正解時のSEを鳴らす */
    public void CorrectSE()
    {
        _seAudioSource.PlayOneShot(_correctSE);
    }

    /* クイズ不正解時のSEを鳴らす */
    public void IncorrectSE()
    {
        _seAudioSource.PlayOneShot(_incorrectSE);
    }

    /* 最終結果表示時の勝利SEを鳴らす */
    public void WinnerSE()
    {
        _seAudioSource.PlayOneShot(_winnerSE);
    }

    /* 最終結果表示時の引き分けSEを鳴らす */
    public void DrawSE()
    {
        _seAudioSource.PlayOneShot(_drawSE);
    }

    /* テキストが表示時のSEを鳴らす */
    public void TextAppearanceSE()
    {
        _seAudioSource.PlayOneShot(_textAppearanceSE);
    }

    /* リバーシの石が置かれる音 */
    public void ReversiStoneSE()
    {
        _seAudioSource.PlayOneShot(_reversiStoneSE);
    }
}