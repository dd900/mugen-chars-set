using SharpCompress.Common;
using SharpCompress.Reader;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MUGENCharsSet
{
    /// <summary>
    /// MUGEN character class
    /// </summary>
    public class Character : IComparable<Character>
    {
        #region Class Constant

        /// <summary>def file extension</summary>
        public const string DefExt = ".def";

        /// <summary>Backup file extension</summary>
        public const string BakExt = ".bak";

        /// <summary>Deleted character file extension</summary>
        public const string DelExt = ".del";

        /// <summary>act file extension</summary>
        public const string ActExt = ".act";

        /// <summary>Character name delimiter</summary>
        public const char NameDelimeter = '"';

        /// <summary>Invalid character name array (used to filter the character list in select.def)</summary>
        public static string[] InvalidCharacterName = new[] { string.Empty, "blank", "empty", "randomselect", "/-", "/" };

        /// <summary>Character compression package extension name array</summary>
        public static string[] ArchiveExt = new[] { ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2" };

        private static string[] _characterDefPathList = null;

        /// <summary>
        /// Character configuration information
        /// </summary>
        public struct SettingInfo
        {
            /// <summary>Info configuration segmentation</summary>
            public const string InfoSection = "Info";

            /// <summary>Files configuration segmentation</summary>
            public const string FilesSection = "Files";

            /// <summary>Data configuration segmentation</summary>
            public const string DataSection = "Data";

            /// <summary>Character name configuration item</summary>
            public const string NameItem = "name";

            /// <summary>Character display name configuration item</summary>
            public const string DisplayNameItem = "displayname";

            /// <summary>cns relative path configuration item</summary>
            public const string CnsItem = "cns";

            /// <summary>Life value configuration items</summary>
            public const string LifeItem = "life";

            /// <summary>Attack power configuration items</summary>
            public const string AttackItem = "attack";

            /// <summary>defense configuration items</summary>
            public const string DefenceItem = "defence";

            /// <summary>gas cap configuration item</summary>
            public const string PowerItem = "power";

            /// <summary>Pal configuration item prefix name</summary>
            public const string PalItemPrefix = "pal";

            /// <summary>Character localcoord configuration item</summary>
            public const string LocalcoordItem = "localcoord";

            /// <summary>stcommon relative path configuration item</summary>
            public const string StcommonItem = "stcommon";

            /// <summary>sprite relative path configuration item</summary>
            public const string SpriteItem = "sprite";
        }

        #endregion Class Constant

        #region Class Private Member

        private string _defPath;
        private string _cns;
        private string _name;
        private string _displayName;
        private int _life;
        private int _attack;
        private int _defence;
        private int _power;
        private Dictionary<string, string> _palList;
        private bool _isWideScreen;
        private string _stcommon;
        private string _spritePath;
        private SpriteFile.SffVerion _spriteVersion;
        private SpriteFile _sprite;
        private bool _isInSelectDef;

        #endregion Class Private Member

        #region class attributes

        /// <summary>
        /// Get or set the absolute path of the def file
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public string DefPath
        {
            get { return _defPath; }
            private set
            {
                if (value == string.Empty) throw new ApplicationException("The def path must not be empty!");
                _defPath = value;
            }
        }

        // <summary>
        /// Get or set the relative path of cns
        /// </summary>
        public string Cns
        {
            get { return _cns; }
            private set { _cns = value; }
        }

        /// <summary>
        /// Get the absolute path of cns
        /// </summary>
        public string CnsFullPath
        {
            get { return DefPath.GetDirPathOfFile() + Cns; }
        }

        /// <summary>
        /// Get or set the character name
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public string Name
        {
            get { return _name; }
            set
            {
                if (value == string.Empty) throw new ApplicationException("The character name cannot be empty!");
                if (value == MainForm.MultiValue) return;
                _name = value;
            }
        }

        /// <summary>
        /// Get or set the display name
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public string DisplayName
        {
            get { return _displayName; }
            set
            {
                if (value == string.Empty) throw new ApplicationException("The display name cannot be empty!");
                if (value == MainForm.MultiValue) return;
                _displayName = value;
            }
        }

        /// <summary>
        /// Get the name displayed on the character list control
        /// </summary>
        public string ItemName
        {
            get
            {
                StringBuilder name = new StringBuilder(6);
                if (AppConfig.ShowCharacterScreenMark && MugenSetting.Version != MugenSetting.MugenVersion.WIN)
                {
                    if (MugenSetting.IsWideScreen)
                    {
                        if (!IsWideScreen) name.Append("(normal)");
                    }
                    else
                    {
                        if (IsWideScreen) name.Append("(wide)");
                    }
                }
                if (!IsInSelectDef)
                {
                    name.Append("(no def)");
                }
                return Name + " " + name.ToString();
            }
        }

        /// <summary>
        /// Get or set health
        /// </summary>
        public int Life
        {
            get { return _life; }
            set
            {
                if (value == 0) return;
                _life = value;
            }
        }

        /// <summary>
        /// Get or set attack power
        /// </summary>
        public int Attack
        {
            get { return _attack; }
            set
            {
                if (value == 0) return;
                _attack = value;
            }
        }

        /// <summary>
        /// Get or set defense
        /// </summary>
        public int Defence
        {
            get { return _defence; }
            set
            {
                if (value == 0) return;
                _defence = value;
            }
        }

        /// <summary>
        /// Get or set power
        /// </summary>
        public int Power
        {
            get { return _power; }
            set
            {
                if (value == 0) return;
                _power = value;
            }
        }

        /// <summary>
        /// Get or set the list of Pal relative path key-value pairs
        /// </summary>
        public Dictionary<string, string> PalList
        {
            get { return _palList; }
            set { _palList = value; }
        }

        /// <summary>
        /// Get the relative path list of all current optional act files
        /// </summary>
        public string[] SelectableActFileList
        {
            get
            {
                string path = DefPath.GetDirPathOfFile();
                if (!Directory.Exists(path)) return null;
                List<string> actArr = new List<string>();
                ScanActList(actArr, path);
                string[] tempActList = new string[actArr.Count];
                actArr.CopyTo(tempActList, 0);
                return tempActList;
            }
        }

        /// <summary>
        /// Get or set whether the character pack is widescreen
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public bool IsWideScreen
        {
            get { return _isWideScreen; }
            set
            {
                if (value)
                {
                    try
                    {
                        IniFiles ini = new IniFiles(DefPath);
                        int width = 427;
                        int height = 240;
                        ini.WriteString(SettingInfo.InfoSection, SettingInfo.LocalcoordItem, string.Format("{0},{1}", width, height));
                    }
                    catch (ApplicationException)
                    {
                        throw new ApplicationException("Widescreen character pack conversion failed！");
                    }
                }
                else
                {
                    try
                    {
                        IniFiles ini = new IniFiles(DefPath);
                        ini.DeleteKey(SettingInfo.InfoSection, SettingInfo.LocalcoordItem);
                    }
                    catch (ApplicationException)
                    {
                        throw new ApplicationException("Failed to convert normal screen character pack！");
                    }
                }
                _isWideScreen = value;
            }
        }

        /// <summary>
        /// Get or set the relative path of stcommon
        /// </summary>
        public string Stcommon
        {
            get { return _stcommon; }
            set { _stcommon = value; }
        }

        /// <summary>
        /// Get or set the relative path of the character sprite file
        /// </summary>
        public string SpritePath
        {
            get { return _spritePath; }
            set { _spritePath = value; }
        }

        /// <summary>
        /// Get or set the version of the character sprite file
        /// </summary>
        public SpriteFile.SffVerion SpriteVersion
        {
            get { return _spriteVersion; }
            set { _spriteVersion = value; }
        }

        /// <summary>
        /// Get or set the character sprite
        /// </summary>
        public SpriteFile Sprite
        {
            get { return _sprite; }
            set { _sprite = value; }
        }

        /// <summary>
        /// Get character model image
        /// </summary>
        public Bitmap SpriteImage
        {
            get
            {
                if (Sprite == null) return null;
                if (Sprite.FirstSubNode == null) return null;
                if (Sprite.FirstSubNode.ImageData == null) return null;
                ImagePcx pcx = new ImagePcx(Sprite.FirstSubNode.ImageData);
                Bitmap image = pcx.PcxImage;
                try
                {
                    image.MakeTransparent(image.GetPixel(0, 0));
                }
                catch (Exception)
                {
                    return null;
                }
                return image;
            }
        }

        /// <summary>
        /// Get or set whether this character is in the character list in the select.def file
        /// </summary>
        public bool IsInSelectDef
        {
            get { return _isInSelectDef; }
            set { _isInSelectDef = value; }
        }

        /// <summary>
        /// Get a list of the absolute path of the character def file in the select.def file
        /// </summary>
        public static string[] CharacterDefPathList
        {
            get { return _characterDefPathList; }
        }

        #endregion class attributes

        /// <summary>
        /// Create a new instance of the <see cref="Character"/> class according to the specified def file path
        /// </summary>
        /// <param name="defPath">Character def file absolute path</param>
        /// <exception cref="System.ApplicationException"></exception>
        public Character(string defPath)
        {
            DefPath = defPath.GetBackSlashPath();
            ReadCharacterSetting();
            PalList = null;
        }

        #region Class Method

        /// <summary>
        /// Used to compare the size of two Character classes
        /// </summary>
        /// <param name="other">the person to compare</param>
        /// <returns>Result of comparison</returns>
        public int CompareTo(Character other)
        {
            return Name.CompareTo(other.Name);
        }

        /// <summary>
        /// Check whether the character def file of this character instance is the same as the specified path
        /// </summary>
        /// <param name="defPath">absolute path of character def file</param>
        /// <returns></returns>
        public bool Equals(string defPath)
        {
            if (DefPath.ToLower().GetBackSlashPath() == defPath.ToLower().GetBackSlashPath())
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Read character attribute settings
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public void ReadCharacterSetting()
        {
            if (!File.Exists(DefPath)) throw new ApplicationException("Character def file does not exist！");
            IniFiles ini = new IniFiles(DefPath);
            Name = GetTrimName(ini.ReadString(SettingInfo.InfoSection, SettingInfo.NameItem, ""));
            DisplayName = GetTrimName(ini.ReadString(SettingInfo.InfoSection, SettingInfo.DisplayNameItem, ""));
            Cns = ini.ReadString(SettingInfo.FilesSection, SettingInfo.CnsItem, "").GetBackSlashPath();
            _isWideScreen = ini.ReadString(SettingInfo.InfoSection, SettingInfo.LocalcoordItem, "") != string.Empty;
            Stcommon = ini.ReadString(SettingInfo.FilesSection, SettingInfo.StcommonItem, "");
            SpritePath = ini.ReadString(SettingInfo.FilesSection, SettingInfo.SpriteItem, "");
            if (!File.Exists(CnsFullPath)) throw new ApplicationException("Character cns file does not exist！");
            ini = new IniFiles(CnsFullPath);
            Life = ini.ReadInteger(SettingInfo.DataSection, SettingInfo.LifeItem, 0);
            Attack = ini.ReadInteger(SettingInfo.DataSection, SettingInfo.AttackItem, 0);
            Defence = ini.ReadInteger(SettingInfo.DataSection, SettingInfo.DefenceItem, 0);
            Power = ini.ReadInteger(SettingInfo.DataSection, SettingInfo.PowerItem, 0);
            if (CharacterDefPathList != null && CharacterDefPathList.Contains(DefPath.ToLower()))
                IsInSelectDef = true;
        }

        /// <summary>
        /// Read Pal settings
        /// </summary>
        public void ReadPalSetting()
        {
            IniFiles ini = new IniFiles(DefPath);
            NameValueCollection tempDict = new NameValueCollection();
            ini.ReadSectionValues(SettingInfo.FilesSection, tempDict);
            PalList = new Dictionary<string, string>();
            foreach (string key in tempDict)
            {
                if (key.StartsWith(SettingInfo.PalItemPrefix, StringComparison.CurrentCultureIgnoreCase))
                {
                    PalList.Add(key, tempDict[key]);
                }
            }
        }

        /// <summary>
        /// Scan all act files in the current character folder
        /// </summary>
        /// <param name="actList">act file relative path list</param>
        /// <param name="dir">act folder absolute path</param>
        private void ScanActList(List<string> actList, string dir)
        {
            if (!Directory.Exists(dir)) return;
            string[] tempPalFiles = Directory.GetFiles(dir, "*" + ActExt);
            for (int i = 0; i < tempPalFiles.Length; i++)
            {
                tempPalFiles[i] = tempPalFiles[i].Substring(DefPath.GetDirPathOfFile().Length).GetSlashPath();
            }
            actList.AddRange(tempPalFiles);
            string[] tempDirs = Directory.GetDirectories(dir);
            foreach (string tempDir in tempDirs)
            {
                ScanActList(actList, tempDir);
            }
        }

        /// <summary>
        /// Save character settings
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public void Save()
        {
            if (!File.Exists(DefPath)) throw new ApplicationException("Character def file does not exist！");
            IniFiles ini;
            try
            {
                if (!Tools.SetFileNotReadOnly(DefPath)) throw new ApplicationException();
                ini = new IniFiles(DefPath);
                ini.WriteString(SettingInfo.InfoSection, SettingInfo.NameItem, GetDelimeterName(Name));
                ini.WriteString(SettingInfo.InfoSection, SettingInfo.DisplayNameItem, GetDelimeterName(DisplayName));
            }
            catch (ApplicationException)
            {
                throw new ApplicationException("Failed to write character def file！");
            }
            try
            {
                foreach (string key in PalList.Keys)
                {
                    ini.WriteString(SettingInfo.FilesSection, key, PalList[key]);
                }
            }
            catch (ApplicationException)
            {
                try
                {
                    ReadPalSetting();
                }
                catch (ApplicationException) { }
                throw new ApplicationException("Failed to write Pal configuration items！");
            }
            if (!File.Exists(CnsFullPath)) throw new ApplicationException("Character cns file does not exist！");
            int total = MultiSave(new Character[] { this });
            if (total == 0) throw new ApplicationException("Failed to write character cns file！");
        }

        /// <summary>
        /// Back up character settings
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public void Backup()
        {
            if (!File.Exists(DefPath)) throw new ApplicationException("Character def file does not exist！");
            if (!File.Exists(CnsFullPath)) throw new ApplicationException("Character cns file does not exist！");
            try
            {
                if (!Tools.SetFileNotReadOnly(DefPath + BakExt)) throw new Exception();
                File.Copy(DefPath, DefPath + BakExt, true);
                if (!Tools.SetFileNotReadOnly(CnsFullPath + BakExt)) throw new Exception();
                File.Copy(CnsFullPath, CnsFullPath + BakExt, true);
            }
            catch (Exception)
            {
                throw new ApplicationException("Character backup failed！");
            }
        }

        /// <summary>
        /// Restore character settings
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public void Restore()
        {
            if (!File.Exists(DefPath + BakExt)) throw new ApplicationException("Character def backup file does not exist！");
            if (!File.Exists(CnsFullPath + BakExt)) throw new ApplicationException("Character cns backup file does not exist！");
            try
            {
                if (!Tools.SetFileNotReadOnly(DefPath)) throw new Exception();
                File.Copy(DefPath + BakExt, DefPath, true);
                if (!Tools.SetFileNotReadOnly(CnsFullPath)) throw new Exception();
                File.Copy(CnsFullPath + BakExt, CnsFullPath, true);
                ReadCharacterSetting();
            }
            catch (Exception)
            {
                throw new ApplicationException("Character recovery failed！");
            }
        }

        /// <summary>
        /// Delete characters (rename the def file to a file with a specific suffix)
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public void Delete()
        {
            if (!File.Exists(DefPath)) throw new ApplicationException("Character def file does not exist！");
            try
            {
                if (!Tools.SetFileNotReadOnly(DefPath + DelExt)) throw new Exception();
                File.Copy(DefPath, DefPath + DelExt, true);
                if (!Tools.SetFileNotReadOnly(DefPath)) throw new Exception();
                File.Delete(DefPath);
            }
            catch (Exception)
            {
                throw new ApplicationException("Character deletion failed！");
            }
            DeleteCharacterListInSelectDef(new Character[] { this });
        }

        /// <summary>
        /// Convert to widescreen character pack
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public void ConvertToWideScreen()
        {
            if (!File.Exists(DefPath)) throw new ApplicationException("Character def file does not exist！");
            try
            {
                IsWideScreen = true;
                string stcommonPath = (DefPath.GetDirPathOfFile() + Stcommon).GetBackSlashPath();
                if (File.Exists(stcommonPath))
                {
                    StcommonConvertToWideScreen(stcommonPath);
                }
            }
            catch (ApplicationException)
            {
                throw new ApplicationException("Widescreen character pack conversion failed！");
            }
        }

        /// <summary>
        /// 转换为普屏人物包
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        public void ConvertToNormalScreen()
        {
            if (!File.Exists(DefPath)) throw new ApplicationException("Character def file does not exist！");
            try
            {
                IsWideScreen = false;
                string stcommonPath = (DefPath.GetDirPathOfFile() + Stcommon).GetBackSlashPath();
                if (File.Exists(stcommonPath))
                {
                    StcommonConvertToNormalScreen(stcommonPath);
                }
            }
            catch (ApplicationException)
            {
                throw new ApplicationException("Failed to convert normal screen character pack！");
            }
        }

        /// <summary>
        /// Read character model
        /// </summary>
        /// <returns>Whether the reading was successful</returns>
        public bool ReadSpriteFile()
        {
            string sffPath = DefPath.GetDirPathOfFile() + SpritePath.GetBackSlashPath();
            if (!File.Exists(sffPath)) return false;
            try
            {
                Sprite = new SpriteFile(sffPath);
            }
            catch (ApplicationException)
            {
                return false;
            }
            SpriteVersion = Sprite.Version;
            return true;
        }

        /// <summary>
        /// Get the character model image replaced by the specified color table
        /// </summary>
        /// <param name="actPath">The relative path of the color table file</param>
        /// <returns>Character model image</returns>
        public Bitmap GetSpriteImageWithPal(string path)
        {
            string actPath = DefPath.GetDirPathOfFile() + path.GetBackSlashPath();
            if (!File.Exists(actPath)) return null;
            if (Sprite == null) return null;
            if (Sprite.FirstSubNode == null) return null;
            if (Sprite.FirstSubNode.ImageData == null) return null;
            FileStream fs = null;
            BinaryReader br = null;
            byte[] newImageData = null;
            try
            {
                fs = new FileStream(actPath, FileMode.Open);
                br = new BinaryReader(fs);
                byte[] actData = br.ReadBytes((int)fs.Length);
                newImageData = Sprite.FirstSubNode.ImageData.Take(Sprite.FirstSubNode.ImageData.Length - (int)fs.Length).Concat(actData.Reverse()).ToArray();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                if (br != null) br.Close();
                if (fs != null) fs.Close();
            }
            if (newImageData == null) return null;
            ImagePcx pcx = new ImagePcx(newImageData);
            Bitmap image = pcx.PcxImage;
            try
            {
                image.MakeTransparent(image.GetPixel(0, 0));
            }
            catch (Exception)
            {
                return null;
            }
            return image;
        }

        #endregion Class Method

        #region Class static method

        /// <summary>
        /// Get the character name without delimiter
        /// </summary>
        /// <param name="name">person’s name</param>
        /// <returns>Character name</returns>
        public static string GetTrimName(string name)
        {
            return name.Trim(NameDelimeter).Trim();
        }

        /// <summary>
        /// Get the character name with delimiter at the beginning and end
        /// </summary>
        /// <param name="name">person’s name</param>
        /// <returns>Character name</returns>
        public static string GetDelimeterName(string name)
        {
            return NameDelimeter + GetTrimName(name) + NameDelimeter;
        }

        /// <summary>
        /// Save characters in batch
        /// </summary>
        /// <param name="characterList">characterList</param>
        /// <returns>Total number of successful modifications</returns>
        public static int MultiSave(Character[] characterList)
        {
            int total = 0;
            IniFiles ini;
            foreach (Character character in characterList)
            {
                if (!File.Exists(character.DefPath)) continue;
                try
                {
                    if (!Tools.SetFileNotReadOnly(character.DefPath)) continue;
                    ini = new IniFiles(character.CnsFullPath);
                    if (character.Life != 0)
                    {
                        ini.WriteInteger(SettingInfo.DataSection, SettingInfo.LifeItem, character.Life);
                    }
                    if (character.Attack != 0)
                    {
                        ini.WriteInteger(SettingInfo.DataSection, SettingInfo.AttackItem, character.Attack);
                    }
                    if (character.Defence != 0)
                    {
                        ini.WriteInteger(SettingInfo.DataSection, SettingInfo.DefenceItem, character.Defence);
                    }
                    if (character.Power != 0)
                    {
                        ini.WriteInteger(SettingInfo.DataSection, SettingInfo.PowerItem, character.Power);
                    }
                }
                catch (ApplicationException)
                {
                    try
                    {
                        character.ReadCharacterSetting();
                    }
                    catch (ApplicationException) { }
                    continue;
                }
                total++;
            }
            return total;
        }

        /// <summary>
        /// Back up people in batches
        /// </summary>
        /// <param name="characterList">CharacterList</param>
        /// <returns>Total number of successful backups</returns>
        public static int MultiBackup(Character[] characterList)
        {
            int total = 0;
            foreach (Character character in characterList)
            {
                try
                {
                    character.Backup();
                    total++;
                }
                catch (ApplicationException)
                {
                    continue;
                }
            }
            return total;
        }

        /// <summary>
        /// Restore characters in batch
        /// </summary>
        /// <param name="characterList">CharacterList</param>
        /// <returns>Total number of successful restores</returns>
        public static int MultiRestore(Character[] characterList)
        {
            int total = 0;
            foreach (Character character in characterList)
            {
                try
                {
                    character.Restore();
                    total++;
                }
                catch (ApplicationException)
                {
                    continue;
                }
            }
            return total;
        }

        /// <summary>
        /// Delete characters in batch
        /// </summary>
        /// <param name="characterList">characterList</param>
        /// <returns>Total number of successful deletions</returns>
        public static int MultiDelete(Character[] characterList)
        {
            int total = 0;
            foreach (Character character in characterList)
            {
                try
                {
                    character.Delete();
                    total++;
                }
                catch (ApplicationException)
                {
                    continue;
                }
            }
            DeleteCharacterListInSelectDef(characterList);
            return total;
        }

        /// <summary>
        /// Batch convert to widescreen character pack
        /// </summary>
        /// <param name="characterList">CharacterList</param>
        /// <returns>Total number of successful conversions</returns>
        public static int MultiConvertToWideScreen(Character[] characterList)
        {
            int total = 0;
            foreach (Character character in characterList)
            {
                try
                {
                    character.ConvertToWideScreen();
                    total++;
                }
                catch (ApplicationException)
                {
                    continue;
                }
            }
            return total;
        }

        /// <summary>
        /// Batch convert to normal screen character pack
        /// </summary>
        /// <param name="characterList">CharacterList</param>
        /// <returns>Total number of successful conversions</returns>
        public static int MultiConvertToNormalScreen(Character[] characterList)
        {
            int total = 0;
            foreach (Character character in characterList)
            {
                try
                {
                    character.ConvertToNormalScreen();
                    total++;
                }
                catch (ApplicationException)
                {
                    continue;
                }
            }
            return total;
        }

        /// <summary>
        /// Modify the gravity in the stcommon file to a state suitable for widescreen
        /// </summary>
        /// <param name="stcommonPath">stcommon file absolute path</param>
        /// <exception cref="System.ApplicationException"></exception>
        public static void StcommonConvertToWideScreen(string stcommonPath)
        {
            if (!File.Exists(stcommonPath)) throw new ApplicationException("stcommon file does not exist！");
            try
            {
                string content = File.ReadAllText(stcommonPath, Encoding.Default);
                Regex regex = new Regex(@"yaccel\s*\)(\s*/\s*\d+)?(\.\d+)?", RegexOptions.IgnoreCase);
                content = regex.Replace(content, "yaccel)/1.2");
                File.WriteAllText(stcommonPath, content, Encoding.Default);
            }
            catch (Exception)
            {
                throw new ApplicationException("stcommon file conversion to widescreen failed！");
            }
        }

        /// <summary>
        /// Modify the gravity in the stcommon file to a state suitable for general screen
        /// </summary>
        /// <param name="stcommonPath">stcommon file absolute path</param>
        /// <exception cref="System.ApplicationException"></exception>
        public static void StcommonConvertToNormalScreen(string stcommonPath)
        {
            if (!File.Exists(stcommonPath)) throw new ApplicationException("stcommon file does not exist！");
            try
            {
                string content = File.ReadAllText(stcommonPath, Encoding.Default);
                Regex regex = new Regex(@"yaccel\s*\)\s*/\s*\d+(\.\d+)?", RegexOptions.IgnoreCase);
                content = regex.Replace(content, "yaccel)");
                File.WriteAllText(stcommonPath, content, Encoding.Default);
            }
            catch (Exception)
            {
                throw new ApplicationException("Failed to convert stcommon file to normal screen！");
            }
        }

        /// <summary>
        /// Get whether the gravity in the stcommon file is suitable for widescreen
        /// </summary>
        /// <param name="stcommonContent">stcommon file content</param>
        /// <returns>Status value (-1: no related items found, 0: normal screen, 1: wide screen)</returns>
        public static int IsStcommonWideScreen(string stcommonContent)
        {
            try
            {
                if (!(new Regex("yaccel", RegexOptions.IgnoreCase)).IsMatch(stcommonContent)) return -1;
                if ((new Regex(@"yaccel\s*\)\s*/\s*\d+(\.\d+)?", RegexOptions.IgnoreCase)).IsMatch(stcommonContent)) return 1;
                else return 0;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Get the list of people by scanning the people folder
        /// </summary>
        /// <param name="characterList">characterList</param>
        /// <param name="charsDir">CharsDir">chars folder</param>
        public static void ScanCharacterDir(List<Character> characterList, string charsDir)
        {
            string[] tempDefList = Directory.GetFiles(charsDir, "*" + Character.DefExt);
            foreach (string tempDefPath in tempDefList)
            {
                System.Windows.Forms.Application.DoEvents();
                try
                {
                    characterList.Add(new Character(tempDefPath));
                }
                catch (ApplicationException)
                {
                    continue;
                }
            }
            string[] tempDirArr = Directory.GetDirectories(charsDir);
            foreach (string tempDir in tempDirArr)
            {
                ScanCharacterDir(characterList, tempDir);
            }
        }

        /// <summary>
        /// Get the list of characters by reading the select.def file
        /// </summary>
        /// <param name="characterList">characterList</param>
        /// <exception cref="System.ApplicationException"></exception>
        public static void ReadCharacterListInSelectDef(List<Character> characterList)
        {
            if (!File.Exists(MugenSetting.SelectDefPath)) throw new ApplicationException("select.def file does not exist！");
            foreach (string defPath in CharacterDefPathList)
            {
                System.Windows.Forms.Application.DoEvents();
                try
                {
                    characterList.Add(new Character(defPath));
                }
                catch (ApplicationException)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Read the list of the absolute path of the character def file in the select.def file
        /// </summary>
        /// <returns>List of absolute paths of character def files</returns>
        public static void ReadCharacterDefPathListInSelectDef()
        {
            _characterDefPathList = null;
            if (!File.Exists(MugenSetting.SelectDefPath)) return;
            Tools.IniFileStandardization(MugenSetting.SelectDefPath);
            string[] characterLines = null;
            try
            {
                string defContent = File.ReadAllText(MugenSetting.SelectDefPath, Encoding.Default);
                Regex regex = new Regex(@"\[Characters\](.*)\r\n\[ExtraStages\]", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                characterLines = regex.Match(defContent).Groups[1].Value.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch (Exception)
            {
                return;
            }
            if (characterLines.Length == 0) return;
            List<string> characterList = new List<string>();
            foreach (string tempLine in characterLines)
            {
                string line = tempLine.Trim();
                line = line.Split(new string[] { IniFiles.CommentMark }, 2, StringSplitOptions.None)[0];
                line = line.Split(new string[] { "," }, 2, StringSplitOptions.None)[0].Trim();
                if (InvalidCharacterName.Contains(line.ToLower())) continue;
                if (Path.GetExtension(line.ToLower()) != Character.DefExt) line = line.GetFormatDirPath() + line + Character.DefExt;
                string defPath = (MugenSetting.MugenCharsDirPath + line.GetBackSlashPath()).ToLower();
                if (!File.Exists(defPath)) continue;
                if (!characterList.Contains(defPath))
                {
                    characterList.Add(defPath);
                }
            }
            _characterDefPathList = characterList.ToArray();
        }

        /// <summary>
        /// Delete the list of characters in the select.def file
        /// </summary>
        /// <param name="characterList">characterList</param>
        public static void DeleteCharacterListInSelectDef(Character[] characterList)
        {
            if (!File.Exists(MugenSetting.SelectDefPath)) return;
            string fileContent = "";
            string oriCharacterContent = "";
            string[] characterLines = null;
            try
            {
                fileContent = File.ReadAllText(MugenSetting.SelectDefPath, Encoding.Default);
                Regex regex = new Regex(@"(\[Characters\].*)\r\n\[ExtraStages\]", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                oriCharacterContent = regex.Match(fileContent).Groups[1].Value;
                characterLines = oriCharacterContent.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            }
            catch (Exception)
            {
                return;
            }
            if (characterLines.Length <= 1) return;
            for (int i = 1; i < characterLines.Length; i++)
            {
                string line = characterLines[i].Trim();
                line = line.Split(new string[] { IniFiles.CommentMark }, 2, StringSplitOptions.None)[0];
                line = line.Split(new string[] { "," }, 2, StringSplitOptions.None)[0].Trim();
                if (InvalidCharacterName.Contains(line.ToLower())) continue;
                if (Path.GetExtension(line.ToLower()) != Character.DefExt) line = line.GetFormatDirPath() + line + Character.DefExt;
                string defPath = MugenSetting.MugenCharsDirPath + line.GetBackSlashPath();
                foreach (Character character in characterList)
                {
                    if (character.Equals(defPath))
                    {
                        characterLines[i] = IniFiles.CommentMark + characterLines[i];
                        break;
                    }
                }
            }
            fileContent = fileContent.Replace(oriCharacterContent, string.Join("\r\n", characterLines));
            try
            {
                File.WriteAllText(MugenSetting.SelectDefPath, fileContent, Encoding.Default);
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Add the characters to the select.def file
        /// </summary>
        /// <param name="characterList">characterList</param>
        /// <returns>Total number of successful additions</returns>
        public static bool AddCharacterToSelectDef(Character[] characterList)
        {
            if (!File.Exists(MugenSetting.SelectDefPath)) return false;
            Tools.IniFileStandardization(MugenSetting.SelectDefPath);
            string fileContent = "";
            string oriCharacterContent = "";
            try
            {
                fileContent = File.ReadAllText(MugenSetting.SelectDefPath, Encoding.Default);
                Regex regex = new Regex(@"(\[Characters\].*?)(\r\n)+(;-*)?(\r\n)+\[ExtraStages\]", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                oriCharacterContent = regex.Match(fileContent).Groups[1].Value;
            }
            catch (Exception)
            {
                return false;
            }
            StringBuilder newCharacterContent = new StringBuilder();
            foreach (Character character in characterList)
            {
                if (CharacterDefPathList != null && CharacterDefPathList.Contains(character.DefPath.ToLower())) continue;
                newCharacterContent.Append("\r\n" + character.DefPath.Substring(MugenSetting.MugenCharsDirPath.Length).GetSlashPath());
                character.IsInSelectDef = true;
            }
            fileContent = fileContent.Replace(oriCharacterContent, oriCharacterContent + newCharacterContent);
            try
            {
                File.WriteAllText(MugenSetting.SelectDefPath, fileContent, Encoding.Default);
            }
            catch (Exception)
            {
                foreach (Character character in characterList)
                {
                    if (CharacterDefPathList != null && CharacterDefPathList.Contains(character.DefPath.ToLower())) continue;
                    character.IsInSelectDef = false;
                }
                return false;
            }
            ReadCharacterDefPathListInSelectDef();
            return true;
        }

        /// <summary>
        /// Unzip the character compressed package
        /// </summary>
        /// <param name="archivePath">absolute path of character compressed package</param>
        /// <returns>The absolute path of the new character folder created</returns>
        /// <exception cref="System.ApplicationException"></exception>
        public static string DecompressionCharacterArchive(string archivePath)
        {
            if (!File.Exists(archivePath)) throw new ApplicationException("The compressed package does not exist！");
            string tempDirPath = MugenSetting.MugenCharsDirPath + "MugenCharsSetTemp\\";
            DirectoryInfo tempDir;
            try
            {
                tempDir = Directory.CreateDirectory(tempDirPath);
            }
            catch (Exception)
            {
                throw new ApplicationException("Failed to create temporary folder！");
            }
            FileStream fs = null;
            try
            {
                fs = File.OpenRead(archivePath);
                IReader reader = ReaderFactory.Open(fs);
                while (reader.MoveToNextEntry())
                {
                    System.Windows.Forms.Application.DoEvents();
                    if (!reader.Entry.IsDirectory)
                    {
                        try
                        {
                            reader.WriteEntryToDirectory(tempDirPath, ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception)
            {
                try
                {
                    tempDir.Delete(true);
                }
                catch (Exception) { }
                throw new ApplicationException("Unzip failed！");
            }
            finally
            {
                if (fs != null) fs.Close();
            }
            string destDirPath = "";
            try
            {
                if (tempDir.GetDirectories().Length == 1 && tempDir.GetFiles().Length == 0)
                {
                    destDirPath = Tools.GetNonExistsDirPath(MugenSetting.MugenCharsDirPath, tempDir.GetDirectories()[0].Name);
                    Directory.Move(tempDir.GetDirectories()[0].FullName, destDirPath);
                    tempDir.Delete(true);
                }
                else
                {
                    destDirPath = Tools.GetNonExistsDirPath(MugenSetting.MugenCharsDirPath, Path.GetFileNameWithoutExtension(archivePath));
                    tempDir.MoveTo(destDirPath);
                }
            }
            catch (Exception)
            {
                try
                {
                    tempDir.Delete(true);
                }
                catch (Exception) { }
                throw new ApplicationException("Failed to move folder！");
            }
            return destDirPath;
        }

        #endregion Class static method
    }
}