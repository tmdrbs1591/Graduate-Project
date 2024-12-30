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

    [SerializeField] Material dieMatetrial;
    [SerializeField] Material hitMaterial;
    [SerializeField] Material originalMaterial;

    [SerializeField] GameObject dieEffect;

    
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

        }
    }

    IEnumerator TimeSlow()
    {
        Time.timeScale = 0.2f;
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1;
        Destroy(Instantiate(dieEffect, transform.position, Quaternion.identity), 3f);
        Destroy(gameObject);

    }

    IEnumerator HitMaterial()
    {
        sprite.material = hitMaterial;
        yield return new WaitForSeconds(0.2f);
        sprite.material = originalMaterial;
    }
}
