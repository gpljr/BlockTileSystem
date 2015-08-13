using UnityEngine;
using System.Collections;

public class LevelStar : MonoBehaviour
{
    private WorldTrigger _worldTrigger;

    [SerializeField]
    private Sprite _sprite;
    [SerializeField]
    private Sprite _spriteStepOn;

    [SerializeField]
    AudioClip _audio;
    
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
            if (LevelCode.levelType == LevelType.Merging)
            {
                MergingStar();
            }

            if (_worldTrigger.isSteppedOn)
            {
                AudioSource.PlayClipAtPoint (_audio, _worldTrigger.Location.ToVector2(), LevelCode.audioVolume);
                _worldTrigger.ChangeVisual(_spriteStepOn);
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
                    
            }
            else
            {
                _worldTrigger.ChangeVisual(_sprite);
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

            }
            
            _worldTrigger.isMessageSent = true;
        }
    }
    void MergingStar()
    {
        if (_worldTrigger.isSteppedOn)
            {
                switch (_worldTrigger.iStepCharacterID)
                {
                    case 1:
                        Events.g.Raise(new MergingStarEvent(isEntered: true, CharacterID: 1));
                        break;
                    case 2:
                        Events.g.Raise(new MergingStarEvent(isEntered: true, CharacterID: 2));
                        break;
                    case 3:
                        Events.g.Raise(new MergingStarEvent(isEntered: true, CharacterID: 3));
                        break;
                }
                    
            }
            else
            {
                switch (_worldTrigger.iStepCharacterID)
                {
                    case 1:
                        Events.g.Raise(new MergingStarEvent(isEntered: false, CharacterID: 1));
                        break;
                    case 2:
                        Events.g.Raise(new MergingStarEvent(isEntered: false, CharacterID: 2));
                        break;
                    case 3:
                        Events.g.Raise(new MergingStarEvent(isEntered: false, CharacterID: 3));
                        break;
                }

            }
    }


}
