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
    private float separationRadius = 0.3f;
    [SerializeField]
    private float combinedRadius = 0.6f;

    private float fixedAspectRatio;

    void Start()
    {
        fixedAspectRatio = Camera.main.aspect;
        _shadowRenderer.gameObject.SetActive(false);
    }
    
    void LateUpdate()
    {
        if ( LevelCode.gameState == GameState.Starting)
        {
            _shadowRenderer.gameObject.SetActive(false);
        }
        if (LevelCode.gameState == GameState.InLevel)
        {
            _shadowRenderer.gameObject.SetActive(true);
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
        print("aspectRatio " + aspectRatio);
        Vector2 vPos1 = Camera.main.WorldToViewportPoint(WorldManager.g.char1Entity.visPosition);
        vPos1.y *= 1 / aspectRatio;
        print("vPos1 " + vPos1);
        Vector2 vPos2 = Camera.main.WorldToViewportPoint(WorldManager.g.char2Entity.visPosition);
        vPos2.y *= 1 / aspectRatio;
    
        float fDistance = Vector2.Distance(vPos1, vPos2);
        float radius = _fadeCurve.Evaluate(fDistance);
        SetShader(radius, vPos1, vPos2);
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
        SetShader(separationRadius, vPos1, vPos2);
    }
    void SetShader(float radius, Vector2 vPos1, Vector2 vPos2)
    {
        _shadowRenderer.material.SetVector("_Position1", vPos1);
        _shadowRenderer.material.SetVector("_Position2", vPos2);
        
        _shadowRenderer.material.SetFloat("_Radius", radius);

        _shadowRenderer.material.SetColor("_Color", _bgColor);
    }
}