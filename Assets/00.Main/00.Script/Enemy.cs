using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.U2D;

public class Enemy : MonoBehaviour
{
    [Header("½ºÅÝ")]
    [SerializeField] float currentHP;
    [SerializeField] float maxHP;
   public float knockbackPower;

    [Header("¸ÞÅ×¸®¾ó")]
    [SerializeField] Material dieMatetrial;
    [SerializeField] Material hitMaterial;
    [SerializeField] Material originalMaterial;

    [Header("ÀÌÆåÆ®")]
    [SerializeField] GameObject dieEffect;
    [SerializeField] GameObject hitEffect;

    [SerializeField] GameObject goodsPrefab;
    [SerializeField] int goodsCount;

    bool isDie;

    Animator anim;
    SpriteRenderer sprite;

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
        Die();
    }

    public void TakeDamage(float damage)
    {

        Destroy(Instantiate(hitEffect,transform.position,Quaternion.identity),2f);
        AudioManager.instance.PlaySound(transform.position, 1, Random.Range(1f, 1.1f), 0.5f);

        CameraShake.instance.ShakeCamera(5f,0.1f);
        currentHP -= damage;
        anim.SetTrigger("isHit");
        StartCoroutine(HitMaterial());
    }

    void Die()
    {
        if(currentHP <= 0 && !isDie)
        {
            CameraShake.instance.ShakeCamera(6f, 0.2f);
            sprite.material = dieMatetrial;
            isDie = true;
            StartCoroutine(TimeSlow());

            for (int i = 0; i < goodsCount; i++) { 
            Instantiate(goodsPrefab,transform.position, Quaternion.identity);
            }

        }
    }

    IEnumerator TimeSlow()
    {
        if (!GameManager.instance.player.isSkill)
        {
            Time.timeScale = 0.15f;
            yield return new WaitForSecondsRealtime(0.23f);
            Time.timeScale = 1;
        }
        Destroy(Instantiate(dieEffect, transform.position, Quaternion.identity), 3f);
        AudioManager.instance.PlaySound(transform.position, 2, Random.Range(1f, 1.1f), 0.5f);
        Destroy(gameObject);

    }

    IEnumerator HitMaterial()
    {
        sprite.material = hitMaterial;
        yield return new WaitForSeconds(0.2f);
        sprite.material = originalMaterial;
    }
}
