namespace Calculator
{
    partial class MainForm
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
            this.btnPlus = new System.Windows.Forms.Button();
            this.btnMinus = new System.Windows.Forms.Button();
            this.btnMultiply = new System.Windows.Forms.Button();
            this.btnDivide = new System.Windows.Forms.Button();
            this.btnEquals = new System.Windows.Forms.Button();
            this.txtInput = new System.Windows.Forms.TextBox();
            this.lblResult1 = new System.Windows.Forms.Label();
            this.lblResult2 = new System.Windows.Forms.Label();
            this.lblResult3 = new System.Windows.Forms.Label();
            this.lblBase = new System.Windows.Forms.Label();
            this.lblOperator = new System.Windows.Forms.Label();
            this.lblEquals = new System.Windows.Forms.Label();
            this.cbBase = new System.Windows.Forms.ComboBox();
            this.lblNextOperator = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnFibonacci = new System.Windows.Forms.Button();
            this.btnPrime = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnPlus
            // 
            this.btnPlus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPlus.Location = new System.Drawing.Point(604, 582);
            this.btnPlus.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnPlus.Name = "btnPlus";
            this.btnPlus.Size = new System.Drawing.Size(38, 26);
            this.btnPlus.TabIndex = 0;
            this.btnPlus.Text = "+";
            this.btnPlus.UseVisualStyleBackColor = true;
            this.btnPlus.Click += new System.EventHandler(this.BtnPlus_Click);
            // 
            // btnMinus
            // 
            this.btnMinus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMinus.Location = new System.Drawing.Point(653, 582);
            this.btnMinus.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnMinus.Name = "btnMinus";
            this.btnMinus.Size = new System.Drawing.Size(38, 26);
            this.btnMinus.TabIndex = 1;
            this.btnMinus.Text = "-";
            this.btnMinus.UseVisualStyleBackColor = true;
            this.btnMinus.Click += new System.EventHandler(this.BtnMinus_Click);
            // 
            // btnMultiply
            // 
            this.btnMultiply.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMultiply.Location = new System.Drawing.Point(701, 582);
            this.btnMultiply.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnMultiply.Name = "btnMultiply";
            this.btnMultiply.Size = new System.Drawing.Size(38, 26);
            this.btnMultiply.TabIndex = 2;
            this.btnMultiply.Text = "X";
            this.btnMultiply.UseVisualStyleBackColor = true;
            this.btnMultiply.Click += new System.EventHandler(this.BtnMultiply_Click);
            // 
            // btnDivide
            // 
            this.btnDivide.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDivide.Location = new System.Drawing.Point(749, 582);
            this.btnDivide.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnDivide.Name = "btnDivide";
            this.btnDivide.Size = new System.Drawing.Size(38, 26);
            this.btnDivide.TabIndex = 3;
            this.btnDivide.Text = "÷";
            this.btnDivide.UseVisualStyleBackColor = true;
            this.btnDivide.Click += new System.EventHandler(this.BtnDivide_Click);
            // 
            // btnEquals
            // 
            this.btnEquals.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEquals.Location = new System.Drawing.Point(797, 582);
            this.btnEquals.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnEquals.Name = "btnEquals";
            this.btnEquals.Size = new System.Drawing.Size(38, 26);
            this.btnEquals.TabIndex = 4;
            this.btnEquals.Text = "=";
            this.btnEquals.UseVisualStyleBackColor = true;
            this.btnEquals.Click += new System.EventHandler(this.BtnEquals_Click);
            // 
            // txtInput
            // 
            this.txtInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtInput.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.txtInput.Location = new System.Drawing.Point(12, 455);
            this.txtInput.Multiline = true;
            this.txtInput.Name = "txtInput";
            this.txtInput.Size = new System.Drawing.Size(832, 120);
            this.txtInput.TabIndex = 5;
            // 
            // lblResult1
            // 
            this.lblResult1.AutoSize = true;
            this.lblResult1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult1.Location = new System.Drawing.Point(49, 9);
            this.lblResult1.MaximumSize = new System.Drawing.Size(800, 0);
            this.lblResult1.MinimumSize = new System.Drawing.Size(800, 0);
            this.lblResult1.Name = "lblResult1";
            this.lblResult1.Size = new System.Drawing.Size(800, 13);
            this.lblResult1.TabIndex = 6;
            // 
            // lblResult2
            // 
            this.lblResult2.AutoSize = true;
            this.lblResult2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult2.Location = new System.Drawing.Point(48, 121);
            this.lblResult2.MaximumSize = new System.Drawing.Size(800, 0);
            this.lblResult2.MinimumSize = new System.Drawing.Size(800, 0);
            this.lblResult2.Name = "lblResult2";
            this.lblResult2.Size = new System.Drawing.Size(800, 13);
            this.lblResult2.TabIndex = 7;
            // 
            // lblResult3
            // 
            this.lblResult3.AutoSize = true;
            this.lblResult3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResult3.ForeColor = System.Drawing.Color.SeaGreen;
            this.lblResult3.Location = new System.Drawing.Point(48, 246);
            this.lblResult3.MaximumSize = new System.Drawing.Size(800, 0);
            this.lblResult3.MinimumSize = new System.Drawing.Size(800, 0);
            this.lblResult3.Name = "lblResult3";
            this.lblResult3.Size = new System.Drawing.Size(800, 13);
            this.lblResult3.TabIndex = 8;
            // 
            // lblBase
            // 
            this.lblBase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBase.AutoSize = true;
            this.lblBase.Location = new System.Drawing.Point(9, 422);
            this.lblBase.Name = "lblBase";
            this.lblBase.Size = new System.Drawing.Size(54, 19);
            this.lblBase.TabIndex = 10;
            this.lblBase.Text = "Base:";
            // 
            // lblOperator
            // 
            this.lblOperator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblOperator.AutoSize = true;
            this.lblOperator.Location = new System.Drawing.Point(8, 121);
            this.lblOperator.Name = "lblOperator";
            this.lblOperator.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblOperator.Size = new System.Drawing.Size(0, 19);
            this.lblOperator.TabIndex = 11;
            // 
            // lblEquals
            // 
            this.lblEquals.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblEquals.AutoSize = true;
            this.lblEquals.Location = new System.Drawing.Point(11, 240);
            this.lblEquals.Name = "lblEquals";
            this.lblEquals.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblEquals.Size = new System.Drawing.Size(22, 19);
            this.lblEquals.TabIndex = 12;
            this.lblEquals.Text = "=";
            // 
            // cbBase
            // 
            this.cbBase.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbBase.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbBase.FormattingEnabled = true;
            this.cbBase.Items.AddRange(new object[] {
            "01",
            "012",
            "01234",
            "0123456789",
            "0123456789ABCDEF",
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ",
            "Radix 63404"});
            this.cbBase.Location = new System.Drawing.Point(69, 422);
            this.cbBase.Name = "cbBase";
            this.cbBase.Size = new System.Drawing.Size(718, 21);
            this.cbBase.TabIndex = 13;
            this.cbBase.Text = "0123456789";
            this.cbBase.SelectedIndexChanged += new System.EventHandler(this.CbBase_SelectedIndexChanged);
            // 
            // lblNextOperator
            // 
            this.lblNextOperator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNextOperator.AutoSize = true;
            this.lblNextOperator.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNextOperator.ForeColor = System.Drawing.Color.LimeGreen;
            this.lblNextOperator.Location = new System.Drawing.Point(796, 418);
            this.lblNextOperator.Name = "lblNextOperator";
            this.lblNextOperator.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblNextOperator.Size = new System.Drawing.Size(0, 33);
            this.lblNextOperator.TabIndex = 14;
            // 
            // btnClear
            // 
            this.btnClear.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClear.Location = new System.Drawing.Point(480, 582);
            this.btnClear.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 26);
            this.btnClear.TabIndex = 15;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
            // 
            // btnFibonacci
            // 
            this.btnFibonacci.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFibonacci.Location = new System.Drawing.Point(15, 582);
            this.btnFibonacci.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnFibonacci.Name = "btnFibonacci";
            this.btnFibonacci.Size = new System.Drawing.Size(93, 26);
            this.btnFibonacci.TabIndex = 16;
            this.btnFibonacci.Text = "Fibonacci";
            this.btnFibonacci.UseVisualStyleBackColor = true;
            this.btnFibonacci.Click += new System.EventHandler(this.BtnFibonacci_Click);
            // 
            // btnPrime
            // 
            this.btnPrime.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPrime.Location = new System.Drawing.Point(118, 582);
            this.btnPrime.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.btnPrime.Name = "btnPrime";
            this.btnPrime.Size = new System.Drawing.Size(67, 26);
            this.btnPrime.TabIndex = 17;
            this.btnPrime.Text = "Prime";
            this.btnPrime.UseVisualStyleBackColor = true;
            this.btnPrime.Click += new System.EventHandler(this.BtnPrime_Click);
            // 
            // frmCalculator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 613);
            this.Controls.Add(this.btnPrime);
            this.Controls.Add(this.btnFibonacci);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.lblNextOperator);
            this.Controls.Add(this.cbBase);
            this.Controls.Add(this.lblEquals);
            this.Controls.Add(this.lblOperator);
            this.Controls.Add(this.lblBase);
            this.Controls.Add(this.lblResult3);
            this.Controls.Add(this.lblResult2);
            this.Controls.Add(this.lblResult1);
            this.Controls.Add(this.txtInput);
            this.Controls.Add(this.btnEquals);
            this.Controls.Add(this.btnDivide);
            this.Controls.Add(this.btnMultiply);
            this.Controls.Add(this.btnMinus);
            this.Controls.Add(this.btnPlus);
            this.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "frmCalculator";
            this.Text = "Variable Base Calculator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnPlus;
        private System.Windows.Forms.Button btnMinus;
        private System.Windows.Forms.Button btnMultiply;
        private System.Windows.Forms.Button btnDivide;
        private System.Windows.Forms.Button btnEquals;
        private System.Windows.Forms.TextBox txtInput;
        private System.Windows.Forms.Label lblResult1;
        private System.Windows.Forms.Label lblResult2;
        private System.Windows.Forms.Label lblResult3;
        private System.Windows.Forms.Label lblBase;
        private System.Windows.Forms.Label lblOperator;
        private System.Windows.Forms.Label lblEquals;
        private System.Windows.Forms.ComboBox cbBase;
        private System.Windows.Forms.Label lblNextOperator;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnFibonacci;
        private System.Windows.Forms.Button btnPrime;
    }
}

