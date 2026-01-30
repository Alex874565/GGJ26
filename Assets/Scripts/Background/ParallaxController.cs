using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    [Range(0f, .05f)]
    [SerializeField] private float _parallaxSpeed;

    private Transform _camera;
    private Vector3 _cameraStartPos;
    private float _distance;

    private int _backgroundsCount;
    private List<GameObject> _backgrounds;
    private List<Material> _materials;
    private List<float> _backgroundSpeeds;

    private float _farthestBackground;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main.transform;
        _cameraStartPos = _camera.position;

        _backgroundsCount = transform.childCount;
        _backgrounds = new List<GameObject>();
        _materials = new List<Material>();
        _backgroundSpeeds = new List<float>();

        for(int i = 0; i < _backgroundsCount; i++)
        {
            _backgrounds.Add(transform.GetChild(i).gameObject);
            _materials.Add(_backgrounds[i].GetComponent<Renderer>().material);
        }

        CalculateBackgroundSpeeds();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        _distance = _camera.position.x - _cameraStartPos.x;

        transform.position = new Vector3(_camera.position.x, transform.position.y, 0);

        for (int i = 0; i < _backgroundsCount; i++)
        {
            _materials[i].SetTextureOffset("_MainTex", Vector2.right * _distance * _parallaxSpeed * _backgroundSpeeds[i]);
        }
    }

    private void CalculateBackgroundSpeeds()
    {
        foreach (GameObject background in _backgrounds)
        {
            if(background.transform.position.z - _camera.position.z > _farthestBackground)
            {
                _farthestBackground = background.transform.position.z - _camera.position.z;
            }
        }

        for (int i = 0; i < _backgroundsCount; i++)
        {
            _backgroundSpeeds.Add(1 - (_backgrounds[i].transform.position.z - _camera.position.z) / _farthestBackground);
        }
    }
}
