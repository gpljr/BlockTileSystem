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
    private bool _isLastLevel;
[SerializeField]
    GameObject startingScreen;
    [SerializeField]
    GameObject endingScreen;
    [SerializeField]
    GameObject inLevelScreen;

    void Start()
    {
        image.SetActive(true);
        _image = image.GetComponent<Image>();
        EnterStartingScreen();
    }

    void Update()
    {
        if (!_inLevel && Input.anyKeyDown)
        {
            LoadLevel(1);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            EnterStartingScreen();
        }
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
            
            LoadNextLevel();
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
    }
    
    void OnDisable()
    {
        Events.g.RemoveListener<LevelLoadedEvent>(LevelLoaded);
        Events.g.RemoveListener<LevelStarEvent>(LevelPass);
        Events.g.RemoveListener<BulletHitEvent>(BulletHit);
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
    void Restart()
    {
        _timeToFadeIn = 0.5f;
        _timeToFadeOut = 0.5f;
        if (WorldManager.g.iCheckPointLocationID > 0)
        {
            //restart to checkpoint
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
        if (e.iLevelType == 4)
        {
            _isLastLevel = true;
        }
    }

    public void LoadLevel(int iLevel)
    {
        _bPlayer1Entered = false;
        _bPlayer2Entered = false;
        if (_isLastLevel)
        {
            _isLastLevel = false;
            EnterEndingScreen();
        }
        else
        {
            _inLevel = true;
            
            EndLevel(iLevel);
        }
    }
    public void LoadNextLevel()
    {
        _timeToFadeIn = 1f;
        _timeToFadeOut = 1f;

        LoadLevel(_iCurrentLevel + 1);

    }
    void EnterEndingScreen()
    {
        _inLevel = false;
        WorldManager.g.inLevel=false;
        startingScreen.SetActive(false);
        endingScreen.SetActive(true);
        inLevelScreen.SetActive(false);

    }
    void EnterStartingScreen()
    {
        _inLevel=false;
        startingScreen.SetActive(true);
        endingScreen.SetActive(false);
        inLevelScreen.SetActive(false);
    }

    void StartLevel()
    {
        StartCoroutine(Fade(_timeToFadeIn, _fadeCurve, fadeIn: true));
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
        StartCoroutine(FadeOut(_timeToFadeOut, _fadeCurve, iLevelToLoad));
    }
}
