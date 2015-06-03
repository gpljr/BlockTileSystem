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

    private WorldEntity _worldEntity;

    [SerializeField]
    private Sprite _closedDoor1;
    [SerializeField]
    private Sprite _halfOpenDoor1;
    [SerializeField]
    private Sprite _openDoor1;
    [SerializeField]
    private Sprite _closedDoor2;
    [SerializeField]
    private Sprite _halfOpenDoor2;
    [SerializeField]
    private Sprite _openDoor2;
    [SerializeField]
    private Sprite _closedDoor3;
    [SerializeField]
    private Sprite _halfOpenDoor3;
    [SerializeField]
    private Sprite _openDoor3;


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
        _worldEntity.CollidingType = EntityCollidingType.Empty;
        _worldEntity.ChangeVisual(GetSpriteByID());
        //play animation;
    }
    void HalfOpenDoor()
    {
        isHalfOpen = true;
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

        if (isHalfOpen)
        {
            switch (iID)
            {
                case 1:
                    sprite = _halfOpenDoor1;
                    break;
                case 2:
                    sprite = _halfOpenDoor2;
                    break;
                case 3:
                    sprite = _halfOpenDoor3;
                    break;
            }
        }
        else if (isOpen)
        {
            switch (iID)
            {
                case 1:
                    sprite = _openDoor1;
                    break;
                case 2:
                    sprite = _openDoor2;
                    break;
                case 3:
                    sprite = _openDoor3;
                    break;
            }
        }
        else
        {
            switch (iID)
            {
                case 1:
                    sprite = _closedDoor1;
                    break;
                case 2:
                    sprite = _closedDoor2;
                    break;
                case 3:
                    sprite = _closedDoor3;
                    break;
            }
        }
        
        
        return sprite;
    }
}
