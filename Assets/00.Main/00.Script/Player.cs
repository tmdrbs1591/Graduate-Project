using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("����")]
    public float speed = 5f; // �÷��̾� �̵� �ӵ�
    public float attackPower = 5f; //���ݷ�
    [SerializeField] Vector3 attackBoxSize; //���� ����
    [SerializeField] Transform attackBoxPos; // ���� ��ġ
    [SerializeField] Vector3 skillBoxSize; //��ų ����
    [SerializeField] Transform skillBoxPos; // ��ų ��ġ
    [Header("��Ÿ��")]
    public int comboStep = 0; // ���� �޺� �ܰ�
    private float comboTimer = 0f; // �޺� Ÿ�̸�
    public float comboDelay = 1f; // �޺� �Է� ���� �ð� (��)
    public float slashActiveTime = 0.5f; // ������ Ȱ��ȭ �ð�
    public float cooldownTime = 0.2f; // ��Ÿ�� (��)
    private float nextAttackTime = 0f; // ���� ��� ���� ����
    public float skillCooldownTime = 0.2f; // ��Ÿ�� (��)
    private float skillnextAttackTime = 0f; // ���� ��� ���� ����

    private Rigidbody rb;    // Rigidbody ������Ʈ ����
    private Animator animator; // Animator ������Ʈ ����
    private SpriteRenderer spriteRen;
    private bool facingRight = true; // ���� �ٶ󺸴� ���� (�������� �⺻)

    [SerializeField] GameObject[] slashs; // ������ ���� �迭
    [SerializeField] GameObject damageText; // ������ ���� �迭
    [SerializeField] GameObject skillEffect;
    public Ghost ghost;

    public bool isAttacking = false; // ���� ������ ���θ� ��Ÿ���� ����
    public bool isSkill = false; // ��ų��� ������ ���θ� ��Ÿ���� ����

    void Start()
    {
        // Rigidbody �� Animator ������Ʈ ��������
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        spriteRen = GetComponent<SpriteRenderer>();

        // ��� ������ ��Ȱ��ȭ
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
    }

    void HandleCombo()
    {
        // �޺� Ÿ�̸� ����
        if (comboTimer > 0)
        {
            comboTimer -= Time.deltaTime;
        }
        else
        {
            ResetCombo(); // �޺� �ʱ�ȭ
        }

        // ���� �Է� ó��
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (Time.time >= nextAttackTime)
            {
                Attack();
                AudioManager.instance.PlaySound(transform.position, 0, Random.Range(1.2f, 1.3f), 1f);

                nextAttackTime = Time.time + cooldownTime;
            }
        }
    }

    void Attack()
    {
        if (comboTimer <= 0)
        {
            comboStep = 0; // Ÿ�̸Ӱ� �ʱ�ȭ�� ��� �޺� ����
        }
        Damage(attackPower,attackBoxPos,attackBoxSize);

        // �޺� �ܰ躰�� ��Ÿ�� ���� (�� �� ���� �� ����)
        switch (comboStep)
        {
            case 0:
                cooldownTime = 0.2f; // 0�ܰ�: �ʱ� ��
                animator.SetTrigger("Attack1");
                break;
            case 1:
                cooldownTime = 0.2f; // 1�ܰ�: ª�� ��
                animator.SetTrigger("Attack2");
                StartCoroutine(GhostActive());
                break;
            case 2:
                cooldownTime = 0.1f; // 2�ܰ�: ª�� ��
                animator.SetTrigger("Attack1");
                break;
            case 3:
                cooldownTime = 0.2f; // 3�ܰ�: �� ����
                animator.SetTrigger("Attack2");
                break;
            case 4:
                cooldownTime = 0.5f; // 4�ܰ�: �⺻��
                animator.SetTrigger("Attack1");
                break;
            default:
                cooldownTime = 0.5f; // �⺻��
                break;
        }

        // ���� �� ���·� ����
        isAttacking = true;

        // ���� �޺� �ܰ��� ������ Ȱ��ȭ
        Debug.Log($"{comboStep + 1}�� ������ Ȱ��ȭ");
        StartCoroutine(ActivateSlash(slashs[comboStep]));

        // Rigidbody�� AddForce�� ������ �̵�
        Vector3 forceDirection = facingRight ? Vector3.right : Vector3.left; // �ٶ󺸴� ����
        rb.AddForce(forceDirection * 2f, ForceMode.Impulse); // �� ũ��� ��� ����

        // ���� �� �̵��� ���߱� ���� �ӵ� 0���� ����
        rb.velocity = Vector3.zero;

        // �޺� �ܰ� ����
        comboStep++;
        if (comboStep >= slashs.Length)
        {
            ResetCombo(); // �޺� �ܰ谡 �ִ�ġ�� ������ �ʱ�ȭ
        }
        else
        {
            comboTimer = comboDelay; // �޺� Ÿ�̸� �ʱ�ȭ
        }
    }

    void Skill()
    {
        if (Input.GetKeyDown(KeyCode.A))
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
        isSkill = true;
        ghost.makeGhost = true;
        DialogManager.instance.isDialogActive = true;
        StopChar();


        skillEffect.SetActive(false);
        skillEffect.SetActive(true);
        spriteRen.enabled = false;

        yield return new WaitForSeconds(0.16f);

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
        yield return new WaitForSeconds(slashActiveTime); // Ȱ��ȭ �ð���ŭ ���
        slash.SetActive(false);

        // ������ ���� �� �̵� �����ϰ� ����
        isAttacking = false;
    }

    void ResetCombo()
    {
        comboStep = 0;
        comboTimer = 0f;
    }

    // ĳ���� ���� ��ȯ �Լ�
    void Flip()
    {
        facingRight = !facingRight; // ���� ��ȯ
        Vector3 scale = transform.localScale;
        scale.x *= -1; // x�� ����
        transform.localScale = scale;
    }

    void Move()
    {
        if (isAttacking) // ���� �߿��� �̵��� ����
            return;

        // ���� �Է°� ��������
        float horizontalInput = Input.GetAxis("Horizontal");

        // Rigidbody�� ���� ������ �����̱�
        Vector3 movement = new Vector3(horizontalInput * speed, rb.velocity.y, 0);
        rb.velocity = movement;


        // �ִϸ��̼� ���� ������Ʈ
        if (horizontalInput != 0)
            animator.SetBool("isRun", true); // Run �ִϸ��̼� ����
        else
            animator.SetBool("isRun", false); // Run �ִϸ��̼� ����

        // ĳ���� ���� ��ȯ
        if (horizontalInput > 0 && !facingRight)
            Flip(); // ���������� ȸ��
        else if (horizontalInput < 0 && facingRight)
            Flip(); // �������� ȸ��
    }

    void Damage(float damage , Transform boxPos , Vector3 boxSize )
    {
        // ��ȣ�ۿ� �ڽ� ���� �浹ü Ȯ��
        Collider[] colliders = Physics.OverlapBox(boxPos.position, boxSize / 2f);

        foreach (Collider collider in colliders)
        {
            if (collider != null && collider.CompareTag("Enemy"))
            {
                var enemyScript = collider.GetComponent<Enemy>();

                if (enemyScript != null)
                {
                    // ������ ���� �ֱ�
                    enemyScript.TakeDamage(damage);

                    float randomX = Random.Range(-2f, 2f);  // X �� ���� (��: -2f���� 2f ����)
                    float randomY = Random.Range(0.5f, 1.5f); // Y �� ���� (��: 0.5f���� 1.5f ����)

                    Vector3 randomPosition = new Vector3(enemyScript.transform.position.x + randomX, enemyScript.transform.position.y + randomY, enemyScript.transform.position.z);
                    var damageTextScript = Instantiate(damageText, randomPosition, Quaternion.identity).GetComponent<TMP_Text>();


                    damageTextScript.text = damage.ToString();

                    Destroy(damageTextScript.gameObject,3f);
                    // ������ٵ� ã�� �˹��Ű��
                    Rigidbody rb = collider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        // �˹� ����� ����
                        Vector2 knockbackDirection = collider.transform.position - transform.position; // �������� ��ġ���� ���� ��ġ�� ���� ���� ���
                        rb.velocity = Vector2.zero; // ���� �ӵ� �ʱ�ȭ (�˹� ���� �̵��� ���߱� ����)
                        rb.AddForce(knockbackDirection.normalized * enemyScript.knockbackPower, ForceMode.Impulse); // �˹�
                    }
                }
            }
        }
    }

    void Dialog()
    {
        if (Input.GetKeyDown(KeyCode.C))
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
        animator.SetBool("isRun", false); // Run �ִϸ��̼� ����
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackBoxPos.position, attackBoxSize);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(skillBoxPos.position, skillBoxSize);
    }
}
