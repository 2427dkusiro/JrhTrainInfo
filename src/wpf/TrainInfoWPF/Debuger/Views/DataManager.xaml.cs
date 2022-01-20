using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Forms;
using TrainInfo;
using TrainInfo.Debuggers;
using TrainInfo.SaveDatas;
using TrainInfo.Stations;
using TrainInfoWPF.Debuger.Views;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace TrainInfoWPF.Debuger
{
    /// <summary>
    /// DataManager.xaml の相互作用ロジック
    /// </summary>
    public partial class DataManager : Window
    {
        private string redirectFolder;

        public DataManager() => InitializeComponent();

        private void StationIdTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (StationIdTextBox.Text == "駅IDまたは駅名を入力...")
            {
                StationIdTextBox.Text = "";
            }
        }

        private async void OpenJsonButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Jsonファイル|*.json"
            };
            openFileDialog.ShowDialog();
            var fileName = openFileDialog.FileName;

            if (SaveFileReader.TryGetIdInFileName(Path.GetFileName(fileName), out var n))
            {
                if (n > 0 && n < 6)
                {
                    var data = await SaveFileReader.ReadSpecialJsonDataAsync(fileName);

                    var trainDataFileViewer = new TrainDataFileViewer();
                    trainDataFileViewer.RenderData(data.First());
                    trainDataFileViewer.Show();
                }
                else
                {
                    var data = await SaveFileReader.ReadJsonDataAsync(fileName);

                    var trainDataFileViewer = new TrainDataFileViewer();
                    trainDataFileViewer.RenderData(data);
                    trainDataFileViewer.Show();
                }
            }
        }

        private void ViewJsonButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new System.Windows.Forms.OpenFileDialog
            {
                Filter = "Jsonファイル|*.json"
            };
            openFileDialog.ShowDialog();
            var fileName = openFileDialog.FileName;

            using (var streamReader = new StreamReader(fileName, Encoding.UTF8))
            {
                var data = JsonConvert.DeserializeObject<AnalyzedTrainFile>(streamReader.ReadToEnd());
                var trainDataFileViewer = new TrainDataFileViewer();
                trainDataFileViewer.RenderData(data.ToTrainDataFile());
                trainDataFileViewer.Show();
            }
        }

        private async void SaveJsonButton_Click(object sender, RoutedEventArgs e)
        {
            if (TryGetStationId(StationIdTextBox.Text, out var id))
            {
                var saveFileDialog = new System.Windows.Forms.SaveFileDialog()
                {
                    FileName = SaveFileWriter.GetSaveFileName(id),
                    Filter = "jsonファイル|*.json",
                };
                saveFileDialog.ShowDialog();

                var path = saveFileDialog.FileName;
                var data = await TrainInfoReader.GetTrainDataJsonAsync(id);

                SaveFileWriter.SaveJsonData(path, data);
            }
            else
            {
                MessageBox.Show("駅IDの入力が正しくありません。");
            }
        }

        private async void SaveAllEndButton_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog()
            {
                Description = "保存先フォルダを選択",
            };
            folderBrowserDialog.ShowDialog();
            var folder = folderBrowserDialog.SelectedPath;

            var saveFileWriter = new SaveFileWriter(folder);
            foreach (var std in StationReader.GetAllEnds())
            {
                await saveFileWriter.SaveTrainData(std.StationId);
            }
        }

        private async void PreviewCurrentButton_Click(object sender, RoutedEventArgs e)
        {
            if (TryGetStationId(StationIdTextBox.Text, out var n))
            {
                try
                {
                    if (n > 5 || n == 1)
                    {
                        var data = await TrainInfoReader.GetTrainDataAsync(n);
                        var trainDataFileViewer = new TrainDataFileViewer();
                        trainDataFileViewer.RenderData(data);
                        trainDataFileViewer.Show();
                    }
                    else
                    {
                        var data = await TrainInfoReader.GetSpecialTrainDatasAsync(n);
                        var firstData = data.First();
                        var trainDataFileViewer = new TrainDataFileViewer();
                        trainDataFileViewer.RenderData(firstData);
                        trainDataFileViewer.Show();
                    }
                }
                catch (System.Net.Http.HttpRequestException ex)
                {
                    var result = MessageBox.Show($"データ取得中にエラーが発生しました。駅IDが不正な可能性があります。{Environment.NewLine}エラーの種類：{ex.ToString()}{Environment.NewLine}処理を続行しますか?", "エラー処理", MessageBoxButton.YesNo, MessageBoxImage.Error);
                    if (result == MessageBoxResult.No)
                    {
                        throw;
                    }
                }
            }
            else
            {
                MessageBox.Show("駅IDの入力が正しくありません。");
            }
        }

        private void DoRedirectCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(redirectFolder))
            {
                TrainInfoReader.SetRedirect(new SaveFileReader(redirectFolder));
            }
            else
            {
                MessageBox.Show("リダイレクト元フォルダが存在しません");
                DoRedirectCheckBox.IsChecked = false;
            }
        }

        private void DoRedirectCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            TrainInfoReader.ClearRedirect();
        }

        private void ChangeRedirectSourceButton_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog()
            {
                Description = "リダイレクト元フォルダの選択",
            };
            folderBrowserDialog.ShowDialog();

            var folder = folderBrowserDialog.SelectedPath;
            DoRedirectCheckBox.IsChecked = false;

            redirectFolder = folder;
            RedirectSourceTextBox.Text = folder;
        }

        private bool TryGetStationId(string text, out int result)
        {
            if (!int.TryParse(text, out var n))
            {
                var station = StationReader.GetStationByName(text);
                if (station is null)
                {
                    result = default;
                    return false;
                }
                else
                {
                    result = station.StationId;
                    return true;
                }
            }
            if (n > 0 && n < 5)
            {
                result = n;
                return true;
            }
            else if (StationReader.GetStationById(n) is null)
            {
                result = default;
                return false;
            }
            else
            {
                result = n;
                return true;
            }
        }

        private void OpenSaveToolButton_Click(object sender, RoutedEventArgs e)
        {
            var autoDataLoader = new AutoDataLoader();
            autoDataLoader.Show();
        }

        private async void MargeTrainDataButton_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = "結合対象の運行情報データがあるフォルダーを選択",
            };
            folderBrowserDialog.ShowDialog();

            var directories = Directory.GetDirectories(folderBrowserDialog.SelectedPath);
            var directoryDatas = await Task.WhenAll(directories.Select(async dir => await Task.WhenAll(Directory.GetFiles(dir, "*.json")
            .Select(file => SaveFileReader.ReadJsonDataAsync(file)))));

            var dict = GetTrainDataDictionary(directoryDatas);
            var result = dict.Select(kvp => TrainDataFile.MargeTrainDataFile(kvp.Value.ToArray()));

            var folderBrowserDialog2 = new FolderBrowserDialog()
            {
                Description = "結合結果の保存先を選択",
            };
            folderBrowserDialog2.ShowDialog();

            foreach (var data in result)
            {
                using (var streamWriter = new StreamWriter
                    (folderBrowserDialog2.SelectedPath + $"\\{data.Station.Name} {data.GetedDateTime.ToString("yyyy-MM-dd")}.json", false, Encoding.UTF8))
                {
                    streamWriter.Write(JsonConvert.SerializeObject(AnalyzedTrainFile.FromTrainDataFile(data)));
                }
            }
        }

        private Dictionary<Station, List<TrainDataFile>> GetTrainDataDictionary(IEnumerable<IEnumerable<TrainDataFile>> trainDataFiles)
        {
            var dict = new Dictionary<Station, List<TrainDataFile>>();
            foreach (var dir in trainDataFiles)
            {
                foreach (var file in dir)
                {
                    if (dict.TryGetValue(file.Station, out var list))
                    {
                        list.Add(file);
                    }
                    else
                    {
                        dict.Add(file.Station, new List<TrainDataFile>(new[] { file }));
                    }
                }
            }
            return dict;
        }

        private async void MargeTrainDataSameFolder()
        {
            var openFileDialog = new OpenFileDialog()
            {

                Filter = "*.json",
                Multiselect = true,
            };
            openFileDialog.ShowDialog();

            var files = await Task.WhenAll(openFileDialog.FileNames.Select(file => TrainInfoReader.GetTrainDataAsync(file)));
            var margeData = TrainDataFile.MargeTrainDataFile(files);

            var str = JsonConvert.SerializeObject(margeData);
        }
    }
}
