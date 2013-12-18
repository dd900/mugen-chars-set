﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MUGENCharsSet
{
    /// <summary>
    /// 起始窗口类
    /// </summary>
    public partial class StartUpForm : Form
    {
        public StartUpForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 当窗口加载时发生
        /// </summary>
        private void StartUpForm_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 当单击确定按钮时发生
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            MainForm owner = (MainForm)Owner;
            try
            {
                owner.AppSetting.MugenExePath = txtMugenExePath.Text.Trim();
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "操作失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DialogResult = DialogResult.OK;
            Close();
        }

        /// <summary>
        /// 当单击取消按钮时发生
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        /// <summary>
        /// 当单击打开MUGEN程序路径按钮时发生
        /// </summary>
        private void btnOpenMugenExePath_Click(object sender, EventArgs e)
        {
            if (ofdOpenMugenExePath.ShowDialog() == DialogResult.OK)
            {
                txtMugenExePath.Text = ofdOpenMugenExePath.FileName;
            }
        }

        private void txtMugenExePath_DragDrop(object sender, DragEventArgs e)
        {
            txtMugenExePath.Text = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
        }

        private void txtMugenExePath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
    }
}
