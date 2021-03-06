﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShadowController : MonoBehaviour
{
    [SerializeField]
    private Image _shadowRenderer;

    [SerializeField]
    private Color _bgColor;

    [SerializeField]
    private AnimationCurve _fadeCurve;
    [SerializeField]
    private AnimationCurve _fadeCurve2;
    //for different resolution

    [SerializeField]
    private AnimationCurve _emrgingRadiusIncreaseCurve;
    private float timer=0f;
    [SerializeField]
    private float timerDuration;
    private bool _isMergingShaderComplete;
    private bool char1OnMergingStar,char2OnMergingStar;

    [SerializeField]
    private float separationRadius = 0.3f;
    [SerializeField]
    private float combinedRadius = 0.6f;

    private float fixedAspectRatio;

    private bool _beginExpanding;
    private bool _inExpanding;
    [SerializeField]
    private float _expandingDuration=5f;

    void Start()
    {
        fixedAspectRatio = Camera.main.aspect;
        _shadowRenderer.gameObject.SetActive(false);
    }
    
    void LateUpdate()
    {
        
        if (LevelCode.gameState == GameState.Starting)
        {
            _shadowRenderer.gameObject.SetActive(false);
        }
        if (LevelCode.gameState == GameState.InLevel)
        {
            if(!CameraControl.isBirdView)
            {
            _shadowRenderer.gameObject.SetActive(true);
        }
        else{
            _shadowRenderer.gameObject.SetActive(false);
        }
            switch (LevelCode.levelType)
            {
                case LevelType.Normal:
                    Normal();
                    break;
                case LevelType.Combined:
                    Combined();
                    break;
                case LevelType.Separation:
                    Separation();
                    break;
                case LevelType.Merging:
                    Separation();
                    break;
            }
        }
    
    }
    void Normal()
    {
        float aspectRatio = Camera.main.aspect;
        Vector2 vPos1 = Camera.main.WorldToViewportPoint(WorldManager.g.char1Entity.visPosition);
        vPos1.y *= 1 / aspectRatio;
        
        Vector2 vPos2 = Camera.main.WorldToViewportPoint(WorldManager.g.char2Entity.visPosition);
        vPos2.y *= 1 / aspectRatio;
    
        //float fScreenDistance = Vector2.Distance(vPos1, vPos2);
        if(WorldManager.g.isDistanceSet)
        {
        float fPhysicalDistance = WorldManager.g.fCharacterDistance;
        float radius;
        if (aspectRatio < 1.5f)
        {
            radius = _fadeCurve2.Evaluate(fPhysicalDistance);
        }
        else
        {
            radius = _fadeCurve2.Evaluate(fPhysicalDistance);
        }
        //print("aspectRatio " + aspectRatio + " distance " + fPhysicalDistance + " radius " + radius);
        SetShader(radius, vPos1, vPos2);
    }
    }
    void Combined()
    {
        float aspectRatio = Camera.main.aspect;
        Vector2 vPos1 = Camera.main.WorldToViewportPoint(WorldManager.g.charCombinedEntity.visPosition);
        vPos1.y *= 1 / aspectRatio;
        Vector2 vPos2 = Camera.main.WorldToViewportPoint(WorldManager.g.charCombinedEntity.visPosition);
        vPos2.y *= 1 / aspectRatio;
        SetShader(combinedRadius, vPos1, vPos2);
    }
    void Separation()
    {
        Vector2 vPos1 = new Vector2(0.25f, 0.5f);
        Vector2 vPos2 = new Vector2(0.75f, 0.5f);
        vPos1.y *= 1 / fixedAspectRatio;
        vPos2.y *= 1 / fixedAspectRatio;
        float radius = separationRadius;
        if (char1OnMergingStar && char2OnMergingStar && !_isMergingShaderComplete)
        {
            //print("merging shader");
            timer += Time.deltaTime;
            radius = _emrgingRadiusIncreaseCurve.Evaluate(timer / timerDuration);
            if(timer>=timerDuration)
            {
                //print("merging shader complete");
                timer=0f;
                Events.g.Raise(new MergingShaderCompleteEvent());
                _isMergingShaderComplete=true;
            }
        }
        SetShader(radius, vPos1, vPos2);
    }
    void SetShader(float radius, Vector2 vPos1, Vector2 vPos2)
    {
        _shadowRenderer.material.SetVector("_Position1", vPos1);
        _shadowRenderer.material.SetVector("_Position2", vPos2);

        _shadowRenderer.material.SetColor("_Color", _bgColor);

        if(_beginExpanding){
            if(!_inExpanding)
            {
                _inExpanding=true;
                StartCoroutine(ShaderExpanding(_expandingDuration,radius));
            }
        }
        else
        {

            _shadowRenderer.material.SetFloat("_Radius", radius);
        } 

    }
    IEnumerator ShaderExpanding(float waitTime,float radius)
    {
        float timer = 0f;
        float r;
        
        do{
            timer += Time.deltaTime;
            
            r = radius * (timer / waitTime);
            _shadowRenderer.material.SetFloat("_Radius", r);
            
            yield return null;
        }
        while (timer < waitTime);        
        _beginExpanding=false;
        _inExpanding=false;
    }

    void OnEnable()
    {
        Events.g.AddListener<MergingStarEvent>(OnMergingStar);
        Events.g.AddListener<FadingEvent>(BeginShaderExpanding);
    }

    void OnDisable()
    {
        Events.g.RemoveListener<MergingStarEvent>(OnMergingStar);
        Events.g.RemoveListener<FadingEvent>(BeginShaderExpanding);
    }
    void BeginShaderExpanding(FadingEvent e)
    {
        if(e.isFading && e.fadeIn)
        {
        _beginExpanding=true;
    }

    }

    void OnMergingStar(MergingStarEvent e)
    {
        if(e.isEntered)
        {
            if(e.CharacterID==1)
            {
                char1OnMergingStar=true;
            }
            else if(e.CharacterID==2)
            {
                char2OnMergingStar=true;
            }
        }
        else
        {
            if(e.CharacterID==1)
            {
                char1OnMergingStar=false;
            }
            else if(e.CharacterID==2)
            {
                char2OnMergingStar=false;
            }
        }

    }
}
