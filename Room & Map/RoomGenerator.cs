using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomGenerator : MonoBehaviour
{
    public static RoomGenerator instance;

    // 방 생성 방향 열거형 정의
    public enum Direction { up, down, left, right };
    public Direction direction;

    [Header("Room Information")]
    public GameObject roomPrefab;
    public int roomNumber;
    private GameObject endRoom;

    [Header("Position Control")]
    public Transform generatorPoint;
    public float xOffset;
    public float yOffset;
    public LayerMask roomLayer;
    public int maxStep; // 시작점으로부터 가장 먼 방까지의 최대 거리(스텝)
    public List<Room> rooms = new List<Room>(); // 생성된 모든 방 리스트

    List<GameObject> farRooms = new List<GameObject>();     // 가장 멀리 있는 방 리스트
    List<GameObject> lessFarRooms = new List<GameObject>(); // 그 다음으로 멀리 있는 방 리스트
    List<GameObject> oneWayRooms = new List<GameObject>();  // 문이 하나뿐인 방 리스트

    public WallType wallType; // 벽 타입(문 조합에 따른 프리팹 모음)

    public int rewardRoomCount = 0; // 생성된 보상 방 개수
    private int maxRewardRooms = 3;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        string previousRoomTag = "StartRoom";

        // 설정된 개수만큼 방 생성
        for (int i = 0; i < roomNumber; i++)
        {
            // 새로운 방 생성 후 리스트에 추가
            var newRoom = Instantiate(roomPrefab, generatorPoint.position, Quaternion.identity).GetComponent<Room>();
            rooms.Add(newRoom);

            if (i == 0)
            {
                newRoom.gameObject.tag = "StartRoom";
            }
            else
            {
                // 이전 방 태그와 현재 보상 방 생성 수를 참고해 보상 또는 적 방을 결정
                if (rewardRoomCount < maxRewardRooms)
                {
                    // 이전 방이 적 방일 경우 일정 확률로 보상 방 생성, 아니면 적 방 생성
                    if (Random.value <= 0.6f && previousRoomTag == "EnemyRoom")
                    {
                        newRoom.gameObject.tag = "RewardRoom";
                        rewardRoomCount++; 
                    }
                    else                    
                        newRoom.gameObject.tag = "EnemyRoom";
                }
                else // 보상 방 한도에 도달하면 적 방 생성
                    newRoom.gameObject.tag = "EnemyRoom";
            }

            previousRoomTag = newRoom.gameObject.tag; // 이전 방 태그 업데이트

            RoomSetup.instance.SetupRoom();
            ChangePointPos(); // 생성 포인트를 다음 위치로 이동
        }

        endRoom = rooms[0].gameObject; // 첫 번째 생성된 방을 기본 최종 방으로 설정

        foreach (var room in rooms) // 각 방의 문 조합에 따른 최종 셋업 진행
        {
            SetupRoom(room, room.transform.position);
        }

        // 최종 방을 찾고 태그 설정
        FindEndRoom();
    }

    public void ChangePointPos()
    {
        // 랜덤 방향(위, 아래, 왼쪽, 오른쪽)으로 생성 포인트 이동
        do
        {
            direction = (Direction)Random.Range(0, 4);

            switch (direction)
            {
                case Direction.up:
                    generatorPoint.position += new Vector3(0, yOffset, 0);
                    break;

                case Direction.down:
                    generatorPoint.position += new Vector3(0, -yOffset, 0);
                    break;

                case Direction.left:
                    generatorPoint.position += new Vector3(-xOffset, 0, 0);
                    break;

                case Direction.right:
                    generatorPoint.position += new Vector3(xOffset, 0, 0);
                    break;
            }
            // 새 위치에 이미 다른 방이 없도록 체크
        } while (Physics2D.OverlapCircle(generatorPoint.position, .2f, roomLayer));
    }

    public void SetupRoom(Room newRoom, Vector3 roomPosition)
    {
        // 인접 위치에 다른 방이 있는지 체크하여 문의 개방 방향 결정
        newRoom.roomUp = Physics2D.OverlapCircle(roomPosition + new Vector3(0, yOffset, 0), .2f, roomLayer);
        newRoom.roomDown = Physics2D.OverlapCircle(roomPosition + new Vector3(0, -yOffset, 0), .2f, roomLayer);
        newRoom.roomLeft = Physics2D.OverlapCircle(roomPosition + new Vector3(-xOffset, 0, 0), .2f, roomLayer);
        newRoom.roomRight = Physics2D.OverlapCircle(roomPosition + new Vector3(xOffset, 0, 0), .2f, roomLayer);

        // 방 정보 업데이트(시작점으로부터의 거리, 문의 수 등)
        newRoom.UpdateRoom(xOffset, yOffset);

        //문의 수에 따라 적절한 벽 프리팹 생성
        switch (newRoom.doorNum)
        {
            case 1:
                if (newRoom.roomUp)
                    Instantiate(wallType.singleUp, roomPosition, Quaternion.identity);
                if (newRoom.roomDown)
                    Instantiate(wallType.singleBottom, roomPosition, Quaternion.identity);
                if (newRoom.roomLeft)
                    Instantiate(wallType.singleLeft, roomPosition, Quaternion.identity);
                if (newRoom.roomRight)
                    Instantiate(wallType.singleRight, roomPosition, Quaternion.identity);
                break;

            case 2:
                if (newRoom.roomLeft && newRoom.roomUp)
                    Instantiate(wallType.doubleLU, roomPosition, Quaternion.identity);
                if (newRoom.roomLeft && newRoom.roomRight)
                    Instantiate(wallType.doubleLR, roomPosition, Quaternion.identity);
                if (newRoom.roomLeft && newRoom.roomDown)
                    Instantiate(wallType.doubleLB, roomPosition, Quaternion.identity);
                if (newRoom.roomUp && newRoom.roomRight)
                    Instantiate(wallType.doubleUR, roomPosition, Quaternion.identity);
                if (newRoom.roomUp && newRoom.roomDown)
                    Instantiate(wallType.doubleUB, roomPosition, Quaternion.identity);
                if (newRoom.roomRight && newRoom.roomDown)
                    Instantiate(wallType.doubleRB, roomPosition, Quaternion.identity);
                break;

            case 3:
                if (!newRoom.roomUp)
                    Instantiate(wallType.tripleLRB, roomPosition, Quaternion.identity);
                if (!newRoom.roomDown)
                    Instantiate(wallType.tripleLUR, roomPosition, Quaternion.identity);
                if (!newRoom.roomLeft)
                    Instantiate(wallType.tripleURB, roomPosition, Quaternion.identity);
                if (!newRoom.roomRight)
                    Instantiate(wallType.tripleLUB, roomPosition, Quaternion.identity);
                break;

            default:
                Instantiate(wallType.fourDoors, roomPosition, Quaternion.identity);
                break;
        }
    }

    public void FindEndRoom()
    {
        // 가장 먼 방까지의 최대 스텝 찾기
        for (int i = 0; i < rooms.Count; i++)
        {
            if (rooms[i].stepToStart > maxStep)           
                maxStep = rooms[i].stepToStart;            
        }

        // 최대 거리와 그 바로 아래 거리값을 가지는 방들을 구분
        foreach (var room in rooms)
        {
            if (room.stepToStart == maxStep)
                farRooms.Add(room.gameObject);
            if (room.stepToStart == maxStep - 1)
                lessFarRooms.Add(room.gameObject);
        }

        // 문의 수가 하나인 방들을 추출(종점 후보)
        for (int i = 0; i < farRooms.Count; i++)
        {
            if (farRooms[i].GetComponent<Room>().doorNum == 1)            
                oneWayRooms.Add(farRooms[i]);            
        }
        for (int i = 0; i < lessFarRooms.Count; i++)
        {
            if (lessFarRooms[i].GetComponent<Room>().doorNum == 1)
                oneWayRooms.Add(lessFarRooms[i]);
        }

        // 우선 단일 문의 방에서 최종 방 선택, 아니면 가장 먼 방 중 랜덤 선택
        if (oneWayRooms.Count != 0)
        {
            endRoom = oneWayRooms[Random.Range(0, oneWayRooms.Count)];
        }
        else
        {
            endRoom = farRooms[Random.Range(0, farRooms.Count)];
        }

        endRoom.tag = "FinalRoom";
    }
}

[System.Serializable]
public class WallType
{
    // 문의 개수 조합에 따른 벽(문) 프리팹들
    public GameObject singleLeft, singleRight, singleUp, singleBottom,
                      doubleLU, doubleLR, doubleLB, doubleUR, doubleUB, doubleRB,
                      tripleLUR, tripleLUB, tripleURB, tripleLRB,
                      fourDoors;
}