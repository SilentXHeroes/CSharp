using System;
using System.Collections.Generic;
using System.IO;
using Helpers;

namespace GaleryParseContent
{
    class Model : Database
    {
        public List<Dictionary<string, dynamic>> getAllItems()
        {
            FileWriter.WriteLog.WriteLine("GET ALL ITEMS");
            return select("folder.id_folder, id_item, CASE WHEN id_subtitle IS NULL THEN '' ELSE id_subtitle END as id_subtitle, name_folder, noSeason_item, noEpisode_item, CASE WHEN name_subtitle IS NULL THEN '' ELSE name_subtitle END as name_subtitle")
                    .from("folder")
                    .join("item", "folder.id_folder = item.id_folder")
                    .join("subtitle", "item.id_item = subtitle.iditem_subtitle")
                    .order_by("name_folder, noSeason_item, noEpisode_item, name_subtitle")
                    .fetch_array();
        }

        public List<Dictionary<string,dynamic>> getFolders()
        {
            return select("*").from("folder").fetch_array();
        }

        public List<Dictionary<string, dynamic>> getVideos()
        {
            return select("*").from("item").fetch_array();
        }

        public List<Dictionary<string,dynamic>> getSubtitles()
        {
            return select("*").from("subtitle").fetch_array();
        }
        
        public int getVideo(int folder, int season, int episode)
        {
            Dictionary<string,dynamic> item = select("id_item")
                    .from("item")
                    .where("id_folder", folder)
                    .where("noSeason_item", season)
                    .where("noEpisode_item", episode)
                    .fetch();

            if (item.Count == 0) return -1;

            return item["id_item"];
        }

        public List<Dictionary<string, dynamic>> getElement(int idFolder = -1, int noSeason = -1, int idItem = -1, int idSubtitle = -1)
        {
            if (idFolder > -1) where("id_folder", idFolder);
            if (noSeason > -1) where("noSeason_item", noSeason);
            if (idItem > -1) where("id_item", idItem);
            if (idSubtitle > -1) where("id_subtitle", idSubtitle);

            return select("*")
                .from("folder")
                .join("item", "folder.id_folder = item.id_folder")
                .join("subtitle", "item.id_item = subtitle.iditem_subtitle")
                .order_by("name_folder, noSeason_item, noEpisode_item, name_subtitle")
                .fetch_array(); ;
        }

        public int getVideoByName(int idFolder, string name)
        {
            Database db = select("id_item").from("item").where("filename_item", name);

            if (idFolder > 0) db.where("id_folder", idFolder);

            Dictionary<string, dynamic> item = db.fetch();

            if (item.Count == 0) return -1;

            return item["id_item"];
        }

        public void DeleteFolder(int idFolder)
        {
            ExecuteStatement("DELETE FROM folder WHERE id_folder = " + idFolder);
            DirectoryInfo dirVideos = new DirectoryInfo(Helper.App + $@"Videos\{ idFolder }");
            DirectoryInfo dirSubtitles = new DirectoryInfo(Helper.App + $@"Subtitles\{ idFolder }");
            dirVideos.Delete(true);
            dirSubtitles.Delete(true);
        }

        public void DeleteItem(int idItem = -1)
        {
            ExecuteStatement($"DELETE FROM item WHERE id_item = " + idItem);
            //DirectoryInfo dirVideos = new DirectoryInfo(Helper.App + $@"Videos\{ idFolder }");
            //DirectoryInfo dirSubtitles = new DirectoryInfo(Helper.App + $@"Subtitles\{ idFolder }");
            //dirVideos.Delete(true);
        }

        public void DeleteSeason(int idFolder, int noSeason)
        {
            ExecuteStatement($"DELETE FROM item WHERE noSeason_item = { noSeason } AND id_folder = { idFolder }");
        }

        public void DeleteSubtitle(int idSubtitle)
        {
            ExecuteStatement("DELETE FROM subtitle WHERE id_subtitle = " + idSubtitle);
        }
    }
}
