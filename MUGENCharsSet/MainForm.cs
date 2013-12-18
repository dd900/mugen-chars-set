﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace MUGENCharsSet
{
    /// <summary>
    /// 程序主窗口类
    /// </summary>
    public partial class MainForm : Form
    {
        #region 类常量

        /// <summary>多值的显示值</summary>
        public const string MultiValue = "(多值)";
        /// <summary>序号的列数</summary>
        public const int PalNoColumnNo = 0;
        /// <summary>Pal值的列数</summary>
        public const int PalValColumnNo = 1;

        #endregion

        #region 类私有变量

        private ApplicationSetting _appSetting;
        private MUGENSetting _mugenSetting;
        private bool _modifyEnabled = false;
        private bool _multiModified = false;
        private ArrayList _characterList;
        private bool _characterListControlPreparing = false;
        private int _currentCharacterSearchIndex = -1;

        #endregion

        #region 类属性

        /// <summary>
        /// 获取或设置当前程序配置
        /// </summary>
        public ApplicationSetting AppSetting
        {
            get { return _appSetting; }
            set { _appSetting = value; }
        }

        /// <summary>
        /// 获取或设置当前MUGEN程序配置
        /// </summary>
        public MUGENSetting MugenSetting
        {
            get { return _mugenSetting; }
            set { _mugenSetting = value; }
        }

        /// <summary>
        /// 获取或设置MUGEN人物列表
        /// </summary>
        private ArrayList CharacterList
        {
            get { return _characterList; }
            set { _characterList = value; }
        }

        /// <summary>
        /// 获取或设置人物列表控件是否处于配置DataSource准备过程中
        /// </summary>
        private bool CharacterListControlPreparing
        {
            get { return _characterListControlPreparing; }
            set { _characterListControlPreparing = value; }
        }

        /// <summary>
        /// 获取或设置当前人物列表搜索索引
        /// </summary>
        private int CurrentCharacterSearchIndex
        {
            get { return _currentCharacterSearchIndex; }
            set { _currentCharacterSearchIndex = value; }
        }

        /// <summary>
        /// 获取或设置是否进入批量修改模式
        /// </summary>
        private bool MultiModified
        {
            get { return _multiModified; }
            set
            {
                if (value)
                {
                    ResetCharacterControls();
                    txtName.ReadOnly = true;
                    txtDisplayName.ReadOnly = true;
                    grpPal.Enabled = false;
                    btnRestore.Enabled = false;
                    lblDefPath.Text = MultiValue;
                }
                else
                {
                    txtName.ReadOnly = false;
                    txtDisplayName.ReadOnly = false;
                    grpPal.Enabled = true;
                }
                _multiModified = value;
            }
        }

        /// <summary>
        /// 获取或设置是否进入修改模式
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

        #endregion

        public MainForm()
        {
            InitializeComponent();
        }

        #region 类方法

        /// <summary>
        /// 显示操作成功消息
        /// </summary>
        /// <param name="msg">消息</param>
        private void ShowSuccessMsg(string msg)
        {
            MessageBox.Show(msg, "操作成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示操作失败消息
        /// </summary>
        /// <param name="msg">消息</param>
        private void ShowErrorMsg(string msg)
        {
            MessageBox.Show(msg, "操作失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 重置人物属性设置及色表设置控件
        /// </summary>
        private void ResetCharacterControls()
        {
            lblDefPath.Text = "";
            foreach (Control ctlTemp in grpProperty.Controls)
            {
                if (ctlTemp is TextBox)
                    ((TextBox)ctlTemp).Clear();
            }
            dgvPal.Rows.Clear();
            ((DataGridViewComboBoxColumn)dgvPal.Columns[1]).Items.Clear();
        }

        /// <summary>
        /// 读取程序配置
        /// </summary>
        private void ReadIniSetting()
        {
            AppSetting = new ApplicationSetting();
            chkAutoSort.Checked = AppSetting.AutoSort;
            cboReadCharacterType.SelectedIndex = (int)AppSetting.ReadCharacterType;
        }

        /// <summary>
        /// 读取人物列表
        /// </summary>
        public void ReadCharacterList()
        {
            ModifyEnabled = false;
            MultiModified = false;
            lstCharacterList.DataSource = null;
            txtKeyword.Clear();
            if (!Directory.Exists(MugenSetting.MugenCharsDirPath))
            {
                ShowErrorMsg("无法找到MUGEN人物文件夹！");
                return;
            }
            CharacterList = new ArrayList();
            try
            {
                if (AppSetting.ReadCharacterType == ApplicationSetting.ReadCharTypeEnum.CharsDir)
                {
                    ScanCharacterDir(CharacterList, MugenSetting.MugenCharsDirPath);
                }
                else
                {
                    ReadSelectDef(CharacterList);
                }
            }
            catch(ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            if (AppSetting.AutoSort)
            {
                CharacterList.Sort(new CharacterCompare());
            }
            RefreshCharacterListDataSource(CharacterList);
            fswCharacterCns.Path = MugenSetting.MugenCharsDirPath;
            fswCharacterCns.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 扫描人物文件夹以获取人物列表
        /// </summary>
        /// <param name="characterList">人物列表</param>
        /// <param name="dir">人物文件夹</param>
        private void ScanCharacterDir(ArrayList characterList, string dir)
        {
            string[] tempDefList = Directory.GetFiles(dir, "*" + Character.DefExt);
            foreach (string tempDefPath in tempDefList)
            {
                Application.DoEvents();
                try
                {
                    characterList.Add(new Character(tempDefPath));
                }
                catch (ApplicationException)
                {
                    continue;
                }
            }
            string[] tempDirArr = Directory.GetDirectories(dir);
            foreach (string tempDir in tempDirArr)
            {
                ScanCharacterDir(characterList, tempDir);
            }
        }


        /// <summary>
        /// 读取select.def文件中的人物列表
        /// </summary>
        /// <param name="characterList">人物列表</param>
        /// <exception cref="System.ApplicationException"></exception>
        private void ReadSelectDef(ArrayList characterList)
        {
            string[] characterLines = null;
            string selectDefPath;
            try
            {
                selectDefPath = MugenSetting.GetSelectDefPath();
            }
            catch(ApplicationException ex)
            {
                throw ex;
            }
            if (!File.Exists(selectDefPath)) throw new ApplicationException("select.def文件不存在！");
            try
            {
                string defContent = File.ReadAllText(selectDefPath, Encoding.Default);
                Regex regex = new Regex(@"\[Characters\](.*)\r\n\[ExtraStages\]", RegexOptions.IgnoreCase | RegexOptions.Singleline);
                characterLines = regex.Match(defContent).Groups[1].Value.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            }
            catch (Exception)
            {
                throw new ApplicationException("读取select.def文件失败！");
            }
            if (characterLines.Length == 0) throw new ApplicationException("读取select.def文件失败！");
            ArrayList tempCharacterList = new ArrayList();
            foreach (string tempLine in characterLines)
            {
                Application.DoEvents();
                string line = tempLine.Trim();
                line = line.Split(new string[] { IniFiles.CommentMark }, 2, StringSplitOptions.None)[0];
                if (line == String.Empty || line.ToLower() == "blank" || line.ToLower() == "empty" || line == "/-" || line == "/") continue;
                line = line.Split(new string[] { "," }, 2, StringSplitOptions.None)[0].Trim();
                if (Path.GetExtension(line) != Character.DefExt) line = Tools.GetFormatDirPath(line) + line + Character.DefExt;
                string defPath = MugenSetting.MugenCharsDirPath + line;
                if (!File.Exists(defPath)) continue;
                if (!tempCharacterList.Contains(defPath))
                {
                    tempCharacterList.Add(defPath);
                    try
                    {
                        characterList.Add(new Character(defPath));
                    }
                    catch(ApplicationException)
                    {
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 刷新人物列表控件的DataSource
        /// </summary>
        /// <param name="characterList">人物列表</param>
        private void RefreshCharacterListDataSource(ArrayList characterList)
        {
            BindingSource bs = new BindingSource();
            bs.DataSource = characterList;
            CharacterListControlPreparing = true;
            lstCharacterList.DataSource = bs;
            lstCharacterList.DisplayMember = "Name";
            lstCharacterList.ValueMember = "DefPath";
            lstCharacterList.ClearSelected();
            CharacterListControlPreparing = false;
        }

        /// <summary>
        /// 读取单个人物设置
        /// </summary>
        private void ReadCharacter()
        {
            if (lstCharacterList.SelectedIndices.Count != 1) return;
            ModifyEnabled = false;
            MultiModified = false;
            Character character = (Character)lstCharacterList.SelectedItem;
            if (!File.Exists(character.DefPath))
            {
                ShowErrorMsg("人物def文件不存在！");
                return;
            }
            try
            {
                character.ReadPalSetting();
            }
            catch(ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            SetSingleDefPathLabel(character.DefPath);
            txtName.Text = character.Name;
            txtDisplayName.Text = character.DisplayName;
            txtLife.Text = character.Life.ToString();
            txtAttack.Text = character.Attack.ToString();
            txtDefence.Text = character.Defence.ToString();
            txtPower.Text = character.Power.ToString();
            string[] selectableActFileList = character.SelectableActFileList;
            if (selectableActFileList.Length == 0)
            {
                ShowErrorMsg("色表文件未找到！");
                return;
            }
            DataGridViewComboBoxColumn dgvPalFileList = (DataGridViewComboBoxColumn)dgvPal.Columns[PalValColumnNo];
            dgvPalFileList.Items.AddRange(selectableActFileList);
            foreach (string palKey in character.PalList)
            {
                dgvPal.Rows.Add(palKey);
            }
            for (int i = 0; i < dgvPal.Rows.Count; i++)
            {
                foreach (string item in selectableActFileList)
                {
                    if (Tools.GetSlashPath(character.PalList[dgvPal.Rows[i].Cells[PalNoColumnNo].Value.ToString()].ToLower())
                        == Tools.GetSlashPath(item.ToLower()))
                    {
                        dgvPal.Rows[i].Cells[PalValColumnNo].Value = item;
                    }
                }
            }
            ModifyEnabled = true;
            if (File.Exists(character.DefPath + Character.BakExt))
            {
                btnRestore.Enabled = true;
            }
        }

        /// <summary>
        /// 批量读取人物设置
        /// </summary>
        private void MultiReadCharacter()
        {
            if (lstCharacterList.SelectedIndices.Count <= 1) return;
            Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
            lstCharacterList.SelectedItems.CopyTo(characterList, 0);
            SetMutliDefPathLabel(characterList);
            MultiModified = true;
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
        /// 在标签上显示当前人物的def文件相对路径
        /// </summary>
        /// <param name="defFullPath">def文件绝对路径</param>
        private void SetSingleDefPathLabel(string defFullPath)
        {
            string msg = defFullPath.Substring(MugenSetting.MugenCharsDirPath.Length);
            lblDefPath.Text = msg;
            ttpCommon.SetToolTip(lblDefPath, msg);
        }

        /// <summary>
        /// 在标签上显示当前批量读取的人物的def文件相对路径
        /// </summary>
        private void SetMutliDefPathLabel(Character[] characterList)
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
        /// 修改单个人物设置
        /// </summary>
        private void ModifyCharacter()
        {
            if (lstCharacterList.SelectedIndices.Count != 1) return;
            Character character = ((Character)lstCharacterList.SelectedItem);
            string oriName = character.Name;
            try
            {
                string name, displayName;
                int life, attack, defence, power;
                ReadCharacterControlValues(out name, out displayName, out life, out attack, out defence, out power);
                character.Name = name;
                character.DisplayName = displayName;
                character.Life = life;
                character.Attack = attack;
                character.Defence = defence;
                character.Power = power;
                ReadPalValues(character.PalList);
            }
            catch (Exception ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            try
            {
                fswCharacterCns.EnableRaisingEvents = false;
                character.Save();
            }
            catch(ApplicationException)
            {
                try
                {
                    character.ReadCharacterSetting();
                    character.ReadPalSetting();
                }
                catch (ApplicationException) { }
                fswCharacterCns.EnableRaisingEvents = true;
                ShowErrorMsg("修改失败！");
                return;
            }
            if (oriName != character.Name)
            {
                int index = lstCharacterList.SelectedIndex;
                RefreshCharacterListDataSource(CharacterList);
                lstCharacterList.SelectedIndex = index;
            }
            fswCharacterCns.EnableRaisingEvents = true;
            ShowSuccessMsg("修改成功！");
        }

        /// <summary>
        /// 批量修改人物设置
        /// </summary>
        private void MultiModifyCharacter()
        {
            if (lstCharacterList.SelectedIndices.Count <= 1) return;
            Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
            lstCharacterList.SelectedItems.CopyTo(characterList, 0);
            int total = 0;
            try
            {
                string name, displayName;
                int life, attack, defence, power;
                ReadCharacterControlValues(out name, out displayName, out life, out attack, out defence, out power);
                for (int i = 0; i < characterList.Length; i++)
                {
                    characterList[i].Life = life;
                    characterList[i].Attack = attack;
                    characterList[i].Defence = defence;
                    characterList[i].Power = power;
                }
            }
            catch (Exception ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            fswCharacterCns.EnableRaisingEvents = false;
            total = Character.MultiSave(characterList);
            if (total > 0)
            {
                ShowSuccessMsg(String.Format("共选择{0}项，其中{1}条项目修改成功！", characterList.Length, total));
            }
            else
            {
                ShowErrorMsg("修改失败！");
            }
            fswCharacterCns.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 读取人物属性相关控件上的字段
        /// </summary>
        /// <exception cref="System.ApplicationException"></exception>
        private void ReadCharacterControlValues(out string name, out string displayName,
            out int life, out int attack, out int defence, out int power)
        {
            name = "";
            displayName = "";
            life = 0;
            attack = 0;
            defence = 0;
            power = 0;
            foreach (Control ctlTemp in grpProperty.Controls)
            {
                if (ctlTemp is TextBox)
                {
                    TextBox txtTemp = ((TextBox)ctlTemp);
                    if (txtTemp.Text.Trim() == String.Empty)
                    {
                        txtTemp.SelectAll();
                        txtTemp.Focus();
                        throw new ApplicationException("字段不得为空！");
                    }
                    string[] txtIntNameArr = { txtLife.Name, txtAttack.Name, txtDefence.Name, txtPower.Name };
                    if (txtIntNameArr.Contains(txtTemp.Name))
                    {
                        try
                        {
                            int value = 0;
                            if (txtTemp.Text.Trim() == MultiValue)
                            {
                                value = 0;
                            }
                            else
                            {
                                value = Convert.ToInt32(txtTemp.Text.Trim());
                            }
                            if (value < 0)
                            {
                                throw new ApplicationException("数值不得小于0！");
                            }
                            if (txtTemp.Name == txtLife.Name)
                            {
                                life = value;
                            }
                            else if (txtTemp.Name == txtAttack.Name)
                            {
                                attack = value;
                            }
                            else if (txtTemp.Name == txtDefence.Name)
                            {
                                defence = value;
                            }
                            else if (txtTemp.Name == txtPower.Name)
                            {
                                power = value;
                            }
                        }
                        catch (FormatException)
                        {
                            txtTemp.SelectAll();
                            txtTemp.Focus();
                            throw new ApplicationException("数值格式错误！");
                        }
                        catch (OverflowException)
                        {
                            txtTemp.SelectAll();
                            txtTemp.Focus();
                            throw new ApplicationException("数值超过范围！");
                        }
                    }
                    else
                    {
                        if (txtTemp.Name == txtName.Name)
                        {
                            name = txtTemp.Text.Trim();
                        }
                        else if (txtTemp.Name == txtDisplayName.Name)
                        {
                            displayName = txtTemp.Text.Trim();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 读取Pal属性字段
        /// </summary>
        private void ReadPalValues(NameValueCollection palList)
        {
            for (int i = 0; i < dgvPal.Rows.Count; i++)
            {
                if (dgvPal.Rows[i].Cells[PalValColumnNo].Value != null &&
                    dgvPal.Rows[i].Cells[PalValColumnNo].Value.ToString() != String.Empty)
                {
                    palList[dgvPal.Rows[i].Cells[PalNoColumnNo].Value.ToString()] =
                        dgvPal.Rows[i].Cells[PalValColumnNo].Value.ToString();
                }
            }
        }

        /// <summary>
        /// 在人物列表里查找关键字
        /// </summary>
        /// <param name="isUp">是否向上搜索</param>
        private void SearchKeyword(bool isUp)
        {
            string keyword = txtKeyword.Text.Trim();
            if (keyword == String.Empty) return;
            if (lstCharacterList.Items.Count == 0) return;
            if (lstCharacterList.SelectedIndex != -1)
            {
                CurrentCharacterSearchIndex = lstCharacterList.SelectedIndex;
            }
            lstCharacterList.ClearSelected();
            bool isFind = false;
            if (isUp)
            {
                for (int i = CurrentCharacterSearchIndex - 1; i >= 0; i--)
                {
                    if (((Character)lstCharacterList.Items[i]).Name.ToString().IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        lstCharacterList.SelectedIndex = i;
                        CurrentCharacterSearchIndex = i;
                        isFind = true;
                        break;
                    }
                }
            }
            else
            {
                for (int i = CurrentCharacterSearchIndex + 1; i < lstCharacterList.Items.Count; i++)
                {
                    if (((Character)lstCharacterList.Items[i]).Name.ToString().IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                    {
                        lstCharacterList.SelectedIndex = i;
                        CurrentCharacterSearchIndex = i;
                        isFind = true;
                        break;
                    }
                }
            }
            if (!isFind)
            {
                if (CurrentCharacterSearchIndex != -1)
                {
                    if (isUp) CurrentCharacterSearchIndex = lstCharacterList.Items.Count;
                    else CurrentCharacterSearchIndex = -1;
                    SearchKeyword(isUp);
                }
                else CurrentCharacterSearchIndex = -1;
            }
        }

        /// <summary>
        /// 在人物列表里查找全部关键字
        /// </summary>
        private void SearchAllKeyword()
        {
            string keyword = txtKeyword.Text.Trim();
            if (keyword == String.Empty) return;
            if (lstCharacterList.Items.Count == 0) return;
            lstCharacterList.ClearSelected();
            CharacterListControlPreparing = true;
            for (int i = 0; i < lstCharacterList.Items.Count; i++)
            {
                if (((Character)lstCharacterList.Items[i]).Name.ToString().IndexOf(keyword, StringComparison.CurrentCultureIgnoreCase) >= 0)
                {
                    lstCharacterList.SelectedIndex = i;
                }
            }
            CharacterListControlPreparing = false;
            lstCharacterList_SelectedIndexChanged(null, null);
        }

        #endregion

        #region 类事件

        /// <summary>
        /// 当窗口加载时发生
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            ReadIniSetting();
            string mugenCfgPath = MUGENSetting.GetMugenCfgPath(AppSetting.MugenExePath);
            if (AppSetting.MugenExePath == String.Empty || !File.Exists(AppSetting.MugenExePath) || !File.Exists(mugenCfgPath))
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
                MugenSetting = new MUGENSetting(AppSetting.MugenExePath);
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
        /// 当单击刷新人物列表按钮时发生
        /// </summary>
        private void btnRefreshCharacterList_Click(object sender, EventArgs e)
        {
            ReadCharacterList();
        }

        /// <summary>
        /// 当单击修改人物设置按钮时发生
        /// </summary>
        private void btnModify_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedIndices.Count == 0) return;
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
        /// 当单击重置人物设置按钮时发生
        /// </summary>
        private void btnReset_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedIndices.Count == 0) return;
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
        /// 当单击备份人物设置按钮时发生
        /// </summary>
        private void btnBackup_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedIndices.Count == 0) return;
            if (MultiModified)
            {
                Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
                lstCharacterList.SelectedItems.CopyTo(characterList, 0);
                int total = Character.MultiBackup(characterList);
                if (total > 0)
                {
                    ShowSuccessMsg(String.Format("共选择{0}项，其中{1}条项目备份成功！", characterList.Length, total));
                }
                else
                {
                    ShowErrorMsg("备份失败！");
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
                ShowSuccessMsg("备份成功！");
            }
            btnRestore.Enabled = true;
        }

        /// <summary>
        /// 当单击恢复人物设置按钮时发生
        /// </summary>
        private void btnRestore_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedIndices.Count == 0) return;
            fswCharacterCns.EnableRaisingEvents = false;
            if (MultiModified)
            {
                Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
                lstCharacterList.SelectedItems.CopyTo(characterList, 0);
                int total = Character.MultiRestore(characterList);
                if (total > 0)
                {
                    ShowSuccessMsg(String.Format("共选择{0}项，其中{1}条项目还原成功！", characterList.Length, total));
                    MultiReadCharacter();
                }
                else
                {
                    ShowErrorMsg("还原失败！");
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
                ShowSuccessMsg("恢复成功！");
            }
            fswCharacterCns.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 当人物列表控件选择项改变时发生
        /// </summary>
        private void lstCharacterList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (CharacterListControlPreparing) return;
            if (lstCharacterList.SelectedIndices.Count > 1)
            {
                MultiReadCharacter();
            }
            else
            {
                ReadCharacter();
            }
        }

        /// <summary>
        /// 当点击色表单元格时自动显示下拉框
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
        /// 当自动选择属性改变时发生
        /// </summary>
        private void chkAutoSort_CheckedChanged(object sender, EventArgs e)
        {
            AppSetting.AutoSort = chkAutoSort.Checked;
        }

        /// <summary>
        /// 当单击全选按钮时发生
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
        /// 当单击反选按钮时发生
        /// </summary>
        private void btnSelectInvert_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.Items.Count == 0) return;
            if (lstCharacterList.SelectedIndices.Count == 0)
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
        /// 当单击向上搜索按钮时发生
        /// </summary>
        private void btnSearchUp_Click(object sender, EventArgs e)
        {
            SearchKeyword(true);
        }

        /// <summary>
        /// 当单击向下搜索按钮时发生
        /// </summary>
        private void btnSearchDown_Click(object sender, EventArgs e)
        {
            SearchKeyword(false);
        }

        /// <summary>
        /// 当单击全部搜索按钮时发生
        /// </summary>
        private void btnSearchAll_Click(object sender, EventArgs e)
        {
            SearchAllKeyword();
        }

        /// <summary>
        /// 当搜索文本框控件按下某个键时发生
        /// </summary>
        private void txtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Enter)
            {
                btnSearchDown_Click(null, null);
            }
        }

        /// <summary>
        /// 当窗体控件按下某个键时发生
        /// </summary>
        private void MainForm_KeyDown(object sender, KeyEventArgs e)
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

        /// <summary>
        /// 当人物属性文本框控件按下某个键时发生
        /// </summary>
        private void textProperty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == (int)Keys.Enter)
            {
                SendKeys.Send("{Tab}");
            }
        }

        /// <summary>
        /// 当读取人物列表类型控件选择项改变时发生
        /// </summary>
        private void cboReadCharacterType_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppSetting.ReadCharacterType = (ApplicationSetting.ReadCharTypeEnum)cboReadCharacterType.SelectedIndex;
        }

        /// <summary>
        /// 当人物文件夹下的cns文件改变时发生
        /// </summary>
        private void fswCharacterCns_Changed(object sender, FileSystemEventArgs e)
        {
            string cnsPath = e.FullPath;
            foreach(Character character in CharacterList)
            {
                if (character.CnsFullPath.ToLower() == cnsPath.ToLower())
                {
                    character.ReadCharacterSetting();
                }
            }
        }

        /// <summary>
        /// 当人物def文件拖拽到人物属性标签页时发生
        /// </summary>
        private void pageCharacter_DragDrop(object sender, DragEventArgs e)
        {
            lstCharacterList.ClearSelected();
            ModifyEnabled = false;
            CharacterListControlPreparing = true;
            foreach (string defPath in (string[])e.Data.GetData(DataFormats.FileDrop))
            {
                if (Path.GetExtension(defPath) != Character.DefExt) continue;
                for (int i = 0; i < lstCharacterList.Items.Count; i++)
                {
                    if (((Character)lstCharacterList.Items[i]).DefPath.ToLower() == defPath.ToLower())
                    {
                        lstCharacterList.SetSelected(i, true);
                    }
                }
            }
            CharacterListControlPreparing = false;
            lstCharacterList_SelectedIndexChanged(null, null);
        }

        private void pageCharacter_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        #endregion

        #region 人物列表右键菜单

        /// <summary>
        /// 当单击打开def文件右键菜单项时发生
        /// </summary>
        private void ctxTsmiOpenDefFile_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedIndices.Count == 0) return;
            Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
            lstCharacterList.SelectedItems.CopyTo(characterList, 0);
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
                        ShowErrorMsg("人物def文件不存在！");
                        return;
                    }
                }
                try
                {
                    Process.Start(AppSetting.EditProgramPath, character.DefPath);
                }
                catch (Exception)
                {
                    if (MultiModified)
                    {
                        continue;
                    }
                    else
                    {
                        ShowErrorMsg("未找到文本编辑器！");
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 当单击打开cns文件右键菜单项时发生
        /// </summary>
        private void ctxTsmiOpenCnsFile_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedIndices.Count == 0) return;
            Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
            lstCharacterList.SelectedItems.CopyTo(characterList, 0);
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
                        ShowErrorMsg("人物cns文件不存在！");
                        return;
                    }
                }
                try
                {
                    Process.Start(AppSetting.EditProgramPath, character.CnsFullPath);
                }
                catch (Exception)
                {
                    if (MultiModified)
                    {
                        continue;
                    }
                    else
                    {
                        ShowErrorMsg("未找到文本编辑器！");
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 当单击打开文件夹右键菜单项时发生
        /// </summary>
        private void ctxTsmiOpenDefDir_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedIndices.Count == 0) return;
            Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
            lstCharacterList.SelectedItems.CopyTo(characterList, 0);
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
                        ShowErrorMsg("人物def文件不存在！");
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
                        ShowErrorMsg("打开文件夹失败！");
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// 当单击删除人物右键菜单项时发生
        /// </summary>
        private void ctxTsmiDeleteCharacter_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedIndices.Count == 0) return;
            if (MessageBox.Show("确定删除人物？", "操作确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            if(MultiModified)
            {
                Character[] characterList = new Character[lstCharacterList.SelectedItems.Count];
                lstCharacterList.SelectedItems.CopyTo(characterList, 0);
                int total = Character.MultiDelete(characterList);
                if (total == 0)
                {
                    ShowErrorMsg("删除失败！");
                    return;
                }
                foreach(Character character in characterList)
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
                catch(ApplicationException ex)
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
        }

        /// <summary>
        /// 当单击复制def文件路径右键菜单项时发生
        /// </summary>
        private void ctxTsmiCopyDefPath_Click(object sender, EventArgs e)
        {
            if (lstCharacterList.SelectedIndices.Count == 0) return;
            string copyContent = "";
            for (int i = 0; i < lstCharacterList.SelectedItems.Count; i++)
            {
                copyContent += ((Character)lstCharacterList.SelectedItems[i]).DefPath.Substring(MugenSetting.MugenCharsDirPath.Length);
                if (i < lstCharacterList.SelectedItems.Count - 1)
                {
                    copyContent += "\r\n";
                }
            }
            try
            {
                Clipboard.SetDataObject(copyContent, true, 2, 200);
            }
            catch(Exception)
            {
                ShowErrorMsg("复制失败！");
                return;
            }
            ShowSuccessMsg(String.Format("{0}条项目已复制到剪贴板！", lstCharacterList.SelectedItems.Count));
        }

        #endregion

        #region 主菜单

        /// <summary>
        /// 当单击选择system.def文件菜单项时发生
        /// </summary>
        private void tsmiSetSystemDefPath_Click(object sender, EventArgs e)
        {
            ofdDefPath.InitialDirectory = MugenSetting.MugenDataDirPath;
            if (ofdDefPath.ShowDialog() != DialogResult.OK) return;
            try
            {
                MugenSetting.SetSystemDefPath(ofdDefPath.FileName);
            }
            catch(ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            ReadCharacterList();
        }

        /// <summary>
        /// 当单击选择select.def文件菜单项时发生
        /// </summary>
        private void tsmiSetSelectDefPath_Click(object sender, EventArgs e)
        {
            ofdDefPath.InitialDirectory = MugenSetting.MugenDataDirPath;
            if (ofdDefPath.ShowDialog() != DialogResult.OK) return;
            try
            {
                MugenSetting.SetSelectDefPath(ofdDefPath.FileName);
            }
            catch (ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            ReadCharacterList();
        }

        /// <summary>
        /// 当单击重新载入菜单项时发生
        /// </summary>
        private void tsmiReload_Click(object sender, EventArgs e)
        {
            ReadCharacterList();
        }

        /// <summary>
        /// 当单击运行MUGEN程序菜单项时发生
        /// </summary>
        private void tsmiLaunchMugenExe_Click(object sender, EventArgs e)
        {
            if (File.Exists(AppSetting.MugenExePath))
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo(AppSetting.MugenExePath);
                    psi.UseShellExecute = false;
                    psi.WorkingDirectory = MugenSetting.MugenDirPath;
                    Process.Start(psi);
                }
                catch (Exception)
                {
                    ShowErrorMsg("运行MUGEN程序失败！");
                    return;
                }
            }
            else
            {
                ShowErrorMsg("MUGEN程序不存在！");
            }
        }

        /// <summary>
        /// 当单击设置菜单项时发生
        /// </summary>
        private void tsmiSetting_Click(object sender, EventArgs e)
        {
            SettingForm settingForm = new SettingForm();
            settingForm.Owner = this;
            settingForm.ShowDialog();
        }

        /// <summary>
        /// 当单击打开select.def菜单项时发生
        /// </summary>
        private void tsmiOpenSelectDef_Click(object sender, EventArgs e)
        {
            string selectDefPath;
            try
            {
                selectDefPath = MugenSetting.GetSelectDefPath();
            }
            catch(ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            if (File.Exists(selectDefPath))
            {
                try
                {
                    Process.Start(AppSetting.EditProgramPath, selectDefPath);
                }
                catch (Exception)
                {
                    ShowErrorMsg("未找到文本编辑器！");
                    return;
                }
            }
            else
            {
                ShowErrorMsg("select.def文件不存在！");
            }
        }

        /// <summary>
        /// 当单击打开system.def菜单项时发生
        /// </summary>
        private void tsmiOpenSystemDef_Click(object sender, EventArgs e)
        {
            string systemDefPath;
            try
            {
                systemDefPath = MugenSetting.GetSystemDefPath();
            }
            catch (ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
            if (File.Exists(systemDefPath))
            {
                try
                {
                    Process.Start(AppSetting.EditProgramPath, systemDefPath);
                }
                catch (Exception)
                {
                    ShowErrorMsg("未找到文本编辑器！");
                    return;
                }
            }
            else
            {
                ShowErrorMsg("system.def文件不存在！");
            }
        }

        /// <summary>
        /// 当单击打开mugen.cfg菜单项时发生
        /// </summary>
        private void tsmiOpenMugenCfg_Click(object sender, EventArgs e)
        {
            if (File.Exists(MugenSetting.MugenCfgPath))
            {
                try
                {
                    Process.Start(AppSetting.EditProgramPath, MugenSetting.MugenCfgPath);
                }
                catch (Exception)
                {
                    ShowErrorMsg("未找到文本编辑器！");
                    return;
                }
            }
            else
            {
                ShowErrorMsg("mugen.cfg文件不存在！");
            }
        }

        /// <summary>
        /// 当单击关于菜单项时发生
        /// </summary>
        private void tsmiAbout_Click(object sender, EventArgs e)
        {
            AboutForm aboutForm = new AboutForm();
            aboutForm.ShowDialog();
        }

        #endregion

    }
}
