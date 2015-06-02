using UnityEngine;
using System.Collections;

public class LevelStar : MonoBehaviour
{
    private WorldTrigger _worldTrigger;

    [SerializeField]
    private Sprite _sprite;
    public void Cache()
    {
        _worldTrigger = GetComponent<WorldTrigger>();
    }
    void Awake()
    {
        Cache();
        _worldTrigger.triggerType = TriggerType.LevelStar;
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
                switch (_worldTrigger.iStepCharacterID)
                {
                    case 1:
                        Events.g.Raise(new LevelStarEvent(isEntered: true, CharacterID: 1));
                        break;
                    case 2:
                        Events.g.Raise(new LevelStarEvent(isEntered: true, CharacterID: 2));
                        break;
                    case 3:
                        Events.g.Raise(new LevelStarEvent(isEntered: true, CharacterID: 3));
                        break;
                }
                _worldTrigger.isMessageSent = true;
            }
            else
            {
                switch (_worldTrigger.iStepCharacterID)
                {
                    case 1:
                        Events.g.Raise(new LevelStarEvent(isEntered: false, CharacterID: 1));
                        break;
                    case 2:
                        Events.g.Raise(new LevelStarEvent(isEntered: false, CharacterID: 2));
                        break;
                    case 3:
                        Events.g.Raise(new LevelStarEvent(isEntered: false, CharacterID: 3));
                        break;
                }
                _worldTrigger.isMessageSent = true;
            }
        }
    }


}
