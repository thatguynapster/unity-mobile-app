using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Firebase;
using Firebase.Auth;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using TMPro;

public class AuthManager : MonoBehaviour
{
    public UIManager uIManager;
    // Firebase
    [Header("Friebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser user;

    // Login
    [Space]
    [Header("Login")]
    public InputField loginEmail;
    public InputField loginPassword;

    // Sign Up
    [Space]
    [Header("Registration")]
    public InputField SignupName;
    public InputField signupEmail;
    public InputField signupPassword;
    public InputField signupConfirmPassword;

    // error
    // Sign Up
    [Space]
    [Header("Error")]
    public TextMeshProUGUI errorMessage;

    private void Awake()
    {
        // set password field types to Password
        loginPassword.contentType = InputField.ContentType.Password;
        signupPassword.contentType = InputField.ContentType.Password;
        signupConfirmPassword.contentType = InputField.ContentType.Password;

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all fierbase dependencies: " + dependencyStatus);
            }
        });
    }

    void InitializeFirebase()
    {
        auth = FirebaseAuth.DefaultInstance;

        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null);
    }

    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != user)
        {
            bool signedIn = user != auth.CurrentUser && auth.CurrentUser != null;

            if (!signedIn && user != null)
            {
                Debug.Log("Signed out " + user.UserId);
            }

            user = auth.CurrentUser;

            if (signedIn)
            {
                Debug.Log("Signed in " + user.UserId);
            }
        }
    }

    public void login()
    {
        StartCoroutine(LoginAsync(loginEmail.text, loginPassword.text));
    }

    private IEnumerator LoginAsync(string email, string password)
    {
        var loginTask = auth.SignInWithEmailAndPasswordAsync(email, password);

        yield return new WaitUntil(() => loginTask.IsCompleted);

        if (loginTask.Exception != null)
        {
            Debug.LogError(loginTask.Exception);

            FirebaseException firebaseException = loginTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            string failureMessage = "Login Failed! Reason: ";

            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failureMessage += "Email is invalid";
                    break;
                case AuthError.WrongPassword:
                    failureMessage += "Wrong password";
                    break;
                case AuthError.MissingEmail:
                    failureMessage += "Email is missing";
                    break;
                case AuthError.MissingPassword:
                    failureMessage += "Password is missing";
                    break;
                default:
                    failureMessage = "Login Failed";
                    break;
            }

            errorMessage.text = failureMessage;
        }
        else
        {
            user = loginTask.Result.User;

            Debug.LogFormat("{0} Login successful", user.Email);

            // clear input fields
            clearInput(loginEmail);
            clearInput(loginPassword);

            SceneManager.LoadScene("Ads");
        }
    }

    public void Register()
    {
        StartCoroutine(RegisterAsync(SignupName.text, signupEmail.text, signupPassword.text, signupConfirmPassword.text));
    }

    private IEnumerator RegisterAsync(string name, string email, string password, string confirmPassword)
    {
        if (string.IsNullOrEmpty(name))
        {
            Debug.LogError("Name is empty");
            errorMessage.text = "Name is empty";
            yield break;
        }

        if (string.IsNullOrEmpty(email))
        {
            errorMessage.text = "Email is empty";
            yield break;
        }

        if (IsPasswordValid(password))
        {
            errorMessage.text = "Invalid password provided";
            yield break;
        }

        if (password.Length <= 8)
        {
            errorMessage.text = "Password is too short";
            yield break;
        }

        if (password != confirmPassword)
        {
            errorMessage.text = "Password does not match";
            yield break;
        }

        var registerTask = auth.CreateUserWithEmailAndPasswordAsync(email, password);
        yield return new WaitUntil(() => registerTask.IsCompleted);

        if (registerTask.Exception != null)
        {
            Debug.LogError(registerTask.Exception);

            FirebaseException firebaseException = registerTask.Exception.GetBaseException() as FirebaseException;
            AuthError authError = (AuthError)firebaseException.ErrorCode;

            string failureMessage = "Registration Failed! Reason: ";
            switch (authError)
            {
                case AuthError.InvalidEmail:
                    failureMessage += "Email is invalid";
                    break;
                case AuthError.WrongPassword:
                    failureMessage += "Wrong password";
                    break;
                case AuthError.MissingEmail:
                    failureMessage += "Email is missing";
                    break;
                case AuthError.MissingPassword:
                    failureMessage += "Password is missing";
                    break;
                default:
                    failureMessage = "Registration Failed";
                    break;
            }
            errorMessage.text = failureMessage;
        }
        else
        {
            Debug.Log("Registration successful. Welcome" + user.Email);

            clearInput(SignupName);
            clearInput(signupEmail);
            clearInput(signupPassword);
            clearInput(signupConfirmPassword);

            uIManager.OpenLoginView();
        }
    }

    private bool IsPasswordValid(string password)
    {
        const string pattern = "^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d)(?=.*[@#$%^&+=]).*$";
        return Regex.IsMatch(password, pattern);
    }

    private void clearInput(InputField field)
    {
        field.text = "";
    }
}