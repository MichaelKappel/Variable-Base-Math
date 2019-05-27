using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using VariableBase.Mathematics;

namespace Calculator
{
    public partial class FibonacciForm : Form
    {
        public Int32 FirstNumberCount = 0;
        public Int32 SecondNumberCount = 1;

        public Number Number1;
        public Number Number2;

        public IMathEnvironment<Number> Environment;

        public FibonacciForm()
        {
            InitializeComponent();
            this.Environment = new CharMathEnvironment("0123456789");
        }

        private void Start_Click(object sender, EventArgs e)
        {
            if (this.txtFirstNumber.Text == String.Empty)
            {
                this.lblFirstNumberCount.Text = "0";
                this.lblSecondNumberCount.Text = "1";
               this.Number1 = this.Environment.GetNumber(0);
               this.Number2 = this.Environment.GetNumber(1);
            }
            else
            {
               this.Number1 = this.Environment.GetNumber(this.txtFirstNumber.Text);
               this.Number2 = this.Environment.GetNumber(this.txtSecondNumber.Text);
            }

            this.bwProcess.RunWorkerAsync();
        }

        public void NextNumber()
        {
            Number number3 =this.Number1 +this.Number2;

            this.FirstNumberCount += 1;
            SecondNumberCount += 1;

            this.Invoke(new MethodInvoker(() =>
            {
                this.lblFirstNumberCount.Text = this.FirstNumberCount.ToString();
                this.lblSecondNumberCount.Text = this.SecondNumberCount.ToString();
            }));

            if (this.FirstNumberCount % 10000 == 0)
            {
                this.Invoke(new MethodInvoker(() =>
                {
                    try
                    {
                        this.txtFirstNumber.Text = this.Number2.GetCharArray();
                        using (FileStream fs = File.Create(@"..\..\..\Fibonacci\" + this.FirstNumberCount + ".p" + this.Environment.Base))
                        {
                            using (StreamWriter tw = new StreamWriter(fs))
                            {
                                tw.WriteLine(this.txtFirstNumber.Text);
            
                            }
                        }

                        this.txtSecondNumber.Text = this.Number2.GetCharArray();
                        using (FileStream fs = File.Create(@"..\..\..\Fibonacci\" + this.SecondNumberCount + ".p" + this.Environment.Base))
                        {
                            using (StreamWriter tw = new StreamWriter(fs))
                            {
                                tw.WriteLine(this.txtSecondNumber.Text);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.txtFirstNumber.Text = ex.Message;
                    }
                }));
            }

           this.Number1 = this.Number2;
           this.Number2 = number3;
        }

        private void CbBase_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.cbBase.Text == "Radix 63404")
            {
                this.Environment = new CharMathEnvironment(true, true, true, true, true, true, false, false, false, false);
            }
            else
            {
                this.Environment = new CharMathEnvironment(this.cbBase.Text);
            }
        }

        private void BwProcess_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.NextNumber();
        }

        private void BwProcess_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {

            this.bwProcess.RunWorkerAsync();
        }
    }
}
