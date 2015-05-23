using UnityEngine;
using System.Collections;

public class CameraControl : MonoBehaviour
{

    bool isSingleCamera = true;

    [SerializeField]
    GameObject _singleCameraObject;
    //GameObject _doubleCameraObject1, _doubleCameraObject2;

    [SerializeField]
    bool isBirdView;

    Camera _singleCamera;
    IntVector _dims;
    float _tileSize;

    void Awake()
    {
        _singleCamera = _singleCameraObject.GetComponent<Camera>();
        _tileSize = WorldManager.g.TileSize;
    }
    void Update()
    {
    	if(Input.GetKeyDown(KeyCode.B))
    	{
    		isBirdView=!isBirdView;
    	}
        if (isBirdView)
        {
            _dims = WorldManager.g.Dims;
            _singleCameraObject.transform.position = new Vector3(_dims.x * _tileSize / 2, _dims.y * _tileSize / 2, -12f);
            _singleCamera.orthographicSize = Mathf.Min(_dims.x, _dims.y) * _tileSize / 2 + 2;
        }
        else
        {
            Vector2 cameraLocation = (WorldManager.g.Char1.Location + WorldManager.g.Char2.Location).ToVector2() * _tileSize / 2f;
            _singleCameraObject.transform.position = new Vector3(cameraLocation.x, cameraLocation.y, -12f);
            float distance = Vector2.Distance(WorldManager.g.Char1.Location.ToVector2(), WorldManager.g.Char2.Location.ToVector2());
            _singleCamera.orthographicSize = Mathf.Max(distance, 6f) * _tileSize;
        }
    }
}
