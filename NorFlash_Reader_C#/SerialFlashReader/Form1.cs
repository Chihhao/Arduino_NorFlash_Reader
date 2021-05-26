using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.IO;
using System.Collections;
using System.Management;
using System.Diagnostics;
using System.Threading;

namespace SerialFlashReader{
    public partial class Form1 : Form    {
        private SerialPort port = new SerialPort();
        private const int SIGN_START = 150;
        private const int SIGN_END = 151;

        private const string CMD_READ_ALL     = "60";
        private const string CMD_CLEAR_ALL    = "61";
        private const string CONFIG_4MB_16B   = "101";
        private const string CONFIG_4MB_32B   = "102";
        private const string CONFIG_4MB_64B   = "103";
        private const string CONFIG_16MB_16B  = "104";
        private const string CONFIG_16MB_32B  = "105";
        private const string CONFIG_16MB_64B  = "106";
        private const string CONFIG_32MB_16B  = "107";
        private const string CONFIG_32MB_32B  = "108";
        private const string CONFIG_32MB_64B  = "109";
        private const string CONFIG_64MB_16B  = "110";
        private const string CONFIG_64MB_32B  = "111";
        private const string CONFIG_64MB_64B  = "112";
        private const string CONFIG_128MB_16B = "113";
        private const string CONFIG_128MB_32B = "114";
        private const string CONFIG_128MB_64B = "115";

        public Form1(){
            InitializeComponent();
            Init();

            comboBox_size.Text = "128MB";
            comboBox_str_len.Text = "32";
        }

        private void Init(){
            port.Parity = Parity.None;
            port.StopBits = StopBits.One;
            port.DataBits = 8;
            port.Handshake = Handshake.None;
            port.RtsEnable = true;
            port.BaudRate = 115200;

            string[] ports = SerialPort.GetPortNames();
            foreach (string p in ports){
                comboBox_port.Items.Add(p);
            }
 
            comboBox_size.Items.Add("4MB");
            comboBox_size.Items.Add("16MB");
            comboBox_size.Items.Add("32MB");
            comboBox_size.Items.Add("64MB");
            comboBox_size.Items.Add("128MB");

            comboBox_str_len.Items.Add("16");
            comboBox_str_len.Items.Add("32");
            comboBox_str_len.Items.Add("64");
                        
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            textBox_path.Text = desktopPath + "\\SerialLog-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";
                        
            label_status.Text = "";           
        }

        private void Form1_Load(object sender, EventArgs e){
            button_clear_all.Enabled = false;
            button_start.Enabled = false;
        }

        private void button_path_Click(object sender, EventArgs e){
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.Description = "選擇儲存位置";

            if (fbd.ShowDialog() == DialogResult.OK){
                textBox_path.Text = fbd.SelectedPath + "\\SerialLog-" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";
            }
        }

