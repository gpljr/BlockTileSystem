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

    void Start()
    {
        image.SetActive(true);
        _image = image.GetComponent<Image>();//TODO
            
            
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
        if (Input.GetKeyDown(KeyCode.N))
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
    }
    
    void OnDisable()
    {
        Events.g.RemoveListener<LevelLoadedEvent>(LevelLoaded);
    }

    void LevelLoaded(LevelLoadedEvent e)
    {
        _bNewLevelLoaded = true;
        _iCurrentLevel = e.iLevel;
    }

    public void LoadLevel(int iLevel)
    {
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
        //Application.LoadLevel(Application.loadedLevel + 1);
        Events.g.Raise(new LoadLevelEvent(iLevel: iLevelToLoad));
    }
    

    public void EndLevel(int iLevelToLoad)
    {
        StartCoroutine(FadeOut(_timeToFadeOut, _fadeCurve, iLevelToLoad));
    }

    //fade, load level, music,general
}
