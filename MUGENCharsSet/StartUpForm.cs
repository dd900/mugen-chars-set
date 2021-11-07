using System;
using System.IO;
using System.Windows.Forms;

namespace MUGENCharsSet
{
    /// <summary>
    /// Start window
    /// </summary>
    public partial class StartUpForm : Form
    {
        public StartUpForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When the window is loaded
        /// </summary>
        private void StartUpForm_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// When you click the OK button
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            MainForm owner = (MainForm)Owner;
            try
            {
                AppConfig.MugenExePath = txtMugenExePath.Text.Trim();
                string mugenCfgPath = AppConfig.MugenExePath.GetDirPathOfFile() + MugenSetting.DataDir + MugenSetting.MugenCfgFileName;
                if (!File.Exists(mugenCfgPath))
                {
                    throw new ApplicationException("Mugen.cfg file does not exist！");
                }
            }
            catch (ApplicationException ex)
            {
                MessageBox.Show(ex.Message, "operation failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        /// <summary>
        /// When you click Open Mugen Program Path button
        /// </summary>
        private void btnOpenMugenExePath_Click(object sender, EventArgs e)
        {
            if (ofdOpenMugenExePath.ShowDialog() == DialogResult.OK)
            {
                txtMugenExePath.Text = ofdOpenMugenExePath.FileName;
            }
        }

        /// <summary>
        /// When the file drags and drops to the Mugen program path text box
        /// </summary>
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