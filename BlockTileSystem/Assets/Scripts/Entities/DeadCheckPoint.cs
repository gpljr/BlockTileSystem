using UnityEngine;
using System.Collections;

public class DeadCheckPoint : MonoBehaviour {
    [SerializeField]
    private Sprite _sprite;
    private WorldTrigger _worldTrigger;
    public void Cache () {
        _worldTrigger = GetComponent<WorldTrigger>();
    }
    void Awake () {
        Cache();
        _worldTrigger.triggerType = TriggerType.Null;
    }

    void Update () {
        if (!_worldTrigger.isSpriteSet) {
            _worldTrigger.SetVisual(_sprite);
        }
    }
}
