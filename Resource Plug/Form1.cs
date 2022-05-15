using OpenHardwareMonitor.Hardware;
using Resource_Plug.PCsource;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Resource_Plug {
    public partial class mainWindow : Form {

        private SerialPort _port;
        private HardwareMonitor _hardwareMonitor;

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
            if (!SerialPort.GetPortNames().Contains(ShowComToolStripMenuItem1.Text)) {
                ShowComToolStripMenuItem1.Text = "NULL";
                ConnectToolStripMenuItem.Enabled = false;
            }
            _hardwareMonitor = new HardwareMonitor();
        }

        //托盘右键关闭,退出程序
        private void CloseToolStripMenuItem_Click(object sender, EventArgs e) {
            this.mainNotifyIcon.Visible = false;
            this.Close();
            this.Dispose();
            System.Environment.Exit(System.Environment.ExitCode);
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

        private void addDataRowToGridView(object name, object value) {
            DataGridViewRow row = new DataGridViewRow();
            DataGridViewTextBoxCell callName = new DataGridViewTextBoxCell();
            callName.Value = name;
            row.Cells.Add(callName);

            DataGridViewTextBoxCell cellValue = new DataGridViewTextBoxCell();
            cellValue.Value = value;
            row.Cells.Add(cellValue);
            dataGridView1.Rows.Add(row);
        }

        private void addSensorToGridView(ISensor[] sensor) {
            foreach (ISensor sensorItem in sensor) {
                addDataRowToGridView(sensorItem.SensorType.ToString() + ":" + sensorItem.Name, sensorItem.Value);
            }
        }
        //定时器
        private void timer1_Tick(object sender, EventArgs e) {
            _hardwareMonitor.Updata();
            List<byte> buff = new List<byte>();
            buff.Add(Convert.ToByte(HardwareMonitor.GetBtyeFromSensor(_hardwareMonitor.CPULoad, "CPU Total").Value));
            buff.Add(Convert.ToByte(HardwareMonitor.GetBtyeFromSensor(_hardwareMonitor.CPUTemperature, "CPU Package").Value));
            buff.Add(Convert.ToByte(HardwareMonitor.GetBtyeFromSensor(_hardwareMonitor.CPUPower, "CPU Package").Value));

            buff.Add(Convert.ToByte(HardwareMonitor.GetBtyeFromSensor(_hardwareMonitor.RAMLoad, "Memory").Value));
            buff.Add(Convert.ToByte(HardwareMonitor.GetBtyeFromSensor(_hardwareMonitor.RAMData, "Used Memory").Value));

            buff.Add(Convert.ToByte(HardwareMonitor.GetBtyeFromSensor(_hardwareMonitor.GPULoad, "GPU Core").Value));
            buff.Add(Convert.ToByte(HardwareMonitor.GetBtyeFromSensor(_hardwareMonitor.GPUTemperature, "GPU Core").Value));
            buff.Add(Convert.ToByte(HardwareMonitor.GetBtyeFromSensor(_hardwareMonitor.GPUPower, "GPU Power").Value));

            byte[] modbusBuff = Modbus.GetBytes(buff.ToArray());
            _port.Write(modbusBuff, 0, modbusBuff.Length);

            if (this.WindowState == FormWindowState.Normal) {
                dataGridView1.Rows.Clear();
                StringBuilder sb = new StringBuilder();
                foreach (var b in modbusBuff) {
                    sb.Append(b.ToString("X2")+" ");
                }
                addDataRowToGridView("Modbus data", sb.ToString());

                addSensorToGridView(_hardwareMonitor.CPULoad);
                addSensorToGridView(_hardwareMonitor.CPUTemperature);
                addSensorToGridView(_hardwareMonitor.CPUPower);

                addSensorToGridView(_hardwareMonitor.RAMLoad);
                addSensorToGridView(_hardwareMonitor.RAMData);

                addSensorToGridView(_hardwareMonitor.GPULoad);
                addSensorToGridView(_hardwareMonitor.GPUTemperature);
                addSensorToGridView(_hardwareMonitor.GPUPower);
                addSensorToGridView(_hardwareMonitor.GPUData);
            }
        }


    }
}
