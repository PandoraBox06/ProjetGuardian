using System;
using Cinemachine;
using UnityEngine;


public class CinemachineShake : MonoBehaviour
{
    public static CinemachineShake Instance { get; private set; }

    private CinemachineFreeLook _cinemachineFreeLook;
    private float shakeTimer;
    private float shakeTimerTotal;
    private float startingIntensity;

    private void Awake()
    {
        Instance = this;
        _cinemachineFreeLook = GetComponent<CinemachineFreeLook>();
    }

    public void ShakeCamera(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = _cinemachineFreeLook.GetRig(0)
            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin1 = _cinemachineFreeLook.GetRig(1)
            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin2 = _cinemachineFreeLook.GetRig(2)
            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;
        cinemachineBasicMultiChannelPerlin1.m_AmplitudeGain = intensity;
        cinemachineBasicMultiChannelPerlin2.m_AmplitudeGain = intensity;
        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }

    private void Update()
    {
        if (!(shakeTimer > 0)) return;
        shakeTimer -= Time.deltaTime;
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = _cinemachineFreeLook.GetRig(0)
            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin1 = _cinemachineFreeLook.GetRig(1)
            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin2 = _cinemachineFreeLook.GetRig(2)
            .GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain =
            Mathf.Lerp(startingIntensity, 0f, (1 - (shakeTimer / shakeTimerTotal)));
        cinemachineBasicMultiChannelPerlin1.m_AmplitudeGain =
            Mathf.Lerp(startingIntensity, 0f, (1 - (shakeTimer / shakeTimerTotal)));
        cinemachineBasicMultiChannelPerlin2.m_AmplitudeGain =
            Mathf.Lerp(startingIntensity, 0f, (1 - (shakeTimer / shakeTimerTotal)));
    }
}
