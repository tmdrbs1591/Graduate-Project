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
        private bool isRightClickHeld = false; // ��Ŭ�� ���� Ȯ�� ����
        private bool isLeftClickHeld = false;  // ��Ŭ�� ���� Ȯ�� ����

        void Update()
        {
            // ��Ŭ���� ������ ��
            if (Input.GetMouseButtonDown(1))
            {
                isRightClickHeld = true; // ��Ŭ���� ���� ���·� ����

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

            // ��Ŭ���� ������ �� (��Ŭ���� ���� ���¿����� scanTrail Ȱ��ȭ)
            if (isRightClickHeld && Input.GetMouseButtonDown(0))
            {
                isLeftClickHeld = true;  // ��Ŭ�� ���� ����
                scanTrail.SetActive(true);
            }

            // ��Ŭ���� ���� �� scanTrail�� ��Ȱ��ȭ
            if (Input.GetMouseButtonUp(0))
            {
                isLeftClickHeld = false;
                scanTrail.SetActive(false);
            }

            // ��Ŭ���� ���� �� ��� ȿ�� ��Ȱ��ȭ
            if (Input.GetMouseButtonUp(1))
            {
                isRightClickHeld = false; // ��Ŭ�� ����
                isLeftClickHeld = false;  // ��Ŭ���� ����
                scanVolume.SetActive(false);
                scanSpere.SetActive(false);
                scanTrail.SetActive(false);

                Time.timeScale = 1f;


                GameManager.instance.isScan = false;



            }
        }

    }
}
