using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ExtractGalery;
using FileWriter;

namespace Helpers
{
    public static class Helper
    {
        public static string App = @"F:\GaleryFolder\";
        public static string Folder = @"E:\Films - Séries\";
        public static BackgroundWorker Worker;
        public static DoWorkEventArgs Worker_DoWorkEvent;
        public static int ExtractionProcessState;

        /* Récupère la liste enfant, la crée si non définie */
        public static TreeNode getNode(dynamic parent, string text, dynamic value = null)
        {
            int idx = parent.Nodes.IndexOfKey(text);
            if (idx == -1)
            {
                idx = parent.Nodes.Count;
                AddTreeNode(parent, text, value);
            }
            return parent.Nodes[idx];
        }

        public static void AddTreeNode(dynamic parent, string text, dynamic value = null)
        {
            parent.Nodes.Add(new TreeNode
            {
                Text = text,
                Name = text,
                Tag = value
            });
        }

        public static void updateChildrenNodes(dynamic node, bool check)
        {
            foreach (TreeNode child in node.Nodes)
            {
                child.Checked = check;
                updateChildrenNodes(child, check);
            }
        }

        public static void updateParentNode(dynamic node, bool check = true)
        {
            dynamic parent = node.Parent;

            if (parent == null || node.Tag.ToString().IndexOf("SUBTITLE") > -1) return;

            bool allChecked = false;

            if (check)
            {
                allChecked = true;
                foreach (TreeNode child in parent.Nodes)
                {
                    if (!child.Checked)
                    {
                        allChecked = false;
                        break;
                    }
                }

                // Les parents sont déjà décochés, on ignore le traitement
                if (parent.Checked == false && allChecked == false) return;
            }

            parent.Checked = allChecked;
            updateParentNode(parent, allChecked);
        }

        public static void InitializeWorker(BackgroundWorker bw)
        {
            Worker = bw;
        }
        public static void InitializeDoWorkEvent(DoWorkEventArgs e)
        {
            Worker_DoWorkEvent = e;
        }

        public static string EncodeUTF8(string str)
        {
            str = str.Replace("â€™", "'");
            str = str.Replace("Ã®", "î");
            str = str.Replace("Ã¯", "ï");
            str = str.Replace("Ã©", "é");
            str = str.Replace("â€¦", "!");
            str = str.Replace("Ã¨", "è");
            str = str.Replace("Ã§", "ç");
            str = str.Replace("Ã´", "ô");
            str = str.Replace("Ã¢", "â");
            str = str.Replace("Ã¹", "ù");
            str = Regex.Replace(str, @"Ã\s", "à");
            str = str.Replace("Ã€", "À");
            str = str.Replace("Ã‡", "Ç");
            str = str.Replace("Ãª", "ê");
            str = str.Replace("Ã»", "û");
            str = str.Replace("Ã‰", "É");

            return str;
        }
    }
}
