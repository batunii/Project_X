using UnityEngine;

public class CanvasManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        LogDebug("CanvasManager started"); 
    }

    // Update is called once per frame
    void Update()
    {
           
    }

    private void LogDebug(string message)
    {
        Debug.Log($"[CanvasManager] {message}");
    }
}
