using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject doorLeft;
    public GameObject doorRight;
    public GameObject doorUp;
    public GameObject doorDown;

    public bool roomLeft;
    public bool roomRight;
    public bool roomUp;
    public bool roomDown;

    [Header("Distance Settings")]
    public int stepToStart; // 시작점으로부터의 거리 (그리드 단위)

    [Header("Door Count")]
    public int doorNum; // 현재 방의 문 개수

    void Start()
    {
        doorLeft.SetActive(roomLeft);
        doorRight.SetActive(roomRight);
        doorUp.SetActive(roomUp);
        doorDown.SetActive(roomDown);
    }

    public void UpdateRoom(float xOffset, float yOffset)
    {
        // 시작점으로부터의 거리 계산
        stepToStart = (int)(Mathf.Abs(transform.position.x / xOffset) + (Mathf.Abs(transform.position.y) / yOffset));

        // 문 개수 세기
        doorNum = 0;
        if (roomUp)     doorNum++;
        if (roomDown)   doorNum++;
        if (roomLeft)   doorNum++;
        if (roomRight)  doorNum++;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
           // Debug.Log(other.gameObject.name);
            CameraContorller.instance.ChangeTarget(transform);
        }
    }
}
