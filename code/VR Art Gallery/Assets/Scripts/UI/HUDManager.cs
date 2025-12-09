using UnityEngine;
using TMPro;
using XRMultiplayer;
// Remove this: using VRGallery.Authentication;

namespace VRGallery.UI
{
    /// <summary>
    /// Central HUD manager that coordinates all HUD UI components in the VR Gallery.
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        #region Singleton
        public static HUDManager Instance { get; private set; }
        #endregion

        [Header("HUD Components")]
        [SerializeField] private PlayerHudNotification playerNotification;
        [SerializeField] private GreetingBoardUI greetingBoard;
        [SerializeField] private UserStatusDisplay userStatusDisplay;

        [Header("Room Info")]
        [SerializeField] private TMP_Text roomNameText;
        [SerializeField] private TMP_Text roomCodeText;
        [SerializeField] private GameObject roomInfoPanel;

        [Header("User Info")]
        [SerializeField] private TMP_Text userEmailText;
        [SerializeField] private TMP_Text userRoleText;
        [SerializeField] private GameObject userInfoPanel;

        [Header("Connection Status")]
        [SerializeField] private TMP_Text connectionStatusText;
        [SerializeField] private GameObject connectionStatusPanel;

        [Header("Settings")]
        [SerializeField] private bool autoHideRoomInfo = true;
        [SerializeField] private float autoHideDelay = 5f;
        [SerializeField] private bool showDebugInfo = false;

        // Use FULLY QUALIFIED name here
        private VRGallery.Authentication.AuthenticationManager authManager;
        private bool isInitialized = false;

