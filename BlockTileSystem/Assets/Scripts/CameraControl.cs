using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{

    bool isSingleCamera = true;
    bool isCombined;

    [SerializeField]
    GameObject _singleCameraObject;
    [SerializeField]
    GameObject _doubleCameraObject1;
    [SerializeField]
    GameObject _doubleCameraObject2;

    [SerializeField]
    bool isBirdView;
    [SerializeField]
    private float _fSingleCameraSize=5f;
    [SerializeField]
    private float _fDoubleCameraSize=4f;

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
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.B))
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
            if (isSingleCamera)
            {
                if (isCombined)
                {
                    Vector2 cameraLocation = WorldManager.g.CharCombined.visPosition * _tileSize;
                    _singleCameraObject.transform.position = new Vector3(cameraLocation.x, cameraLocation.y, -12f);
                    _singleCamera.orthographicSize = _fSingleCameraSize * _tileSize;
                }
                else
                {
                    Vector2 cameraLocation = (WorldManager.g.Char1.visPosition + WorldManager.g.Char2.visPosition)* _tileSize / 2f;
                    _singleCameraObject.transform.position = new Vector3(cameraLocation.x, cameraLocation.y, -12f);
                    float distance = Vector2.Distance(WorldManager.g.Char1.visPosition, WorldManager.g.Char2.visPosition);
                    _singleCamera.orthographicSize = Mathf.Max(distance, _fSingleCameraSize) * _tileSize;
                }
            }
            else
            {
                Vector2 cameraLocation1 = WorldManager.g.Char1.visPosition * _tileSize;
                Vector2 cameraLocation2 = WorldManager.g.Char2.visPosition * _tileSize;
                _doubleCameraObject1.transform.position = new Vector3(cameraLocation1.x, cameraLocation1.y, -12f);
                _doubleCameraObject2.transform.position = new Vector3(cameraLocation2.x, cameraLocation2.y, -12f);
                _doubleCamera1.orthographicSize = _fDoubleCameraSize * _tileSize;
                _doubleCamera2.orthographicSize = _fDoubleCameraSize * _tileSize;
            }
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
        if (e.iLevelType == 2 || e.iLevelType == 3)
        {
            isCombined = false;
            isSingleCamera = false;
        }
        else if (e.iLevelType == 4)
        {
            isSingleCamera = true;
            isCombined = true;
        }
        else
        {
            isSingleCamera = true;
            isCombined = false;
        }

        if (isSingleCamera)
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
