namespace OctoAwesome
{
    partial class InventoryForm
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listViewPlayer = new System.Windows.Forms.ListView();
            this.listViewBox = new System.Windows.Forms.ListView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewPlayer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listViewBox);
            this.splitContainer1.Size = new System.Drawing.Size(800, 450);
            this.splitContainer1.SplitterDistance = 389;
            this.splitContainer1.TabIndex = 0;
            // 
            // listViewPlayer
            // 
            this.listViewPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPlayer.HideSelection = false;
            this.listViewPlayer.Location = new System.Drawing.Point(0, 0);
            this.listViewPlayer.MultiSelect = false;
            this.listViewPlayer.Name = "listViewPlayer";
            this.listViewPlayer.Size = new System.Drawing.Size(389, 450);
            this.listViewPlayer.TabIndex = 0;
            this.listViewPlayer.UseCompatibleStateImageBehavior = false;
            this.listViewPlayer.DoubleClick += new System.EventHandler(this.listViewPlayer_DoubleClick);
            // 
            // listViewBox
            // 
            this.listViewBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewBox.HideSelection = false;
            this.listViewBox.Location = new System.Drawing.Point(0, 0);
            this.listViewBox.MultiSelect = false;
            this.listViewBox.Name = "listViewBox";
            this.listViewBox.Size = new System.Drawing.Size(407, 450);
            this.listViewBox.TabIndex = 1;
            this.listViewBox.UseCompatibleStateImageBehavior = false;
            this.listViewBox.DoubleClick += new System.EventHandler(this.listViewBox_DoubleClick);
            // 
            // InventoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.splitContainer1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "InventoryForm";
            this.Text = "Inventory";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.InventoryForm_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ListView listViewPlayer;
        private System.Windows.Forms.ListView listViewBox;
    }
}