using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class LoginController : MonoBehaviour
{
    [Header("Panels")] [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject registerPanel, profilePanel, resetPasswordPanel;

    [Header("Login")] [SerializeField] TMP_InputField username;
    [SerializeField] TMP_InputField password;

    [Header("Register")] [SerializeField] TMP_InputField usernameReg;
    [SerializeField] TMP_InputField emailReg, passwordReg, verifyPasswordReg;
    [SerializeField] TextMeshProUGUI status;

    [Header("Reset Password")] [SerializeField]
    TMP_InputField emailReset;
    [SerializeField] TextMeshProUGUI message;

    [Header("Profile")] [SerializeField] TextMeshProUGUI usernameProfile;

    delegate void Function();
    Function gamePage, nationSelection;
    public void CallLogin()
    {
        if (CheckForNotEmpty(username.text, password.text))
        {
            StartCoroutine(Login()); 
        }
        else Debug.Log("Fields cannot be empty!");
    }

    public void CallRegister()
    {
        if (CheckForNotEmpty(usernameReg.text, emailReg.text, passwordReg.text, verifyPasswordReg.text))
        {
            StartCoroutine(Register()); 
        }
    }

    public void CallResetPassword()
    {
        StartCoroutine(ResetPassword());
    }

    public void CallLogout()
    {
        StartCoroutine(Logout());
    }

    public void OpenRegister()
    {
        usernameReg.gameObject.SetActive(true);
        emailReg.gameObject.SetActive(true);
        passwordReg.gameObject.SetActive(true);
        verifyPasswordReg.gameObject.SetActive(true);
        registerPanel.SetActive(true);
        loginPanel.SetActive(false);
        profilePanel.SetActive(false);
    }

    public void OpenLogin()
    {
        registerPanel.SetActive(false);
        loginPanel.SetActive(true);
        profilePanel.SetActive(false);
        resetPasswordPanel.SetActive(false);
        message.text = "";
    }

    public void OpenReset()
    {
        registerPanel.SetActive(false);
        loginPanel.SetActive(false);
        profilePanel.SetActive(false);
        resetPasswordPanel.SetActive(true);
    }

    void OpenProfile()
    {
        gamePage = HelperFunctions.GoToGamePage;
        nationSelection = HelperFunctions.GoToNationSelection;
        var swap = GameManager.instance.player.accountSetup ? gamePage : nationSelection;
        swap();
    }

    IEnumerator Login()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username.text);
        form.AddField("password", password.text);
        form.AddField("login", "login");
        GameManager.instance.player.user = username.text;
        WWW www = new WWW("http://nationhoodgame.com/login.php", form);
        
        yield return www;

        string[] errorResults = www.text.Split('\t');
        foreach (var result in errorResults)
        {
            Debug.Log(result);
        }

        if (int.Parse(errorResults[1]) == 5)
        {
            Debug.Log("User Logged in.");
            OpenProfile();
        }
        else
        {
            Debug.Log("Login Failed. Error #" + errorResults[1]);
        }
    }

    IEnumerator Register()
    {
        WWWForm form = new WWWForm();
        form.AddField("username", usernameReg.text);
        form.AddField("email", emailReg.text);
        form.AddField("password", passwordReg.text);
        form.AddField("register", "register");
        WWW www = new WWW("http://nationhoodgame.com/index.php", form);
        yield return www;

        status.gameObject.SetActive(true);

        if (www.error == null)
        {
            Debug.Log("User is registered");
            status.text = "Account Registered, Verification Email Sent";
            usernameReg.gameObject.SetActive(false);
            emailReg.gameObject.SetActive(false);
            passwordReg.gameObject.SetActive(false);
            verifyPasswordReg.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Registration Failed");
            status.text = "Registration Failed";
            usernameReg.gameObject.SetActive(false);
            emailReg.gameObject.SetActive(false);
            passwordReg.gameObject.SetActive(false);
            verifyPasswordReg.gameObject.SetActive(false);
        }
    }

    IEnumerator ResetPassword()
    {
        WWWForm form = new WWWForm();
        form.AddField("email", emailReset.text);
        form.AddField("reset", "reset");
        WWW www = new WWW("http://nationhoodgame.com/index.php", form);
        yield return www;

        if (www.error == null)
        {
            Debug.Log("Reset Link Sent");
            message.text = "Reset Link Sent";
        }
        else
        {
            Debug.Log("Reset Failed. Error #" + www.text);
            message.text = "Account not Found";
        }
    }

    IEnumerator Logout()
    {
        WWWForm form = new WWWForm();
        form.AddField("logout", "logout");
        WWW www = new WWW("http://nationhoodgame.com/index.php", form);
        yield return www;
        OpenLogin();
        GameManager.instance.player.user = "";
    }

    bool CheckForNotEmpty(params string[] fields)
    {
        foreach (var field in fields)
        {
            if (field == "")
            {
                return false;
            }
        }
        return true;
    }
}
