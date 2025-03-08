using UnityEngine;

public class MouseTrail : MonoBehaviour
{
    void Update()
    {
        // ���콺 ��ġ�� �����ͼ� ���� ��ǥ�� ��ȯ
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 6f; // ī�޶󿡼� ������ �Ÿ� ���� (������ ������ ����)
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // ���콺 ��ġ�� �̵�
        transform.position = worldPos;
    }
}
