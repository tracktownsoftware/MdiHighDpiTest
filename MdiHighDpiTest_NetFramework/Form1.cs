using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MdiDpiTest_NetFramework
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.IsMdiContainer = true;
            this.Text = "Mdi Container (.Net Framework)";
            var childForm = new Form();
            childForm.Text = "Mdi Child";
            childForm.MdiParent = this;
            childForm.WindowState = FormWindowState.Maximized;
            var label = new Label();
            label.AutoSize = true;
            label.Text = "This is an MDI bug when using High DPI:" + System.Environment.NewLine;
            label.Text += "1) In Windows settings change the display to a value greater than 100% such as 175%." + System.Environment.NewLine;
            label.Text += "2) When the Mdi child form is maximized it does not fill the Mdi Parent correctly, and the icons for resizing the Mdi child are not rendered correctly.";
            childForm.Controls.Add(label);
            childForm.Show();
        }
    }
}
