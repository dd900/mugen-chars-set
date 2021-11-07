using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MUGENCharsSet
{
    #region MUGEN program configuration class

    /// <summary>
    /// MUGEN program configuration class
    /// </summary>
    public static class MugenSetting
    {
        #region Typical

        /// <summary>Character folder relative path</summary>
        public const string CharsDir = @"chars\";

        /// <summary>Data folder relative path</summary>
        public const string DataDir = @"data\";

        /// <summary>Mugen.cfg file name</summary>
        public const string MugenCfgFileName = "mugen.cfg";

        /// <summary>Public STCOMMON file name</summary>
        public const string StcommonFileName = "common1.cns";

        /// <summary>Backup file extension</summary>
        public const string BakExt = ".bak";

        /// <summary>
        /// MUGEN program configuration information structure
        /// </summary>
        public struct SettingInfo
        {
            /// <summary>Options configuration segmentation</summary>
            public const string OptionsSection = "Options";

            /// <summary>INFO configuration segmentation</summary>
            public const string InfoSection = "Info";

            /// <summary>FILES configuration segmentation</summary>
            public const string FilesSection = "Files";

            /// <summary>Config configuration segmentation</summary>
            public const string ConfigSection = "Config";

            /// <summary>Video configuration segmentation</summary>
            public const string VideoSection = "Video";

            /// <summary>System.def file relative path configuration item</summary>
            public const string MotifItem = "motif";

            /// <summary>Select.def file relative path configuration item</summary>
            public const string SelectDefItem = "select";

            /// <summary>Fight.def file relative path configuration item</summary>
            public const string FightDefItem = "fight";

            /// <summary>System LocalCoord configuration item</summary>
            public const string LocalcoordItem = "localcoord";

            /// <summary>DIFFICULTY configuration item</summary>
            public const string DifficultyItem = "Difficulty";

            /// <summary>Life configuration item</summary>
            public const string LifeItem = "Life";

            /// <summary>TIME configuration item</summary>
            public const string TimeItem = "Time";

            /// <summary>Game speed configuration item</summary>
            public const string GameSpeedItem = "GameSpeed";

            /// <summary>Game Speed ​​Configuration Item</summary>
            public const string GameFrameItem = "GameSpeed";

            /// <summary>Team.1vs2life configuration item</summary>
            public const string Team1VS2LifeItem = "Team.1VS2Life";

            /// <summary>Team.lose On KO Configuration Item</summary>
            public const string TeamLoseOnKOItem = "Team.LoseOnKO";

            /// <summary>Game Width Configuration Item</summary>
            public const string GameWidthItem = "GameWidth";

            /// <summary>Game HEIGHT configuration item</summary>
            public const string GameHeightItem = "GameHeight";

            /// <summary>Render Mode Configuration Item</summary>
            public const string RenderModeItem = "RenderMode";

            /// <summary>Full Screen configuration item</summary>
            public const string FullScreenItem = "FullScreen";
        }

        /// <summary>
        /// Mugen program version enumeration
        /// </summary>
        public enum MugenVersion { WIN, V1_X };

        #endregion Typical

        #region Private member

        private static string _mugenExePath;
        private static string _mugenCfgPath;
        private static string _systemDefPath = "";
        private static string _selectDefPath = "";
        private static string _fightDefPath = "";
        private static Size _localcoord;
        private static bool _isWideScreen = false;
        private static int _difficulty = 0;
        private static int _life = 0;
        private static int _time = 0;
        private static int _gameSpeed = 0;
        private static int _gameFrame = 0;
        private static int _team1VS2Life = 0;
        private static bool _teamLoseOnKO = false;
        private static int _gameWidth = 0;
        private static int _gameHeight = 0;
        private static string _renderMode = "";
        private static bool _fullScreen = false;
        private static KeyPressSetting _keyPressP1;
        private static KeyPressSetting _keyPressP2;
        private static MugenVersion _version = MugenVersion.V1_X;

        #endregion Private member

        #region Class properties

        /// <summary>
        /// Get a Mugen program absolute path
        /// </summary>
        private static string MugenExePath
        {
            get { return _mugenExePath; }
        }

        /// <summary>
        /// Get a Mugen program root directory absolute path
        /// </summary>
        public static string MugenDirPath
        {
            get { return MugenExePath.GetDirPathOfFile(); }
        }

        /// <summary>
        /// Get the absolute path of the Mugen Data folder
        /// </summary>
        public static string MugenDataDirPath
        {
            get { return MugenDirPath + DataDir; }
        }

        /// <summary>
        /// Get the absolute path of Mugen character folder
        /// </summary>
        public static string MugenCharsDirPath
        {
            get { return MugenDirPath + CharsDir; }
        }

        /// <summary>
        /// Get the absolute path of Mugen.cfg files
        /// </summary>
        public static string MugenCfgPath
        {
            get { return _mugenCfgPath; }
        }

        /// <summary>
        /// Get or set the system.def file absolute path
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static string SystemDefPath
        {
            get { return _systemDefPath; }
            set
            {
                if (value == string.Empty) throw new ApplicationException("The path must not be empty！");
                if (!File.Exists(MugenCfgPath)) throw new ApplicationException("Mugen.cfg file does not exist！");
                try
                {
                    IniFiles ini = new IniFiles(MugenCfgPath);
                    ini.WriteString(SettingInfo.OptionsSection, SettingInfo.MotifItem,
                        value.Substring(MugenDirPath.Length).GetSlashPath());
                }
                catch (ApplicationException)
                {
                    throw new ApplicationException("Mugen.cfg file failed！");
                }
                _systemDefPath = value;
            }
        }

        /// <summary>
        /// Get or set the SELECT.DEF file absolute path
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static string SelectDefPath
        {
            get { return _selectDefPath; }
            set
            {
                if (value == string.Empty) throw new ApplicationException("The path must not be empty！");
                if (!File.Exists(SystemDefPath)) throw new ApplicationException("System.def file does not exist！");
                try
                {
                    IniFiles ini = new IniFiles(SystemDefPath);
                    string path = GetIniFileBestPath(SystemDefPath, value);
                    if (path == string.Empty) new ApplicationException();
                    ini.WriteString(SettingInfo.FilesSection, SettingInfo.SelectDefItem, path.GetSlashPath());
                }
                catch (ApplicationException)
                {
                    throw new ApplicationException("System.def file failed！");
                }
                _selectDefPath = value;
            }
        }

        /// <summary>
        /// Get or set the fast.def file absolute path
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static string FightDefPath
        {
            get { return _fightDefPath; }
            set
            {
                if (value == string.Empty) throw new ApplicationException("The path must not be empty！");
                if (!File.Exists(SystemDefPath)) throw new ApplicationException("System.def file does not exist！");
                try
                {
                    IniFiles ini = new IniFiles(SystemDefPath);
                    string path = GetIniFileBestPath(SystemDefPath, value);
                    if (path == string.Empty) new ApplicationException();
                    ini.WriteString(SettingInfo.FilesSection, SettingInfo.FightDefItem, path.GetSlashPath());
                }
                catch (ApplicationException)
                {
                    throw new ApplicationException("System.def file failed！");
                }
                _fightDefPath = value;
            }
        }

        /// <summary>
        /// Get or set system localcoord
        /// </summary>
        public static Size Localcoord
        {
            get { return _localcoord; }
            set { _localcoord = value; }
        }

        /// <summary>
        /// Get or set the system picture package to be a widescreen
        /// </summary>
        public static bool IsWideScreen
        {
            get { return _isWideScreen; }
            set { _isWideScreen = value; }
        }

        /// <summary>
        /// Get or set Difficulty
        /// </summary>
        public static int Difficulty
        {
            get { return _difficulty; }
            set { _difficulty = value; }
        }

        /// <summary>
        /// Get or set up LIFE
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static int Life
        {
            get { return _life; }
            set
            {
                if (value < 0) throw new ApplicationException("Life is not less than 0！");
                _life = value;
            }
        }

        /// <summary>
        /// Get or set up TIME
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static int Time
        {
            get { return _time; }
            set
            {
                if (value < -1) throw new ApplicationException("Time is not less than -1！");
                _time = value;
            }
        }

        /// <summary>
        /// Get or set up Game Speed
        /// </summary>
        public static int GameSpeed
        {
            get { return _gameSpeed; }
            set { _gameSpeed = value; }
        }

        /// <summary>
        /// Get or set Game Speed ​​(frame rate)
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static int GameFrame
        {
            get { return _gameFrame; }
            set
            {
                if (value < 10) throw new ApplicationException("Game speed is not less than 10！");
                _gameFrame = value;
            }
        }

        /// <summary>
        /// Get or set Team1vs2Life
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static int Team1VS2Life
        {
            get { return _team1VS2Life; }
            set
            {
                if (value < 0) throw new ApplicationException("Team1vs2life must not be less than 0！");
                _team1VS2Life = value;
            }
        }

        /// <summary>
        /// Get or set to Team Lose on Ko
        /// </summary>
        public static bool TeamLoseOnKO
        {
            get { return _teamLoseOnKO; }
            set { _teamLoseOnKO = value; }
        }

        /// <summary>
        /// Get or set Game Width
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static int GameWidth
        {
            get { return _gameWidth; }
            set
            {
                if (value <= 0) throw new ApplicationException("Game Width is not less than 1！");
                _gameWidth = value;
            }
        }

        /// <summary>
        /// Get or set up Game HearT
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static int GameHeight
        {
            get { return _gameHeight; }
            set
            {
                if (value <= 0) throw new ApplicationException("Game HEIGHT is not less than 1！");
                _gameHeight = value;
            }
        }

        /// <summary>
        /// Get or set Render Mode
        /// </summary>
        public static string RenderMode
        {
            get { return _renderMode; }
            set { _renderMode = value; }
        }

        /// <summary>
        /// Get or set to full screen
        /// </summary>
        public static bool FullScreen
        {
            get { return _fullScreen; }
            set { _fullScreen = value; }
        }

        /// <summary>
        /// Get or set P1 buttons
        /// </summary>
        public static KeyPressSetting KeyPressP1
        {
            get { return _keyPressP1; }
            set { _keyPressP1 = value; }
        }

        /// <summary>
        /// Get or set P2 buttons
        /// </summary>
        public static KeyPressSetting KeyPressP2
        {
            get { return _keyPressP2; }
            set { _keyPressP2 = value; }
        }

        /// <summary>
        /// Get the current MUGEN program version
        /// </summary>
        public static MugenVersion Version
        {
            get { return _version; }
        }

        #endregion Class properties

        #region Class method

        /// <summary>
        /// initialization method
        /// </ summary>
        /// <param name = "Mugen EXE PATH"> MUGEN program absolute path </ param>
        /// <exception cref="System.ApplicationException"></exception>
        public static void Init(string mugenExePath)
        {
            if (!File.Exists(mugenExePath)) throw new ApplicationException("Mugen program does not exist！");
            _mugenExePath = mugenExePath.GetBackSlashPath();
            _mugenCfgPath = MugenDataDirPath + MugenCfgFileName;
            if (!File.Exists(MugenCfgPath))
            {
                _mugenExePath = "";
                _mugenCfgPath = "";
                throw new ApplicationException("Mugen.cfg file does not exist！");
            }
        }

        /// <summary>
        /// Read MUGEN program configuration
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static void ReadMugenSetting()
        {
            if (!File.Exists(MugenCfgPath)) throw new ApplicationException("Mugen.cfg file does not exist！");
            IniFiles ini = new IniFiles(MugenCfgPath);
            _systemDefPath = MugenDirPath + ini.ReadString(SettingInfo.OptionsSection, SettingInfo.MotifItem, "").GetBackSlashPath();
            if (SystemDefPath == string.Empty) throw new ApplicationException("System.def path read failed！");
            if (!File.Exists(SystemDefPath)) throw new ApplicationException("system.def文件不存在！");
            ini = new IniFiles(SystemDefPath);
            string selectDefFileName = ini.ReadString(SettingInfo.FilesSection, SettingInfo.SelectDefItem, "");
            if (selectDefFileName == string.Empty) throw new ApplicationException("Select.def path read failed！");
            _selectDefPath = GetIniFileExistPath(SystemDefPath, selectDefFileName);
            if (SelectDefPath == string.Empty) throw new ApplicationException("select.def file does not exist！");
            _version = MugenVersion.V1_X;
            string localcoord = ini.ReadString(SettingInfo.InfoSection, SettingInfo.LocalcoordItem, "");
            if (localcoord != string.Empty)
            {
                string[] size = localcoord.Split(',');
                try
                {
                    Localcoord = new Size(Convert.ToInt32(size[0]), Convert.ToInt32(size[1]));
                    if (Math.Round((decimal)Localcoord.Width / Localcoord.Height, 2) == Math.Round(16m / 9m, 2))
                    {
                        IsWideScreen = true;
                    }
                    else
                    {
                        IsWideScreen = false;
                    }
                }
                catch (Exception) { }
            }
            else
            {
                _version = MugenVersion.WIN;
                IsWideScreen = false;
            }
        }

        /// <summary>
        /// Read Mugen.cfg file configuration
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static void ReadMugenCfgSetting()
        {
            if (!File.Exists(MugenCfgPath)) throw new ApplicationException("Mugen.cfg file does not exist！");
            try
            {
                IniFiles ini = new IniFiles(MugenCfgPath);
                _difficulty = ini.ReadInteger(SettingInfo.OptionsSection, SettingInfo.DifficultyItem, 1);
                _life = ini.ReadInteger(SettingInfo.OptionsSection, SettingInfo.LifeItem, 100);
                _time = ini.ReadInteger(SettingInfo.OptionsSection, SettingInfo.TimeItem, 100);
                _gameSpeed = ini.ReadInteger(SettingInfo.OptionsSection, SettingInfo.GameSpeedItem, 0);
                _gameFrame = ini.ReadInteger(SettingInfo.ConfigSection, SettingInfo.GameFrameItem, 60);
                _team1VS2Life = ini.ReadInteger(SettingInfo.OptionsSection, SettingInfo.Team1VS2LifeItem, 100);
                _teamLoseOnKO = Convert.ToBoolean(ini.ReadInteger(SettingInfo.OptionsSection, SettingInfo.TeamLoseOnKOItem, 0));
                _gameWidth = ini.ReadInteger(SettingInfo.ConfigSection, SettingInfo.GameWidthItem, 0);
                _gameHeight = ini.ReadInteger(SettingInfo.ConfigSection, SettingInfo.GameHeightItem, 0);
                _renderMode = ini.ReadString(SettingInfo.VideoSection, SettingInfo.RenderModeItem, "");
                _fullScreen = Convert.ToBoolean(ini.ReadInteger(SettingInfo.VideoSection, SettingInfo.FullScreenItem, 0));
            }
            catch (Exception)
            {
                throw new ApplicationException("Read the mugen.cfg file failed！");
            }
            KeyPressP1 = new KeyPressSetting(KeyPressSetting.SettingInfo.P1KeysSection);
            KeyPressP2 = new KeyPressSetting(KeyPressSetting.SettingInfo.P2KeysSection);
        }

        /// <summary>
        /// Save MUGEN.CFG file configuration
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static void SaveMugenCfgSetting()
        {
            if (!File.Exists(MugenCfgPath)) throw new ApplicationException("Mugen.cfg file does not exist！");
            try
            {
                if (!Tools.SetFileNotReadOnly(MugenCfgPath)) throw new ApplicationException();
                IniFiles ini = new IniFiles(MugenCfgPath);
                ini.WriteInteger(SettingInfo.OptionsSection, SettingInfo.DifficultyItem, Difficulty);
                ini.WriteInteger(SettingInfo.OptionsSection, SettingInfo.LifeItem, Life);
                ini.WriteInteger(SettingInfo.OptionsSection, SettingInfo.TimeItem, Time);
                ini.WriteInteger(SettingInfo.OptionsSection, SettingInfo.GameSpeedItem, GameSpeed);
                ini.WriteInteger(SettingInfo.ConfigSection, SettingInfo.GameFrameItem, GameFrame);
                ini.WriteInteger(SettingInfo.OptionsSection, SettingInfo.Team1VS2LifeItem, Team1VS2Life);
                ini.WriteInteger(SettingInfo.OptionsSection, SettingInfo.TeamLoseOnKOItem, TeamLoseOnKO ? 1 : 0);
                ini.WriteInteger(SettingInfo.VideoSection, SettingInfo.FullScreenItem, FullScreen ? 1 : 0);
                if (Version != MugenVersion.WIN)
                {
                    ini.WriteInteger(SettingInfo.ConfigSection, SettingInfo.GameWidthItem, GameWidth);
                    ini.WriteInteger(SettingInfo.ConfigSection, SettingInfo.GameHeightItem, GameHeight);
                    ini.WriteString(SettingInfo.VideoSection, SettingInfo.RenderModeItem, RenderMode);
                }
            }
            catch (ApplicationException)
            {
                throw new ApplicationException("Set saving failed！");
            }
            KeyPressP1.Save();
            KeyPressP2.Save();
        }

        /// <summary>
        /// Backup Mugen.cfg file
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static void BackupMugenCfgSetting()
        {
            if (!File.Exists(MugenCfgPath)) throw new ApplicationException("Mugen.cfg file does not exist！");
            try
            {
                if (!Tools.SetFileNotReadOnly(MugenCfgPath + BakExt)) throw new Exception();
                File.Copy(MugenCfgPath, MugenCfgPath + BakExt, true);
            }
            catch (Exception)
            {
                throw new ApplicationException("File backup failed！");
            }
        }

        /// <summary>
        /// Restore Mugen.cfg file
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public static void RestoreMugenCfgSetting()
        {
            if (!File.Exists(MugenCfgPath + BakExt)) throw new ApplicationException("Backup file does not exist！");
            try
            {
                if (!Tools.SetFileNotReadOnly(MugenCfgPath + BakExt)) throw new Exception();
                if (!Tools.SetFileNotReadOnly(MugenCfgPath)) throw new Exception();
                File.Copy(MugenCfgPath + BakExt, MugenCfgPath, true);
            }
            catch (Exception)
            {
                throw new ApplicationException("File recovery failed！");
            }
        }

        /// <summary>
        /// Get absolute paths available in Mugen
        /// </summary>
        /// <param name="parentFilePath">Parent configuration file absolute path</param>
        /// <param name="iniFileName">Profile relative path</param>
        /// <returns>Profile absolute path</returns>
        public static string GetIniFileExistPath(string parentFilePath, string iniFileName)
        {
            if (parentFilePath == string.Empty) return "";
            string path = (parentFilePath.GetDirPathOfFile() + iniFileName).GetBackSlashPath();
            if (File.Exists(path)) return path;
            path = (MugenSetting.MugenDataDirPath + iniFileName).GetBackSlashPath();
            if (File.Exists(path)) return path;
            path = (MugenSetting.MugenDirPath + iniFileName).GetBackSlashPath();
            if (File.Exists(path)) return path;
            else return "";
        }

        /// <summary>
        /// Get the best relative path in Mugen's configuration file
        /// </summary>
        /// <param name="parentFilePath">Parent configuration file absolute path</param>
        /// <param name="iniFilePath">Profile absolute path</param>
        /// <returns>Profile relative path</returns>
        public static string GetIniFileBestPath(string parentFilePath, string iniFilePath)
        {
            if (parentFilePath == string.Empty) return "";
            if (iniFilePath == string.Empty) return "";
            parentFilePath = parentFilePath.GetBackSlashPath();
            iniFilePath = iniFilePath.GetBackSlashPath();
            if (iniFilePath.StartsWith(parentFilePath.GetDirPathOfFile())) return iniFilePath.Substring(parentFilePath.GetDirPathOfFile().Length);
            else if (iniFilePath.StartsWith(MugenSetting.MugenDataDirPath)) return iniFilePath.Substring(MugenSetting.MugenDataDirPath.Length);
            else if (iniFilePath.StartsWith(MugenSetting.MugenDirPath)) return iniFilePath.Substring(MugenSetting.MugenDirPath.Length);
            else return "";
        }

        #endregion Class method
    }

    #endregion MUGEN程序配置类

    #region MUGEN button configuration class

    /// <summary>
    /// MUGEN button configuration class
    /// </summary>
    public class KeyPressSetting
    {
        /// <summary>
        /// MUGEN button configuration information structure
        /// </summary>
        public struct SettingInfo
        {
            /// <summary>P1 Keys configuration segmentation</summary>
            public const string P1KeysSection = "P1 Keys";

            /// <summary>P2 Keys configuration segmentation</summary>
            public const string P2KeysSection = "P2 Keys";

            /// <summary>JUMP configuration item</summary>
            public const string JumpItem = "Jump";

            /// <summary>CROUCH configuration item</summary>
            public const string CrouchItem = "Crouch";

            /// <summary>LEFT configuration item</summary>
            public const string LeftItem = "Left";

            /// <summary>Right configuration item</summary>
            public const string RightItem = "Right";

            /// <summary>A configuration item</summary>
            public const string AItem = "A";

            /// <summary>B configuration item</summary>
            public const string BItem = "B";

            /// <summary>C configuration item</summary>
            public const string CItem = "C";

            /// <summary>X configuration item</summary>
            public const string XItem = "X";

            /// <summary>Y configuration item</summary>
            public const string YItem = "Y";

            /// <summary>Z configuration item</summary>
            public const string ZItem = "Z";

            /// <summary>START configuration item</summary>
            public const string StartItem = "Start";
        }

        /// <summary>Button code left definition</summary>
        public const string LeftDelimeter = "(";

        /// <summary>Button code right definition</summary>
        public const string RightDelimeter = ")";

        /// <summary>MUGEN 1.X version button coding table</summary>
        private static readonly Dictionary<ushort, string> KeyCodeV1_X = new Dictionary<ushort, string>()
        {
            {0, "Not Used"}, {8, "Backspace"}, {9, "Tab"}, {13, "Return"}, {19, "Pause"}, {27, "Escape"}, {32, "Space"}, {39, "'"}, {44, ","}, {45, "-"}, {46, "."}, {47, "/"}, {48, "0"}, {49, "1"}, {50, "2"}, {51, "3"}, {52, "4"}, {53, "5"}, {54, "6"}, {55, "7"}, {56, "8"}, {57, "9"}, {59, ";"}, {61, "="}, {91, "["}, {92, "\\"}, {93, "]"}, {96, "`"}, {97, "a"}, {98, "b"}, {99, "c"}, {100, "d"}, {101, "e"}, {102, "f"}, {103, "g"}, {104, "h"}, {105, "i"}, {106, "j"}, {107, "k"}, {108, "l"}, {109, "m"}, {110, "n"}, {111, "o"}, {112, "p"}, {113, "q"}, {114, "r"}, {115, "s"}, {116, "t"}, {117, "u"}, {118, "v"}, {119, "w"}, {120, "x"}, {121, "y"}, {122, "z"}, {127, "Delete"}, {256, "Num 0"}, {257, "Num 1"}, {258, "Num 2"}, {259, "Num 3"}, {260, "Num 4"}, {261, "Num 5"}, {262, "Num 6"}, {263, "Num 7"}, {264, "Num 8"}, {265, "Num 9"}, {266, "Num ."}, {267, "Num /"}, {268, "Num *"}, {269, "Num -"}, {270, "Num +"}, {271, "Num Enter"}, {272, "Equals"}, {273, "Up"}, {274, "Down"}, {275, "Right"}, {276, "Left"}, {277, "Insert"}, {278, "Home"}, {279, "End"}, {280, "Page Up"}, {281, "Page Down"}, {282, "F1"}, {283, "F2"}, {284, "F3"}, {285, "F4"}, {286, "F5"}, {287, "F6"}, {288, "F7"}, {289, "F8"}, {290, "F9"}, {291, "F10"}, {292, "F11"}, {293, "F12"}, {294, "F13"}, {295, "F14"}, {296, "F15"}, {300, "Num Lock"}, {301, "Caps Lock"}, {302, "Scroll Lock"}, {303, "Right Shift"}, {304, "Left Shift"}, {305, "Right Ctrl"}, {306, "Left Ctrl"}, {307, "Right Alt"}, {308, "Left Alt"}, {311, "Left Super"}, {312, "Right Super"}, {316, "Print Screen"}, {319, "Menu"}
        };

        /// <summary>MUGEN WIN version button code table</summary>
        private static readonly Dictionary<ushort, string> KeyCodeWIN = new Dictionary<ushort, string>()
        {
            {0, "Not Used"}, {1, "Esc"}, {2, "1"}, {3, "2"}, {4, "3"}, {5, "4"}, {6, "5"}, {7, "6"}, {8, "7"}, {9, "8"}, {10, "9"}, {11, "0"}, {12, "-"}, {13, "="}, {14, "Backspace"}, {15, "Tab"}, {16, "Q"}, {17, "W"}, {18, "E"}, {19, "R"}, {20, "T"}, {21, "Y"}, {22, "U"}, {23, "I"}, {24, "O"}, {25, "P"}, {26, "["}, {27, "]"}, {28, "Enter"}, {29, "Left Ctrl"}, {30, "A"}, {31, "S"}, {32, "D"}, {33, "F"}, {34, "G"}, {35, "H"}, {36, "J"}, {37, "K"}, {38, "L"}, {39, ";"}, {40, "'"}, {41, "`"}, {42, "Left Shift"}, {43, "\\"}, {44, "Z"}, {45, "X"}, {46, "C"}, {47, "V"}, {48, "B"}, {49, "N"}, {50, "M"}, {51, ", "}, {52, "."}, {53, "/"}, {54, "Right Shift"}, {55, "Pad *"}, {56, "Left Alt"}, {57, "Space"}, {58, "Caps Lock"}, {59, "F1"}, {60, "F2"}, {61, "F3"}, {62, "F4"}, {63, "F5"}, {64, "F6"}, {65, "F7"}, {66, "F8"}, {67, "F9"}, {68, "F10"}, {69, "Num Lock"}, {70, "Scroll Lock"}, {71, "Pad 7"}, {72, "Pad 8"}, {73, "Pad 9"}, {74, "Pad -"}, {75, "Pad 4"}, {76, "Pad 5"}, {77, "Pad 6"}, {78, "Pad +"}, {79, "Pad 1"}, {80, "Pad 2"}, {81, "Pad 3"}, {82, "Pad 0"}, {83, "Pad ."}, {87, "F11"}, {88, "F12"}, {156, "Pad Enter"}, {157, "Right Ctrl"}, {181, "Pad /"}, {184, "Right Alt"}, {199, "Home"}, {200, "Up"}, {201, "Page Up"}, {203, "Left"}, {205, "Right"}, {207, "End"}, {208, "Down"}, {209, "Page Down"}, {210, "Insert"}, {211, "Delete"}
        };

        private string _keyPressType;
        private ushort _jump;
        private ushort _crouch;
        private ushort _left;
        private ushort _right;
        private ushort _a;
        private ushort _b;
        private ushort _c;
        private ushort _x;
        private ushort _y;
        private ushort _z;
        private ushort _start;

        /// <summary>
        /// Get or set key types
        /// </summary>
        public string KeyPressType
        {
            get { return _keyPressType; }
            set { _keyPressType = value; }
        }

        /// <summary>
        /// Get or set the JUMP button code
        /// </summary>
        public ushort Jump
        {
            get { return _jump; }
            set { _jump = value; }
        }

        /// <summary>
        /// Get or set the CROUCH button code
        /// </summary>
        public ushort Crouch
        {
            get { return _crouch; }
            set { _crouch = value; }
        }

        /// <summary>
        /// Get or set the Left button code
        /// </summary>
        public ushort Left
        {
            get { return _left; }
            set { _left = value; }
        }

        /// <summary>
        /// Get or set up Right button code
        /// </summary>
        public ushort Right
        {
            get { return _right; }
            set { _right = value; }
        }

        /// <summary>
        /// Get or set a button code
        /// </summary>
        public ushort A
        {
            get { return _a; }
            set { _a = value; }
        }

        /// <summary>
        /// Get or set B button code
        /// </summary>
        public ushort B
        {
            get { return _b; }
            set { _b = value; }
        }

        /// <summary>
        /// Get or set C button code
        /// </summary>
        public ushort C
        {
            get { return _c; }
            set { _c = value; }
        }

        /// <summary>
        /// Get or set x button code
        /// </summary>
        public ushort X
        {
            get { return _x; }
            set { _x = value; }
        }

        /// <summary>
        /// Get or set Y button code
        /// </summary>
        public ushort Y
        {
            get { return _y; }
            set { _y = value; }
        }

        /// <summary>
        /// Get or set z button code
        /// </summary>
        public ushort Z
        {
            get { return _z; }
            set { _z = value; }
        }

        /// <summary>
        /// Get or set START button code
        /// </summary>
        public ushort Start
        {
            get { return _start; }
            set { _start = value; }
        }

        /// <summary>
        /// Get Mugen button coding table
        /// </summary>
        public static Dictionary<ushort, string> KeyCode
        {
            get
            {
                if (MugenSetting.Version == MugenSetting.MugenVersion.WIN) return KeyCodeWIN;
                else return KeyCodeV1_X;
            }
        }

        /// <summary>
        /// Create according to the specified button type<see cref="KeyPressSetting"/>New instance
        /// </summary>
        /// <param name="keyPressType">Button type</param>
        /// <exception cref="System.ApplicationException"></exception>
        public KeyPressSetting(string keyPressType)
        {
            KeyPressType = keyPressType;
            ReadKeyPressSetting();
        }

        /// <summary>
        /// Read MuGen button settings
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public void ReadKeyPressSetting()
        {
            if (!File.Exists(MugenSetting.MugenCfgPath)) throw new ApplicationException("Mugen.cfg file does not exist！");
            try
            {
                IniFiles ini = new IniFiles(MugenSetting.MugenCfgPath);
                Jump = (ushort)ini.ReadInteger(KeyPressType, SettingInfo.JumpItem, 0);
                Crouch = (ushort)ini.ReadInteger(KeyPressType, SettingInfo.CrouchItem, 0);
                Left = (ushort)ini.ReadInteger(KeyPressType, SettingInfo.LeftItem, 0);
                Right = (ushort)ini.ReadInteger(KeyPressType, SettingInfo.RightItem, 0);
                A = (ushort)ini.ReadInteger(KeyPressType, SettingInfo.AItem, 0);
                B = (ushort)ini.ReadInteger(KeyPressType, SettingInfo.BItem, 0);
                C = (ushort)ini.ReadInteger(KeyPressType, SettingInfo.CItem, 0);
                X = (ushort)ini.ReadInteger(KeyPressType, SettingInfo.XItem, 0);
                Y = (ushort)ini.ReadInteger(KeyPressType, SettingInfo.YItem, 0);
                Z = (ushort)ini.ReadInteger(KeyPressType, SettingInfo.ZItem, 0);
                Start = (ushort)ini.ReadInteger(KeyPressType, SettingInfo.StartItem, 0);
            }
            catch (Exception)
            {
                throw new ApplicationException("Read the MUGEN button failed！");
            }
        }

        /// <summary>
        /// Save MUGEN button settings
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public void Save()
        {
            if (!File.Exists(MugenSetting.MugenCfgPath)) throw new ApplicationException("Mugen.cfg file does not exist！");
            try
            {
                IniFiles ini = new IniFiles(MugenSetting.MugenCfgPath);
                ini.WriteInteger(KeyPressType, SettingInfo.JumpItem, Jump);
                ini.WriteInteger(KeyPressType, SettingInfo.CrouchItem, Crouch);
                ini.WriteInteger(KeyPressType, SettingInfo.LeftItem, Left);
                ini.WriteInteger(KeyPressType, SettingInfo.RightItem, Right);
                ini.WriteInteger(KeyPressType, SettingInfo.AItem, A);
                ini.WriteInteger(KeyPressType, SettingInfo.BItem, B);
                ini.WriteInteger(KeyPressType, SettingInfo.CItem, C);
                ini.WriteInteger(KeyPressType, SettingInfo.XItem, X);
                ini.WriteInteger(KeyPressType, SettingInfo.YItem, Y);
                ini.WriteInteger(KeyPressType, SettingInfo.ZItem, Z);
                ini.WriteInteger(KeyPressType, SettingInfo.StartItem, Start);
            }
            catch (ApplicationException)
            {
                throw new ApplicationException("Button settings save failed！");
            }
        }

        /// <summary>
        /// Get the specified button coding name
        /// </summary>
        /// <param name="keyCode">Button coding</param>
        /// <returns>Button coding name</returns>
        public static string GetKeyName(ushort keyCode)
        {
            if (KeyCode.ContainsKey(keyCode)) return KeyCode[keyCode];
            else return LeftDelimeter + keyCode + RightDelimeter;
        }

        /// <summary>
        /// Get the specified button coding
        /// </summary>
        /// <param name="keyName">Button coding name</param>
        /// <returns>Button coding</returns>
        /// <exception cref="System.ApplicationException"></exception>
        public static ushort GetKeyCode(string keyName)
        {
            Regex regex = new Regex(@"\((\d+)\)");
            ushort keyCode = 0;
            if (regex.IsMatch(keyName))
            {
                try
                {
                    keyCode = Convert.ToUInt16(regex.Match(keyName).Groups[1].Value);
                }
                catch (Exception)
                {
                    throw new ApplicationException("Button coding format error");
                }
                return keyCode;
            }
            else
            {
                try
                {
                    keyCode = KeyCode.First(k => k.Value.ToLower() == keyName.ToLower()).Key;
                }
                catch (Exception)
                {
                    throw new ApplicationException("Button coding format error");
                }
                return keyCode;
            }
        }
    }

    #endregion MUGEN button configuration class
}