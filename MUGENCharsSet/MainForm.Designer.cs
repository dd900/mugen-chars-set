﻿namespace MUGENCharsSet
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lblMugenExePath = new System.Windows.Forms.Label();
            this.txtMugenExePath = new System.Windows.Forms.TextBox();
            this.btnOpenMugenExe = new System.Windows.Forms.Button();
            this.grpChars = new System.Windows.Forms.GroupBox();
            this.cbbReadCharType = new System.Windows.Forms.ComboBox();
            this.btnSearchDown = new System.Windows.Forms.Button();
            this.btnSearchUp = new System.Windows.Forms.Button();
            this.txtKeyword = new System.Windows.Forms.TextBox();
            this.btnSelectInvert = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.chkAutoSort = new System.Windows.Forms.CheckBox();
            this.lstChars = new System.Windows.Forms.ListBox();
            this.ctxmnuCharList = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiOpenDefFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenCnsFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenDefDir = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDeleteChar = new System.Windows.Forms.ToolStripMenuItem();
            this.btnReadChars = new System.Windows.Forms.Button();
            this.grpProperty = new System.Windows.Forms.GroupBox();
            this.txtDefence = new System.Windows.Forms.TextBox();
            this.lblDefence = new System.Windows.Forms.Label();
            this.txtAttack = new System.Windows.Forms.TextBox();
            this.lblAttack = new System.Windows.Forms.Label();
            this.txtPower = new System.Windows.Forms.TextBox();
            this.lblPower = new System.Windows.Forms.Label();
            this.txtLife = new System.Windows.Forms.TextBox();
            this.lblLife = new System.Windows.Forms.Label();
            this.txtDisplayName = new System.Windows.Forms.TextBox();
            this.lblDisplayName = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblName = new System.Windows.Forms.Label();
            this.lblDefPath = new System.Windows.Forms.Label();
            this.grpPal = new System.Windows.Forms.GroupBox();
            this.dgvPal = new System.Windows.Forms.DataGridView();
            this.PalNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PalVal = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.btnModify = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnBackup = new System.Windows.Forms.Button();
            this.btnRestore = new System.Windows.Forms.Button();
            this.ttpCommon = new System.Windows.Forms.ToolTip(this.components);
            this.grpDefPath = new System.Windows.Forms.GroupBox();
            this.grpMugenExePath = new System.Windows.Forms.GroupBox();
            this.mnuMain = new System.Windows.Forms.MenuStrip();
            this.tsmiApp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiLaunchMugenExe = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiOpenSelectDef = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiSetting = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.ofdOpenMugenExe = new System.Windows.Forms.OpenFileDialog();
            this.grpChars.SuspendLayout();
            this.ctxmnuCharList.SuspendLayout();
            this.grpProperty.SuspendLayout();
            this.grpPal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPal)).BeginInit();
            this.grpDefPath.SuspendLayout();
            this.grpMugenExePath.SuspendLayout();
            this.mnuMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblMugenExePath
            // 
            this.lblMugenExePath.AutoSize = true;
            this.lblMugenExePath.Location = new System.Drawing.Point(6, 18);
            this.lblMugenExePath.Name = "lblMugenExePath";
            this.lblMugenExePath.Size = new System.Drawing.Size(95, 12);
            this.lblMugenExePath.TabIndex = 0;
            this.lblMugenExePath.Text = "MUGEN程序位置：";
            // 
            // txtMugenExePath
            // 
            this.txtMugenExePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMugenExePath.Location = new System.Drawing.Point(107, 15);
            this.txtMugenExePath.Name = "txtMugenExePath";
            this.txtMugenExePath.Size = new System.Drawing.Size(383, 21);
            this.txtMugenExePath.TabIndex = 1;
            this.txtMugenExePath.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtMugenDir_KeyDown);
            // 
            // btnOpenMugenExe
            // 
            this.btnOpenMugenExe.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenMugenExe.Location = new System.Drawing.Point(496, 13);
            this.btnOpenMugenExe.Name = "btnOpenMugenExe";
            this.btnOpenMugenExe.Size = new System.Drawing.Size(31, 23);
            this.btnOpenMugenExe.TabIndex = 2;
            this.btnOpenMugenExe.Text = "...";
            this.btnOpenMugenExe.UseVisualStyleBackColor = true;
            this.btnOpenMugenExe.Click += new System.EventHandler(this.btnOpenMugenExe_Click);
            // 
            // grpChars
            // 
            this.grpChars.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpChars.Controls.Add(this.cbbReadCharType);
            this.grpChars.Controls.Add(this.btnSearchDown);
            this.grpChars.Controls.Add(this.btnSearchUp);
            this.grpChars.Controls.Add(this.txtKeyword);
            this.grpChars.Controls.Add(this.btnSelectInvert);
            this.grpChars.Controls.Add(this.btnSelectAll);
            this.grpChars.Controls.Add(this.chkAutoSort);
            this.grpChars.Controls.Add(this.lstChars);
            this.grpChars.Controls.Add(this.btnReadChars);
            this.grpChars.Location = new System.Drawing.Point(12, 77);
            this.grpChars.Name = "grpChars";
            this.grpChars.Size = new System.Drawing.Size(220, 485);
            this.grpChars.TabIndex = 1;
            this.grpChars.TabStop = false;
            this.grpChars.Text = "人物列表";
            // 
            // cbbReadCharType
            // 
            this.cbbReadCharType.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbbReadCharType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbbReadCharType.FormattingEnabled = true;
            this.cbbReadCharType.Items.AddRange(new object[] {
            "select.def",
            "人物文件夹"});
            this.cbbReadCharType.Location = new System.Drawing.Point(6, 425);
            this.cbbReadCharType.Name = "cbbReadCharType";
            this.cbbReadCharType.Size = new System.Drawing.Size(208, 20);
            this.cbbReadCharType.TabIndex = 7;
            this.cbbReadCharType.SelectedIndexChanged += new System.EventHandler(this.cbbReadCharType_SelectedIndexChanged);
            // 
            // btnSearchDown
            // 
            this.btnSearchDown.Location = new System.Drawing.Point(191, 18);
            this.btnSearchDown.Name = "btnSearchDown";
            this.btnSearchDown.Size = new System.Drawing.Size(23, 23);
            this.btnSearchDown.TabIndex = 2;
            this.btnSearchDown.Text = "∨";
            this.btnSearchDown.UseVisualStyleBackColor = true;
            this.btnSearchDown.Click += new System.EventHandler(this.btnSearchDown_Click);
            // 
            // btnSearchUp
            // 
            this.btnSearchUp.Location = new System.Drawing.Point(168, 18);
            this.btnSearchUp.Name = "btnSearchUp";
            this.btnSearchUp.Size = new System.Drawing.Size(23, 23);
            this.btnSearchUp.TabIndex = 1;
            this.btnSearchUp.Text = "∧";
            this.btnSearchUp.UseVisualStyleBackColor = true;
            this.btnSearchUp.Click += new System.EventHandler(this.btnSearchUp_Click);
            // 
            // txtKeyword
            // 
            this.txtKeyword.Location = new System.Drawing.Point(6, 20);
            this.txtKeyword.MaxLength = 255;
            this.txtKeyword.Name = "txtKeyword";
            this.txtKeyword.Size = new System.Drawing.Size(156, 21);
            this.txtKeyword.TabIndex = 0;
            this.txtKeyword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSearch_KeyDown);
            // 
            // btnSelectInvert
            // 
            this.btnSelectInvert.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectInvert.Location = new System.Drawing.Point(49, 396);
            this.btnSelectInvert.Name = "btnSelectInvert";
            this.btnSelectInvert.Size = new System.Drawing.Size(42, 23);
            this.btnSelectInvert.TabIndex = 5;
            this.btnSelectInvert.Text = "反选";
            this.btnSelectInvert.UseVisualStyleBackColor = true;
            this.btnSelectInvert.Click += new System.EventHandler(this.btnSelectInvert_Click);
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnSelectAll.Location = new System.Drawing.Point(6, 396);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(42, 23);
            this.btnSelectAll.TabIndex = 4;
            this.btnSelectAll.Text = "全选";
            this.btnSelectAll.UseVisualStyleBackColor = true;
            this.btnSelectAll.Click += new System.EventHandler(this.btnSelectAll_Click);
            // 
            // chkAutoSort
            // 
            this.chkAutoSort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoSort.AutoSize = true;
            this.chkAutoSort.Location = new System.Drawing.Point(142, 400);
            this.chkAutoSort.Name = "chkAutoSort";
            this.chkAutoSort.Size = new System.Drawing.Size(72, 16);
            this.chkAutoSort.TabIndex = 6;
            this.chkAutoSort.Text = "自动排序";
            this.chkAutoSort.UseVisualStyleBackColor = true;
            this.chkAutoSort.CheckedChanged += new System.EventHandler(this.chkAutoSort_CheckedChanged);
            // 
            // lstChars
            // 
            this.lstChars.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstChars.ContextMenuStrip = this.ctxmnuCharList;
            this.lstChars.FormattingEnabled = true;
            this.lstChars.HorizontalScrollbar = true;
            this.lstChars.ItemHeight = 12;
            this.lstChars.Location = new System.Drawing.Point(6, 53);
            this.lstChars.Name = "lstChars";
            this.lstChars.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstChars.Size = new System.Drawing.Size(208, 340);
            this.lstChars.TabIndex = 3;
            this.lstChars.SelectedIndexChanged += new System.EventHandler(this.lstChars_SelectedIndexChanged);
            // 
            // ctxmnuCharList
            // 
            this.ctxmnuCharList.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiOpenDefFile,
            this.tsmiOpenCnsFile,
            this.tsmiOpenDefDir,
            this.tsmiDeleteChar});
            this.ctxmnuCharList.Name = "contextMenuStrip1";
            this.ctxmnuCharList.Size = new System.Drawing.Size(170, 92);
            // 
            // tsmiOpenDefFile
            // 
            this.tsmiOpenDefFile.Name = "tsmiOpenDefFile";
            this.tsmiOpenDefFile.Size = new System.Drawing.Size(169, 22);
            this.tsmiOpenDefFile.Text = "打开def文件(&D)";
            this.tsmiOpenDefFile.Click += new System.EventHandler(this.tsmiOpenDefFile_Click);
            // 
            // tsmiOpenCnsFile
            // 
            this.tsmiOpenCnsFile.Name = "tsmiOpenCnsFile";
            this.tsmiOpenCnsFile.Size = new System.Drawing.Size(169, 22);
            this.tsmiOpenCnsFile.Text = "打开cns文件(&C)";
            this.tsmiOpenCnsFile.Click += new System.EventHandler(this.tsmiOpenCnsFile_Click);
            // 
            // tsmiOpenDefDir
            // 
            this.tsmiOpenDefDir.Name = "tsmiOpenDefDir";
            this.tsmiOpenDefDir.Size = new System.Drawing.Size(169, 22);
            this.tsmiOpenDefDir.Text = "打开文件夹(&O)";
            this.tsmiOpenDefDir.Click += new System.EventHandler(this.tsmiOpenDefDir_Click);
            // 
            // tsmiDeleteChar
            // 
            this.tsmiDeleteChar.Name = "tsmiDeleteChar";
            this.tsmiDeleteChar.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.tsmiDeleteChar.Size = new System.Drawing.Size(169, 22);
            this.tsmiDeleteChar.Text = "删除人物";
            this.tsmiDeleteChar.Click += new System.EventHandler(this.tsmiDeleteChar_Click);
            // 
            // btnReadChars
            // 
            this.btnReadChars.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnReadChars.Location = new System.Drawing.Point(6, 451);
            this.btnReadChars.Name = "btnReadChars";
            this.btnReadChars.Size = new System.Drawing.Size(208, 28);
            this.btnReadChars.TabIndex = 8;
            this.btnReadChars.Text = "读取人物列表";
            this.btnReadChars.UseVisualStyleBackColor = true;
            this.btnReadChars.Click += new System.EventHandler(this.btnReadChars_Click);
            // 
            // grpProperty
            // 
            this.grpProperty.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpProperty.Controls.Add(this.txtDefence);
            this.grpProperty.Controls.Add(this.lblDefence);
            this.grpProperty.Controls.Add(this.txtAttack);
            this.grpProperty.Controls.Add(this.lblAttack);
            this.grpProperty.Controls.Add(this.txtPower);
            this.grpProperty.Controls.Add(this.lblPower);
            this.grpProperty.Controls.Add(this.txtLife);
            this.grpProperty.Controls.Add(this.lblLife);
            this.grpProperty.Controls.Add(this.txtDisplayName);
            this.grpProperty.Controls.Add(this.lblDisplayName);
            this.grpProperty.Controls.Add(this.txtName);
            this.grpProperty.Controls.Add(this.lblName);
            this.grpProperty.Location = new System.Drawing.Point(238, 120);
            this.grpProperty.Name = "grpProperty";
            this.grpProperty.Size = new System.Drawing.Size(307, 187);
            this.grpProperty.TabIndex = 3;
            this.grpProperty.TabStop = false;
            this.grpProperty.Text = "人物属性设置";
            // 
            // txtDefence
            // 
            this.txtDefence.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDefence.Location = new System.Drawing.Point(61, 128);
            this.txtDefence.MaxLength = 10;
            this.txtDefence.Name = "txtDefence";
            this.txtDefence.Size = new System.Drawing.Size(240, 21);
            this.txtDefence.TabIndex = 5;
            this.txtDefence.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textProperty_KeyDown);
            // 
            // lblDefence
            // 
            this.lblDefence.AutoSize = true;
            this.lblDefence.Location = new System.Drawing.Point(6, 131);
            this.lblDefence.Name = "lblDefence";
            this.lblDefence.Size = new System.Drawing.Size(41, 12);
            this.lblDefence.TabIndex = 0;
            this.lblDefence.Text = "防御力";
            // 
            // txtAttack
            // 
            this.txtAttack.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtAttack.Location = new System.Drawing.Point(61, 101);
            this.txtAttack.MaxLength = 10;
            this.txtAttack.Name = "txtAttack";
            this.txtAttack.Size = new System.Drawing.Size(240, 21);
            this.txtAttack.TabIndex = 4;
            this.txtAttack.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textProperty_KeyDown);
            // 
            // lblAttack
            // 
            this.lblAttack.AutoSize = true;
            this.lblAttack.Location = new System.Drawing.Point(6, 104);
            this.lblAttack.Name = "lblAttack";
            this.lblAttack.Size = new System.Drawing.Size(41, 12);
            this.lblAttack.TabIndex = 0;
            this.lblAttack.Text = "攻击力";
            // 
            // txtPower
            // 
            this.txtPower.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPower.Location = new System.Drawing.Point(61, 155);
            this.txtPower.MaxLength = 10;
            this.txtPower.Name = "txtPower";
            this.txtPower.Size = new System.Drawing.Size(240, 21);
            this.txtPower.TabIndex = 6;
            this.txtPower.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textProperty_KeyDown);
            // 
            // lblPower
            // 
            this.lblPower.AutoSize = true;
            this.lblPower.Location = new System.Drawing.Point(6, 158);
            this.lblPower.Name = "lblPower";
            this.lblPower.Size = new System.Drawing.Size(41, 12);
            this.lblPower.TabIndex = 0;
            this.lblPower.Text = "气上限";
            // 
            // txtLife
            // 
            this.txtLife.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLife.Location = new System.Drawing.Point(61, 74);
            this.txtLife.MaxLength = 10;
            this.txtLife.Name = "txtLife";
            this.txtLife.Size = new System.Drawing.Size(240, 21);
            this.txtLife.TabIndex = 3;
            this.txtLife.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textProperty_KeyDown);
            // 
            // lblLife
            // 
            this.lblLife.AutoSize = true;
            this.lblLife.Location = new System.Drawing.Point(6, 77);
            this.lblLife.Name = "lblLife";
            this.lblLife.Size = new System.Drawing.Size(41, 12);
            this.lblLife.TabIndex = 0;
            this.lblLife.Text = "生命值";
            // 
            // txtDisplayName
            // 
            this.txtDisplayName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDisplayName.Location = new System.Drawing.Point(61, 47);
            this.txtDisplayName.MaxLength = 255;
            this.txtDisplayName.Name = "txtDisplayName";
            this.txtDisplayName.Size = new System.Drawing.Size(240, 21);
            this.txtDisplayName.TabIndex = 2;
            this.txtDisplayName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textProperty_KeyDown);
            // 
            // lblDisplayName
            // 
            this.lblDisplayName.AutoSize = true;
            this.lblDisplayName.Location = new System.Drawing.Point(6, 50);
            this.lblDisplayName.Name = "lblDisplayName";
            this.lblDisplayName.Size = new System.Drawing.Size(41, 12);
            this.lblDisplayName.TabIndex = 0;
            this.lblDisplayName.Text = "显示名";
            // 
            // txtName
            // 
            this.txtName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtName.Location = new System.Drawing.Point(61, 20);
            this.txtName.MaxLength = 255;
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(240, 21);
            this.txtName.TabIndex = 1;
            this.txtName.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textProperty_KeyDown);
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(6, 23);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(29, 12);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "名称";
            // 
            // lblDefPath
            // 
            this.lblDefPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDefPath.AutoEllipsis = true;
            this.lblDefPath.Location = new System.Drawing.Point(6, 17);
            this.lblDefPath.Name = "lblDefPath";
            this.lblDefPath.Size = new System.Drawing.Size(295, 12);
            this.lblDefPath.TabIndex = 0;
            // 
            // grpPal
            // 
            this.grpPal.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpPal.Controls.Add(this.dgvPal);
            this.grpPal.Location = new System.Drawing.Point(238, 313);
            this.grpPal.Name = "grpPal";
            this.grpPal.Size = new System.Drawing.Size(307, 214);
            this.grpPal.TabIndex = 4;
            this.grpPal.TabStop = false;
            this.grpPal.Text = "色表设置";
            // 
            // dgvPal
            // 
            this.dgvPal.AllowUserToAddRows = false;
            this.dgvPal.AllowUserToDeleteRows = false;
            this.dgvPal.AllowUserToResizeRows = false;
            this.dgvPal.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPal.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.PalNo,
            this.PalVal});
            this.dgvPal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPal.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvPal.Location = new System.Drawing.Point(3, 17);
            this.dgvPal.Name = "dgvPal";
            this.dgvPal.RowHeadersVisible = false;
            this.dgvPal.RowTemplate.Height = 23;
            this.dgvPal.Size = new System.Drawing.Size(301, 194);
            this.dgvPal.TabIndex = 0;
            this.dgvPal.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvPal_EditingControlShowing);
            // 
            // PalNo
            // 
            this.PalNo.HeaderText = "No.";
            this.PalNo.Name = "PalNo";
            this.PalNo.ReadOnly = true;
            this.PalNo.Width = 60;
            // 
            // PalVal
            // 
            this.PalVal.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.PalVal.FillWeight = 184F;
            this.PalVal.HeaderText = "色表";
            this.PalVal.Name = "PalVal";
            // 
            // btnModify
            // 
            this.btnModify.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnModify.Enabled = false;
            this.btnModify.Location = new System.Drawing.Point(238, 533);
            this.btnModify.Name = "btnModify";
            this.btnModify.Size = new System.Drawing.Size(72, 29);
            this.btnModify.TabIndex = 5;
            this.btnModify.Text = "修改";
            this.ttpCommon.SetToolTip(this.btnModify, "快捷键：Ctrl+Enter");
            this.btnModify.UseVisualStyleBackColor = true;
            this.btnModify.Click += new System.EventHandler(this.btnModify_Click);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReset.Enabled = false;
            this.btnReset.Location = new System.Drawing.Point(316, 533);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(72, 29);
            this.btnReset.TabIndex = 6;
            this.btnReset.Text = "重置";
            this.ttpCommon.SetToolTip(this.btnReset, "快捷键：Ctrl+S");
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnBackup
            // 
            this.btnBackup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnBackup.Enabled = false;
            this.btnBackup.Location = new System.Drawing.Point(394, 533);
            this.btnBackup.Name = "btnBackup";
            this.btnBackup.Size = new System.Drawing.Size(72, 29);
            this.btnBackup.TabIndex = 7;
            this.btnBackup.Text = "备份";
            this.ttpCommon.SetToolTip(this.btnBackup, "快捷键：Ctrl+B");
            this.btnBackup.UseVisualStyleBackColor = true;
            this.btnBackup.Click += new System.EventHandler(this.btnBackup_Click);
            // 
            // btnRestore
            // 
            this.btnRestore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRestore.Enabled = false;
            this.btnRestore.Location = new System.Drawing.Point(472, 533);
            this.btnRestore.Name = "btnRestore";
            this.btnRestore.Size = new System.Drawing.Size(72, 29);
            this.btnRestore.TabIndex = 8;
            this.btnRestore.Text = "还原";
            this.ttpCommon.SetToolTip(this.btnRestore, "快捷键：Ctrl+R");
            this.btnRestore.UseVisualStyleBackColor = true;
            this.btnRestore.Click += new System.EventHandler(this.btnRestore_Click);
            // 
            // grpDefPath
            // 
            this.grpDefPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDefPath.Controls.Add(this.lblDefPath);
            this.grpDefPath.Location = new System.Drawing.Point(238, 77);
            this.grpDefPath.Name = "grpDefPath";
            this.grpDefPath.Size = new System.Drawing.Size(307, 37);
            this.grpDefPath.TabIndex = 2;
            this.grpDefPath.TabStop = false;
            this.grpDefPath.Text = "人物配置文件";
            // 
            // grpMugenExePath
            // 
            this.grpMugenExePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpMugenExePath.Controls.Add(this.lblMugenExePath);
            this.grpMugenExePath.Controls.Add(this.txtMugenExePath);
            this.grpMugenExePath.Controls.Add(this.btnOpenMugenExe);
            this.grpMugenExePath.Location = new System.Drawing.Point(12, 28);
            this.grpMugenExePath.Name = "grpMugenExePath";
            this.grpMugenExePath.Size = new System.Drawing.Size(533, 43);
            this.grpMugenExePath.TabIndex = 0;
            this.grpMugenExePath.TabStop = false;
            this.grpMugenExePath.Text = "MUGEN程序";
            // 
            // mnuMain
            // 
            this.mnuMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiApp,
            this.tsmiHelp});
            this.mnuMain.Location = new System.Drawing.Point(0, 0);
            this.mnuMain.Name = "mnuMain";
            this.mnuMain.Size = new System.Drawing.Size(559, 25);
            this.mnuMain.TabIndex = 9;
            // 
            // tsmiApp
            // 
            this.tsmiApp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiLaunchMugenExe,
            this.tsmiOpenSelectDef,
            this.tsmiSetting});
            this.tsmiApp.Name = "tsmiApp";
            this.tsmiApp.Size = new System.Drawing.Size(44, 21);
            this.tsmiApp.Text = "程序";
            // 
            // tsmiLaunchMugenExe
            // 
            this.tsmiLaunchMugenExe.Name = "tsmiLaunchMugenExe";
            this.tsmiLaunchMugenExe.Size = new System.Drawing.Size(194, 22);
            this.tsmiLaunchMugenExe.Text = "运行MUGEN程序(&L)";
            this.tsmiLaunchMugenExe.Click += new System.EventHandler(this.tsmiLaunchMugenExe_Click);
            // 
            // tsmiOpenSelectDef
            // 
            this.tsmiOpenSelectDef.Name = "tsmiOpenSelectDef";
            this.tsmiOpenSelectDef.Size = new System.Drawing.Size(194, 22);
            this.tsmiOpenSelectDef.Text = "打开select.def文件(&S)";
            this.tsmiOpenSelectDef.Click += new System.EventHandler(this.tsmiOpenSelectDef_Click);
            // 
            // tsmiSetting
            // 
            this.tsmiSetting.Name = "tsmiSetting";
            this.tsmiSetting.Size = new System.Drawing.Size(194, 22);
            this.tsmiSetting.Text = "设置(&S)";
            this.tsmiSetting.Click += new System.EventHandler(this.tsmiSetting_Click);
            // 
            // tsmiHelp
            // 
            this.tsmiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiAbout});
            this.tsmiHelp.Name = "tsmiHelp";
            this.tsmiHelp.Size = new System.Drawing.Size(44, 21);
            this.tsmiHelp.Text = "帮助";
            // 
            // tsmiAbout
            // 
            this.tsmiAbout.Name = "tsmiAbout";
            this.tsmiAbout.Size = new System.Drawing.Size(116, 22);
            this.tsmiAbout.Text = "关于(&A)";
            this.tsmiAbout.Click += new System.EventHandler(this.tsmiAbout_Click);
            // 
            // ofdOpenMugenExe
            // 
            this.ofdOpenMugenExe.Filter = "可执行程序|*.exe";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(559, 570);
            this.Controls.Add(this.mnuMain);
            this.Controls.Add(this.grpMugenExePath);
            this.Controls.Add(this.grpDefPath);
            this.Controls.Add(this.btnRestore);
            this.Controls.Add(this.btnBackup);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnModify);
            this.Controls.Add(this.grpPal);
            this.Controls.Add(this.grpProperty);
            this.Controls.Add(this.grpChars);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.mnuMain;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "M.U.G.E.N人物设置";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.grpChars.ResumeLayout(false);
            this.grpChars.PerformLayout();
            this.ctxmnuCharList.ResumeLayout(false);
            this.grpProperty.ResumeLayout(false);
            this.grpProperty.PerformLayout();
            this.grpPal.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPal)).EndInit();
            this.grpDefPath.ResumeLayout(false);
            this.grpMugenExePath.ResumeLayout(false);
            this.grpMugenExePath.PerformLayout();
            this.mnuMain.ResumeLayout(false);
            this.mnuMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMugenExePath;
        private System.Windows.Forms.TextBox txtMugenExePath;
        private System.Windows.Forms.Button btnOpenMugenExe;
        private System.Windows.Forms.GroupBox grpChars;
        private System.Windows.Forms.ListBox lstChars;
        private System.Windows.Forms.GroupBox grpProperty;
        private System.Windows.Forms.TextBox txtDefence;
        private System.Windows.Forms.Label lblDefence;
        private System.Windows.Forms.TextBox txtAttack;
        private System.Windows.Forms.Label lblAttack;
        private System.Windows.Forms.TextBox txtPower;
        private System.Windows.Forms.Label lblPower;
        private System.Windows.Forms.TextBox txtLife;
        private System.Windows.Forms.Label lblLife;
        private System.Windows.Forms.TextBox txtDisplayName;
        private System.Windows.Forms.Label lblDisplayName;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.GroupBox grpPal;
        private System.Windows.Forms.DataGridView dgvPal;
        private System.Windows.Forms.Button btnModify;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnBackup;
        private System.Windows.Forms.Button btnRestore;
        private System.Windows.Forms.Button btnReadChars;
        private System.Windows.Forms.ContextMenuStrip ctxmnuCharList;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenDefFile;
        private System.Windows.Forms.Label lblDefPath;
        private System.Windows.Forms.ToolTip ttpCommon;
        private System.Windows.Forms.GroupBox grpDefPath;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenDefDir;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenCnsFile;
        private System.Windows.Forms.CheckBox chkAutoSort;
        private System.Windows.Forms.DataGridViewTextBoxColumn PalNo;
        private System.Windows.Forms.DataGridViewComboBoxColumn PalVal;
        private System.Windows.Forms.GroupBox grpMugenExePath;
        private System.Windows.Forms.Button btnSelectInvert;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnSearchUp;
        private System.Windows.Forms.TextBox txtKeyword;
        private System.Windows.Forms.MenuStrip mnuMain;
        private System.Windows.Forms.ToolStripMenuItem tsmiHelp;
        private System.Windows.Forms.ToolStripMenuItem tsmiApp;
        private System.Windows.Forms.ToolStripMenuItem tsmiLaunchMugenExe;
        private System.Windows.Forms.ToolStripMenuItem tsmiAbout;
        private System.Windows.Forms.Button btnSearchDown;
        private System.Windows.Forms.ToolStripMenuItem tsmiSetting;
        private System.Windows.Forms.ToolStripMenuItem tsmiDeleteChar;
        private System.Windows.Forms.OpenFileDialog ofdOpenMugenExe;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenSelectDef;
        private System.Windows.Forms.ComboBox cbbReadCharType;
    }
}

