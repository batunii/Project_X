using UnityEngine;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CloudLogger : MonoBehaviour
{
    private bool isInitialized = false;

    // Make this object persist across scene loads
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    async void Start()
    {
        try
        {
            // Initialize Unity Services (only if not already initialized)
            if (UnityServices.State == ServicesInitializationState.Uninitialized)
            {
                await UnityServices.InitializeAsync();
            }

            // Wait a moment for any other initialization to complete
            await Task.Delay(500);

            // Only sign in if not already signed in
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
                Debug.Log("CloudLogger: Signed in to Unity Auth for Cloud Code access");
            }
            else
            {
                Debug.Log("CloudLogger: Already signed in to Unity Auth");
            }

            isInitialized = true;
            Application.logMessageReceived += HandleLog;
            Debug.Log("CloudLogger initialized successfully");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"CloudLogger init failed: {e.Message}");
        }
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private async void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (!isInitialized) return;
        if (logString.Contains("Cloud Code")) return; // Prevent infinite loop

        await SendLogToCloudCode(logString, type.ToString());
    }

    public async Task SendLogToCloudCode(string message, string logType)
    {
        if (!isInitialized) return;

        try
        {
            var args = new Dictionary<string, object>
            {
                { "message", message },
                { "type", logType }
            };

            await CloudCodeService.Instance.CallEndpointAsync("gamelogging", args);
        }
        catch (System.Exception)
        {
            // Silent fail to avoid spam
        }
    }
}
