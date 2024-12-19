using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public class HowToPlayScript : MonoBehaviour
{
    // 遊び方関連
    public GameObject HowToPlayPanel;    // 遊び方の背景
    public List<GameObject> slides;      // 遊び方のスライド画像
    private int _currentSlideIndex = 0;  // 現在のスライド番号

    // Sounds関連
    private SEControllerScript _seControllerScript;

    // Start is called before the first frame update
    void Start()
    {
        // SEControllerScriptコンポーネントを取得
        _seControllerScript = FindObjectOfType<SEControllerScript>();
    }

    // Update is called once per frame
    void Update()
    {
        /* 「←」を押した時、スライド１枚戻る関数 */
        if(Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow))
        {
            ShowPreviousSlide();
        }

        /* 「→」を押した時、スライド１枚進む関数 */
        if(Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow))
        {
            ShowNextSlide();
        }

        // 「Space」を押した時、タイトル画面に戻る
        if(Input.GetKeyDown(KeyCode.Space))
        {
            BackToTitle();
        }
    }

    /* スライドを更新する関数 */
    private void UpdateSlide()
    {
        for(int i = 0; i < slides.Count; i++)
        {
            slides[i].SetActive(i == _currentSlideIndex);
        }
    }

    /* 「←」を押した時、スライド１枚戻る関数 */
    private void ShowPreviousSlide()
    {
        if(_currentSlideIndex > 0 && HowToPlayPanel.activeSelf)
        {
            _currentSlideIndex--;
            UpdateSlide();
            _seControllerScript.ClickSE();
        }
    }

    /* 「→」を押した時、スライド１枚進む関数 */
    private void ShowNextSlide()
    {
        if(_currentSlideIndex < slides.Count - 1 && HowToPlayPanel.activeSelf)
        {
            _currentSlideIndex++;
            UpdateSlide();
            _seControllerScript.ClickSE();
        }
    }

    // 「Space」を押した時、タイトル画面に戻る
    public void BackToTitle()
    {
        _currentSlideIndex = 0;
        HowToPlayPanel.SetActive(false);
        for(int i = 0; i < slides.Count; i++)
        {
            slides[i].SetActive(i == _currentSlideIndex);
        }
        _seControllerScript.ClickSE();
    }
}
