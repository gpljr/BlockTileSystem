using UnityEngine;
using System.Collections;

public class WorldTrigger : MonoBehaviour
{

    [HideInInspector]
    public bool isSteppedOn;
    [HideInInspector]
    public bool isMessageSent =true;
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

    private bool _registered = false;

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
        Events.g.AddListener<LevelLoadedEvent>(Reset);
    }
    
    void OnDisable()
    {
        Events.g.RemoveListener<LevelLoadedEvent>(Reset);
    }
    private void Reset(LevelLoadedEvent e)
    {
        isSteppedOn = false;
        isMessageSent = true;
    }

    void Start()
    {
        RegisterMe();
    }

    public void SteppedOn(WorldEntity e)
    {
        isSteppedOn = true;
        steppingEntityType = e.entityType;
        iStepCharacterID = e.characterID;
        isMessageSent=false;
    }
    public void SteppedOut(WorldEntity e)
    {
        isSteppedOn = false;
        steppingEntityType = e.entityType;
        iStepCharacterID = e.characterID;
        isMessageSent= false;
    }


}