using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSetup : MonoBehaviour
{
    public static RoomSetup instance;

    [Header("Objects")]
    public GameObject chestObject;
    public GameObject machineObject;
    public GameObject BulletMaxCollider;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        SetupRoom();
    }

    public void SetupRoom()
    {
        if (chestObject == null || machineObject == null)
        {
            Debug.LogError("Chest or Machine object is not assigned in the RoomSetup script!");
            return;
        }

        // 방 태그에 따라 보상 아이템 활성화
        if (gameObject.CompareTag("RewardRoom"))
        {
            chestObject.SetActive(true);
            machineObject.SetActive(true);
        }
        else
        {
            chestObject.SetActive(false);
            machineObject.SetActive(false);
        }
    }

    public void SetBulletMaxCollider(bool isEnable)
    {
        BulletMaxCollider.SetActive(isEnable);
    }
}
