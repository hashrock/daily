﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Daily
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            WindowState = FormWindowState.Minimized;
            ReadTasks();
        }

        /// <summary>
        /// タスク一覧をテキストファイルから読み込む
        /// </summary>
        private void ReadTasks()
        {
            using (StreamReader sr = new StreamReader(Properties.Settings.Default.Tasks))
            {
                string lines = sr.ReadToEnd();
                string nl = Environment.NewLine;
                string[] liness = lines.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                foreach (string line in liness)
                {
                    string item = line.Trim();
                    if (item.Length > 0)
                    {
                        checkedListBoxTasks.Items.Add(item);
                    }
                }
            }
        }

        private void checkedListBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int idx = checkedListBoxTasks.SelectedIndex;
            if (idx < 0)
            {
                return;
            }
            CheckItem(idx);
            
        }

        /// <summary>
        /// アイテムをチェックし、ログに出力する
        /// </summary>
        /// <param name="idx"></param>
        private void CheckItem(int idx)
        {
            string item = (string)checkedListBoxTasks.Items[idx];

            string output = DateTime.Now + "," + item + "," + checkedListBoxTasks.GetItemChecked(idx);
            string path = Properties.Settings.Default.LogFile;

            if (!File.Exists(path))
            {
                File.CreateText(path);
            }

            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(output);
                Console.WriteLine(output);
            }
        }

        private void 終了ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            WindowState = FormWindowState.Normal;
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
           DialogResult result = MessageBox.Show("終了しますか？",
            "Dailyの終了",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Exclamation,
            MessageBoxDefaultButton.Button2);

           if (result == DialogResult.Yes)
           {
               Properties.Settings.Default.Save();
               notifyIcon1.Visible = false;
           }
           else if (result == DialogResult.No)
           {
               e.Cancel = true; 
               WindowState = FormWindowState.Minimized;
           }
           else if (result == DialogResult.Cancel)
           {
               e.Cancel = true;
           }

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckTaskNotification();
        }

        /// <summary>
        /// タスクの実行時刻が来たら通知する
        /// </summary>
        private void CheckTaskNotification()
        {
            foreach (object item in checkedListBoxTasks.Items)
            {
                var label = (string)item;
                var timeStr = label.Split(' ')[0];
                List<string> ballon = new List<string>();
                if (timeStr == DateTime.Now.ToShortTimeString())
                {
                    ballon.Add(label);
                }

                if (ballon.Count > 0)
                {
                    notifyIcon1.BalloonTipTitle = "お知らせ";
                    notifyIcon1.BalloonTipText = string.Join(Environment.NewLine, ballon);
                    notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                    notifyIcon1.ShowBalloonTip(10000);
                }
            }
        }
    }
}
