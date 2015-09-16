using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{
    bool isCombined;
    [SerializeField]
    GameObject _singleCameraObject;
    [SerializeField]
    GameObject _doubleCameraObject1;
    [SerializeField]
    GameObject _doubleCameraObject2;

    [SerializeField]
    bool isDevMode;
    public static bool isBirdView;
    [SerializeField]
    private float _fSingleCameraSize = 5f;
    [SerializeField]
    private float _fDoubleCameraSize = 4f;


    Camera _singleCamera, _doubleCamera1, _doubleCamera2;
    IntVector _dims;
    float _tileSize;

    void Awake()
    {
        _singleCamera = _singleCameraObject.GetComponent<Camera>();
        _doubleCamera1 = _doubleCameraObject1.GetComponent<Camera>();
        _doubleCamera2 = _doubleCameraObject2.GetComponent<Camera>();
        _tileSize = WorldManager.g.TileSize;
        _singleCameraObject.SetActive(true);
        _doubleCameraObject1.SetActive(false);
        _doubleCameraObject2.SetActive(false);
    }
    void LateUpdate()
    {
        if (LevelCode.gameState == GameState.InLevel)
        {
            if (isDevMode && Input.GetKeyDown(KeyCode.Space))
            {
                isBirdView = !isBirdView;
            }
            if (isBirdView)
            {
                _dims = WorldManager.g.Dims;
                _singleCameraObject.transform.position = new Vector3(_dims.x * _tileSize / 2, _dims.y * _tileSize / 2, -12f);
                _singleCamera.orthographicSize = Mathf.Min(_dims.x, _dims.y) * _tileSize / 2 + 2;
            }
            else
            {
                
                Vector2 cameraLocation;
                switch (LevelCode.levelType)
                {
                    case LevelType.Normal:
                        if (WorldManager.g.char1Entity != null && WorldManager.g.char2Entity != null)
                        {
                            cameraLocation = (WorldManager.g.char1Entity.visPosition + WorldManager.g.char2Entity.visPosition) * _tileSize / 2f;
                            _singleCameraObject.transform.position = new Vector3(cameraLocation.x, cameraLocation.y, -12f);
                            float distance = Vector2.Distance(WorldManager.g.char1Entity.visPosition, WorldManager.g.char2Entity.visPosition);
                            _singleCamera.orthographicSize = Mathf.Max(distance, _fSingleCameraSize) * _tileSize;
                        }
                        break;
                    case LevelType.Combined:
                        if (WorldManager.g.charCombinedEntity != null)
                        {
                            cameraLocation = WorldManager.g.charCombinedEntity.visPosition * _tileSize;
                            _singleCameraObject.transform.position = new Vector3(cameraLocation.x, cameraLocation.y, -12f);
                            _singleCamera.orthographicSize = _fSingleCameraSize * _tileSize;
                        }
                        break;
                    case LevelType.Separation: 
                        CameraSeparation();
                        break;
                    case LevelType.Merging:
                        CameraSeparation();
                        break;
                }
                
                

            }
        }
    }
    void CameraSeparation()
    {
        if (WorldManager.g.char1Entity != null && WorldManager.g.char2Entity != null)
        {
            Vector2 cameraLocation1 = WorldManager.g.char1Entity.visPosition * _tileSize;
            Vector2 cameraLocation2 = WorldManager.g.char2Entity.visPosition * _tileSize;
            _doubleCameraObject1.transform.position = new Vector3(cameraLocation1.x, cameraLocation1.y, -12f);
            _doubleCameraObject2.transform.position = new Vector3(cameraLocation2.x, cameraLocation2.y, -12f);
            _doubleCamera1.orthographicSize = _fDoubleCameraSize * _tileSize;
            _doubleCamera2.orthographicSize = _fDoubleCameraSize * _tileSize;
        }
    }
    void OnEnable()
    {
        Events.g.AddListener<LevelLoadedEvent>(LevelLoaded);
    }
    
    void OnDisable()
    {
        Events.g.RemoveListener<LevelLoadedEvent>(LevelLoaded);
    }
    void LevelLoaded(LevelLoadedEvent e)
    {
        if (LevelCode.levelType == LevelType.Combined || LevelCode.levelType == LevelType.Normal)
        {
            _singleCameraObject.SetActive(true);
            _doubleCameraObject1.SetActive(false);
            _doubleCameraObject2.SetActive(false);
        }
        else
        {
            _singleCameraObject.SetActive(false);
            _doubleCameraObject1.SetActive(true);
            _doubleCameraObject2.SetActive(true);
        }
    }
}
