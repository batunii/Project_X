using UnityEngine;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections;

//TODO: Implement Guest User 

public class CloudLogger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool enableCloudLogging = true;
    [SerializeField] private bool logForGuestUsers = false; // Don't log for guests by default
    [SerializeField] private int maxQueueSize = 100;
    [SerializeField] private float authCheckInterval = 0.5f;

    private Queue<LogEntry> logQueue = new Queue<LogEntry>();
    private bool isAuthenticated = false;
    private bool isGuestUser = false;
    private bool isProcessingQueue = false;

    private struct LogEntry
    {
        public string message;
        public string logType;
        public string timestamp;
    }

    async void Start()
    {
        try
        {
            // Initialize Unity Services
            await UnityServices.InitializeAsync();
            
            // Hook into Unity's log system
            Application.logMessageReceived += HandleLog;
            
            // Start checking for authentication
            StartCoroutine(MonitorAuthentication());
        }
        catch (System.Exception e)
        {
            Debug.LogError($"CloudLogger initialization failed: {e.Message}");
        }
    }

    void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private IEnumerator MonitorAuthentication()
    {
        while (true)
        {
            // Check if authenticated
            bool wasAuthenticated = isAuthenticated;
            
            if (AuthenticationService.Instance != null && 
                AuthenticationService.Instance.IsSignedIn)
            {
                isAuthenticated = true;
                
                // Check if user is a guest
                // Guest users typically have a PlayerId but may have limited permissions
                // You might need to adjust this check based on how your system identifies guests
                string playerId = AuthenticationService.Instance.PlayerId;
                
                // Common ways to detect guest users:
                // 1. Check if they're signed in anonymously
                // 2. Check a custom player profile field
                // 3. Check if they have an external ID (non-guests usually do)
                
                // For now, we'll assume guests are anonymous sign-ins
                // Adjust this logic based on your authentication system
                isGuestUser = string.IsNullOrEmpty(AuthenticationService.Instance.Profile);
            }
            else
            {
                isAuthenticated = false;
                isGuestUser = false;
            }

            // If just became authenticated (and not a guest or logging for guests), process queued logs
            if (isAuthenticated && !wasAuthenticated && 
                (logForGuestUsers || !isGuestUser) && 
                !isProcessingQueue)
            {
                StartCoroutine(ProcessLogQueue());
            }

            // Clear queue if user is a guest and we don't log for guests
            if (isAuthenticated && isGuestUser && !logForGuestUsers && logQueue.Count > 0)
            {
                Debug.Log($"[CloudLogger] Clearing {logQueue.Count} queued logs - guest user detected");
                logQueue.Clear();
            }

            yield return new WaitForSeconds(authCheckInterval);
        }
    }

    private IEnumerator ProcessLogQueue()
    {
        isProcessingQueue = true;

        while (logQueue.Count > 0 && isAuthenticated && (logForGuestUsers || !isGuestUser))
        {
            LogEntry entry = logQueue.Dequeue();
            
            // Send without waiting to avoid blocking
            _ = SendLogToCloudCode(entry.message, entry.logType, entry.timestamp);
            
            // Small delay between sends to avoid rate limiting
            yield return new WaitForSeconds(0.1f);
        }

        isProcessingQueue = false;
    }

    private async void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (!enableCloudLogging)
            return;

        // Create log entry with timestamp
        LogEntry entry = new LogEntry
        {
            message = logString,
            logType = type.ToString(),
            timestamp = System.DateTime.UtcNow.ToString("o")
        };

        // Skip logging if user is a guest and we don't log for guests
        if (isAuthenticated && isGuestUser && !logForGuestUsers)
        {
            return;
        }

        // If not authenticated, queue the log
        if (!isAuthenticated)
        {
            if (logQueue.Count < maxQueueSize)
            {
                logQueue.Enqueue(entry);
            }
            else
            {
                // Queue is full, drop oldest log
                logQueue.Dequeue();
                logQueue.Enqueue(entry);
            }
            return;
        }

        // If authenticated (and allowed to log), send immediately
        await SendLogToCloudCode(entry.message, entry.logType, entry.timestamp);
    }

    private async Task SendLogToCloudCode(string message, string logType, string timestamp)
    {
        // Check authentication and guest status before sending
        if (!isAuthenticated || 
            AuthenticationService.Instance == null || 
            !AuthenticationService.Instance.IsSignedIn ||
            (isGuestUser && !logForGuestUsers))
        {
            return;
        }

        try
        {
            var args = new Dictionary<string, object>
            {
                { "message", message },
                { "type", logType },
                { "timestamp", timestamp },
                { "userId", AuthenticationService.Instance.PlayerId }
            };

            await CloudCodeService.Instance.CallEndpointAsync(
                "gamelogging",
                args
            );
        }
        catch (System.Exception e)
        {
            // Don't use Debug.LogError here to avoid infinite loop
            // Check if it's a permission error (common for guest users)
            if (e.Message.Contains("AccessTokenMissing") || 
                e.Message.Contains("Unauthorized") ||
                e.Message.Contains("permission"))
            {
                Debug.LogWarning($"[CloudLogger] Cloud logging unavailable for current user type");
                // Disable cloud logging to prevent repeated errors
                enableCloudLogging = false;
            }
            else
            {
                Debug.LogWarning($"[CloudLogger] Failed to send log: {e.Message}");
            }
        }
    }

    // Public method to manually clear the queue if needed
    public void ClearLogQueue()
    {
        logQueue.Clear();
    }

    // Public method to get queue status
    public int GetQueuedLogCount()
    {
        return logQueue.Count;
    }

    // Public method to check current status
    public string GetStatus()
    {
        return $"Authenticated: {isAuthenticated}, Guest: {isGuestUser}, Queue: {logQueue.Count}, Enabled: {enableCloudLogging}";
    }
}