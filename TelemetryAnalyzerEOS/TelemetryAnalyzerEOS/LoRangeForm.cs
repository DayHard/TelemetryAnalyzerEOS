using System;
using System.Windows.Forms;

namespace TelemetryAnalyzerEOS
{
    public partial class LoRangeForm : Form
    {
        public int Range;
        public LoRangeForm()
        {
            InitializeComponent();
        }

        private void btnLoRange_Click(object sender, EventArgs e)
        {
            var isValid = int.TryParse(tbLoRange.Text, out Range);
            if(isValid)
                Hide();
            else
                MessageBox.Show("Недопустимое значение дальности, пожалуйста введите другое значение.");
        }

        private void LoRangeForm_Load(object sender, EventArgs e)
        {

        }
    }
}
