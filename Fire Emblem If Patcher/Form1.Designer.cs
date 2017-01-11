namespace Fire_Emblem_If_Patcher
{
    partial class Form1
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
            this.RTB_Progress = new System.Windows.Forms.RichTextBox();
            this.CHK_Card2 = new System.Windows.Forms.CheckBox();
            this.B_Patch = new System.Windows.Forms.Button();
            this.B_Xorpads = new System.Windows.Forms.Button();
            this.TB_PatchDir = new System.Windows.Forms.TextBox();
            this.TB_XorpadDir = new System.Windows.Forms.TextBox();
            this.PB_Show = new System.Windows.Forms.ProgressBar();
            this.TB_ROM = new System.Windows.Forms.TextBox();
            this.B_3DS = new System.Windows.Forms.Button();
            this.B_ROM = new System.Windows.Forms.Button();
            this.B_CIA = new System.Windows.Forms.Button();
            this.B_Validate = new System.Windows.Forms.Button();
            this.ProgressLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // RTB_Progress
            // 
            this.RTB_Progress.BackColor = System.Drawing.SystemColors.Control;
            this.RTB_Progress.Location = new System.Drawing.Point(5, 87);
            this.RTB_Progress.Name = "RTB_Progress";
            this.RTB_Progress.ReadOnly = true;
            this.RTB_Progress.Size = new System.Drawing.Size(308, 236);
            this.RTB_Progress.TabIndex = 43;
            this.RTB_Progress.Text = "";
            // 
            // CHK_Card2
            // 
            this.CHK_Card2.AutoSize = true;
            this.CHK_Card2.Location = new System.Drawing.Point(319, 89);
            this.CHK_Card2.Name = "CHK_Card2";
            this.CHK_Card2.Size = new System.Drawing.Size(126, 17);
            this.CHK_Card2.TabIndex = 39;
            this.CHK_Card2.Text = "Media Device: Card2";
            this.CHK_Card2.UseVisualStyleBackColor = true;
            this.CHK_Card2.CheckedChanged += new System.EventHandler(this.CHK_Card2_CheckedChanged);
            // 
            // B_Patch
            // 
            this.B_Patch.Location = new System.Drawing.Point(319, 60);
            this.B_Patch.Name = "B_Patch";
            this.B_Patch.Size = new System.Drawing.Size(120, 22);
            this.B_Patch.TabIndex = 33;
            this.B_Patch.Text = "Open Patch Folder";
            this.B_Patch.UseVisualStyleBackColor = true;
            this.B_Patch.Click += new System.EventHandler(this.B_Patch_Click);
            // 
            // B_Xorpads
            // 
            this.B_Xorpads.Location = new System.Drawing.Point(319, 34);
            this.B_Xorpads.Name = "B_Xorpads";
            this.B_Xorpads.Size = new System.Drawing.Size(120, 22);
            this.B_Xorpads.TabIndex = 32;
            this.B_Xorpads.Text = "Open Xorpad Folder";
            this.B_Xorpads.UseVisualStyleBackColor = true;
            this.B_Xorpads.Click += new System.EventHandler(this.B_Xorpads_Click);
            // 
            // TB_PatchDir
            // 
            this.TB_PatchDir.Location = new System.Drawing.Point(7, 61);
            this.TB_PatchDir.Name = "TB_PatchDir";
            this.TB_PatchDir.ReadOnly = true;
            this.TB_PatchDir.Size = new System.Drawing.Size(306, 20);
            this.TB_PatchDir.TabIndex = 31;
            // 
            // TB_XorpadDir
            // 
            this.TB_XorpadDir.Location = new System.Drawing.Point(7, 35);
            this.TB_XorpadDir.Name = "TB_XorpadDir";
            this.TB_XorpadDir.ReadOnly = true;
            this.TB_XorpadDir.Size = new System.Drawing.Size(306, 20);
            this.TB_XorpadDir.TabIndex = 30;
            // 
            // PB_Show
            // 
            this.PB_Show.Location = new System.Drawing.Point(5, 329);
            this.PB_Show.Name = "PB_Show";
            this.PB_Show.Size = new System.Drawing.Size(434, 23);
            this.PB_Show.TabIndex = 29;
            // 
            // TB_ROM
            // 
            this.TB_ROM.Location = new System.Drawing.Point(7, 9);
            this.TB_ROM.Name = "TB_ROM";
            this.TB_ROM.ReadOnly = true;
            this.TB_ROM.Size = new System.Drawing.Size(306, 20);
            this.TB_ROM.TabIndex = 28;
            // 
            // B_3DS
            // 
            this.B_3DS.Enabled = false;
            this.B_3DS.Location = new System.Drawing.Point(319, 169);
            this.B_3DS.Name = "B_3DS";
            this.B_3DS.Size = new System.Drawing.Size(120, 22);
            this.B_3DS.TabIndex = 27;
            this.B_3DS.Text = "Build .3DS";
            this.B_3DS.UseVisualStyleBackColor = true;
            this.B_3DS.Visible = false;
            this.B_3DS.Click += new System.EventHandler(this.B_3DS_Click);
            // 
            // B_ROM
            // 
            this.B_ROM.Location = new System.Drawing.Point(319, 8);
            this.B_ROM.Name = "B_ROM";
            this.B_ROM.Size = new System.Drawing.Size(120, 22);
            this.B_ROM.TabIndex = 26;
            this.B_ROM.Text = "Open ROM (.3ds)";
            this.B_ROM.UseVisualStyleBackColor = true;
            this.B_ROM.Click += new System.EventHandler(this.B_ROM_Click);
            // 
            // B_CIA
            // 
            this.B_CIA.Enabled = false;
            this.B_CIA.Location = new System.Drawing.Point(319, 141);
            this.B_CIA.Name = "B_CIA";
            this.B_CIA.Size = new System.Drawing.Size(120, 22);
            this.B_CIA.TabIndex = 44;
            this.B_CIA.Text = "Build .CIA";
            this.B_CIA.UseVisualStyleBackColor = true;
            this.B_CIA.Visible = false;
            this.B_CIA.Click += new System.EventHandler(this.B_CIA_Click);
            // 
            // B_Validate
            // 
            this.B_Validate.Enabled = false;
            this.B_Validate.Location = new System.Drawing.Point(319, 112);
            this.B_Validate.Name = "B_Validate";
            this.B_Validate.Size = new System.Drawing.Size(120, 23);
            this.B_Validate.TabIndex = 45;
            this.B_Validate.Text = "Validate Xorpads";
            this.B_Validate.UseVisualStyleBackColor = true;
            this.B_Validate.Click += new System.EventHandler(this.B_Validate_Click);
            // 
            // ProgressLabel
            // 
            this.ProgressLabel.AutoSize = true;
            this.ProgressLabel.Location = new System.Drawing.Point(319, 310);
            this.ProgressLabel.Name = "ProgressLabel";
            this.ProgressLabel.Size = new System.Drawing.Size(21, 13);
            this.ProgressLabel.TabIndex = 46;
            this.ProgressLabel.Text = "0%";
            this.ProgressLabel.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(444, 361);
            this.Controls.Add(this.ProgressLabel);
            this.Controls.Add(this.B_Validate);
            this.Controls.Add(this.B_CIA);
            this.Controls.Add(this.RTB_Progress);
            this.Controls.Add(this.CHK_Card2);
            this.Controls.Add(this.B_Patch);
            this.Controls.Add(this.B_Xorpads);
            this.Controls.Add(this.TB_PatchDir);
            this.Controls.Add(this.TB_XorpadDir);
            this.Controls.Add(this.PB_Show);
            this.Controls.Add(this.TB_ROM);
            this.Controls.Add(this.B_3DS);
            this.Controls.Add(this.B_ROM);
            this.MaximumSize = new System.Drawing.Size(460, 400);
            this.MinimumSize = new System.Drawing.Size(460, 400);
            this.Name = "Form1";
            this.Text = "Fire Emblem: If Patcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox RTB_Progress;
        private System.Windows.Forms.CheckBox CHK_Card2;
        private System.Windows.Forms.Button B_Patch;
        private System.Windows.Forms.Button B_Xorpads;
        private System.Windows.Forms.TextBox TB_PatchDir;
        private System.Windows.Forms.TextBox TB_XorpadDir;
        private System.Windows.Forms.ProgressBar PB_Show;
        private System.Windows.Forms.TextBox TB_ROM;
        private System.Windows.Forms.Button B_3DS;
        private System.Windows.Forms.Button B_ROM;
        private System.Windows.Forms.Button B_CIA;
        private System.Windows.Forms.Button B_Validate;
        private System.Windows.Forms.Label ProgressLabel;
    }
}

