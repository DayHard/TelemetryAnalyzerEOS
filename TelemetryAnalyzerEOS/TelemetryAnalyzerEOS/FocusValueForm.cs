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
            if (isValid)         
               Hide();
            else
                MessageBox.Show("Недопустимое значение фокуса, пожалуйста введите другое значение.");
        }
    }
}
