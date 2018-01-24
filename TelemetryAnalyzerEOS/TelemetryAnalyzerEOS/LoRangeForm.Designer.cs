namespace TelemetryAnalyzerEOS
{
    partial class LoRangeForm
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
            this.btnLoRange = new System.Windows.Forms.Button();
            this.lbLoRange = new System.Windows.Forms.Label();
            this.tbLoRange = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnLoRange
            // 
            this.btnLoRange.Location = new System.Drawing.Point(52, 31);
            this.btnLoRange.Name = "btnLoRange";
            this.btnLoRange.Size = new System.Drawing.Size(88, 23);
            this.btnLoRange.TabIndex = 0;
            this.btnLoRange.Text = "Ok";
            this.btnLoRange.UseVisualStyleBackColor = true;
            this.btnLoRange.Click += new System.EventHandler(this.btnLoRange_Click);
            // 
            // lbLoRange
            // 
            this.lbLoRange.AutoSize = true;
            this.lbLoRange.Location = new System.Drawing.Point(8, 8);
            this.lbLoRange.Name = "lbLoRange";
            this.lbLoRange.Size = new System.Drawing.Size(110, 13);
            this.lbLoRange.TabIndex = 1;
            this.lbLoRange.Text = "Ввод дальности ЛО:";
            // 
            // tbLoRange
            // 
            this.tbLoRange.Location = new System.Drawing.Point(124, 5);
            this.tbLoRange.Name = "tbLoRange";
            this.tbLoRange.Size = new System.Drawing.Size(46, 20);
            this.tbLoRange.TabIndex = 2;
            this.tbLoRange.Text = "0";
            // 
            // LoRangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(182, 61);
            this.Controls.Add(this.tbLoRange);
            this.Controls.Add(this.lbLoRange);
            this.Controls.Add(this.btnLoRange);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Name = "LoRangeForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LoRangeForm";
            this.Load += new System.EventHandler(this.LoRangeForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoRange;
        private System.Windows.Forms.Label lbLoRange;
        private System.Windows.Forms.TextBox tbLoRange;
    }
}