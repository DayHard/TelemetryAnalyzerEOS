using System;
using System.Windows.Forms;

namespace TelemetryAnalyzerEOS
{
    public partial class FocusValueForm : Form
    {
        public double FocusValue;
        public FocusValueForm()
        {
            InitializeComponent();
        }

        private void btnFocusValue_Click(object sender, EventArgs e)
        {
            var isValid = double.TryParse(tbFocusValue.Text, out FocusValue);
            if (isValid && FocusValue >= 300 && FocusValue <= 400)         
               Hide();
            else
                MessageBox.Show(@"Недопустимое значение фокуса, значение фокуса может быть от 300 до 400.");
        }
    }
}
