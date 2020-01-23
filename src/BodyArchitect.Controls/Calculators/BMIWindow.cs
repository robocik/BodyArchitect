using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using BodyArchitect.Controls.Localization;

namespace BodyArchitect.Controls.Calculators
{
    public partial class BMIWindow : DevExpress.XtraEditors.XtraForm
    {
        public BMIWindow()
        {
            InitializeComponent();
            fillSuperTips();
        }

        void fillSuperTips()
        {
            ControlHelper.AddSuperTip(this.txtWeight, lblWeight.Text, SuperTips.BMI_Weight);
            ControlHelper.AddSuperTip(this.txtBMI, lblBMI.Text, SuperTips.BMI_BMI);
            ControlHelper.AddSuperTip(this.textEdit1, labelControl1.Text, SuperTips.BMI_Height);
            ControlHelper.AddSuperTip(this.btnOK, btnOK.Text, SuperTips.BMI_Calculate);
            ControlHelper.AddSuperTip(this.btnClose, btnClose.Text, SuperTips.BMI_Close);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            double height = double.Parse(textEdit1.Text);
            double weight = double.Parse(txtWeight.Text);
            double bmi=BmiCalculator.Calculate(height, weight);
            txtBMI.Text = bmi.ToString("0.00");
            lblBmiResult.Text = EnumLocalizer.Default.Translate(BmiCalculator.GetResult(bmi, true));
        }

        private void textEdit1_EditValueChanged(object sender, EventArgs e)
        {
            btnOK.Enabled = txtWeight.Text.Length > 0 && textEdit1.Text.Length > 0;
        }
    }
}