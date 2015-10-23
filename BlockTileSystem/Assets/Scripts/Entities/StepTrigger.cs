using UnityEngine;
using System.Collections;

public class StepTrigger : MonoBehaviour {
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
    AudioClip _audio;

    private WorldTrigger _worldTrigger;
    private bool isFunctioning = true;

    public void Cache () {
        _worldTrigger = GetComponent<WorldTrigger>();
    }
    void Awake () {
        Cache();
        _worldTrigger.triggerType = TriggerType.StepTrigger;
    }
    void LateUpdate () {
        if (!_worldTrigger.isSpriteSet) {
            _worldTrigger.SetVisual(GetSpriteByID());
        }
        
        if (!_worldTrigger.isMessageSent) {
            if (_worldTrigger.isSteppedOn) {
                TriggerSteppedOn();
                _worldTrigger.isMessageSent = true;
            }
        }
    }

    void OnEnable () {
        Events.g.AddListener<RestartEvent>(Reset);
    }

    void OnDisable () {
        Events.g.RemoveListener<RestartEvent>(Reset);
    }

    private void Reset (RestartEvent e) {
        Reset();
    }
    void Reset () {
        isTriggered = false;
        isFunctioning=true;
        _worldTrigger.ChangeVisual(GetSpriteByID ());
    }

    void TriggerSteppedOn () {
        if (isFunctioning) {
            isFunctioning = false;
            //texture change, sound
            isTriggered = true;
            AudioSource.PlayClipAtPoint(_audio, CameraControl.cameraLoc, LevelCode.audioVolume);
            Events.g.Raise(new StepTriggerEvent(triggerID: iID));
            _worldTrigger.ChangeVisual(GetSpriteByID());
            //_worldTrigger.DestroyVisual();
            //_worldTrigger.DeregisterMe();
            //Destroy(this);
        }
    }


    private Sprite GetSpriteByID () {
        Sprite sprite = new Sprite();
        if (!isTriggered) {
            switch (iID % 6) {
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
        } else {
            switch (iID % 6) {
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
        if (sprite == null) {
            print("sprite unset");
        }
        return sprite;
    }
}
