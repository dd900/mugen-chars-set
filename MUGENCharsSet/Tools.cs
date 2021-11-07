using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MUGENCharsSet
{
    /// <summary>
    /// Tools
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Get the folder of the program where the program is absolutely path
        /// </summary>
        public static string AppDirPath
        {
            get { return Application.StartupPath + "\\"; }
        }

        /// <summary>
        /// Get the folder path of the end tail strip backslash ()
        /// </summary>
        /// <param name="dirPath">Folder path</param>
        /// <returns>Folder path</returns>
        public static string GetFormatDirPath(this string dirPath)
        {
            if (dirPath.Length > 0 && dirPath[dirPath.Length - 1] != '\\')
                return dirPath + "\\";
            else return dirPath;
        }

        /// <summary>
        /// Get the folder path where the file is located
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <returns>Folder path</returns>
        public static string GetDirPathOfFile(this string filePath)
        {
            try
            {
                return Path.GetDirectoryName(filePath) + "\\";
            }
            catch (Exception) { return ""; }
        }

        /// <summary>
        /// Get files (clips) paths for forward slash (/)
        /// </summary>
        /// <param name="path">File (clip) path</param>
        /// <returns>File (clip) path</returns>
        public static string GetSlashPath(this string path)
        {
            return path.Replace('\\', '/');
        }

        /// <summary>
        /// Get a file (clip) path (clip) path
        /// </summary>
        /// <param name="path">File (clip) path</param>
        /// <returns>File (clip) path</returns>
        public static string GetBackSlashPath(this string path)
        {
            return path.Replace('/', '\\');
        }

        /// <summary>
        /// Cancel specified file read-only properties
        /// </summary>
        /// <param name="path">file path</param>
        /// <returns>Whether to modify success</returns>
        public static bool SetFileNotReadOnly(string path)
        {
            if (!File.Exists(path)) return true;
            try
            {
                FileInfo fileInfo = new FileInfo(path);
                if (fileInfo.IsReadOnly)
                {
                    fileInfo.IsReadOnly = false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Make the configuration file specifications (add a blank line at the beginning, convert UTF-8 and Unicode encoded files to default encoding)
        /// </summary>
        /// <param name="path">File absolute path</param>
        public static void IniFileStandardization(string path)
        {
            FileStream fs = null;
            byte[] data = null;
            try
            {
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                data = new byte[fs.Length];
                fs.Read(data, 0, data.Length);
            }
            catch (Exception)
            {
                return;
            }
            finally
            {
                if (fs != null) fs.Close();
            }

            try
            {
                if (data[0] == '[')
                {
                    string content = File.ReadAllText(path, Encoding.Default);
                    File.WriteAllText(path, "\r\n" + content, Encoding.Default);
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// Get the same index item as the specified value in the COMBO Box control
        /// </summary>
        /// <param name="combobox">Combo Box control</param>
        /// <param name="value">Specified value</param>
        public static int GetComboBoxEqualValueIndex(ComboBox combobox, int value)
        {
            for (int i = 0; i < combobox.Items.Count; i++)
            {
                try
                {
                    if (Convert.ToInt32(combobox.Items[i].ToString()) == value)
                    {
                        return i;
                    }
                }
                catch (Exception)
                {
                    return -1;
                }
            }
            return -1;
        }

        /// <summary>
        /// Get the same index item as the specified value in the COMBO Box control
        /// </summary>
        /// <param name="combobox">Combo Box control</param>
        /// <param name="value">Specified value</param>
        public static int GetComboBoxEqualValueIndex(ComboBox combobox, string value)
        {
            for (int i = 0; i < combobox.Items.Count; i++)
            {
                if (combobox.Items[i].ToString().ToLower() == value.ToLower())
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Get a folder absolute path with other folders.
        /// </summary>
        /// <param name="parentDirPath">Parent folder absolute path</param>
        /// <param name="dirName">Want to create folder name</param>
        /// <returns>Folder absolute path with other folders</returns>
        public static string GetNonExistsDirPath(string parentDirPath, string dirName)
        {
            string path = parentDirPath + dirName + "\\";
            if (!Directory.Exists(path))
            {
                return path;
            }
            else
            {
                for (int i = 1; i <= 1000; i++)
                {
                    path = parentDirPath + dirName + "_" + i + "\\";
                    if (!Directory.Exists(path)) return path;
                }
            }
            return "";
        }
    }
}