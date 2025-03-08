using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Player player;
    public bool isScan;
    public List<Transform> enemyTransform;

    [SerializeField] GameObject playerAttackTrail;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        // isScan�� false�� ���� �� �� ����Ʈ�� ���� ���߰� ��
        if (!isScan && !isMoving && enemyTransform.Count > 0)
        {
            StartCoroutine(MovePlayerToEnemies());
        }
    }

    private bool isMoving = false;

    // �ڷ�ƾ�� ����Ͽ� �÷��̾ ������� �̵���Ű�� �޼���
    IEnumerator MovePlayerToEnemies()
    {
        isMoving = true;
        playerAttackTrail.SetActive(true);
        foreach (var enemy in enemyTransform)
        {
            // ��ǥ ��ġ�� �̵�
            while (Vector3.Distance(player.transform.position, enemy.position) > 0.1f)
            {
                // �÷��̾��� ���� ��ġ���� ��ǥ ��ġ�� �̵�
                player.transform.position = Vector3.MoveTowards(player.transform.position, enemy.position, Time.deltaTime * 40f); // �ӵ� 10f�� ���� ����
                yield return null; // �� �����Ӹ��� ���
            }

            player.Attack();
            // ��ǥ ��ġ�� �����ϸ� �ش� ���� Die �޼��带 ȣ��
            Enemy enemyScript = enemy.GetComponent<Enemy>(); // ���� Enemy ��ũ��Ʈ ��������
            if (enemyScript != null)
            {
                enemyScript.Die(); // Die �޼��� ȣ��
            }

            // ��� ��� (0.06�� ����)
            yield return new WaitForSeconds(0.01f);
        }

        // ����Ʈ�� �� �� �� ���� ���� ����Ʈ�� ��� ��Ҹ� ����
        enemyTransform.Clear();
        playerAttackTrail.SetActive(false);

        // �̵��� �������� isMoving�� false�� ����
        isMoving = false;
    }
}
