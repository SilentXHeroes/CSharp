using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Helpers;
using GaleryParseContent;
using FileWriter;

namespace ExtractGalery
{
    public partial class Form1 : Form
    {
        private delegate void SafeCallDelegate(dynamic text);
        private Galery galery = new Galery();
        private Model model = new Model();
        private bool ExtractedOnce;
        private bool Extracting;
        private int BaseWidth;
        private int BaseHeight;
        private int BaseWidthLayoutGalery;
        private int ExtractionPanelBaseHeight;
        private int ExtractionDetailsBaseHeight;
        private int ParseSubtitleCounter = 0;
        private int ParseVideoCounter = 0;
        private List<string> ExtractionList;
        private bool galeryShown = false;

        public Form1()
        {
            InitializeComponent();

            BackgroundWorker.WorkerReportsProgress = true;
            BackgroundWorker.WorkerSupportsCancellation = true;
            BackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            BackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

            galeryTree.AfterCheck += galeryTree_AfterCheck;

            Helper.InitializeWorker(BackgroundWorker);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Dimensions de la fenêtre
            ExtractionPanelBaseHeight = ExtractionPanel.Height;
            ExtractionDetailsBaseHeight = ExtractionDetails.Height;
            BaseHeight = this.Height - ExtractionPanelBaseHeight;

            // Extraction de vidéos par défaut
            ExtractVideos.Checked = true;
            // Extraction de sous-titres par défaut
            ExtractSubtitles.Checked = true;

            // Chemin par défaut des vidéos
            PathFolder.Text = Helper.Folder;

            //galeryTree.Hide();
            this.Width = layoutGalery.Width + layoutExtract.Width;
            BaseWidth = this.Width;
            BaseWidthLayoutGalery = layoutGalery.Width;
            layoutMain.Width = this.Width - 15;
            layoutMain.ColumnStyles[0].Width = 0;
            layoutMain.ColumnStyles[1].Width = this.Width;

            ExtractedOnce = false;
            Extracting = false;
            ShowDetails.Checked = false;
            HandleShownElements();
            ResetInformationsExtraction();
        }

