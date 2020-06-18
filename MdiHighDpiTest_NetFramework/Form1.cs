using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
            childForm.SizeChanged += ChildForm_SizeChanged;
            childForm.MinimizeBox = true;
            var label = new Label();
            label.AutoSize = true;
            label.Text = "This is a workaround for the High Def PerMonitorV2 MDI bug:" + System.Environment.NewLine + System.Environment.NewLine;
            label.Text += "a) When child is maximized set its FormBorderStyle to FormBorderStyle.None." + System.Environment.NewLine;
            label.Text += "b) Also when child is maximized make sure its Location is set to Point(0,0)" + System.Environment.NewLine;
            childForm.Controls.Add(label);
            childForm.Show();
        }

        // Refactor needed: These variables should be maintained for each MDI child form.
        private bool alreadyInMethod = false;
        private Point maximizedLocation = new Point(0, 0);
        private Point sizableLocation = new Point(0, 0);
        private void ChildForm_SizeChanged(object sender, EventArgs e)
        {
            if (alreadyInMethod)
                return;
            alreadyInMethod = true;
            var childForm = sender as Form;
            if (childForm.WindowState == FormWindowState.Maximized && childForm.FormBorderStyle != FormBorderStyle.None)
            {
                childForm.FormBorderStyle = FormBorderStyle.None;
                if (childForm.Location != maximizedLocation)
                {
                    // childForm WindowState has to change from Maximized to Normal to allow changing Location
                    childForm.WindowState = FormWindowState.Normal;
                    sizableLocation = childForm.Location;
                    childForm.Location = maximizedLocation;
                    // restore childForm WindowState to Maximized
                    childForm.WindowState = FormWindowState.Maximized;
                }
            }
            else if (childForm.WindowState != FormWindowState.Maximized && childForm.FormBorderStyle != FormBorderStyle.Sizable)
            {
                childForm.FormBorderStyle = FormBorderStyle.Sizable;
                if (childForm.Location == maximizedLocation)
                    childForm.Location = sizableLocation;
            }
            alreadyInMethod = false;
        }

    }
}
