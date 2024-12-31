using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneCameraShake : MonoBehaviour
{


    public CinemachineVirtualCamera cinemachineVirtualCamera;
    private float shakeTimer;
    // Start is called before the first frame update
    void Awake()
    {
    }

    public void ShakeCamera(float instensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = instensity;
        shakeTimer = time;
    }

    public void VNShakeCamera()
    {
        ShakeCamera(3f, 0.2f);
    }

    private void Update()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0f)
            {
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
          cinemachineVirtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0f;
            }
        }
    }
}
