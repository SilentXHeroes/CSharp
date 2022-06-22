using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FileWriter;
using Helpers;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using System.Security.Permissions;
using System.Windows.Forms;

namespace GaleryParseContent
{
    class ParseFile
    {
        private MatchCollection matches;
        private GroupCollection groups;
        private Regex rx;
        private Model model = new Model();
        private StreamReader sr;

        public bool Scanning;
        public Dictionary<string, int> folders;
        public Dictionary<string, int> videos;
        public Dictionary<int, Dictionary<string,int>> subtitles;

        private Process MergeProcess = new Process();
        private Process ParseVideoProcess = new Process();
        private Process ExtractProcess = new Process();
        private Process handleBrake = new Process();
        private Process FfmpegProcess;

        private int subtitlesAddedCount;
        private int idSubtitle;
        private int idFolder;
        private int idItem;
        private int noSeason;
        private int noEpisode;
        private string videosDirectory;
        private string subtitlesDirectory;
        private string tmpVideoPath;
        private dynamic itemMetadata;
        private string FilePath;
        private string folderName;
        private char[] lettersList = new char[] { 'C','D','E','F' };
        private int tryCountDeleteTmpFile;
        private Dictionary<string, string> letters = new Dictionary<string, string>
        {
            { "à", "C0" },
            { "â", "C2" },
            { "ä", "C4" },
            { "æ", "C6" },
            { "é", "C8" },
            { "è", "C9" },
            { "ê", "CA" },
            { "ë", "CB" },
            { "î", "CE" },
            { "ï", "CF" },
            { "ô", "D4" },
            { "ö", "D6" },
            { "û", "DB" },
            { "ü", "DC" },
            { "ç", "C7" }
        };

