using UnityEngine;

public class BGMControllerScript : MonoBehaviour
{
    [SerializeField] private AudioClip _bgm;       // BGM
    private AudioSource _bgmAudioSource;
    private static BGMControllerScript _instance;  // シングルトンインスタンス
    public static BGMControllerScript Instance     // シングルトンインスタンスにアクセスするプロパティ
    {
        get { return _instance; }
    }

    
    void Awake()
    {
        // シングルトンの実装
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // このオブジェクトを破棄しない
        }
        else
        {
            Destroy(gameObject); // すでにインスタンスが存在する場合は新しいオブジェクトを破棄
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // AudioSourceコンポーネントを取得
        _bgmAudioSource = GetComponent<AudioSource>();
    }

    /* BGMを流す */ 
    public void PlayBGM()
    {
        _bgmAudioSource.clip = _bgm;
        _bgmAudioSource.Play();
    }

    /* BGMを止める */ 
    public void StopBGM()
    {
        _bgmAudioSource.Stop();
    }
}
