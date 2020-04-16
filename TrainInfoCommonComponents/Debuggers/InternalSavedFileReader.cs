using System.IO;

namespace TrainInfo.Debuggers
{
    /// <summary>
    /// 埋め込みリソースとして保存された運行情報データを読み込む列車データソースです。
    /// </summary>
    public class InternalSavedDataReader : TrainInfoReader.ITrainDataSource
    {
        /// <summary>
        /// IDを指定して列車データを読み取ります。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetTrainDataAsync(int id)
        {
            return Properties.Resources.ResourceManager.GetString($"_{id}");
        }

        /// <summary>
        ///ファイル名を埋め込みリソース用に変更します。
        /// </summary>
        /// <param name="folder"></param>
        public void ChangeFileName(string folder)
        {
            var files = Directory.GetFiles(folder);
            foreach (var file in files)
            {
                if (SaveFileReader.TryGetIdInFileName(Path.GetFileName(file), out var id))
                {
                    File.Move(file, $"{folder}\\{id}.json");
                }
            }
        }
    }
}
