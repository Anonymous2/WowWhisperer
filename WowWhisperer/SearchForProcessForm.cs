using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WowWhisperer
{
    public partial class SearchForProcessForm : Form
    {
        private Dictionary<int /* selectedIndex */, int /* process id */> processes = new Dictionary<int, int>();

        public SearchForProcessForm()
        {
            InitializeComponent();
        }

        private void SearchForProcess_Load(object sender, EventArgs e)
        {
            Process[] _processes = Process.GetProcesses();
            _processes.OrderBy(_process => _process.ProcessName);

            for (int i = 0; i < _processes.Length; ++i)
            {
                listBoxProcesses.Items.Add(_processes[i].ProcessName);
                processes.Add(i, _processes[i].Id);
            }
        }

        private void buttonContinue_Click(object sender, EventArgs e)
        {
            if (listBoxProcesses.SelectedIndex == -1)
            {
                MessageBox.Show("No process selected!", "Select a process", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            DialogResult = DialogResult.OK;
            ((MainForm)Owner).process = Process.GetProcessById(processes[listBoxProcesses.SelectedIndex]);
            Close();
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void listBoxProcesses_DoubleClick(object sender, EventArgs e)
        {
            buttonContinue.PerformClick();
        }

        private void SearchForProcessForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    buttonContinue.PerformClick();
                    break;
                case Keys.Escape:
                    Close();
                    break;
            }
        }
    }
}
