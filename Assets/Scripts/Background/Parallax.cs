using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [Range(0, 0.5f)]
    [SerializeField] private float _speed = .2f;
    public float Speed => _speed;

    private Material _material;
    private float _distance;

    // Start is called before the first frame update
    void Start()
    {
        _material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        _distance += Time.deltaTime * _speed;
        _material.SetTextureOffset("_MainTex", Vector2.right * _distance);
    }
}