        public void Parse(string filePath)
        {
            model.EnableWriteLogs();

            FilePath = filePath;
            string[] pathArray = filePath.Split('\\');
            string fileName = pathArray.Last();
            bool subtitle = fileName.Substring(fileName.Length - 4) == ".srt";
            bool executeVideoProcess = true;
            bool isMovie = filePath.ToLower().IndexOf("films") > -1;

            WriteLog.EnableWriteLogs(true);

            folderName = "";

            if (isMovie == false)
            {
                for (int i = 0; i <= pathArray.Length - 1; i++)
                {
                    string value = pathArray[i];
                    if (value.ToLower().IndexOf("subtitles") > -1 || value.ToLower().IndexOf("saison") > -1)
                    {
                        folderName = pathArray[i - 1];
                        break;
                    }
                }
            }

            // Fichier à la racine du dossier
            if (folderName == "") folderName = pathArray[pathArray.Length - 2];

            if (folders.ContainsKey(folderName))
            {
                idFolder = folders[folderName];
            }
            else if (Scanning)
            {
                idFolder = 0;
            }
            else
            {
                idFolder = model.insertRow("folder",
                    new Dictionary<string, dynamic>
                    {
                        { "name_folder", folderName },
                        { "type_folder", isMovie ? 'F' : 'S' }
                    }
                );
                folders.Add(folderName, idFolder);
                Console.WriteLine("# -------------");
                Console.WriteLine("#");
                Console.WriteLine("# Dossier: " + folderName);
                Console.WriteLine("#");
                Console.WriteLine("# -------------");
                WriteLog.WriteLine("# -------------");
                WriteLog.WriteLine("#");
                WriteLog.WriteLine("# Dossier: " + folderName);
                WriteLog.WriteLine("#");
                WriteLog.WriteLine("# -------------");
            }

            WriteLog.WriteLine("-- Fichier: " + fileName);
            Console.WriteLine("-- Fichier: " + fileName);

            if (filePath.IndexOf("IGNORED") > -1)
            {
                Console.WriteLine("DOSSIER IGNORÉ");
                WriteLog.WriteLine("DOSSIER IGNORÉ");
                return;
            }

            string argumentsParseVideo = "";
            bool ffmpegParseOriginalVideo = false;

            videosDirectory = Helper.App + $@"Videos\{ idFolder }\";
            subtitlesDirectory = Helper.App + $@"Subtitles\{ idFolder }\";
            tmpVideoPath = Helper.App + "tmp.mkv";
            tryCountDeleteTmpFile = 0;

            if (!Scanning)
            {
                if (!Directory.Exists(videosDirectory)) Directory.CreateDirectory(videosDirectory);
                if (!Directory.Exists(subtitlesDirectory)) Directory.CreateDirectory(subtitlesDirectory);
            }

            ExtractProcess.StartInfo.FileName = "mkvextract";
            MergeProcess.StartInfo.FileName = "mkvmerge";

            MergeProcess.StartInfo.Arguments = $"\"{filePath}\" -J";
            MergeProcess.StartInfo.RedirectStandardOutput = true;
            MergeProcess.StartInfo.UseShellExecute = false;
            MergeProcess.StartInfo.CreateNoWindow = true;
            MergeProcess.Start();

            if (subtitle)
            {
                if (isMovie == false) rx = new Regex(@"\b(S?(\d+)[\s\.]?)?(e[p]?|x|episode\w?)?(\d+)\b", RegexOptions.IgnoreCase);
            }
            else
            {
                if (isMovie == false) rx = new Regex(@"\b(S(\d+)[x\s\.]?)?(e[p]?|[eé]pisode[\w\s]?)(\d+)\b", RegexOptions.IgnoreCase);
                itemMetadata = JsonConvert.DeserializeObject(MergeProcess.StandardOutput.ReadToEnd());

                if (itemMetadata.errors.Count > 0)
                {
                    WriteLog.WriteLine(JsonConvert.SerializeObject(itemMetadata.errors));
                    return;
                }
                else if (itemMetadata.container.recognized == false || itemMetadata.container.supported == false)
                {
                    WriteLog.WriteLine("Error: Fichier non supporté");
                    return;
                }
            }

            noSeason = 0;
            noEpisode = 0;

            if (isMovie == false)
            {
                matches = rx.Matches(fileName);

                if (matches.Count == 0)
                {
                    WriteLog.WriteLine("Aucun MATCH");
                    return;
                }

                groups = matches[0].Groups;

                noSeason = 1;
                noEpisode = Convert.ToInt32(groups[4].ToString());

                // Aucune info de la saison et le dossier nommé avec le numéro de saison
                if (groups[2].ToString() != "")
                {
                    noSeason = Convert.ToInt16(groups[2].ToString());
                }
                else if (filePath.IndexOf("Saison") > -1)
                {
                    matches = new Regex(@"\bsaison\s+(\d+)\b", RegexOptions.IgnoreCase).Matches(filePath);
                    if (matches.Count > 0 && matches[0].Groups[1].ToString() != "")
                    {
                        noSeason = Convert.ToInt32(matches[0].Groups[1].ToString());
                    }
                }
            }

            if (subtitle)
            {
                ExtractSubtitle(filePath, null);
                return;
            }

            string strVideoID = $"{idFolder}.{noSeason}x{noEpisode}";
            bool useHandleBrakeCLI = false;

            idItem = model.getVideo(idFolder, noSeason, noEpisode);

            WriteLog.WriteLine("BDD: "+ idItem +" / ID: " + strVideoID);
            //if (videos.ContainsKey(strVideoID))
            if (idItem > -1)
            {
                idItem = videos[strVideoID];
                model.set("filename_item", fileName).where("id_item", idItem).update("item");

                executeVideoProcess = !File.Exists(videosDirectory + idItem + ".mp4");
                if (!executeVideoProcess) model.set("video_exists_item", 1).where("id_item", idItem).update("item");
            }
            else if( ! Scanning)
            {
                idItem = model.insertRow("item",
                    new Dictionary<string, dynamic>
                    {
                        { "id_folder", idFolder },
                        { "name_item", "" },
                        { "noSeason_item", noSeason },
                        { "noEpisode_item", noEpisode },
                        { "chapters_item", null }
                    }
                );
            }

            subtitlesAddedCount = 1;

            WriteLog.WriteLine("- Extraction des méta données -");

            // Autorisation à récupérer l'audio français si aucune autre langue
            bool enableFrenchAudio = true;
            foreach (var info in itemMetadata.tracks)
            {
                if (info.type == "audio" && info.properties.language != "fre")
                {
                    enableFrenchAudio = false;
                    WriteLog.WriteLine("NON UTILISATION DE LA PISTE AUDIO FRANCAISE");
                    break;
                }
            }
            foreach (var info in itemMetadata.tracks)
            {
                WriteLog.WriteLine($"TYPE: {info.type}");
                if (info.type == "subtitles")
                {
                    if(enableFrenchAudio == false) ExtractSubtitle(filePath, info);
                }
                else if (Scanning || !executeVideoProcess) continue;
                else if (info.type == "video")
                {
                    if (info.codec != null && info.codec.ToString().IndexOf("H.265") > -1)
                    {
                        useHandleBrakeCLI = true;
                        WriteLog.WriteLine("Utilisation du processus HandleBrakeCLI");
                    }
                    else
                    {
                        argumentsParseVideo += $" --display-dimensions {info.id}:{info.properties.pixel_dimensions} --default-track {info.id}:yes";
                    }

                    //model.set("name_item", info.properties.track_name == null ? "" : info.properties.track_name)
                    //    .where("id_item", idItem)
                    //    .update("item");
                }
                else if (info.type == "audio" && !ffmpegParseOriginalVideo)
                {
                    string language = info.properties.language;
                    WriteLog.WriteLine("Language: " + language);
                    if (language != "jpn" && language != "en" && (enableFrenchAudio == false || language != "fre")) {
                        continue;
                    }
                    //if (info.properties.default_track == true && !useHandleBrakeCLI)
                    //{
                    //    WriteLog.WriteLine("AUDIO DEFAULT: " + info.properties.language.ToString().ToUpper());
                    //    ffmpegParseOriginalVideo = true;
                    //    continue;
                    //}

                    string iso = "fr";
                    if (info.properties.language_ietf != null) iso = info.properties.language_ietf;
                    else if (language == "jpn") iso = "ja";
                    else if (language == "en") iso = "en";

                    if (useHandleBrakeCLI)
                    {
                        if (argumentsParseVideo.IndexOf("--audio-lang-list") == -1)
                        {
                            WriteLog.WriteLine("HandBrakeCLI: Restricition des languages à " + language);
                            argumentsParseVideo += " --audio-lang-list " + language;
                        }
                    }
                    else
                    {
                        WriteLog.WriteLine("MKVMERGE: Intégration de l'audio " + iso.ToUpper());
                        argumentsParseVideo += $" --audio-tracks {info.id} --language {info.id}:{iso}";
                    }
                }
            }

            if (Scanning)
            {
                if (executeVideoProcess)
                {
                    Helper.Worker.ReportProgress(4, new Dictionary<string, dynamic>()
                    {
                        { "file", filePath },
                        { "folder", folderName },
                        { "noSeason", noSeason },
                        { "noEpisode", noEpisode }
                    });
                }
                return;
            }

            try
            {
                string log = "Extraction de la vidéo: ";
                if (!executeVideoProcess)
                {
                    WriteLog.WriteLine(log + "SKIPPED - ALREADY EXTRACTED");
                }
                else if (ffmpegParseOriginalVideo)
                {
                    DeleteTmpFile();
                    File.Copy(filePath, tmpVideoPath);

                    //string user = WindowsIdentity.GetCurrent().Name;

                    //WriteLog.WriteLine(log + "SKIPPED - VOSTFR");
                    //WriteLog.WriteLine("USER: " + user);
                    //FileInfo info = new FileInfo(tmpVideoPath);
                    //FileSecurity fl = File.GetAccessControl(tmpVideoPath);
                    //AuthorizationRuleCollection rules = fl.GetAccessRules(true, true, typeof(NTAccount));

                    //foreach (AccessRule rule in rules)
                    //{
                    //    WriteLog.WriteLine(rule.IdentityReference.Value);
                    //    if (rule.IdentityReference.Value == user)
                    //    {
                    //        fl.AddAccessRule(new FileSystemAccessRule(rule.IdentityReference, FileSystemRights.FullControl, AccessControlType.Allow));
                    //        WriteLog.WriteLine(rule.IdentityReference.Value);
                    //    }
                    //}
                    //info.SetAccessControl(fl);
                }
                else
                {
                    if (useHandleBrakeCLI)
                    {
                        WriteLog.WriteLine("Création de la vidéo .MP4");
                        ParseVideoProcess.StartInfo.FileName = "HandBrakeCLI";
                        ParseVideoProcess.StartInfo.Arguments = $"-i \"{filePath}\" --output \"{videosDirectory + idItem}.mp4\" --encoder x264 --quality 20 --rate 60 --optimize --subtitle none {argumentsParseVideo}";
                    }
                    else
                    {
                        WriteLog.Write(log + "EXTRACTING");
                        ParseVideoProcess.StartInfo.FileName = "mkvmerge";
                        ParseVideoProcess.StartInfo.Arguments = $"--ui-language fr --output \"{tmpVideoPath}\" --no-subtitles --no-global-tags --no-chapters {argumentsParseVideo} \"{filePath}\"";
                    }

                    WriteLog.WriteLine("COMMAND LINE: " + ParseVideoProcess.StartInfo.Arguments);

                    ParseVideoProcess.StartInfo.UseShellExecute = false;
                    ParseVideoProcess.StartInfo.CreateNoWindow = true;
                    ParseVideoProcess.Start();
                    ParseVideoProcess.WaitForExit();

                    WriteLog.Write("\n");

                    ExtractChapters(filePath);
                }

                if (useHandleBrakeCLI)
                {
                    model.set("video_exists_item", 1).where("id_item", idItem).update("item");
                }
                else if (executeVideoProcess)
                {
                    WriteLog.WriteLine("Création de la vidéo .MP4");
                    LaunchProcessMP4(true, false);
                }
            }
            catch (Exception e)
            {
                WriteLog.WriteLine("ERREUR: " + e);
            }
        }

