using System;
using System.Windows.Forms;

namespace MUGENCharsSet
{
    /// <summary>
    /// 读取人物列表进度窗口类
    /// </summary>
    public partial class ReadCharacterListProgressForm : Form
    {
        /// <summary>
        /// 读取人物列表方法委托
        /// </summary>
        private delegate void ReadCharacterListDelegate();

        public ReadCharacterListProgressForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 当窗口加载时发生
        /// </summary>
        private void ReadCharacterListProgressForm_Load(object sender, EventArgs e)
        {
            ReadCharacterListDelegate readCharacterListDelegate = new ReadCharacterListDelegate(ReadCharacterList);
            BeginInvoke(readCharacterListDelegate);
        }

        /// <summary>
        /// 读取人物列表
        /// </summary>
        private void ReadCharacterList()
        {
            ((MainForm)Owner).ReadCharacterList();
            Close();
        }
    }
}