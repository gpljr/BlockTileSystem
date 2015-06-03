using UnityEngine;
using System.Collections;

public class TutorialKey : MonoBehaviour
{
    public int iID;
    public bool isTriggered;

    [SerializeField]
    private Sprite _spriteW;
    [SerializeField]
    private Sprite _spriteA;
    [SerializeField]
    private Sprite _spriteS;
    [SerializeField]
    private Sprite _spriteD;
    [SerializeField]
    private Sprite _spriteUp;
    [SerializeField]
    private Sprite _spriteDown;
    [SerializeField]
    private Sprite _spriteLeft;
    [SerializeField]
    private Sprite _spriteRight;

    private WorldTrigger _worldTrigger;
    public void Cache()
    {
        _worldTrigger = GetComponent<WorldTrigger>();
    }
    void Awake()
    {
        Cache();
        _worldTrigger.triggerType = TriggerType.TutorialKey;
    }
    void LateUpdate()
    {
        if (!_worldTrigger.isSpriteSet && !isTriggered)
        {
            _worldTrigger.SetVisual(GetSpriteByID());
        }
        
        // if (!_worldTrigger.isMessageSent)
        // {
        //     if (_worldTrigger.isSteppedOn)
        //     {
        //         TriggerSteppedOn();
        //         _worldTrigger.isMessageSent = true;
        //     }
        // }
    }
    // void TriggerSteppedOn()
    // {
    //     isTriggered = true;
    //     _worldTrigger.DestroyVisual();
    // }


    private Sprite GetSpriteByID()
    {
        Sprite sprite = new Sprite();
        switch (iID)
        {
            case 1:
                sprite = _spriteW;
                break;
            case 2:
                sprite = _spriteA;
                break;
            case 3:
                sprite = _spriteS;
                break;
            case 4:
                sprite = _spriteD;
                break;
            case 5:
                sprite = _spriteUp;
                break;
            case 6:
                sprite = _spriteDown;
                break;
            case 7:
                sprite = _spriteLeft;
                break;
            case 8:
                sprite = _spriteRight;
                break;
        }
        return sprite;
    }
}
