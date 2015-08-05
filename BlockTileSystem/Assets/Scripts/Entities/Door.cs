using UnityEngine;
using System.Collections;

public class Door : MonoBehaviour
{

    //set in the XML
    public int iID;
    //to associate with the trigger
    public int iTriggerNumber = 1;

    public bool isTriggered1, isTriggered2;
    public bool isOpen;
    public bool isHalfOpen;

    public bool isVertical;//up and down passing

    private WorldEntity _worldEntity;

    [SerializeField]
    private Sprite _closedDoor1vertical;
    [SerializeField]
    private Sprite _halfOpenDoor1vertical;
    [SerializeField]
    private Sprite _openDoor1vertical;
    [SerializeField]
    private Sprite _closedDoor1horizontal;
    [SerializeField]
    private Sprite _halfOpenDoor1horizontal;
    [SerializeField]
    private Sprite _openDoor1horizontal;


    [SerializeField]
    private Sprite _closedDoor2vertical;
    [SerializeField]
    private Sprite _halfOpenDoor2vertical;
    
    [SerializeField]
    private Sprite _closedDoor2horizontal;
    [SerializeField]
    private Sprite _halfOpenDoor2horizontal;
    


    [SerializeField]
    private Sprite _closedDoor3vertical;
    [SerializeField]
    private Sprite _halfOpenDoor3vertical;
    
    [SerializeField]
    private Sprite _closedDoor3horizontal;
    [SerializeField]
    private Sprite _halfOpenDoor3horizontal;
    




    public void Cache()
    {
        _worldEntity = GetComponent<WorldEntity>();
    }

    void Awake()
    {
        Cache();
        _worldEntity.CollidingType = EntityCollidingType.Colliding;
        _worldEntity.entityType = EntityType.Door;

    }
    void Update()
    {
        if (!_worldEntity.isSpriteSet)
        {
            _worldEntity.SetVisual(GetSpriteByID());
        }
    }

    void OnEnable()
    {
        _worldEntity.Simulators += Simulate;
        Events.g.AddListener<StepTriggerEvent>(Triggered);
    }

    void OnDisable()
    {
        _worldEntity.Simulators -= Simulate;
        Events.g.RemoveListener<StepTriggerEvent>(Triggered);
    }

    private void Simulate()
    {        
        if (iTriggerNumber == 1)
        {
            if (isTriggered1)
            {
                OpenDoor();
            }
        }
        else if (iTriggerNumber == 2)
        {
            if (isTriggered1 && isTriggered2)
            {
                OpenDoor();
            }
            else if (isTriggered1 || isTriggered2)
            {
                HalfOpenDoor();
            }
        }
        
    }
    void OpenDoor()
    {
        isOpen = true;
        isHalfOpen = false;
        _worldEntity.CollidingType = EntityCollidingType.Empty;
        //_worldEntity.DestroyVisual();
        
        _worldEntity.ChangeVisual(GetSpriteByID());
    }
    void HalfOpenDoor()
    {
        isHalfOpen = true;
        isOpen = false;
        _worldEntity.ChangeVisual(GetSpriteByID());
        //play animation;
    }
    void Triggered(StepTriggerEvent e)
    {
        if (e.triggerID == iID)
        {
            if (iTriggerNumber == 1)
            {
                isTriggered1 = true;
            }
            else if (iTriggerNumber == 2)
            {
                if (isTriggered1)
                {
                    isTriggered2 = true;
                }
                else
                {
                    isTriggered1 = true;
                }
            }
        }
    }
    private Sprite GetSpriteByID()
    {
        Sprite sprite = new Sprite();
        if(isVertical)
        {
        if (isHalfOpen)
        {
            switch (iID%3)
            {
                case 1:
                    sprite = _halfOpenDoor1vertical;
                    break;
                case 2:
                    sprite = _halfOpenDoor2vertical;
                    break;
                case 0:
                    sprite = _halfOpenDoor3vertical;
                    break;
            }
        }
        else if (isOpen)
        {
            sprite = _openDoor1vertical;
                   
        }
        else
        {
            switch (iID%3)
            {
                case 1:
                    sprite = _closedDoor1vertical;
                    break;
                case 2:
                    sprite = _closedDoor2vertical;
                    break;
                case 0:
                    sprite = _closedDoor3vertical;
                    break;
            }
        }
        }else
            {
                if (isHalfOpen)
        {
            switch (iID%3)
            {
                case 1:
                    sprite = _halfOpenDoor1horizontal;
                    break;
                case 2:
                    sprite = _halfOpenDoor2horizontal;
                    break;
                case 0:
                    sprite = _halfOpenDoor3horizontal;
                    break;
            }
        }
        else if (isOpen)
        {
            
                    sprite = _openDoor1horizontal;
                
        }
        else
        {
            switch (iID%3)
            {
                case 1:
                    sprite = _closedDoor1horizontal;
                    break;
                case 2:
                    sprite = _closedDoor2horizontal;
                    break;
                case 0:
                    sprite = _closedDoor3horizontal;
                    break;
            }
        }
            }
        
        if( sprite == null)
        {
            print("sprite unset");
        }
        return sprite;
    }
}
