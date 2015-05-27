using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FuzzyLogic_Mozerov
{
    public partial class MainForm : Form
    {
        DataSetClass dataClass = new DataSetClass();
        BackgroundWorker worker = new BackgroundWorker();
        
        public MainForm()
        {
            InitializeComponent();
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);           
        }

        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            logLabel.Visible = false;
            processBtn.Enabled = true;
        }

        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            MethodType type;
            if (backPropRadioButton.Checked)
            {
                type = MethodType.BackPropagation;
            }
            else
            {
                type = MethodType.Pso;
            }
            if (dataClass.CreateFromFile(inputTxtBx.Text, isBinaryChckBx.Checked))
            {
                double accuracy = dataClass.Process(type);
                this.Invoke(new MethodInvoker(delegate()
                {
                    accuracyTxtBx.Text = string.Format("{0}%", Math.Round(accuracy, 3).ToString());
                }));
            }
            else
            {
                this.Invoke(new MethodInvoker(delegate()
                {
                    MessageBox.Show("File does not exist!");
                }));
            }           
        }

        private void processBtn_Click(object sender, EventArgs e)
        {
            logLabel.Visible = true;
            processBtn.Enabled = false;
          
            worker.RunWorkerAsync();         
        }

        private void BrowseBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Data Files|*.data";
            openFileDialog.Title = "Select data file";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                inputTxtBx.Text = openFileDialog.FileName;
            }
        }
    }
}
