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
    private Sprite _sprite4;
    [SerializeField]
    private Sprite _sprite5;
    [SerializeField]
    private Sprite _sprite6;
    [SerializeField]
    private Sprite _sprite1Triggered;
    [SerializeField]
    private Sprite _sprite2Triggered;
    [SerializeField]
    private Sprite _sprite3Triggered;
    [SerializeField]
    private Sprite _sprite4Triggered;
    [SerializeField]
    private Sprite _sprite5Triggered;
    [SerializeField]
    private Sprite _sprite6Triggered;

    [SerializeField]
    AudioClip _audioOn;
    [SerializeField]
    AudioClip _audioOff;

    private WorldTrigger _worldTrigger;
    private bool isStayed;

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
                AudioSource.PlayClipAtPoint(_audioOn, _worldTrigger.Location.ToVector2(), LevelCode.audioVolume);
            }
            else
            {
                TriggerStayed(false);
                AudioSource.PlayClipAtPoint(_audioOff, _worldTrigger.Location.ToVector2(), LevelCode.audioVolume);
            }
            _worldTrigger.isMessageSent = true;
        }
    }
    void TriggerStayed(bool stayed)
    {
        isStayed=stayed;
        _worldTrigger.ChangeVisual(GetSpriteByID());
        
        Events.g.Raise(new StayTriggerEvent(isEntered: stayed, triggerID: iID));
    }
    private Sprite GetSpriteByID()
    {
        Sprite sprite = new Sprite();
        if(!isStayed)
        {
        switch (iID%6)
        {
            case 1:
                sprite = _sprite1;
                break;
            case 2:
                sprite = _sprite2;
                break;
            case 3:
                sprite = _sprite3;
                break;
            case 4:
                sprite = _sprite4;
                break;
            case 5:
                sprite = _sprite5;
                break;
            case 0:
                sprite = _sprite6;
                break;
        }
    }
    else
    {
        switch (iID%6)
        {
            case 1:
                sprite = _sprite1Triggered;
                break;
            case 2:
                sprite = _sprite2Triggered;
                break;
            case 3:
                sprite = _sprite3Triggered;
                break;
            case 4:
                sprite = _sprite4Triggered;
                break;
            case 5:
                sprite = _sprite5Triggered;
                break;
            case 0:
                sprite = _sprite6Triggered;
                break;
        }
    }
        if (sprite == null)
        {
            print("sprite unset");
        }
        return sprite;
    }
}
