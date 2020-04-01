namespace OctoAwesome
{
    partial class MainForm
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.programMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.closeMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.renderControl = new OctoAwesome.RenderControl();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 25;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // statusStrip
            // 
            this.statusStrip.Location = new System.Drawing.Point(0, 428);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(800, 22);
            this.statusStrip.TabIndex = 1;
            this.statusStrip.Text = "statusStrip";
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.programMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(800, 24);
            this.menuStrip.TabIndex = 2;
            this.menuStrip.Text = "menuStrip";
            // 
            // programMenu
            // 
            this.programMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeMenu});
            this.programMenu.Name = "programMenu";
            this.programMenu.Size = new System.Drawing.Size(65, 20);
            this.programMenu.Text = "Program";
            // 
            // closeMenu
            // 
            this.closeMenu.Name = "closeMenu";
            this.closeMenu.Size = new System.Drawing.Size(103, 22);
            this.closeMenu.Text = "Close";
            this.closeMenu.Click += new System.EventHandler(this.closeMenu_Click);
            // 
            // renderControl
            // 
            this.renderControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderControl.Game = null;
            this.renderControl.Location = new System.Drawing.Point(0, 24);
            this.renderControl.Name = "renderControl";
            this.renderControl.Size = new System.Drawing.Size(800, 404);
            this.renderControl.TabIndex = 3;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.renderControl);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.menuStrip);
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.Text = "OctoAwesome";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem programMenu;
        private System.Windows.Forms.ToolStripMenuItem closeMenu;
        private RenderControl renderControl;
    }
}

