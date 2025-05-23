using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("스텟")]
    public float speed = 5f; // 플레이어 이동 속도
    public float attackPower = 5f; //공격력
    public float jumpPower;
    [Header("공격")]
    [SerializeField] Vector3 attackBoxSize; //공격 범위
    [SerializeField] Transform attackBoxPos; // 공격 위치

    public float groundCheckDistance = 0.2f; // 땅과의 거리 확인 (레이캐스트를 사용)
    public LayerMask groundLayer; // 바닥 레이어

    public int comboStep = 0; // 현재 콤보 단계
    private float comboTimer = 0f; // 콤보 타이머
    public float comboDelay = 1f; // 콤보 입력 가능 시간 (초)
    public float slashActiveTime = 0.5f; // 슬래시 활성화 시간

    public float cooldownTime = 0.2f; // 쿨타임 (초)
    private float nextAttackTime = 0f; // 공격 사용 가능 시점

    [Header("스킬")]
    [SerializeField] Vector3 skillBoxSize; //스킬 범위
    [SerializeField] Transform skillBoxPos; // 스킬 위치

    public float skillCooldownTime = 0.2f; // 쿨타임 (초)
    private float skillnextAttackTime = 0f; // 공격 사용 가능 시점

    [Header("대쉬")]
    public float dashCooldownTime = 0.2f; // 쿨타임 (초)
    private float dashNextAttackTime = 0f; // 사용 가능 시점

    public Rigidbody rb;    // Rigidbody 컴포넌트 참조
    private Animator animator; // Animator 컴포넌트 참조
    private SpriteRenderer spriteRen;
    private bool facingRight = true; // 현재 바라보는 방향 (오른쪽이 기본)

    [SerializeField] GameObject[] slashs; // 슬래시 공격 배열
    [SerializeField] GameObject damageText; // 슬래시 공격 배열
    [SerializeField] GameObject skillEffect;
    [SerializeField] GameObject weaknessEffect;
    public Ghost ghost;

    public bool isAttacking = false; // 공격 중인지 여부를 나타내는 변수
    public bool isSkill = false; // 스킬사용 중인지 여부를 나타내는 변수
    private bool isGrounded; // 현재 땅에 닿아있는지
    private bool canDoubleJump;
    void Start()
    {
        // Rigidbody 및 Animator 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        spriteRen = GetComponent<SpriteRenderer>();

        // 모든 슬래시 비활성화
        foreach (GameObject slash in slashs)
        {
            slash.SetActive(false);
        }
    }

    void FixedUpdate()
    {

        if (DialogManager.instance.isDialogActive || TimeLineManager.instance.isCutScene)
            return;
        Move();

    }

    void Update()
    {
        if (DialogManager.instance.isDialogActive || TimeLineManager.instance.isCutScene)
            return;
        HandleCombo();
        Dialog();
        Skill();
        Dash();

        // 바닥에 닿아있는지 확인
        isGrounded = IsGrounded();

        // 바닥에 닿아 있으면 더블 점프를 사용할 수 있도록 설정
        if (isGrounded)
        {
            canDoubleJump = true;
        }

        // 점프 처리
        if ((isGrounded || canDoubleJump) && Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void HandleCombo()
    {
        // 콤보 타이머 감소
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
        }
        else
        {
            ResetCombo(); // 콤보 초기화
        }

        // 공격 입력 처리
        if (Input.GetMouseButtonDown(0) && !GameManager.instance.isScan)
        {
            if (Time.time >= nextAttackTime)
            {
                Attack();

                nextAttackTime = Time.time + cooldownTime;
            }
        }
    }

    void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            if (Time.time >= dashNextAttackTime)
            {
                animator.SetTrigger("isDash");

                // Rigidbody에 AddForce로 앞으로 이동
                Vector3 forceDirection = facingRight ? Vector3.right : Vector3.left; // 바라보는 방향
                rb.AddForce(forceDirection * 15f, ForceMode.Impulse); // 힘 크기와 모드 설정
                                                                            // 공격 중 이동을 멈추기 위해 속도 0으로 설정
                StartCoroutine(DashCor());
                rb.velocity = Vector3.zero;
                dashNextAttackTime = Time.time + dashCooldownTime;
            }
        }
    }

    IEnumerator DashCor()
    {
        isAttacking = true;
        ghost.ghostDelay /= 5;
        ghost.makeGhost = true;

        yield return new WaitForSeconds(0.1f);

        isAttacking = false;
        ghost.ghostDelay *= 5;
        ghost.makeGhost = false;
    }

    IEnumerator DashWithRigidbody(Vector3 direction, float force, float duration)
    {
        ghost.ghostDelay /= 5;
        ghost.makeGhost = true;

        float elapsedTime = 0f;
        rb.velocity = Vector3.zero; // 대쉬 전에 기존 속도 제거

        while (elapsedTime < duration)
        {
            rb.velocity = direction * force; // Rigidbody에 속도 부여
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector3.zero; // 대쉬 후 속도 초기화
        ghost.makeGhost = false;
        ghost.ghostDelay *= 5;
    }

    // 점프 구현
    public void Jump()
    {
        if (isGrounded)
        {
            // 첫 번째 점프
            rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
        }
        else if (canDoubleJump)
        {
            // 두 번째 점프 (더블 점프)
            rb.velocity = new Vector3(rb.velocity.x, jumpPower, rb.velocity.z);
            canDoubleJump = false; // 더블 점프 후에는 다시 사용할 수 없도록 설정
        }
    }

    // 바닥에 닿아 있는지 확인하는 함수 (레이캐스트로 확인)
    private bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance, groundLayer))
        {
            return true; // 바닥에 닿아 있음
        }
        return false; // 바닥에 닿아 있지 않음
    }
    public void Attack()
    {
        if (comboTimer <= 0)
        {
            comboStep = 0; // 타이머가 초기화된 경우 콤보 시작
        }
        Damage(attackPower,attackBoxPos,attackBoxSize);
        AudioManager.instance.PlaySound(transform.position, 0, Random.Range(1.2f, 1.3f), 1f);

        // 콤보 단계별로 쿨타임 설정 (딴 딴 딴딴 딴 패턴)
        switch (comboStep)
        {
            case 0:
                cooldownTime = 0.2f; // 0단계: 초기 딴
                animator.SetTrigger("Attack1");
                break;
            case 1:
                cooldownTime = 0.2f; // 1단계: 짧은 딴
                animator.SetTrigger("Attack2");
                StartCoroutine(GhostActive());
                break;
            case 2:
                cooldownTime = 0.1f; // 2단계: 짧은 딴
                animator.SetTrigger("Attack1");
                break;
            case 3:
                cooldownTime = 0.2f; // 3단계: 긴 딴딴
                animator.SetTrigger("Attack2");
                break;
            case 4:
                cooldownTime = 0.5f; // 4단계: 기본값
                animator.SetTrigger("Attack1");
                break;
            default:
                cooldownTime = 0.5f; // 기본값
                break;
        }

        // 공격 중 상태로 설정
        isAttacking = true;

        // 현재 콤보 단계의 슬래시 활성화
        Debug.Log($"{comboStep + 1}번 슬래쉬 활성화");
        StartCoroutine(ActivateSlash(slashs[comboStep]));

        // Rigidbody에 AddForce로 앞으로 이동
        Vector3 forceDirection = facingRight ? Vector3.right : Vector3.left; // 바라보는 방향
        rb.AddForce(forceDirection * 2f, ForceMode.Impulse); // 힘 크기와 모드 설정

        // 공격 중 이동을 멈추기 위해 속도 0으로 설정
        rb.velocity = Vector3.zero;

        // 콤보 단계 증가
        comboStep++;
        if (comboStep >= slashs.Length)
        {
            ResetCombo(); // 콤보 단계가 최대치를 넘으면 초기화
        }
        else
        {
            comboTimer = comboDelay; // 콤보 타이머 초기화
        }
    }

    void Skill()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (Time.time >= skillnextAttackTime)
            {
                StartCoroutine(SkillCor());

                skillnextAttackTime = Time.time + skillCooldownTime;
            }
        }
    }

    IEnumerator SkillCor()
    {

        CameraShake.instance.ShakeCamera(5f, 0.1f);

        AudioManager.instance.PlaySound(transform.position, 3, Random.Range(1.2f, 1.3f), 1f);

        isSkill = true;
        ghost.makeGhost = true;
        DialogManager.instance.isDialogActive = true;
        StopChar();


        skillEffect.SetActive(false);
        skillEffect.SetActive(true);
        spriteRen.enabled = false;

        yield return new WaitForSeconds(0.16f);
        Time.timeScale = 0.3f;
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 1;
        ghost.makeGhost = false;
        for (int i = 0; i < 12; i++)
        {
            AudioManager.instance.PlaySound(transform.position, 0, Random.Range(1.2f, 1.3f), 1f);
            Damage(attackPower - 2, skillBoxPos, skillBoxSize);
            yield return new WaitForSeconds(0.05f);
        }
        spriteRen.enabled = true;

        yield return new WaitForSeconds(0.5f);
        DialogManager.instance.isDialogActive = false;
        isSkill = false;

    }
    IEnumerator GhostActive()
    {
        ghost.makeGhost = true;
        yield return new WaitForSeconds(1f);
        ghost.makeGhost = false;

    }

    IEnumerator ActivateSlash(GameObject slash)
    {
        slash.SetActive(true);
        yield return new WaitForSeconds(slashActiveTime); // 활성화 시간만큼 대기
        slash.SetActive(false);

        // 공격이 끝난 후 이동 가능하게 설정
        isAttacking = false;
    }

    void ResetCombo()
    {
        comboStep = 0;
        comboTimer = 0f;
    }

    // 캐릭터 방향 전환 함수
    void Flip()
    {
        facingRight = !facingRight; // 방향 전환
        Vector3 scale = transform.localScale;
        scale.x *= -1; // x축 반전
        transform.localScale = scale;
    }

    void Move()
    {
        if (isAttacking) // 공격 중에는 이동을 막음
            return;

        // 수평 입력값 가져오기
        float horizontalInput = Input.GetAxis("Horizontal");

        // Rigidbody에 힘을 적용해 움직이기
        Vector3 movement = new Vector3(horizontalInput * speed, rb.velocity.y, 0);
        rb.velocity = movement;


        // 애니메이션 상태 업데이트
        if (horizontalInput != 0)
            animator.SetBool("isRun", true); // Run 애니메이션 시작
        else
            animator.SetBool("isRun", false); // Run 애니메이션 멈춤

        // 캐릭터 방향 전환
        if (horizontalInput > 0 && !facingRight)
            Flip(); // 오른쪽으로 회전
        else if (horizontalInput < 0 && facingRight)
            Flip(); // 왼쪽으로 회전
    }



    void Damage(float damage , Transform boxPos , Vector3 boxSize )
    {
        // 상호작용 박스 내의 충돌체 확인
        Collider[] colliders = Physics.OverlapBox(boxPos.position, boxSize / 2f);

        foreach (Collider collider in colliders)
        {
            if (collider != null && collider.CompareTag("Enemy"))
            {
                var enemyScript = collider.GetComponent<Enemy>();

                if (enemyScript != null)
                {
                    // 적에게 피해 주기
                    enemyScript.TakeDamage(damage);

                    float randomX = Random.Range(-2f, 2f);  // X 값 범위 (예: -2f에서 2f 사이)
                    float randomY = Random.Range(0.5f, 1.5f); // Y 값 범위 (예: 0.5f에서 1.5f 사이)

                    Vector3 randomPosition = new Vector3(enemyScript.transform.position.x + randomX, enemyScript.transform.position.y + randomY, enemyScript.transform.position.z);
                    var damageTextScript = Instantiate(damageText, randomPosition, Quaternion.identity).GetComponent<TMP_Text>();


                    damageTextScript.text = damage.ToString();

                    Destroy(damageTextScript.gameObject,3f);
                    // 리지드바디를 찾아 넉백시키기
                    Rigidbody rb = collider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        // 넉백 방향과 강도
                        Vector2 knockbackDirection = collider.transform.position - transform.position; // 공격자의 위치에서 적의 위치를 빼서 방향 계산
                        rb.velocity = Vector2.zero; // 기존 속도 초기화 (넉백 전에 이동을 멈추기 위해)
                        rb.AddForce(knockbackDirection.normalized * enemyScript.knockbackPower, ForceMode.Impulse); // 넉백
                    }
                }
            }
        }
    }

    void Dialog()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Collider[] colliders = Physics.OverlapBox(attackBoxPos.position, attackBoxSize / 2f);

            foreach (Collider collider in colliders)
            {
                if (collider != null && collider.CompareTag("NPC"))
                {
                    StopChar();

                    var npcScript = collider.GetComponent<NPC>();
                    if (npcScript != null) 
                    DialogManager.instance.DialogStart(npcScript.NPCID, collider.transform.position);
                }
            } 
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Goods") ){
            Destroy(other.gameObject);
        }
        if (other.CompareTag("CutSceneBox"))
        {
            StopChar();

            other.gameObject.SetActive(false);
            TimeLineManager.instance.StartCutScene(0);
        }
        
    }

    void StopChar()
    {
        rb.velocity = Vector3.zero;
        animator.SetBool("isRun", false); // Run 애니메이션 멈춤
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackBoxPos.position, attackBoxSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(skillBoxPos.position, skillBoxSize);
    }
}
