using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.U2D;

public class Enemy : MonoBehaviour
{
    [Header("스텟")]
    [SerializeField] float currentHP;
    [SerializeField] float maxHP;
   public float knockbackPower;

    [Header("메테리얼")]
    [SerializeField] Material dieMatetrial;
    [SerializeField] Material hitMaterial;
    [SerializeField] Material originalMaterial;

    [Header("이펙트")]
    [SerializeField] GameObject dieEffect;
    [SerializeField] GameObject hitEffect;

    [SerializeField] GameObject goodsPrefab;
    [SerializeField] GameObject weakness;
    [SerializeField] int goodsCount;


    bool isDie;

    Animator anim;
    SpriteRenderer sprite;

    private GameObject currentScanSphere; // 현재 감지된 ScanSpere를 저장할 변수
    private void Awake()
    {
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {

        if (currentScanSphere != null && !currentScanSphere.activeSelf)
        {
            if (weakness != null)
            {
                weakness.SetActive(false);
                currentScanSphere = null;
            } // 체크 종료
        }
    }

    public void TakeDamage(float damage)
    {

        Destroy(Instantiate(hitEffect,transform.position,Quaternion.identity),2f);
        AudioManager.instance.PlaySound(transform.position, 1, Random.Range(1f, 1.1f), 0.5f);

        CameraShake.instance.ShakeCamera(5f,0.1f);
        currentHP -= damage;

        if (anim != null)
        anim.SetTrigger("isHit");

        StartCoroutine(HitMaterial());
    }

    public void Die()
    {
            CameraShake.instance.ShakeCamera(6f, 0.2f);
            sprite.material = dieMatetrial;
            isDie = true;
            StartCoroutine(TimeSlow());
    }

    IEnumerator TimeSlow()
    {
        if (!GameManager.instance.player.isSkill)
        {
            Time.timeScale = 0.9f;
            yield return new WaitForSecondsRealtime(0.06f);
            Time.timeScale = 1;
        }
        Destroy(Instantiate(dieEffect, transform.position, Quaternion.identity), 3f);
        AudioManager.instance.PlaySound(transform.position, 2, Random.Range(1f, 1.1f), 0.5f);
        for (int i = 0; i < goodsCount; i++)
        {
            Instantiate(goodsPrefab, transform.position, Quaternion.identity);
        }
        Destroy(gameObject);

    }

    IEnumerator HitMaterial()
    {
        sprite.material = hitMaterial;
        yield return new WaitForSeconds(0.2f);
        sprite.material = originalMaterial;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ScanSpere"))
        {
            if (weakness != null)
            {
                weakness.SetActive(true);
                currentScanSphere = other.gameObject;  // 현재 감지된 ScanSpere 저장
            }
        }
    }
}