        private void ChoseFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDlg = new FolderBrowserDialog();
            // Show the FolderBrowserDialog.  
            folderDlg.SelectedPath = this.PathFolder.Text;
            DialogResult result = folderDlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                PathFolder.Text = folderDlg.SelectedPath + '\\';
            }
        }

        private void PathFolder_TextChanged(object sender, EventArgs e)
        {
            galery.ExtractionPath(PathFolder.Text);
        }

        private void StartExtraction_Click(object sender, EventArgs e)
        {
            if(Extracting)
            {
                Helper.Worker.CancelAsync();
                EnableForm(true);
            }
            else
            {
                Extracting = true;
                ExtractedOnce = true;

                ResetInformationsExtraction();
                EnableForm(false);

                Helper.ExtractionProcessState = 1;
                galery.ExtractVideos(ExtractVideos.Checked);
                galery.ExtractSubtitles(ExtractSubtitles.Checked);

                if (BackgroundWorker.IsBusy != true)
                {
                    BackgroundWorker.RunWorkerAsync();
                }
            }
        }

        private void ShowDetails_CheckedChanged_1(object sender, EventArgs e)
        {
            HandleShownElements();
        }

        #region BackgroundWorker

        private void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Helper.InitializeDoWorkEvent(e);

            if (BackgroundWorker.CancellationPending)
            {
                e.Cancel = true;
            }
            // Balayage du nombre de total de fichiers présents
            else 
            {
                WriteLog("## Balayage des fichiers");
                galery.GetTotalFilesToProcess();
                WriteLog($"## Détection des fichier à traiter");
                ChangeBtnText("Détection des fichiers ...");
                galery.StartScanning();
                WriteLog($"## Début de l'extraction");
                ChangeBtnText("Extraction en cours ...");
                ResetProgressBar(ExtractionList.Count);
                WriteLog(ExtractionList.Count.ToString());
                galery.StartExtraction(ExtractionList);
                e.Cancel = true;
            }
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            object value = e.UserState;
            int action = e.ProgressPercentage;
            Dictionary<string, dynamic> result = null;

            if (action == 3 || action == 4)
            {
                result = e.UserState as Dictionary<string, dynamic>;
                if (result != null)
                {
                    TreeNode FolderNode = Helper.getNode(ListToProcess, result["folder"]);
                    TreeNode ActionNode = Helper.getNode(FolderNode, action == 3 ? "Sous-titres" : "Vidéos");
                    TreeNode SeasonNode = Helper.getNode(ActionNode, "Saison " + result["noSeason"]);

                    SeasonNode.Nodes.Add("Épisode " + result["noEpisode"]);
                    ExtractionList.Add(result["file"]);
                }
            }

            // Mise à jour du nombre total de fichiers
            if (action == 1)
            {
                ProgressBar.Maximum++;
            }
            // Mise à jour du pourcentage de progression
            else if (action == 2)
            {
                ProgressBar.Value++;
            }
            // Mise à jour du nombre de sous-titres à traiter
            else if (action == 3)
            {
                SubtitlesCount.Text = $"{++ParseSubtitleCounter} fichiers";
            }
            // Mise à jour du nombre de vidéos à traiter
            else if (action == 4)
            {
                VideosCount.Text = $"{++ParseVideoCounter} fichiers";
            }
            // LOGS
            else if (action == 5)
            {
                WriteLog(value.ToString());
            }
            // Mise à jour du fichier en cours de traitement
            else if(action == 6)
            {
                ProcessingFileName.Text = value.ToString().Replace(PathFolder.Text, @".\");
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                WriteLog(" ## Extraction terminée");
                Extracting = false;
                EnableForm(true);
            }
        }

        private void HandleShownElements()
        {
            int TotalHeight = BaseHeight;

            ExtractionPanel.Visible = ExtractedOnce;
            if (ExtractionPanel.Visible) TotalHeight += ExtractionPanelBaseHeight * (ExtractionPanel.Visible ? 1 : -1);

            ExtractionDetails.Visible = ShowDetails.Checked;
            if (ExtractionPanel.Visible && !ExtractionDetails.Visible) TotalHeight -= ExtractionDetailsBaseHeight;

            ProcessInfos.Show();
            ExtractionProcessInfo.Show();
            ShowDetails.Show();

            this.Height = TotalHeight;
        }

        private void WriteLog(dynamic log)
        {
            if (LogEvents.InvokeRequired)
            {
                LogEvents.Invoke(new SafeCallDelegate(WriteLog), new object[] { log });
            }
            else
            {
                bool selectLastLog = LogEvents.Items.Count == 0 || LogEvents.SelectedIndex == LogEvents.Items.Count - 1;

                LogEvents.Items.Add($"[{DateTime.Now}] " + log);

                if (selectLastLog) LogEvents.SelectedIndex = LogEvents.Items.Count - 1;
            }
        }

        private void ChangeBtnText(dynamic text)
        {
            if (StartExtraction.InvokeRequired)
            {
                StartExtraction.Invoke(new SafeCallDelegate(ChangeBtnText), new object[] { text });
            }
            else
            {
                StartExtraction.Text = text + (text == "Commencer l'extraction" ? "" : " (Cliquer pour annuler)");
            }
        }

        private void EnableForm(bool enabled)
        {
            ChangeBtnText(enabled ? "Commencer l'extraction" : "Balayage des fichiers ...");
            ExtractVideos.Enabled = enabled;
            ExtractSubtitles.Enabled = enabled;
            ChoseFolder.Enabled = enabled;
            HandleShownElements();
        }

        private void ResetProgressBar(dynamic value)
        {
            if (StartExtraction.InvokeRequired)
            {
                ProgressBar.Invoke(new SafeCallDelegate(ResetProgressBar), new object[] { value });
            }
            else
            {
                ProgressBar.Maximum = ExtractionList.Count;
                ProgressBar.Value = 0;
            }
        }

        private void ResetInformationsExtraction()
        {
            ParseVideoCounter = 0;
            ParseSubtitleCounter = 0;

            VideosCount.Text = "Aucun fichier";
            SubtitlesCount.Text = "Aucun fichier";
            ProcessingFileName.Text = "...";
            ProgressBar.Value = 0;
            ProgressBar.Maximum = 1;
            ProcessInfos.Hide();
            ExtractionProcessInfo.Hide();
            ShowDetails.Hide();

            ListToProcess.Nodes.Clear();
            ExtractionList = new List<string>();
        }

        private void showGalery_Click(object sender, EventArgs e)
        {
            // On cache la galerie
            if (galeryShown)
            {
                this.Width = BaseWidth;
                layoutMain.ColumnStyles[0].Width = 0;
                layoutMain.ColumnStyles[1].Width = this.Width;
                this.Height = BaseHeight;
                showGalery.Text = "<<    Afficher ma galerie";
                showGalery.BackColor = SystemColors.Control;
                showGalery.FlatAppearance.BorderColor = Color.Black;
            }
            // On affiche la galerie
            else
            {
                this.Width = BaseWidth + BaseWidthLayoutGalery;
                layoutMain.ColumnStyles[0].Width = (float)(this.Width * .3);
                layoutMain.ColumnStyles[1].Width = (float)(this.Width * .7);
                this.Height = 350;
                showGalery.Text = ">>    Cacher ma galerie";
                showGalery.BackColor = SystemColors.ActiveCaption;
                showGalery.FlatAppearance.BorderColor = Color.DeepSkyBlue;

                PrintGalery();
            }

            galeryShown = !galeryShown;
        }

        private void PrintGalery()
        {
            List<Dictionary<string, dynamic>> allItems = model.getAllItems();

            galeryTree.Nodes.Clear();

            allItems.ForEach(delegate (Dictionary<string, dynamic> item)
            {
                TreeNode FolderNode = Helper.getNode(galeryTree, item["name_folder"], "FOLDER." + item["id_folder"]);
                TreeNode SeasonNode = Helper.getNode(FolderNode, "Saison " + item["noSeason_item"], $"SEASON.{ item["id_folder"] }.{ item["noSeason_item"] }");
                TreeNode EpisodeNode = Helper.getNode(SeasonNode, "Épisode " + item["noEpisode_item"], "EPISODE." + item["id_item"]);

                if (item["name_subtitle"] != "") Helper.AddTreeNode(EpisodeNode, item["name_subtitle"] + " (" + item["id_subtitle"] + ')', "SUBTITLE." + item["id_subtitle"]);
            });
        }

        private void galeryTree_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (e.Action == TreeViewAction.ByMouse)
            {
                Helper.updateChildrenNodes(e.Node, e.Node.Checked);
                Helper.updateParentNode(e.Node, e.Node.Checked);
            }
        }

        private void supprItem_Click(object sender, EventArgs e)
        {
            DialogResult response = MessageBox.Show("Voulez-vous vraiment supprimer l.es élément.s sélectionné.s ?", "Confirmation", MessageBoxButtons.YesNo);

            if (response == DialogResult.No) return;

            try
            {
                model.EnableWriteLogs();

                void RemoveSelectedNodes(dynamic node)
                {
                    foreach (TreeNode child in node.Nodes)
                    {
                        if (child.Checked && child.Tag != null)
                        {
                            string[] value = child.Tag.ToString().Split('.');
                            string typeTag = value[0].ToString();
                            int id = Convert.ToInt32(value[1]);

                            if (typeTag == "FOLDER")
                            {
                                model.DeleteFolder(id);
                                // On supprime le dossier + les saisons + épisodes + sous-titres
                                // On passe à l'élément suivant
                                continue;
                            }
                            else if (typeTag == "SEASON")
                            {
                                model.DeleteSeason(id, Convert.ToInt32(value[2]));
                                // On supprime la saison + épisodes + sous-titres
                                // On passe à l'élément suivant
                                continue;
                            }
                            else if (typeTag == "EPISODE")
                            {
                                model.DeleteItem(idItem: id);
                            }
                            else if (typeTag == "SUBTITLE")
                            {
                                model.DeleteSubtitle(id);
                            }
                        }

                        RemoveSelectedNodes(child);
                    }
                }

                RemoveSelectedNodes(galeryTree);
                PrintGalery();
                model.DisableWriteLogs();
            }
            catch(Exception exc)
            {
                MessageBox.Show(exc.Message +" => "+ exc.StackTrace, "Une erreur est survenue", MessageBoxButtons.OK);
                FileWriter.WriteLog.WriteLine(exc.Message);
                FileWriter.WriteLog.WriteLine(exc.StackTrace);
            }
        }

        #endregion

        #region Fonctions non utilisées

        private void BrowseFolders_HelpRequest(object sender, EventArgs e) { }
        private void ProgressBar_Click(object sender, EventArgs e) { }
        private void ExtractVideos_CheckedChanged(object sender, EventArgs e) { }
        private void LogEvents_SelectedIndexChanged(object sender, EventArgs e) { }
        private void ExtractionProcessInfo_Paint(object sender, PaintEventArgs e) { }
        private void ExtractionPanel_Paint(object sender, PaintEventArgs e) { }
        private void label1_Click(object sender, EventArgs e) { }
        private void VideosCount_Click(object sender, EventArgs e) { }
        private void SubtitlesCount_Click(object sender, EventArgs e) { }
        private void ProcessingFileName_TextChanged(object sender, EventArgs e) { }
        private void ListToProcess_AfterSelect(object sender, TreeViewEventArgs e) { }
        private void LogEvents_SelectedIndexChanged_2(object sender, EventArgs e) { }
        private void TabLogs_Click(object sender, EventArgs e) { }
        private void FilesToProcess_Click(object sender, EventArgs e) { }
        private void ExtractionProcessInfo_Paint_1(object sender, PaintEventArgs e) { }
        private void tableLayoutPanel2_Paint(object sender, PaintEventArgs e) { }
        private void label_Click(object sender, EventArgs e) { }
        private void layoutMain_Paint(object sender, PaintEventArgs e) { }

        #endregion

    }
}
