
namespace MuteIndicator
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.linkHueButton = new System.Windows.Forms.Button();
            this.unlinkHueButton = new System.Windows.Forms.Button();
            this.linkProgressBar = new System.Windows.Forms.ProgressBar();
            this.statusLabel = new System.Windows.Forms.Label();
            this.lightsCheckedListBox = new System.Windows.Forms.CheckedListBox();
            this.lightsLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // linkHueButton
            // 
            this.linkHueButton.Location = new System.Drawing.Point(12, 12);
            this.linkHueButton.Name = "linkHueButton";
            this.linkHueButton.Size = new System.Drawing.Size(114, 23);
            this.linkHueButton.TabIndex = 0;
            this.linkHueButton.Text = "Link Phillips Hue";
            this.linkHueButton.UseVisualStyleBackColor = true;
            this.linkHueButton.Click += new System.EventHandler(this.linkHueButton_Click);
            // 
            // unlinkHueButton
            // 
            this.unlinkHueButton.Location = new System.Drawing.Point(132, 12);
            this.unlinkHueButton.Name = "unlinkHueButton";
            this.unlinkHueButton.Size = new System.Drawing.Size(123, 23);
            this.unlinkHueButton.TabIndex = 1;
            this.unlinkHueButton.Text = "Unlink Phillips Hue";
            this.unlinkHueButton.UseVisualStyleBackColor = true;
            this.unlinkHueButton.Click += new System.EventHandler(this.unlinkHueButton_Click);
            // 
            // linkProgressBar
            // 
            this.linkProgressBar.Enabled = false;
            this.linkProgressBar.Location = new System.Drawing.Point(12, 74);
            this.linkProgressBar.Name = "linkProgressBar";
            this.linkProgressBar.Size = new System.Drawing.Size(243, 23);
            this.linkProgressBar.TabIndex = 2;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.Location = new System.Drawing.Point(12, 47);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(39, 15);
            this.statusLabel.TabIndex = 3;
            this.statusLabel.Text = "Status";
            // 
            // lightsCheckedListBox
            // 
            this.lightsCheckedListBox.CheckOnClick = true;
            this.lightsCheckedListBox.FormattingEnabled = true;
            this.lightsCheckedListBox.Location = new System.Drawing.Point(12, 141);
            this.lightsCheckedListBox.Name = "lightsCheckedListBox";
            this.lightsCheckedListBox.Size = new System.Drawing.Size(243, 148);
            this.lightsCheckedListBox.TabIndex = 4;
            this.lightsCheckedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lightsCheckedListBox_ItemCheck);
            // 
            // lightsLabel
            // 
            this.lightsLabel.AutoSize = true;
            this.lightsLabel.Location = new System.Drawing.Point(12, 113);
            this.lightsLabel.Name = "lightsLabel";
            this.lightsLabel.Size = new System.Drawing.Size(39, 15);
            this.lightsLabel.TabIndex = 5;
            this.lightsLabel.Text = "Lights";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(267, 301);
            this.Controls.Add(this.lightsLabel);
            this.Controls.Add(this.lightsCheckedListBox);
            this.Controls.Add(this.statusLabel);
            this.Controls.Add(this.linkProgressBar);
            this.Controls.Add(this.unlinkHueButton);
            this.Controls.Add(this.linkHueButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button linkHueButton;
        private System.Windows.Forms.Button unlinkHueButton;
        private System.Windows.Forms.ProgressBar linkProgressBar;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.CheckedListBox lightsCheckedListBox;
        private System.Windows.Forms.Label lightsLabel;
    }
}