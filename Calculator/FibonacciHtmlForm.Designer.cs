using System;
using System.Collections.Generic;
using System.Text;

namespace Calculator
{
    partial class FibonacciHtmlForm
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
            this.txtFirstNumber = new System.Windows.Forms.TextBox();
            this.lblFirstNumber = new System.Windows.Forms.Label();
            this.txtSecondNumber = new System.Windows.Forms.TextBox();
            this.lblSecondNumber = new System.Windows.Forms.Label();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblFirstNumberCount = new System.Windows.Forms.Label();
            this.lblSecondNumberCount = new System.Windows.Forms.Label();
            this.bwProcess = new System.ComponentModel.BackgroundWorker();
            this.txtNumberSequence = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtFirstNumber
            // 
            this.txtFirstNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFirstNumber.Location = new System.Drawing.Point(11, 69);
            this.txtFirstNumber.Multiline = true;
            this.txtFirstNumber.Name = "txtFirstNumber";
            this.txtFirstNumber.Size = new System.Drawing.Size(953, 229);
            this.txtFirstNumber.TabIndex = 1;
            // 
            // lblFirstNumber
            // 
            this.lblFirstNumber.AutoSize = true;
            this.lblFirstNumber.Location = new System.Drawing.Point(13, 47);
            this.lblFirstNumber.Name = "lblFirstNumber";
            this.lblFirstNumber.Size = new System.Drawing.Size(114, 19);
            this.lblFirstNumber.TabIndex = 2;
            this.lblFirstNumber.Text = "First Number";
            // 
            // txtSecondNumber
            // 
            this.txtSecondNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSecondNumber.Location = new System.Drawing.Point(11, 331);
            this.txtSecondNumber.Multiline = true;
            this.txtSecondNumber.Name = "txtSecondNumber";
            this.txtSecondNumber.Size = new System.Drawing.Size(953, 278);
            this.txtSecondNumber.TabIndex = 3;
            // 
            // lblSecondNumber
            // 
            this.lblSecondNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSecondNumber.AutoSize = true;
            this.lblSecondNumber.Location = new System.Drawing.Point(13, 309);
            this.lblSecondNumber.Name = "lblSecondNumber";
            this.lblSecondNumber.Size = new System.Drawing.Size(136, 19);
            this.lblSecondNumber.TabIndex = 4;
            this.lblSecondNumber.Text = "Second Number";
            // 
            // txtNumberSequence
            // 
            this.txtNumberSequence.Location = new System.Drawing.Point(971, 12);
            this.txtNumberSequence.Name = "txtNumberSequence";
            this.txtNumberSequence.Size = new System.Drawing.Size(228, 27);
            this.txtNumberSequence.TabIndex = 19;
            // 
            // btnStart
            // 
            this.btnStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStart.Location = new System.Drawing.Point(970, 571);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(229, 38);
            this.btnStart.TabIndex = 5;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.Start_Click);
            // 
            // lblFirstNumberCount
            // 
            this.lblFirstNumberCount.AutoSize = true;
            this.lblFirstNumberCount.Location = new System.Drawing.Point(133, 47);
            this.lblFirstNumberCount.Name = "lblFirstNumberCount";
            this.lblFirstNumberCount.Size = new System.Drawing.Size(0, 19);
            this.lblFirstNumberCount.TabIndex = 17;
            // 
            // lblSecondNumberCount
            // 
            this.lblSecondNumberCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSecondNumberCount.AutoSize = true;
            this.lblSecondNumberCount.Location = new System.Drawing.Point(155, 309);
            this.lblSecondNumberCount.Name = "lblSecondNumberCount";
            this.lblSecondNumberCount.Size = new System.Drawing.Size(0, 19);
            this.lblSecondNumberCount.TabIndex = 18;
            // 
            // bwProcess
            // 
            this.bwProcess.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BwProcess_DoWork);
            this.bwProcess.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.BwProcess_RunWorkerCompleted);
            // 
            // FibonacciForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1201, 613);
            this.Controls.Add(this.lblSecondNumberCount);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lblSecondNumber);
            this.Controls.Add(this.txtSecondNumber);
            this.Controls.Add(this.lblFirstNumber);
            this.Controls.Add(this.txtFirstNumber);
            this.Controls.Add(this.txtNumberSequence);
            this.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "FibonacciForm";
            this.Text = "Fibonacci Generator";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtFirstNumber;
        private System.Windows.Forms.Label lblFirstNumber;
        private System.Windows.Forms.TextBox txtSecondNumber;
        private System.Windows.Forms.Label lblSecondNumber;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblFirstNumberCount;
        private System.Windows.Forms.Label lblSecondNumberCount;
        private System.ComponentModel.BackgroundWorker bwProcess;
        private System.Windows.Forms.TextBox txtNumberSequence;
    }
}

