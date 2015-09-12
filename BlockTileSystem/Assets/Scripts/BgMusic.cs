using UnityEngine;
using System.Collections;

public class BgMusic : MonoBehaviour
{
    private int _iCurrentMusicId = 1;
    private int _iMusicId = 1;

    public AudioClip beginMusic;
    public AudioClip sadMusic;
    public AudioClip happyMusic;

    // void OnEnable()
    // {			
    //     Events.g.AddListener<LevelLoadedEvent>(LevelEntered);
    // }
	
    // void OnDisable()
    // {
    //     Events.g.AddListener<LevelLoadedEvent>(LevelEntered);
    // }

    void Update()
    {
        switch (LevelCode.gameState)
        {
            case GameState.Starting:
                _iMusicId = 1;
                break;
            case GameState.InLevel:
                switch (LevelCode.levelType)
                {
                    case LevelType.Normal:
                        _iMusicId = 1;
                        break;
                    case LevelType.Separation:
                        _iMusicId = 2;
                        break;
                    case LevelType.Merging:
                        _iMusicId = 2;
                        break;
                    case LevelType.Combined:
                        _iMusicId = 3;
                        break;
                }
                break;
            case GameState.Ending:
                _iMusicId = 3;
                break;
        }
        
        if (_iCurrentMusicId != _iMusicId)
        {
            PlayMusicById(_iMusicId);
            _iCurrentMusicId = _iMusicId;
        }
			
    }

    private void PlayMusicById(int musicId)
    {
        while (gameObject.GetComponent<AudioSource>().volume > 0)
        {
            gameObject.GetComponent<AudioSource>().volume -= 0.5f * Time.deltaTime;
						
            if (gameObject.GetComponent<AudioSource>().volume < 0.1f)
            {										
                switch (musicId)
                {
                    case 1:
                        gameObject.GetComponent<AudioSource>().clip = beginMusic;
                        break;
                    case 2:
                        gameObject.GetComponent<AudioSource>().clip = sadMusic;

                        break;
                    case 3:
                        gameObject.GetComponent<AudioSource>().clip = happyMusic;
                        break;
                }
                gameObject.GetComponent<AudioSource>().Play();
                break;
            }
        }
        while (gameObject.GetComponent<AudioSource>().volume < 1)
        {
            gameObject.GetComponent<AudioSource>().volume += 0.5f * Time.deltaTime;
        }
    }
    
    
}
