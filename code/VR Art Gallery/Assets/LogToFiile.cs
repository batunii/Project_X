using UnityEngine;
using Unity.Services.Core;
using Unity.Services.CloudCode;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CloudLogForwarder : MonoBehaviour
{
    async void Awake()
    {
        Debug.Log("CloudLogForwarder Awake: initializing Unity Services...");

        await UnityServices.InitializeAsync();

        Debug.Log("CloudLogForwarder: Unity Services initialized, subscribing to logMessageReceived.");
        Application.logMessageReceived += HandleLog;

        // Force a test log to see if pipeline works
        Debug.LogError("TEST ERROR from CloudLogForwarder");
    }

    async void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (logString.StartsWith("CloudLogForwarder"))
            return;
            
        // (Optional) only send errors/exceptions:
        // if (type != LogType.Error && type != LogType.Exception) return;

        Debug.Log($"CloudLogForwarder.HandleLog sending to Cloud Code: [{type}] {logString}");

        try
        {
            var payload = new Dictionary<string, object>
            {
                { "message", logString },
                { "type", type.ToString() }
            };

            await CloudCodeService.Instance.CallEndpointAsync(
                "gamelogging",   // must match script name in dashboard
                payload
            );

            Debug.Log("CloudLogForwarder: Successfully sent log to Cloud Code.");
        }
        catch (CloudCodeException e)
        {
            // Use ErrorCode + Message so we know *why* it failed
            Debug.LogError($"CloudLogForwarder: Cloud Code Exception. ErrorCode={e.ErrorCode}, Message={e.Message}");
        }
        catch (System.Exception e)
        {
            Debug.LogError("CloudLogForwarder: Generic Exception when calling Cloud Code: " + e);
        }

    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
}
