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

    void Start()
    {
        image.SetActive(true);
        _image = image.GetComponent<Image>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            LoadLevel(1);
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            LoadLevel(_iCurrentLevel);
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

        if(Input.GetKeyDown(KeyCode.Keypad1))
        {
            LoadLevel(1);
        }
        if(Input.GetKeyDown(KeyCode.Keypad2))
        {
            LoadLevel(2);
        }
        if(Input.GetKeyDown(KeyCode.Keypad3))
        {
            LoadLevel(3);
        }
        if(Input.GetKeyDown(KeyCode.Keypad4))
        {
            LoadLevel(4);
        }
        if(Input.GetKeyDown(KeyCode.Keypad5))
        {
            LoadLevel(5);
        }
        if(Input.GetKeyDown(KeyCode.Keypad6))
        {
            LoadLevel(6);
        }
        if(Input.GetKeyDown(KeyCode.Keypad7))
        {
            LoadLevel(7);
        }
        if(Input.GetKeyDown(KeyCode.Keypad8))
        {
            LoadLevel(8);
        }
        if(Input.GetKeyDown(KeyCode.Keypad9))
        {
            LoadLevel(9);
        }

    }

    void OnEnable()
    {
        Events.g.AddListener<LevelLoadedEvent>(LevelLoaded);
        Events.g.AddListener<LevelStarEvent>(LevelPass);
    }
    
    void OnDisable()
    {
        Events.g.RemoveListener<LevelLoadedEvent>(LevelLoaded);
        Events.g.RemoveListener<LevelStarEvent>(LevelPass);
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
            }
        }
    }

    void LevelLoaded(LevelLoadedEvent e)
    {
        _bNewLevelLoaded = true;
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
        LoadLevel(_iCurrentLevel + 1);
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
