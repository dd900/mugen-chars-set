using System;
using System.Windows.Forms;

namespace MUGENCharsSet
{
    /// <summary>
    /// About the window class
    /// </summary>
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Occurs when the window is loaded
        /// </summary>
        private void AboutForm_Load(object sender, EventArgs e)
        {
            lblAppName.Text = "M.U.G.E.N character settings " + Application.ProductVersion;
        }

        /// <summary>
        /// Occurs when the program item Url is clicked
        /// </summary>
        private void lnkAppUrl_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                System.Diagnostics.ProcessStartInfo proInfo = new System.Diagnostics.ProcessStartInfo(lnkAppUrl.Text);
                System.Diagnostics.Process pro = System.Diagnostics.Process.Start(proInfo);
            }
            catch (Exception) { }
        }
    }
}