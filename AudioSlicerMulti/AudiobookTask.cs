using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AudioSlicerMulti
{
    public class AudiobookTask
    {
        public string SourceFilename { get; set; }
        public string TargetPath { get; set; }
        public string TargetName { get; set; }

        public AudiobookTask(global::System.String sourceFilename, global::System.String targetPath, global::System.String targetName)
        {
            SourceFilename = sourceFilename;
            TargetPath = targetPath;
            TargetName = targetName;
        }
    }
}
