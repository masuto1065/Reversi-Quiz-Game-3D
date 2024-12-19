using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReversiScript : MonoBehaviour
{
    // リバーシゲーム関連
    public GameObject ReversiStone;        // リバーシの石
    public GameObject Cube;                // 選択マス視覚化用の黄色四角
    public GameObject Sphere;              // 選択可能マス表示用の球体
    const int BOARD_SIZE_X = 8;  // リバーシ盤の横サイズ
    const int BOARD_SIZE_Y = 8;  // リバーシ盤の縦サイズ
    private int _cubePositionX = 5;  // Cubeの初期x座標
    private int _cubePositionY = 4;  // Cubeの初期y座標
    private bool _blackCheckFlag = true;  // 黒プレイヤーが石を置けるマスがあるならTrue
    private bool _whiteCheckFlag = true;  // 白プレイヤーが石を置けるマスがあるならTrue
    private List<(int, int)> _infoList = new List<(int, int)>();  // 裏返せる石の座標を格納するリスト

    public enum stoneState{None, White, Black}  // 石の状態（非表示, 白, 黒）
    public enum canPutState{Cannot, Can}        // 石を置けるかを表す（置けない、置ける）
    private enum assistState{Off, On}           // アシストの表示状態（表示しない、表示する）

    private stoneState _playerTurn = stoneState.Black;                                          // プレイヤーのターン状態
    private stoneState[,] _fieldState = new stoneState[BOARD_SIZE_X, BOARD_SIZE_Y];             // 盤の状態管理用
    private StoneScript[,] _fieldStoneState = new StoneScript[BOARD_SIZE_X, BOARD_SIZE_Y];      // 石の状態管理用
    private canPutState[,] _canPutState = new canPutState[BOARD_SIZE_X, BOARD_SIZE_Y];          // 置けるマスの状態管理用
    private SphereScript[,] _canPutSphereState = new SphereScript[BOARD_SIZE_X, BOARD_SIZE_Y];  // 置けるマス表示用のSphereの状態管理用
    private assistState _assistFlag = assistState.Off;                                          // アシストの表示状態

    // UI画像・テキスト関連
    // プレイヤー画像
    public GameObject BlackBearImage;    // 黒熊プレイヤーの画像
    public GameObject BlackCircleImage;  // 黒熊プレイヤーの背景画像
    public GameObject WhiteBearImage;    // 白熊プレイヤーの画像
    public GameObject WhiteCircleImage;  // 白熊プレイヤーの背景画像
    private Image _blackCircleImageCom;  // BlackCircleImageのImageコンポーネント
    private Image _whiteCircleImageCom;  // WhiteCircleImageのImageコンポーネント
    private Color _originalColor;        // CircleImageの元色を保持するための変数

    // 操作方法関連
    public GameObject SpaceKeyImage;      // 「Spaceキー」の画像
    public GameObject ArrowKeyImage;      // 「矢印キー」の画像
    public GameObject ReturnKeyImage;     // 「Enterキー」の画像
    public TextMeshProUGUI AssistText;    // アシスト状態の表示用のテキスト
    public TextMeshProUGUI ChoiseText;    // 「選択」のテキスト
    public TextMeshProUGUI DicisionText;  // 「決定」のテキスト

    // 先手・後手関連
    public GameObject FirstPlayerImage;   // 「先手」の画像
    public GameObject SecondPlayerImage;  // 「後手」の画像
    private bool _showTurnFlag = false;   // 先手・後手は表示済みか

    // 最終結果表示画面関連
    public GameObject ResultPanel;                // 最終結果表示画面の背景
    public GameObject GoTitleSceneButton;         // タイトル画面移動用のボタン
    public TextMeshProUGUI BlackResultText;       // 黒の勝敗表示用のテキスト
    public TextMeshProUGUI BlackStoneNumText;     // 黒石の数を表示する用のテキスト
    public TextMeshProUGUI BlackAccuracyText;     // 黒のクイズ正解率
    public TextMeshProUGUI BlackTotalPointsText;  // 黒の合計点
    public TextMeshProUGUI WhiteResultText;       // 白の勝敗表示用のテキスト
    public TextMeshProUGUI WhiteStoneNumText;     // 白石の数を表示する用のテキスト
    public TextMeshProUGUI WhiteAccuracyText;     // 白のクイズ正解率
    public TextMeshProUGUI WhiteTotalPointsText;  // 白の合計点
    public TextMeshProUGUI DrawResultText;        // 引き分け表示用のテキスト
    public TextMeshProUGUI StoneNumText;          // 「石の数」のテキスト
    public TextMeshProUGUI PlusText;              // 「+ +」のテキスト
    public TextMeshProUGUI AccuracyText;          // 「正解率」のテキスト
    public TextMeshProUGUI BlackEqualText;        // 黒の「=」のテキスト
    public TextMeshProUGUI WhiteEqualText;        // 白の「=」のテキスト
    public TextMeshProUGUI TotalPointsText;       // 「合計点」のテキスト
    private int _blackCorrectAnswers = 0;         // 黒が正解した問題数
    private int _blackTotalQuestions = 0;         // 黒が解答した問題数
    private int _whiteCorrectAnswers = 0;         // 白が正解した問題数
    private int _whiteTotalQuestions = 0;         // 白が解答した問題数

    
    // クイズゲーム関連
    private QuizScript _quizScript;
    private stoneState[,] _savedFieldState = new stoneState[BOARD_SIZE_X, BOARD_SIZE_Y];  // クイズ解答前の盤面保存用
    private List<(int x, int y)> _flipPositions = new List<(int x, int y)>();  // クイズ正解時にFlipアニメーションを適用する石の座標
    private (int x, int y) _dropPosition;  // クイズ正解時にDropアニメーションを適用する石の座標
    private bool _isQuizActive = false;    // クイズゲーム状態管理用
    public bool GetSetIsQuizActive         // クイズゲーム状態管理用プロパティ
    {
        get { return _isQuizActive; }
        set { _isQuizActive = value; }
    }

    // Sounds関連
    private SEControllerScript _seControllerScript;
    private AudioSource _seAudioSource;  // SEのAudioSource


    // Start is called before the first frame update
    void Start()
    {
        // 8×8の大きさで、リバーシの石を生成 & 置けるマス表示用のSphereを生成
        for(int x = 0; x < BOARD_SIZE_X; x++)
        {
            for(int y = 0; y < BOARD_SIZE_Y; y++)
            {
                // 石
                var stone = Instantiate(ReversiStone, new Vector3(1.03f*x, -0.045f, 1.03f*y), Quaternion.Euler(90, 0, 0));
                _fieldState[x, y] = stoneState.None;
                _fieldStoneState[x, y] = stone.GetComponent<StoneScript>();

                // Sphere
                var sphere = Instantiate(Sphere, new Vector3(1.03f*x, -0.1f, 1.03f*y), Quaternion.Euler(90, 0, 0));
                _canPutState[x, y] = canPutState.Cannot;
                _canPutSphereState[x, y] = sphere.GetComponent<SphereScript>();
                _canPutSphereState[x, y].ShowAssist(_canPutState[x, y]);
            }
        }

        // 中央4マスの石の状態を初期設定
        _fieldState[3, 3] = stoneState.Black;
        _fieldState[3, 4] = stoneState.White;
        _fieldState[4, 3] = stoneState.White;
        _fieldState[4, 4] = stoneState.Black;

        // 8×8のマスの石の状態を更新
        for(int x = 0; x < BOARD_SIZE_X; x++)
        {
            for(int y = 0; y < BOARD_SIZE_Y; y++)
            {
                _fieldStoneState[x, y].SetState(_fieldState[x, y]);
            }
        }

        // 背景画像コンポーネントを取得 & 元色を保存
        _blackCircleImageCom = BlackCircleImage.GetComponent<Image>();
        _whiteCircleImageCom = WhiteCircleImage.GetComponent<Image>();
        _originalColor = _blackCircleImageCom.color;

        // QuizScriptコンポーネントを取得
        _quizScript = GetComponent<QuizScript>();

        // SEControllerScriptコンポーネントを取得
        _seControllerScript = GetComponent<SEControllerScript>();

        // SEの音量設定
        _seAudioSource = GetComponent<AudioSource>();
        _seAudioSource.volume = PlayerPrefs.GetFloat("SEVolume", 1f);

        StartCoroutine(ShowTurnText());
    }

    // Update is called once per frame
    void Update()
    {
        // クイズゲーム中 & 「先手・後手」表示中ならば、これ以降は実行しない
        if (_isQuizActive || !_showTurnFlag) return;

        // キーボードの矢印キーによるマス選択処理（黄色のCubeを動かす）
        if(Cube.activeSelf)
        {
            HandleCubeMovement();
        }
            
        // Enterが押されたとき、そのマスに石が置けるならばクイズを開始
        if (Input.GetKeyDown(KeyCode.Return))
        {
            HandleQuizState();
        }
        
        // 全マスに対して、プレイヤーが石を置くことができるか調べる
        PutStoneCheck();

        // プレイヤーが石を置けるマスの表示（アシスト機能）の切り替え
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ChangeAssist();
        }
        ShowAssistSphere();
            
        // プレイヤーターンに応じてCircleImageの色変更
        ChangePlayerCircleColor();
    }

    /* 「先手・後手」を表示する関数 */
    /* 引数：なし                   */
    /* 戻り値：なし                 */
    private IEnumerator ShowTurnText()
    {
        Cube.SetActive(false);
        yield return new WaitForSeconds(2.0f);
        FirstPlayerImage.SetActive(true);
        _seControllerScript.TextAppearanceSE();
        yield return new WaitForSeconds(1.0f);
        SecondPlayerImage.SetActive(true);
        _seControllerScript.TextAppearanceSE();
        yield return new WaitForSeconds(1.5f);
        FirstPlayerImage.SetActive(false);
        SecondPlayerImage.SetActive(false);
        _showTurnFlag = true;
        Cube.SetActive(true);
    }

    /* キーボードの矢印キーによるマス選択処理（黄色のCubeを動かす）関数 */
    /* 引数：なし                                                       */
    /* 戻り値：なし                                                     */
    private void HandleCubeMovement()
    {
        // Cubeの現在位置を取得
        var position = Cube.transform.localPosition;
        // 右に動かす
        if(Input.GetKeyDown(KeyCode.RightArrow) && _cubePositionX < 7 && !Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow))
        {
            _cubePositionX++;
            Cube.transform.localPosition = new Vector3(position.x + 1.03f, position.y, position.z);
        }
        // 左に動かす
        if(Input.GetKeyDown(KeyCode.LeftArrow) && _cubePositionX > 0 && !Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.UpArrow) && !Input.GetKeyDown(KeyCode.DownArrow))
        {
            _cubePositionX--;
            Cube.transform.localPosition = new Vector3(position.x - 1.03f, position.y, position.z);
        }
        // 上に動かす
        if(Input.GetKeyDown(KeyCode.UpArrow) && _cubePositionY < 7 && !Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.DownArrow))
        {
            _cubePositionY++;
            Cube.transform.localPosition = new Vector3(position.x, position.y, position.z + 1.03f);
        }
        // 下に動かす
        if(Input.GetKeyDown(KeyCode.DownArrow) && _cubePositionY > 0 && !Input.GetKeyDown(KeyCode.RightArrow) && !Input.GetKeyDown(KeyCode.LeftArrow) && !Input.GetKeyDown(KeyCode.UpArrow))
        {
            _cubePositionY--;
            Cube.transform.localPosition = new Vector3(position.x, position.y, position.z - 1.03f);
        }
    }

    /* Enterが押されたとき、そのマスに石が置けるならばクイズを開始する関数 */
    /* 引数：なし                                                          */
    /* 戻り値：なし                                                        */
    private void HandleQuizState()
    {
        // 選択したマスから8方向に対して、裏返せる石があるか判定
        var turnCheck = false;
        for(int i = 0; i <= 7; i++)
        {
            if(TurnCheck(i))
            {
                turnCheck = true;
            }
        }

        // 選択したマスに石を置くことができるならば
        if(turnCheck && _fieldState[_cubePositionX, _cubePositionY] == stoneState.None)
        {
            // Cubeを非表示
            Cube.SetActive(false);

            // 石を置く前に盤面を保存
            SaveBoardState();

            // クリックされたマスのクイズを表示
            _isQuizActive = true;
            Vector2Int boardPosition = new Vector2Int(_cubePositionX, _cubePositionY);
            _quizScript.CreateQuizPanel(boardPosition);

            // 裏返せる石の座標情報を取り出し、自分の色の石に変える
            foreach(var info in _infoList)
            {
                var position_X = info.Item1;
                var position_Y = info.Item2;
                _fieldState[position_X, position_Y] = _playerTurn;
                _flipPositions.Add((position_X, position_Y));
            }
                
            // 選択したマスに自分の色の状態に一旦変更 & その座標を保存
            _fieldState[_cubePositionX, _cubePositionY] = _playerTurn;
            _dropPosition = (_cubePositionX, _cubePositionY);
        }
    }

    /* 石が置けるか判定する関数        */
    /* 引数：                          */
    /* direction：置く位置に対する方向 */
    /* 戻り値：                        */
    /* turnCheck：置けるならばtrue     */
    /*         　 置けないならばfalse  */
    private bool TurnCheck(int direction)
    {
        var turnCheck = false;                                                                           // false：そのマスに石を置くことができない
        var position_X = _cubePositionX;                                                                 // 選択したマスのx座標
        var position_Y = _cubePositionY;                                                                 // 選択したマスのy座標
        var opponentPlayerTurn = _playerTurn == stoneState.Black ? stoneState.White : stoneState.Black;  // 相手の石の色
        var infoList = new List<(int, int)>();                                                           // 挟める石の座標情報

        // 選択したマスから8方向を見る
        while(0 <= position_X && 7 >= position_X && 0 <= position_X && 7 >= position_Y)
        {
            switch(direction)
            {
                case 0:  // 左
                    if(position_X == 0){return turnCheck;}
                    position_X--;
                    break;
                case 1:  // 右
                    if(position_X == 7){return turnCheck;}
                    position_X++;
                    break;
                case 2:  // 下
                    if(position_Y == 0){return turnCheck;}
                    position_Y--;
                    break;
                case 3:  // 上
                    if(position_Y == 7){return turnCheck;}
                    position_Y++;
                    break;
                case 4:  // 左下
                    if(position_X == 0){return turnCheck;}
                    if(position_Y == 0){return turnCheck;}
                    position_X--;
                    position_Y--;
                    break;
                case 5:  // 右上
                    if(position_X == 7){return turnCheck;}
                    if(position_Y == 7){return turnCheck;}
                    position_X++;
                    position_Y++;
                    break;
                case 6:  // 左上
                    if(position_X == 0){return turnCheck;}
                    if(position_Y == 7){return turnCheck;}
                    position_X--;
                    position_Y++;
                    break;
                case 7:  // 右下
                    if(position_X == 7){return turnCheck;}
                    if(position_Y == 0){return turnCheck;}
                    position_X++;
                    position_Y--;
                    break;
            }

            // ある方向の一つ目のマスから順に見た時、その座標の石が相手の石ならば、その石の座標情報を残しておく
            if(_fieldState[position_X, position_Y] == opponentPlayerTurn)
            {
                infoList.Add((position_X, position_Y));
            }

            // ある方向を見た時、選択したマスの隣が、自分の石か何もないなら終了
            if(infoList.Count == 0 && _fieldState[position_X, position_Y] == _playerTurn || _fieldState[position_X, position_Y] == stoneState.None)
            {
                break;
            }

            // ある方向を見た時、相手の石の情報が保存されているかつ、その方向に自分の石があるならば、裏返せる石の座標情報を格納
            if(infoList.Count > 0 && _fieldState[position_X, position_Y] == _playerTurn)
            {
                turnCheck = true;
                foreach(var info in infoList)
                {
                    _infoList.Add(info);
                }
                break;
            }
        }
        return turnCheck;
    }

    /* 盤面状態を保存する関数  */
    /* 引数：なし              */
    /* 戻り値：なし            */
    private void SaveBoardState()
    {
        for (int x = 0; x < BOARD_SIZE_X; x++)
        {
            for (int y = 0; y < BOARD_SIZE_Y; y++)
            {
                _savedFieldState[x, y] = _fieldState[x, y];
            }
        }
    }

    /* 黒か白のターンにそのプレイヤーが置けるマスがない場合、ターン変更する関数 */
    /* 引数：なし                                                               */
    /* 戻り値：なし                                                             */
    private void CheckAndTurnChange()
    {
        if(!PutStoneCheck() && !GameOver())
        {
            if(_playerTurn == stoneState.Black)
            {
                _blackCheckFlag = false;
            }
            else if(_playerTurn == stoneState.White)
            {
                _whiteCheckFlag = false;
            }

            // プレイヤーターン変更
            TurnChange();
        }
    }

    /* 全マスに対して、プレイヤーが石を置くことができるか調べる関数 */
    /* 引数：なし                                                   */
    /* 戻り値：                                                     */
    /* turnCheck：置けるならばtrue                                  */
    /*         　 置けないならばfalse                               */
    private bool PutStoneCheck()
    {
        // どこかしらのマスに石を置くことができるか
        var turnCheck = false;
        for(int x = 0; x < BOARD_SIZE_X; x++)
        {
            for(int y = 0; y < BOARD_SIZE_Y; y++)
            {
                // 「そのマスには石を置くことができない」で初期化
                _canPutState[x, y] = canPutState.Cannot;

                for(int i = 0; i <= 7; i++)
                {
                    if(TurnCheck(i, x, y) && _fieldState[x, y] == stoneState.None)
                    {
                        turnCheck = true;
                        _canPutState[x, y] = canPutState.Can;
                    }
                }
            }
        }
        _infoList.Clear();
        return turnCheck;
    }

    /* 石が置けるか判定する関数         */
    /* 引数：                           */
    /* direction：置く位置に対する8方向 */
    /* field_size_x：リバーシ盤のx座標  */
    /* field_size_y：リバーシ盤のy座標  */
    /* 戻り値：                         */
    /* turnCheck：置けるならばtrue      */
    /*         　 置けないならばfalse   */
    private bool TurnCheck(int direction, int field_size_x, int field_size_y)
    {
        var turnCheck = false;                                                                          // false：そのマスに石を置くことができない
        var position_X = field_size_x;                                                                  // 判定対象マスのx座標
        var position_Y = field_size_y;                                                                  // 判定対象マスのy座標
        var OpponentPlayerTurn = _playerTurn == stoneState.Black ? stoneState.White : stoneState.Black; // 相手の石の色
        var infoList = new List<(int, int)>();                                                          // 挟める石の座標情報

        // 選択したマスから8方向を見る
        while(0 <= position_X && 7 >= position_X && 0 <= position_X && 7 >= position_Y)
        {
            switch(direction)
            {
                case 0:  // 左
                    if(position_X == 0){return turnCheck;}
                    position_X--;
                    break;
                case 1:  // 右
                    if(position_X == 7){return turnCheck;}
                    position_X++;
                    break;
                case 2:  // 下
                    if(position_Y == 0){return turnCheck;}
                    position_Y--;
                    break;
                case 3:  // 上
                    if(position_Y == 7){return turnCheck;}
                    position_Y++;
                    break;
                case 4:  // 左下
                    if(position_X == 0){return turnCheck;}
                    if(position_Y == 0){return turnCheck;}
                    position_X--;
                    position_Y--;
                    break;
                case 5:  // 右上
                    if(position_X == 7){return turnCheck;}
                    if(position_Y == 7){return turnCheck;}
                    position_X++;
                    position_Y++;
                    break;
                case 6:  // 左上
                    if(position_X == 0){return turnCheck;}
                    if(position_Y == 7){return turnCheck;}
                    position_X--;
                    position_Y++;
                    break;
                case 7:  // 右下
                    if(position_X == 7){return turnCheck;}
                    if(position_Y == 0){return turnCheck;}
                    position_X++;
                    position_Y--;
                    break;
            }

            // ある方向の一つ目のマスから順に見た時、その座標の石が相手の石ならば、その石の座標情報を残しておく
            if(_fieldState[position_X, position_Y] == OpponentPlayerTurn)
            {
                infoList.Add((position_X, position_Y));
            }

            // ある方向を見た時、選択したマスの隣が、自分の石か何もないなら終了
            if(infoList.Count == 0 && _fieldState[position_X, position_Y] == _playerTurn || _fieldState[position_X, position_Y] == stoneState.None)
            {
                break;
            }

            // ある方向を見た時、相手の石の情報が保存されているかつ、その方向に自分の石があるならば、裏返せる石の座標情報を格納
            if(infoList.Count > 0 && _fieldState[position_X, position_Y] == _playerTurn)
            {
                turnCheck = true;
                foreach(var info in infoList)
                {
                    _infoList.Add(info);
                }
                break;
            }
        }
        return turnCheck;
    }

    /* 終了判定用関数 */
    /* 引数：なし     */
    /* 戻り値：なし   */
    private bool GameOver()
    {
        int whiteNum;  // 白石の数
        int blackNum;  // 黒石の数

        // 白石と黒石の数を数える
        CountStones(out whiteNum, out blackNum);

        // 全てのマスが埋まった or どちらの石も置くことができない or どちらかの石が0個になったときにTrueを返す
        if(whiteNum + blackNum == 64 || !_whiteCheckFlag && !_blackCheckFlag || whiteNum == 0 || blackNum == 0)
        {
            return true;
        }
        return false;
    }

    /* 白石と黒石の数を数える関数       */
    /* 引数：                           */
    /* whiteNum：白石の数を受け取る変数 */
    /* blackNum：黒石の数を受け取る変数 */
    /* 戻り値：                         */
    /* whiteNum：白石の数               */
    /* blackNum：黒石の数               */
    private void CountStones(out int whiteNum, out int blackNum)
    {
        whiteNum = 0;
        blackNum = 0;
        
        for (int x = 0; x < BOARD_SIZE_X; x++)
        {
            for (int y = 0; y < BOARD_SIZE_Y; y++)
            {
                if (_fieldState[x, y] == stoneState.White)
                {
                    whiteNum++;
                }
                else if (_fieldState[x, y] == stoneState.Black)
                {
                    blackNum++;
                }
            }
        }
    }

    /* プレイヤーターンを変更する関数 */
    /* 引数：なし                     */
    /* 戻り値：なし                   */
    private void TurnChange()
    {
        if(_playerTurn == stoneState.Black)
        {
            _playerTurn = stoneState.White;
        }
        else if(_playerTurn == stoneState.White)
        {
            _playerTurn = stoneState.Black;
        }
    }

    /* アシスト状態を切り替える関数 */
    /* 引数：なし                   */
    /* 戻り値：なし                 */
    private void ChangeAssist()
    {
        _assistFlag = _assistFlag == assistState.Off ? assistState.On : assistState.Off;
        AssistText.text = AssistText.text == "アシストON" ? "アシストOFF" : "アシストON";
    }

    /* アシストを表示する関数 */
    /* 引数：なし             */
    /* 戻り値：なし           */
    private void ShowAssistSphere()
    {

        for(int x = 0; x < BOARD_SIZE_X; x++)
        {
            for(int y = 0; y < BOARD_SIZE_Y; y++)
            {
                _canPutSphereState[x, y].FlipSphere(_playerTurn);
            }
        }

        if(_assistFlag == assistState.On)
        {
            ShowAssistSphere(true);
        }
        else if(_assistFlag == assistState.Off)
        {
            ShowAssistSphere(false);
        }
    }

    /* アシストを表示する関数 */
    /* 引数：なし             */
    /* 戻り値：なし           */
    private void ShowAssistSphere(bool showFlag)
    {
        for(int x = 0; x < BOARD_SIZE_X; x++)
        {
            for(int y = 0; y < BOARD_SIZE_Y; y++)
            {
                if(!showFlag)
                {
                    _canPutState[x, y] = canPutState.Cannot;
                }
                _canPutSphereState[x, y].ShowAssist(_canPutState[x, y]);
            }
        }
    }

    /* CircleImageの色を変更する関数 */
    /* 引数：なし                    */
    /* 戻り値：なし                  */
    private void ChangePlayerCircleColor()
    {
        if(_playerTurn == stoneState.Black)  // 黒
        {
            _blackCircleImageCom.color = _originalColor;
            _whiteCircleImageCom.color = Color.gray;
        }
        else if(_playerTurn == stoneState.White)  // 白
        {
            _blackCircleImageCom.color = Color.gray;
            _whiteCircleImageCom.color = _originalColor;
        }
        else if(_playerTurn == stoneState.None)  // ゲーム終了時
        {
            _blackCircleImageCom.color = _originalColor;
            _whiteCircleImageCom.color = _originalColor;
        }
    }

    /* クイズ正解時の処理を行う関数 */
    /* 引数：なし                   */
    /* 戻り値：なし                 */
    public IEnumerator CorrectToQuiz()
    {
        // 裏返す相手の石にFlipアニメーションを適用
        foreach(var pos in _flipPositions)
        {
            _fieldStoneState[pos.x, pos.y].SetState(_playerTurn, StoneScript.AnimationType.Flip);
        }

        // 選択したマスに置く石にDropアニメーションを適用
        _fieldStoneState[_dropPosition.x, _dropPosition.y].SetState(_playerTurn, StoneScript.AnimationType.Drop);

        // 石を置く時のSEを鳴らす
        _seControllerScript.ReversiStoneSE();
        yield return new WaitForSeconds(0.75f);

        // クイズの正解数と解答数をカウント
        if(_playerTurn == stoneState.Black)
        {
            _blackCorrectAnswers++;
            _blackTotalQuestions++;
        }
        else if(_playerTurn == stoneState.White)
        {
            _whiteCorrectAnswers++;
            _whiteTotalQuestions++;
        }

        // Cubeを表示
        Cube.SetActive(true);

        // ターン変更
        TurnChange();

        // 黒か白のターンにそのプレイヤーが置けるマスがない場合、ターン変更
        _blackCheckFlag = true;
        _whiteCheckFlag = true;
        CheckAndTurnChange();
        CheckAndTurnChange();

        // 裏返せる石の座標を格納するリストを初期化
        _infoList.Clear();
        _flipPositions.Clear();

        // ゲーム終了時の処理
        if(GameOver())
        {
            // Cubeを非表示
            Cube.SetActive(false);

            // プレイヤーターンを無効化
            _playerTurn = stoneState.None;

            // BGMを止める
            if (BGMControllerScript.Instance != null)
            {
                BGMControllerScript.Instance.StopBGM();
            }

            // 最終結果表示
            yield return new WaitForSeconds(2.0f);
            StartCoroutine(ShowResult());
        }
    }

    /* 最終結果を表示する関数 */
    /* 引数：なし             */
    /* 戻り値：なし           */
    private IEnumerator ShowResult()
    {
        // 左上に表示されているサポートテキストを全て非表示
        SpaceKeyImage.SetActive(false);
        AssistText.gameObject.SetActive(false);
        ArrowKeyImage.SetActive(false);
        ChoiseText.gameObject.SetActive(false);
        ReturnKeyImage.SetActive(false);
        DicisionText.gameObject.SetActive(false);

        // 結果表示画面の背景を表示
        ResultPanel.SetActive(true);

        // プレイヤーの熊と背景の画像を移動
        MovePlayerImage();
        yield return new WaitForSeconds(1.0f);
        // 勝利判定
        StartCoroutine(Judgement());
        yield return new WaitForSeconds(4.0f);
        // タイトル画面へ移動するボタンを表示
        GoTitleSceneButton.SetActive(true);
    }

    /* プレイヤーの熊と背景の画像を移動する関数 */
    /* 引数：なし                               */
    /* 戻り値：なし                             */
    private void MovePlayerImage()
    {
        // 移動先の座標
        Vector3 targetBlackPosition = new Vector3(-150f, 110f, 0f);
        Vector3 targetWhitePosition = new Vector3(150f, 110f, 0f);

        // プレイヤーの熊と背景の画像を移動させる
        StartCoroutine(MovePlayerImage(BlackBearImage, targetBlackPosition, 1.5f));
        StartCoroutine(MovePlayerImage(BlackCircleImage, targetBlackPosition, 1.5f));
        StartCoroutine(MovePlayerImage(WhiteBearImage, targetWhitePosition, 1.5f));
        StartCoroutine(MovePlayerImage(WhiteCircleImage, targetWhitePosition, 1.5f));
    }

    /* 画像を移動させる関数         */
    /* 引数：                       */
    /* image：移動させる画像        */
    /* targetPosition：移動先の座標 */
    /* duration：移動時間           */ 
    /* 戻り値：なし                 */
    private IEnumerator MovePlayerImage(GameObject image, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = image.transform.localPosition;  // 画像の初期位置
        float elapsedTime = 0f;  // 経過時間

        while(elapsedTime < duration)
        {
            image.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        image.transform.localPosition = targetPosition;
    }

    /* 勝利判定を行う関数 */
    /* 引数：なし         */
    /* 戻り値：なし       */
    private IEnumerator Judgement()
    {
        int whiteNum;  // 白石の数
        int blackNum;  // 黒石の数
        int blackAccuracy;
        int whiteAccuracy;
        int whiteTotalPoints;
        int blackTotalPoints;

        // 白石と黒石の数を数える
        CountStones(out whiteNum, out blackNum);

        // 白と黒の石の数を表示
        yield return new WaitForSeconds(2.0f);
        BlackStoneNumText.text = blackNum.ToString();
        WhiteStoneNumText.text = whiteNum.ToString();
        _seControllerScript.TextAppearanceSE();
        BlackStoneNumText.gameObject.SetActive(true);
        WhiteStoneNumText.gameObject.SetActive(true);
        StoneNumText.gameObject.SetActive(true);

        // 正解率を計算&表示
        yield return new WaitForSeconds(1.0f);
        blackAccuracy = (_blackTotalQuestions > 0) ? (int)((float)_blackCorrectAnswers / _blackTotalQuestions * 100) : 0;
        whiteAccuracy = (_whiteTotalQuestions > 0) ? (int)((float)_whiteCorrectAnswers / _whiteTotalQuestions * 100) : 0;
        BlackAccuracyText.text = blackAccuracy.ToString();
        WhiteAccuracyText.text = whiteAccuracy.ToString();
        _seControllerScript.TextAppearanceSE();
        BlackAccuracyText.gameObject.SetActive(true);
        WhiteAccuracyText.gameObject.SetActive(true);
        PlusText.gameObject.SetActive(true);
        AccuracyText.gameObject.SetActive(true);

        // 合計点を計算
        yield return new WaitForSeconds(1.0f);
        blackTotalPoints = blackNum + blackAccuracy;
        whiteTotalPoints = whiteNum + whiteAccuracy;
        BlackTotalPointsText.text = blackTotalPoints.ToString();
        WhiteTotalPointsText.text = whiteTotalPoints.ToString();
        _seControllerScript.TextAppearanceSE();
        BlackTotalPointsText.gameObject.SetActive(true);
        WhiteTotalPointsText.gameObject.SetActive(true);
        BlackEqualText.gameObject.SetActive(true);
        WhiteEqualText.gameObject.SetActive(true);
        TotalPointsText.gameObject.SetActive(true);

        // 勝利判定
        yield return new WaitForSeconds(1.0f);
        if (blackTotalPoints < whiteTotalPoints)
        {
            // SE
            _seControllerScript.WinnerSE();
            // テキスト変更&表示
            BlackResultText.text = "Lose";
            WhiteResultText.text = "Winner!";
            BlackResultText.gameObject.SetActive(true);
            WhiteResultText.gameObject.SetActive(true);
        }
        else if (blackTotalPoints > whiteTotalPoints)
        {
            // SE
            _seControllerScript.WinnerSE();
            // テキスト変更&表示
            BlackResultText.text = "Winner!";
            WhiteResultText.text = "Lose";
            BlackResultText.gameObject.SetActive(true);
            WhiteResultText.gameObject.SetActive(true);
        }
        else
        {
            // SE
            _seControllerScript.DrawSE();
            // テキスト表示
            DrawResultText.gameObject.SetActive(true);
        }
    }

    /* クイズ不正解時の処理を行う関数 */
    /* 引数：なし                     */
    /* 戻り値：なし                   */
    public void IncorrectToQuiz()
    {
        for (int x = 0; x < BOARD_SIZE_X; x++)
        {
            for (int y = 0; y < BOARD_SIZE_Y; y++)
            {
                _fieldState[x, y] = _savedFieldState[x, y];
                _fieldStoneState[x, y].SetState(_savedFieldState[x, y]);
            }
        }

        // クイズの解答数をカウント
        if(_playerTurn == stoneState.Black)
        {
            _blackTotalQuestions++;
        }
        else if(_playerTurn == stoneState.White)
        {
            _whiteTotalQuestions++;
        }

        // ターン変更
        TurnChange();

        // 黒か白のターンにそのプレイヤーが置けるマスがない場合、ターン変更
        CheckAndTurnChange();

        Cube.SetActive(true);
        _infoList.Clear();
        _flipPositions.Clear();
    }
}