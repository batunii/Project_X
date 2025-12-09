using UnityEngine;
using VRGallery.Authentication;
using System.Threading.Tasks;

public class SimpleAuthTest : MonoBehaviour
{
    [Header("Test Settings")]

    [SerializeField] private bool runTestOnStart = true;
    [SerializeField] private string testEmail = "admin@gmail.com";
    [SerializeField] private string testPassword = "";


    private AuthenticationManager authManager;

    async void Start()
    {
        if (!runTestOnStart) return;

        Debug.Log("[SimpleAuthTest] Starting basic authentication test...");

        // Create AuthenticationManager if it doesn't exist
        if (AuthenticationManager.Instance == null)
        {
            Debug.Log("Creating AuthenticationManager...");
            GameObject authManagerGO = new GameObject("AuthenticationManager");
            authManagerGO.AddComponent<VRGallery.Authentication.AuthenticationManager>();
        }

        // Wait for AuthenticationManager to be ready
        while (AuthenticationManager.Instance == null)
        {
            await Task.Delay(100);
        }

        authManager = AuthenticationManager.Instance;

        // Subscribe to basic auth events
        authManager.OnUserLoggedIn += (user) => Debug.Log($"User logged in: {user.Email}");
        authManager.OnUserLoggedOut += () => Debug.Log("User logged out");
        authManager.OnAuthenticationError += (error) => Debug.LogError($"Auth error: {error}");

        await RunBasicAuthTest();
    }

    private async Task RunBasicAuthTest()
    {
        try
        {
            Debug.Log("Testing authentication system...");

            // Test 1: Check current auth state
            Debug.Log($"Current auth state: {authManager.IsAuthenticated}");

            // Test 2: Login
            Debug.Log($"Testing login with: {testEmail}");
            bool loginSuccess = await authManager.LoginUser(testEmail, testPassword);

            if (loginSuccess)
            {
                Debug.Log("Login successful!");
                Debug.Log($"Current user: {authManager.CurrentUser?.Email}");

                // Test 3: Get user role
                var role = await authManager.GetCurrentUserRole();
                Debug.Log($"User role: {role}");

                // Test 4: Test logout
                Debug.Log("Testing logout...");
                bool logoutSuccess = await authManager.LogoutUser();
                Debug.Log($"Logout result: {logoutSuccess}");
            }
            else
            {
                Debug.LogError("Login failed!");
            }

            Debug.Log("Basic authentication test completed!");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Test failed with exception: {ex.Message}");
        }
    }

    // Manual test methods
    [ContextMenu("Test Login")]
    public async void TestLogin()
    {
        if (authManager == null)
        {
            authManager = AuthenticationManager.Instance;
        }

        if (authManager != null)
        {
            Debug.Log("Testing login...");
            bool success = await authManager.LoginUser(testEmail, testPassword);
            Debug.Log($"Login result: {success}");
        }
        else
        {
            Debug.LogError("AuthenticationManager not found!");
        }
    }

    [ContextMenu("Test Logout")]
    public async void TestLogout()
    {
        if (authManager != null && authManager.IsAuthenticated)
        {
            Debug.Log("Testing logout...");
            bool success = await authManager.LogoutUser();
            Debug.Log($"Logout result: {success}");
        }
        else
        {
            Debug.Log("No user to logout or AuthManager not found");
        }
    }

    [ContextMenu("Check Auth Status")]
    public async void CheckAuthStatus()
    {
        if (authManager == null)
        {
            authManager = AuthenticationManager.Instance;
        }

        if (authManager == null)
        {
            Debug.Log("AuthenticationManager not found - creating one...");
            GameObject authManagerGO = new GameObject("AuthenticationManager");
            authManagerGO.AddComponent<VRGallery.Authentication.AuthenticationManager>();

            // Wait a moment for it to initialize
            await Task.Delay(500);
            authManager = AuthenticationManager.Instance;
        }

        if (authManager != null)
        {
            Debug.Log($"Is Authenticated: {authManager.IsAuthenticated}");
            if (authManager.IsAuthenticated)
            {
                Debug.Log($"Current User: {authManager.CurrentUser?.Email}");
                var role = await authManager.GetCurrentUserRole();
                Debug.Log($"User Role: {role}");
            }
        }
        else
        {
            Debug.LogError("Failed to create AuthenticationManager");
        }
    }
}
