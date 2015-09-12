using UnityEngine;
using System.Collections;

public class CheckPointsManager : MonoBehaviour {
	[HideInInspector]
	public GameObject checkPoint1Object, checkPoint2Object;
    private bool _bPlayer1Entered, _bPlayer2Entered;
    [HideInInspector]
    public IntVector[] CheckPoint1Locations;
    [HideInInspector]
    public IntVector[] CheckPoint2Locations;
    [HideInInspector]
    public int iCheckPointLocationID;
    [HideInInspector]
    public int iChar1InCheckPoint, iChar2InCheckPoint;

    [SerializeField]
    AudioClip _audio;

        private MapEditor mapEditor;
    void Awake()
    {
                mapEditor = gameObject.GetComponent<MapEditor>();
    }
        void OnEnable()
    {
        Events.g.AddListener<CheckPointEvent>(CheckPointPass);
    }
    
    void OnDisable()
    {
        Events.g.RemoveListener<CheckPointEvent>(CheckPointPass);
    }
    void Update()
    {
    	CheckPointsCheck();
    }
    private void CheckPointsCheck()
    {
        if (_bPlayer1Entered && _bPlayer2Entered)
        {
            AudioSource.PlayClipAtPoint(_audio, Vector3.zero, LevelCode.audioVolume);
            _bPlayer1Entered = false;
            _bPlayer2Entered = false;
            MoveCheckPoints();
        }
    }
    private void MoveCheckPoints()
    {
        iCheckPointLocationID++;
        IntVector zero = new IntVector(0, 0);
        if (LevelCode.levelType != LevelType.Combined)
        {
            if (iCheckPointLocationID < CheckPoint1Locations.Length &&
                CheckPoint1Locations[iCheckPointLocationID] != zero &&
                CheckPoint2Locations[iCheckPointLocationID] != zero)
            {
            
                checkPoint1Object.GetComponent<WorldTrigger>().Location 
                = mapEditor.PositionFlip(CheckPoint1Locations[iCheckPointLocationID]);
                checkPoint2Object.GetComponent<WorldTrigger>().Location 
                = mapEditor.PositionFlip(CheckPoint2Locations[iCheckPointLocationID]);
            }
            else
            {
                Destroy(checkPoint1Object);
                Destroy(checkPoint2Object);
                iCheckPointLocationID--;
                WorldManager.g.checkPointsMoved = true;
            }
        }
        else
        {
            if (iCheckPointLocationID < CheckPoint1Locations.Length &&
                CheckPoint1Locations[iCheckPointLocationID] != zero)
            {
                checkPoint1Object.GetComponent<WorldTrigger>().Location 
                = mapEditor.PositionFlip(CheckPoint1Locations[iCheckPointLocationID]);
            }
            else
            {
                Destroy(checkPoint1Object);
                iCheckPointLocationID--;
                WorldManager.g.checkPointsMoved = true;
            }
        }
    }
    void CheckPointPass(CheckPointEvent e)
    {

        if (e.isEntered)
        {
            switch (e.CharacterID)
            {
                case 1:
                    _bPlayer1Entered = true;
                    iChar1InCheckPoint = e.iCheckPointID;
        
                    break;
                case 2:
                    _bPlayer2Entered = true;
                    iChar2InCheckPoint = e.iCheckPointID;
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
                    iChar1InCheckPoint = 0;
                    break;
                case 2:
                    _bPlayer2Entered = false;
                    iChar2InCheckPoint = 0;
                    break;
            }
        }
    }
}
