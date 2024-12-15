using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public static Gun instance;

    [Header("Gun Settings")]
    public float inserval; // 사격 간격

    [Header("Prefabs")]
    public GameObject bulletPrefab; // 탄환과 탄피 프리팹
    public GameObject shellPrefab;

    protected Transform muzzlePos; // 총구와 탄피 생성 위치
    protected Transform shellPos;

    protected Vector2 mousePos;
    protected Vector2 direction; // 사격 방향

    protected float timer; // 쿨다운 타이머와 캐릭터 플립 상태
    protected float flipY; 

    protected Animator animator;

    protected virtual void OnEnable()
    {
        instance = this;
    }

    protected virtual void OnDisable()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    public float CooldownTimer
    {
        get { return timer; }
    }

    public float CooldownInterval
    {
        get { return inserval; }
    }

    protected virtual void Start()
    {
        animator = GetComponent<Animator>();
        muzzlePos = transform.Find("Muzzle"); // 총구 위치 가져오기
        shellPos = transform.Find("BulletShell"); // 탄피 생성 위치 가져오기
        flipY = transform.localScale.y; // 초기 방향 기록
    }

    protected virtual void Update()
    {
        if (!PauseMenu.instance.isPaused)
        {
            // 마우스 위치의 월드 좌표 가져오기
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 마우스 위치에 따라 캐릭터 플립 설정
            if (mousePos.x < transform.position.x)
                transform.localScale = new Vector3(flipY, -flipY, 1);
            else
                transform.localScale = new Vector3(flipY, flipY, 1);

            Shoot();

            // UI에 무기 쿨다운 진행도 업데이트
            if (instance == this)
            {
                UIController.instance.UpdateGunCooldown(GetCooldownProgress());
            }
        }
    }

    // 쿨다운 진행도(0~1) 반환
    public virtual float GetCooldownProgress()
    {
        if (CooldownInterval <= 0f)
            return 1f; // 0으로 나누기 방지
        return CooldownTimer / CooldownInterval;
    }

    protected virtual void Shoot()
    {
        // 사격 방향 계산
        Vector2 currentPos = transform.position;
        direction = (mousePos - currentPos).normalized;

        // 총구를 사격 방향으로 회전
        transform.right = direction;

        // 쿨다운 타이머 감소
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer < 0)
                timer = 0;
        }

        // 사격 버튼 누르고 쿨다운 완료 시 발사
        if (Input.GetButtonDown("Fire1"))
        {
            if (timer == 0)
            {
                timer = inserval;
                Fire();
            }
        }
    }

    protected virtual void Fire()
    {
        animator.SetTrigger("Shoot");
        AudioManager.instance.PlaySFX(5);

        // 탄환 생성
        GameObject bullet = ObjectPool.Instance.GetObject(bulletPrefab); // 오브젝트 풀에서 탄환 가져오기
        bullet.transform.position = muzzlePos.position; // 탄환 위치 설정

        float angle = Random.Range(-5f, 5f); // 랜덤 각도 오프셋 생성   
        bullet.GetComponent<Bullet>().SetSpeed(Quaternion.AngleAxis(angle, Vector3.forward) * direction); // 탄환 속도 설정

        // 탄피 생성
        GameObject shell = ObjectPool.Instance.GetObject(shellPrefab); // 오브젝트 풀에서 탄피 가져오기
        shell.transform.position = shellPos.position;
        shell.transform.rotation = shellPos.rotation;
    }
}
