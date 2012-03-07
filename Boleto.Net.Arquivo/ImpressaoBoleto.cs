using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace BoletoNet.Arquivo
{
    public partial class ImpressaoBoleto : Form
    {
        public ImpressaoBoleto()
        {
            InitializeComponent();
        }

        private string GerarImagem()
        {
            string address = webBrowser.Url.ToString();
            int width = 670;
            int height = 805;

            int webBrowserWidth = 670;
            int webBrowserHeight = 805;

            Bitmap bmp = WebsiteThumbnailImageGenerator.GetWebSiteThumbnail(address, webBrowserWidth, webBrowserHeight, width, height);

            string file = Path.Combine(Environment.CurrentDirectory, "boleto.bmp");
            
            bmp.Save(file);

            return file;
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser.ShowPrintDialog();
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            webBrowser.ShowPrintPreviewDialog();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GerarImagem();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            PageSetupDialog Janela = new PageSetupDialog();
            webBrowser.ShowPrintPreviewDialog();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}