        #region Unity Lifecycle

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            InitializeHUD();
        }

        private void OnDestroy()
        {
            UnsubscribeFromEvents();
        }

        #endregion

        #region Initialization

        private void InitializeHUD()
        {
            Debug.Log("[HUDManager] Initializing HUD...");

            // Get YOUR AuthenticationManager (not XRMultiplayer's)
            authManager = VRGallery.Authentication.AuthenticationManager.Instance;
            if (authManager == null)
            {
                Debug.LogWarning("[HUDManager] AuthenticationManager not found. Some features will be disabled.");
            }

            HideAllPanels();
            SubscribeToEvents();
            UpdateAuthenticationStatus();
            UpdateNetworkStatus();

            isInitialized = true;
            Debug.Log("[HUDManager] HUD initialized successfully!");
        }

        private void SubscribeToEvents()
        {
            if (authManager != null)
            {
                authManager.OnUserLoggedIn += HandleUserLoggedIn;
                authManager.OnUserLoggedOut += HandleUserLoggedOut;
                authManager.OnUserRoleChanged += HandleUserRoleChanged;
                authManager.OnAuthenticationError += HandleAuthenticationError;
            }

            if (XRINetworkGameManager.Instance != null)
            {
                XRINetworkGameManager.Connected?.Subscribe(HandleNetworkConnected);
                XRINetworkGameManager.ConnectedRoomName?.Subscribe(HandleRoomNameChanged);
                XRINetworkGameManager.CurrentConnectionState?.Subscribe(HandleConnectionStateChanged);
            }
        }

        private void UnsubscribeFromEvents()
        {
            if (authManager != null)
            {
                authManager.OnUserLoggedIn -= HandleUserLoggedIn;
                authManager.OnUserLoggedOut -= HandleUserLoggedOut;
                authManager.OnUserRoleChanged -= HandleUserRoleChanged;
                authManager.OnAuthenticationError -= HandleAuthenticationError;
            }

            if (XRINetworkGameManager.Instance != null)
            {
                XRINetworkGameManager.Connected?.Unsubscribe(HandleNetworkConnected);
                XRINetworkGameManager.ConnectedRoomName?.Unsubscribe(HandleRoomNameChanged);
                XRINetworkGameManager.CurrentConnectionState?.Unsubscribe(HandleConnectionStateChanged);
            }
        }

        #endregion

        #region Authentication Event Handlers

        private void HandleUserLoggedIn(Supabase.Gotrue.User user)
        {
            if (userEmailText != null)
                userEmailText.text = $"User: {user.Email}";

            ShowUserInfo();
            ShowNotification($"Welcome, {user.Email}!", 3f);
            UpdateAuthenticationStatus();
        }

        private void HandleUserLoggedOut()
        {
            HideUserInfo();
            ShowNotification("Logged out", 2f);
            UpdateAuthenticationStatus();
        }

        private void HandleUserRoleChanged(VRGallery.Authentication.UserRole role)
        {
            if (userRoleText != null)
                userRoleText.text = $"Role: {role.ToDisplayString()}";
        }

        private void HandleAuthenticationError(string error)
        {
            ShowNotification($"Auth Error: {error}", 4f);
        }

        #endregion

        #region Network Event Handlers

        private void HandleNetworkConnected(bool connected)
        {
            if (connected)
            {
                UpdateRoomInfo();
                if (autoHideRoomInfo)
                {
                    Invoke(nameof(HideRoomInfo), autoHideDelay);
                }
            }
            else
            {
                HideRoomInfo();
            }
        }

        private void HandleRoomNameChanged(string roomName)
        {
            if (roomNameText != null)
                roomNameText.text = roomName;
        }

        private void HandleConnectionStateChanged(XRINetworkGameManager.ConnectionState state)
        {
            UpdateConnectionStatus(state.ToString());
        }

        #endregion

        #region UI Panel Control

        private void HideAllPanels()
        {
            if (roomInfoPanel != null) roomInfoPanel.SetActive(false);
            if (userInfoPanel != null) userInfoPanel.SetActive(false);
            if (connectionStatusPanel != null) connectionStatusPanel.SetActive(false);
        }

        public void ShowRoomInfo()
        {
            if (roomInfoPanel != null)
            {
                roomInfoPanel.SetActive(true);
                UpdateRoomInfo();
            }
        }

        public void HideRoomInfo()
        {
            if (roomInfoPanel != null)
                roomInfoPanel.SetActive(false);
        }

        public void ShowUserInfo()
        {
            if (userInfoPanel != null)
                userInfoPanel.SetActive(true);
        }

        public void HideUserInfo()
        {
            if (userInfoPanel != null)
                userInfoPanel.SetActive(false);
        }

        public void ShowConnectionStatus()
        {
            if (connectionStatusPanel != null)
                connectionStatusPanel.SetActive(true);
        }

        public void HideConnectionStatus()
        {
            if (connectionStatusPanel != null)
                connectionStatusPanel.SetActive(false);
        }

        #endregion

        #region Update Methods

        private void UpdateRoomInfo()
        {
            if (XRINetworkGameManager.Instance != null && XRINetworkGameManager.Connected.Value)
            {
                if (roomNameText != null)
                    roomNameText.text = XRINetworkGameManager.ConnectedRoomName.Value;

                if (roomCodeText != null)
                    roomCodeText.text = XRINetworkGameManager.ConnectedRoomCode;

                ShowRoomInfo();
            }
        }

        private async void UpdateAuthenticationStatus()
        {
            if (authManager == null) return;

            if (authManager.IsAuthenticated && authManager.CurrentUser != null)
            {
                if (userEmailText != null)
                    userEmailText.text = $"User: {authManager.CurrentUser.Email}";

                if (userRoleText != null)
                {
                    var role = await authManager.GetCurrentUserRole();
                    userRoleText.text = $"Role: {role.ToDisplayString()}";
                }
            }
        }

        private void UpdateNetworkStatus()
        {
            if (XRINetworkGameManager.Instance != null)
            {
                var state = XRINetworkGameManager.CurrentConnectionState.Value;
                UpdateConnectionStatus(state.ToString());
            }
        }

        private void UpdateConnectionStatus(string status)
        {
            if (connectionStatusText != null)
            {
                connectionStatusText.text = $"Status: {status}";

                if (showDebugInfo)
                {
                    ShowConnectionStatus();
                }
            }
        }

        #endregion

        #region Public API

        public void ShowNotification(string message, float duration = 3f)
        {
            if (PlayerHudNotification.Instance != null)
            {
                PlayerHudNotification.Instance.ShowText(message, duration);
            }
            else
            {
                Debug.Log($"[HUDManager] Notification: {message}");
            }
        }

        public void ToggleRoomInfo()
        {
            if (roomInfoPanel != null)
            {
                if (roomInfoPanel.activeSelf)
                    HideRoomInfo();
                else
                    ShowRoomInfo();
            }
        }

        public void ToggleUserInfo()
        {
            if (userInfoPanel != null)
            {
                if (userInfoPanel.activeSelf)
                    HideUserInfo();
                else
                    ShowUserInfo();
            }
        }

        public void RefreshAll()
        {
            UpdateAuthenticationStatus();
            UpdateNetworkStatus();
            UpdateRoomInfo();
        }

        #endregion

        #region Debug

        [ContextMenu("Test Notification")]
        private void TestNotification()
        {
            ShowNotification("This is a test notification!", 3f);
        }

        [ContextMenu("Refresh HUD")]
        private void TestRefresh()
        {
            RefreshAll();
        }

        [ContextMenu("Toggle Room Info")]
        private void TestToggleRoomInfo()
        {
            ToggleRoomInfo();
        }

        #endregion
    }
}
