using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weakness : MonoBehaviour
{
    [SerializeField] GameObject weaknessEffect;

    SpriteRenderer spriteRenderer;

    [SerializeField] Color[] spriteColor;


    public float maxHp;
    public float curHP;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        curHP = maxHp;
    }
    private void Update()
    {
        if (curHP <= 0)
        {


            // 부모의 위치를 GameManager의 enemyTransform 리스트에 추가
            if (GameManager.instance != null && transform.parent != null)
            {
                GameManager.instance.enemyTransform.Add(transform.parent); // 부모의 위치 추가
            }

            // 오브젝트 삭제
            Destroy(gameObject);
        }

        switch (curHP)
        {
            case 5:
                spriteRenderer.color = spriteColor[0];
                break;
            case 4:
                spriteRenderer.color = spriteColor[1];
                break;
            case 3:
                spriteRenderer.color = spriteColor[2];
                break;
            case 2:
                spriteRenderer.color = spriteColor[3];
                break;
            case 1:
                spriteRenderer.color = spriteColor[4];
                break;
            default:
                spriteRenderer.color = spriteColor[0];
                break;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ScanTrail"))
        {
            curHP--;

            CameraShake.instance.ShakeCamera(6f, 0.1f);
            AudioManager.instance.PlaySound(transform.position, 5, Random.Range(1f, 1.3f), 0.5f);

            // 이펙트 생성
            Instantiate(weaknessEffect, transform.position, Quaternion.identity);
        }
    }
}
