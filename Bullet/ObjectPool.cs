using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private static ObjectPool instance; 

    private Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>(); // 오브젝트 풀 딕셔너리
    private HashSet<GameObject> pooledObjects = new HashSet<GameObject>(); // 오브젝트 풀에 이미 들어간 객체를 기록
    private GameObject pool; 

    // 기존 객체 풀을 저장하기 위한 정적 필드
    private static Dictionary<string, Queue<GameObject>> existingPool = new Dictionary<string, Queue<GameObject>>();

    public static ObjectPool Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ObjectPool();
            }
            return instance;
        }
    }

    private ObjectPool()
    {
        // 생성자에서 저장된 객체 풀이 있다면 현재 객체 풀을 초기화하는 데 사용됩니다.
        if (existingPool.Count > 0)
        {
            objectPool = existingPool;
            existingPool = new Dictionary<string, Queue<GameObject>>();
        }
    }

    // 객체 가져오기
    public GameObject GetObject(GameObject prefab)
    {
        GameObject _object;

        // 객체 풀이 해당 객체 유형을 포함하지 않거나 객체 풀이 비어 있을 경우, 새로운 객체를 생성하여 객체 풀에 추가
        if (!objectPool.ContainsKey(prefab.name) || objectPool[prefab.name].Count == 0)
        {
            _object = GameObject.Instantiate(prefab);
            PushObject(_object); // 새로 생성된 객체를 객체 풀에 추가
            if (pool == null)
                pool = new GameObject("ObjectPool"); // 부모 객체 풀이 없으면 생성

            // 동일한 유형의 객체를 저장하기 위한 자식 객체 풀 생성
            GameObject childPool = GameObject.Find(prefab.name + "Pool");
            if (!childPool)
            {
                childPool = new GameObject(prefab.name + "Pool");
                childPool.transform.SetParent(pool.transform); // 자식 객체 풀의 부모 객체를 총 객체 풀로 설정
            }
            _object.transform.SetParent(childPool.transform); // 새로 생성된 객체의 부모 객체 설정
        }

        // 객체 풀 큐에서 객체를 꺼내고 기록된 객체 집합에서 제거
        _object = objectPool[prefab.name].Dequeue();
        pooledObjects.Remove(_object); // HashSet에서 가져온 객체 제거
        _object.SetActive(true);
        return _object;
    }

    // 객체를 넣는 메서드, 객체를 객체 풀에 되돌리고 중복 여부를 검사
    public void PushObject(GameObject prefab)
    {
        string _name = prefab.name.Replace("(Clone)", string.Empty); // 객체 이름에서 (Clone) 접미사를 제거하여 원래 객체 이름 획득

        // 객체 풀이 해당 유형의 객체 큐를 포함하지 않으면 새로운 큐 생성
        if (!objectPool.ContainsKey(_name))
        {
            objectPool.Add(_name, new Queue<GameObject>());
        }

        // 객체가 이미 오브젝트 풀에 있는지 확인
        if (!pooledObjects.Contains(prefab))
        {
            objectPool[_name].Enqueue(prefab);
            pooledObjects.Add(prefab); // HashSet에 추가
            prefab.SetActive(false);
        }
    }

    // 씬 전환 시 현재 객체 풀을 저장하기 위해 이 메서드를 호출합니다.
    public void SavePool()
    {
        existingPool = objectPool;
        objectPool = new Dictionary<string, Queue<GameObject>>();
    }
}
