using UnityEngine;
using System.Collections;

public class StepTrigger : MonoBehaviour
{
    //set in the XML
    public int iID;
    //to associate with the trigger

    public bool isTriggered;

    [SerializeField]
    private Sprite _sprite;

    private WorldTrigger _worldTrigger;
    public void Cache()
    {
        _worldTrigger = GetComponent<WorldTrigger>();
    }
    void Awake()
    {
        Cache();
        _worldTrigger.triggerType = TriggerType.StepTrigger;
    }
    void LateUpdate()
    {
        if (!_worldTrigger.isSpriteSet)
        {
            _worldTrigger.SetVisual(_sprite);
        }
        
        if (!_worldTrigger.isMessageSent)
        {
            if (_worldTrigger.isSteppedOn)
            {
                TriggerSteppedOn();
                _worldTrigger.isMessageSent = true;
            }
        }
    }
    void TriggerSteppedOn()
    {
    	//texture change, sound
    	isTriggered=true;
    	Events.g.Raise(new StepTriggerEvent(triggerID: iID));
        _worldTrigger.DestroyVisual();
    }
}
