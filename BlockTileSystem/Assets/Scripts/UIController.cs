using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    [SerializeField] GameObject UIPanel;
    float tempMusicVolume;
    float tempAudioVolume;

    [SerializeField] Slider musicSlider;
    [SerializeField] Slider audioSlider;

    [SerializeField] Image MusicButton;
    [SerializeField] Image AudioButton;
    [SerializeField] Sprite MusicOn;
    [SerializeField] Sprite MusicOff;
    [SerializeField] Sprite AudioOn;
    [SerializeField] Sprite AudioOff;

    [SerializeField] Button Setting;
    [SerializeField] Button RestartLevel;
    [SerializeField] Sprite settingHighlight;
    [SerializeField] Sprite settingNormal;
    [SerializeField] Sprite restartLevelHighlight;
    [SerializeField] Sprite restartLevelNormal;
    [SerializeField] Text RestartText;
    bool restartTextShowed;

    [SerializeField] Text StuckText;
    bool stuckTextShowed;

    [SerializeField] GameObject cameraControl;

    // Use this for initialization
    void Start () {
        HideUIPanel();
        musicSlider.onValueChanged.AddListener(delegate {
                musicValueUpdate();
            });
        musicSlider.onValueChanged.AddListener(delegate {
                audioValueUpdate();
            });

        SetButtonHighlight(Setting, settingHighlight);
        RestartText.gameObject.SetActive(false);
        StuckText.gameObject.SetActive(false);
    }
    void SetButtonHighlight (Button button, Sprite highlightSprite) {
        SpriteState settingST = new SpriteState();
        settingST.highlightedSprite = highlightSprite;
        settingST.pressedSprite = highlightSprite;
        settingST.disabledSprite = highlightSprite;
        button.spriteState = settingST;
        print("set state");
    }
    //not working

    public void RestartButtonPointerEnter () {
        RestartLevel.image.sprite = restartLevelHighlight;
        if (!restartTextShowed) {
            RestartText.gameObject.SetActive(true);
        }
        restartTextShowed = true;
        StuckText.gameObject.SetActive(false);
    }
    public void RestartButtonPointerExit () {
        RestartLevel.image.sprite = restartLevelNormal;
        StartCoroutine(RestartTextDisappear());
    }
    IEnumerator RestartTextDisappear () {
        yield return new WaitForSeconds(0.5f);
        RestartText.gameObject.SetActive(false);
        
    }
    public void SettingButtonPointerEnter () {
        Setting.image.sprite = settingHighlight;
    }
    public void SettingButtonPointerExit () {
        Setting.image.sprite = settingNormal;
    }
    
    void Update () {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ShowUIPanel();
        }
    }
    // void SettingButtonHighlight () {
    //     Setting.image.sprite = settingHighlight;
    // }
    public void ShowUIPanel () {
        UIPanel.SetActive(!UIPanel.activeSelf);
        cameraControl.GetComponent<CameraControl>().CameraBlur(UIPanel.activeSelf);
    }
    public void HideUIPanel () {
        UIPanel.SetActive(false);
        cameraControl.GetComponent<CameraControl>().CameraBlur(false);
    }
    public void QuitGame () {
        Application.Quit();
    }
    public void RestartGame () {
        Application.LoadLevel(Application.loadedLevel);
    }
    public void MuteMusic () {
        if (LevelCode.musicVolume > 0) {
            //mute
            tempMusicVolume = LevelCode.musicVolume;
            LevelCode.musicVolume = 0f;
            musicSlider.value = 0f;
            MusicButton.sprite = MusicOff;
        } else {
            //unmute
            LevelCode.musicVolume = tempMusicVolume;
            musicSlider.value = tempMusicVolume;
            MusicButton.sprite = MusicOn;
        }
    }
    public void MuteAudio () {
        if (LevelCode.audioVolume > 0) {
            //mute
            tempAudioVolume = LevelCode.audioVolume;
            LevelCode.audioVolume = 0f;
            audioSlider.value = 0f;
            AudioButton.sprite = AudioOff;
        } else {
            //unmute
            LevelCode.audioVolume = tempAudioVolume;
            audioSlider.value = tempAudioVolume;
            AudioButton.sprite = AudioOn;
        }
    }
    void musicValueUpdate () {
        LevelCode.musicVolume = musicSlider.value;
    }
    void audioValueUpdate () {
        LevelCode.audioVolume = audioSlider.value;
    }

    void LevelLoaded (LevelLoadedEvent e) {
        if (e.iLevel == 2) {
            StartCoroutine(ShowStuckRestartUI());
        }
    }
    IEnumerator ShowStuckRestartUI () {
        yield return new WaitForSeconds(1f);
        if (!stuckTextShowed) {
            StuckText.gameObject.SetActive(true);
            stuckTextShowed = true;
            StartCoroutine(StuckTextDisappear());
        }
    }
    IEnumerator StuckTextDisappear () {
        yield return new WaitForSeconds(4f);
        StuckText.gameObject.SetActive(false);
    }

    void OnEnable () {
        Events.g.AddListener<LevelLoadedEvent>(LevelLoaded);
    }
    void OnDisable () {
        Events.g.RemoveListener<LevelLoadedEvent>(LevelLoaded);
    }
}
