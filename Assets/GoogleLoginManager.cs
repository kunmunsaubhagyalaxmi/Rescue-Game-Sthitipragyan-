using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using System.Threading.Tasks;
using TMPro;

public class GoogleLoginManager : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject loginPanel;
    public GameObject playerPanel;
    public GameObject errorPanel;
    public GameObject profileEditPanel; // New panel for profile editing
    
    [Header("Login Panel Elements")]
    public Button signInButton;
    public Button skipButton;
    
    [Header("Player Panel Elements")]
    public Image profileImage; // Profile image component
    public TextMeshProUGUI playerNameText;
    public TextMeshProUGUI playerIdText;
    public Button signOutButton;
    public Button closePlayerPanelButton;
    public Button togglePlayerIdButton; // Button to show/hide player ID
    public Button editProfileButton; // Button to edit profile picture
    
    [Header("Profile Edit Panel Elements")]
    public Image previewProfileImage; // Preview image in edit panel
    public Button leftArrowButton; // Navigate left through default images
    public Button rightArrowButton; // Navigate right through default images
    public Button confirmProfileButton; // Confirm profile change
    public Button cancelProfileButton; // Cancel profile change
    public Button useGoogleImageButton; // Use original Google image
    
    [Header("Error Panel Elements")]
    public TextMeshProUGUI errorMessageText;
    
    [Header("Main Canvas Button")]
    public Button profileButton; // Button in main canvas to open player panel or login
    
    [Header("Default Profile Images")]
    public Sprite[] defaultProfileImages; // Array of default profile images
    
    [Header("Google Sign-In Configuration")]
    private string webClientId = "823402143092-tu9oa42hjnerk902j5bcogmfsa5348sv.apps.googleusercontent.com";
    private FirebaseAuth auth;
    private GoogleSignInConfiguration configuration;
    private bool isSignedIn = false;
    private bool isFirebaseInitialized = false;
    
    // Profile image management
    private Sprite currentGoogleProfileImage;
    private int currentDefaultImageIndex = 0;
    private bool isUsingGoogleImage = true;
    private bool isPlayerIdVisible = false;
    
    // Static variable to track if user has interacted in this session
    private static bool hasUserInteractedThisSession = false;
    
    // PlayerPrefs keys
    private const string PREFS_PLAYER_ID_VISIBLE = "PlayerIdVisible";
    private const string PREFS_USING_GOOGLE_IMAGE = "UsingGoogleImage";
    private const string PREFS_DEFAULT_IMAGE_INDEX = "DefaultImageIndex";
    private const string PREFS_USER_INTERACTED = "UserInteracted";
    private const string PREFS_GOOGLE_IMAGE_DATA = "GoogleImageData";
    private const string PREFS_HAS_GOOGLE_IMAGE = "HasGoogleImage";
    
    void Start()
    {
        // Initialize all panels state
        InitializePanels();
        
        // Initialize Firebase
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {dependencyStatus}");
                ShowError("Signing Failed, Try again!");
            }
        });
        
        // Configure Google Sign-In
        configuration = new GoogleSignInConfiguration
        {
            WebClientId = webClientId,
            RequestIdToken = true,
            RequestEmail = true
        };
        
        // Setup UI button listeners
        SetupButtonListeners();
    }
    
    void InitializePanels()
    {
        // Load saved preferences first
        LoadUserPreferences();
        
        // Hide all panels initially
        if (loginPanel != null) loginPanel.SetActive(false);
        if (playerPanel != null) playerPanel.SetActive(false);
        if (errorPanel != null) errorPanel.SetActive(false);
        if (profileEditPanel != null) profileEditPanel.SetActive(false);
        
        // Initialize player ID visibility
        UpdatePlayerIdVisibility();
        
        // Update profile button text
        UpdateProfileButton();
    }
    
    void SetupButtonListeners()
    {
        Debug.Log("Setting up button listeners...");
        
        // Login panel buttons
        if (signInButton != null)
        {
            signInButton.onClick.RemoveAllListeners(); // Clear existing listeners first
            signInButton.onClick.AddListener(OnSignInButtonClicked);
            Debug.Log("Sign In button listener added");
        }
        
        if (skipButton != null)
        {
            skipButton.onClick.RemoveAllListeners();
            skipButton.onClick.AddListener(OnSkipButtonClicked);
        }
        
        // Player panel buttons
        if (signOutButton != null)
        {
            signOutButton.onClick.RemoveAllListeners();
            signOutButton.onClick.AddListener(OnSignOutButtonClicked);
        }
            
        if (closePlayerPanelButton != null)
        {
            closePlayerPanelButton.onClick.RemoveAllListeners();
            closePlayerPanelButton.onClick.AddListener(ClosePlayerPanel);
        }
            
        if (togglePlayerIdButton != null)
        {
            togglePlayerIdButton.onClick.RemoveAllListeners();
            togglePlayerIdButton.onClick.AddListener(OnTogglePlayerIdClicked);
        }
            
        if (editProfileButton != null)
        {
            editProfileButton.onClick.RemoveAllListeners();
            editProfileButton.onClick.AddListener(OnEditProfileClicked);
        }
        
        // Profile edit panel buttons
        if (leftArrowButton != null)
        {
            leftArrowButton.onClick.RemoveAllListeners();
            leftArrowButton.onClick.AddListener(OnLeftArrowClicked);
            Debug.Log("Left arrow button listener added");
        }
            
        if (rightArrowButton != null)
        {
            rightArrowButton.onClick.RemoveAllListeners();
            rightArrowButton.onClick.AddListener(OnRightArrowClicked);
            Debug.Log("Right arrow button listener added");
        }
            
        if (confirmProfileButton != null)
        {
            confirmProfileButton.onClick.RemoveAllListeners();
            confirmProfileButton.onClick.AddListener(OnConfirmProfileClicked);
        }
            
        if (cancelProfileButton != null)
        {
            cancelProfileButton.onClick.RemoveAllListeners();
            cancelProfileButton.onClick.AddListener(OnCancelProfileClicked);
        }
            
        if (useGoogleImageButton != null)
        {
            useGoogleImageButton.onClick.RemoveAllListeners();
            useGoogleImageButton.onClick.AddListener(OnUseGoogleImageClicked);
        }
        
        // Main canvas profile button
        if (profileButton != null)
        {
            profileButton.onClick.RemoveAllListeners();
            profileButton.onClick.AddListener(OnProfileButtonClicked);
        }
    }
    
    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;
        isFirebaseInitialized = true;
        
        Debug.Log("Firebase initialized successfully");
        
        // Check if user is already signed in
        if (auth.CurrentUser != null)
        {
            isSignedIn = true;
            UpdateProfileButton();
            Debug.Log("User already signed in: " + auth.CurrentUser.Email);
            
            // If the user prefers Google image but we don't have it saved, re-download it
            if (isUsingGoogleImage && currentGoogleProfileImage == null && auth.CurrentUser.PhotoUrl != null)
            {
                StartCoroutine(DownloadProfileImage(auth.CurrentUser.PhotoUrl.ToString()));
            }
            
            // Don't show any panel automatically if already signed in
        }
        else
        {
            isSignedIn = false;
            UpdateProfileButton();
            
            // Only show login panel on first launch, not when switching scenes
            if (!HasUserInteractedThisSession())
            {
                Debug.Log("First time this session - showing login panel");
                ShowLoginPanel();
            }
            else
            {
                Debug.Log("User has interacted this session - not showing login panel");
            }
        }
    }
    
    #region Button Click Handlers
    
    public void OnSignInButtonClicked()
    {
        Debug.Log("Sign In button clicked");
        
        if (!isFirebaseInitialized)
        {
            Debug.LogError("Firebase not initialized yet");
            ShowError("Signing Failed, Try again!");
            return;
        }
        
        SignInWithGoogle();
    }
    
    public void OnSkipButtonClicked()
    {
        Debug.Log("Skip button clicked");
        CloseLoginPanel();
        MarkUserInteraction();
    }
    
    public void OnSignOutButtonClicked()
    {
        Debug.Log("Sign Out button clicked");
        SignOut();
    }
    
    public void OnProfileButtonClicked()
    {
        Debug.Log("Profile button clicked - isSignedIn: " + isSignedIn);
        
        if (isSignedIn)
        {
            ShowPlayerPanel();
        }
        else
        {
            ShowLoginPanel();
        }
    }
    
    public void OnTogglePlayerIdClicked()
    {
        Debug.Log("Toggle Player ID button clicked - current state: " + isPlayerIdVisible);
        isPlayerIdVisible = !isPlayerIdVisible;
        SaveUserPreferences(); // Save the preference
        UpdatePlayerIdVisibility();
        
        // Update the display immediately if user is signed in
        if (isSignedIn && auth != null && auth.CurrentUser != null)
        {
            DisplayUserInfo(auth.CurrentUser);
        }
    }
    
    public void OnEditProfileClicked()
    {
        Debug.Log("Edit Profile button clicked");
        ShowProfileEditPanel();
    }
    
    public void OnLeftArrowClicked()
    {
        Debug.Log("Left arrow button clicked - current index: " + currentDefaultImageIndex);
        if (defaultProfileImages != null && defaultProfileImages.Length > 0)
        {
            currentDefaultImageIndex = (currentDefaultImageIndex - 1 + defaultProfileImages.Length) % defaultProfileImages.Length;
            Debug.Log("New index: " + currentDefaultImageIndex);
            UpdatePreviewImage();
        }
        else
        {
            Debug.LogWarning("No default profile images available");
        }
    }
    
    public void OnRightArrowClicked()
    {
        Debug.Log("Right arrow button clicked - current index: " + currentDefaultImageIndex);
        if (defaultProfileImages != null && defaultProfileImages.Length > 0)
        {
            currentDefaultImageIndex = (currentDefaultImageIndex + 1) % defaultProfileImages.Length;
            Debug.Log("New index: " + currentDefaultImageIndex);
            UpdatePreviewImage();
        }
        else
        {
            Debug.LogWarning("No default profile images available");
        }
    }
    
    public void OnConfirmProfileClicked()
    {
        Debug.Log("Confirm Profile button clicked");
        SaveUserPreferences(); // Save the profile preferences
        ApplySelectedProfileImage();
        CloseProfileEditPanel();
    }
    
    public void OnCancelProfileClicked()
    {
        Debug.Log("Cancel Profile button clicked");
        CloseProfileEditPanel();
    }
    
    public void OnUseGoogleImageClicked()
    {
        Debug.Log("Use Google Image button clicked");
        isUsingGoogleImage = true;
        if (previewProfileImage != null && currentGoogleProfileImage != null)
        {
            previewProfileImage.sprite = currentGoogleProfileImage;
            Debug.Log("Google image applied to preview");
        }
        else
        {
            Debug.LogWarning("Google image not available - currentGoogleProfileImage is null");
        }
    }
    
    #endregion
    
    #region Panel Management
    
    void ShowLoginPanel()
    {
        if (loginPanel != null)
        {
            loginPanel.SetActive(true);
            Debug.Log("Login panel shown");
        }
    }
    
    void CloseLoginPanel()
    {
        if (loginPanel != null)
        {
            loginPanel.SetActive(false);
            Debug.Log("Login panel closed");
        }
    }
    
    void ShowPlayerPanel()
    {
        if (playerPanel != null && auth != null && auth.CurrentUser != null)
        {
            playerPanel.SetActive(true);
            DisplayUserInfo(auth.CurrentUser);
            Debug.Log("Player panel shown");
        }
    }
    
    void ClosePlayerPanel()
    {
        if (playerPanel != null)
        {
            playerPanel.SetActive(false);
            Debug.Log("Player panel closed");
        }
    }
    
    void ShowProfileEditPanel()
    {
        if (profileEditPanel != null)
        {
            profileEditPanel.SetActive(true);
            InitializeProfileEditPanel();
            Debug.Log("Profile edit panel shown");
        }
    }
    
    void CloseProfileEditPanel()
    {
        if (profileEditPanel != null)
        {
            profileEditPanel.SetActive(false);
            Debug.Log("Profile edit panel closed");
        }
    }
    
    void InitializeProfileEditPanel()
    {
        // Set preview to current profile image
        if (previewProfileImage != null)
        {
            if (isUsingGoogleImage && currentGoogleProfileImage != null)
            {
                previewProfileImage.sprite = currentGoogleProfileImage;
                Debug.Log("Preview set to Google image");
            }
            else if (defaultProfileImages != null && defaultProfileImages.Length > 0)
            {
                previewProfileImage.sprite = defaultProfileImages[currentDefaultImageIndex];
                Debug.Log("Preview set to default image at index: " + currentDefaultImageIndex);
            }
        }
    }
    
    void UpdatePreviewImage()
    {
        if (previewProfileImage != null && defaultProfileImages != null && defaultProfileImages.Length > 0)
        {
            previewProfileImage.sprite = defaultProfileImages[currentDefaultImageIndex];
            isUsingGoogleImage = false;
            Debug.Log("Preview image updated to index: " + currentDefaultImageIndex);
        }
    }
    
    void ApplySelectedProfileImage()
    {
        if (profileImage != null)
        {
            if (isUsingGoogleImage && currentGoogleProfileImage != null)
            {
                profileImage.sprite = currentGoogleProfileImage;
                Debug.Log("Applied Google profile image");
            }
            else if (defaultProfileImages != null && defaultProfileImages.Length > 0)
            {
                profileImage.sprite = defaultProfileImages[currentDefaultImageIndex];
                Debug.Log("Applied default profile image at index: " + currentDefaultImageIndex);
            }
        }
    }
    
    void ShowError(string message)
    {
        if (errorPanel != null && errorMessageText != null)
        {
            errorMessageText.text = message;
            errorPanel.SetActive(true);
            Debug.Log("Error shown: " + message);
            
            // Auto-close error panel after 3 seconds
            StartCoroutine(AutoCloseErrorPanel());
        }
    }
    
    void CloseErrorPanel()
    {
        if (errorPanel != null)
        {
            errorPanel.SetActive(false);
            Debug.Log("Error panel closed");
        }
    }
    
    IEnumerator AutoCloseErrorPanel()
    {
        yield return new WaitForSeconds(3f);
        CloseErrorPanel();
    }
    
    void UpdateProfileButton()
    {
        if (profileButton != null)
        {
            TextMeshProUGUI buttonText = profileButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = isSignedIn ? "Profile" : "Sign In";
                Debug.Log("Profile button updated to: " + buttonText.text);
            }
        }
    }
    
    void UpdatePlayerIdVisibility()
    {
        if (playerIdText != null)
        {
            playerIdText.gameObject.SetActive(isPlayerIdVisible);
            Debug.Log("Player ID visibility set to: " + isPlayerIdVisible);
        }
        
        if (togglePlayerIdButton != null)
        {
            TextMeshProUGUI buttonText = togglePlayerIdButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = isPlayerIdVisible ? "Hide Player ID" : "Show Player ID";
            }
        }
    }
    
    #endregion
    
    #region Google Sign-In Logic
    
    private void SignInWithGoogle()
    {
        Debug.Log("Starting Google Sign-In process...");
        
        try
        {
            GoogleSignIn.Configuration = configuration;
            GoogleSignIn.Configuration.UseGameSignIn = false;
            GoogleSignIn.Configuration.RequestIdToken = true;
            
            Debug.Log("Configuration set, calling Google Sign-In...");
            GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(OnGoogleAuthenticateFinished);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Exception during Google Sign-In: " + e.Message);
            ShowError("Signing Failed, Try again!");
        }
    }
    
    void OnGoogleAuthenticateFinished(Task<GoogleSignInUser> task)
    {
        Debug.Log("Google authentication task completed with status: " + task.Status);
        
        if (task.IsFaulted)
        {
            Debug.LogError("Google Sign-In failed: " + task.Exception);
            if (task.Exception != null)
            {
                foreach (var innerException in task.Exception.InnerExceptions)
                {
                    Debug.LogError("Inner exception: " + innerException.Message);
                }
            }
            ShowError("Signing Failed, Try again!");
            return;
        }
        
        if (task.IsCanceled)
        {
            Debug.Log("Google Sign-In was canceled by user");
            return;
        }
        
        GoogleSignInUser googleUser = task.Result;
        Debug.Log("Google Sign-In successful!");
        Debug.Log("User Email: " + googleUser.Email);
        Debug.Log("User DisplayName: " + googleUser.DisplayName);
        
        // Download profile image if available
        if (!string.IsNullOrEmpty(googleUser.ImageUrl?.ToString()))
        {
            StartCoroutine(DownloadProfileImage(googleUser.ImageUrl.ToString()));
        }
        
        // Create Firebase credential from Google Sign-In
        Credential credential = GoogleAuthProvider.GetCredential(googleUser.IdToken, null);
        Debug.Log("Firebase credential created, attempting Firebase authentication...");
        
        // Sign in to Firebase with the Google credential
        auth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(OnFirebaseAuthFinished);
    }
    
    void OnFirebaseAuthFinished(Task<FirebaseUser> task)
    {
        Debug.Log("Firebase authentication task completed with status: " + task.Status);
        
        if (task.IsFaulted)
        {
            Debug.LogError("Firebase authentication failed: " + task.Exception);
            if (task.Exception != null)
            {
                foreach (var innerException in task.Exception.InnerExceptions)
                {
                    Debug.LogError("Firebase inner exception: " + innerException.Message);
                }
            }
            ShowError("Signing Failed, Try again!");
            return;
        }
        
        if (task.IsCanceled)
        {
            Debug.Log("Firebase authentication was canceled");
            ShowError("Signing Failed, Try again!");
            return;
        }
        
        FirebaseUser firebaseUser = task.Result;
        isSignedIn = true;
        MarkUserInteraction();
        
        Debug.Log("Successfully signed in to Firebase!");
        Debug.Log("Firebase User Display Name: " + firebaseUser.DisplayName);
        Debug.Log("Firebase User Email: " + firebaseUser.Email);
        Debug.Log("Firebase User UID: " + firebaseUser.UserId);
        
        // Download profile image from Firebase if not already downloaded
        if (currentGoogleProfileImage == null && firebaseUser.PhotoUrl != null)
        {
            StartCoroutine(DownloadProfileImage(firebaseUser.PhotoUrl.ToString()));
        }
        
        // Close login panel and show player panel
        CloseLoginPanel();
        ShowPlayerPanel();
        
        // Update profile button
        UpdateProfileButton();
    }
    
    #endregion
    
    #region Profile Image Management
    
    IEnumerator DownloadProfileImage(string imageUrl)
    {
        Debug.Log("Downloading profile image from: " + imageUrl);
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            currentGoogleProfileImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            
            Debug.Log("Profile image downloaded successfully");
            
            // Save the image to PlayerPrefs
            SaveGoogleProfileImage();
            
            // Set the profile image if using Google image
            if (isUsingGoogleImage && profileImage != null)
            {
                profileImage.sprite = currentGoogleProfileImage;
            }
        }
        else
        {
            Debug.LogError("Failed to download profile image: " + request.error);
            // Use default image if available
            if (defaultProfileImages != null && defaultProfileImages.Length > 0 && profileImage != null)
            {
                profileImage.sprite = defaultProfileImages[0];
                isUsingGoogleImage = false;
            }
        }
    }
    
    private void SaveGoogleProfileImage()
    {
        if (currentGoogleProfileImage != null)
        {
            try
            {
                // Convert sprite to PNG bytes
                Texture2D texture = currentGoogleProfileImage.texture;
                byte[] imageData = texture.EncodeToPNG();
                
                // Convert to base64 string and save
                string base64String = System.Convert.ToBase64String(imageData);
                PlayerPrefs.SetString(PREFS_GOOGLE_IMAGE_DATA, base64String);
                PlayerPrefs.SetInt(PREFS_HAS_GOOGLE_IMAGE, 1);
                PlayerPrefs.Save();
                
                Debug.Log("Google profile image saved to PlayerPrefs");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to save Google profile image: " + e.Message);
                PlayerPrefs.SetInt(PREFS_HAS_GOOGLE_IMAGE, 0);
            }
        }
        else
        {
            PlayerPrefs.SetInt(PREFS_HAS_GOOGLE_IMAGE, 0);
        }
    }
    
    private void LoadGoogleProfileImage()
    {
        if (PlayerPrefs.GetInt(PREFS_HAS_GOOGLE_IMAGE, 0) == 1)
        {
            try
            {
                string base64String = PlayerPrefs.GetString(PREFS_GOOGLE_IMAGE_DATA, "");
                if (!string.IsNullOrEmpty(base64String))
                {
                    // Convert base64 string back to texture
                    byte[] imageData = System.Convert.FromBase64String(base64String);
                    Texture2D texture = new Texture2D(2, 2);
                    
                    if (texture.LoadImage(imageData))
                    {
                        currentGoogleProfileImage = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        Debug.Log("Google profile image loaded from PlayerPrefs");
                        
                        // Apply the image if user is using Google image
                        if (isUsingGoogleImage && profileImage != null)
                        {
                            profileImage.sprite = currentGoogleProfileImage;
                        }
                    }
                    else
                    {
                        Debug.LogError("Failed to load image data from PlayerPrefs");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("Failed to load Google profile image: " + e.Message);
                PlayerPrefs.SetInt(PREFS_HAS_GOOGLE_IMAGE, 0);
            }
        }
    }
    
    #endregion
    
    #region User Info Display
    
    void DisplayUserInfo(FirebaseUser user)
    {
        if (user != null)
        {
            Debug.Log("Displaying user info for: " + user.Email);
            Debug.Log("User UID: " + user.UserId);
            Debug.Log("Player ID visible: " + isPlayerIdVisible);
            
            // Update name with length restriction
            if (playerNameText != null)
            {
                string displayName = !string.IsNullOrEmpty(user.DisplayName) ? user.DisplayName : "No Name";
                playerNameText.text = TruncateText(displayName, 15);
                Debug.Log("Player name set to: " + playerNameText.text);
            }
                
            // Update player ID based on visibility setting
            if (playerIdText != null)
            {
                if (isPlayerIdVisible)
                {
                    string uid = "UID: " + TruncateText(user.UserId, 15);
                    playerIdText.text = uid;
                    Debug.Log("Player ID set to: " + uid);
                }
                else
                {
                    playerIdText.text = "";
                    Debug.Log("Player ID hidden");
                }
            }
            
            // Update profile image display
            if (profileImage != null)
            {
                if (isUsingGoogleImage && currentGoogleProfileImage != null)
                {
                    profileImage.sprite = currentGoogleProfileImage;
                }
                else if (defaultProfileImages != null && defaultProfileImages.Length > 0)
                {
                    profileImage.sprite = defaultProfileImages[currentDefaultImageIndex];
                }
            }
        }
    }
    
    string TruncateText(string text, int maxLength)
    {
        if (string.IsNullOrEmpty(text)) return text;
        
        if (text.Length <= maxLength)
            return text;
        
        return text.Substring(0, maxLength) + "...";
    }
    
    #endregion
    
    #region Sign Out
    
    public void SignOut()
    {
        if (auth != null && auth.CurrentUser != null)
        {
            Debug.Log("Signing out user: " + auth.CurrentUser.Email);
            
            auth.SignOut();
            GoogleSignIn.DefaultInstance.SignOut();
            
            isSignedIn = false;
            ClosePlayerPanel();
            UpdateProfileButton();
            
            // Clear player info
            if (playerNameText != null) playerNameText.text = "";
            if (playerIdText != null) playerIdText.text = "";
            if (profileImage != null) profileImage.sprite = null;
            
            // Clear saved Google profile image
            PlayerPrefs.DeleteKey(PREFS_GOOGLE_IMAGE_DATA);
            PlayerPrefs.SetInt(PREFS_HAS_GOOGLE_IMAGE, 0);
            PlayerPrefs.Save();
            
            // Reset profile image data
            currentGoogleProfileImage = null;
            
            Debug.Log("Sign out completed");
        }
    }
    
    #endregion
    
    #region Session Management
    
    private bool HasUserInteractedThisSession()
    {
        // Check both static variable and PlayerPrefs
        if (hasUserInteractedThisSession)
            return true;
            
        return PlayerPrefs.GetInt(PREFS_USER_INTERACTED, 0) == 1;
    }
    
    private void MarkUserInteraction()
    {
        hasUserInteractedThisSession = true;
        PlayerPrefs.SetInt(PREFS_USER_INTERACTED, 1);
        PlayerPrefs.Save();
        Debug.Log("User interaction marked for this session and saved");
    }
    
    #endregion
    
    #region PlayerPrefs Management
    
    private void LoadUserPreferences()
    {
        // Load player ID visibility
        isPlayerIdVisible = PlayerPrefs.GetInt(PREFS_PLAYER_ID_VISIBLE, 0) == 1;
        
        // Load profile image preferences
        isUsingGoogleImage = PlayerPrefs.GetInt(PREFS_USING_GOOGLE_IMAGE, 1) == 1;
        currentDefaultImageIndex = PlayerPrefs.GetInt(PREFS_DEFAULT_IMAGE_INDEX, 0);
        
        // Ensure index is within bounds
        if (defaultProfileImages != null && currentDefaultImageIndex >= defaultProfileImages.Length)
        {
            currentDefaultImageIndex = 0;
        }
        
        // Load the saved Google profile image
        LoadGoogleProfileImage();
        
        Debug.Log($"Loaded preferences - PlayerID visible: {isPlayerIdVisible}, Using Google image: {isUsingGoogleImage}, Default image index: {currentDefaultImageIndex}");
    }
    
    private void SaveUserPreferences()
    {
        PlayerPrefs.SetInt(PREFS_PLAYER_ID_VISIBLE, isPlayerIdVisible ? 1 : 0);
        PlayerPrefs.SetInt(PREFS_USING_GOOGLE_IMAGE, isUsingGoogleImage ? 1 : 0);
        PlayerPrefs.SetInt(PREFS_DEFAULT_IMAGE_INDEX, currentDefaultImageIndex);
        
        // Save the Google profile image
        SaveGoogleProfileImage();
        
        PlayerPrefs.Save();
        
        Debug.Log($"Saved preferences - PlayerID visible: {isPlayerIdVisible}, Using Google image: {isUsingGoogleImage}, Default image index: {currentDefaultImageIndex}");
    }
    
    // Method to clear all saved preferences (useful for testing)
    [System.Obsolete("Only use for testing purposes")]
    public void ClearAllPreferences()
    {
        PlayerPrefs.DeleteKey(PREFS_PLAYER_ID_VISIBLE);
        PlayerPrefs.DeleteKey(PREFS_USING_GOOGLE_IMAGE);
        PlayerPrefs.DeleteKey(PREFS_DEFAULT_IMAGE_INDEX);
        PlayerPrefs.DeleteKey(PREFS_USER_INTERACTED);
        PlayerPrefs.DeleteKey(PREFS_GOOGLE_IMAGE_DATA);
        PlayerPrefs.DeleteKey(PREFS_HAS_GOOGLE_IMAGE);
        PlayerPrefs.Save();
        Debug.Log("All preferences cleared");
    }
    
    #endregion
    
    void OnDestroy()
    {
        // Remove all listeners
        if (signInButton != null)
            signInButton.onClick.RemoveAllListeners();
        if (skipButton != null)
            skipButton.onClick.RemoveAllListeners();
        if (signOutButton != null)
            signOutButton.onClick.RemoveAllListeners();
        if (closePlayerPanelButton != null)
            closePlayerPanelButton.onClick.RemoveAllListeners();
        if (profileButton != null)
            profileButton.onClick.RemoveAllListeners();
        if (togglePlayerIdButton != null)
            togglePlayerIdButton.onClick.RemoveAllListeners();
        if (editProfileButton != null)
            editProfileButton.onClick.RemoveAllListeners();
        if (leftArrowButton != null)
            leftArrowButton.onClick.RemoveAllListeners();
        if (rightArrowButton != null)
            rightArrowButton.onClick.RemoveAllListeners();
        if (confirmProfileButton != null)
            confirmProfileButton.onClick.RemoveAllListeners();
        if (cancelProfileButton != null)
            cancelProfileButton.onClick.RemoveAllListeners();
        if (useGoogleImageButton != null)
            useGoogleImageButton.onClick.RemoveAllListeners();
    }
}