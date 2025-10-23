using ETAG_ERP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace ETAG_ERP.Helpers
{
    public static class SessionManager
    {
        private static User? _currentUser;
        private static DateTime _sessionStartTime;
        private static string _sessionId;
        private static Dictionary<string, object> _sessionData = new Dictionary<string, object>();
        private static readonly string _sessionFilePath = Path.Combine(FileHelper.GetApplicationDataDirectory(), "session.json");

        /// <summary>
        /// Current logged in user
        /// </summary>
        public static User? CurrentUser
        {
            get => _currentUser;
            set
            {
                _currentUser = value;
                if (value != null)
                {
                    _sessionStartTime = DateTime.Now;
                    _sessionId = Guid.NewGuid().ToString();
                    SaveSession();
                }
                else
                {
                    ClearSession();
                }
            }
        }

        /// <summary>
        /// Session start time
        /// </summary>
        public static DateTime SessionStartTime => _sessionStartTime;

        /// <summary>
        /// Session ID
        /// </summary>
        public static string SessionId => _sessionId;

        /// <summary>
        /// Check if user is logged in
        /// </summary>
        public static bool IsLoggedIn => _currentUser != null;

        /// <summary>
        /// Check if current user is admin
        /// </summary>
        public static bool IsAdmin => _currentUser?.IsAdmin ?? false;

        /// <summary>
        /// Get current user role
        /// </summary>
        public static string CurrentUserRole => _currentUser?.Role ?? "";

        /// <summary>
        /// Get current user branch ID
        /// </summary>
        public static int? CurrentUserBranchId => _currentUser?.BranchId;

        /// <summary>
        /// Initialize session manager
        /// </summary>
        public static void Initialize()
        {
            try
            {
                LoadSession();
                ErrorHandler.LogInfo("Session Manager initialized", "Session Manager");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Initialize Session Manager");
            }
        }

        /// <summary>
        /// Login user
        /// </summary>
        public static bool Login(string username, string password)
        {
            try
            {
                var users = DatabaseHelper_Extensions.GetAllUsers();
                var user = users.FirstOrDefault(u => u.Username == username && u.IsActive);

                if (user == null)
                {
                    ErrorHandler.LogWarning($"Login attempt with invalid username: {username}", "Session Manager");
                    return false;
                }

                // Verify password (in real application, use proper password hashing)
                if (user.PasswordHash != HashPassword(password))
                {
                    ErrorHandler.LogWarning($"Login attempt with invalid password for user: {username}", "Session Manager");
                    return false;
                }

                CurrentUser = user;
                PermissionManager.SetCurrentUser(user);
                
                ErrorHandler.LogInfo($"User logged in successfully: {username}", "Session Manager");
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Login");
                return false;
            }
        }

        /// <summary>
        /// Logout current user
        /// </summary>
        public static void Logout()
        {
            try
            {
                if (_currentUser != null)
                {
                    var username = _currentUser.Username;
                    ErrorHandler.LogInfo($"User logged out: {username}", "Session Manager");
                }

                CurrentUser = null;
                PermissionManager.SetCurrentUser(null);
                ClearSession();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Logout");
            }
        }

        /// <summary>
        /// Set session data
        /// </summary>
        public static void SetSessionData(string key, object value)
        {
            try
            {
                _sessionData[key] = value;
                SaveSession();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Set Session Data");
            }
        }

        /// <summary>
        /// Get session data
        /// </summary>
        public static T? GetSessionData<T>(string key)
        {
            try
            {
                if (_sessionData.ContainsKey(key))
                {
                    return (T)_sessionData[key];
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Session Data");
            }

            return default(T);
        }

        /// <summary>
        /// Remove session data
        /// </summary>
        public static void RemoveSessionData(string key)
        {
            try
            {
                if (_sessionData.ContainsKey(key))
                {
                    _sessionData.Remove(key);
                    SaveSession();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Remove Session Data");
            }
        }

        /// <summary>
        /// Clear all session data
        /// </summary>
        public static void ClearSessionData()
        {
            try
            {
                _sessionData.Clear();
                SaveSession();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Clear Session Data");
            }
        }

        /// <summary>
        /// Get session duration
        /// </summary>
        public static TimeSpan GetSessionDuration()
        {
            return DateTime.Now - _sessionStartTime;
        }

        /// <summary>
        /// Check if session is expired
        /// </summary>
        public static bool IsSessionExpired(int timeoutMinutes = 480) // 8 hours default
        {
            return GetSessionDuration().TotalMinutes > timeoutMinutes;
        }

        /// <summary>
        /// Extend session
        /// </summary>
        public static void ExtendSession()
        {
            try
            {
                _sessionStartTime = DateTime.Now;
                SaveSession();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Extend Session");
            }
        }

        /// <summary>
        /// Save session to file
        /// </summary>
        private static void SaveSession()
        {
            try
            {
                if (_currentUser == null)
                    return;

                var sessionInfo = new
                {
                    UserId = _currentUser.Id,
                    Username = _currentUser.Username,
                    SessionId = _sessionId,
                    SessionStartTime = _sessionStartTime,
                    SessionData = _sessionData
                };

                var json = JsonSerializer.Serialize(sessionInfo, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(_sessionFilePath, json);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Save Session");
            }
        }

        /// <summary>
        /// Load session from file
        /// </summary>
        private static void LoadSession()
        {
            try
            {
                if (!File.Exists(_sessionFilePath))
                    return;

                var json = File.ReadAllText(_sessionFilePath);
                var sessionInfo = JsonSerializer.Deserialize<JsonElement>(json);

                if (sessionInfo.TryGetProperty("UserId", out var userIdElement) &&
                    sessionInfo.TryGetProperty("SessionStartTime", out var startTimeElement))
                {
                    var userId = userIdElement.GetInt32();
                    var startTime = startTimeElement.GetDateTime();

                    // Check if session is not expired
                    if (DateTime.Now - startTime < TimeSpan.FromHours(8))
                    {
                        var users = DatabaseHelper_Extensions.GetAllUsers();
                        var user = users.FirstOrDefault(u => u.Id == userId && u.IsActive);

                        if (user != null)
                        {
                            _currentUser = user;
                            _sessionStartTime = startTime;
                            _sessionId = sessionInfo.TryGetProperty("SessionId", out var sessionIdElement) 
                                ? sessionIdElement.GetString() ?? Guid.NewGuid().ToString()
                                : Guid.NewGuid().ToString();

                            // Load session data
                            if (sessionInfo.TryGetProperty("SessionData", out var sessionDataElement))
                            {
                                _sessionData = JsonSerializer.Deserialize<Dictionary<string, object>>(sessionDataElement.GetRawText()) 
                                    ?? new Dictionary<string, object>();
                            }

                            PermissionManager.SetCurrentUser(user);
                            ErrorHandler.LogInfo($"Session restored for user: {user.Username}", "Session Manager");
                        }
                    }
                    else
                    {
                        // Session expired, delete file
                        File.Delete(_sessionFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Load Session");
                // If loading fails, delete the corrupted session file
                try
                {
                    if (File.Exists(_sessionFilePath))
                    {
                        File.Delete(_sessionFilePath);
                    }
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        /// <summary>
        /// Clear session
        /// </summary>
        private static void ClearSession()
        {
            try
            {
                _currentUser = null;
                _sessionStartTime = DateTime.MinValue;
                _sessionId = "";
                _sessionData.Clear();

                if (File.Exists(_sessionFilePath))
                {
                    File.Delete(_sessionFilePath);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Clear Session");
            }
        }

        /// <summary>
        /// Hash password (simple implementation - use proper hashing in production)
        /// </summary>
        private static string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        /// <summary>
        /// Get user activity log
        /// </summary>
        public static void LogUserActivity(string activity)
        {
            try
            {
                if (_currentUser != null)
                {
                    var message = $"User Activity: {_currentUser.Username} - {activity}";
                    ErrorHandler.LogInfo(message, "User Activity");
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Log User Activity");
            }
        }

        /// <summary>
        /// Get session statistics
        /// </summary>
        public static SessionStatistics GetSessionStatistics()
        {
            return new SessionStatistics
            {
                IsLoggedIn = IsLoggedIn,
                Username = _currentUser?.Username ?? "",
                SessionDuration = GetSessionDuration(),
                SessionDataCount = _sessionData.Count,
                IsAdmin = IsAdmin,
                UserRole = CurrentUserRole
            };
        }

        /// <summary>
        /// Validate session
        /// </summary>
        public static bool ValidateSession()
        {
            try
            {
                if (_currentUser == null)
                    return false;

                if (IsSessionExpired())
                {
                    Logout();
                    return false;
                }

                // Check if user still exists and is active
                var users = DatabaseHelper_Extensions.GetAllUsers();
                var user = users.FirstOrDefault(u => u.Id == _currentUser.Id && u.IsActive);

                if (user == null)
                {
                    Logout();
                    return false;
                }

                // Update user data
                _currentUser = user;
                PermissionManager.SetCurrentUser(user);

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Validate Session");
                return false;
            }
        }

        /// <summary>
        /// Force logout all sessions
        /// </summary>
        public static void ForceLogoutAllSessions()
        {
            try
            {
                Logout();
                ErrorHandler.LogInfo("All sessions force logged out", "Session Manager");
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Force Logout All Sessions");
            }
        }

        /// <summary>
        /// Get active sessions count
        /// </summary>
        public static int GetActiveSessionsCount()
        {
            try
            {
                // In a real application, this would check a database or cache
                // For now, return 1 if user is logged in, 0 otherwise
                return IsLoggedIn ? 1 : 0;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Active Sessions Count");
                return 0;
            }
        }
    }

    /// <summary>
    /// Session statistics class
    /// </summary>
    public class SessionStatistics
    {
        public bool IsLoggedIn { get; set; }
        public string Username { get; set; } = "";
        public TimeSpan SessionDuration { get; set; }
        public int SessionDataCount { get; set; }
        public bool IsAdmin { get; set; }
        public string UserRole { get; set; } = "";
    }
}