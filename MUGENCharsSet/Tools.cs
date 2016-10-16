﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MUGENCharsSet
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// 获取程序所在文件夹绝对路径
        /// </summary>
        public static string AppDirPath
        {
            get { return Application.StartupPath + "\\"; }
        }

        /// <summary>
        /// 获取末尾带反斜杠(\)的文件夹路径
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <returns>文件夹路径</returns>
        public static string GetFormatDirPath(this string dirPath)
        {
            if (dirPath.Length > 0 && dirPath[dirPath.Length - 1] != '\\')
                return dirPath + "\\";
            else return dirPath;
        }

        /// <summary>
        /// 获取文件所在的文件夹路径
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns>文件夹路径</returns>
        public static string GetDirPathOfFile(this string filePath)
        {
            try
            {
                return Path.GetDirectoryName(filePath) + "\\";
            }
            catch (Exception) { return ""; }
        }

        /// <summary>
        /// 获取正斜杠(/)的文件(夹)路径
        /// </summary>
        /// <param name="path">文件(夹)路径</param>
        /// <returns>文件(夹)路径</returns>
        public static string GetSlashPath(this string path)
        {
            return path.Replace('\\', '/');
        }

        /// <summary>
        /// 获取反斜杠(\)的文件(夹)路径
        /// </summary>
        /// <param name="path">文件(夹)路径</param>
        /// <returns>文件(夹)路径</returns>
        public static string GetBackSlashPath(this string path)
        {
            return path.Replace('/', '\\');
        }

        /// <summary>
        /// 取消指定文件只读属性
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>是否修改成功</returns>
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
        /// 判断指定字节流是否为UTF-8编码
        /// </summary>
        /// <param name="data">字节流</param>
        /// <returns>是否为UTF-8编码</returns>
        public static bool IsUTF8(byte[] data)
        {
            int encodingBytesCount = 0;
            bool allTextsAreASCIIChars = true;
            if (data.Length >= 3 && (byte)0xEF == data[0] && (byte)0xBB == data[1] && (byte)0xBF == data[2])
            {
                return true;
            }
            for (int i = 0; i < data.Length; i++)
            {
                byte current = data[i];
                if ((current & 0x80) == 0x80)
                {
                    allTextsAreASCIIChars = false;
                }
                if (encodingBytesCount == 0)
                {
                    if ((current & 0x80) == 0)
                    {
                        continue;
                    }
                    if ((current & 0xC0) == 0xC0)
                    {
                        encodingBytesCount = 1;
                        current <<= 2;
                        while ((current & 0x80) == 0x80)
                        {
                            current <<= 1;
                            encodingBytesCount++;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    if ((current & 0xC0) == 0x80)
                    {
                        encodingBytesCount--;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            if (encodingBytesCount != 0)
            {
                return false;
            }
            return !allTextsAreASCIIChars;
        }

        /// <summary>
        /// 判断指定字节流是否为Unicode编码
        /// </summary>
        /// <param name="data">字节流</param>
        /// <returns>是否为Unicode编码</returns>
        public static bool IsUnicode(byte[] data)
        {
            if (data.Length >= 2 && (((byte)0xFE == data[0] && (byte)0xFF == data[1]) || ((byte)0xFF == data[0] && (byte)0xFE == data[1])))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 获取指定字节流使用的编码
        /// </summary>
        /// <param name="data">字节流</param>
        /// <returns><see cref="Encoding"/>类对象</returns>
        public static Encoding GetEncoding(byte[] data)
        {
            if (IsUTF8(data)) return Encoding.UTF8;
            else if (IsUnicode(data)) return Encoding.Unicode;
            else return Encoding.Default;
        }

        /// <summary>
        /// 使配置文件规格化(在文件开头添加空行、将UTF-8及Unicode编码的文件转换为默认编码)
        /// </summary>
        /// <param name="path">文件绝对路径</param>
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
                Encoding encoding = GetEncoding(data);
                if (encoding != Encoding.UTF8)
                {
                    string content = File.ReadAllText(path, encoding);
                    if (content[0] == '[')
                    {
                        content = "\r\n" + content;
                    }
                    File.WriteAllText(path, content, Encoding.UTF8);
                }
                else
                {
                    if (data[0] == '[')
                    {
                        string content = File.ReadAllText(path, Encoding.UTF8);
                        File.WriteAllText(path, "\r\n" + content, Encoding.UTF8);
                    }
                }
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// 获取ComboBox控件中与指定数值相同的索引项
        /// </summary>
        /// <param name="combobox">ComboBox控件</param>
        /// <param name="value">指定数值</param>
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
        /// 获取ComboBox控件中与指定数值相同的索引项
        /// </summary>
        /// <param name="combobox">ComboBox控件</param>
        /// <param name="value">指定数值</param>
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
        /// 获取与其它文件夹不重名的文件夹绝对路径
        /// </summary>
        /// <param name="parentDirPath">父文件夹绝对路径</param>
        /// <param name="dirName">想要创建的文件夹名字</param>
        /// <returns>与其它文件夹不重名的文件夹绝对路径</returns>
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
