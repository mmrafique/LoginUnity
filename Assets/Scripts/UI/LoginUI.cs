using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField usernameInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private GameObject errorPopupObject;

    private DatabaseManager dbManager;
    private ErrorPopup errorPopup;

    private void Start()
    {
        dbManager = DatabaseManager.Instance;
        errorPopup = errorPopupObject.GetComponent<ErrorPopup>();
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
    }

    private void OnLoginButtonClicked()
    {
        string username = usernameInput.text;
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            errorPopup.ShowError("Please fill in all fields");
            return;
        }

        if (dbManager.LoginUser(username, password))
        {
            PlayerPrefs.SetString("CurrentUser", username);
            SceneManager.LoadScene("DashboardScene");
        }
        else
        {
            errorPopup.ShowError("Invalid username or password");
            passwordInput.text = "";
        }
    }

    private void OnRegisterButtonClicked()
    {
        SceneManager.LoadScene("RegisterScene");
    }

    private void ClearFields()
    {
        usernameInput.text = "";
        passwordInput.text = "";
    }
}
