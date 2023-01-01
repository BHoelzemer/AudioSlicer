using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace AudioSlicer
{
    public partial class Form1 : Form
    {
        List<string> mp3Files = new List<string>();
        int interval = 1;
        public Form1()
        {
            InitializeComponent();
            labelInfo.Text = "";
            label4.Text = $"{interval} minute(s)";
            textBox2.Text = Properties.Settings.Default["Destination"]?.ToString() ?? "";
        }

        private void textBox1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                mp3Files.Clear();
                //all directories dropped
                foreach (var item in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    FileAttributes attr = File.GetAttributes(item);

                    if (attr.HasFlag(FileAttributes.Directory))
                    {
                        string[] fileEntries = Directory.GetFiles(item);
                        DirectoryInfo dinfo = new DirectoryInfo(item);
                        if (textBox1.Text == "")
                        {
                            textBox1.Text = dinfo.FullName;
                        }
                        else
                        {
                            textBox1.Text += $"\n{dinfo.FullName}";
                        }

                        var infos = GetFilesByExtensions(dinfo, ".mp3");
                        var selection = infos.Select(x => x.FullName);
                        mp3Files.AddRange(selection);
                    }
                    else
                    {
                        var extension = Path.GetExtension(item);
                        if (extension == ".m4b")
                        {
                            var target = $@"{Path.GetFileNameWithoutExtension(item)}.mp3";
                            M4A2ToMP3Converter.Convert(item, target);
                            mp3Files.Add(target);
                        }
                        else if (extension == ".mp3")
                        {
                            mp3Files.Add(item);
                        }
                        if (textBox1.Text == "")
                        {
                            textBox1.Text = item;
                        }
                        else
                        {
                            textBox1.Text += $"\n{item}";
                        }
                    }



                }
            }
        }

        public IEnumerable<FileInfo> GetFilesByExtensions(DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");
            return dir.EnumerateFiles().Where(f => extensions.Contains(f.Extension));
        }

        private void textBox2_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                //all directories dropped
                foreach (var item in (string[])e.Data.GetData(DataFormats.FileDrop))
                {
                    textBox2.Text = item;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var audio = new AudioSlice();
            audio.OnInfoEvent += (s, info) =>
            {
                labelInfo.InvokeIfRequired(() =>
                {
                    labelInfo.Text = info;
                });

            };


            audio.Start(mp3Files, $"{textBox2.Text}\\{textBox3.Text}", interval, Subfolder.Checked);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            InitDragEnter(e);
        }

        private static void InitDragEnter(DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.All :
                                                         DragDropEffects.None;
        }

        private void textBox1_DragEnter(object sender, DragEventArgs e)
        {
            InitDragEnter(e);
        }

        private void textBox2_DragEnter(object sender, DragEventArgs e)
        {
            InitDragEnter(e);
        }

        private void trackBar1_ValueChanged(object sender, EventArgs e)
        {
            interval = trackBar1.Value;
            label4.Text = $"{interval} m";
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

    }
}
