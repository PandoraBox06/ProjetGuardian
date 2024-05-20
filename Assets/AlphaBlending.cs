using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class AlphaBlending : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private float _speed = 2f;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _image = GetComponent<Image>();
    }

    public void StartFadeOutImage()
    {
        StartCoroutine(FadeOutImage());
    }

    public void StartFadeOutText()
    {
        StartCoroutine(FadeOutText());
    }
    public void StartFadeInImage()
    {
        StartCoroutine(FadeInImage());
    }

    public void StartFadeInText()
    {
        StartCoroutine(FadeInText());
    }
    //Fade Out
    private IEnumerator FadeOutImage()
    {
        Color startColor = _image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0); // Target color with alpha = 0

        float elapsedTime = 0.0f;

        while (elapsedTime < _speed)
        {
            elapsedTime += Time.deltaTime;
            _image.color = Color.Lerp(startColor, endColor, elapsedTime / _speed);
            yield return null; // Wait until the next frame
        }

        // Ensure the image is completely faded out
        _image.color = endColor;
        StopCoroutine(FadeOutImage());
    }
    private IEnumerator FadeOutText()
    {
        Color startColor = _text.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0); // Target color with alpha = 0

        float elapsedTime = 0.0f;

        while (elapsedTime < _speed)
        {
            elapsedTime += Time.deltaTime;
            _text.color = Color.Lerp(startColor, endColor, elapsedTime / _speed);
            yield return null; // Wait until the next frame
        }

        // Ensure the image is completely faded out
        _text.color = endColor;
        StopCoroutine(FadeOutText());
    }
    //Fade In
    private IEnumerator FadeInImage()
    {
        Color startColor = _image.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1); // Target color with alpha = 1

        float elapsedTime = 0.0f;

        while (elapsedTime < _speed)
        {
            elapsedTime += Time.deltaTime;
            _image.color = Color.Lerp(startColor, endColor, elapsedTime / _speed);
            yield return null; // Wait until the next frame
        }

        // Ensure the image is completely faded out
        _image.color = endColor;
        StopCoroutine(FadeOutImage());
    }
    private IEnumerator FadeInText()
    {
        Color startColor = _text.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1); // Target color with alpha = 1

        float elapsedTime = 0.0f;

        while (elapsedTime < _speed)
        {
            elapsedTime += Time.deltaTime;
            _text.color = Color.Lerp(startColor, endColor, elapsedTime / _speed);
            yield return null; // Wait until the next frame
        }

        // Ensure the image is completely faded out
        _text.color = endColor;
        StopCoroutine(FadeOutText());
    }
}

