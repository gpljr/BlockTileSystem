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
        //play animation, change texture;
    }
    void HalfOpenDoor()
    {
        isHalfOpen = true;
        //play animation, change texture;
    }
    void Triggered(StepTriggerEvent e)
    {
        if (e.triggerID == iID)
        {
            if (iTriggerNumber == 1)
            {
            	isTriggered1=true;
            }
            else if (iTriggerNumber == 2)
            {
            	if(isTriggered1)
            	{
            		isTriggered2=true;
            	}
            	else{
            		isTriggered1=true;
            	}
            }
        }
    }
}
