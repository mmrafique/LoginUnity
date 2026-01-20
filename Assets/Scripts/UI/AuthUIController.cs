using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class AuthUIController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject registerPanel;

    [Header("Login UI")]
    [SerializeField] private TMP_InputField loginUsername;
    [SerializeField] private TMP_InputField loginPassword;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button goRegisterButton;

    [Header("Register UI")]
    [SerializeField] private TMP_InputField registerUsername;
    [SerializeField] private TMP_InputField registerPassword;
    [SerializeField] private TMP_InputField registerConfirm;
    [SerializeField] private Button registerNowButton;
    [SerializeField] private Button backButton;

    [Header("Popup")]
    [SerializeField] private GameObject errorPopupObject;

    private DatabaseManager dbManager;
    private ErrorPopup errorPopup;

    private void Start()
    {
        dbManager = DatabaseManager.Instance;
        if (errorPopupObject != null)
        {
            errorPopup = errorPopupObject.GetComponent<ErrorPopup>();
        }

        ShowLogin();

        loginButton.onClick.AddListener(OnLoginClicked);
        goRegisterButton.onClick.AddListener(ShowRegister);
        registerNowButton.onClick.AddListener(OnRegisterClicked);
        backButton.onClick.AddListener(ShowLogin);
    }

    private void ShowLogin()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        ClearLogin();
        ClearRegister();
    }

    private void ShowRegister()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        ClearRegister();
    }

    private void OnLoginClicked()
    {
        if (errorPopup == null) return;

        string u = loginUsername.text;
        string p = loginPassword.text;

        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p))
        {
            errorPopup.ShowError("Please fill in all fields");
            return;
        }

        if (dbManager.LoginUser(u, p))
        {
            PlayerPrefs.SetString("CurrentUser", u);
            SceneManager.LoadScene("DashboardScene");
        }
        else
        {
            errorPopup.ShowError("Invalid username or password");
            loginPassword.text = string.Empty;
        }
    }

    private void OnRegisterClicked()
    {
        if (errorPopup == null) return;

        string u = registerUsername.text;
        string p = registerPassword.text;
        string c = registerConfirm.text;

        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p))
        {
            errorPopup.ShowError("Please fill in all fields");
            return;
        }
        if (p != c)
        {
            errorPopup.ShowError("Passwords do not match");
            return;
        }
        if (p.Length < 8)
        {
            errorPopup.ShowError("Password must be at least 8 characters long");
            return;
        }

        if (dbManager.RegisterUser(u, p))
        {
            errorPopup.ShowError("Registration successful. Returning to login...");
            Invoke(nameof(ShowLogin), 1.5f);
        }
        else
        {
            errorPopup.ShowError("User already exists or error during registration");
        }
    }

    private void ClearLogin()
    {
        loginUsername.text = string.Empty;
        loginPassword.text = string.Empty;
    }

    private void ClearRegister()
    {
        registerUsername.text = string.Empty;
        registerPassword.text = string.Empty;
        registerConfirm.text = string.Empty;
    }
}
