namespace MapEditor
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.programToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.programMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.smallMapMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.mediumMapMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.loadMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.closeMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.grassButton = new System.Windows.Forms.ToolStripButton();
            this.sandButton = new System.Windows.Forms.ToolStripButton();
            this.waterButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.treeButton = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.renderPanel = new System.Windows.Forms.Panel();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.cellPositionLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.renderPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.programToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(800, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // programToolStripMenuItem
            // 
            this.programToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.programMenu,
            this.loadMenu,
            this.saveMenu,
            this.toolStripSeparator2,
            this.closeMenu});
            this.programToolStripMenuItem.Name = "programToolStripMenuItem";
            this.programToolStripMenuItem.Size = new System.Drawing.Size(65, 20);
            this.programToolStripMenuItem.Text = "Program";
            // 
            // programMenu
            // 
            this.programMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.smallMapMenu,
            this.mediumMapMenu});
            this.programMenu.Name = "programMenu";
            this.programMenu.Size = new System.Drawing.Size(109, 22);
            this.programMenu.Text = "New";
            // 
            // smallMapMenu
            // 
            this.smallMapMenu.Name = "smallMapMenu";
            this.smallMapMenu.Size = new System.Drawing.Size(110, 22);
            this.smallMapMenu.Text = "20 x 20";
            this.smallMapMenu.Click += new System.EventHandler(this.smallMapMenu_Click);
            // 
            // mediumMapMenu
            // 
            this.mediumMapMenu.Name = "mediumMapMenu";
            this.mediumMapMenu.Size = new System.Drawing.Size(110, 22);
            this.mediumMapMenu.Text = "40 x 40";
            this.mediumMapMenu.Click += new System.EventHandler(this.mediumMapMenu_Click);
            // 
            // loadMenu
            // 
            this.loadMenu.Name = "loadMenu";
            this.loadMenu.Size = new System.Drawing.Size(109, 22);
            this.loadMenu.Text = "Load...";
            this.loadMenu.Click += new System.EventHandler(this.loadMenu_Click);
            // 
            // saveMenu
            // 
            this.saveMenu.Enabled = false;
            this.saveMenu.Name = "saveMenu";
            this.saveMenu.Size = new System.Drawing.Size(109, 22);
            this.saveMenu.Text = "Save...";
            this.saveMenu.Click += new System.EventHandler(this.saveMenu_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(106, 6);
            // 
            // closeMenu
            // 
            this.closeMenu.Name = "closeMenu";
            this.closeMenu.Size = new System.Drawing.Size(109, 22);
            this.closeMenu.Text = "Close";
            this.closeMenu.Click += new System.EventHandler(this.closeMenu_Click);
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.grassButton,
            this.sandButton,
            this.waterButton,
            this.toolStripSeparator1,
            this.treeButton,
            this.deleteButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(800, 25);
            this.toolStrip.TabIndex = 4;
            this.toolStrip.Text = "toolStrip1";
            // 
            // grassButton
            // 
            this.grassButton.Checked = true;
            this.grassButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.grassButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.grassButton.Image = ((System.Drawing.Image)(resources.GetObject("grassButton.Image")));
            this.grassButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.grassButton.Name = "grassButton";
            this.grassButton.Size = new System.Drawing.Size(39, 22);
            this.grassButton.Text = "Grass";
            this.grassButton.Click += new System.EventHandler(this.grassButton_Click);
            // 
            // sandButton
            // 
            this.sandButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sandButton.Image = ((System.Drawing.Image)(resources.GetObject("sandButton.Image")));
            this.sandButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sandButton.Name = "sandButton";
            this.sandButton.Size = new System.Drawing.Size(37, 22);
            this.sandButton.Text = "Sand";
            this.sandButton.Click += new System.EventHandler(this.sandButton_Click);
            // 
            // waterButton
            // 
            this.waterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.waterButton.Image = ((System.Drawing.Image)(resources.GetObject("waterButton.Image")));
            this.waterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.waterButton.Name = "waterButton";
            this.waterButton.Size = new System.Drawing.Size(42, 22);
            this.waterButton.Text = "Water";
            this.waterButton.Click += new System.EventHandler(this.waterButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // treeButton
            // 
            this.treeButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.treeButton.Image = ((System.Drawing.Image)(resources.GetObject("treeButton.Image")));
            this.treeButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.treeButton.Name = "treeButton";
            this.treeButton.Size = new System.Drawing.Size(32, 22);
            this.treeButton.Text = "Tree";
            this.treeButton.Click += new System.EventHandler(this.treeButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.deleteButton.Image = ((System.Drawing.Image)(resources.GetObject("deleteButton.Image")));
            this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(44, 22);
            this.deleteButton.Text = "Delete";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // timer
            // 
            this.timer.Interval = 40;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // renderPanel
            // 
            this.renderPanel.Controls.Add(this.statusStrip);
            this.renderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderPanel.Location = new System.Drawing.Point(0, 49);
            this.renderPanel.Name = "renderPanel";
            this.renderPanel.Size = new System.Drawing.Size(800, 401);
            this.renderPanel.TabIndex = 3;
            this.renderPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.renderPanel_Paint);
            this.renderPanel.MouseDown += new System.Windows.Forms.MouseEventHandler(this.renderPanel_MouseDown);
            this.renderPanel.MouseEnter += new System.EventHandler(this.renderPanel_MouseEnter);
            this.renderPanel.MouseLeave += new System.EventHandler(this.renderPanel_MouseLeave);
            this.renderPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.renderPanel_MouseMove);
            this.renderPanel.MouseUp += new System.Windows.Forms.MouseEventHandler(this.renderPanel_MouseUp);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cellPositionLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 379);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(800, 22);
            this.statusStrip.TabIndex = 0;
            // 
            // cellPositionLabel
            // 
            this.cellPositionLabel.Name = "cellPositionLabel";
            this.cellPositionLabel.Size = new System.Drawing.Size(33, 17);
            this.cellPositionLabel.Text = "[cell]";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "OctoMap|*.map";
            this.openFileDialog.Title = "Load OctoMap";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "OctoMap|*.map";
            this.saveFileDialog.Title = "Save OctoMap";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.renderPanel);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "OctoAwesome - MapEditor";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.renderPanel.ResumeLayout(false);
            this.renderPanel.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem programToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem programMenu;
        private System.Windows.Forms.ToolStripMenuItem smallMapMenu;
        private System.Windows.Forms.ToolStripMenuItem mediumMapMenu;
        private System.Windows.Forms.ToolStripMenuItem closeMenu;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Panel renderPanel;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel cellPositionLabel;
        private System.Windows.Forms.ToolStripButton grassButton;
        private System.Windows.Forms.ToolStripButton sandButton;
        private System.Windows.Forms.ToolStripButton waterButton;
        private System.Windows.Forms.ToolStripMenuItem loadMenu;
        private System.Windows.Forms.ToolStripMenuItem saveMenu;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton treeButton;
        private System.Windows.Forms.ToolStripButton deleteButton;
    }
}

