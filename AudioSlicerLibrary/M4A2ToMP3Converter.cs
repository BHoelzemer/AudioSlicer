using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualBasic;
using System.IO;

namespace AudioSlicer
{
    public static class M4A2ToMP3Converter
    {
        public static void Convert(string fromPath, string toPath)
        {
            int lastIndex = fromPath.Contains("\\") ? fromPath.LastIndexOf('\\') : 0;
            string tempFileName = Environment.CurrentDirectory + "\\temp\\" + fromPath.Substring(lastIndex);
            
            if(File.Exists(tempFileName))
            {
                File.Delete(tempFileName);
            }
            if(!Directory.GetParent(tempFileName).Exists)
            {
                Directory.GetParent(tempFileName).Create();
            }

            Interaction.Shell($"faad.exe -o \"{tempFileName}\" \"{fromPath}\"", 
                AppWinStyle.Hide, true, -1);
            Interaction.Shell($"lame.exe --preset standard \"{tempFileName}\" \"{toPath}\"",
                AppWinStyle.Hide, true, -1);

            File.Delete(tempFileName);
        }
    }
}
