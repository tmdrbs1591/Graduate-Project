using UnityEngine;

namespace INab.WorldScanFX
{
    [RequireComponent(typeof(ScanFXBase))]
    public class ScanControl : MonoBehaviour
    {
        // Reference to the ScanFXBase component
        public ScanFXBase scanFX;

        public GameObject scanVolume;
        private void OnEnable()
        {
            scanFX = GetComponent<ScanFXBase>();
        }

        // Update is called once per frame
        void Update()
        {
            // Start a new scan when the N key is pressed
            if (Input.GetMouseButtonDown(1))
            {

                // Ensure scanFX reference is not null
                if (scanFX != null)
                {
                    // Check if there are any scans left
                    if (scanFX.ScansLeft > 0)
                    {
                        // Warn the user if scans are still active
                        Debug.LogWarning("There are " + scanFX.ScansLeft + " scans left. You need to wait for the last scan to end until you can start a new one.");
                    }
                    else
                    {
                        scanVolume.SetActive(true);
                        AudioManager.instance.PlaySound(transform.position, 4, Random.Range(1.4f, 1.4f), 1f);
                        // Pass scan origin properties and start a new scan
                        scanFX.PassScanOriginProperties();
                        scanFX.StartScan(1);
                    }
                }
            }
            else if (Input.GetMouseButtonUp(1))
            {
                scanVolume.SetActive(false);
            }
        }
    }
}
