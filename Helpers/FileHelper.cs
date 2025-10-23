using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace ETAG_ERP.Helpers
{
    public static class FileHelper
    {
        /// <summary>
        /// Open file dialog and return selected file path
        /// </summary>
        public static string? OpenFileDialog(string filter = "All files (*.*)|*.*", string title = "اختر ملف")
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Filter = filter,
                    Title = title
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    return openFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Open File Dialog");
            }

            return null;
        }

        /// <summary>
        /// Save file dialog and return selected file path
        /// </summary>
        public static string? SaveFileDialog(string filter = "All files (*.*)|*.*", string title = "حفظ ملف", string defaultFileName = "")
        {
            try
            {
                var saveFileDialog = new SaveFileDialog
                {
                    Filter = filter,
                    Title = title,
                    FileName = defaultFileName
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    return saveFileDialog.FileName;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Save File Dialog");
            }

            return null;
        }

        /// <summary>
        /// Open folder dialog and return selected folder path
        /// </summary>
        public static string? OpenFolderDialog(string description = "اختر مجلد")
        {
            try
            {
                var folderDialog = new System.Windows.Forms.FolderBrowserDialog
                {
                    Description = description,
                    UseDescriptionForTitle = true
                };

                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return folderDialog.SelectedPath;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Open Folder Dialog");
            }

            return null;
        }

        /// <summary>
        /// Check if file exists
        /// </summary>
        public static bool FileExists(string filePath)
        {
            try
            {
                return File.Exists(filePath);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Check File Exists");
                return false;
            }
        }

        /// <summary>
        /// Check if directory exists
        /// </summary>
        public static bool DirectoryExists(string directoryPath)
        {
            try
            {
                return Directory.Exists(directoryPath);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Check Directory Exists");
                return false;
            }
        }

        /// <summary>
        /// Create directory if it doesn't exist
        /// </summary>
        public static bool CreateDirectory(string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Create Directory");
                return false;
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        public static bool DeleteFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Delete File");
                return false;
            }
        }

        /// <summary>
        /// Delete directory
        /// </summary>
        public static bool DeleteDirectory(string directoryPath, bool recursive = true)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath, recursive);
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Delete Directory");
                return false;
            }
        }

        /// <summary>
        /// Copy file
        /// </summary>
        public static bool CopyFile(string sourcePath, string destinationPath, bool overwrite = true)
        {
            try
            {
                if (File.Exists(sourcePath))
                {
                    // Create destination directory if it doesn't exist
                    var destinationDir = Path.GetDirectoryName(destinationPath);
                    if (!string.IsNullOrEmpty(destinationDir))
                    {
                        CreateDirectory(destinationDir);
                    }

                    File.Copy(sourcePath, destinationPath, overwrite);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Copy File");
            }

            return false;
        }

        /// <summary>
        /// Move file
        /// </summary>
        public static bool MoveFile(string sourcePath, string destinationPath)
        {
            try
            {
                if (File.Exists(sourcePath))
                {
                    // Create destination directory if it doesn't exist
                    var destinationDir = Path.GetDirectoryName(destinationPath);
                    if (!string.IsNullOrEmpty(destinationDir))
                    {
                        CreateDirectory(destinationDir);
                    }

                    File.Move(sourcePath, destinationPath);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Move File");
            }

            return false;
        }

        /// <summary>
        /// Get file size in bytes
        /// </summary>
        public static long GetFileSize(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return new FileInfo(filePath).Length;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get File Size");
            }

            return 0;
        }

        /// <summary>
        /// Get file size in human readable format
        /// </summary>
        public static string GetFileSizeFormatted(string filePath)
        {
            try
            {
                var size = GetFileSize(filePath);
                return FormatFileSize(size);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get File Size Formatted");
                return "0 B";
            }
        }

        /// <summary>
        /// Format file size in human readable format
        /// </summary>
        public static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        /// <summary>
        /// Get file extension
        /// </summary>
        public static string GetFileExtension(string filePath)
        {
            try
            {
                return Path.GetExtension(filePath).ToLower();
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get File Extension");
                return "";
            }
        }

        /// <summary>
        /// Get file name without extension
        /// </summary>
        public static string GetFileNameWithoutExtension(string filePath)
        {
            try
            {
                return Path.GetFileNameWithoutExtension(filePath);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get File Name Without Extension");
                return "";
            }
        }

        /// <summary>
        /// Get file name with extension
        /// </summary>
        public static string GetFileName(string filePath)
        {
            try
            {
                return Path.GetFileName(filePath);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get File Name");
                return "";
            }
        }

        /// <summary>
        /// Get directory name
        /// </summary>
        public static string GetDirectoryName(string filePath)
        {
            try
            {
                return Path.GetDirectoryName(filePath) ?? "";
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Directory Name");
                return "";
            }
        }

        /// <summary>
        /// Read all text from file
        /// </summary>
        public static string ReadAllText(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return File.ReadAllText(filePath);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Read All Text");
            }

            return "";
        }

        /// <summary>
        /// Write all text to file
        /// </summary>
        public static bool WriteAllText(string filePath, string content)
        {
            try
            {
                // Create directory if it doesn't exist
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    CreateDirectory(directory);
                }

                File.WriteAllText(filePath, content);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Write All Text");
                return false;
            }
        }

        /// <summary>
        /// Read all bytes from file
        /// </summary>
        public static byte[] ReadAllBytes(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return File.ReadAllBytes(filePath);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Read All Bytes");
            }

            return new byte[0];
        }

        /// <summary>
        /// Write all bytes to file
        /// </summary>
        public static bool WriteAllBytes(string filePath, byte[] bytes)
        {
            try
            {
                // Create directory if it doesn't exist
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    CreateDirectory(directory);
                }

                File.WriteAllBytes(filePath, bytes);
                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Write All Bytes");
                return false;
            }
        }

        /// <summary>
        /// Load image from file
        /// </summary>
        public static BitmapImage? LoadImage(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(filePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.EndInit();
                    return bitmap;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Load Image");
            }

            return null;
        }

        /// <summary>
        /// Save image to file
        /// </summary>
        public static bool SaveImage(BitmapImage image, string filePath)
        {
            try
            {
                // Create directory if it doesn't exist
                var directory = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(directory))
                {
                    CreateDirectory(directory);
                }

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(image));
                
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }

                return true;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Save Image");
                return false;
            }
        }

        /// <summary>
        /// Get files in directory
        /// </summary>
        public static string[] GetFiles(string directoryPath, string searchPattern = "*.*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    return Directory.GetFiles(directoryPath, searchPattern, searchOption);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Files");
            }

            return new string[0];
        }

        /// <summary>
        /// Get directories in directory
        /// </summary>
        public static string[] GetDirectories(string directoryPath, string searchPattern = "*", SearchOption searchOption = SearchOption.TopDirectoryOnly)
        {
            try
            {
                if (Directory.Exists(directoryPath))
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, searchOption);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Directories");
            }

            return new string[0];
        }

        /// <summary>
        /// Get temporary file path
        /// </summary>
        public static string GetTempFilePath(string extension = ".tmp")
        {
            try
            {
                return Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + extension);
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Temp File Path");
                return "";
            }
        }

        /// <summary>
        /// Get application data directory
        /// </summary>
        public static string GetApplicationDataDirectory()
        {
            try
            {
                var appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ETAG_ERP");
                CreateDirectory(appDataPath);
                return appDataPath;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Application Data Directory");
                return "";
            }
        }

        /// <summary>
        /// Get documents directory
        /// </summary>
        public static string GetDocumentsDirectory()
        {
            try
            {
                var documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ETAG_ERP");
                CreateDirectory(documentsPath);
                return documentsPath;
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Get Documents Directory");
                return "";
            }
        }

        /// <summary>
        /// Clean up temporary files
        /// </summary>
        public static void CleanupTempFiles()
        {
            try
            {
                var tempPath = Path.GetTempPath();
                var tempFiles = GetFiles(tempPath, "ETAG_ERP_*", SearchOption.TopDirectoryOnly);
                
                foreach (var file in tempFiles)
                {
                    try
                    {
                        // Delete files older than 1 hour
                        var fileInfo = new FileInfo(file);
                        if (DateTime.Now - fileInfo.CreationTime > TimeSpan.FromHours(1))
                        {
                            DeleteFile(file);
                        }
                    }
                    catch
                    {
                        // Ignore individual file deletion errors
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandler.HandleException(ex, "Cleanup Temp Files");
            }
        }
    }
}