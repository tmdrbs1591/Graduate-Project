using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("스텟")]
    public float speed = 5f; // 플레이어 이동 속도


    private Rigidbody rb;    // Rigidbody 컴포넌트 참조
    private Animator animator; // Animator 컴포넌트 참조
    private bool facingRight = true; // 현재 바라보는 방향 (오른쪽이 기본)

    void Start()
    {
        // Rigidbody 및 Animator 컴포넌트 가져오기
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        Move();
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
}
