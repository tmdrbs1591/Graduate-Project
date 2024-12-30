using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("����")]
    public float speed = 5f; // �÷��̾� �̵� �ӵ�


    private Rigidbody rb;    // Rigidbody ������Ʈ ����
    private Animator animator; // Animator ������Ʈ ����
    private bool facingRight = true; // ���� �ٶ󺸴� ���� (�������� �⺻)

    void Start()
    {
        // Rigidbody �� Animator ������Ʈ ��������
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Move();
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
}