        private void button_exit_Click(object sender, EventArgs e){
            this.Close();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e){
            if (port.IsOpen) port.Close();   
            if (MessageBox.Show("確認關閉？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true;
            }
        }

        private void button_clear_all_Click(object sender, EventArgs e){
            if (!port.IsOpen){
                MessageBox.Show("尚未連線");
                return;
            }
            if (MessageBox.Show("確認清除所有資料？\n(確認後，紅色LED會亮起，資料清除完畢後，\n紅色LED將會熄滅，整個過程可能需要數分鐘。)", 
                "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No){
                return;
            }

            label_status.Text = "清除中";
            Application.DoEvents();
            port.Write(CMD_CLEAR_ALL);
            string sRec = DoReceive(300);

            if (sRec == "CleanOK") {
                label_status.Text = "清除完畢";        
            }
            else {
                label_status.Text = "清除失敗";
            }
        }

        private string configString() {
            if (comboBox_size.Text == "4MB" && comboBox_str_len.Text == "16"){
                toolStripProgressBar1.Maximum = 4 * 1024 * 1024; 
                return CONFIG_4MB_16B;
            }
            else if (comboBox_size.Text == "4MB" && comboBox_str_len.Text == "32") {
                toolStripProgressBar1.Maximum = 4 * 1024 * 1024;
                return CONFIG_4MB_32B;
            }
            else if (comboBox_size.Text == "4MB" && comboBox_str_len.Text == "64") {
                toolStripProgressBar1.Maximum = 4 * 1024 * 1024;
                return CONFIG_4MB_64B;
            }
            else if (comboBox_size.Text == "16MB" && comboBox_str_len.Text == "16") {
                toolStripProgressBar1.Maximum = 16 * 1024 * 1024;
                return CONFIG_16MB_16B;
            }
            else if (comboBox_size.Text == "16MB" && comboBox_str_len.Text == "32") {
                toolStripProgressBar1.Maximum = 16 * 1024 * 1024;
                return CONFIG_16MB_32B;
            }
            else if (comboBox_size.Text == "16MB" && comboBox_str_len.Text == "64") {
                toolStripProgressBar1.Maximum = 16 * 1024 * 1024;
                return CONFIG_16MB_64B;
            }
            else if (comboBox_size.Text == "32MB" && comboBox_str_len.Text == "16") {
                toolStripProgressBar1.Maximum = 32 * 1024 * 1024;
                return CONFIG_32MB_16B;
            }
            else if (comboBox_size.Text == "32MB" && comboBox_str_len.Text == "32") {
                toolStripProgressBar1.Maximum = 32 * 1024 * 1024;
                return CONFIG_32MB_32B;
            }
            else if (comboBox_size.Text == "32MB" && comboBox_str_len.Text == "64") {
                toolStripProgressBar1.Maximum = 32 * 1024 * 1024;
                return CONFIG_32MB_64B;
            }
            else if (comboBox_size.Text == "64MB" && comboBox_str_len.Text == "16") {
                toolStripProgressBar1.Maximum = 64 * 1024 * 1024;
                return CONFIG_64MB_16B;
            }
            else if (comboBox_size.Text == "64MB" && comboBox_str_len.Text == "32") {
                toolStripProgressBar1.Maximum = 64 * 1024 * 1024;
                return CONFIG_64MB_32B;
            }
            else if (comboBox_size.Text == "64MB" && comboBox_str_len.Text == "64") {
                toolStripProgressBar1.Maximum = 64 * 1024 * 1024;
                return CONFIG_64MB_64B;
            }
            else if (comboBox_size.Text == "128MB" && comboBox_str_len.Text == "16") {
                toolStripProgressBar1.Maximum = 128 * 1024 * 1024;
                return CONFIG_128MB_16B;
            }
            else if (comboBox_size.Text == "128MB" && comboBox_str_len.Text == "32") {
                toolStripProgressBar1.Maximum = 128 * 1024 * 1024;
                return CONFIG_128MB_32B;
            }
            else if (comboBox_size.Text == "128MB" && comboBox_str_len.Text == "64") {
                toolStripProgressBar1.Maximum = 128 * 1024 * 1024;
                return CONFIG_128MB_64B;
            }

            return "0";
        }

        private void button_connect_Click(object sender, EventArgs e){
            if (comboBox_port.Text == ""){
                MessageBox.Show("尚未選擇Com Port");
                return;
            }

            string sSelectComport = comboBox_port.Text;
            label_status.Text = "連線中";
            Application.DoEvents();

            try {
                if (!port.IsOpen) {
                    port.PortName = sSelectComport;
                    port.Open();
                    port.DiscardInBuffer();
                    port.DiscardOutBuffer();
                    
                    port.Write(configString());
                    string sRec = DoReceive(15);

                    if (sRec == "SettingOK") {
                        label_status.Text = "已連線";
                        button_connect.Enabled = false;
                        comboBox_port.Enabled = false;
                        comboBox_size.Enabled = false;
                        comboBox_str_len.Enabled = false;
                        button_clear_all.Enabled = true;
                        button_start.Enabled = true;                        
                    }
                    else {
                        MessageBox.Show(sRec);
                        label_status.Text = "連線失敗";
                        if (port.IsOpen) port.Close();
                    }
                }
            }
            catch(Exception ex)  {
                label_status.Text = "連線失敗";
                if (port.IsOpen) port.Close();
                MessageBox.Show(ex.Message);
            }
        }

        private string DoReceive(int timeout){
            List<Byte> tempList = new List<Byte>();
            timeout = timeout * 1000;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (true ){

                if (sw.ElapsedMilliseconds > timeout) throw new TimeoutException();
                try {
                    Int32 receivedValue = 0;
                    if (port.BytesToRead > 0) {
                        receivedValue = port.ReadByte();
                        if (receivedValue == SIGN_START) {
                            tempList.Clear();
                            tempList.Add((Byte)receivedValue);
                        }
                        else if (receivedValue == SIGN_END) {
                            tempList.Add((Byte)receivedValue);
                            parse(tempList);
                            break;
                        }
                        else {
                            tempList.Add((Byte)receivedValue);
                        }
                    }                    

                }
                catch { }
                
            }
            return Encoding.ASCII.GetString(tempList.ToArray());
        }

        private void parse(List<Byte> tempList){
            if (tempList[0] == (Byte)SIGN_START && tempList[tempList.Count - 1] == (Byte)SIGN_END){
                tempList.RemoveAt(0);
                tempList.RemoveAt(tempList.Count - 1);
            }
        }

        private void button_start_Click(object sender, EventArgs e) {
            if (!port.IsOpen) {
                MessageBox.Show("尚未連線");
                return;
            }
            if (MessageBox.Show("即將開始讀取資料？\n(確認後，綠色LED會亮起，讀取完畢後，\n綠色LED將會熄滅，整個過程可能需要數分鐘。)",
                "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No) {
                return;
            }

            label_status.Text = "讀取中";
            Application.DoEvents();
            port.Write(CMD_READ_ALL);

            bool bDisconnect = false;
            const int TEMPLEN = 10240;
            char[] tmpRec = new char[TEMPLEN];
            int i=0;
            toolStripProgressBar1.Value = 0;

            using (StreamWriter writer = new StreamWriter(textBox_path.Text)) {
                while (true) {
                    try {
                        int receivedValue = port.ReadByte();
                         tmpRec[i++] = (char)receivedValue;
                         if (i >= TEMPLEN) {
                            writer.Write(tmpRec);
                            Array.Clear(tmpRec, 0, tmpRec.Length);
                            i = 0;
                            if (toolStripProgressBar1.Value + TEMPLEN <= toolStripProgressBar1.Maximum)
                                toolStripProgressBar1.Value += TEMPLEN;                            
                        }
                        if (receivedValue == SIGN_END) {
                            Array.Resize(ref tmpRec, i);
                            writer.Write(tmpRec);
                            break;
                        }
                        Application.DoEvents();
                        
                        //writer.Write ((char)receivedValue);
                    }
                    catch {
                        MessageBox.Show("已斷線");
                        bDisconnect = true;
                        break;
                    }
                    
                }
            }
            if (bDisconnect) {
                label_status.Text = "中斷";
            }
            else {
                label_status.Text = "讀取完畢";
                toolStripProgressBar1.Value = 0;
            }
        }

    }
}
