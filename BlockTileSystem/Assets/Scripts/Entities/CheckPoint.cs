using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour {


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
        _worldTrigger.triggerType = TriggerType.CheckPoint;
    }
    public void MoveCheckPoint()
    {
        
    }
    void LateUpdate()
    {
        if (!_worldTrigger.isSpriteSet)
        {
            _worldTrigger.SetVisual(_sprite);
        }

        if (!_worldTrigger.isMessageSent &&_worldTrigger.steppingEntityType==EntityType.Character)
        {
            if (_worldTrigger.isSteppedOn)
            {
                switch (_worldTrigger.iStepCharacterID)
                {
                    case 1:
                        Events.g.Raise(new CheckPointEvent(isEntered: true, CharacterID: 1));
                        break;
                    case 2:
                        Events.g.Raise(new CheckPointEvent(isEntered: true, CharacterID: 2));
                        break;
                        case 3:
                        Events.g.Raise(new CheckPointEvent(isEntered: true, CharacterID: 3));
                        break;
                }
                _worldTrigger.isMessageSent = true;
            }
            else
            {
                switch (_worldTrigger.iStepCharacterID)
                {
                    case 1:
                        Events.g.Raise(new CheckPointEvent(isEntered: false, CharacterID: 1));
                        break;
                    case 2:
                        Events.g.Raise(new CheckPointEvent(isEntered: false, CharacterID: 2));
                        break;
                        case 3:
                        Events.g.Raise(new CheckPointEvent(isEntered: false, CharacterID: 3));
                        break;
                }
                _worldTrigger.isMessageSent = true;
            }
        }
    }
    
}
