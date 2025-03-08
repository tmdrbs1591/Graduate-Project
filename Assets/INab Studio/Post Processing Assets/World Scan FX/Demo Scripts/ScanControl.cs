using UnityEngine;

namespace INab.WorldScanFX
{
    [RequireComponent(typeof(ScanFXBase))]
    public class ScanControl : MonoBehaviour
    {
        // Reference to the ScanFXBase component
        public ScanFXBase scanFX;

        public GameObject scanVolume;
        public GameObject scanSpere;
        public GameObject scanTrail;
        private void OnEnable()
        {
            scanFX = GetComponent<ScanFXBase>();
        }

        // Update is called once per frame
        private bool isRightClickHeld = false; // 우클릭 상태 확인 변수
        private bool isLeftClickHeld = false;  // 좌클릭 상태 확인 변수

        void Update()
        {
            // 우클릭을 눌렀을 때
            if (Input.GetMouseButtonDown(1))
            {
                isRightClickHeld = true; // 우클릭이 눌린 상태로 변경

                if (scanFX != null)
                {
                    if (scanFX.ScansLeft > 0)
                    {
                        Debug.LogWarning("There are " + scanFX.ScansLeft + " scans left. You need to wait for the last scan to end until you can start a new one.");
                    }
                    else
                    {
                        scanVolume.SetActive(true);
                        scanSpere.SetActive(true);

                        GameManager.instance.isScan = true;

                        Time.timeScale = 0.9f;

                        AudioManager.instance.PlaySound(transform.position, 4, Random.Range(1.4f, 1.4f), 1f);

                        scanFX.PassScanOriginProperties();
                        scanFX.StartScan(1);
                    }
                }
            }

            // 좌클릭을 눌렀을 때 (우클릭이 눌린 상태에서만 scanTrail 활성화)
            if (isRightClickHeld && Input.GetMouseButtonDown(0))
            {
                isLeftClickHeld = true;  // 좌클릭 상태 변경
                scanTrail.SetActive(true);
            }

            // 좌클릭을 뗐을 때 scanTrail을 비활성화
            if (Input.GetMouseButtonUp(0))
            {
                isLeftClickHeld = false;
                scanTrail.SetActive(false);
            }

            // 우클릭을 뗐을 때 모든 효과 비활성화
            if (Input.GetMouseButtonUp(1))
            {
                isRightClickHeld = false; // 우클릭 해제
                isLeftClickHeld = false;  // 좌클릭도 해제
                scanVolume.SetActive(false);
                scanSpere.SetActive(false);
                scanTrail.SetActive(false);

                Time.timeScale = 1f;


                GameManager.instance.isScan = false;



            }
        }

    }
}
