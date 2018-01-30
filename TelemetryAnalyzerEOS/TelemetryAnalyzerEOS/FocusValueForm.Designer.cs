namespace TelemetryAnalyzerEOS
{
    partial class FocusValueForm
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
            this.btnFocusValue = new System.Windows.Forms.Button();
            this.tbFocusValue = new System.Windows.Forms.TextBox();
            this.lbFocusValue = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnFocusValue
            // 
            this.btnFocusValue.Location = new System.Drawing.Point(86, 31);
            this.btnFocusValue.Name = "btnFocusValue";
            this.btnFocusValue.Size = new System.Drawing.Size(69, 23);
            this.btnFocusValue.TabIndex = 0;
            this.btnFocusValue.Text = "Ок";
            this.btnFocusValue.UseVisualStyleBackColor = true;
            this.btnFocusValue.Click += new System.EventHandler(this.btnFocusValue_Click);
            // 
            // tbFocusValue
            // 
            this.tbFocusValue.Location = new System.Drawing.Point(12, 12);
            this.tbFocusValue.Name = "tbFocusValue";
            this.tbFocusValue.Size = new System.Drawing.Size(48, 20);
            this.tbFocusValue.TabIndex = 1;
            this.tbFocusValue.Text = "323";
            // 
            // lbFocusValue
            // 
            this.lbFocusValue.AutoSize = true;
            this.lbFocusValue.Location = new System.Drawing.Point(66, 15);
            this.lbFocusValue.Name = "lbFocusValue";
            this.lbFocusValue.Size = new System.Drawing.Size(174, 13);
            this.lbFocusValue.TabIndex = 2;
            this.lbFocusValue.Text = "Значение фокуса в канале 2, мм";
            // 
            // FocusValueForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(254, 61);
            this.Controls.Add(this.lbFocusValue);
            this.Controls.Add(this.tbFocusValue);
            this.Controls.Add(this.btnFocusValue);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "FocusValueForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Значение фокуса";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFocusValue;
        private System.Windows.Forms.TextBox tbFocusValue;
        private System.Windows.Forms.Label lbFocusValue;
    }
}