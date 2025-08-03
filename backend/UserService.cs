using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public class UserService
{
    private readonly string _filePath = "users.csv";
    private readonly List<User> _users = new();

    public UserService()
    {
        LoadUsers();
    }

    private void LoadUsers()
    {
        if (!File.Exists(_filePath))
        {
            return;
        }

        var lines = File.ReadAllLines(_filePath);
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length >= 3)
            {
                _users.Add(new User
                {
                    Username = parts[0],
                    Email = parts[1],
                    PasswordHash = parts[2]
                });
            }
        }
    }

    private void SaveUsers()
    {
        var lines = _users.Select(u => $"{u.Username},{u.Email},{u.PasswordHash}");
        File.WriteAllLines(_filePath, lines);
    }

    public bool AddUser(string username, string email, string password)
    {
        if (_users.Any(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase)
                            || string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var hash = HashPassword(password);
        var user = new User
        {
            Username = username,
            Email = email,
            PasswordHash = hash
        };
        _users.Add(user);
        SaveUsers();
        return true;
    }

    public bool VerifyUser(string username, string password)
    {
        var user = _users.FirstOrDefault(u => string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));
        if (user == null)
        {
            return false;
        }

        var hash = HashPassword(password);
        return user.PasswordHash == hash;
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
