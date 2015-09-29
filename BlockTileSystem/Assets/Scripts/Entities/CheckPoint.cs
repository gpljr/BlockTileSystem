using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour
{


    [SerializeField]
    private Sprite _sprite;
    [SerializeField] Sprite _spriteStepOn;
    [SerializeField] GameObject deadCheckPointPrefab;

    public int iID;

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
        var deadCheckObject = Instantiate(deadCheckPointPrefab);
        deadCheckObject.GetComponent<WorldTrigger>().Location=_worldTrigger.Location;
        _worldTrigger.ChangeVisual(_sprite);
    }
    void LateUpdate()
    {
        if (!_worldTrigger.isSpriteSet)
        {
            _worldTrigger.SetVisual(_sprite);
        }
        _worldTrigger.visPosition=(_worldTrigger.Location.ToVector2() 
            + new Vector2(0.5f, -0.5f)) * WorldManager.g.TileSize;
        

        if (!_worldTrigger.isMessageSent && _worldTrigger.steppingEntityType == EntityType.Character)
        {
            if (_worldTrigger.isSteppedOn)
            {
                _worldTrigger.ChangeVisual(_spriteStepOn);
                switch (_worldTrigger.iStepCharacterID)
                {
                    case 1:
                        Events.g.Raise(new CheckPointEvent(isEntered: true, CharacterID: 1,iCheckPointID:iID));
                        break;
                    case 2:
                        Events.g.Raise(new CheckPointEvent(isEntered: true, CharacterID: 2,iCheckPointID:iID));
                        break;
                    case 3:
                        Events.g.Raise(new CheckPointEvent(isEntered: true, CharacterID: 3,iCheckPointID:iID));
                        break;
                }
                _worldTrigger.isMessageSent = true;
            }
            else
            {
                _worldTrigger.ChangeVisual(_sprite);
                switch (_worldTrigger.iStepCharacterID)
                {

                    case 1:
                        Events.g.Raise(new CheckPointEvent(isEntered: false, CharacterID: 1,iCheckPointID:iID));
                        break;
                    case 2:
                        Events.g.Raise(new CheckPointEvent(isEntered: false, CharacterID: 2,iCheckPointID:iID));
                        break;
                    case 3:
                        Events.g.Raise(new CheckPointEvent(isEntered: false, CharacterID: 3,iCheckPointID:iID));
                        break;
                }
                _worldTrigger.isMessageSent = true;
            }
        }
    }
    
}
