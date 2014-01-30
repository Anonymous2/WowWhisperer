namespace WowWhisperer
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
            this.buttonLaunchWow = new System.Windows.Forms.Button();
            this.buttonPerformWhisperer = new System.Windows.Forms.Button();
            this.textBoxWowDir = new System.Windows.Forms.TextBox();
            this.textBoxAccountName = new System.Windows.Forms.TextBox();
            this.textBoxAccountPassword = new System.Windows.Forms.TextBox();
            this.textBoxWhisperMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // buttonLaunchWow
            // 
            this.buttonLaunchWow.Location = new System.Drawing.Point(12, 90);
            this.buttonLaunchWow.Name = "buttonLaunchWow";
            this.buttonLaunchWow.Size = new System.Drawing.Size(121, 23);
            this.buttonLaunchWow.TabIndex = 0;
            this.buttonLaunchWow.Text = "Launch WoW";
            this.buttonLaunchWow.UseVisualStyleBackColor = true;
            this.buttonLaunchWow.Click += new System.EventHandler(this.buttonLaunchWow_Click);
            // 
            // buttonPerformWhisperer
            // 
            this.buttonPerformWhisperer.Location = new System.Drawing.Point(135, 90);
            this.buttonPerformWhisperer.Name = "buttonPerformWhisperer";
            this.buttonPerformWhisperer.Size = new System.Drawing.Size(121, 23);
            this.buttonPerformWhisperer.TabIndex = 1;
            this.buttonPerformWhisperer.Text = "Perform whispers";
            this.buttonPerformWhisperer.UseVisualStyleBackColor = true;
            this.buttonPerformWhisperer.Click += new System.EventHandler(this.buttonPerformWhisperer_Click);
            // 
            // textBoxWowDir
            // 
            this.textBoxWowDir.Location = new System.Drawing.Point(12, 12);
            this.textBoxWowDir.Name = "textBoxWowDir";
            this.textBoxWowDir.Size = new System.Drawing.Size(244, 20);
            this.textBoxWowDir.TabIndex = 2;
            // 
            // textBoxAccountName
            // 
            this.textBoxAccountName.Location = new System.Drawing.Point(12, 64);
            this.textBoxAccountName.Name = "textBoxAccountName";
            this.textBoxAccountName.Size = new System.Drawing.Size(121, 20);
            this.textBoxAccountName.TabIndex = 4;
            // 
            // textBoxAccountPassword
            // 
            this.textBoxAccountPassword.Location = new System.Drawing.Point(135, 64);
            this.textBoxAccountPassword.Name = "textBoxAccountPassword";
            this.textBoxAccountPassword.PasswordChar = '*';
            this.textBoxAccountPassword.Size = new System.Drawing.Size(121, 20);
            this.textBoxAccountPassword.TabIndex = 5;
            // 
            // textBoxWhisperMessage
            // 
            this.textBoxWhisperMessage.Location = new System.Drawing.Point(12, 38);
            this.textBoxWhisperMessage.Name = "textBoxWhisperMessage";
            this.textBoxWhisperMessage.Size = new System.Drawing.Size(244, 20);
            this.textBoxWhisperMessage.TabIndex = 6;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(271, 124);
            this.Controls.Add(this.textBoxWhisperMessage);
            this.Controls.Add(this.textBoxAccountPassword);
            this.Controls.Add(this.textBoxAccountName);
            this.Controls.Add(this.textBoxWowDir);
            this.Controls.Add(this.buttonPerformWhisperer);
            this.Controls.Add(this.buttonLaunchWow);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Whisperer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonLaunchWow;
        private System.Windows.Forms.Button buttonPerformWhisperer;
        private System.Windows.Forms.TextBox textBoxWowDir;
        private System.Windows.Forms.TextBox textBoxAccountName;
        private System.Windows.Forms.TextBox textBoxAccountPassword;
        private System.Windows.Forms.TextBox textBoxWhisperMessage;
    }
}

