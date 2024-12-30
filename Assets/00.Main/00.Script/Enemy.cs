using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("½ºÅÝ")]
    [SerializeField] float currentHP;
    [SerializeField] float maxHP;

    [SerializeField] GameObject dieEffect;
    bool isDie;

    // Start is called before the first frame update
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
        currentHP -= damage;
    }

    void Die()
    {
        if(currentHP <= 0 && !isDie)
        {
            Destroy(Instantiate(dieEffect,transform.position, Quaternion.identity),3f);
            isDie = true;
            StartCoroutine(TimeSlow());

        }
    }

    IEnumerator TimeSlow()
    {
        Time.timeScale = 0.2f;
        yield return new WaitForSecondsRealtime(0.3f);
        Time.timeScale = 1;
        Destroy(gameObject);

    }
}
