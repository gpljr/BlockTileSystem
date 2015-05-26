using UnityEngine;
using System.Collections;

public class CheckPointManager : MonoBehaviour
{

    [SerializeField]
    private GameObject _checkPoint1Object;
    [SerializeField]
    private GameObject _checkPoint2Object;
    [HideInInspector]
    public IntVector[] CheckPoint1Locations;
    public IntVector[] CheckPoint2Locations;
    private int iCheckPointLocationID;

    public static CheckPointManager g;

    void Awake(){
        if (g == null)
        {
            g = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private bool _bPlayer1Entered, _bPlayer2Entered;
    void Update()
    {
        if (_bPlayer1Entered && _bPlayer2Entered)
        {
            MoveCheckPoints();
        }
    }
    void MoveCheckPoints()
    {
		iCheckPointLocationID++;
        if(CheckPoint1Locations[iCheckPointLocationID] != null && CheckPoint1Locations[iCheckPointLocationID] != null)
        {
        WorldManager.g.checkPoint1.gameObject.GetComponent<WorldTrigger>().Location=CheckPoint1Locations[iCheckPointLocationID];
        WorldManager.g.checkPoint2.gameObject.GetComponent<WorldTrigger>().Location=CheckPoint2Locations[iCheckPointLocationID];
    }
    else
    {
        //WorldManager.g.checkPoint1
    }
}

    void OnEnable()
    {
        Events.g.AddListener<LevelLoadedEvent>(LevelLoaded);
        Events.g.AddListener<CheckPointEvent>(CheckPointPass);
    }
    
    void OnDisable()
    {
        Events.g.RemoveListener<LevelLoadedEvent>(LevelLoaded);
        Events.g.RemoveListener<CheckPointEvent>(CheckPointPass);
    }
    void LevelLoaded(LevelLoadedEvent e)
    {
        iCheckPointLocationID=0;
    }
    void CheckPointPass(CheckPointEvent e)
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
}
