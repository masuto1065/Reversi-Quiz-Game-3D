using UnityEngine;
using UnityEngine.UI;

public class SettingScript : MonoBehaviour
{
    public GameObject SettingPanel;     // 設定画面の背景
    public Slider BGMSlider;            // BGMの音量スライダー
    public Slider SESlider;             // SEの音量スライダー
    public AudioSource BGMAudioSource;  // BGMのAudioSource
    public AudioSource SEAudioSource;   // SEのAudioSource

    // Start is called before the first frame update
    void Start()
    {
        BGMSlider.value = PlayerPrefs.GetFloat("BGMVolume", 0.2f);  // 保存されたBGM音量を読み込む
        SESlider.value = PlayerPrefs.GetFloat("SEVolume", 0.45f);   // 保存されたSE音量を読み込む
        UpdateVolume();
    }

    /* 「閉じる」ボタンを押したときに設定画面を閉じる関数 */
    public void CloseSettingPanel()
    {
        // 音量設定の保存
        PlayerPrefs.SetFloat("BGMVolume", BGMSlider.value);
        PlayerPrefs.SetFloat("SEVolume", SESlider.value);

        // SettingPanelを非表示
        SettingPanel.SetActive(false);
    }

    /* 音量を設定する関数 */
    public void UpdateVolume()
    {
        BGMAudioSource.volume = BGMSlider.value;  // BGM音量を設定
        SEAudioSource.volume = SESlider.value;    // SE音量を設定
    }
}
