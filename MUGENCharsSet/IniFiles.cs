using System;
using System.Collections.Specialized;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace MUGENCharsSet
{
    /// <summary>
    /// INI configuration class
    /// </summary>
    public class IniFiles
    {
        /// <summary>Comment separator</summary>
        public const string CommentMark = ";";

        private readonly string _filePath;

        // Declare API functions for reading and writing INI files
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(string section, string key, byte[] val, string filePath);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, byte[] retVal, int size, string filePath);

        /// <summary>
        /// Obtain the absolute path of the configuration file
        /// </summary>
        public string FilePath
        {
            get { return _filePath; }
        }

        /// <summary>
        /// Create a new instance of the <see cref="IniFiles"/> class according to the specified file path
        /// </summary>
        /// <param name="fileName">ini file path</param>
        /// <exception cref="System.ApplicationException"></exception>
        public IniFiles(string fileName)
        {
            // Determine whether the file exists
            FileInfo fileInfo = new FileInfo(fileName);
            //Todo:Figure out the usage of enumeration
            if (!fileInfo.Exists)
            {
                StreamWriter sw = null;
                try
                {
                    //File does not exist, create file
                    sw = new StreamWriter(fileName, false, Encoding.Default);
                    sw.Write("\r\n");
                }
                catch
                {
                    throw new ApplicationException("The configuration file does not exist！");
                }
                finally
                {
                    if (sw != null) sw.Close();
                }
            }
            else
            {
                Tools.IniFileStandardization(fileInfo.FullName);
            }
            //Must be a full path, not a relative path
            _filePath = fileInfo.FullName;
        }

        /// <summary>
        /// Write the specified configuration item
        /// </summary>
        /// <param name="Section">Configure segmentation</param>
        /// <param name="Ident">Configuration items</param>
        /// <param name="Value">Configuration value</param>
        /// <exception cref="System.ApplicationException"></exception>
        public void WriteString(string Section, string Ident, string Value)
        {
            if (!WritePrivateProfileString(Section, Ident, Encoding.UTF8.GetBytes(" " + Value.TrimStart()), FilePath))
            {
                throw new ApplicationException("Failed to write configuration file！");
            }
        }

        /// <summary>
        /// Read the specified configuration item
        /// </summary>
        /// <param name="Section">Configure segmentation</param>
        /// <param name="Ident">Configuration items</param>
        /// <param name="Default">default value</param>
        /// <returns>Configuration value</returns>
        public string ReadString(string Section, string Ident, string Default)
        {
            byte[] Buffer = new byte[65535];
            int bufLen = GetPrivateProfileString(Section, Ident, Default, Buffer, Buffer.GetUpperBound(0), FilePath);
            string s = Encoding.UTF8.GetString(Buffer);
            s = s.Substring(0, bufLen);
            if (s.IndexOf(CommentMark) >= 0)
            {
                s = s.Substring(0, s.IndexOf(CommentMark));
            }
            return s.Trim('\0').Trim();
        }

        /// <summary>
        /// Read the specified configuration item (integer)
        /// </summary>
        /// <param name="Section">Configure segmentation</param>
        /// <param name="Ident">Configuration items</param>
        /// <param name="Default">default value</param>
        /// <returns>Configuration value</returns>
        public int ReadInteger(string Section, string Ident, int Default)
        {
            string intStr = ReadString(Section, Ident, Convert.ToString(Default));
            try
            {
                return Convert.ToInt32(intStr);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Default;
            }
        }

        /// <summary>
        /// Write the specified configuration item (integer)
        /// </summary>
        /// <param name="Section">Configure segmentation</param>
        /// <param name="Ident">Configuration items</param>
        /// <param name="Value">Configuration value</param>
        public void WriteInteger(string Section, string Ident, int Value)
        {
            WriteString(Section, Ident, Value.ToString());
        }

        /// <summary>
        /// Read the specified configuration item (Boolean value)
        /// </summary>
        /// <param name="Section">Configure segmentation</param>
        /// <param name="Ident">Configuration items</param>
        /// <param name="Default">default value</param>
        /// <returns>Configuration value</returns>
        public bool ReadBool(string Section, string Ident, bool Default)
        {
            try
            {
                return Convert.ToBoolean(ReadString(Section, Ident, Convert.ToString(Default)));
            }
            catch (Exception)
            {
                //Console.WriteLine(ex.Message);
                return Default;
            }
        }

        /// <summary>
        /// Write the specified configuration item (Boolean value)
        /// </summary>
        /// <param name="Section">Configure segmentation</param>
        /// <param name="Ident">Configuration items</param>
        /// <param name="Value">Configuration value</param>
        public void WriteBool(string Section, string Ident, bool Value)
        {
            WriteString(Section, Ident, Convert.ToString(Value));
        }

        /// <summary>
        /// Read all items in the specified configuration segment
        /// </summary>
        /// <param name="Section">Configure segmentation</param>
        /// <param name="Idents">Configuration value list</param>
        public void ReadSection(string Section, StringCollection Idents)
        {
            byte[] Buffer = new byte[16384];
            //Idents.Clear();

            int bufLen = GetPrivateProfileString(Section, null, null, Buffer, Buffer.GetUpperBound(0),
             FilePath);
            //Analyze Section
            GetStringsFromBuffer(Buffer, bufLen, Idents);
        }

        /// <summary>
        /// Get a list of strings from the Buffer
        /// </summary>
        /// <param name="Buffer">Buffer</param>
        /// <param name="bufLen">Buffer length</param>
        /// <param name="Strings">list of strings</param>
        private void GetStringsFromBuffer(byte[] Buffer, int bufLen, StringCollection Strings)
        {
            Strings.Clear();
            if (bufLen != 0)
            {
                int start = 0;
                for (int i = 0; i < bufLen; i++)
                {
                    if ((Buffer[i] == 0) && ((i - start) > 0))
                    {
                        String s = Encoding.UTF8.GetString(Buffer, start, i - start);
                        Strings.Add(s);
                        start = i + 1;
                    }
                }
            }
        }

        /// <summary>
        /// Read all configuration segment names
        /// </summary>
        /// <param name="SectionList">Configuration section name list</param>
        public void ReadSections(StringCollection SectionList)
        {
            //Note: It must be implemented with Bytes, StringBuilder can only get the first Section
            byte[] Buffer = new byte[65535];
            int bufLen = 0;
            bufLen = GetPrivateProfileString(null, null, null, Buffer, Buffer.GetUpperBound(0), FilePath);
            GetStringsFromBuffer(Buffer, bufLen, SectionList);
        }

        /// <summary>
        /// Read the key-value pairs of all items in the specified configuration segment
        /// </summary>
        /// <param name="Section">Configure segmentation</param>
        /// <param name="Values">key-value pairs of configuration items</param>
        public void ReadSectionValues(string Section, NameValueCollection Values)
        {
            StringCollection KeyList = new StringCollection();
            ReadSection(Section, KeyList);
            Values.Clear();
            foreach (string key in KeyList)
            {
                Values.Add(key, ReadString(Section, key, ""));
            }
        }

        ////Read all Values ​​of the specified Section to the list,
        //public void ReadSectionValues(string Section, NameValueCollection Values,char splitString)
        //{　 string sectionValue;
        //　　string[] sectionValueSplit;
        //　　StringCollection KeyList = new StringCollection();
        //　　ReadSection(Section, KeyList);
        //　　Values.Clear();
        //　　foreach (string key in KeyList)
        //　　{
        //　　　　sectionValue=ReadString(Section, key, "");
        //　　　　sectionValueSplit=sectionValue.Split(splitString);
        //　　　　Values.Add(key, sectionValueSplit[0].ToString(),sectionValueSplit[1].ToString());

        //　　}
        //}

        /// <summary>
        /// Delete the specified configuration segment
        /// </summary>
        /// <param name="Section">Configure segmentation</param>
        /// <exception cref="System.ApplicationException"></exception>
        public void EraseSection(string Section)
        {
            if (!WritePrivateProfileString(Section, null, null, FilePath))
            {
                throw new ApplicationException("Unable to clear Section in the configuration file！");
            }
        }

        /// <summary>
        /// Delete the specified configuration item
        /// </summary>
        /// <param name="Section">Configure segmentation</param>
        /// <param name="Ident">Configuration items</param>
        public void DeleteKey(string Section, string Ident)
        {
            WritePrivateProfileString(Section, Ident, null, FilePath);
        }

        /// <summary>
        /// For Win9X, it is necessary to implement the UpdateFile method to write the data in the buffer to the file.
        /// On Win NT, 2000 and XP, files are written directly without buffering, so there is no need to implement UpdateFile.
        /// After executing the modification to the Ini file, you should call this method to update the buffer.
        /// </summary>
        public void UpdateFile()
        {
            WritePrivateProfileString(null, null, null, FilePath);
        }

        /// <summary>
        /// Check if the specified configuration item exists
        /// </summary>
        /// <param name="Section">Configure segmentation</param>
        /// <param name="Ident">Configuration items</param>
        /// <returns>Does it exist?</returns>
        public bool ValueExists(string Section, string Ident)
        {
            //
            StringCollection Idents = new StringCollection();
            ReadSection(Section, Idents);
            return Idents.IndexOf(Ident) > -1;
        }

        //Ensure the release of resources
        ~IniFiles()
        {
            UpdateFile();
        }
    }
}