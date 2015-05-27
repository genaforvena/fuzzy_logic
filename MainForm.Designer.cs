namespace FuzzyLogic_Mozerov
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
            this.inputTxtBx = new System.Windows.Forms.TextBox();
            this.BrowseBtn = new System.Windows.Forms.Button();
            this.accuracyLbl = new System.Windows.Forms.Label();
            this.accuracyTxtBx = new System.Windows.Forms.TextBox();
            this.processBtn = new System.Windows.Forms.Button();
            this.isBinaryChckBx = new System.Windows.Forms.CheckBox();
            this.inputLbl = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.backPropRadioButton = new System.Windows.Forms.RadioButton();
            this.psoRadioButton = new System.Windows.Forms.RadioButton();
            this.logLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // inputTxtBx
            // 
            this.inputTxtBx.Location = new System.Drawing.Point(135, 41);
            this.inputTxtBx.Name = "inputTxtBx";
            this.inputTxtBx.Size = new System.Drawing.Size(287, 22);
            this.inputTxtBx.TabIndex = 1;
            this.inputTxtBx.Text = "iris.data";
            // 
            // BrowseBtn
            // 
            this.BrowseBtn.Location = new System.Drawing.Point(428, 37);
            this.BrowseBtn.Name = "BrowseBtn";
            this.BrowseBtn.Size = new System.Drawing.Size(95, 25);
            this.BrowseBtn.TabIndex = 2;
            this.BrowseBtn.Text = "Browse";
            this.BrowseBtn.UseVisualStyleBackColor = true;
            this.BrowseBtn.Click += new System.EventHandler(this.BrowseBtn_Click);
            // 
            // accuracyLbl
            // 
            this.accuracyLbl.AutoSize = true;
            this.accuracyLbl.Location = new System.Drawing.Point(23, 234);
            this.accuracyLbl.Name = "accuracyLbl";
            this.accuracyLbl.Size = new System.Drawing.Size(66, 17);
            this.accuracyLbl.TabIndex = 3;
            this.accuracyLbl.Text = "Accuracy";
            // 
            // accuracyTxtBx
            // 
            this.accuracyTxtBx.Location = new System.Drawing.Point(95, 231);
            this.accuracyTxtBx.Name = "accuracyTxtBx";
            this.accuracyTxtBx.Size = new System.Drawing.Size(100, 22);
            this.accuracyTxtBx.TabIndex = 4;
            // 
            // processBtn
            // 
            this.processBtn.Location = new System.Drawing.Point(253, 255);
            this.processBtn.Name = "processBtn";
            this.processBtn.Size = new System.Drawing.Size(108, 40);
            this.processBtn.TabIndex = 5;
            this.processBtn.Text = "Process";
            this.processBtn.UseVisualStyleBackColor = true;
            this.processBtn.Click += new System.EventHandler(this.processBtn_Click);
            // 
            // isBinaryChckBx
            // 
            this.isBinaryChckBx.AutoSize = true;
            this.isBinaryChckBx.Location = new System.Drawing.Point(26, 81);
            this.isBinaryChckBx.Name = "isBinaryChckBx";
            this.isBinaryChckBx.Size = new System.Drawing.Size(209, 21);
            this.isBinaryChckBx.TabIndex = 6;
            this.isBinaryChckBx.Text = "Is Binary (0 or 1 output only)";
            this.isBinaryChckBx.UseVisualStyleBackColor = true;
            // 
            // inputLbl
            // 
            this.inputLbl.AutoSize = true;
            this.inputLbl.Location = new System.Drawing.Point(23, 41);
            this.inputLbl.Name = "inputLbl";
            this.inputLbl.Size = new System.Drawing.Size(93, 17);
            this.inputLbl.TabIndex = 0;
            this.inputLbl.Text = "Input data file";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.psoRadioButton);
            this.panel1.Controls.Add(this.backPropRadioButton);
            this.panel1.Location = new System.Drawing.Point(12, 121);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(295, 91);
            this.panel1.TabIndex = 7;
            // 
            // backPropRadioButton
            // 
            this.backPropRadioButton.AutoSize = true;
            this.backPropRadioButton.Checked = true;
            this.backPropRadioButton.Location = new System.Drawing.Point(13, 18);
            this.backPropRadioButton.Name = "backPropRadioButton";
            this.backPropRadioButton.Size = new System.Drawing.Size(270, 21);
            this.backPropRadioButton.TabIndex = 0;
            this.backPropRadioButton.TabStop = true;
            this.backPropRadioButton.Text = "Обратное распространение ошибки";
            this.backPropRadioButton.UseVisualStyleBackColor = true;
            // 
            // psoRadioButton
            // 
            this.psoRadioButton.AutoSize = true;
            this.psoRadioButton.Location = new System.Drawing.Point(13, 54);
            this.psoRadioButton.Name = "psoRadioButton";
            this.psoRadioButton.Size = new System.Drawing.Size(104, 21);
            this.psoRadioButton.TabIndex = 1;
            this.psoRadioButton.Text = "Рой частиц";
            this.psoRadioButton.UseVisualStyleBackColor = true;
            // 
            // logLabel
            // 
            this.logLabel.AutoSize = true;
            this.logLabel.Location = new System.Drawing.Point(260, 313);
            this.logLabel.Name = "logLabel";
            this.logLabel.Size = new System.Drawing.Size(90, 17);
            this.logLabel.TabIndex = 8;
            this.logLabel.Text = "Processing...";
            this.logLabel.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 350);
            this.Controls.Add(this.logLabel);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.isBinaryChckBx);
            this.Controls.Add(this.processBtn);
            this.Controls.Add(this.accuracyTxtBx);
            this.Controls.Add(this.accuracyLbl);
            this.Controls.Add(this.BrowseBtn);
            this.Controls.Add(this.inputTxtBx);
            this.Controls.Add(this.inputLbl);
            this.Name = "MainForm";
            this.Text = "Fuzzy logic";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox inputTxtBx;
        private System.Windows.Forms.Button BrowseBtn;
        private System.Windows.Forms.Label accuracyLbl;
        private System.Windows.Forms.TextBox accuracyTxtBx;
        private System.Windows.Forms.Button processBtn;
        private System.Windows.Forms.CheckBox isBinaryChckBx;
        private System.Windows.Forms.Label inputLbl;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton psoRadioButton;
        private System.Windows.Forms.RadioButton backPropRadioButton;
        private System.Windows.Forms.Label logLabel;
    }
}

