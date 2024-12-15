using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager instance;

    [Header("Shake Settings")]
    [SerializeField] private float globalShakeForce = 1f; // 글로벌 카메라 흔들림 강도

    private void Awake()
    {
        instance = this;
    }

    public void CameraShake(CinemachineImpulseSource impulseSource)
    {
        // 글로벌 흔들림 강도를 사용하여 임펄스 생성
        impulseSource.GenerateImpulseWithForce(globalShakeForce);
    }
}
