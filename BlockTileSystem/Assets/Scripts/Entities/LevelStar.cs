using UnityEngine;
using System.Collections;

public class LevelStar : MonoBehaviour
{
    private WorldTrigger _worldTrigger;

    [SerializeField]
    Texture _starTexture;
    public void Cache()
    {
        _worldTrigger = GetComponent<WorldTrigger>();
    }
    void Awake()
    {
        Cache();
        _worldTrigger.triggerType= TriggerType.LevelStar;
    }
    
    void LateUpdate()
    {
        if (!_worldTrigger.isMessageSent)
        {
            if (_worldTrigger.isSteppedOn)
            {
                switch (_worldTrigger.steppingEntityType)
                {
                    case EntityType.Character1:
                        //print("enter 1");
                        Events.g.Raise(new LevelStarEvent(isEntered: true, CharacterID: 1));
                        break;
                    case EntityType.Character2:
                        //print("enter 2");
                        Events.g.Raise(new LevelStarEvent(isEntered: true, CharacterID: 2));
                        break;
                }
                _worldTrigger.isMessageSent = true;
            }
            else
            {
                switch (_worldTrigger.steppingEntityType)
                {
                    case EntityType.Character1:
                    //print("leave 1");
                        Events.g.Raise(new LevelStarEvent(isEntered: false, CharacterID: 1));
                        break;
                    case EntityType.Character2:
                    //print("leave 2");
                        Events.g.Raise(new LevelStarEvent(isEntered: false, CharacterID: 2));
                        break;
                }
                _worldTrigger.isMessageSent = true;
            }
        }
    }
    // void OnDrawGizmos()
    // {
    //     if (_worldTrigger != null)
    //     {
    //         IntVector l = _worldTrigger.Location;
    //         Rect rect = new Rect(l.ToVector2().x, l.ToVector2().y, 1, 1);                
    //         Gizmos.DrawGUITexture(rect, _starTexture);
    //     }
    // }

}
