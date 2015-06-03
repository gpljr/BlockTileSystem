using UnityEngine;
using System.Collections;

public class StayTrigger : MonoBehaviour
{

    //set in the XML
    public int iID;
    //to associate with the trigger

    [SerializeField]
    private Sprite _sprite;

    [SerializeField]
    AudioClip audio;

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
        if (!_worldTrigger.isSpriteSet)
        {
            _worldTrigger.SetVisual(_sprite);
        }

        if (!_worldTrigger.isMessageSent)
        {
            if (_worldTrigger.isSteppedOn)
            {
                TriggerStayed(true);
                AudioSource.PlayClipAtPoint(audio, _worldTrigger.Location.ToVector2(), LevelCode.audioVolume);
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
        
        Events.g.Raise(new StayTriggerEvent(isEntered: stayed, triggerID: iID));
    }
}