        private void ExtractChapters(string path)
        {
            if (itemMetadata.chapters.Count > 0)
            {
                WriteLog.WriteLine("## Extraction des chapitres");
                ExtractProcess.StartInfo.Arguments = $"chapters \"{path}\" -s";
                ExtractProcess.StartInfo.RedirectStandardOutput = true;
                ExtractProcess.StartInfo.UseShellExecute = false;
                ExtractProcess.StartInfo.CreateNoWindow = true;
                ExtractProcess.Start();

                int loop = 0;
                List<string> chaptersToStr = new List<string>();
                TimeSpan lastTime = new TimeSpan();
                foreach (string chapter in ExtractProcess.StandardOutput.ReadToEnd().Split(new char[] { '\r', '\n' }))
                {
                    if (chapter.IndexOf("NAME") > -1 || chapter.Length == 0) continue;

                    string time = chapter.Split('=')[1];
                    string[] detail = time.Split(':');
                    TimeSpan toTime = new TimeSpan(Convert.ToInt32(detail[0]), Convert.ToInt32(detail[1]), Convert.ToInt32(detail[2].Split('.')[0]));

                    if (loop > 0)
                    {
                        TimeSpan diff = toTime.Subtract(lastTime);
                        int diffToSeconds = diff.Hours * 3600 + diff.Minutes * 60 + diff.Seconds;
                        bool averageTimeOpening = diffToSeconds > 85 && diffToSeconds < 95;
                        WriteLog.WriteLine($"{loop} - {diffToSeconds} - AVERAGE: {averageTimeOpening}");
                        // Opening ?
                        if (loop <= 2 && averageTimeOpening)
                        {
                            chaptersToStr.Add("OPENING=" + lastTime.TotalSeconds.ToString());
                        }
                        // Récapitulatif ?
                        else if (loop <= 2)
                        {
                            chaptersToStr.Add("INTRO=" + lastTime.TotalSeconds.ToString());
                        }
                        // Ending ?
                        else if (averageTimeOpening)
                        {
                            chaptersToStr.Add("ENDING=" + lastTime.TotalSeconds.ToString());
                        }
                    }

                    lastTime = toTime;
                    loop++;
                }

                model.set("chapters_item", String.Join(";", chaptersToStr))
                    .where("id_item", idItem)
                    .update("item");
            }
        }

