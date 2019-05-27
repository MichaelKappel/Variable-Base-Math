using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VariableBase.Mathematics;

namespace Calculator
{
    public partial class MainForm : Form
    {
        public IMathEnvironment<Number> Environment;

        public MainForm()
        {
            InitializeComponent();
            this.Environment = new CharMathEnvironment("0123456789");
        }

        public void SetOperator(String arg)
        {
            if (this.lblResult1.Text == String.Empty)
            {
                this.lblResult1.Text = this.txtInput.Text;
            }
            else if(this.lblResult1.Text != String.Empty)
            {
                if (this.lblResult3.Text != String.Empty)
                {
                    this.lblResult1.Text = this.lblResult3.Text;
                }
                if (this.txtInput.Text == String.Empty){
                    this.lblResult2.Text = String.Empty;
                    this.lblResult3.Text = String.Empty;
                }
                else
                {
                    this.lblResult2.Text = this.txtInput.Text;
                    this.CalculateResult();
                }
            }

            this.lblNextOperator.Text = arg;
            this.txtInput.Text = String.Empty;
            this.txtInput.Focus();
        }

        public void CalculateResult()
        {
            try
            {

                Number numberInput1 = this.Environment.GetNumber(this.lblResult1.Text);
                Number numberInput2 = this.Environment.GetNumber(this.lblResult2.Text);
                Number numberInput3 = default(Number);
                if (this.lblNextOperator.Text == "+")
                {
                    numberInput3 = numberInput1 + numberInput2;
                }
                else if (this.lblNextOperator.Text == "-")
                {
                    numberInput3 = numberInput1 - numberInput2;
                }
                else if (this.lblNextOperator.Text == "x")
                {
                    numberInput3 = numberInput1 * numberInput2;
                }
                else if (this.lblNextOperator.Text == "÷")
                {
                    numberInput3 = numberInput1 / numberInput2;
                }

                this.lblOperator.Text = this.lblNextOperator.Text;

                this.lblResult3.Text = numberInput3.GetCharArray();

                this.txtInput.Text = String.Empty;
                this.txtInput.Focus();

            }
            catch (Exception ex)
            {
                this.lblResult3.Text = ex.Message;
            }
        }

        private void BtnPlus_Click(object sender, EventArgs e)
        {
            this.btnPlus.BackColor = Color.LimeGreen;
            this.btnMinus.BackColor = Color.LightGray;
            this.btnMultiply.BackColor = Color.LightGray;
            this.btnDivide.BackColor = Color.LightGray;
            this.SetOperator("+");
        }

        private void BtnMinus_Click(object sender, EventArgs e)
        {
            this.btnPlus.BackColor = Color.LightGray;
            this.btnMinus.BackColor = Color.LimeGreen;
            this.btnMultiply.BackColor = Color.LightGray;
            this.btnDivide.BackColor = Color.LightGray;
            this.SetOperator("-");
        }

        private void BtnMultiply_Click(object sender, EventArgs e)
        {
            this.btnPlus.BackColor = Color.LightGray;
            this.btnMinus.BackColor = Color.LightGray;
            this.btnMultiply.BackColor = Color.LimeGreen;
            this.btnDivide.BackColor = Color.LightGray;
            this.SetOperator("x");
        }

        private void BtnDivide_Click(object sender, EventArgs e)
        {
            this.btnPlus.BackColor = Color.LightGray;
            this.btnMinus.BackColor = Color.LightGray;
            this.btnMultiply.BackColor = Color.LightGray;
            this.btnDivide.BackColor = Color.LimeGreen;
            this.SetOperator("÷");
        }

        private void BtnEquals_Click(object sender, EventArgs e)
        {
            if (this.lblResult3.Text != String.Empty)
            {
                this.lblResult1.Text = this.lblResult3.Text;
            }
            if (this.txtInput.Text == String.Empty)
            {
                this.lblResult2.Text = String.Empty;
                this.lblResult3.Text = String.Empty;
            }
            else
            {
                this.lblResult2.Text = this.txtInput.Text;
                this.CalculateResult();
            }
        }

        private void CbBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.Environment = new CharMathEnvironment(this.cbBase.Text);
        }

        private void BtnClear_Click(object sender, EventArgs e)
        {
            this.txtInput.Text = String.Empty;
            this.lblResult1.Text = String.Empty;
            this.lblResult2.Text = String.Empty;
            this.lblResult3.Text = String.Empty;
            this.lblOperator.Text = String.Empty;
            this.lblNextOperator.Text = String.Empty;
        }

        private void buttonExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void BtnPrime_Click(object sender, EventArgs e)
        {
            var childForm = new PrimeForm();
            childForm.Show();
        }

        private void BtnFibonacci_Click(object sender, EventArgs e)
        {
            var childForm = new FibonacciForm();
            childForm.Show();

            var childForm2 = new FibonacciHtmlForm();
            childForm2.Show();
        }


    }
}
