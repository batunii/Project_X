using UnityEngine;
using System.IO;

public class LogToFile : MonoBehaviour
{
    private string logFilePath;
    
    void Start()
    {
        logFilePath = "/home/shrey/game_log.txt";
        Debug.Log("Log file saved at: " + logFilePath);
    }
    
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }
    
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
    
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        using (StreamWriter writer = new StreamWriter(logFilePath, true))
        {
            writer.WriteLine($"[{System.DateTime.Now}] [{type}] {logString}");
        }
    }
}
