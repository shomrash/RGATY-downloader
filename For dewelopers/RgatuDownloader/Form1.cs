using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RgatuDownloader
{
   
    public partial class Form1 : Form
    {
        List<FileSite> stash;
        public Form1()
        {
            InitializeComponent();
            stash = new List<FileSite>();
            button3.Enabled = false;
            label1.Visible = false;
            label2.Visible = false;

            Navigate("http://ino-rgatu.ru/course/index.php?categoryid=1906");
            
        }

        private void Navigate(String address)
        {
            if (String.IsNullOrEmpty(address)) return;
            if (address.Equals("about:blank")) return;
            if (!address.StartsWith("http://") &&
                !address.StartsWith("https://"))
            {
                address = "http://" + address;
            }
            try
            {
                webBrowser1.Navigate(new Uri(address));
            }
            catch (System.UriFormatException)
            {
                return;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Navigate("http://ino-rgatu.ru/course/index.php?categoryid=1906");
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string dialogPath;
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                dialogPath = dialog.SelectedPath;
            }
            
            
            foreach (var item in stash)
            {
                if (item.name.Length > 254)
                {
                    var spltmp = item.name.Split('/');
                    item.name = spltmp[0]+ "/" + spltmp[spltmp.Length - 2]+ "/" + spltmp[spltmp.Length-1];
                }

                string tmpstr = item.name.Remove(item.name.IndexOf(Path.GetFileName(item.name))); 


                try
                {
                    bool exists = System.IO.Directory.Exists(tmpstr);

                    if (!exists && tmpstr != "")
                        System.IO.Directory.CreateDirectory(tmpstr);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);                    
                }
                
            }

            DownloadManyFiles(stash);
        }

        private void DownComp(object sender, AsyncCompletedEventArgs e)
        {
            Name += "1";
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string line = webBrowser1.Document.GetElementById("folder_tree0").InnerHtml;

            stash.Clear();
            var tmp = line;
            
            while (line.Contains("http://ino-rgatu.ru/pluginfile.php"))
            {
                tmp = line.Remove(0, line.IndexOf("http://ino-rgatu.ru/pluginfile.php"));
                tmp = tmp.Remove(tmp.IndexOf("forcedownload=1") + 15, tmp.Length - tmp.IndexOf("forcedownload=1") - 15);
                line = line.Remove(0, line.IndexOf("forcedownload=1") + 15);

                stash.Add(new FileSite(tmp));
            }

            listBox1.Items.Clear();
            foreach (var item in stash)
            {
                if (checkBox1.Checked)
                    listBox1.Items.Add("  "+ item.name + "         URL  =  " + item.url.ToString());
                else
                    listBox1.Items.Add("  "+ item.name);
                // Console.Write(item.name);
                //Console.WriteLine("    URL = " + item.url.ToString());
            }

            button3.Enabled = true;
        }

        public async Task DownloadManyFiles(List<FileSite> files)
        {
            MessageBox.Show("Начинается процесс сохранения " + stash.Count + " файлов. Данная операция может занять продолжительное время. Не закрывайте программу");
            button1.Enabled = false;
            button3.Enabled = false;
            checkBox1.Enabled = false;
            var client = new WebClient();
            var cookies = webBrowser1.Document.Cookie;
            client.Headers.Add(HttpRequestHeader.Cookie, cookies);
            client.DownloadProgressChanged += (s, e) => progressBar1.Value = e.ProgressPercentage;
            int index = 0;
            progressBar2.Maximum = files.Count;
            label1.Visible = true;
            label2.Visible = true;
            foreach (var item in files) 
            {
                label2.Text = index + "/" + files.Count;
                progressBar2.Value = index;
                await client.DownloadFileTaskAsync(item.url, item.name);
                if (checkBox1.Checked)
                    listBox1.Items.Insert(index, "✓ " + item.name + "         URL  =  " + item.url.ToString());
                else
                    listBox1.Items.Insert(index, "✓ " + item.name);
                listBox1.Items.RemoveAt(index + 1);
                index++;
            }
            client.Dispose();
            MessageBox.Show("Is done!");
            button1.Enabled = true;
            button3.Enabled = true;
            checkBox1.Enabled = true;
            label1.Visible = false;
            label2.Visible = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                stash.RemoveAt(listBox1.SelectedIndex);
                listBox1.Items.RemoveAt(listBox1.SelectedIndex);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < stash.Count;)
            {
                if (stash[i].name.ToLower().Contains("jpg") || stash[i].name.ToLower().Contains("jpeg"))
                {
                    stash.RemoveAt(i);
                    listBox1.Items.RemoveAt(i);
                }
                else i++;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBox1.Items.Count;)
            {
                if (listBox1.Items.Contains("✓"))
                {
                    stash.RemoveAt(i);
                    listBox1.Items.RemoveAt(i);
                }
                else i++;
            }
        }
    }
}
