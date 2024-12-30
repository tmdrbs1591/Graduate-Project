using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("����")]
    public float speed = 5f; // �÷��̾� �̵� �ӵ�
    [SerializeField] Vector3 attackBoxSize; //���� ����
    [SerializeField] Transform attackBoxPos; // ���� ��ġ

    [Header("��Ÿ��")]
    public int comboStep = 0; // ���� �޺� �ܰ�
    private float comboTimer = 0f; // �޺� Ÿ�̸�
    public float comboDelay = 1f; // �޺� �Է� ���� �ð� (��)
    public float slashActiveTime = 0.5f; // ������ Ȱ��ȭ �ð�
    public float cooldownTime = 0.2f; // ��Ÿ�� (��)
    private float nextAttackTime = 0f; // ���� ��� ���� ����

    private Rigidbody rb;    // Rigidbody ������Ʈ ����
    private Animator animator; // Animator ������Ʈ ����
    private bool facingRight = true; // ���� �ٶ󺸴� ���� (�������� �⺻)

    [SerializeField] GameObject[] slashs; // ������ ���� �迭
    public Ghost ghost;

    public bool isAttacking = false; // ���� ������ ���θ� ��Ÿ���� ����

    void Start()
    {
        // Rigidbody �� Animator ������Ʈ ��������
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();

        // ��� ������ ��Ȱ��ȭ
        foreach (GameObject slash in slashs)
        {
            slash.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        Move();
    }

    void Update()
    {
        HandleCombo();
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
        Damage(1);
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
        Debug.Log("�޺� �ʱ�ȭ");
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

    void Damage(float damage)
    {
        // ��ȣ�ۿ� �ڽ� ���� �浹ü Ȯ��
        Collider[] colliders = Physics.OverlapBox(attackBoxPos.position, attackBoxSize / 2f);

        foreach (Collider collider in colliders)
        {
            if (collider != null && collider.CompareTag("Enemy"))
            {
                var enemyScript = collider.GetComponent<Enemy>();

                if (enemyScript != null)
                {
                    // ������ ���� �ֱ�
                    enemyScript.TakeDamage(damage);

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(attackBoxPos.position, attackBoxSize);
    }
}
