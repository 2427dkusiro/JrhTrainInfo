using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainInfo.Stations;
using TrainInfo.TrainDatas;

namespace TrainInfo.Debuggers
{
    public class SaveFileWriter
    {
        public string FolderPath { get; }

        private static readonly Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <exception cref="ArgumentException" />
        public SaveFileWriter(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new ArgumentException("フォルダーが存在しません", nameof(folderPath));
            }
            else
            {
                FolderPath = folderPath;
            }
        }

        public async Task SaveTrainData(int id)
        {
            var jsonData = await TrainInfoReader.GetTrainDataJsonAsync(id);

            TrainDataFile td = null;
            if (id < 5)
            {
                td = await TrainInfoReader.GetTrainDataAsync(jsonData);
            }
            else
            {
                td = (await TrainInfoReader.GetSpecialTrainDatasAsync(jsonData)).FirstOrDefault();
            }

            var path = $"{FolderPath}\\{GetSaveFileName(id, td?.GetedDateTime ?? DateTime.Now)}.json";

            SaveJsonData(path, jsonData);
        }

        public static void SaveJsonData(string path, string data)
        {
            using (var streamWriter = new StreamWriter(path, false, encoding))
            {
                streamWriter.Write(data);
            }
        }

        public static string GetSaveFileName(int id)
        {
            string defaultFileName;
            if (id >= 1 && id < 5)
            {
                defaultFileName = $"{id} {DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}";
            }
            else
            {
                defaultFileName = $"{StationReader.GetStationById(id).Name} {DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}";
            }
            return defaultFileName;
        }

        public static string GetSaveFileName(int id, DateTime dateTime)
        {
            string defaultFileName;
            if (id >= 1 && id < 5)
            {
                defaultFileName = $"{id} {dateTime.ToString("yyyy-MM-dd_HH-mm-ss")}";
            }
            else
            {
                defaultFileName = $"{StationReader.GetStationById(id).Name} {dateTime.ToString("yyyy-MM-dd_HH-mm-ss")}";
            }
            return defaultFileName;
        }

    }
}
