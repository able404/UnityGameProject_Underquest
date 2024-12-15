using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Movement Settings")]
    public float     speed;
    private Vector2  movement;
    private Vector2  mousePos;

    [Header("Knockback Settings")]
    public float    knockBackLength;
    public float    knockBackForce;
    private float   knockBackCounter;

    [Header("Dash Settings")]
    public float    dashingPower;
    public float    dashingTime;
    public float    dashCooldown;
    private float   dashCooldownTimer;
    private bool    canDash = true;
    private bool    isDashing;

    [Header("Weapons")]
    public GameObject[] guns;
    private int gunNum;
    public bool[] isGunUnlocked; 

    [Header("Components")]
    private Rigidbody2D theRB;
    private Animator anim;
    [SerializeField] private TrailRenderer tr;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        theRB = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        isGunUnlocked = new bool[guns.Length];
        isGunUnlocked[0] = true;
        guns[0].SetActive(true);

        dashCooldownTimer = dashCooldown;
    }

    void Update()
    {
        if (PauseMenu.instance.isPaused) return;

        HandleMovementInput();
        HandleRotation();
        HandleDash();
        HandleGunSwitch();
        UpdateAnimation();

        // 대쉬 쿨다운 UI 업데이트
        UIController.instance.UpdateDashCooldown(dashCooldownTimer);
    }

    private void FixedUpdate()
    {
        if (knockBackCounter <= 0 && !isDashing && !PauseMenu.instance.isPaused)
        {
            theRB.MovePosition(theRB.position + movement * speed * Time.fixedDeltaTime);
        }
    }

    // 플레이어 입력 처리
    private void HandleMovementInput()
    {
        if (knockBackCounter > 0)
        {
            knockBackCounter -= Time.deltaTime;
            return;
        }

        if (!isDashing)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
    }

    // 플레이어 바라보는 방향 처리
    private void HandleRotation()
    {
        if (mousePos.x > transform.position.x)
        {
            transform.rotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
    }

    // 무기 교체 처리
    private void HandleGunSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwitchGun(-1);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            SwitchGun(1);
        }
    }

    // 무기 전환
    private void SwitchGun(int direction)
    {
        guns[gunNum].SetActive(false);
        do
        {
            gunNum += direction;
            if (gunNum < 0) gunNum = guns.Length - 1;
            if (gunNum >= guns.Length) gunNum = 0;
        } while (!isGunUnlocked[gunNum]);
        guns[gunNum].SetActive(true);
    }

    // 현재 활성 무기 설정
    private void SetActiveGun(int index)
    {
        foreach (var g in guns) g.SetActive(false);
        guns[index].SetActive(true);
        gunNum = index;
    }

    // 무기 획득 시 자동 전환
    public void PickUpGun(int gunIndex)
    {
        isGunUnlocked[gunIndex] = true;
        SetActiveGun(gunIndex);
    }

    // 넉백 처리
    public void KnockBack(Transform attackerTransform)
    {
        knockBackCounter = knockBackLength;
        Vector2 knockBackDirection = (transform.position - attackerTransform.position).normalized;
        theRB.velocity = knockBackDirection * knockBackForce;
    }

    // 대쉬 처리
    private void HandleDash()
    {
        if (!canDash)
        {
            dashCooldownTimer += Time.deltaTime;
            if (dashCooldownTimer >= dashCooldown)
            {
                dashCooldownTimer = dashCooldown;
                canDash = true;
            }
        }

        if (Input.GetMouseButtonDown(1) && canDash && !isDashing)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        dashCooldownTimer = 0f;

        float originalGravity = theRB.gravityScale;
        theRB.gravityScale = 0f;

        theRB.velocity = ((mousePos - (Vector2)transform.position).normalized) * dashingPower;
        tr.emitting = true;

        PlayerHealthController.instance.StartInvincibility(dashingTime);

        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;

        theRB.gravityScale = originalGravity;
        isDashing = false;
    }

    // 애니메이션 상태 업데이트
    private void UpdateAnimation()
    {
        anim.SetFloat("speed", movement.magnitude);
    }
}
