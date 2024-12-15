using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraContorller : MonoBehaviour
{
    public static CameraContorller instance;

    [Header("Camera Movement Settings")]
    public float speed;

    public Transform target; // 카메라가 추적할 타겟

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        // 타겟이 존재하면 매 프레임 카메라를 타겟 위치로 부드럽게 이동
        if (target != null)
        {
            Vector3 targetPosition = new Vector3(target.position.x, target.position.y, transform.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }

    public void ChangeTarget(Transform newTarget)
    {
        if (target != newTarget)
            target = newTarget;
    }
}