        private void LaunchProcessMP4(bool over, bool useStrict)
        {
            string args = "-c copy";

            if (over)
            {
                if (useStrict)
                {
                    WriteLog.WriteLine("ERROR - Ajout du paramètre '-strict -2'");

                    // Suppression du fichier en erreur
                    File.Delete(videosDirectory + idItem + ".mp4");

                    args += " -strict -2";
                }

                FfmpegProcess = new Process();
                FfmpegProcess.StartInfo.FileName = "ffmpeg";
                FfmpegProcess.StartInfo.Arguments = $"-i \"{tmpVideoPath}\" {args} \"{videosDirectory + idItem}.mp4\"";

                WriteLog.WriteLine("COMMAND LINE: " + FfmpegProcess.StartInfo.Arguments);

                FfmpegProcess.StartInfo.RedirectStandardError = true;
                FfmpegProcess.StartInfo.UseShellExecute = false;
                FfmpegProcess.StartInfo.CreateNoWindow = true;
                FfmpegProcess.Start();
            }

            string output = FfmpegProcess.StandardError.ReadToEnd();

            if (!FfmpegProcess.WaitForExit(5000))
            {
                LaunchProcessMP4(false, false);
            }
            // Le script nous indique de forcer la commande
            // On recommence avec le paramètre supplémentaire
            else if (output.IndexOf("add '-strict -2'") > -1)
            {
                FfmpegProcess.Close();
                FfmpegProcess.Dispose();
                LaunchProcessMP4(true, true);
            }
            else
            {
                model.set("video_exists_item", 1).where("id_item", idItem).update("item");
                DeleteTmpFile();
            }
        }

