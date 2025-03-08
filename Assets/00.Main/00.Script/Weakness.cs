using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weakness : MonoBehaviour
{
    [SerializeField] GameObject weaknessEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("ScanTrail"))
        {
            // ī�޶� ����
            CameraShake.instance.ShakeCamera(6f, 0.1f);
            AudioManager.instance.PlaySound(transform.position, 5, Random.Range(1f, 1.3f), 0.5f);

            // ����Ʈ ����
            Instantiate(weaknessEffect, transform.position, Quaternion.identity);

            // �θ��� ��ġ�� GameManager�� enemyTransform ����Ʈ�� �߰�
            if (GameManager.instance != null && transform.parent != null)
            {
                GameManager.instance.enemyTransform.Add(transform.parent); // �θ��� ��ġ �߰�
            }

            // ������Ʈ ����
            Destroy(gameObject);
        }
    }
}
