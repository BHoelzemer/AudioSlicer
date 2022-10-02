using SharpShell.Attributes;
using SharpShell.SharpContextMenu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AudioSlicer
{
    [ComVisible(true)]
    [COMServerAssociation(AssociationType.Directory)]
    class ShellExtention : SharpContextMenu
    {
        protected override bool CanShowMenu()
        {
            return true;
        }

        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();
            var sliceAudioBook = new ToolStripMenuItem
            {
                Text = "Slice Audio Book"
            };
            sliceAudioBook.Click += (sender, args) => SliceAudioBook();

            //  Add the item to the context menu
            menu.Items.Add(sliceAudioBook);

            //  Return the menu
            return menu;
        }

        private void SliceAudioBook()
        {
            List<string> mp3Files = null;
            var audio = new AudioSlice();
            foreach (var item in SelectedItemPaths)
            {
                string[] fileEntries = Directory.GetFiles(item);
                DirectoryInfo dinfo = new DirectoryInfo(item);


                var infos = GetFilesByExtensions(dinfo, ".mp3");
                var selection = infos.Select(x => x.FullName);
                
                mp3Files.AddRange(selection);

            }
            string destinyFolder = Properties.Settings.Default["Destination"]?.ToString() ?? "";
            if (ShowInputDialog(ref destinyFolder) == DialogResult.OK)
            {
                audio.Start(mp3Files, destinyFolder, 1,true);
            }
            
        }

        public IEnumerable<FileInfo> GetFilesByExtensions(DirectoryInfo dir, params string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");
            IEnumerable<FileInfo> files = dir.EnumerateFiles();

            return files.Where(f => extensions.Contains(f.Extension));
        }

        private static DialogResult ShowInputDialog(ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(200, 70);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = "Name";

            TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }
    }
}
