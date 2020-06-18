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
    public partial class MDIParent1 : Form
    {
        private int childFormNumber = 0;

        public MDIParent1()
        {
            InitializeComponent();

            //this.menuStrip.ItemAdded += MenuStrip_ItemAdded;
        }

        //private void MenuStrip_ItemAdded(object sender, ToolStripItemEventArgs e)
        //{
        //    ;// throw new NotImplementedException();

        //    //Type itemType = e.Item.GetType();
        //    //if (itemType.Name == "ControlBoxMenuItem")
        //    //{
        //    //    e.Item.Visible = true;
        //    //}
        //}

        private void ShowNewForm(object sender, EventArgs e)
        {
            Form childForm = new Form();
            childForm.MdiParent = this;
            childForm.Text = "Window " + childFormNumber++;
            var label = new Label();
            label.AutoSize = true;
            label.Text = "This is an MDI bug when using High DPI:" + System.Environment.NewLine;
            label.Text += "1) In Windows settings change the display to a value greater than 100% such as 175%." + System.Environment.NewLine;
            label.Text += "2) When the Mdi child form is maximized it does not fill the Mdi Parent correctly, and the icons for resizing the Mdi child are not rendered correctly.";
            childForm.Controls.Add(label);
            //childForm.FormBorderStyle = FormBorderStyle.None;
            //childForm.WindowState = FormWindowState.Maximized;
            childForm.SizeChanged += ChildForm_SizeChanged;
            //childForm.ControlBox

            //childForm.StartPosition = FormStartPosition.Manual;
            //childForm.Dock = DockStyle.Fill;
            childForm.Show();
        }

        private bool alreadyInMethod = false;
        private Point maximizedLocation = new Point(0, 0);
        private Point resizableLocation = new Point(0, 0);

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
                    resizableLocation = childForm.Location;
                    childForm.Location = maximizedLocation;
                    // restore childForm WindowState to Maximized
                    childForm.WindowState = FormWindowState.Maximized;
                }
            }
            else if (childForm.WindowState != FormWindowState.Maximized && childForm.FormBorderStyle != FormBorderStyle.Sizable)
            {
                childForm.FormBorderStyle = FormBorderStyle.Sizable;
                childForm.Location = resizableLocation;
            }
            alreadyInMethod = false;
        }

        private void ActiveChildFormShiftUp(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                ActiveMdiChild.Location = new Point(ActiveMdiChild.Location.X, ActiveMdiChild.Location.Y - 50);
        }

        private void ActiveChildFormShiftDown(object sender, EventArgs e)
        {
            if (ActiveMdiChild!=null)
                ActiveMdiChild.Location = new Point(ActiveMdiChild.Location.X, ActiveMdiChild.Location.Y + 50);
        }

        private void ActiveChildFormShiftLeft(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                ActiveMdiChild.Location = new Point(ActiveMdiChild.Location.X - 50, ActiveMdiChild.Location.Y);
        }

        private void ActiveChildFormShiftRight(object sender, EventArgs e)
        {
            if (ActiveMdiChild != null)
                ActiveMdiChild.Location = new Point(this.ActiveMdiChild.Location.X + 50, ActiveMdiChild.Location.Y);
        }


        private void OpenFile(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            openFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = openFileDialog.FileName;
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                string FileName = saveFileDialog.FileName;
            }
        }

        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }

        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }

        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }

        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void menuStrip_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }


    }
}
