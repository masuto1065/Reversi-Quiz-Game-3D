using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class QuizScript : MonoBehaviour
{
    // 問題関連
    public GameObject QuizPanel;  // クイズパネル
    public TextMeshProUGUI questionText;  // 問題文のテキスト
    private static List<QuizData> _quizDataList = new List<QuizData>();  // クイズデータのリスト
    private int _correctAnswerIndex;  // 正解のインデックス
    private ReversiScript _reversiScript;
    private Dictionary<Vector2Int, int> _quizMapping = new Dictionary<Vector2Int, int>();  // 盤面の座標とクイズのインデックスを関連付ける辞書

    // ○×関連
    public List<GameObject> answerMarks;  // 選択肢のマルバツオブジェクトを格納するリスト
    public List<Image> answerImages;      // 選択肢のマルバツ画像を格納するリスト
    public Sprite correctImage;           // 正解の画像
    public Sprite incorrectImage;         // 不正解の画像

    // 選択肢のボタン関連
    public List<Button> optionButtons;         // 選択肢のボタンを格納するリスト
    public List<TextMeshProUGUI> optionTexts;  // 選択肢のテキストを格納するリスト
    private float selectedR = 253 / 255f;      // ボタン押後の色
    private float selectedG = 211 / 255f;
    private float selectedB = 92 / 255f;
    private float normalR = 215 / 255f;        // ボタンの通常色
    private float normalG = 215 / 255f;
    private float normalB = 215 / 255f;

    // カウントダウンタイマー関連
    public Image TimerBackgroundImage;    // カウントダウンタイマーの画像
    public TextMeshProUGUI TimerText;     // カウントダウンタイマーのテキスト
    public GameObject TimeIsUpImage;      // 時間切れのテキスト
    private float _totalTime = 15.0f;     // カウントダウンの秒数
    public Coroutine countdownCoroutine;  // カウントダウンコルーチンの参照を保持

    // SE関連
    private SEControllerScript _seControllerScript;


    // Start is called before the first frame update
    void Start()
    {
        // クイズデータの読込
        _quizDataList.Clear();
        ReadCSV();

        // ReversiScriptコンポーネントを取得
        _reversiScript = GetComponent<ReversiScript>();

        // SEControllerScriptコンポーネントを取得
        _seControllerScript = GetComponent<SEControllerScript>();
        
        // クイズを盤面の座標に割り当て
        AssignQuizzesToBoard();
    }

    /* Quizが記述されたCSVファイルを読み込む関数 */
    /* 引数：なし                                */
    /* 戻り値：なし                              */
    private void ReadCSV()
    {
        // Quiz.csvの読込
        TextAsset csvData = Resources.Load<TextAsset>("Quiz");
        string[] lines = csvData.text.Split('\n');
        lines = lines.Where(line => !string.IsNullOrEmpty(line)).ToArray();

        // ランダムに並び替え
        var random = new System.Random();
        lines = lines.Skip(1).OrderBy(x => random.Next()).ToArray();

        // _quizDataListに追加
        for (int i = 0; i < lines.Length; i++)
        {
            string[] values = lines[i].Split(',');
            QuizData quizData = new QuizData();
            quizData.id = int.Parse(values[0]);
            quizData.question = values[1];
            quizData.option1 = values[2];
            quizData.option2 = values[3];
            quizData.option3 = values[4];
            quizData.option4 = values[5];
            _quizDataList.Add(quizData);
        }
    }

    /* クイズを盤面に割り当てる関数 */
    /* 引数：なし                   */
    /* 戻り値：なし                 */
    private void AssignQuizzesToBoard()
    {
        int index = 0;

        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                // 中心の4つのマスは除外
                if ((x == 3 && y == 3) || (x == 4 && y == 4) || (x == 3 && y == 4) || (x == 4 && y == 3))
                    continue;

                if (index < _quizDataList.Count)
                {
                    _quizMapping[new Vector2Int(x, y)] = index;
                    index++;
                }
            }
        }
    }

    /* クイズを表示する関数                    */
    /* 引数：                                  */
    /* boardPosition：プレイヤーが選択したマス */
    /* 戻り値：なし                            */
    public void CreateQuizPanel(Vector2Int boardPosition)
    {
        if (_quizMapping.ContainsKey(boardPosition))
        {
            // 座標に対応するクイズインデックスを取得
            int index = _quizMapping[boardPosition];

            // 現在のクイズデータを取得
            QuizData currentQuiz = _quizDataList[index];
            var options = new List<string> { currentQuiz.option1, currentQuiz.option2, currentQuiz.option3, currentQuiz.option4 };
            string correctAnswer = currentQuiz.option1;
            var random = new System.Random();
            options = options.OrderBy(x => random.Next()).ToList();
            _correctAnswerIndex = options.IndexOf(correctAnswer);
            
            // 問題文と選択肢をUIに設定
            questionText.text = currentQuiz.question;
            for (int i = 0; i < options.Count; i++)
            {
                optionTexts[i].text = options[i];
            }

            // 各選択肢のボタンにOnClickイベントを設定
            for (int i = 0; i < optionButtons.Count; i++)
            {
                optionButtons[i].onClick.RemoveAllListeners();
                int buttonIndex = i;
                optionButtons[i].onClick.AddListener(_seControllerScript.ClickSE);
                optionButtons[i].onClick.AddListener(() => StartCoroutine(CheckAnswer(buttonIndex)));
            }

            // 選択肢のDisabledColorを元に戻す & 押せるようにする
            foreach (var button in optionButtons)
            {
                ColorBlock colors = button.colors;
                colors.disabledColor = new Color(normalR, normalG, normalB);
                button.colors = colors;
                button.interactable = true;
            }

            // ○×を非表示
            ShowAnswerMark(false, _correctAnswerIndex, -1);

            // 「時間切れ」を非表示
            TimeIsUpImage.SetActive(false);

            // クイズ出題時のSEを鳴らす
            _seControllerScript.QuestionSE();
            
            // クイズパネルを表示
            QuizPanel.SetActive(true);
            
            // カウントダウン開始
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
            }
            countdownCoroutine = StartCoroutine(StartCountdown());
        }
    }

    /* ○×表示関数                     */
    /* 引数：                           */
    /* show：表示状態                   */
    /* correctAnswerIndex：正解の選択肢 */
    /* selectedIndex：選択した選択肢    */
    /* 戻り値：なし                     */
    private void ShowAnswerMark(bool show, int correctAnswerIndex, int selectedIndex)
    {
        // すべての選択肢に不正解の画像を設定
        foreach (var image in answerImages)
        {
            image.sprite = incorrectImage;
        }

        // 正解の選択肢に正解の画像を設定
        if (correctAnswerIndex >= 0 && correctAnswerIndex < answerImages.Count)
        {
            answerImages[correctAnswerIndex].sprite = correctImage;
        }

        if(selectedIndex == correctAnswerIndex || selectedIndex == -1)
        {
            // 表示・非表示を設定
            foreach (var mark in answerMarks)
            {
                mark.SetActive(show);
            }
        }
        else
        {
            answerMarks[selectedIndex].SetActive(show);
        }
    }

    /* 正誤判定を行う関数                        */
    /* 引数：                                    */
    /* selectedIndex：プレイヤーが回答した選択肢 */
    /* 戻り値：なし                              */
    public IEnumerator CheckAnswer(int selectedIndex)
    {
        // 選択したボタンの色を変更 & 全てのボタンを押せなくする
        ColorBlock colors = optionButtons[selectedIndex].colors;
        colors.disabledColor = new Color(selectedR, selectedG, selectedB);
        optionButtons[selectedIndex].colors = colors;
        foreach (var button in optionButtons)
        {
            button.interactable = false;
        }

        // クイズ終了時にタイマー停止
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }

        if(selectedIndex == _correctAnswerIndex)  // 正解
        {
            // ○×を表示
            yield return new WaitForSeconds(1.0f);
            ShowAnswerMark(true, _correctAnswerIndex, selectedIndex);

            // 正解時のSEを鳴らす
            _seControllerScript.CorrectSE();
            yield return new WaitForSeconds(3.0f);

            // 正解時のリバーシの処理
            StartCoroutine(_reversiScript.CorrectToQuiz());
        }
        else  // 不正解
        {
            // ○×を表示
            yield return new WaitForSeconds(1.0f);
            ShowAnswerMark(true, _correctAnswerIndex, selectedIndex);

            // 不正解時のSEを鳴らす
            _seControllerScript.IncorrectSE();
            yield return new WaitForSeconds(3.0f);

            // 不正解時のリバーシの処理
            _reversiScript.IncorrectToQuiz();
        }

        // クイズパネルを非表示
        QuizPanel.SetActive(false);
        // クイズを終わる
        _reversiScript.GetSetIsQuizActive = false;
    }

    /* カウントダウンを行う関数 */
    /* 引数：なし               */
    /* 戻り値：なし             */
    private IEnumerator StartCountdown()
    {
        float timeRemaining = _totalTime;

        while (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            TimerBackgroundImage.fillAmount = timeRemaining / _totalTime;
            TimerText.text = Mathf.CeilToInt(timeRemaining).ToString();
            yield return null;
        }
        TimerText.text = "0";

        // 時間切れならば
        // ボタンを押せなくする
        foreach (var button in optionButtons)
        {
            button.interactable = false;
        }
        TimeIsUpImage.SetActive(true);
        // 不正解時のSEを鳴らす
        _seControllerScript.IncorrectSE();
        yield return new WaitForSeconds(3.0f);
        // 不正解処理
        _reversiScript.IncorrectToQuiz();
        // クイズパネルを非表示
        QuizPanel.SetActive(false);
        // クイズ終了時にタイマー停止
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
        }
        // クイズを終わる
        _reversiScript.GetSetIsQuizActive = false;
    }
}

// クイズのデータ構造を定義
[System.Serializable]
public class QuizData
{
    public int id;           // ID
    public string question;  // 問題文
    public string option1;   // 選択肢１（正解）
    public string option2;   // 選択肢２
    public string option3;   // 選択肢３
    public string option4;   // 選択肢４
}