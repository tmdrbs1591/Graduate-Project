using UnityEngine;

public class RopeController : MonoBehaviour
{
    public Transform startPoint; // 로프의 시작 지점
    public Transform endPoint; // 로프의 끝 지점
    public float grindSpeed = 3f; // 로프를 타고 이동하는 속도
    public float detectionRange = 5f; // 로프 탑승 가능 거리
    public LineRenderer lineRenderer; // 시각적 효과를 위한 점선
    public KeyCode grindKey = KeyCode.F; // 로프 타기 키

    private bool isGrinding = false; // 로프 타는 상태
    private bool isNearRope = false; // 로프 근처에 있는지 여부
    private Transform playerTransform; // 플레이어의 Transform

    public Player playerScript;

    void Update()
    {
        DetectPlayerProximity(); // 플레이어가 가까운지 감지

        if (isGrinding)
        {
            GrindOnRope(); // 로프 타는 중 이동 처리
        }
    }

    // 플레이어 감지
    private void DetectPlayerProximity()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, startPoint.position);
            isNearRope = distance <= detectionRange;

            if (isNearRope)
            {
                lineRenderer.enabled = true; // 점선 활성화

                if (Input.GetKeyDown(grindKey))
                {
                    StartGrinding(player.transform);
                }
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }
    }

    // 로프 타기 시작
    public void StartGrinding(Transform player)
    {
        if (GameManager.instance.isMoving)
            return;

        playerScript.ghost.ghostDelay /= 3;
        playerScript.ghost.makeGhost = true;

        CameraShake.instance.ShakeCamera(15f, 0.2f);
        playerTransform = player;
        playerTransform.position = startPoint.position; // 로프 시작점으로 이동
        isGrinding = true;
        playerScript.rb.isKinematic = true;
    }

    // 로프 타기 중 이동 처리
    private void GrindOnRope()
    {
        if (playerTransform != null)
        {
            playerTransform.position = Vector3.MoveTowards(playerTransform.position, endPoint.position, grindSpeed * Time.deltaTime);

            if (playerTransform.position == endPoint.position)
            {
                StopGrinding();
            }
        }
    }

    // 로프 타기 종료
    public void StopGrinding()
    {
        isGrinding = false;
        playerScript.rb.isKinematic = false;


        playerScript.ghost.ghostDelay *= 3;
        playerScript.ghost.makeGhost = false;
    }

    // 로프 시작 시 시각적 효과
    void Start()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPoint.position);
            lineRenderer.SetPosition(1, endPoint.position);
            lineRenderer.enabled = false; // 기본적으로 비활성화
        }
    }
}
