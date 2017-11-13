namespace TelemetryAnalyzerEOS
{
    partial class LoadingForm
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
            this.pbAnalysing = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // pbAnalysing
            // 
            this.pbAnalysing.Location = new System.Drawing.Point(12, 12);
            this.pbAnalysing.Maximum = 15;
            this.pbAnalysing.Name = "pbAnalysing";
            this.pbAnalysing.Size = new System.Drawing.Size(370, 23);
            this.pbAnalysing.Step = 1;
            this.pbAnalysing.TabIndex = 1;
            // 
            // LoadingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Silver;
            this.ClientSize = new System.Drawing.Size(394, 47);
            this.Controls.Add(this.pbAnalysing);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoadingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LoadingForm";
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.ProgressBar pbAnalysing;
    }
}