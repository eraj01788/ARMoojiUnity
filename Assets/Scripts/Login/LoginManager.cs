using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Google;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Text;
using AppleAuth.Enums;
using AppleAuth;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using Firebase.Database;

public class LoginManager : MonoBehaviour
{
    public FirebaseAuth auth;
    public FirebaseUser user;
    AppleAuthManager appleAuthManager;
    public Image ProfileImage;
    public TMP_Text UserName, LoginMethod;
    public GameObject LoginSignUpPanel, LoginPanel, SignUpPanel,LoadingPanel;
    public DatabaseReference reference;
    public UserDataManager dataManager;

    private void Start()
    {
        dataManager = FindAnyObjectByType<UserDataManager>();
        if (AppleAuthManager.IsCurrentPlatformSupported)
        {
            // Creates a default JSON deserializer, to transform JSON Native responses to C# instances
            var deserializer = new PayloadDeserializer();
            // Creates an Apple Authentication manager with the deserializer
            appleAuthManager = new AppleAuthManager(deserializer);
        }

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
            reference = FirebaseDatabase.DefaultInstance.RootReference;
            if (task.Result == DependencyStatus.Available)
            {
                if (auth.CurrentUser != null)
                {
                    // The user is already signed in. You can perform actions here.
                    user = auth.CurrentUser;
                    UserName.text = user.DisplayName;
                    LoginMethod.text = PlayerPrefs.GetString("LoginMethod");
                    dataManager.enabled = true;
                    if (PlayerPrefs.GetString("LoginMethod")=="Google")
                    {
                        if (ProfileImage != null && !string.IsNullOrEmpty(user.PhotoUrl.ToString()))
                        {
                            StartCoroutine(LoadProfileImage(user.PhotoUrl.ToString()));
                            print("Yes");
                        }
                    }
                    else if(PlayerPrefs.GetString("LoginMethod") == "Anonymous")
                    {
                        UserName.text = "Anonymous";
                        SignUpPanel.SetActive(false);
                        LoginPanel.SetActive(true);
                    }
                }
                else
                {
                    // The user is not signed in.
                    SignUpPanel.SetActive(true);
                    LoginPanel.SetActive(false);
                    Debug.Log("User is not signed in.");
                }
            }
        });

    }

    public void AuthSignOut()
    {
        auth.SignOut();
        SignUpPanel.SetActive(true);
        LoginPanel.SetActive(false);
    }
    //LoginAnonymusly
    public void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                return;
            }

            // Extract the user information from the AuthResult
            AuthResult authResult = task.Result;
            user = authResult.User;
            UserName.text = "Anonymous";
            PlayerPrefs.SetString("LoginMethod", "Anonymous");
            LoginMethod.text = PlayerPrefs.GetString("LoginMethod");
            User newUser = new User(user.UserId.ToString(), user.DisplayName,true);
            AddUserToDatabase(newUser);
            SignUpPanel.SetActive(false);
            LoginPanel.SetActive(true);
        });
    }
    //LoginWithGoogle
   

    //LoginWithApple

    private string GenerateRandomString(int length)
    {
        if (length <= 0)
        {
            throw new Exception("Expected nonce to have positive length");
        }

        const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
        var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
        var result = string.Empty;
        var remainingLength = length;

        var randomNumberHolder = new byte[1];
        while (remainingLength > 0)
        {
            var randomNumbers = new List<int>(16);
            for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
            {
                cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
                randomNumbers.Add(randomNumberHolder[0]);
            }

            for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
            {
                if (remainingLength == 0)
                {
                    break;
                }

                var randomNumber = randomNumbers[randomNumberIndex];
                if (randomNumber < charset.Length)
                {
                    result += charset[randomNumber];
                    remainingLength--;
                }
            }
        }

        return result;
    }

    private string GenerateSHA256NonceFromRawNonce(string rawNonce)
    {
        var sha = new SHA256Managed();
        var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
        var hash = sha.ComputeHash(utf8RawNonce);

        var result = string.Empty;
        for (var i = 0; i < hash.Length; i++)
        {
            result += hash[i].ToString("x2");
        }

        return result;
    }

    public void LoginWithApple()
    {
        PerformLoginWithAppleIdAndFirebase(user =>
        {
            UserName.text = user.DisplayName;
            PlayerPrefs.SetString("LoginMethod", "Apple");
            LoginMethod.text = PlayerPrefs.GetString("LoginMethod");
            SignUpPanel.SetActive(false);
            LoginPanel.SetActive(true);
            User newUser = new User(user.UserId.ToString(), user.DisplayName,true);
            AddUserToDatabase(newUser);
        });
    }

    public void PerformLoginWithAppleIdAndFirebase(Action<FirebaseUser> firebaseAuthCallback)
    {
        var rawNonce = GenerateRandomString(32);
        var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);

        var loginArgs = new AppleAuthLoginArgs(
            LoginOptions.IncludeEmail | LoginOptions.IncludeFullName,
            nonce);

            appleAuthManager.LoginWithAppleId(
            loginArgs,
            credential =>
            {
                var appleIdCredential = credential as IAppleIDCredential;
                if (appleIdCredential != null)
                {
                    PerformFirebaseAuthentication(appleIdCredential, rawNonce, firebaseAuthCallback);
                }
            },
            error =>
            {
                // Something went wrong
            });
    }


    private void PerformFirebaseAuthentication(
    IAppleIDCredential appleIdCredential,
    string rawNonce,
    Action<FirebaseUser> firebaseAuthCallback)
    {
        var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
        var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
        var firebaseCredential = OAuthProvider.GetCredential(
            "apple.com",
            identityToken,
            rawNonce,
            authorizationCode);

        auth.SignInWithCredentialAsync(firebaseCredential)
            .ContinueWithOnMainThread(task => HandleSignInWithUser(task, firebaseAuthCallback));
    }

    private static void HandleSignInWithUser(Task<FirebaseUser> task, Action<FirebaseUser> firebaseUserCallback)
    {
        if (task.IsCanceled)
        {
            Debug.Log("Firebase auth was canceled");
            firebaseUserCallback(null);
        }
        else if (task.IsFaulted)
        {
            Debug.Log("Firebase auth failed");
            firebaseUserCallback(null);
        }
        else
        {
            var firebaseUser = task.Result;
            Debug.Log("Firebase auth completed | User ID:" + firebaseUser.UserId);
            firebaseUserCallback(firebaseUser);
        }
    }

    private IEnumerator LoadProfileImage(string url)
    {
        LoadingPanel.SetActive(true);
        using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                // Set the profile image texture
                Texture2D texture = DownloadHandlerTexture.GetContent(www);
                ProfileImage.sprite = Sprite.Create(
                    texture,
                    new Rect(0, 0, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f)
                );
                SignUpPanel.SetActive(false);
                LoginPanel.SetActive(true);
                LoadingPanel.SetActive(false);
            }
            else
            {
                Debug.LogError("Failed to load profile image: " + www.error);
            }
        }
    }

    public void AddUserToDatabase(User newuser)
    {
        // Convert the User object to a serializable format, such as a dictionary.
        Dictionary<string, object> userDict = new Dictionary<string, object>
        {
            { "ID", newuser.userID },
            { "Name", newuser.Name },
            { "IsActive", newuser.IsActive },
        };

        string userId = newuser.userID.ToString();
        reference.Child("users").Child(userId).SetValueAsync(userDict).ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                Debug.Log("User added to Firebase Realtime Database.");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("Error adding user to Firebase Realtime Database: " + task.Exception);
            }
        });
    }
}
