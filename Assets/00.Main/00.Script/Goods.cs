using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goods : MonoBehaviour
{
    public float speed = 5f;  // 일정한 이동 속도
    public float curveStrength = 2f;  // 곡선의 강도 (제어점의 이동 거리)
    public float minStartDelay = 0f;  // 시작 지연의 최소 값 (초)
    public float maxStartDelay = 0.5f;  // 시작 지연의 최대 값 (초)

    [SerializeField] GameObject goodsEffect;

    private Transform player;  // 플레이어의 Transform
    private Vector3 startPoint;
    private Vector3 controlPoint;
    private Vector3 endPoint;
    private float t = 0f;  // 시간 변수
    private float startTime;  // 시작 시간
    private bool useCurve = true;  // 곡선 사용 여부

    void Start()
    {
        // 플레이어 오브젝트를 태그를 통해 찾기
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            return;
        }

        // 시작점, 끝점 초기화
        startPoint = transform.position;
        endPoint = player.position;
        SetRandomControlPoint();  // 랜덤 제어점 설정

        // 시작 지연을 랜덤으로 설정
        startTime = Time.time + Random.Range(minStartDelay, maxStartDelay);
    }

    void Update()
    {
        if (player == null)
            return;

        // 지연이 지나지 않았으면 기다림
        if (Time.time < startTime)
            return;

        // 현재 목표 점으로 이동
        if (useCurve)
        {
            // 곡선 경로를 따라 이동
            t += Time.deltaTime * speed / Vector3.Distance(startPoint, endPoint);
            t = Mathf.Clamp01(t);  // t를 0과 1 사이로 제한

            // 베지어 곡선의 위치를 계산
            Vector3 curvePosition = CalculateBezierCurve(startPoint, controlPoint, endPoint, t);
            transform.position = curvePosition;

            // 오브젝트가 끝점에 도달하면 플레이어의 새로운 위치에 맞게 업데이트
            if (t >= 1f)
            {
                // 정확한 위치로 설정
                transform.position = endPoint;
                // 새로운 시작점과 끝점 설정
                startPoint = endPoint;
                endPoint = player.position;
                SetRandomControlPoint();  // 새로운 랜덤 제어점 설정
                t = 0f;  // 시간 리셋
                useCurve = false;  // 직선 이동으로 변경
            }
        }
        else
        {
            // 직선으로 이동
            transform.position = Vector3.MoveTowards(transform.position, endPoint, speed * Time.deltaTime);

            // 오브젝트가 끝점에 도달하면 플레이어의 새로운 위치에 맞게 업데이트
            if (transform.position == endPoint)
            {
                // 새로운 시작점과 끝점 설정
                startPoint = endPoint;
                endPoint = player.position;
                // 직선 이동으로 계속 설정
                useCurve = true;
                SetRandomControlPoint();  // 새로운 랜덤 제어점 설정
            }
        }
    }

    // 2차 베지어 곡선 계산
    Vector3 CalculateBezierCurve(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        float u = 1f - t;
        return u * u * start + 2f * u * t * control + t * t * end;
    }

    // 제어점을 랜덤하게 설정
    void SetRandomControlPoint()
    {
        // 제어점을 현재 시작점과 끝점의 중간점에서 랜덤한 방향으로 이동
        Vector3 midPoint = (startPoint + endPoint) / 2;
        Vector3 randomDirection = Random.onUnitSphere;  // 랜덤 방향 벡터
        controlPoint = midPoint + randomDirection * curveStrength;

        // Y축 방향만 고려하도록 설정하려면 다음과 같이 할 수 있음:
        // controlPoint = midPoint + new Vector3(Random.Range(-curveStrength, curveStrength), Random.Range(-curveStrength, curveStrength), Random.Range(-curveStrength, curveStrength));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(Instantiate(goodsEffect, transform.position, Quaternion.identity), 2f);
        }
    }
}
