using System;
using System.IO;

namespace MUGENCharsSet
{
    /// <summary>
    /// Program configuration class
    /// </summary>
    public static class AppConfig
    {
        /// <summary>Configuration file name</summary>
        private const string ConfigFileName = "AppConfig.xml";

        /// <summary>Default text editor path</summary>
        public const string DefaultEditProgramPath = "notepad.exe";

        /// <summary>
        /// Program configuration information structure
        /// </summary>
        public struct ConfigInfo
        {
            /// <summary>MUGEN program absolute path element name</summary>
            public const string MugenExePath = "mugenExePath";

            /// <summary>Automatic sorting of element names</summary>
            public const string AutoSort = "autoSort";

            /// <summary>Text editor element name</summary>
            public const string EditProgramPath = "editProgramPath";

            /// <summary>Read character list type element name</summary>
            public const string ReadCharacterType = "readCharacterType";

            /// <summary>Display character wide/normal screen tag element name</summary>
            public const string ShowCharacterScreenMark = "showCharacterScreenMark";
        }

        /// <summary>
        /// Character list reading method type enumeration
        /// </summary>
        public enum ReadCharTypeEnum { SelectDef = 0, CharsDir = 1 };

        /// <summary>Current <see cref="XmlConfig"/> object</summary>
        private static XmlConfig Config = null;

        private static string _mugenExePath = "";
        private static bool _autoSort = false;
        private static ReadCharTypeEnum _readCharacterType = ReadCharTypeEnum.SelectDef;
        private static string _editProgramPath = DefaultEditProgramPath;
        private static bool _showCharacterScreenMark = false;

        /// <summary>
        /// Obtain the absolute path of the configuration file
        /// </summary>
        public static string ConfigPath
        {
            get
            {
                return Tools.AppDirPath + ConfigFileName;
            }
        }

        /// <summary>
        /// Get or set the absolute path of the MUGEN program
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static string MugenExePath
        {
            get { return _mugenExePath; }
            set
            {
                if (value == string.Empty) throw new ApplicationException("The path cannot be empty!");
                if (Path.GetExtension(value) != ".exe") throw new ApplicationException("Must be an executable program!");
                _mugenExePath = value;
                if (Config != null) Config.SetValue(ConfigInfo.MugenExePath, value);
            }
        }

        /// <summary>
        /// Get or set whether the person list is automatically arranged
        /// </summary>
        public static bool AutoSort
        {
            get { return _autoSort; }
            set
            {
                _autoSort = value;
                if (Config != null) Config.SetValue(ConfigInfo.AutoSort, value);
            }
        }

        /// <summary>
        /// Get or set the way to read people list
        /// </summary>
        public static ReadCharTypeEnum ReadCharacterType
        {
            get { return _readCharacterType; }
            set
            {
                _readCharacterType = value;
                if (Config != null) Config.SetValue(ConfigInfo.ReadCharacterType, (int)value);
            }
        }

        /// <summary>
        /// Get or set the path of the text editor
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static string EditProgramPath
        {
            get { return _editProgramPath; }
            set
            {
                if (value == string.Empty) throw new ApplicationException("The path cannot be empty!");
                if (Path.GetExtension(value) != ".exe") throw new ApplicationException("Must be an executable program!");
                _editProgramPath = value;
                if (Config != null) Config.SetValue(ConfigInfo.EditProgramPath, value);
            }
        }

        /// <summary>
        /// Get or set whether to display character width/full screen mark
        /// </summary>
        public static bool ShowCharacterScreenMark
        {
            get { return _showCharacterScreenMark; }
            set
            {
                _showCharacterScreenMark = value;
                if (Config != null) Config.SetValue(ConfigInfo.ShowCharacterScreenMark, value);
            }
        }

        /// <summary>
        /// Read configuration file
        /// </summary>
        /// <returns>Whether the reading was successful</returns>
        public static bool Read()
        {
            try
            {
                Config = new XmlConfig(ConfigPath, true);
            }
            catch (ApplicationException)
            {
                return false;
            }
            _mugenExePath = Config.GetValue(ConfigInfo.MugenExePath, "").GetBackSlashPath();
            _autoSort = Config.GetValue(ConfigInfo.AutoSort, false);
            _editProgramPath = Config.GetValue(ConfigInfo.EditProgramPath, DefaultEditProgramPath);
            _readCharacterType = Config.GetValue(ConfigInfo.EditProgramPath, 0) == 1 ?
                ReadCharTypeEnum.CharsDir : ReadCharTypeEnum.SelectDef;
            _showCharacterScreenMark = Config.GetValue(ConfigInfo.ShowCharacterScreenMark, false);
            return true;
        }

        /// <summary>
        /// Save the configuration file
        /// </summary>
        /// <returns>Whether the save was successful</returns>
        public static bool Save()
        {
            if (Config != null) return Config.Save();
            else return false;
        }
    }
}