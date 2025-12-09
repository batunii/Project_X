using UnityEngine;

public class HUD_Component : MonoBehaviour
{
    [SerializeField] protected bool showDebugLogs = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        if (showDebugLogs)
            Debug.Log($"[{GetType().Name}] Component started!");
    }

    protected virtual void OnEnable()
    {
        if (showDebugLogs)
            Debug.Log($"[{GetType().Name}] Component activated!");
    }

    protected virtual void OnDisable()
    {
        if (showDebugLogs)
            Debug.Log($"[{GetType().Name}] Component deactivated!");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
