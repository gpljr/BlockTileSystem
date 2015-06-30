using UnityEngine;
using System.Collections;

public class StepTrigger : MonoBehaviour
{
    //set in the XML
    public int iID;
    //to associate with the trigger

    public bool isTriggered;

    [SerializeField]
    private Sprite _sprite1;
    [SerializeField]
    private Sprite _sprite2;
    [SerializeField]
    private Sprite _sprite3;

    [SerializeField]
    AudioClip _audio;

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
        if (!_worldTrigger.isSpriteSet && !isTriggered)
        {
            _worldTrigger.SetVisual(GetSpriteByID());
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
        isTriggered = true;
        AudioSource.PlayClipAtPoint(_audio, _worldTrigger.Location.ToVector2(), LevelCode.audioVolume);
        Events.g.Raise(new StepTriggerEvent(triggerID: iID));
        _worldTrigger.DestroyVisual();
        _worldTrigger.DeregisterMe();
        Destroy(this);
    }


    private Sprite GetSpriteByID()
    {
        Sprite sprite = new Sprite();
        switch (iID%3)
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
        if( sprite == null)
        {
            print("sprite unset");
        }
        return sprite;
    }
}
