using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using GaleryParseContent;
using Helpers;
using FileWriter;
using System.ComponentModel;

namespace ExtractGalery
{
    class Galery
    {
        private readonly ParseFile Parser = new ParseFile();
        private readonly Model Model = new Model();

        private readonly Dictionary<string, List<string>> galery = new Dictionary<string, List<string>>();
        private readonly List<string> extensions = new List<string>();

        private bool GettingFileTotalCount;
        private bool EnableExtractVideos;
        private bool EnableExtractSubtitles;
        private string ExtractionPathFolder;

        public void GetTotalFilesToProcess()
        {
            GettingFileTotalCount = true;
            ReadFolderContent(ExtractionPathFolder, ExtractItems);
        }

        public void StartScanning()
        {
            Model.EnableWriteLogs();

            List<Dictionary<string, dynamic>> videos = Model.getVideos();
            List<Dictionary<string, dynamic>> folders = Model.getFolders();
            List<Dictionary<string, dynamic>> subtitles = Model.getSubtitles();

            Parser.folders = new Dictionary<string, int>();
            Parser.videos = new Dictionary<string, int>();
            Parser.subtitles = new Dictionary<int, Dictionary<string, int>>();

            WriteLog.EnableWriteLogs(true);
            WriteLog.WriteLine("FOLDERS LENGTH: " + folders.Count);
            WriteLog.WriteLine("ITEMS LENGTH: " + videos.Count);
            WriteLog.WriteLine("SUBTITLES LENGTH: " + subtitles.Count);

            try
            {
                foreach(Dictionary<string, dynamic> item in folders)
                {
                    Parser.folders.Add(item["name_folder"], item["id_folder"]);
                }
                foreach (Dictionary<string, dynamic> item in videos)
                {
                    Parser.videos.Add($"{ item["id_folder"] }.{ item["noSeason_item"] }x{ item["noEpisode_item"] }", item["id_item"]);
                }
                foreach (Dictionary<string, dynamic> item in subtitles)
                {
                    int idVideo = item["iditem_subtitle"];

                    if (!Parser.subtitles.ContainsKey(idVideo)) Parser.subtitles.Add(idVideo, new Dictionary<string, int>());

                    Parser.subtitles[idVideo].Add(item["name_subtitle"], item["id_subtitle"]);
                }
            }
            catch(Exception e)
            {
                WriteLog.WriteLine("ERROR: " + e.Message);
            }

            GettingFileTotalCount = false;
            Parser.Scanning = true;
            ReadFolderContent(ExtractionPathFolder, ExtractItems);
        }

        public void StartExtraction(List<string> ListToExtract)
        {
            WriteLog.EnableWriteLogs(true);
            Parser.Scanning = false;

            foreach (string file in ListToExtract)
            {
                Helper.Worker.ReportProgress(6, file);
                Parser.Parse(file);
                Helper.Worker.ReportProgress(2);
            }
        }

        public void ExtractVideos(bool enabled)
        {
            EnableExtractVideos = enabled;
        }

        public void ExtractSubtitles(bool enabled)
        {
            EnableExtractSubtitles = enabled;
        }

        public void ExtractionPath(string path)
        {
            ExtractionPathFolder = path;
        }

        private void ExtractItems(string path)
        {
            if (EnableExtractVideos)
            {
                ParseFilesOfExt("mkv", path);
                ParseFilesOfExt("mp4", path);
                ParseFilesOfExt("avi", path);
            }

            if(EnableExtractSubtitles)
            {
                ParseFilesOfExt("srt", path);
            }
        }

        private void ReadFolderContent(string path, Action<string> func)
        {
            var dirs = from dir in Directory.EnumerateDirectories(path) select dir;
            foreach (var dir in dirs)
            {
                ReadFolderContent(dir + "\\", func);
            }

            func(path);
        }

        private void ParseFilesOfExt(string ext, string path)
        {
            var files = from file in Directory.EnumerateFiles(path, "*." + ext) select file;
            foreach (string file in files)
            {
                if (Helper.Worker.CancellationPending)
                {
                    Helper.Worker_DoWorkEvent.Cancel = true;
                    break;
                }

                Helper.Worker.ReportProgress(6, file);
                if (GettingFileTotalCount)
                {
                    Helper.Worker.ReportProgress(1);
                    continue;
                }
                WriteLog.WriteLine("FILE: " + file);
                try
                {
                    Parser.Parse(file);
                }
                catch(Exception e)
                {
                    WriteLog.WriteLine("Error: " + e);
                }
                Helper.Worker.ReportProgress(2);
            }
        }

        private void ParseSubtitles(string path)
        {
            var files = from file in Directory.EnumerateFiles(path, "*.srt") select file;
            foreach (var file in files)
            {
                Parser.ParseSubtitle(file);
            }
        }
    }
}
