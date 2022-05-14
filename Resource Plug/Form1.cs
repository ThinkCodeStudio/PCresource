using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenHardwareMonitor.Hardware;
using System.IO.Ports;

namespace Resource_Plug {
    public partial class mainWindow : Form {

        private SerialPort _port;
        private PCsource _PCsource;

        public mainWindow() {
            InitializeComponent();
        }

        //窗口关闭
        private void mainWindow_FormClosing(object sender, FormClosingEventArgs e) {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
            this.mainNotifyIcon.Visible = true;
            this.Hide();
        }
        //窗口加载
        private void mainWindow_Load(object sender, EventArgs e) {
            this.WindowState = FormWindowState.Minimized;
            this.mainNotifyIcon.Visible = true;
            this.Hide();
            timer1.Stop();
            this._port = new SerialPort();
            //检查串口是否存在
            if (!SerialPort.GetPortNames().Contains( ShowComToolStripMenuItem1.Text )) {
                ShowComToolStripMenuItem1.Text = "NULL";
                ConnectToolStripMenuItem.Enabled = false;
            }
            _PCsource = new PCsource();
        }

        //托盘右键关闭,退出程序
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e) {
            this.mainNotifyIcon.Visible = false;
            this.Close();
            this.Dispose();
            System.Environment.Exit( System.Environment.ExitCode );
        }
        //托盘右键设置
        private void SettingToolStripMenuItem_Click(object sender, EventArgs e) {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }
        //托盘右键连接
        private void ConnectToolStripMenuItem_Click(object sender, EventArgs e) {

            if (!this._port.IsOpen) {
                this._port.PortName = ShowComToolStripMenuItem1.Text;
                this._port.Open();
                timer1.Start();
                ConnectToolStripMenuItem.Text = "断开";
            }
            else {
                timer1.Stop();
                this._port.Close();
                this._port.Dispose();
                ConnectToolStripMenuItem.Text = "连接";
            }
        }

        private void addDataRowToGridView(ISensor[] sensor) {
            foreach (ISensor sensorItem in sensor) {
                DataGridViewRow row = new DataGridViewRow();
                DataGridViewTextBoxCell callName = new DataGridViewTextBoxCell();
                callName.Value = sensorItem.Name;
                row.Cells.Add( callName );

                DataGridViewTextBoxCell cellValue = new DataGridViewTextBoxCell();
                cellValue.Value = sensorItem.Value;
                row.Cells.Add( cellValue );
                dataGridView1.Rows.Add( row );
            }
        }
        //定时器
        private void timer1_Tick(object sender, EventArgs e) {
            _PCsource.Updata();
            if (this.WindowState == FormWindowState.Normal) {
                dataGridView1.Rows.Clear();
                addDataRowToGridView( _PCsource.CPULoad );
                addDataRowToGridView( _PCsource.CPUTemperature );
                addDataRowToGridView( _PCsource.CPUPower );
                addDataRowToGridView( _PCsource.RAMLoad );
                addDataRowToGridView( _PCsource.RAMData );
                addDataRowToGridView( _PCsource.GPULoad );
                addDataRowToGridView( _PCsource.GPUTemperature );
                addDataRowToGridView( _PCsource.GPUPower );
                addDataRowToGridView( _PCsource.GPUData );
            }
        }


    }
}
