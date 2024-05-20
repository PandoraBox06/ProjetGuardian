using System;
using UnityEngine;


public class GrayscalePostProcess : MonoBehaviour
{
    private Material _material;
    [SerializeField] private Shader _shader;

    private void Start()
    {
        _material = new Material(_shader);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _material);
    }
}