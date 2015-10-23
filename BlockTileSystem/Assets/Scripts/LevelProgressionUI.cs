using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelProgressionUI : MonoBehaviour {

    Mesh _mesh;
    [SerializeField] Material _material;
    [SerializeField] int _levelCount = 1;
    CanvasRenderer _renderer;
    Vector3[] _vertices;
    [SerializeField] Texture _texture;
    [SerializeField] int _resolution = 100;
    [SerializeField] Text levelText;
    [SerializeField] Text levelCountText;

    void Awake () {
        _renderer = GetComponent<CanvasRenderer>();
    }

    void Start () {
        

        levelCountText.text = LevelCode.LevelCount.ToString();
        levelText.text="1";
    }
    void ShowProgressionCircle () {
        _mesh = new Mesh();
        _vertices = new Vector3[_levelCount * 3];
        _vertices[0] = new Vector3(0f, 0f, 0f) * 10f;
        _vertices[1] = new Vector3(0f, 1f, 0f) * 10f;
        _vertices[2] = new Vector3(-1f, 1f, 0f) * 10f;
        // _vertices[3] = new Vector3(-1f,0f,0f) * 10f;
        _mesh.vertices = _vertices;
        _mesh.uv = new Vector2[] { new Vector2(1 - 1f / _resolution, 1), new Vector2(1, 1), new Vector2(1, 1) };
        _mesh.triangles = new int[] { 0, 1, 2 };
        _renderer.SetMaterial(_material, _texture);
        _renderer.SetMesh(_mesh);
    }
	
    void Update () {
        
        
    }
    void OnEnable () {
        Events.g.AddListener<LevelLoadedEvent>(LevelLoaded);
    }
    
    void OnDisable () {
        Events.g.RemoveListener<LevelLoadedEvent>(LevelLoaded);
    }
    void LevelLoaded (LevelLoadedEvent e) {
        levelText.text = LevelCode.CurrentLevel.ToString();
    }
}
