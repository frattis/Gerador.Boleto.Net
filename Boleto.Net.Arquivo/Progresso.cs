using System.Windows.Forms;

namespace BoletoNet.Arquivo
{
    public partial class Progresso : Form
    {
        private bool _allowClose;

        public Progresso()
        {
            InitializeComponent();
        }

/*
        private void Progresso_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing & _allowClose)
                e.Cancel = true;
        }
*/

        public bool AllowClose
        {
            get { return _allowClose; }
        }

        public void ForceClose()
        {
            _allowClose = true;
            _allowClose = false;
        }
    }
}