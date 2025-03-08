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
        // isScan이 false일 때만 한 번 리스트를 돌고 멈추게 함
        if (!isScan && !isMoving && enemyTransform.Count > 0)
        {
            StartCoroutine(MovePlayerToEnemies());
        }
    }

    private bool isMoving = false;

    // 코루틴을 사용하여 플레이어를 순서대로 이동시키는 메서드
    IEnumerator MovePlayerToEnemies()
    {
        isMoving = true;
        playerAttackTrail.SetActive(true);
        foreach (var enemy in enemyTransform)
        {
            // 목표 위치로 이동
            while (Vector3.Distance(player.transform.position, enemy.position) > 0.1f)
            {
                // 플레이어의 현재 위치에서 목표 위치로 이동
                player.transform.position = Vector3.MoveTowards(player.transform.position, enemy.position, Time.deltaTime * 40f); // 속도 10f는 조정 가능
                yield return null; // 매 프레임마다 대기
            }

            player.Attack();
            // 목표 위치에 도달하면 해당 적의 Die 메서드를 호출
            Enemy enemyScript = enemy.GetComponent<Enemy>(); // 적의 Enemy 스크립트 가져오기
            if (enemyScript != null)
            {
                enemyScript.Die(); // Die 메서드 호출
            }

            // 잠깐 대기 (0.06초 간격)
            yield return new WaitForSeconds(0.01f);
        }

        // 리스트를 한 번 다 돌고 나서 리스트의 모든 요소를 제거
        enemyTransform.Clear();
        playerAttackTrail.SetActive(false);

        // 이동이 끝났으면 isMoving을 false로 설정
        isMoving = false;
    }
}
