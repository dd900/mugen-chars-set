using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MUGENCharsSet
{
    /// <summary>
    /// Program main window class
    /// </summary>
    public partial class MainForm : Form
    {
        #region Typical

        /// <summary>Multi-value display value</summary>
        public const string MultiValue = "(Multivalued)";

        /// <summary>Number of columns of the serial number</summary>
        public const int PalNoColumnNo = 0;

        /// <summary>The number of columns of the PAL value</summary>
        public const int PalValColumnNo = 1;

        #endregion Typical

        #region Private variable

        private bool _modifyEnabled = false;
        private bool _multiModified = false;
        private List<Character> _characterList;
        private bool _characterListControlPreparing = false;

        #endregion Private variable

        #region Class properties

        /// <summary>
        /// Get or set a list of Mugen characters
        /// </summary>
        private List<Character> CharacterList
        {
            get { return _characterList; }
            set { _characterList = value; }
        }

        /// <summary>
        /// Get or set whether the character list control is in the configuration Data Source preparation process
        /// </summary>
        private bool CharacterListControlPreparing
        {
            get { return _characterListControlPreparing; }
            set { _characterListControlPreparing = value; }
        }

        /// <summary>
        /// Get or set to enter bulk modification mode
        /// </summary>
        private bool MultiModified
        {
            get { return _multiModified; }
            set
            {
                if (value)
                {
                    txtName.ReadOnly = true;
                    txtDisplayName.ReadOnly = true;
                    grpPal.Enabled = false;
                    btnRestore.Enabled = false;
                    cboSelectableActFileList.Visible = false;
                    ResetCharacterControls();
                }
                else
                {
                    txtName.ReadOnly = false;
                    txtDisplayName.ReadOnly = false;
                    grpPal.Enabled = true;
                    cboSelectableActFileList.Visible = true;
                }
                _multiModified = value;
            }
        }

        /// <summary>
        /// Get or set to enter modification mode
        /// </summary>
        private bool ModifyEnabled
        {
            get { return _modifyEnabled; }
            set
            {
                if (value)
                {
                    btnModify.Enabled = true;
                    btnReset.Enabled = true;
                    btnBackup.Enabled = true;
                    btnRestore.Enabled = false;
                }
                else
                {
                    btnModify.Enabled = false;
                    btnReset.Enabled = false;
                    btnBackup.Enabled = false;
                    btnRestore.Enabled = false;
                    ResetCharacterControls();
                }
                _modifyEnabled = value;
            }
        }

        #endregion Class properties

        public MainForm()
        {
            InitializeComponent();
        }

        #region Class method

        /// <summary>
        /// Show operation success message
        /// </summary>
        /// <param name="msg">information</param>
        private void ShowSuccessMsg(string msg)
        {
            MessageBox.Show(msg, "Successful operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// Display operation failure message
        /// </ summary>
        /// <param name = "msg"> Message </ param>
        private void ShowErrorMsg(string msg)
        {
            MessageBox.Show(msg, "operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Resets Character Attribute Settings and Color Table Settings Controls
        /// </summary>
        private void ResetCharacterControls()
        {
            lblDefPath.Text = "";
            ttpCommon.SetToolTip(lblDefPath, "");
            foreach (Control ctlTemp in grpProperty.Controls)
            {
                if (ctlTemp is TextBox)
                    ((TextBox)ctlTemp).Clear();
            }
            picSpriteImage.Image = null;
            lblSpriteVersion.Text = "";
            cboSelectableActFileList.Items.Clear();
            dgvPal.Rows.Clear();
            ((DataGridViewComboBoxColumn)dgvPal.Columns[1]).Items.Clear();
        }

        /// <summary>
        /// Read program configuration
        /// </summary>
        private void ReadConfig()
        {
            if (!AppConfig.Read())
            {
                ShowErrorMsg("Read the configuration file failed！");
            }
            chkAutoSort.Checked = AppConfig.AutoSort;
            cboReadCharacterType.SelectedIndex = (int)AppConfig.ReadCharacterType;
        }

        /// <summary>
        /// Read the list of characters
        /// </summary>
        public void ReadCharacterList(bool showProgress = false)
        {
            ModifyEnabled = false;
            MultiModified = false;
            lstCharacterList.DataSource = null;
            txtKeyword.Clear();
            lblMugenInfo.Text = "";
            lblCharacterCount.Text = "";
            lblCharacterSelectCount.Text = "";
            fswCharacterCns.EnableRaisingEvents = false;
            try
            {
                MugenSetting.ReadMugenSetting();
            }
            catch (ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            if (!Directory.Exists(MugenSetting.MugenCharsDirPath))
            {
                ShowErrorMsg("Unable to find Mugen character folder！");
                return;
            }
            Panel pnlProgress = null;
            if (showProgress)
            {
                pnlProgress = ShowProgressPanel(this);
            }
            Character.ReadCharacterDefPathListInSelectDef();
            CharacterList = new List<Character>();
            try
            {
                if (AppConfig.ReadCharacterType == AppConfig.ReadCharTypeEnum.CharsDir)
                {
                    Character.ScanCharacterDir(CharacterList, MugenSetting.MugenCharsDirPath);
                }
                else
                {
                    Character.ReadCharacterListInSelectDef(CharacterList);
                }
            }
            catch (ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            if (AppConfig.AutoSort)
            {
                CharacterList.Sort();
            }
            if (CharacterList.Count != 0)
            {
                RefreshCharacterListDataSource(CharacterList);
            }
            lblMugenInfo.Text = string.Format("Mugen version：{0}  Package：{1}Screen", MugenSetting.Version == MugenSetting.MugenVersion.WIN ? "win" : "1.x",
                MugenSetting.IsWideScreen ? "width" : "normal");
            lblCharacterCount.Text = string.Format("Total {0} item", lstCharacterList.Items.Count);
            fswCharacterCns.Path = MugenSetting.MugenCharsDirPath;
            fswCharacterCns.EnableRaisingEvents = true;
            SetMugenVersionForControls(MugenSetting.Version);
            if (pnlProgress != null)
            {
                foreach (Control control in pnlProgress.Controls)
                {
                    pnlProgress.Controls.Remove(control);
                    control.Dispose();
                }
                Controls.Remove(pnlProgress);
                pnlProgress.Dispose();
            }
        }

        /// <summary>
        /// Refresh the Data Source of the character list control
        /// </ summary>
        /// <param name = "character list"> Product list </ param>
        private void RefreshCharacterListDataSource(List<Character> characterList)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = characterList;
            CharacterListControlPreparing = true;
            lstCharacterList.DataSource = bs;
            lstCharacterList.DisplayMember = "ItemName";
            lstCharacterList.ValueMember = "DefPath";
            lstCharacterList.ClearSelected();
            CharacterListControlPreparing = false;
        }

        /// <summary>
        /// Read a single character setting
        /// </ summary>
        private void ReadCharacter()
        {
            if (lstCharacterList.SelectedItems.Count != 1) return;
            ModifyEnabled = false;
            MultiModified = false;
            Character character = (Character)lstCharacterList.SelectedItem;
            if (!File.Exists(character.DefPath))
            {
                ShowErrorMsg("Character def file does not exist！");
                return;
            }
            character.ReadPalSetting();
            SetSingleCharacterLabel(character);
            txtName.Text = character.Name;
            txtDisplayName.Text = character.DisplayName;
            txtLife.Text = character.Life.ToString();
            txtAttack.Text = character.Attack.ToString();
            txtDefence.Text = character.Defence.ToString();
            txtPower.Text = character.Power.ToString();
            if (character.Sprite == null) character.ReadSpriteFile();
            lblSpriteVersion.Text = "SFF version：" + SpriteFile.GetFormatVersion(character.SpriteVersion);
            if (character.PalList.Count > 0)
            {
                string[] selectableActFileList = character.SelectableActFileList;
                cboSelectableActFileList.Items.Add("The original image");
                cboSelectableActFileList.Items.AddRange(selectableActFileList);
                cboSelectableActFileList.SelectedIndex = 0;
                DataGridViewComboBoxColumn dgvPalFileList = (DataGridViewComboBoxColumn)dgvPal.Columns[PalValColumnNo];
                dgvPalFileList.Items.AddRange(selectableActFileList);
                foreach (string palKey in character.PalList.Keys)
                {
                    dgvPal.Rows.Add(palKey);
                }
                for (int i = 0; i < dgvPal.Rows.Count; i++)
                {
                    dgvPal.Rows[i].Cells[PalValColumnNo].Value = (from f in selectableActFileList
                                                                  where f.ToLower().GetSlashPath()
                        == character.PalList[dgvPal.Rows[i].Cells[PalNoColumnNo].Value.ToString()].ToLower().GetSlashPath()
                                                                  select f).FirstOrDefault();
                }
            }
            ModifyEnabled = true;
            if (File.Exists(character.DefPath + Character.BakExt))
            {
                btnRestore.Enabled = true;
            }
        }

        /// <summary>
        /// Batch reading character settings
        /// </summary>
        private void MultiReadCharacter()
        {
            if (lstCharacterList.SelectedItems.Count <= 1) return;
            Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
            lstCharacterList.SelectedItems.CopyTo(characterList, 0);
            MultiModified = true;
            SetMutliCharacterLabel(characterList);
            string name = characterList[0].Name;
            string displayName = characterList[0].DisplayName;
            int life = characterList[0].Life;
            int attack = characterList[0].Attack;
            int defence = characterList[0].Defence;
            int power = characterList[0].Power;
            txtName.Text = name;
            txtDisplayName.Text = displayName;
            txtLife.Text = life.ToString();
            txtAttack.Text = attack.ToString();
            txtDefence.Text = defence.ToString();
            txtPower.Text = power.ToString();
            bool canRestore = false;
            foreach (Character character in characterList)
            {
                if (name != character.Name) txtName.Text = MultiValue;
                if (displayName != character.DisplayName) txtDisplayName.Text = MultiValue;
                if (life != character.Life) txtLife.Text = MultiValue;
                if (attack != character.Attack) txtAttack.Text = MultiValue;
                if (defence != character.Defence) txtDefence.Text = MultiValue;
                if (power != character.Power) txtPower.Text = MultiValue;
                if (File.Exists(character.DefPath + Character.BakExt)) canRestore = true;
            }
            btnRestore.Enabled = canRestore;
        }

        /// <summary>
        /// Display the relative path of the DEF file of the current figure on the label
        /// </summary>
        /// <param name="defFullPath">DEF file absolute path</param>
        private void SetSingleCharacterLabel(Character character)
        {
            string path = character.DefPath.Substring(MugenSetting.MugenCharsDirPath.Length);
            lblDefPath.Text = path;
            ttpCommon.SetToolTip(lblDefPath, path);
        }

        /// <summary>
        /// Show the DEF file of the current batch read by the current batch
        /// </summary>
        private void SetMutliCharacterLabel(Character[] characterList)
        {
            string msg = "";
            foreach (Character character in characterList)
            {
                msg += character.DefPath.Substring(MugenSetting.MugenCharsDirPath.Length) + "\r\n";
            }
            lblDefPath.Text = MultiValue;
            ttpCommon.SetToolTip(lblDefPath, msg);
        }

        /// <summary>
        /// Modify single character settings
        /// </summary>
        private void ModifyCharacter()
        {
            if (lstCharacterList.SelectedItems.Count != 1) return;
            Character character = ((Character)lstCharacterList.SelectedItem);
            string oriName = character.Name;
            try
            {
                ReadCharacterControlsValue(out string name, out string displayName, out int life, out int attack, out int defence, out int power);
                character.Name = name;
                character.DisplayName = displayName;
                character.Life = life;
                character.Attack = attack;
                character.Defence = defence;
                character.Power = power;
                ReadPalValues(character.PalList);
            }
            catch (ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            try
            {
                fswCharacterCns.EnableRaisingEvents = false;
                character.Save();
            }
            catch (ApplicationException)
            {
                ShowErrorMsg("fail to edit！");
                return;
            }
            finally
            {
                fswCharacterCns.EnableRaisingEvents = true;
            }
            if (oriName != character.Name)
            {
                int index = lstCharacterList.SelectedIndex;
                RefreshCharacterListDataSource(CharacterList);
                lstCharacterList.SelectedIndex = index;
            }
            ShowSuccessMsg("Successfully modified！");
        }

        /// <summary>
        /// Batch modification person setting
        /// </summary>
        private void MultiModifyCharacter()
        {
            if (lstCharacterList.SelectedItems.Count <= 1) return;
            Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
            lstCharacterList.SelectedItems.CopyTo(characterList, 0);
            int total = 0;
            try
            {
                ReadCharacterControlsValue(out string name, out string displayName, out int life, out int attack, out int defence, out int power);
                for (int i = 0; i < characterList.Length; i++)
                {
                    characterList[i].Life = life;
                    characterList[i].Attack = attack;
                    characterList[i].Defence = defence;
                    characterList[i].Power = power;
                }
            }
            catch (ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            fswCharacterCns.EnableRaisingEvents = false;
            total = Character.MultiSave(characterList);
            if (total > 0)
            {
                ShowSuccessMsg(string.Format("A total of {0} items to modify success！", total));
            }
            else
            {
                ShowErrorMsg("fail to edit！");
            }
            fswCharacterCns.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Read the value of the character attribute control
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        private void ReadCharacterControlsValue(out string name, out string displayName,
            out int life, out int attack, out int defence, out int power)
        {
            name = ""; displayName = "";
            life = 0; attack = 0; defence = 0; power = 0;
            foreach (Control control in grpProperty.Controls)
            {
                if (control is TextBox)
                {
                    TextBox txtTemp = (TextBox)control;
                    try
                    {
                        if (txtTemp.Text.Trim() == string.Empty) throw new ApplicationException("Fields must not be empty！");
                        TextBox[] txtIntegerArray = { txtLife, txtAttack, txtDefence, txtPower };
                        if (txtIntegerArray.Contains(txtTemp))
                        {
                            int value = 0;
                            if (txtTemp.Text.Trim() == MultiValue) value = 0;
                            else value = Convert.ToInt32(txtTemp.Text.Trim());
                            if (value < 0) throw new ApplicationException("Numerical value is not less than 0！");
                            if (txtTemp == txtLife) life = value;
                            else if (txtTemp == txtAttack) attack = value;
                            else if (txtTemp == txtDefence) defence = value;
                            else if (txtTemp == txtPower) power = value;
                        }
                        else
                        {
                            if (txtTemp == txtName) name = txtTemp.Text.Trim();
                            else if (txtTemp == txtDisplayName) displayName = txtTemp.Text.Trim();
                        }
                    }
                    catch (FormatException)
                    {
                        txtTemp.SelectAll();
                        txtTemp.Focus();
                        throw new ApplicationException("Numerical format error！");
                    }
                    catch (OverflowException)
                    {
                        txtTemp.SelectAll();
                        txtTemp.Focus();
                        throw new ApplicationException("Numerical value！");
                    }
                    catch (ApplicationException ex)
                    {
                        txtTemp.SelectAll();
                        txtTemp.Focus();
                        throw ex;
                    }
                }
            }
        }

        /// <summary>
        /// Read the PAL property field
        /// </summary>
        private void ReadPalValues(Dictionary<string, string> palList)
        {
            for (int i = 0; i < dgvPal.Rows.Count; i++)
            {
                if (dgvPal.Rows[i].Cells[PalValColumnNo].Value != null &&
                    dgvPal.Rows[i].Cells[PalValColumnNo].Value.ToString() != string.Empty)
                {
                    palList[dgvPal.Rows[i].Cells[PalNoColumnNo].Value.ToString()] =
                        dgvPal.Rows[i].Cells[PalValColumnNo].Value.ToString();
                }
            }
        }

        /// <summary>
        /// Find keywords in the list of characters
        /// </summary>
        /// <param name="isUp">Whether to search up</param>
        private void SearchKeyword(bool isUp)
        {
            string keyword = txtKeyword.Text.Trim();
            if (keyword == string.Empty) return;
            if (lstCharacterList.Items.Count == 0) return;
            ModifyEnabled = false;
            int searchIndex = -1;
            if (lstCharacterList.SelectedIndex != -1)
            {
                searchIndex = lstCharacterList.SelectedIndex;
            }
            else
            {
                if (isUp) searchIndex = lstCharacterList.Items.Count;
                else searchIndex = -1;
            }
            lstCharacterList.ClearSelected();
            bool isFind = false;
            if (isUp)
            {
                for (int i = searchIndex - 1; i >= 0; i--)
                {
                    if (((Character)lstCharacterList.Items[i]).Name.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        lstCharacterList.SelectedIndex = i;
                        searchIndex = i;
                        isFind = true;
                        break;
                    }
                }
            }
            else
            {
                for (int i = searchIndex + 1; i < lstCharacterList.Items.Count; i++)
                {
                    if (((Character)lstCharacterList.Items[i]).Name.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        lstCharacterList.SelectedIndex = i;
                        searchIndex = i;
                        isFind = true;
                        break;
                    }
                }
            }
            if (!isFind)
            {
                if (searchIndex == -1 || searchIndex == lstCharacterList.Items.Count)
                {
                    searchIndex = -1;
                }
                else
                {
                    if (isUp) searchIndex = lstCharacterList.Items.Count;
                    else searchIndex = -1;
                    SearchKeyword(isUp);
                }
            }
        }

        /// <summary>
        /// Find all keywords in the list of characters
        /// </summary>
        private void SearchAllKeyword()
        {
            string keyword = txtKeyword.Text.Trim();
            if (keyword == string.Empty) return;
            if (lstCharacterList.Items.Count == 0) return;
            lstCharacterList.ClearSelected();
            ModifyEnabled = false;
            CharacterListControlPreparing = true;
            for (int i = 0; i < lstCharacterList.Items.Count; i++)
            {
                if (((Character)lstCharacterList.Items[i]).Name.IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    lstCharacterList.SetSelected(i, true);
                }
            }
            CharacterListControlPreparing = false;
            lstCharacterList_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// Convert to a wide / Push-screen character package
        /// </summary>
        /// <param name="isWideScreen">Whether a widescreen</param>
        private void ConvertToFitScreen(bool isWideScreen)
        {
            if (lstCharacterList.SelectedItems.Count == 0) return;
            string stcommonPath = MugenSetting.MugenDataDirPath + MugenSetting.StcommonFileName;
            if (!File.Exists(stcommonPath))
            {
                ShowErrorMsg("Public commit1.cns file does not exist！");
                return;
            }
            try
            {
                string stcommonContent = File.ReadAllText(stcommonPath);
                string msg = string.Format("Public common1.cns file is not suitable for {0} screen, is it necessary to convert？", isWideScreen ? "wide" : "normal");
                if (isWideScreen)
                {
                    if (Character.IsStcommonWideScreen(stcommonContent) == 0)
                    {
                        if (MessageBox.Show(msg, "Operation confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            Character.StcommonConvertToWideScreen(stcommonPath);
                        }
                    }
                }
                else
                {
                    if (Character.IsStcommonWideScreen(stcommonContent) == 1)
                    {
                        if (MessageBox.Show(msg, "Operation confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            Character.StcommonConvertToNormalScreen(stcommonPath);
                        }
                    }
                }
            }
            catch (ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            fswCharacterCns.EnableRaisingEvents = false;
            if (MultiModified)
            {
                Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
                lstCharacterList.SelectedItems.CopyTo(characterList, 0);
                int total = 0;
                if (isWideScreen)
                {
                    total = Character.MultiConvertToWideScreen(characterList);
                }
                else
                {
                    total = Character.MultiConvertToNormalScreen(characterList);
                }
                if (total > 0)
                {
                    ShowSuccessMsg(string.Format("Total {0} Succession！", total));
                }
                else
                {
                    fswCharacterCns.EnableRaisingEvents = true;
                    ShowErrorMsg("Conversion failure！");
                    return;
                }
            }
            else
            {
                Character character = (Character)lstCharacterList.SelectedItem;
                try
                {
                    if (isWideScreen)
                    {
                        character.ConvertToWideScreen();
                    }
                    else
                    {
                        character.ConvertToNormalScreen();
                    }
                }
                catch (ApplicationException ex)
                {
                    fswCharacterCns.EnableRaisingEvents = true;
                    ShowErrorMsg(ex.Message);
                    return;
                }
                ShowSuccessMsg("Successful conversion！");
            }
            int[] selectedIndices = new int[lstCharacterList.SelectedIndices.Count];
            lstCharacterList.SelectedIndices.CopyTo(selectedIndices, 0);
            if (AppConfig.ShowCharacterScreenMark)
            {
                RefreshCharacterListDataSource(CharacterList);
            }
            CharacterListControlPreparing = true;
            foreach (int index in selectedIndices)
            {
                lstCharacterList.SetSelected(index, true);
            }
            CharacterListControlPreparing = false;
            lstCharacterList_SelectedIndexChanged(null, null);
            fswCharacterCns.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Set the corresponding control for different MUGEN programs
        /// </summary>
        /// <param name="version">Mugen program version</param>
        private void SetMugenVersionForControls(MugenSetting.MugenVersion version)
        {
            if (version == MugenSetting.MugenVersion.WIN)
            {
                ctxTsmiConvertToWideScreen.Enabled = false;
                ctxTsmiConvertToNormalScreen.Enabled = false;
                txtGameWidth.Enabled = false;
                txtGameHeight.Enabled = false;
                cboRenderMode.Enabled = false;
            }
            else
            {
                ctxTsmiConvertToWideScreen.Enabled = true;
                ctxTsmiConvertToNormalScreen.Enabled = true;
                txtGameWidth.Enabled = true;
                txtGameHeight.Enabled = true;
                cboRenderMode.Enabled = true;
            }
            KeyPressComboBoxInit();
        }

        /// <summary>
        /// Create a <see cref = "panel" /> class instance with progress bar and add it to the parent container
        /// </ summary>
        /// <param name = "parent"> Parental container </ param>
        /// <returns> New <SEE CREF = "Panel" /> class instance </ returns>
        private Panel ShowProgressPanel(Control parent)
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            ProgressBar progressBar = new ProgressBar();
            progressBar.Size = new Size(200, 28);
            progressBar.Location = new Point((Width - progressBar.Width) / 2, Height / 2 - progressBar.Height - 25);
            progressBar.Style = ProgressBarStyle.Marquee;
            progressBar.MarqueeAnimationSpeed = 10;
            panel.Controls.Add(progressBar);
            parent.Controls.Add(panel);
            parent.Controls.SetChildIndex(panel, 0);
            return panel;
        }

        /// <summary>
        /// Add a character by a compressed package
        /// </ summary>
        /// <param name = "Archive path list"> Compressed package absolute path list </ param>
        /// <returns> Add a total number of characters </ returns>
        private int AddCharacterByArchive(string[] archivePathList)
        {
            Panel pnlProgress = ShowProgressPanel(this);
            fswCharacterCns.EnableRaisingEvents = false;
            int total = 0;
            foreach (string path in archivePathList)
            {
                if (!Character.ArchiveExt.Contains(Path.GetExtension(path).ToLower())) continue;
                string newDirPath = "";
                try
                {
                    newDirPath = Character.DecompressionCharacterArchive(path);
                }
                catch (ApplicationException)
                {
                    continue;
                }
                List<Character> tempCharacterList = new List<Character>();
                Character.ScanCharacterDir(tempCharacterList, newDirPath);
                if (tempCharacterList.Count > 0)
                {
                    CharacterList.AddRange(tempCharacterList);
                    total += tempCharacterList.Count;
                }
                else
                {
                    try
                    {
                        Directory.Delete(newDirPath, true);
                    }
                    catch (Exception) { }
                }
            }
            if (total > 0)
            {
                RefreshCharacterListDataSource(CharacterList);
                CharacterListControlPreparing = true;
                for (int i = 0; i < total; i++)
                {
                    lstCharacterList.SetSelected(lstCharacterList.Items.Count - i - 1, true);
                }
                CharacterListControlPreparing = false;
                lstCharacterList_SelectedIndexChanged(null, null);
            }
            fswCharacterCns.EnableRaisingEvents = true;
            foreach (Control control in pnlProgress.Controls)
            {
                pnlProgress.Controls.Remove(control);
                control.Dispose();
            }
            Controls.Remove(pnlProgress);
            pnlProgress.Dispose();
            return total;
        }

        /// <summary>
        /// Add a character by DEF file
        /// </ summary>
        /// <param name = "def path list"> DEF file absolute path list </ param>
        private void AddCharacterByDef(string[] defPathList)
        {
            lstCharacterList.ClearSelected();
            ModifyEnabled = false;
            CharacterListControlPreparing = true;
            int newItemNum = 0;
            foreach (string defPath in defPathList)
            {
                if (!defPath.StartsWith(MugenSetting.MugenCharsDirPath)) continue;
                if (Path.GetExtension(defPath).ToLower() != Character.DefExt) continue;
                int index;
                for (index = 0; index < lstCharacterList.Items.Count; index++)
                {
                    if (((Character)lstCharacterList.Items[index]).Equals(defPath))
                    {
                        lstCharacterList.SetSelected(index, true);
                        break;
                    }
                }
                if (index == lstCharacterList.Items.Count)
                {
                    try
                    {
                        CharacterList.Add(new Character(defPath));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    newItemNum++;
                }
            }
            if (newItemNum != 0)
            {
                RefreshCharacterListDataSource(CharacterList);
                CharacterListControlPreparing = true;
                for (int i = 0; i < newItemNum; i++)
                {
                    lstCharacterList.SetSelected(lstCharacterList.Items.Count - i - 1, true);
                }
            }
            CharacterListControlPreparing = false;
            lstCharacterList_SelectedIndexChanged(null, null);
        }

        #endregion Class method

        #region Class event

        /// <summary>
        /// When the window is loaded
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            ReadConfig();
            string mugenCfgPath = AppConfig.MugenExePath.GetDirPathOfFile() + MugenSetting.DataDir + MugenSetting.MugenCfgFileName;
            if (AppConfig.MugenExePath == string.Empty || !File.Exists(AppConfig.MugenExePath) || !File.Exists(mugenCfgPath))
            {
                Visible = false;
                StartUpForm startUpForm = new StartUpForm();
                startUpForm.Owner = this;
                if (startUpForm.ShowDialog() != DialogResult.OK)
                {
                    Close();
                    return;
                }
            }
            try
            {
                MugenSetting.Init(AppConfig.MugenExePath);
            }
            catch (ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                Close();
                return;
            }
            ReadCharacterListProgressForm readCharListProgressForm = new ReadCharacterListProgressForm();
            readCharListProgressForm.Owner = this;
            readCharListProgressForm.ShowDialog();
            Visible = true;
        }

        /// <summary>
        /// When the window will be turned off
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            AppConfig.Save();
        }

        /// <summary>
        /// When the character attribute tag page gets the focus
        /// </summary>
        private void pageCharacter_Enter(object sender, EventArgs e)
        {
            Size = new Size(575, 590);
        }

        /// <summary>
        /// When you click the Refreshing Character list button
        /// </summary>
        private void btnRefreshCharacterList_Click(object sender, EventArgs e)
        {
            ReadCharacterList(true);
        }

        /// <summary>
        /// When you click Modify the Character Set button
        /// </summary>
        private void btnModify_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedItems.Count == 0) return;
            if (MultiModified)
            {
                MultiModifyCharacter();
            }
            else
            {
                ModifyCharacter();
            }
        }

        /// <summary>
        /// When a reset person setting button is clicked
        /// </summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedItems.Count == 0) return;
            if (MultiModified)
            {
                MultiReadCharacter();
            }
            else
            {
                ReadCharacter();
            }
        }

        /// <summary>
        /// When a backup character setting button is clicked
        /// </summary>
        private void btnBackup_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedItems.Count == 0) return;
            if (MultiModified)
            {
                Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
                lstCharacterList.SelectedItems.CopyTo(characterList, 0);
                int total = Character.MultiBackup(characterList);
                if (total > 0)
                {
                    ShowSuccessMsg(string.Format("Total {0} Successful project backup！", total));
                }
                else
                {
                    ShowErrorMsg("Backup failed！");
                    return;
                }
            }
            else
            {
                Character character = (Character)lstCharacterList.SelectedItem;
                try
                {
                    character.Backup();
                }
                catch (ApplicationException ex)
                {
                    ShowErrorMsg(ex.Message);
                    return;
                }
                ShowSuccessMsg("Successful backup！");
            }
            btnRestore.Enabled = true;
        }

        /// <summary>
        /// When you click the recovery person setting button
        /// </summary>
        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedItems.Count == 0) return;
            fswCharacterCns.EnableRaisingEvents = false;
            if (MultiModified)
            {
                Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
                lstCharacterList.SelectedItems.CopyTo(characterList, 0);
                int total = Character.MultiRestore(characterList);
                if (total > 0)
                {
                    ShowSuccessMsg(string.Format("Total {0} Project Restore success！", total));
                    MultiReadCharacter();
                }
                else
                {
                    ShowErrorMsg("Restore failure！");
                }
            }
            else
            {
                Character character = (Character)lstCharacterList.SelectedItem;
                try
                {
                    character.Restore();
                    character.ReadCharacterSetting();
                }
                catch (ApplicationException ex)
                {
                    fswCharacterCns.EnableRaisingEvents = true;
                    ShowErrorMsg(ex.Message);
                    return;
                }
                ReadCharacter();
                ShowSuccessMsg("Recover success！");
            }
            fswCharacterCns.EnableRaisingEvents = true;
        }

        /// <summary>
        /// When the characters list control selection changes
        /// </summary>
        private void lstCharacterList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CharacterListControlPreparing) return;
            lblCharacterSelectCount.Text = string.Format("Selected {0} item", lstCharacterList.SelectedItems.Count);
            if (lstCharacterList.SelectedItems.Count > 1)
            {
                MultiReadCharacter();
            }
            else
            {
                ReadCharacter();
            }
        }

        /// <summary>
        /// Automatically display the drop-down frame when you click the color table cell.
        /// </summary>
        private void dgvPal_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            try
            {
                SendKeys.Send("{F4}");
            }
            catch (Exception) { }
        }

        /// <summary>
        /// When the automatic selection of attribute changes
        /// </summary>
        private void chkAutoSort_CheckedChanged(object sender, EventArgs e)
        {
            AppConfig.AutoSort = chkAutoSort.Checked;
        }

        /// <summary>
        /// What happens when you click a full-selection button?
        /// </summary>
        private void btnSelectAll_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.Items.Count == 0) return;
            CharacterListControlPreparing = true;
            for (int i = 0; i < lstCharacterList.Items.Count; i++)
            {
                lstCharacterList.SetSelected(i, true);
            }
            CharacterListControlPreparing = false;
            lstCharacterList_SelectedIndexChanged(null, null);
        }

        /// <summary>
        /// What happens when you click the Reflect button?
        /// </summary>
        private void btnSelectInvert_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.Items.Count == 0) return;
            if (lstCharacterList.SelectedItems.Count == 0)
            {
                btnSelectAll_Click(null, null);
                return;
            }
            CharacterListControlPreparing = true;
            bool isAllSelect = true;
            for (int i = 0; i < lstCharacterList.Items.Count; i++)
            {
                bool isSelected = !lstCharacterList.GetSelected(i);
                lstCharacterList.SetSelected(i, isSelected);
                if (isSelected)
                {
                    isAllSelect = false;
                }
            }
            CharacterListControlPreparing = false;
            lstCharacterList_SelectedIndexChanged(null, null);
            if (isAllSelect)
            {
                ModifyEnabled = false;
            }
        }

        /// <summary>
        /// What happens when you click the search button
        /// </summary>
        private void btnSearchUp_Click(object sender, EventArgs e)
        {
            SearchKeyword(true);
        }

        /// <summary>
        /// What happens when you click a button?
        /// </summary>
        private void btnSearchDown_Click(object sender, EventArgs e)
        {
            SearchKeyword(false);
        }

        /// <summary>
        /// When you click on the full search button
        /// </summary>
        private void btnSearchAll_Click(object sender, EventArgs e)
        {
            SearchAllKeyword();
        }

        /// <summary>
        /// When searching for text box control Press a key
        /// </summary>
        private void txtKeyword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Enter)
            {
                btnSearchDown_Click(null, null);
            }
        }

        /// <summary>
        /// When the form control presses a key
        /// </summary>
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (pageCharacter.CanFocus)
            {
                switch (e.KeyValue)
                {
                    case (int)Keys.Enter:
                        if (e.Control)
                        {
                            if (btnModify.Enabled) btnModify_Click(null, null);
                        }
                        break;

                    case (int)Keys.S:
                        if (e.Control)
                        {
                            if (btnReset.Enabled) btnReset_Click(null, null);
                        }
                        break;

                    case (int)Keys.B:
                        if (e.Control)
                        {
                            if (btnBackup.Enabled) btnBackup_Click(null, null);
                        }
                        break;

                    case (int)Keys.R:
                        if (e.Control)
                        {
                            if (btnRestore.Enabled) btnRestore_Click(null, null);
                        }
                        break;
                }
            }
            else if (pageMugenCfgSetting.CanFocus)
            {
                switch (e.KeyValue)
                {
                    case (int)Keys.Enter:
                        if (e.Control)
                        {
                            if (btnMugenCfgModify.Enabled) btnMugenCfgModify_Click(null, null);
                        }
                        break;

                    case (int)Keys.S:
                        if (e.Control)
                        {
                            if (btnMugenCfgReset.Enabled) btnMugenCfgReset_Click(null, null);
                        }
                        break;

                    case (int)Keys.B:
                        if (e.Control)
                        {
                            if (btnMugenCfgBackup.Enabled) btnMugenCfgBackup_Click(null, null);
                        }
                        break;

                    case (int)Keys.R:
                        if (e.Control)
                        {
                            if (btnMugenCfgRestore.Enabled) btnMugenCfgRestore_Click(null, null);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// When the character's property text box control presses a key
        /// </summary>
        private void textProperty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Enter)
            {
                SendKeys.Send("{Tab}");
            }
        }

        /// <summary>
        /// When reading the character list type control selection changes
        /// </summary>
        private void cboReadCharacterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppConfig.ReadCharacterType = (AppConfig.ReadCharTypeEnum)cboReadCharacterType.SelectedIndex;
        }

        /// <summary>
        /// When the CNS file under the character folder changes
        /// </summary>
        private void fswCharacterCns_Changed(object sender, FileSystemEventArgs e)
        {
            string cnsPath = e.FullPath;
            Character character = (from c in CharacterList
                                   where c.CnsFullPath.ToLower() == cnsPath.ToLower()
                                   select c).FirstOrDefault();
            if (character != null)
            {
                character.ReadCharacterSetting();
            }
        }

        /// <summary>
        /// When the character DEF file drags and drops to the character attribute tab
        /// </summary>
        private void pageCharacter_DragDrop(object sender, DragEventArgs e)
        {
            string[] pathList = (string[])e.Data.GetData(DataFormats.FileDrop);
            if (Path.GetExtension(pathList[0]).ToLower() == Character.DefExt)
            {
                AddCharacterByDef(pathList);
            }
            else if (Character.ArchiveExt.Contains(Path.GetExtension(pathList[0]).ToLower()))
            {
                if (AddCharacterByArchive(pathList) == 0)
                {
                    ShowErrorMsg("Adding a character compressed package failed！");
                }
            }
        }

        private void pageCharacter_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        /// <summary>
        /// When the list of the ACT file list is removed, the selection is changed.
        /// </summary>
        private void cboSelectableActFileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedItems.Count == 0) return;
            if (cboSelectableActFileList.Items.Count == 0) return;
            Character character = (Character)lstCharacterList.SelectedItem;
            if (cboSelectableActFileList.SelectedIndex == 0)
            {
                picSpriteImage.Image = character.SpriteImage;
            }
            else
            {
                picSpriteImage.Image = character.GetSpriteImageWithPal(cboSelectableActFileList.SelectedItem.ToString());
            }
        }

        #endregion Class event

        #region Character list Right-click menu

        /// <summary>
        /// When you click Open the DEF file right key menu item
        /// </summary>
        private void ctxTsmiOpenDefFile_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedItems.Count == 0) return;
            Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
            lstCharacterList.SelectedItems.CopyTo(characterList, 0);
            if (characterList.Length > 1)
            {
                if (MessageBox.Show("Do you want to open multiple files?？", "Operation confirmation", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes) return;
            }
            foreach (Character character in characterList)
            {
                if (!File.Exists(character.DefPath))
                {
                    if (MultiModified)
                    {
                        continue;
                    }
                    else
                    {
                        ShowErrorMsg("Character def file does not exist！");
                        return;
                    }
                }
                try
                {
                    Process.Start(AppConfig.EditProgramPath, character.DefPath);
                }
                catch (Exception)
                {
                    if (MultiModified)
                    {
                        continue;
                    }
                    else
                    {
                        ShowErrorMsg("Did you find a text editor！");
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// When you click Open the CNS file right key menu item
        /// </summary>
        private void ctxTsmiOpenCnsFile_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedItems.Count == 0) return;
            Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
            lstCharacterList.SelectedItems.CopyTo(characterList, 0);
            if (characterList.Length > 1)
            {
                if (MessageBox.Show("Do you want to open multiple files?？", "Operation confirmation", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes) return;
            }
            foreach (Character character in characterList)
            {
                if (!File.Exists(character.CnsFullPath))
                {
                    if (MultiModified)
                    {
                        continue;
                    }
                    else
                    {
                        ShowErrorMsg("People CNS files do not exist！");
                        return;
                    }
                }
                try
                {
                    Process.Start(AppConfig.EditProgramPath, character.CnsFullPath);
                }
                catch (Exception)
                {
                    if (MultiModified)
                    {
                        continue;
                    }
                    else
                    {
                        ShowErrorMsg("Did you find a text editor！");
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// When you click Open a folder right-click menu item?
        /// </summary>
        private void ctxTsmiOpenDefDir_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedItems.Count == 0) return;
            Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
            lstCharacterList.SelectedItems.CopyTo(characterList, 0);
            if (characterList.Length > 1)
            {
                if (MessageBox.Show("Do you want to open multiple folders?？", "Operation confirmation", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question) != DialogResult.Yes) return;
            }
            foreach (Character character in characterList)
            {
                if (!File.Exists(character.DefPath))
                {
                    if (MultiModified)
                    {
                        continue;
                    }
                    else
                    {
                        ShowErrorMsg("Character def file does not exist！");
                        return;
                    }
                }
                try
                {
                    Process.Start("explorer.exe", "/select," + character.DefPath);
                }
                catch (Exception)
                {
                    if (MultiModified)
                    {
                        continue;
                    }
                    else
                    {
                        ShowErrorMsg("Open a folder failed！");
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// When you click Copy the DEF file path right, the key metrop will occur.
        /// </summary>
        private void ctxTsmiCopyDefPath_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedItems.Count == 0) return;
            string copyContent = "";
            for (int i = 0; i < lstCharacterList.SelectedItems.Count; i++)
            {
                copyContent += ((Character)lstCharacterList.SelectedItems[i]).DefPath.Substring(MugenSetting.MugenCharsDirPath.Length).GetSlashPath();
                if (i < lstCharacterList.SelectedItems.Count - 1)
                {
                    copyContent += "\r\n";
                }
            }
            try
            {
                Clipboard.SetDataObject(copyContent, true, 2, 300);
            }
            catch (Exception)
            {
                ShowErrorMsg("Copy failure！");
                return;
            }
            ShowSuccessMsg(string.Format("{0}Article project has been copied to a clipboard！", lstCharacterList.SelectedItems.Count));
        }

        /// <summary>
        /// When you click Add Sytermall, right-click mesh item
        /// </summary>
        private void ctxTsmiAddCharacter_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedItems.Count == 0) return;
            Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
            lstCharacterList.SelectedItems.CopyTo(characterList, 0);
            if (Character.AddCharacterToSelectDef(characterList))
            {
                int[] selectedIndices = new int[lstCharacterList.SelectedIndices.Count];
                lstCharacterList.SelectedIndices.CopyTo(selectedIndices, 0);
                RefreshCharacterListDataSource(CharacterList);
                CharacterListControlPreparing = true;
                foreach (int index in selectedIndices)
                {
                    lstCharacterList.SetSelected(index, true);
                }
                CharacterListControlPreparing = false;
                lstCharacterList_SelectedIndexChanged(null, null);
            }
            else
            {
                ShowErrorMsg("添加失败！");
            }
        }

        /// <summary>
        /// When you click Delete Syareder Right-click Menu item?
        /// </summary>
        private void ctxTsmiDeleteCharacter_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedItems.Count == 0) return;
            if (MessageBox.Show("Whether to delete the characters？", "Operation confirmation", MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes) return;
            if (MultiModified)
            {
                Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
                lstCharacterList.SelectedItems.CopyTo(characterList, 0);
                int total = Character.MultiDelete(characterList);
                if (total == 0)
                {
                    ShowErrorMsg("failed to delete！");
                    return;
                }
                foreach (Character character in characterList)
                {
                    CharacterList.Remove(character);
                }
            }
            else
            {
                Character character = (Character)lstCharacterList.SelectedItem;
                try
                {
                    character.Delete();
                }
                catch (ApplicationException ex)
                {
                    ShowErrorMsg(ex.Message);
                    return;
                }
                CharacterList.Remove(character);
            }
            int index = lstCharacterList.SelectedIndex;
            RefreshCharacterListDataSource(CharacterList);
            if (index < lstCharacterList.Items.Count)
            {
                lstCharacterList.SelectedIndex = index;
            }
            else
            {
                lstCharacterList.SelectedIndex = lstCharacterList.Items.Count - 1;
            }
            lblCharacterCount.Text = string.Format("Total {0} item", lstCharacterList.Items.Count);
            if (lstCharacterList.Items.Count == 0)
            {
                lblCharacterSelectCount.Text = "";
                ModifyEnabled = false;
            }
        }

        /// <summary>
        /// When you click Convert to Widescreen Character Pack Right-click on the menu item
        /// </summary>
        private void ctxTsmiConvertToWideScreen_Click(object sender, EventArgs e)
        {
            ConvertToFitScreen(true);
        }

        /// <summary>
        /// When you click Convert to the Push-block character, right-click menu item
        /// </summary>
        private void ctxTsmiConvertToNormalScreen_Click(object sender, EventArgs e)
        {
            ConvertToFitScreen(false);
        }

        #endregion Character list Right-click menu

        #region 主菜单

        /// <summary>
        /// When you click Add a character to compress the package menu item
        /// </summary>
        private void tsmiAddCharacterByDefOrArchive_Click(object sender, EventArgs e)
        {
            if (ofdAddCharacterPath.ShowDialog() == DialogResult.OK)
            {
                if (Character.ArchiveExt.Contains(Path.GetExtension(ofdAddCharacterPath.FileNames[0]).ToLower()))
                {
                    if (AddCharacterByArchive(ofdAddCharacterPath.FileNames) == 0)
                    {
                        ShowErrorMsg("Adding a character compressed package failed！");
                        return;
                    }
                }
                else if (Path.GetExtension(ofdAddCharacterPath.FileNames[0]).ToLower() == Character.DefExt)
                {
                    AddCharacterByDef(ofdAddCharacterPath.FileNames);
                    ctxTsmiAddCharacter_Click(null, null);
                }
            }
        }

        /// <summary>
        /// When you click Replace the System.DEF file menu item,
        /// </summary>
        private void tsmiChangeSystemDefPath_Click(object sender, EventArgs e)
        {
            ofdDefPath.FileName = MugenSetting.SystemDefPath;
            if (File.Exists(MugenSetting.SystemDefPath))
            {
                ofdDefPath.InitialDirectory = MugenSetting.SystemDefPath.GetDirPathOfFile();
            }
            else
            {
                ofdDefPath.InitialDirectory = MugenSetting.MugenDataDirPath;
            }
            if (ofdDefPath.ShowDialog() != DialogResult.OK) return;
            try
            {
                MugenSetting.SystemDefPath = ofdDefPath.FileName;
            }
            catch (ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            btnRefreshCharacterList_Click(null, null);
        }

        /// <summary>
        /// When you click Replace the Select.DEF file menu item,
        /// </summary>
        private void tsmiChangeSelectDefPath_Click(object sender, EventArgs e)
        {
            ofdDefPath.FileName = MugenSetting.SelectDefPath;
            if (File.Exists(MugenSetting.SelectDefPath))
            {
                ofdDefPath.InitialDirectory = MugenSetting.SelectDefPath.GetDirPathOfFile();
            }
            else
            {
                ofdDefPath.InitialDirectory = MugenSetting.MugenDataDirPath;
            }
            if (ofdDefPath.ShowDialog() != DialogResult.OK) return;
            try
            {
                MugenSetting.SelectDefPath = ofdDefPath.FileName;
            }
            catch (ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            btnRefreshCharacterList_Click(null, null);
        }

        /// <summary>
        /// When you click Replace the Fight.DEF file menu item,
        /// </summary>
        private void tsmiChangeFightDefPath_Click(object sender, EventArgs e)
        {
            ofdDefPath.FileName = MugenSetting.FightDefPath;
            if (File.Exists(MugenSetting.FightDefPath))
            {
                ofdDefPath.InitialDirectory = MugenSetting.FightDefPath.GetDirPathOfFile();
            }
            else
            {
                ofdDefPath.InitialDirectory = MugenSetting.MugenDataDirPath;
            }
            if (ofdDefPath.ShowDialog() != DialogResult.OK) return;
            try
            {
                MugenSetting.FightDefPath = ofdDefPath.FileName;
            }
            catch (ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            ShowSuccessMsg("Fight.def file replacement success！");
        }

        /// <summary>
        /// What happened when you click reload menu item?
        /// </summary>
        private void tsmiReload_Click(object sender, EventArgs e)
        {
            ReadCharacterList(true);
            ReadMugenCfgSetting();
        }

        /// <summary>
        /// When you click Run Mugen Program Menu items
        /// </summary>
        private void tsmiLaunchMugenExe_Click(object sender, EventArgs e)
        {
            if (File.Exists(AppConfig.MugenExePath))
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo(AppConfig.MugenExePath);
                    psi.UseShellExecute = false;
                    psi.WorkingDirectory = MugenSetting.MugenDirPath;
                    Process.Start(psi);
                }
                catch (Exception)
                {
                    ShowErrorMsg("Run the Mugen program failed！");
                    return;
                }
            }
            else
            {
                ShowErrorMsg("Mugen program does not exist！");
            }
        }

        /// <summary>
        /// When you click Settings menu items
        /// </summary>
        private void tsmiSetting_Click(object sender, EventArgs e)
        {
            SettingForm settingForm = new SettingForm();
            settingForm.Owner = this;
            settingForm.ShowDialog();
        }

        /// <summary>
        /// When you click Open Select.DEF menu item?
        /// </summary>
        private void tsmiOpenSelectDef_Click(object sender, EventArgs e)
        {
            if (File.Exists(MugenSetting.SelectDefPath))
            {
                try
                {
                    Process.Start(AppConfig.EditProgramPath, MugenSetting.SelectDefPath);
                }
                catch (Exception)
                {
                    ShowErrorMsg("Did you find a text editor！");
                    return;
                }
            }
            else
            {
                ShowErrorMsg("select.def file does not exist！");
            }
        }

        /// <summary>
        /// When you click Open the System.def menu item
        /// </summary>
        private void tsmiOpenSystemDef_Click(object sender, EventArgs e)
        {
            if (File.Exists(MugenSetting.SystemDefPath))
            {
                try
                {
                    Process.Start(AppConfig.EditProgramPath, MugenSetting.SystemDefPath);
                }
                catch (Exception)
                {
                    ShowErrorMsg("Did you find a text editor！");
                    return;
                }
            }
            else
            {
                ShowErrorMsg("System.def file does not exist！");
            }
        }

        /// <summary>
        /// When you click Open Mugen.cfg menu item?
        /// </summary>
        private void tsmiOpenMugenCfg_Click(object sender, EventArgs e)
        {
            if (File.Exists(MugenSetting.MugenCfgPath))
            {
                try
                {
                    Process.Start(AppConfig.EditProgramPath, MugenSetting.MugenCfgPath);
                }
                catch (Exception)
                {
                    ShowErrorMsg("Did you find a text editor！");
                    return;
                }
            }
            else
            {
                ShowErrorMsg("Mugen.cfg file does not exist！");
            }
        }

        /// <summary>
        /// When you click on the menu item
        /// </summary>
        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        #endregion 主菜单

        #region Main program configuration tab

        private bool _mugenCfgModifyEnabled = false;

        /// <summary>
        /// Get or set up the main program configuration tab to enter modification mode
        /// </summary>
        private bool MugenCfgModifyEnabled
        {
            get { return _mugenCfgModifyEnabled; }
            set
            {
                if (value)
                {
                    btnMugenCfgModify.Enabled = true;
                    btnMugenCfgReset.Enabled = true;
                    btnMugenCfgBackup.Enabled = true;
                    btnMugenCfgRestore.Enabled = false;
                }
                else
                {
                    btnMugenCfgModify.Enabled = false;
                    btnMugenCfgReset.Enabled = false;
                    btnMugenCfgBackup.Enabled = false;
                    btnMugenCfgRestore.Enabled = false;
                    ResetMugenCfgControls();
                }
                _mugenCfgModifyEnabled = value;
            }
        }

        /// <summary>
        /// Reset the respective controls on the main program configuration tab
        /// </summary>
        private void ResetMugenCfgControls()
        {
            foreach (Control groupBox in pageMugenCfgSetting.Controls)
            {
                if (groupBox is GroupBox)
                {
                    foreach (Control control in ((GroupBox)groupBox).Controls)
                    {
                        if (control is TextBox)
                        {
                            ((TextBox)control).Clear();
                        }
                        else if (control is ComboBox)
                        {
                            ((ComboBox)control).SelectedIndex = -1;
                        }
                    }
                }
            }
            trbDifficulty.Value = 1;
            trbGameSpeed.Value = 0;
        }

        /// <summary>
        /// Read Mugen.cfg file configuration
        /// </summary>
        public void ReadMugenCfgSetting()
        {
            try
            {
                MugenSetting.ReadMugenCfgSetting();
            }
            catch (ApplicationException ex)
            {
                MugenCfgModifyEnabled = false;
                ShowErrorMsg(ex.Message);
                return;
            }
            try
            {
                trbDifficulty.Value = MugenSetting.Difficulty;
            }
            catch (ArgumentOutOfRangeException)
            {
                trbDifficulty.Value = 1;
            }
            finally
            {
                trbDifficulty_ValueChanged(null, null);
            }
            txtMugenCfgLife.Text = MugenSetting.Life.ToString();
            txtTime.Text = MugenSetting.Time.ToString();
            try
            {
                trbGameSpeed.Value = MugenSetting.GameSpeed;
            }
            catch (ArgumentOutOfRangeException)
            {
                trbGameSpeed.Value = 0;
            }
            finally
            {
                trbGameSpeed_ValueChanged(null, null);
            }
            txtGameFrame.Text = MugenSetting.GameFrame.ToString();
            txtTeam1VS2Life.Text = MugenSetting.Team1VS2Life.ToString();
            cboTeamLoseOnKO.SelectedIndex = Convert.ToInt32(MugenSetting.TeamLoseOnKO);
            txtGameWidth.Text = MugenSetting.GameWidth.ToString();
            txtGameHeight.Text = MugenSetting.GameHeight.ToString();
            cboRenderMode.SelectedIndex = Tools.GetComboBoxEqualValueIndex(cboRenderMode, MugenSetting.RenderMode);
            cboFullScreen.SelectedIndex = Convert.ToInt32(MugenSetting.FullScreen);

            cboP1Jump.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP1.Jump);
            cboP1Crouch.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP1.Crouch);
            cboP1Left.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP1.Left);
            cboP1Right.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP1.Right);
            cboP1A.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP1.A);
            cboP1B.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP1.B);
            cboP1C.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP1.C);
            cboP1X.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP1.X);
            cboP1Y.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP1.Y);
            cboP1Z.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP1.Z);
            cboP1Start.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP1.Start);

            cboP2Jump.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP2.Jump);
            cboP2Crouch.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP2.Crouch);
            cboP2Left.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP2.Left);
            cboP2Right.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP2.Right);
            cboP2A.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP2.A);
            cboP2B.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP2.B);
            cboP2C.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP2.C);
            cboP2X.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP2.X);
            cboP2Y.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP2.Y);
            cboP2Z.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP2.Z);
            cboP2Start.Text = KeyPressSetting.GetKeyName(MugenSetting.KeyPressP2.Start);

            MugenCfgModifyEnabled = true;
            if (File.Exists(MugenSetting.MugenCfgPath + MugenSetting.BakExt))
            {
                btnMugenCfgRestore.Enabled = true;
            }
        }

        /// <summary>
        /// Read the value of the control on the main program configuration tab
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        private void ReadMugenCfgControlsValue()
        {
            foreach (Control groupBox in pageMugenCfgSetting.Controls)
            {
                if (groupBox is GroupBox)
                {
                    foreach (Control control in ((GroupBox)groupBox).Controls)
                    {
                        if (!control.Enabled) continue;
                        try
                        {
                            if (control is TextBox)
                            {
                                if (((TextBox)control).Text.Trim() == string.Empty) throw new ApplicationException("Fields must not be empty！");
                            }
                            else if (control is ComboBox)
                            {
                                if (((ComboBox)control).DropDownStyle == ComboBoxStyle.DropDownList)
                                {
                                    if (((ComboBox)control).SelectedIndex == -1) throw new ApplicationException("Must choose one！");
                                }
                                else
                                {
                                    if (((ComboBox)control).Text.Trim() == string.Empty) throw new ApplicationException("Fields must not be empty！");
                                }
                            }
                            if (control == trbDifficulty) MugenSetting.Difficulty = trbDifficulty.Value;
                            else if (control == txtLife) MugenSetting.Life = Convert.ToInt32(txtLife.Text.Trim());
                            else if (control == txtTime) MugenSetting.Time = Convert.ToInt32(txtTime.Text.Trim());
                            else if (control == trbGameSpeed) MugenSetting.GameSpeed = trbGameSpeed.Value;
                            else if (control == txtGameFrame) MugenSetting.GameFrame = Convert.ToInt32(txtGameFrame.Text.Trim());
                            else if (control == txtTeam1VS2Life) MugenSetting.Team1VS2Life = Convert.ToInt32(txtTeam1VS2Life.Text.Trim());
                            else if (control == cboTeamLoseOnKO) MugenSetting.TeamLoseOnKO = Convert.ToBoolean(cboTeamLoseOnKO.SelectedIndex);
                            else if (control == txtGameWidth) MugenSetting.GameWidth = Convert.ToInt32(txtGameWidth.Text.Trim());
                            else if (control == txtGameHeight) MugenSetting.GameHeight = Convert.ToInt32(txtGameHeight.Text.Trim());
                            else if (control == cboRenderMode) MugenSetting.RenderMode = cboRenderMode.SelectedItem.ToString();
                            else if (control == cboFullScreen) MugenSetting.FullScreen = Convert.ToBoolean(cboFullScreen.SelectedIndex);
                            else if (control == cboP1Jump) MugenSetting.KeyPressP1.Jump = KeyPressSetting.GetKeyCode(cboP1Jump.Text.Trim());
                            else if (control == cboP1Crouch) MugenSetting.KeyPressP1.Crouch = KeyPressSetting.GetKeyCode(cboP1Crouch.Text.Trim());
                            else if (control == cboP1Left) MugenSetting.KeyPressP1.Left = KeyPressSetting.GetKeyCode(cboP1Left.Text.Trim());
                            else if (control == cboP1Right) MugenSetting.KeyPressP1.Right = KeyPressSetting.GetKeyCode(cboP1Right.Text.Trim());
                            else if (control == cboP1A) MugenSetting.KeyPressP1.A = KeyPressSetting.GetKeyCode(cboP1A.Text.Trim());
                            else if (control == cboP1B) MugenSetting.KeyPressP1.B = KeyPressSetting.GetKeyCode(cboP1B.Text.Trim());
                            else if (control == cboP1C) MugenSetting.KeyPressP1.C = KeyPressSetting.GetKeyCode(cboP1C.Text.Trim());
                            else if (control == cboP1X) MugenSetting.KeyPressP1.X = KeyPressSetting.GetKeyCode(cboP1X.Text.Trim());
                            else if (control == cboP1Y) MugenSetting.KeyPressP1.Y = KeyPressSetting.GetKeyCode(cboP1Y.Text.Trim());
                            else if (control == cboP1Z) MugenSetting.KeyPressP1.Z = KeyPressSetting.GetKeyCode(cboP1Z.Text.Trim());
                            else if (control == cboP1Start) MugenSetting.KeyPressP1.Start = KeyPressSetting.GetKeyCode(cboP1Start.Text.Trim());
                            else if (control == cboP2Jump) MugenSetting.KeyPressP2.Jump = KeyPressSetting.GetKeyCode(cboP2Jump.Text.Trim());
                            else if (control == cboP2Crouch) MugenSetting.KeyPressP2.Crouch = KeyPressSetting.GetKeyCode(cboP2Crouch.Text.Trim());
                            else if (control == cboP2Left) MugenSetting.KeyPressP2.Left = KeyPressSetting.GetKeyCode(cboP2Left.Text.Trim());
                            else if (control == cboP2Right) MugenSetting.KeyPressP2.Right = KeyPressSetting.GetKeyCode(cboP2Right.Text.Trim());
                            else if (control == cboP2A) MugenSetting.KeyPressP2.A = KeyPressSetting.GetKeyCode(cboP2A.Text.Trim());
                            else if (control == cboP2B) MugenSetting.KeyPressP2.B = KeyPressSetting.GetKeyCode(cboP2B.Text.Trim());
                            else if (control == cboP2C) MugenSetting.KeyPressP2.C = KeyPressSetting.GetKeyCode(cboP2C.Text.Trim());
                            else if (control == cboP2X) MugenSetting.KeyPressP2.X = KeyPressSetting.GetKeyCode(cboP2X.Text.Trim());
                            else if (control == cboP2Y) MugenSetting.KeyPressP2.Y = KeyPressSetting.GetKeyCode(cboP2Y.Text.Trim());
                            else if (control == cboP2Z) MugenSetting.KeyPressP2.Z = KeyPressSetting.GetKeyCode(cboP2Z.Text.Trim());
                            else if (control == cboP2Start) MugenSetting.KeyPressP2.Start = KeyPressSetting.GetKeyCode(cboP2Start.Text.Trim());
                        }
                        catch (FormatException)
                        {
                            if (control is TextBox) ((TextBox)control).SelectAll();
                            control.Focus();
                            throw new ApplicationException("Numerical format error！");
                        }
                        catch (OverflowException)
                        {
                            if (control is TextBox) ((TextBox)control).SelectAll();
                            control.Focus();
                            throw new ApplicationException("Numerical value！");
                        }
                        catch (ApplicationException ex)
                        {
                            if (control is TextBox) ((TextBox)control).SelectAll();
                            control.Focus();
                            throw ex;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Initialization button Set drop-down list
        /// </summary>
        private void KeyPressComboBoxInit()
        {
            foreach (Control control in grpKeyPressSetting.Controls)
            {
                if (control is ComboBox)
                {
                    ((ComboBox)control).Items.Clear();
                    ((ComboBox)control).Items.AddRange(KeyPressSetting.KeyCode.Values.ToArray());
                }
            }
        }

        /// <summary>
        /// When the main program configuration tab gets the focus
        /// </summary>
        private void pageMugenCfgSetting_Enter(object sender, EventArgs e)
        {
            Size = new Size(532, 468);
            if (!MugenCfgModifyEnabled)
            {
                ReadMugenCfgSetting();
            }
        }

        /// <summary>
        /// When the modification button of the master program configuration tab is clicked
        /// </summary>
        private void btnMugenCfgModify_Click(object sender, EventArgs e)
        {
            try
            {
                ReadMugenCfgControlsValue();
                MugenSetting.SaveMugenCfgSetting();
            }
            catch (ApplicationException ex)
            {
                try
                {
                    MugenSetting.ReadMugenCfgSetting();
                }
                catch (ApplicationException) { }
                ShowErrorMsg(ex.Message);
                return;
            }
            ShowSuccessMsg("Set the modification success！");
        }

        /// <summary>
        /// When a reset button is clicked in the master program
        /// </summary>
        private void btnMugenCfgReset_Click(object sender, EventArgs e)
        {
            ReadMugenCfgSetting();
        }

        /// <summary>
        /// When a backup button is clicked in the primary program configuration tab
        /// </summary>
        private void btnMugenCfgBackup_Click(object sender, EventArgs e)
        {
            try
            {
                MugenSetting.BackupMugenCfgSetting();
            }
            catch (ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            btnMugenCfgRestore.Enabled = true;
            ShowSuccessMsg("Set backup success！");
        }

        /// <summary>
        /// When a restore button is clicked in the master program configuration tab
        /// </summary>
        private void btnMugenCfgRestore_Click(object sender, EventArgs e)
        {
            try
            {
                MugenSetting.RestoreMugenCfgSetting();
            }
            catch (ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            ReadMugenCfgSetting();
            ShowSuccessMsg("Set the restore success！");
        }

        /// <summary>
        /// When the value of the Difficulty slider changes
        /// </summary>
        private void trbDifficulty_ValueChanged(object sender, EventArgs e)
        {
            lblDifficultyValue.Text = trbDifficulty.Value.ToString();
        }

        /// <summary>
        /// When the value of the Game Speed ​​slider changes
        /// </summary>
        private void trbGameSpeed_ValueChanged(object sender, EventArgs e)
        {
            lblGameSpeedValue.Text = (trbGameSpeed.Value > 0 ? "+" : "") + trbGameSpeed.Value;
        }

        #endregion Main program configuration tab
    }
}