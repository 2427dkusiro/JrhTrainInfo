using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainInfo.ExtensionMethods;
using TrainInfo.Stations;

namespace TrainInfo.Debuggers
{
    public class SaveFileReader : TrainInfoReader.ITrainDataSource
    {
        public string FolderPath { get; }

        private readonly string[] Files;
        private static readonly Encoding encoding = Encoding.UTF8;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderPath"></param>
        /// <exception cref="ArgumentException" />
        public SaveFileReader(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                throw new ArgumentException("フォルダーが存在しません", nameof(folderPath));
            }
            else
            {
                FolderPath = folderPath;
                Files = Directory.GetFiles(folderPath, "*.json");
            }
        }

        public string GetTrainDataAsync(int id)
        {
            var path = Files.FirstOrDefault(str =>
            {
                var b = TryGetIdInFileName(Path.GetFileName(str), out var n);
                return b && (id == n);
            });

            using (var streamReader = new StreamReader(path, encoding))
            {
                return streamReader.ReadToEnd();
            }
        }

        public static async Task<TrainDataFile> ReadJsonDataAsync(string path)
        {
            using (var streamReader = new StreamReader(path, encoding))
            {
                return await TrainInfoReader.GetTrainDataAsync(streamReader.ReadToEnd());
            }
        }

        public static async Task<TrainDataFile[]> ReadSpecialJsonDataAsync(string path)
        {
            using (var streamReader = new StreamReader(path, encoding))
            {
                return await TrainInfoReader.GetSpecialTrainDatasAsync(streamReader.ReadToEnd());
            }
        }

        public static bool TryGetIdInFileName(string fileName, out int id)
        {
            var str = fileName.GetRangeWithEnd(' ');
            if (int.TryParse(str, out var n))
            {
                id = n;
                return true;
            }
            var res = StationReader.GetStationByName(str);
            id = res?.StationId ?? default;
            return res != null;
        }
    }
}
