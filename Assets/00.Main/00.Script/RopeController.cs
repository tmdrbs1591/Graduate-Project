using UnityEngine;

public class RopeController : MonoBehaviour
{
    public Transform startPoint; // ������ ���� ����
    public Transform endPoint; // ������ �� ����
    public float grindSpeed = 3f; // ������ Ÿ�� �̵��ϴ� �ӵ�
    public float detectionRange = 5f; // ���� ž�� ���� �Ÿ�
    public LineRenderer lineRenderer; // �ð��� ȿ���� ���� ����
    public KeyCode grindKey = KeyCode.F; // ���� Ÿ�� Ű

    private bool isGrinding = false; // ���� Ÿ�� ����
    private bool isNearRope = false; // ���� ��ó�� �ִ��� ����
    private Transform playerTransform; // �÷��̾��� Transform

    public Player playerScript;

    void Update()
    {
        DetectPlayerProximity(); // �÷��̾ ������� ����

        if (isGrinding)
        {
            GrindOnRope(); // ���� Ÿ�� �� �̵� ó��
        }
    }

    // �÷��̾� ����
    private void DetectPlayerProximity()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            float distance = Vector3.Distance(player.transform.position, startPoint.position);
            isNearRope = distance <= detectionRange;

            if (isNearRope)
            {
                lineRenderer.enabled = true; // ���� Ȱ��ȭ

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

    // ���� Ÿ�� ����
    public void StartGrinding(Transform player)
    {
        if (GameManager.instance.isMoving)
            return;

        playerScript.ghost.ghostDelay /= 3;
        playerScript.ghost.makeGhost = true;

        CameraShake.instance.ShakeCamera(15f, 0.2f);
        playerTransform = player;
        playerTransform.position = startPoint.position; // ���� ���������� �̵�
        isGrinding = true;
        playerScript.rb.isKinematic = true;
    }

    // ���� Ÿ�� �� �̵� ó��
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

    // ���� Ÿ�� ����
    public void StopGrinding()
    {
        isGrinding = false;
        playerScript.rb.isKinematic = false;


        playerScript.ghost.ghostDelay *= 3;
        playerScript.ghost.makeGhost = false;
    }

    // ���� ���� �� �ð��� ȿ��
    void Start()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, startPoint.position);
            lineRenderer.SetPosition(1, endPoint.position);
            lineRenderer.enabled = false; // �⺻������ ��Ȱ��ȭ
        }
    }
}