        private void DeleteTmpFile()
        {
            if (File.Exists(tmpVideoPath))
            {
                try
                {
                    WriteLog.WriteLine("== Tentative de suppression du fichier temporaire");
                    File.Delete(tmpVideoPath);
                    WriteLog.WriteLine("Fichier temporaire supprimé avec succès");
                }
                catch(Exception e)
                {
                    WriteLog.WriteLine("Tentative echouée: " + e);

                    Thread.Sleep(2000);
                    tryCountDeleteTmpFile++;
                    if (tryCountDeleteTmpFile < 5) DeleteTmpFile();
                }
            }
        }

        private void ExtractSubtitle(string path, dynamic data)
        {
            if (!subtitles.ContainsKey(idItem))
            {
                subtitles.Add(idItem, new Dictionary<string, int>());
            }

            idSubtitle = -1;
            string log = "Extraction du sous-titre: ";
            string subtitleFileName = "";
            string trackName = (data == null ? "1 - Français" : (data.properties.track_name == null ? subtitlesAddedCount + " - " + (data.properties.language == "fre" ? "Francais" : "Non définie") : data.properties.track_name));

            trackName = Helper.EncodeUTF8(trackName);

            if (data != null && (data.properties.codec_id == "S_HDMV/PGS" || data.properties.codec_id == "S_VOBSUB"))
            {
                WriteLog.WriteLine($"{log}FORMAT NON SUPPORTÉ ({data.properties.codec_id})");
                return;
            }

            bool executeSubtitleExtactor = !subtitles[idItem].ContainsKey(trackName);

            // Le sous-titre est présent en base
            // On vérifie si le fichier VTT existe
            if (!executeSubtitleExtactor)
            {
                idSubtitle = subtitles[idItem][trackName];
                subtitleFileName = $"{subtitlesDirectory}{idSubtitle}.srt";
                executeSubtitleExtactor = !File.Exists(subtitleFileName.Replace(".srt", ".vtt"));
                WriteLog.WriteLine("SUBTITLE EXISTS: " + subtitleFileName.Replace(".srt", ".vtt"));
            }

            if (
                (data != null && data.properties.language != "fre" && data.properties.language != "und") ||
                !executeSubtitleExtactor)
            {
                if(!executeSubtitleExtactor) WriteLog.WriteLine("FICHIER EN BASE ET DEJA PRESENT");
                else if(data != null)
                {
                    WriteLog.WriteLine($"LANGUAGE: {data.properties.language}");
                }
                WriteLog.WriteLine($"{log}SKIPPED");
                return;
            }

            if (Scanning)
            {
                Dictionary <string, dynamic> result = null;
                if(data == null)
                {
                    result = new Dictionary<string, dynamic>()
                    {
                        { "file", FilePath },
                        { "folder", folderName },
                        { "noSeason", noSeason },
                        { "noEpisode", noEpisode }
                    };
                }
                Helper.Worker.ReportProgress(3, result);
                return;
            }

            WriteLog.WriteLine($"{log}\"{trackName}\"");

            if (idSubtitle == -1)
            {
                idSubtitle = model.insertRow("subtitle",
                    new Dictionary<string, dynamic>
                    {
                        { "iditem_subtitle", idItem },
                        { "name_subtitle", trackName },
                        { "php_parse", 0 }
                    }
                );
                subtitles[idItem].Add(trackName, idSubtitle);
                subtitleFileName = $"{subtitlesDirectory}{idSubtitle}.srt";                
            }

            subtitlesAddedCount++;

            // Le fichier SRT est déjà extrait
            if (data == null)
            {
                if (File.Exists(subtitleFileName))
                {
                    try
                    {
                        File.Delete(subtitleFileName);
                        File.Copy(path, subtitleFileName);
                    }
                    catch (Exception e)
                    {
                        WriteLog.WriteLine("Erreur copie de fichier: " + e);
                    }
                }
                else
                {
                    File.Copy(path, subtitleFileName);
                }
            }
            else
            {
                if (data.properties.codec_id == "S_TEXT/WEBVTT") subtitleFileName = subtitleFileName.Replace(".srt",".vtt");

                ExtractProcess.StartInfo.Arguments = $"tracks \"{path}\" {data.id}:{subtitleFileName}";
                ExtractProcess.StartInfo.RedirectStandardOutput = true;
                ExtractProcess.StartInfo.UseShellExecute = false;
                ExtractProcess.StartInfo.CreateNoWindow = true;
                ExtractProcess.Start();
                ExtractProcess.StandardOutput.ReadToEnd();
                ExtractProcess.WaitForExit();

                // Format WebVTT, on ignore le formatage
                if (data.properties.codec_id == "S_TEXT/WEBVTT") return;
            }

            WriteLog.WriteLine($"Formatage du sous-titre");

            ParseSubtitle(subtitleFileName);
        }

