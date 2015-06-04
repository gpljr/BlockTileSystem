using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelCode : MonoBehaviour
{
    private int _iCurrentLevel;
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

    [SerializeField]
    Text levelProgression;

    private bool _isMergingShaderComplete;

    public static GameState gameState;
    public static LevelType levelType;

    public static float musicVolume = 1f;
    public static float audioVolume = 0.3f;


    void Start()
    {
        image.SetActive(true);
        _image = image.GetComponent<Image>();
        EnterStartingScreen();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            LoadLevel(10);
        }
        switch (gameState)
        {
            case GameState.Starting:
                if (Input.anyKeyDown)
                {
                    
                    LoadLevel(1);
                }
                break;
            case GameState.InLevel:
            
                levelProgression.text = "Level: " + _iCurrentLevel + "/12";
                if (Input.GetKeyDown(KeyCode.Return))
                {            
                    Restart();
                }
                if (Input.GetKeyDown(KeyCode.N))
                {
                    LoadNextLevel();
                }
                if (_bPlayer1Entered && _bPlayer2Entered)
                {
                    if (levelType == LevelType.Merging)
                    {
                        if (_isMergingShaderComplete)
                        {
                            LoadNextLevel();
                            _isMergingShaderComplete=false;
                        }
                    }
                    else
                    {
                        LoadNextLevel();
                    }
                }
                break;
        }
        
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            EnterStartingScreen();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (_bNewLevelLoaded)
        {
            StartLevel();
            _bNewLevelLoaded = false;            
        }

        
        
    }

    void OnEnable()
    {
        Events.g.AddListener<LevelLoadedEvent>(LevelLoaded);
        Events.g.AddListener<LevelStarEvent>(LevelPass);
        Events.g.AddListener<BulletHitEvent>(BulletHit);
        Events.g.AddListener<MergingShaderCompleteEvent>(MergingShaderComplete);

    }
    
    void OnDisable()
    {
        Events.g.RemoveListener<LevelLoadedEvent>(LevelLoaded);
        Events.g.RemoveListener<LevelStarEvent>(LevelPass);
        Events.g.RemoveListener<BulletHitEvent>(BulletHit);
        Events.g.RemoveListener<MergingShaderCompleteEvent>(MergingShaderComplete);
    }
    void LevelPass(LevelStarEvent e)
    {
        if (e.isEntered)
        {
            switch (e.CharacterID)
            {
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
        }
        else
        {
            switch (e.CharacterID)
            {
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

    void MergingShaderComplete(MergingShaderCompleteEvent e)
    {
        _isMergingShaderComplete=true;
    }

    void Restart()
    {
        _timeToFadeIn = 0.5f;
        _timeToFadeOut = 0.5f;
        if (WorldManager.g.checkPointsMoved)
        {
            //restart to checkpoint
            StartLevel();
            WorldManager.g.RestartToCheckPoints();
        }
        else
        {
            LoadLevel(_iCurrentLevel);
        }
    }
    void BulletHit(BulletHitEvent e)
    {
        
        Restart();
    }

    void LevelLoaded(LevelLoadedEvent e)
    {
        _bNewLevelLoaded = true;
        inLevelScreen.SetActive(true);
        startingScreen.SetActive(false);
        endingScreen.SetActive(false);
        _iCurrentLevel = e.iLevel;

    }

    public void LoadLevel(int iLevel)
    {
        _bPlayer1Entered = false;
        _bPlayer2Entered = false;
        
        EndLevel(iLevel);
    }
    public void LoadNextLevel()
    {
        _timeToFadeIn = 1f;
        _timeToFadeOut = 1f;
        if (levelType == LevelType.Combined)
        {
            EnterEndingScreen();
        }
        else
        {
            LoadLevel(_iCurrentLevel + 1);
        }

    }
    void EnterEndingScreen()
    {
        gameState = GameState.Ending;
        startingScreen.SetActive(false);
        endingScreen.SetActive(true);
        inLevelScreen.SetActive(false);

    }
    void EnterStartingScreen()
    {
        gameState = GameState.Starting;
        startingScreen.SetActive(true);
        endingScreen.SetActive(false);
        inLevelScreen.SetActive(false);
    }

    void StartLevel()
    {
        StartCoroutine(Fade(_timeToFadeIn, _fadeCurve, fadeIn: true));
        gameState = GameState.InLevel;
    }

    private IEnumerator Fade(float timerDuration, AnimationCurve curve, bool fadeIn)
    {
        Color startColor = _image.color;
        Color newColor;
        float alpha;
        float timer = 0f;
        if ((timer < timerDuration * 0.75f && fadeIn) || (timer < timerDuration && !fadeIn))
        {
            inFading = true;
            Events.g.Raise(new FadingEvent(true));
        }
        while (timer < timerDuration)
        {
            timer += Time.deltaTime;
            if (fadeIn)
            {
                alpha = 1f - curve.Evaluate(timer / timerDuration);
            }
            else
            {
                alpha = curve.Evaluate(timer / timerDuration);
            }
            newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            _image.color = newColor;
            yield return null;
        }
        inFading = false;
        Events.g.Raise(new FadingEvent(false));
        if (fadeIn)
        {
            alpha = 1f - curve.Evaluate(1f);
        }
        else
        {
            alpha = curve.Evaluate(1f);
        }
        newColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
        _image.color = newColor;
    }
    IEnumerator FadeOut(float timerDuration, AnimationCurve curve, int iLevelToLoad)
    {
        yield return StartCoroutine(Fade(timerDuration, curve, fadeIn: false));
        Events.g.Raise(new LoadLevelEvent(iLevel: iLevelToLoad));
    }
    

    public void EndLevel(int iLevelToLoad)
    {
        gameState = GameState.InTransition;
        StartCoroutine(FadeOut(_timeToFadeOut, _fadeCurve, iLevelToLoad));
    }
}
