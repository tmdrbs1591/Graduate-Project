using UnityEngine;

public class MouseTrail : MonoBehaviour
{
    void Update()
    {
        // 마우스 위치를 가져와서 월드 좌표로 변환
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 6f; // 카메라에서 떨어진 거리 설정 (적절한 값으로 조정)
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);

        // 마우스 위치로 이동
        transform.position = worldPos;
    }
}
