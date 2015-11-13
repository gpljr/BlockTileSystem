using UnityEngine;
using System.Collections;

public class WorldTrigger : MonoBehaviour
{

    [HideInInspector]
    public bool isSteppedOn;
    [HideInInspector]
    public bool isMessageSent = true;
    [HideInInspector]
    public EntityType steppingEntityType;
    public int iStepCharacterID;

    public TriggerType triggerType;

    [SerializeField]
    IntVector _location;

    public IntVector Location
    {
        get { return _location; }
        set { _location = value; }
    }

    [SerializeField]
    private GameObject _visualPrefab;
    private Transform _visuals;
    public Vector2 visPosition
    {
        get
        {
            return _visuals.position;
        }
        set
        {
            _visuals.position = value;
        }
    }

    private bool _registered = false;

    public bool isSpriteSet;
    public void SetVisual(Sprite sprite)
    {
        _visuals = Instantiate(_visualPrefab).transform;
        _visuals.position = (_location.ToVector2() + new Vector2(0.5f, -0.5f)) * WorldManager.g.TileSize;
        _visuals.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
        isSpriteSet = true;
    }
    public void ChangeVisual(Sprite sprite)
    {
        _visuals.gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }
    public void DestroyVisual()
    {
        if (isSpriteSet)
        {
            if (_visuals != null)
            {
                Destroy(_visuals.gameObject);
            }
            isSpriteSet = false;
        }
    }

    public void RegisterMe()
    {
        if (!_registered)
        {
            WorldManager.g.RegisterTrigger(this);
            _registered = true;
        }
    }

    public void DeregisterMe()
    {
        if (_registered)
        {
            WorldManager.g.RegisterTrigger(this);
            _registered = false;
        }
    }
    void OnEnable()
    {
        // Events.g.AddListener<LevelLoadedEvent>(Reset);
        Events.g.AddListener<RestartEvent>(Reset);
    }
    
    void OnDisable()
    {
        // Events.g.RemoveListener<LevelLoadedEvent>(Reset);
        Events.g.RemoveListener<RestartEvent>(Reset);
        DestroyVisual();
    }
    // private void Reset(LevelLoadedEvent e)
    // {
    //     Reset();
    // }
    private void Reset(RestartEvent e)
    {
        Reset();
    }
    void Reset()
    {
        isSteppedOn = false;
        isMessageSent = false;
        steppingEntityType=EntityType.Null;
    }

    void Start()
    {
        RegisterMe();
    }

    public void SteppedOn(WorldEntity e)
    {
        isSteppedOn = true;
        if(e != null)
        {
            steppingEntityType = e.entityType;
            iStepCharacterID = e.characterID;
        }
        isMessageSent = false;
    }
    public void SteppedOut(WorldEntity e)
    {
        isSteppedOn = false;
        if(e != null)
        {
            steppingEntityType = e.entityType;
            iStepCharacterID = e.characterID;
        }
        isMessageSent = false;
    }


}