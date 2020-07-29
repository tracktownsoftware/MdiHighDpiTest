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

            var childForm2 = new Form();
            childForm2.Text = "Mdi Child 2";
            childForm2.MdiParent = this;
            childForm2.WindowState = FormWindowState.Maximized;
            childForm2.SizeChanged += ChildForm_SizeChanged2;
            childForm2.MinimizeBox = true;
            var label2 = new Label();
            label2.AutoSize = true;
            label2.Text = "Hello World";

            Button tileVertical = new Button();
            tileVertical.Text = "TileVertical";
            tileVertical.SetBounds(0, 60, 150, 40);
            tileVertical.Click += (s2, e2) => this.LayoutMdi(MdiLayout.TileVertical);
            childForm2.Controls.Add(tileVertical);

            Button tileHorizontal = new Button();
            tileHorizontal.Text = "TileHorizontal";
            tileHorizontal.SetBounds(0, 120, 150, 40);
            tileHorizontal.Click += (s2, e2) => this.LayoutMdi(MdiLayout.TileHorizontal);
            childForm2.Controls.Add(tileHorizontal);

            Button tileCascade = new Button();
            tileCascade.Text = "TileCascade";
            tileCascade.SetBounds(0, 180, 150, 40);
            tileCascade.Click += (s2, e2) => this.LayoutMdi(MdiLayout.Cascade);
            childForm2.Controls.Add(tileCascade);

            Button newForm = new Button();
            newForm.Text = "New Form";
            newForm.SetBounds(0, 240, 150, 40);
            newForm.Click += (s2, e2) => {
                var newChildForm = new Form();
                newChildForm.Text = "New MDI child";
                newChildForm.MdiParent = this;
                newChildForm.MinimizeBox = true;
                newChildForm.WindowState = FormWindowState.Normal;
                newChildForm.Show();

            };
            childForm2.Controls.Add(newForm);

            childForm2.Controls.Add(label2);
            childForm2.Show();

            this.LayoutMdi(MdiLayout.TileVertical);
            //this.LayoutMdi(MdiLayout.TileHorizontal);
        }


        protected override void OnDpiChanged(DpiChangedEventArgs e)
        {
            base.OnDpiChanged(e);
            Debug.WriteLine("OnDpiChanged");
            CheckMDILayout();
        }

        private void CheckMDILayout()
        {
            if (MdiChildren.Length > 0)
            {
                Form firstChild = firstChild = MdiChildren[0];
                bool isTiledVertical;
                bool isTiledHorizontal;
                switch (MdiChildren.Length)
                {
                    case 2:
                    case 3:
                        isTiledVertical = (bool)(firstChild.Location.Y == 0);
                        isTiledHorizontal = (bool)(firstChild.Location.X == 0);
                        if (isTiledVertical || isTiledHorizontal)
                        {
                            int totalChildWidths = firstChild.Width;
                            int totalChildHeights = firstChild.Height;
                            for (int i = 1; i < MdiChildren.Length; i++)
                            {
                                var nextChild = MdiChildren[i];
                                totalChildWidths += nextChild.Width;
                                totalChildHeights += nextChild.Height;
                                if (firstChild.Size != nextChild.Size)
                                {
                                    isTiledVertical = false;
                                    isTiledHorizontal = false;
                                    break;
                                }
                                if (nextChild.Location.X != 0)
                                    isTiledHorizontal = false;
                                if (nextChild.Location.Y != 0)
                                    isTiledVertical = false;
                            }
                            var fudgeFactor = .80; // acount for buggy .net dpi conversion values
                            if (isTiledVertical && (totalChildWidths > fudgeFactor * ClientSize.Width))
                            {
                                Debug.WriteLine("IsTileVertical totalChildWidths: " + totalChildWidths + " ClientSize.Width: " + ClientSize.Width);
                                this.LayoutMdi(MdiLayout.TileVertical);
                            }
                            else if (isTiledHorizontal && (totalChildHeights > fudgeFactor * ClientSize.Height))
                            {
                                Debug.WriteLine("IsTileVertical totalChildHeights: " + totalChildHeights + " ClientSize.Height: " + ClientSize.Height);
                                this.LayoutMdi(MdiLayout.TileHorizontal);
                            }
                        }
                        break;
                    case int n when (n % 2 == 0):
                        int countTopAligned = 0;
                        int countLeftAligned = 0;
                        isTiledVertical = true;
                        isTiledHorizontal = true;
                        foreach (var childForm in MdiChildren)
                        {
                            if (childForm.Location.X == 0)
                                countLeftAligned++;
                            if (childForm.Location.Y == 0)
                                countTopAligned++;
                        }

                        for (int i = 1; i < MdiChildren.Length; i++)
                        {
                            var nextChild = MdiChildren[i];
                            if (firstChild.Size != nextChild.Size)
                            {
                                isTiledVertical = false;
                                isTiledHorizontal = false;
                                break;
                            }
                            if (nextChild.Location.X == 0)
                                countLeftAligned++;
                            if (nextChild.Location.Y == 0)
                                countTopAligned++;
                        }
                        if (isTiledVertical && (countTopAligned >= countLeftAligned))
                        {
                            Debug.WriteLine("IsTileVertical > 3");
                            this.LayoutMdi(MdiLayout.TileVertical);
                        }
                        else if (isTiledHorizontal && (countLeftAligned > countTopAligned))
                        {
                            Debug.WriteLine("IsTileHorizontal > 3");
                            this.LayoutMdi(MdiLayout.TileHorizontal);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        protected override void OnDpiChangedBeforeParent(EventArgs e)
        {
            base.OnDpiChangedBeforeParent(e);
            Debug.WriteLine("OnDpiChangedBeforeParent");
        }

        protected override void OnDpiChangedAfterParent(EventArgs e)
        {
            base.OnDpiChangedAfterParent(e);
            Debug.WriteLine("OnDpiChangedAfterParent");
        }

        // Refactor needed: These variables should be maintained for each MDI child form.
        private bool alreadyInMethod = false;
        private Point maximizedLocation = new Point(0, 0);
        private Point sizableLocation = new Point(0, 0);
        private void ChildForm_SizeChanged(object sender, EventArgs e)
        {
            Debug.WriteLine("ChildForm_SizeChanged");
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


        private bool alreadyInMethod2 = false;
        private Point maximizedLocation2 = new Point(0, 0);
        private Point sizableLocation2 = new Point(0, 0);
        private void ChildForm_SizeChanged2(object sender, EventArgs e)
        {
            Debug.WriteLine("ChildForm_SizeChanged2");
            if (alreadyInMethod2)
                return;
            alreadyInMethod2 = true;
            var childForm2 = sender as Form;
            if (childForm2.WindowState == FormWindowState.Maximized && childForm2.FormBorderStyle != FormBorderStyle.None)
            {
                childForm2.FormBorderStyle = FormBorderStyle.None;
                if (childForm2.Location != maximizedLocation)
                {
                    // childForm WindowState has to change from Maximized to Normal to allow changing Location
                    childForm2.WindowState = FormWindowState.Normal;
                    sizableLocation = childForm2.Location;
                    childForm2.Location = maximizedLocation;
                    // restore childForm WindowState to Maximized
                    childForm2.WindowState = FormWindowState.Maximized;
                }
            }
            else if (childForm2.WindowState != FormWindowState.Maximized && childForm2.FormBorderStyle != FormBorderStyle.Sizable)
            {
                childForm2.FormBorderStyle = FormBorderStyle.Sizable;
                if (childForm2.Location == maximizedLocation)
                    childForm2.Location = sizableLocation;
            }
            alreadyInMethod2 = false;
        }

    }
}
