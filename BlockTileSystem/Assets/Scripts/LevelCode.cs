using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelCode : MonoBehaviour {
    private static int _iCurrentLevel;
    public static int CurrentLevel {
        get { return _iCurrentLevel; }
    }
    private bool _bNewLevelLoaded;

    [SerializeField]
    float
        _timeToFadeIn;
        
    [SerializeField]
    float
        _timeToFadeOut;
    [SerializeField]
    AnimationCurve
        _fadeCurve;
    [SerializeField]
    private GameObject image;
    private Image _image;
    [HideInInspector]
    public bool inFading = true;
    private bool _bPlayer1Entered, _bPlayer2Entered;

    private bool _inLevel;
    [SerializeField]
    GameObject startingScreen;
    [SerializeField]
    GameObject endingScreen;
    [SerializeField]
    GameObject inLevelScreen;
    [SerializeField] Text startingText;
    [SerializeField] GameObject inLevelText;

    [SerializeField]
    Text levelProgression;

    [SerializeField]
    AudioClip _audioNextLevel;

    private bool _isMergingShaderComplete;

    public static GameState gameState;
    public static LevelType levelType;

    public static float musicVolume = 1f;
    public static float audioVolume = 1f;

    [SerializeField] bool _isDevMode=true;
    public static bool isDevMode;

    [SerializeField] GameObject DevModeText;

    bool isRestarting;

    [SerializeField]int levelCount=16;
    public static int LevelCount;

    [SerializeField] GameObject inLevelUI;

    void Start () {
        image.SetActive(true);
        _image = image.GetComponent<Image>();
        EnterStartingScreen();
        //Cursor.visible = false;
        isDevMode = _isDevMode;
        DevModeText.SetActive(isDevMode);
        LevelCount=levelCount;

    }

    void Update () {
        
        // if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
        //     if (Input.GetKeyDown(KeyCode.D)) {
        //         isDevMode = !isDevMode;
        //         //DevModeText.SetActive(isDevMode);
        //     }
        // }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            if (Input.GetKeyDown(KeyCode.U)) {
                inLevelUI.SetActive(!inLevelUI.activeSelf);
            }
        }
        switch (gameState) {
            case GameState.Starting:
                if (Input.anyKeyDown) {
                    
                    LoadLevel(1);
                }
                break;
            case GameState.InLevel:
            
                if (Input.GetKeyDown(KeyCode.R) || Input.GetKeyDown(KeyCode.Return)) {            
                    Restart();
                }
                if (Input.GetKeyDown(KeyCode.N)) {
                    LoadNextLevel();
                }
                if (_bPlayer1Entered && _bPlayer2Entered) {
                    if (levelType == LevelType.Merging) {
                        if (_isMergingShaderComplete) {
                            LoadNextLevel();
                            _isMergingShaderComplete = false;
                        }
                    } else {
                        LoadNextLevel();
                    }
                }
                break;
            case GameState.Ending:
                if (Input.GetKeyDown(KeyCode.Return)) {            
                    Application.LoadLevel(Application.loadedLevel);
                }
                if (Input.GetKeyDown(KeyCode.Escape)) {            
                    Application.Quit();
                }
                break;
        }
        
        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Escape)) {
            Application.LoadLevel(Application.loadedLevel);
        }
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {

            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                LoadLevel(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2)) {
                LoadLevel(2);
            }
            if (Input.GetKeyDown(KeyCode.Alpha3)) {
                LoadLevel(3);
            }
            if (Input.GetKeyDown(KeyCode.Alpha4)) {
                LoadLevel(4);
            }
            if (Input.GetKeyDown(KeyCode.Alpha5)) {
                LoadLevel(5);
            }
            if (Input.GetKeyDown(KeyCode.Alpha6)) {
                LoadLevel(6);
            }
            if (Input.GetKeyDown(KeyCode.Alpha7)) {
                LoadLevel(7);
            }
            if (Input.GetKeyDown(KeyCode.Alpha8)) {
                LoadLevel(8);
            }
            if (Input.GetKeyDown(KeyCode.Alpha9)) {
                LoadLevel(9);
            }

            if (Input.GetKeyDown(KeyCode.Alpha0)) {
                LoadLevel(levelCount);
            }
        }

        if (_bNewLevelLoaded) {
            StartLevel();
            _bNewLevelLoaded = false;            
        }

        
        
    }

    void OnEnable () {
        Events.g.AddListener<LevelLoadedEvent>(LevelLoaded);
        Events.g.AddListener<LevelStarEvent>(LevelPass);
        Events.g.AddListener<BulletHitEvent>(BulletHit);
        Events.g.AddListener<MergingShaderCompleteEvent>(MergingShaderComplete);

    }
    
    void OnDisable () {
        Events.g.RemoveListener<LevelLoadedEvent>(LevelLoaded);
        Events.g.RemoveListener<LevelStarEvent>(LevelPass);
        Events.g.RemoveListener<BulletHitEvent>(BulletHit);
        Events.g.RemoveListener<MergingShaderCompleteEvent>(MergingShaderComplete);
    }
    void LevelPass (LevelStarEvent e) {
        if (e.isEntered) {
            switch (e.CharacterID) {
                case 1:
                    _bPlayer1Entered = true;
                    break;
                case 2:
                    _bPlayer2Entered = true;
                    break;
                case 3:
                    _bPlayer1Entered = true;
                    _bPlayer2Entered = true;
                    break;
            }
        } else {
            switch (e.CharacterID) {
                case 1:
                    _bPlayer1Entered = false;
                    break;
                case 2:
                    _bPlayer2Entered = false;
                    break;
                case 3:
                    _bPlayer1Entered = false;
                    _bPlayer2Entered = false;
                    break;
            }
        }
    }

    void MergingShaderComplete (MergingShaderCompleteEvent e) {
        _isMergingShaderComplete = true;
    }

    public void Restart () {
        isRestarting=true;
        _timeToFadeIn = 0.5f;
        _timeToFadeOut = 0.5f;
        Events.g.Raise(new RestartBeginEvent());
        if (WorldManager.g.checkPointsMoved) {
            //restart to checkpoint
            StartLevel();
            WorldManager.g.RestartToCheckPoints();
        } else {
            LoadLevel(_iCurrentLevel);
        }
    }
    void BulletHit (BulletHitEvent e) {
        if (!isRestarting) {
            Restart();
        }
    }


    void LevelLoaded (LevelLoadedEvent e) {
        _bNewLevelLoaded = true;
        inLevelScreen.SetActive(true);
        startingScreen.SetActive(false);
        endingScreen.SetActive(false);
        _iCurrentLevel = e.iLevel;
    }

    public void LoadLevel (int iLevel) {
        _bPlayer1Entered = false;
        _bPlayer2Entered = false;
        
        EndLevel(iLevel);
    }
    public void LoadNextLevel () {
        AudioSource.PlayClipAtPoint(_audioNextLevel, CameraControl.cameraLoc, LevelCode.audioVolume);

        _timeToFadeIn = 1f;
        _timeToFadeOut = 1f;
        if (levelType == LevelType.Combined || _iCurrentLevel==levelCount) {
            //EndLevel(0);
            EnterEndingScreen();
        } else {
            LoadLevel(_iCurrentLevel + 1);
        }

    }
    public void EnterEndingScreen () {
        StartCoroutine(Fade(_timeToFadeIn, _fadeCurve, fadeIn: true));
        gameState = GameState.Ending;
        startingScreen.SetActive(false);
        endingScreen.SetActive(true);
        inLevelScreen.SetActive(false);

    }
    void EnterStartingScreen () {
        StartCoroutine(Fade(_timeToFadeIn, _fadeCurve, fadeIn: true));
        gameState = GameState.Starting;
        
        startingScreen.SetActive(true);
        StartCoroutine(ShowStartingText());
        endingScreen.SetActive(false);
        inLevelScreen.SetActive(false);
    }
    IEnumerator ShowStartingText () {
        startingText.color = new Color(0.67f, 0.67f, 0.67f, 0f);
        yield return new WaitForSeconds(1.5f);
        float timer = 0f;
        while (timer < 3f) {
            timer += Time.deltaTime;
            startingText.color = new Color(0.67f, 0.67f, 0.67f, 0.67f * Mathf.Max(0f, (timer / 3f)));
            yield return null;
        }
    }

    void StartLevel () {
        StartCoroutine(Fade(_timeToFadeIn, _fadeCurve, fadeIn: true));
        gameState = GameState.InLevel;
        isRestarting=false;
    }

    private IEnumerator Fade (float timerDuration, AnimationCurve curve, bool fadeIn) {
        Color startColor = _image.color;
        Color newColor;
        float alpha;
        float timer = 0f;
        if ((timer < timerDuration * 0.75f && fadeIn) || (timer < timerDuration && !fadeIn)) {
            inFading = true;
            Events.g.Raise(new FadingEvent(isFading: inFading, fadeIn: fadeIn));
        }
        while (timer < timerDuration) {
            timer += Time.deltaTime;
            if (fadeIn) {
                alpha = 1f - curve.Evaluate(timer / timerDuration);
            } else {
                alpha = curve.Evaluate(timer / timerDuration);
            }
            newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            _image.color = newColor;
            yield return null;
        }
        inFading = false;
        Events.g.Raise(new FadingEvent(isFading: inFading, fadeIn: fadeIn));
        if (fadeIn) {
            alpha = 1f - curve.Evaluate(1f);
        } else {
            alpha = curve.Evaluate(1f);
        }
        newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
        _image.color = newColor;
        Events.g.Raise(new RestartEvent());
    }
    IEnumerator FadeOut (float timerDuration, AnimationCurve curve, int iLevelToLoad) {
        yield return StartCoroutine(Fade(timerDuration, curve, fadeIn: false));
        if (iLevelToLoad != 0) {
            Events.g.Raise(new LoadLevelEvent(iLevel: iLevelToLoad));
        }

    }
    

    public void EndLevel (int iLevelToLoad) {
        gameState = GameState.InTransition;
        StartCoroutine(FadeOut(_timeToFadeOut, _fadeCurve, iLevelToLoad));
    }
}
