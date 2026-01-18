using UnityEngine;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    private static DatabaseManager instance;
    private string connectionString;
    private const string DATABASE_NAME = "users.db";

    public static DatabaseManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject dbObject = new GameObject("DatabaseManager");
                instance = dbObject.AddComponent<DatabaseManager>();
                DontDestroyOnLoad(dbObject);
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeDatabase();
    }

    public void InitializeDatabase()
    {
        string dbPath = Path.Combine(Application.persistentDataPath, DATABASE_NAME);
        connectionString = "URI=file:" + dbPath;

        using (var connection = new SqliteConnection(connectionString))
        {
            connection.Open();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT UNIQUE NOT NULL,
                        Password TEXT NOT NULL
                    )";
                command.ExecuteNonQuery();
            }

            connection.Close();
        }

        Debug.Log("Database initialized at: " + dbPath);
    }

    public bool RegisterUser(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Username and password cannot be empty");
            return false;
        }

        if (password.Length < 8)
        {
            Debug.LogError("Password must be at least 8 characters long");
            return false;
        }

        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "INSERT INTO Users (Username, Password) VALUES (@username, @password)";
                    command.Parameters.AddWithValue("@username", username);
                    command.Parameters.AddWithValue("@password", password);
                    command.ExecuteNonQuery();
                }

                connection.Close();
            }

            Debug.Log("User registered successfully: " + username);
            return true;
        }
        catch (SqliteException ex)
        {
            if (ex.Message.Contains("UNIQUE constraint failed"))
            {
                Debug.LogError("User already exists");
                return false;
            }
            Debug.LogError("Error registering user: " + ex.Message);
            return false;
        }
    }

    public bool LoginUser(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            Debug.LogError("Username and password cannot be empty");
            return false;
        }

        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT Password FROM Users WHERE Username = @username";
                    command.Parameters.AddWithValue("@username", username);

                    object result = command.ExecuteScalar();

                    if (result == null)
                    {
                        Debug.LogError("User not found");
                        return false;
                    }

                    string storedPassword = result.ToString();

                    if (storedPassword == password)
                    {
                        Debug.Log("Successful login: " + username);
                        return true;
                    }
                    else
                    {
                        Debug.LogError("Password incorrect");
                        return false;
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error during login: " + ex.Message);
            return false;
        }
    }

    public bool UserExists(string username)
    {
        try
        {
            using (var connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @username";
                    command.Parameters.AddWithValue("@username", username);

                    int count = (int)(long)command.ExecuteScalar();
                    return count > 0;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error checking user: " + ex.Message);
            return false;
        }
    }
}
