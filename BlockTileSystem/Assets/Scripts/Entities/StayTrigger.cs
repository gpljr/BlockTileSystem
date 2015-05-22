using UnityEngine;
using System.Collections;

public class StayTrigger : MonoBehaviour {

	//set in the XML
    public int iID;
    //to associate with the trigger

    private WorldTrigger _worldTrigger;
    public void Cache()
    {
        _worldTrigger = GetComponent<WorldTrigger>();
    }
    void Awake()
    {
        Cache();
        _worldTrigger.triggerType = TriggerType.StayTrigger;
    }
    void LateUpdate()
    {
        if (!_worldTrigger.isMessageSent)
        {
            if (_worldTrigger.isSteppedOn)
            {
                TriggerStayed(true);
                
            }
            else
            {
            	TriggerStayed(false);
            }
            _worldTrigger.isMessageSent = true;
        }
    }
    void TriggerStayed(bool stayed)
    {
    	//texture change, sound
    	Events.g.Raise(new StayTriggerEvent(isEntered: stayed, triggerID: iID));
        _worldTrigger.isMessageSent=true;
    }
}