        public void ParseSubtitle(string file)
        {
            if (!File.Exists(file)) return;
            sr = new StreamReader(file);

            string newFile = file.Replace(".srt", ".vtt");
            string str = sr.ReadLine();
            string previousTime = "";
            bool vttFormat = false;

            if (File.Exists(newFile)) return;

            File.AppendAllText(newFile, "WEBVTT\r\n");

            while (str != null)
            {
                if (!vttFormat && str.IndexOf("Dialogue") == 0)
                {
                    string data = str.Substring(str.IndexOf(':') + 2);
                    string value, time = "";
                    int idx;

                    for (int i = 0; i < 9; i++)
                    {
                        idx = data.IndexOf(',');
                        if (idx == -1)
                        {
                            value = data;
                            data = "";
                        }
                        else
                        {
                            value = data.Substring(0, idx);
                            data = data.Substring(idx + 1);
                        }

                        // Temps entrée - sortie
                        if (i == 1 || i == 2)
                        {
                            if (i == 1)
                            {
                                time = '0' + value + "0 --> ";
                            }
                            else
                            {
                                time += '0' + value + "0";
                                if (time != previousTime)
                                {
                                    previousTime = time;
                                    File.AppendAllText(newFile, "\r\n" + time + "\r\n");
                                }
                            }
                        }
                        // Texte
                        else if (i == 8)
                        {
                            data = data.Replace("{\\*FORCED}", ""); // Suppression forçage
                            data = Regex.Replace(data, @"\\(pos|fad)\(\d+([\.,]\d+)?,\d+([\.,]\d+)?\)\s?", ""); // Suppression repositionnement
                            data = Regex.Replace(data, @"\\((a|an|fs|blur)\d+|pub)", ""); // Informations inutiles
                            data = Regex.Replace(data, @"\\(\d+)?c&\w+&", ""); // Suppression des couleurs
                            // Fermeture des tags html syntaxé bizarrement
                            data = data.Replace(@"{\i\i1\i}", "</i>");
                            data = data.Replace("{i}", "<i>");
                            data = data.Replace(@"{\i1\i}", "</i>");
                            data = data.Replace("{/i}", "</i>");
                            data = data.Replace(@"{\i\i1}", "");

                            MatchCollection matches = new Regex(@"\{\\(\w+)\d?\}", RegexOptions.IgnoreCase).Matches(data);
                            for (int j = 0; j < matches.Count; j++)
                            {
                                string group = matches[j].Groups[1].ToString();
                                if (group.IndexOf('1') > -1)
                                {
                                    data = data.Replace("{\\" + group + "}", "<" + group.Replace("1", "") + ">");
                                }
                                else
                                {
                                    data = data.Replace("{\\" + group + "}", "</" + group.Replace("0", "") + ">");
                                }
                            }

                            data = Regex.Replace(data, @"(\{|\})((.+)?\})?", ""); // Suppression des accolades vides / seules + textes inutiles des mecs...
                            data = data.Replace("\\N", "\r\n");
                            data = Helper.EncodeUTF8(data);
                            File.AppendAllText(newFile, data + "\r\n");
                        }
                    }
                }
                // Format VTT, on laisse PHP s'en occuper
                else if (str.IndexOf("-->") > -1)
                {
                    model.set("php_parse", 1)
                        .where("id_subtitle", idSubtitle)
                        .update("subtitle");
                    File.Delete(newFile);
                    break;
                }
                str = sr.ReadLine();
            }
        }

    }
}
