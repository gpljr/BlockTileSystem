using UnityEngine;
using System.Collections;

public class StayTrigger : MonoBehaviour
{

    //set in the XML
    public int iID;
    //to associate with the trigger

    [SerializeField]
    private Sprite _sprite1;
    [SerializeField]
    private Sprite _sprite2;
    [SerializeField]
    private Sprite _sprite3;

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
            _worldTrigger.SetVisual(GetSpriteByID());
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
    private Sprite GetSpriteByID()
    {
        Sprite sprite = new Sprite();
        switch (iID % 3)
        {
            case 1:
                sprite = _sprite1;
                break;
            case 2:
                sprite = _sprite2;
                break;
            case 0:
                sprite = _sprite3;
                break;

        }
        if (sprite == null)
        {
            print("sprite unset");
        }
        return sprite;
    }
}
