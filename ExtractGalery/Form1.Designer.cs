
namespace ExtractGalery
{
    partial class Form1
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.StartExtraction = new System.Windows.Forms.Button();
            this.ExtractVideos = new System.Windows.Forms.CheckBox();
            this.ExtractSubtitles = new System.Windows.Forms.CheckBox();
            this.browseFolders = new System.Windows.Forms.FolderBrowserDialog();
            this.ChoseFolder = new System.Windows.Forms.Button();
            this.PathFolder = new System.Windows.Forms.TextBox();
            this.ProgressBar = new System.Windows.Forms.ProgressBar();
            this.label = new System.Windows.Forms.Label();
            this.ExtractionProcessInfo = new System.Windows.Forms.TableLayoutPanel();
            this.SubtitlesCount = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.VideosCount = new System.Windows.Forms.Label();
            this.ProcessingFileName = new System.Windows.Forms.TextBox();
            this.ProcessInfos = new System.Windows.Forms.TabControl();
            this.FilesToProcess = new System.Windows.Forms.TabPage();
            this.ListToProcess = new System.Windows.Forms.TreeView();
            this.TabLogs = new System.Windows.Forms.TabPage();
            this.LogEvents = new System.Windows.Forms.ListBox();
            this.BackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.ShowDetails = new System.Windows.Forms.CheckBox();
            this.layoutExtract = new System.Windows.Forms.TableLayoutPanel();
            this.layoutExtractButtons = new System.Windows.Forms.TableLayoutPanel();
            this.showGalery = new System.Windows.Forms.Button();
            this.ExtractionPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ExtractionDetails = new System.Windows.Forms.TableLayoutPanel();
            this.layoutExtractOptions = new System.Windows.Forms.TableLayoutPanel();
            this.layoutPath = new System.Windows.Forms.TableLayoutPanel();
            this.layoutMain = new System.Windows.Forms.TableLayoutPanel();
            this.layoutGalery = new System.Windows.Forms.TableLayoutPanel();
            this.supprItem = new System.Windows.Forms.Button();
            this.galeryTree = new System.Windows.Forms.TreeView();
            this.ExtractionProcessInfo.SuspendLayout();
            this.ProcessInfos.SuspendLayout();
            this.FilesToProcess.SuspendLayout();
            this.TabLogs.SuspendLayout();
            this.layoutExtract.SuspendLayout();
            this.layoutExtractButtons.SuspendLayout();
            this.ExtractionPanel.SuspendLayout();
            this.ExtractionDetails.SuspendLayout();
            this.layoutExtractOptions.SuspendLayout();
            this.layoutPath.SuspendLayout();
            this.layoutMain.SuspendLayout();
            this.layoutGalery.SuspendLayout();
            this.SuspendLayout();
            // 
            // StartExtraction
            // 
            resources.ApplyResources(this.StartExtraction, "StartExtraction");
            this.StartExtraction.BackColor = System.Drawing.SystemColors.Control;
            this.StartExtraction.Name = "StartExtraction";
            this.StartExtraction.UseVisualStyleBackColor = false;
            this.StartExtraction.Click += new System.EventHandler(this.StartExtraction_Click);
            // 
            // ExtractVideos
            // 
            resources.ApplyResources(this.ExtractVideos, "ExtractVideos");
            this.ExtractVideos.Name = "ExtractVideos";
            this.ExtractVideos.UseVisualStyleBackColor = true;
            this.ExtractVideos.CheckedChanged += new System.EventHandler(this.ExtractVideos_CheckedChanged);
            // 
            // ExtractSubtitles
            // 
            resources.ApplyResources(this.ExtractSubtitles, "ExtractSubtitles");
            this.ExtractSubtitles.Name = "ExtractSubtitles";
            this.ExtractSubtitles.UseVisualStyleBackColor = true;
            // 
            // browseFolders
            // 
            this.browseFolders.HelpRequest += new System.EventHandler(this.BrowseFolders_HelpRequest);
            // 
            // ChoseFolder
            // 
            resources.ApplyResources(this.ChoseFolder, "ChoseFolder");
            this.ChoseFolder.Name = "ChoseFolder";
            this.ChoseFolder.UseVisualStyleBackColor = true;
            this.ChoseFolder.Click += new System.EventHandler(this.ChoseFolder_Click);
            // 
            // PathFolder
            // 
            resources.ApplyResources(this.PathFolder, "PathFolder");
            this.PathFolder.Name = "PathFolder";
            this.PathFolder.TextChanged += new System.EventHandler(this.PathFolder_TextChanged);
            // 
            // ProgressBar
            // 
            resources.ApplyResources(this.ProgressBar, "ProgressBar");
            this.ProgressBar.Name = "ProgressBar";
            this.ProgressBar.Step = 1;
            this.ProgressBar.Click += new System.EventHandler(this.ProgressBar_Click);
            // 
            // label
            // 
            resources.ApplyResources(this.label, "label");
            this.label.Name = "label";
            this.label.Click += new System.EventHandler(this.label_Click);
            // 
            // ExtractionProcessInfo
            // 
            resources.ApplyResources(this.ExtractionProcessInfo, "ExtractionProcessInfo");
            this.ExtractionProcessInfo.Controls.Add(this.SubtitlesCount, 1, 1);
            this.ExtractionProcessInfo.Controls.Add(this.label3, 0, 2);
            this.ExtractionProcessInfo.Controls.Add(this.label2, 0, 1);
            this.ExtractionProcessInfo.Controls.Add(this.label1, 0, 0);
            this.ExtractionProcessInfo.Controls.Add(this.VideosCount, 1, 0);
            this.ExtractionProcessInfo.Controls.Add(this.ProcessingFileName, 1, 2);
            this.ExtractionProcessInfo.Name = "ExtractionProcessInfo";
            this.ExtractionProcessInfo.Paint += new System.Windows.Forms.PaintEventHandler(this.ExtractionProcessInfo_Paint_1);
            // 
            // SubtitlesCount
            // 
            resources.ApplyResources(this.SubtitlesCount, "SubtitlesCount");
            this.SubtitlesCount.Name = "SubtitlesCount";
            this.SubtitlesCount.Click += new System.EventHandler(this.SubtitlesCount_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // VideosCount
            // 
            resources.ApplyResources(this.VideosCount, "VideosCount");
            this.VideosCount.Name = "VideosCount";
            this.VideosCount.Click += new System.EventHandler(this.VideosCount_Click);
            // 
            // ProcessingFileName
            // 
            resources.ApplyResources(this.ProcessingFileName, "ProcessingFileName");
            this.ProcessingFileName.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ProcessingFileName.Name = "ProcessingFileName";
            this.ProcessingFileName.ReadOnly = true;
            this.ProcessingFileName.TextChanged += new System.EventHandler(this.ProcessingFileName_TextChanged);
            // 
            // ProcessInfos
            // 
            resources.ApplyResources(this.ProcessInfos, "ProcessInfos");
            this.ProcessInfos.Controls.Add(this.FilesToProcess);
            this.ProcessInfos.Controls.Add(this.TabLogs);
            this.ProcessInfos.Name = "ProcessInfos";
            this.ProcessInfos.SelectedIndex = 0;
            // 
            // FilesToProcess
            // 
            this.FilesToProcess.Controls.Add(this.ListToProcess);
            resources.ApplyResources(this.FilesToProcess, "FilesToProcess");
            this.FilesToProcess.Name = "FilesToProcess";
            this.FilesToProcess.UseVisualStyleBackColor = true;
            this.FilesToProcess.Click += new System.EventHandler(this.FilesToProcess_Click);
            // 
            // ListToProcess
            // 
            resources.ApplyResources(this.ListToProcess, "ListToProcess");
            this.ListToProcess.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.ListToProcess.Name = "ListToProcess";
            this.ListToProcess.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.ListToProcess_AfterSelect);
            // 
            // TabLogs
            // 
            this.TabLogs.Controls.Add(this.LogEvents);
            resources.ApplyResources(this.TabLogs, "TabLogs");
            this.TabLogs.Name = "TabLogs";
            this.TabLogs.UseVisualStyleBackColor = true;
            this.TabLogs.Click += new System.EventHandler(this.TabLogs_Click);
            // 
            // LogEvents
            // 
            resources.ApplyResources(this.LogEvents, "LogEvents");
            this.LogEvents.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LogEvents.Name = "LogEvents";
            this.LogEvents.SelectedIndexChanged += new System.EventHandler(this.LogEvents_SelectedIndexChanged_2);
            // 
            // BackgroundWorker
            // 
            this.BackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.BackgroundWorker_DoWork);
            // 
            // ShowDetails
            // 
            resources.ApplyResources(this.ShowDetails, "ShowDetails");
            this.ShowDetails.Name = "ShowDetails";
            this.ShowDetails.UseVisualStyleBackColor = true;
            this.ShowDetails.CheckedChanged += new System.EventHandler(this.ShowDetails_CheckedChanged_1);
            // 
            // layoutExtract
            // 
            resources.ApplyResources(this.layoutExtract, "layoutExtract");
            this.layoutExtract.Controls.Add(this.layoutExtractButtons, 0, 2);
            this.layoutExtract.Controls.Add(this.ExtractionPanel, 0, 3);
            this.layoutExtract.Controls.Add(this.layoutExtractOptions, 0, 1);
            this.layoutExtract.Controls.Add(this.layoutPath, 0, 0);
            this.layoutExtract.Name = "layoutExtract";
            this.layoutExtract.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel2_Paint);
            // 
            // layoutExtractButtons
            // 
            resources.ApplyResources(this.layoutExtractButtons, "layoutExtractButtons");
            this.layoutExtractButtons.Controls.Add(this.StartExtraction, 1, 0);
            this.layoutExtractButtons.Controls.Add(this.showGalery, 0, 0);
            this.layoutExtractButtons.Name = "layoutExtractButtons";
            // 
            // showGalery
            // 
            resources.ApplyResources(this.showGalery, "showGalery");
            this.showGalery.BackColor = System.Drawing.SystemColors.Control;
            this.showGalery.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.showGalery.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.showGalery.Name = "showGalery";
            this.showGalery.UseVisualStyleBackColor = false;
            this.showGalery.Click += new System.EventHandler(this.showGalery_Click);
            // 
            // ExtractionPanel
            // 
            resources.ApplyResources(this.ExtractionPanel, "ExtractionPanel");
            this.ExtractionPanel.Controls.Add(this.ProgressBar, 0, 0);
            this.ExtractionPanel.Controls.Add(this.ExtractionDetails, 0, 2);
            this.ExtractionPanel.Controls.Add(this.ShowDetails, 0, 1);
            this.ExtractionPanel.Name = "ExtractionPanel";
            // 
            // ExtractionDetails
            // 
            resources.ApplyResources(this.ExtractionDetails, "ExtractionDetails");
            this.ExtractionDetails.Controls.Add(this.ExtractionProcessInfo, 0, 0);
            this.ExtractionDetails.Controls.Add(this.ProcessInfos, 0, 1);
            this.ExtractionDetails.Name = "ExtractionDetails";
            // 
            // layoutExtractOptions
            // 
            resources.ApplyResources(this.layoutExtractOptions, "layoutExtractOptions");
            this.layoutExtractOptions.Controls.Add(this.ExtractVideos, 0, 0);
            this.layoutExtractOptions.Controls.Add(this.ExtractSubtitles, 1, 0);
            this.layoutExtractOptions.Name = "layoutExtractOptions";
            // 
            // layoutPath
            // 
            resources.ApplyResources(this.layoutPath, "layoutPath");
            this.layoutPath.Controls.Add(this.ChoseFolder, 2, 0);
            this.layoutPath.Controls.Add(this.PathFolder, 1, 0);
            this.layoutPath.Controls.Add(this.label, 0, 0);
            this.layoutPath.Name = "layoutPath";
            // 
            // layoutMain
            // 
            resources.ApplyResources(this.layoutMain, "layoutMain");
            this.layoutMain.Controls.Add(this.layoutExtract, 1, 0);
            this.layoutMain.Controls.Add(this.layoutGalery, 0, 0);
            this.layoutMain.Name = "layoutMain";
            this.layoutMain.Paint += new System.Windows.Forms.PaintEventHandler(this.layoutMain_Paint);
            // 
            // layoutGalery
            // 
            resources.ApplyResources(this.layoutGalery, "layoutGalery");
            this.layoutGalery.Controls.Add(this.supprItem, 0, 1);
            this.layoutGalery.Controls.Add(this.galeryTree, 0, 0);
            this.layoutGalery.Name = "layoutGalery";
            // 
            // supprItem
            // 
            resources.ApplyResources(this.supprItem, "supprItem");
            this.supprItem.Name = "supprItem";
            this.supprItem.UseVisualStyleBackColor = true;
            this.supprItem.Click += new System.EventHandler(this.supprItem_Click);
            // 
            // galeryTree
            // 
            resources.ApplyResources(this.galeryTree, "galeryTree");
            this.galeryTree.CheckBoxes = true;
            this.galeryTree.Name = "galeryTree";
            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.layoutMain);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ExtractionProcessInfo.ResumeLayout(false);
            this.ExtractionProcessInfo.PerformLayout();
            this.ProcessInfos.ResumeLayout(false);
            this.FilesToProcess.ResumeLayout(false);
            this.TabLogs.ResumeLayout(false);
            this.layoutExtract.ResumeLayout(false);
            this.layoutExtractButtons.ResumeLayout(false);
            this.ExtractionPanel.ResumeLayout(false);
            this.ExtractionPanel.PerformLayout();
            this.ExtractionDetails.ResumeLayout(false);
            this.layoutExtractOptions.ResumeLayout(false);
            this.layoutExtractOptions.PerformLayout();
            this.layoutPath.ResumeLayout(false);
            this.layoutPath.PerformLayout();
            this.layoutMain.ResumeLayout(false);
            this.layoutGalery.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button StartExtraction;
        private System.Windows.Forms.CheckBox ExtractVideos;
        private System.Windows.Forms.CheckBox ExtractSubtitles;
        private System.Windows.Forms.FolderBrowserDialog browseFolders;
        private System.Windows.Forms.Button ChoseFolder;
        private System.Windows.Forms.TextBox PathFolder;
        private System.Windows.Forms.ProgressBar ProgressBar;
        private System.Windows.Forms.Label label;
        private System.Windows.Forms.TableLayoutPanel ExtractionProcessInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label SubtitlesCount;
        private System.Windows.Forms.TextBox ProcessingFileName;
        private System.ComponentModel.BackgroundWorker BackgroundWorker;
        private System.Windows.Forms.TabControl ProcessInfos;
        private System.Windows.Forms.TabPage FilesToProcess;
        private System.Windows.Forms.TabPage TabLogs;
        private System.Windows.Forms.TreeView ListToProcess;
        private System.Windows.Forms.ListBox LogEvents;
        private System.Windows.Forms.Label VideosCount;
        private System.Windows.Forms.CheckBox ShowDetails;
        private System.Windows.Forms.TableLayoutPanel layoutExtract;
        private System.Windows.Forms.TableLayoutPanel ExtractionPanel;
        private System.Windows.Forms.TableLayoutPanel ExtractionDetails;
        private System.Windows.Forms.TableLayoutPanel layoutExtractOptions;
        private System.Windows.Forms.TableLayoutPanel layoutExtractButtons;
        private System.Windows.Forms.Button showGalery;
        private System.Windows.Forms.TableLayoutPanel layoutPath;
        private System.Windows.Forms.TableLayoutPanel layoutMain;
        private System.Windows.Forms.TableLayoutPanel layoutGalery;
        private System.Windows.Forms.TreeView galeryTree;
        private System.Windows.Forms.Button supprItem;
    }
}

