using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeaController : MonoBehaviour
{
    MeshRenderer meshRenderer;
    [SerializeField] float scrollSpeed = .1f;
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }
    void Update()
    {
        //Change offset like waves.
        meshRenderer.material.mainTextureOffset += new Vector2 (scrollSpeed * Time.deltaTime, scrollSpeed * Time.deltaTime / 2);
    }
}
