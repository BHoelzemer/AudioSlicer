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
        public static void Convert(string sourceFileName, string targetFileName)
        {
            
            if(File.Exists(targetFileName))
            {
                File.Delete(targetFileName);
            }
            if(!Directory.GetParent(targetFileName).Exists)
            {
                Directory.GetParent(targetFileName).Create();
            }

            Interaction.Shell($"faad.exe -o \"{targetFileName}\" \"{sourceFileName}\"", 
                AppWinStyle.Hide, true, -1);
            Interaction.Shell($"lame.exe --preset standard \"{targetFileName}\" \"{targetFileName}\"",
                AppWinStyle.Hide, true, -1);

            File.Delete(targetFileName);
        }
    }
}
