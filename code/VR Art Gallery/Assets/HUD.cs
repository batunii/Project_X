using UnityEngine;
using VRGallery.UI;

namespace VRGallery.UI
{
    public class HUD : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            Debug.Log("[INFO]::HUD has been started!");

            // Ensure HUDManager exists
            if (HUDManager.Instance == null)
            {
                Debug.LogWarning("[HUD] HUDManager not found in scene!");
            }
            else
            {
                Debug.Log("[HUD] HUDManager found successfully!");
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}