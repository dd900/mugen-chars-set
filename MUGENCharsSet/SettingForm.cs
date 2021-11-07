using System;
using System.IO;
using System.Windows.Forms;

namespace MUGENCharsSet
{
    /// <summary>
    /// Set the window class
    /// </summary>
    public partial class SettingForm : Form
    {
        public SettingForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Show operation success message
        /// </summary>
        /// <param name="msg">information</param>
        private void ShowSuccessMsg(string msg)
        {
            MessageBox.Show(msg, "Successful operation", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 显示操作失败消息
        /// </summary>
        /// <param name="msg">消息</param>
        private void ShowErrorMsg(string msg)
        {
            MessageBox.Show(msg, "operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// 当窗口加载时发生
        /// </summary>
        private void SettingForm_Load(object sender, EventArgs e)
        {
            txtMugenExePath.Text = AppConfig.MugenExePath;
            txtEditProgramPath.Text = AppConfig.EditProgramPath;
            chkShowCharacterScreenMark.Checked = AppConfig.ShowCharacterScreenMark;
        }

        /// <summary>
        /// When you click Open Mugen Program Path button
        /// </summary>
        private void btnOpenMugenExePath_Click(object sender, EventArgs e)
        {
            ofdExePath.FileName = AppConfig.MugenExePath;
            if (File.Exists(AppConfig.MugenExePath))
            {
                ofdExePath.InitialDirectory = AppConfig.MugenExePath.GetDirPathOfFile();
            }
            if (ofdExePath.ShowDialog() == DialogResult.OK)
            {
                txtMugenExePath.Text = ofdExePath.FileName;
            }
        }

        /// <summary>
        /// When you click the Open Text Editor Program Path button
        /// </summary>
        private void btnOpenEditProgramPath_Click(object sender, EventArgs e)
        {
            if (ofdExePath.ShowDialog() == DialogResult.OK)
            {
                txtEditProgramPath.Text = ofdExePath.FileName;
            }
        }

        /// <summary>
        /// When you click the OK button
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            MainForm owner = (MainForm)Owner;
            try
            {
                AppConfig.EditProgramPath = txtEditProgramPath.Text.Trim();
                AppConfig.ShowCharacterScreenMark = chkShowCharacterScreenMark.Checked;
                string mugenCfgPath = txtMugenExePath.Text.Trim().GetDirPathOfFile() + MugenSetting.DataDir + MugenSetting.MugenCfgFileName;
                if (!File.Exists(mugenCfgPath))
                {
                    throw new ApplicationException("Mugen.cfg file does not exist！");
                }
                if (AppConfig.MugenExePath != txtMugenExePath.Text.Trim())
                {
                    AppConfig.MugenExePath = txtMugenExePath.Text.Trim();
                    MugenSetting.Init(AppConfig.MugenExePath);
                    owner.ReadCharacterList(true);
                    owner.ReadMugenCfgSetting();
                }
            }
            catch (ApplicationException ex)
            {
                ShowErrorMsg(ex.Message);
                return;
            }
        }

        /// <summary>
        /// When you click the default value button
        /// </summary>
        private void btnDefault_Click(object sender, EventArgs e)
        {
            txtEditProgramPath.Text = AppConfig.DefaultEditProgramPath;
            chkShowCharacterScreenMark.Checked = false;
        }

        /// <summary>
        /// When the file drags and drops to the program path text box
        /// </summary>
        private void txtPath_DragDrop(object sender, DragEventArgs e)
        {
            ((TextBox)sender).Text = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
        }

        private void txtPath_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }
    }
}