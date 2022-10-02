using NAudio.Wave;
using NLayer.NAudioSupport;
using System;
using System.IO;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using System.Linq;

namespace AudioSlicer
{
    class AudioSlice
    {
        List<string> paths;
        string destinationPath;
        int interval = 180; // 3 minutes
        List<byte> overflowBytes;
        int overflowTime;
        int counter;
        int digits;
        //Label infoLabel;
        int maximum;
        int folderDigits;
        bool subfolder;
        static ReaderWriterLock locker = new ReaderWriterLock();

        //ProgressBar progessBar;
        Mp3FileReader.FrameDecompressorBuilder builder = new Mp3FileReader.FrameDecompressorBuilder(wf => new Mp3FrameDecompressor(wf));
        FileStream filestream;

        public event EventHandler<string> OnInfoEvent;
        public event EventHandler OnProgressUpdate;


        public AudioSlice()
        {
            //InitProgessbar(mp3Files.Count, progressBar);
        }

        public void Start(List<string> mp3Files, string destinationFolder, int interval, bool subfolder)
        {
            this.subfolder = subfolder;
            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }
            new Thread(() =>
            {
                try
                {
                    this.interval = interval * 60;
                    mp3Files.Sort(new CustomComparer());
                    destinationPath = destinationFolder;
                    var totalTime = 0;
                    double index = 0;
                    double count = mp3Files.Count;
                    foreach (var mp3File in mp3Files)
                    {
                        totalTime += GetFileTime(mp3File);
                        index++;
                        OnProgressUpdate?.Invoke(this, EventArgs.Empty);
                        var percent = (index / count * 100).ToString("0.##");
                        OnInfoEvent?.Invoke(this, $"Calculating output Count: {percent}%");
                    }
                    maximum = totalTime / (this.interval);
                    digits = maximum.Digits();
                    folderDigits = (maximum / 100).Digits();
                    //InitProgessbar(maximum, progressBar);
                    locker.AcquireWriterLock(int.MaxValue);
                    foreach (var mp3File in mp3Files)
                    {
                        Slice(mp3File);
                    }
                    GetLastPart(filestream);

                    OnInfoEvent.Invoke(this, "Finished");
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    filestream?.Close();
                    filestream?.Dispose();
                }
            }).Start();
        }

        private int GetFileTime(string mp3File)
        {
            int result;
            using (var reader = new Mp3FileReader(mp3File, builder))
            {
                result = (int)reader.TotalTime.TotalSeconds;
            }
            return result;
        }

        //private void InitProgessbar(int maximum, ProgressBar progessBar)
        //{
        //    progessBar.InvokeIfRequired(() =>
        //    {
        //        this.progessBar = progessBar;
        //        this.progessBar.Visible = true;
        //        this.progessBar.Minimum = 1;
        //        this.progessBar.Maximum = maximum;
        //        this.progessBar.Value = 1;
        //        this.progessBar.Step = 1;
        //    });

        //}

        void Slice(string path)
        {
            try
            {
                using (var reader = new Mp3FileReader(path, builder))
                {
                    Mp3Frame frame;

                    // rest secondes from the last file
                    var restOffset = GetLastPart(filestream);
                    if (restOffset == 0)
                    {
                        CreateWriter();
                    }
                    var timeOfLastCut = 0;
                    var totalTime = reader.TotalTime.TotalSeconds + restOffset;
                    while ((frame = reader.ReadNextFrame()) != null)
                    {
                        //the rest of the file
                        if (overflowTime > 0)
                        {
                            overflowBytes.AddRange(frame.RawData);
                        }
                        else
                        {
                            // not enough time left for a cut
                            if ((totalTime - interval) < timeOfLastCut)
                            {
                                overflowTime = (int)(totalTime - timeOfLastCut);
                                overflowBytes = new List<byte>();
                                overflowBytes.AddRange(frame.RawData);
                            }
                            else
                            {
                                if (((int)reader.CurrentTime.TotalSeconds + restOffset - timeOfLastCut) >= interval)
                                {

                                    CreateWriter();
                                    timeOfLastCut = (int)reader.CurrentTime.TotalSeconds + restOffset;
                                    //one File Finished
                                    //progessBar.InvokeIfRequired(() => { progessBar.PerformStep(); });
                                    var percent = ((double)timeOfLastCut / totalTime * 100).ToString("0.##");
                                    OnInfoEvent?.Invoke(this, $"Cutting: {percent}%");
                                }

                                filestream.Write(frame.RawData, 0, frame.RawData.Length);
                            }

                        }

                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void CreateWriter()
        {
            filestream?.Close();
            filestream?.Dispose();
            var name = $"{(++counter).ToString($"D{digits}")}.mp3";
            OnInfoEvent.Invoke(this, name);
            var path = destinationPath;
            if (subfolder)
            {
                var dir =
                path += $@"\{(counter / 100).ToString($"D{folderDigits}")}";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            filestream = File.Open(Path.Combine(path, name), FileMode.OpenOrCreate);
        }

        private int GetLastPart(FileStream writer)
        {
            var result = overflowTime;
            if (overflowTime > 0)
            {
                var data = overflowBytes.ToArray();
                writer.Write(data, 0, data.Length);
                overflowTime = 0;
            }
            return result;
        }
    }

    public class CustomComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            try
            {
                // run the regex on both strings
                var xVAlue = new string(x.Split('\\').LastOrDefault()?.TakeWhile(Char.IsDigit).ToArray());
                var yValue = new string(y.Split('\\').LastOrDefault()?.TakeWhile(Char.IsDigit).ToArray());

                // check if they are both numbers
                if (xVAlue != "" && yValue != "")
                {
                    return int.Parse(xVAlue).CompareTo(int.Parse(yValue));
                }
            }
            catch (Exception) { }


            // otherwise return as string comparison
            return x.CompareTo(y);
        }
    }
}
