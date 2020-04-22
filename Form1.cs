using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using System.Security.AccessControl;

namespace rathena_Conf3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string Conf_ini;
            try
            {
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
                button6.Enabled = false;
                string Conf_path = Application.StartupPath;
                if (File.Exists(Conf_path + @"\conf\battle\extend.conf") == false)
                {
                    Conf_ini = Conf_path + @"\rathena_Conf3.ini";
                }
                else
                {
                    Conf_ini = Conf_path + @"\rathena_cht_Conf3.ini";
                    if (Directory.Exists(Conf_path + @"\conf\import\") == false)
                    {
                        Directory.CreateDirectory(Conf_path + @"\conf\import\");
                    }

                    foreach (string conf_file in Directory.GetFiles(Conf_path + @"\conf\import-tmpl\"))
                    {
                        if(File.Exists(Conf_path + @"\conf\import\" + Path.GetFileName(conf_file)) == false)
                            File.Copy(conf_file, Conf_path + @"\conf\import\" + Path.GetFileName(conf_file),false);
                    }

                    if (Directory.Exists(Conf_path + @"\conf\msg_conf\import\") == false)
                    {
                        Directory.CreateDirectory(Conf_path + @"\conf\msg_conf\import\");
                    }

                    foreach (string conf_file in Directory.GetFiles(Conf_path + @"\conf\msg_conf\import-tmpl\"))
                    {
                        if (File.Exists(Conf_path + @"\conf\msg_conf\import\" + Path.GetFileName(conf_file)) == false)
                            File.Copy(conf_file, Conf_path + @"\conf\msg_conf\import\" + Path.GetFileName(conf_file),false);
                    }
                }

                if (File.Exists(Conf_ini) == false)
                {
                    MessageBox.Show("缺少重要ini檔!!","",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    this.Close();
                }
                treeView1.Nodes.Clear();
                treeView2.Nodes.Clear();
                listBox1.SelectedIndex = -1;
                listBox2.SelectedIndex = -1;
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                IniManager iniManager = new IniManager(Conf_ini);
                foreach (string section in iniManager.ReadSections(Conf_ini))
                {
                    foreach (string key in iniManager.ReadSingleSection(section, Conf_ini))
                    {
                        if (File.Exists(Conf_path + section + key) == false)
                        {
                            this.Close();
                        }
                        string value = iniManager.ReadIniFile(section, key, null);
                        treeView1.Nodes.Add(value, section + key);
                    }
                }
                label1.Text = Conf_path;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            button6.Enabled = false;
            Text = Application.StartupPath;
            button1_Click(null, null);
        }

        private void PrintRecursive(TreeNode treeNode, Color backcolor, Color forecolor)
        {
            treeNode.BackColor = backcolor;
            treeNode.ForeColor = forecolor;
            foreach (TreeNode tn in treeNode.Nodes)
            {
                PrintRecursive(tn, backcolor, forecolor);
            }
        }

        private void CallRecursive(TreeView treeView, Color backcolor, Color forecolor)
        {
            TreeNodeCollection nodes = treeView.Nodes;
            foreach (TreeNode n in nodes)
            {
                PrintRecursive(n, backcolor, forecolor);
            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                treeView2.Nodes.Clear();
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                listBox1.SelectedIndex = -1;
                listBox2.SelectedIndex = -1;
                button2.Enabled = false;
                button3.Enabled = false;
                button4.Enabled = false;
                Text = Application.StartupPath + treeView1.SelectedNode.Text;
                CallRecursive(treeView1, Color.White, Color.Black);
                treeView1.SelectedNode.BackColor = SystemColors.MenuHighlight;
                treeView1.SelectedNode.ForeColor = Color.White;
                
                string[] read_conf_re = File.ReadAllLines(label1.Text + e.Node.Text,Encoding.GetEncoding(e.Node.Name.Split(':')[1]));
                foreach(string conf_line_out in read_conf_re)
                {
                    string conf_line_out_trim = conf_line_out.Trim();
                    listBox1.Items.Add(conf_line_out_trim);
                    if(!conf_line_out_trim.StartsWith("//"))
                    {
                        if(conf_line_out_trim.Length > 0)
                        {
                            treeView2.Nodes.Add(conf_line_out_trim, conf_line_out_trim);
                        }
                    }
                }

                string[] read_txt = File.ReadAllLines(label1.Text + e.Node.Name.Split(':')[0], Encoding.GetEncoding(e.Node.Name.Split(':')[1]));
                label5.Text = label1.Text + e.Node.Name.Split(':')[0];
                foreach (string txt_line_out in read_txt)
                {
                    string txt_line_out_trim = txt_line_out.Trim();
                    listBox2.Items.Add(txt_line_out_trim);
                }
                button4.Enabled = true;
                button5.Enabled = true;
                button6.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void treeView2_AfterSelect(object sender, TreeViewEventArgs e)
        {
            try
            {
                CallRecursive(treeView2, Color.White, Color.Black);
                int select_node1,select_node2;
                if (e.Node.Text.StartsWith("import:"))
                {
                    select_node1 = listBox1.FindString(treeView2.SelectedNode.Text);
                    select_node2 = listBox2.FindString(treeView2.SelectedNode.Text);
                }
                else
                {
                    select_node1 = listBox1.FindString(treeView2.SelectedNode.Text.Split(':')[0] + ":");
                    select_node2 = listBox2.FindString(treeView2.SelectedNode.Text.Split(':')[0] + ":");
                }
                listBox1.SelectedIndex = select_node1;
                listBox2.SelectedIndex = select_node2;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listBox1.SelectedIndex > -1)
            {
                button3.Enabled = true;
                if (listBox1.Focused)
                {
                    if(!listBox1.Text.Split(':')[0].StartsWith("import:"))
                    {
                        if(listBox1.Text != "" & !listBox1.Text.StartsWith("//"))
                        {
                            listBox2.SelectedIndex = listBox2.FindString(listBox1.Text.Split(':')[0] + ":");
                            CallRecursive(treeView2, Color.White, Color.Black);
                            treeView2.SelectedNode = treeView2.Nodes[listBox1.Text.Trim()];
                            treeView2.SelectedNode.BackColor = SystemColors.MenuHighlight;
                            treeView2.SelectedNode.ForeColor = Color.White;
                        }
                        else
                        {
                            CallRecursive(treeView2, Color.White, Color.Black);
                        }
                    }
                    else
                    {
                        if (listBox1.Text != "" & !listBox1.Text.StartsWith("//"))
                        {
                            CallRecursive(treeView2, Color.White, Color.Black);
                            treeView2.SelectedNode = treeView2.Nodes[listBox1.Text.Trim()];
                            treeView2.SelectedNode.BackColor = SystemColors.MenuHighlight;
                            treeView2.SelectedNode.ForeColor = Color.White;
                        }
                        else
                        {
                            CallRecursive(treeView2, Color.White, Color.Black);
                        }
                    }
                }
                else
                {
                    CallRecursive(treeView2, Color.White, Color.Black);
                    treeView2.SelectedNode = treeView2.Nodes[listBox1.Text.Trim()];
                    treeView2.SelectedNode.BackColor = SystemColors.MenuHighlight;
                    treeView2.SelectedNode.ForeColor = Color.White;
                }
            }
            else
            {
                CallRecursive(treeView2, Color.White, Color.Black);
                button3.Enabled = false;
            }
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex > -1)
            {
                button2.Enabled = true;
                if (listBox2.Focused)
                {
                    if (!listBox2.Text.Split(':')[0].StartsWith("import"))
                    {
                        if (listBox2.Text != "" & !listBox2.Text.StartsWith("//"))
                        {
                            int get_index = listBox1.FindString(listBox2.Text.Split(':')[0] + ":");
                            listBox1.SelectedIndex = get_index;
                            if(get_index > -1)
                            {
                                listBox1.SelectedIndex = get_index;
                            }
                            else
                            {
                                string Conf_path = Application.StartupPath;
                                string key = @"\conf\import\battle_conf.txt:utf-8";
                                if (treeView1.SelectedNode.Name.Equals(key))
                                {
                                    treeView2.Nodes.Clear();
                                    listBox1.Items.Clear();
                                    TreeNode[] treeNodes = treeView1.Nodes.Find(key, true);
                                    foreach (TreeNode battle_temp in treeNodes)
                                    {
                                        string[] read_txt = File.ReadAllLines(Conf_path + battle_temp.Text, Encoding.GetEncoding(key.Split(':')[1]));
                                        foreach (string txt_line_out in read_txt)
                                        {
                                            if (!txt_line_out.StartsWith("//") & txt_line_out.Trim() != "")
                                            {
                                                if (txt_line_out.Split(':')[0].Equals(listBox2.Text.Split(':')[0]))
                                                {
                                                    treeView1.SelectedNode = battle_temp;
                                                    listBox2.SelectedIndex = listBox2.FindString(txt_line_out.Split(':')[0] + ":");
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                button2.Enabled = false;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                if(listBox1.SelectedIndex != -1 && listBox2.FindString(listBox1.Text.Split(':')[0] + ":") < 0)
                {
                    if(!listBox1.Text.StartsWith("//") & !listBox1.Text.Trim().Equals(""))
                    {
                        listBox2.Items.Add(listBox1.SelectedItem);
                        listBox2.SelectedIndex = listBox2.FindString(listBox1.Text.Split(':')[0] + ":");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                listBox2.Items.RemoveAt(listBox2.SelectedIndex);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.StackTrace);
            }
        }

        private void listBox2_DoubleClick(object sender, EventArgs e)
        {
            if(listBox2.SelectedIndex > -1 & listBox2.Text.Trim() != "")
            {
                string promat = listBox2.Text.Substring(listBox2.Text.IndexOf(":") + 1).Trim();
                string title = listBox2.Text.Split(':')[0];
                string defaultresponse = promat;
                string temp_conf = Interaction.InputBox(promat, title, defaultresponse);
                if(temp_conf != "")
                {
                    listBox2.Items[listBox2.SelectedIndex] = listBox2.Text.Split(':')[0] + ": " + temp_conf;
                }
            }
        }

        private void listBox1_DoubleClick(object sender, EventArgs e)
        {
            button3_Click(null, null);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string promat = "請輸入import的位置:";
            string title = "import";
            string defaultresponse = "";
            string into_import = Interaction.InputBox(promat, title, defaultresponse);
            if(into_import != "")
            {
                listBox2.Items.Add("import: " + into_import);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            string promat = "格式:\n\n[設定名稱]: [設定值]";
            string title = "請輸入自訂義:";
            string defaultresponse = "[設定名稱]: [設定值]";
            string into_import = Interaction.InputBox(promat, title, defaultresponse);
            if(into_import != "")
            {
                if(into_import != "[設定名稱]: [設定值]")
                {
                    listBox2.Items.Add(into_import);
                }
            }
        }

        public static string GetUTF8String(byte[] buffer)
        {
            if (buffer == null)
                return null;

            if (buffer.Length <= 3)
            {
                return Encoding.UTF8.GetString(buffer);
            }

            byte[] bomBuffer = new byte[] { 0xef, 0xbb, 0xbf };

            if (buffer[0] == bomBuffer[0]
                && buffer[1] == bomBuffer[1]
                && buffer[2] == bomBuffer[2])
            {
                return new UTF8Encoding(false).GetString(buffer, 3, buffer.Length - 3);
            }

            return Encoding.UTF8.GetString(buffer);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                string Conf_path = Application.StartupPath;
                StreamWriter streamWriter = new StreamWriter(Conf_path + treeView1.SelectedNode.Name.Split(':')[0], false, (treeView1.SelectedNode.Name.Split(':')[1]=="utf-8" ?
                        new UTF8Encoding(false):Encoding.GetEncoding(treeView1.SelectedNode.Name.Split(':')[1])));
                foreach (string writeW in listBox2.Items)
                {
                    streamWriter.WriteLine(writeW);
                }
                streamWriter.Close();
                MessageBox.Show("已存檔完畢!!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }

    public class IniManager
    {
        private string filePath;
        private StringBuilder lpReturnedString;
        private int bufferSize;
        
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string lpString, string lpFileName);

        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string lpDefault, StringBuilder lpReturnedString, int nSize, string lpFileName);

        [DllImport("kernel32")]
        private static extern uint GetPrivateProfileString(
            string lpAppName, // points to section name
            string lpKeyName, // points to key name
            string lpDefault, // points to default string
            byte[] lpReturnedString, // points to destination buffer
            uint nSize, // size of destination buffer
            string lpFileName  // points to initialization filename
        );

        public List<string> ReadSections(string iniFilename)
        {
            List<string> result = new List<string>();
            byte[] buf = new byte[65536];
            uint len = GetPrivateProfileString(null, null, null, buf, (uint)buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < len; i++)
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            return result;
        }

        public List<string> ReadSingleSection(string Section, string iniFilename)
        {
            List<string> result = new List<string>();
            byte[] buf = new byte[65536];
            uint lenf = GetPrivateProfileString(Section, null, null, buf, (uint)buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < lenf; i++)
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            return result;
        }
        public IniManager(string iniPath)
        {
            filePath = iniPath;
            bufferSize = 512;
            lpReturnedString = new StringBuilder(bufferSize);
        }

        /*
         引數說明
         1.要寫入哪個Section
         2.該Section下要寫入哪個Key
         3.要寫入的訊息
         
        IniManager iniManager = new IniManager("D:/test.ini");

        iniManager.WriteIniFile("Section_A", "Key_A", "1");
        iniManager.WriteIniFile("Section_B", "Key_B_1", "2");
        iniManager.WriteIniFile("Section_B", "Key_B_2", "3");

        引數說明
        1.要讀取哪個Section
        2.該Section下要讀取哪個Key
        3.如果沒有找到這組Section與Key的話，預設回傳第三個引數

        IniManager iniManager = new IniManager("D:/test.ini");
 
        iniManager.ReadIniFile("Section_A", "Key_A", "default");
        iniManager.ReadIniFile("Section_A", "Key_B", "default");

        刪除Key：將寫入的值設為null即可刪除key
        WriteIniFile(Section_A, key, null)
        刪除整個Section：只要將key設為null即可刪除整個section
        WriteIniFile(Section_A, null, null)，Section_A會整個被刪除
        */

        // read ini date depend on section and key
        public string ReadIniFile(string section, string key, string defaultValue)
        {
            lpReturnedString.Clear();
            GetPrivateProfileString(section, key, defaultValue, lpReturnedString, bufferSize, filePath);
            return lpReturnedString.ToString();
        }

        // write ini data depend on section and key
        public void WriteIniFile(string section, string key, Object value)
        {
            WritePrivateProfileString(section, key, value.ToString(), filePath);
        }
    }
}
