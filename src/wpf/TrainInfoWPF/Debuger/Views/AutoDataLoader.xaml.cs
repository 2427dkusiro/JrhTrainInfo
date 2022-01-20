using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TrainInfo;
using TrainInfo.Debuggers;
using TrainInfo.Stations;
using TrainInfo.TrainDatas;
using MessageBox = System.Windows.MessageBox;

namespace TrainInfoWPF.Debuger.Views
{
    /// <summary>
    /// AutoDataLoader.xaml の相互作用ロジック
    /// </summary>
    public partial class AutoDataLoader : Window
    {
        public AutoDataLoader() => InitializeComponent();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            StartTimeTextBox.Text = DateTime.Now.ToString();
        }

        private string path;
        private DateTime startTime;
        private int syuyoDelayTime;
        private int DelayTime;
        private CancellationTokenSource cancellationTokenSource;

        private async void DoButton_Click(object sender, RoutedEventArgs e)
        {
            var folderBrowserDialog = new FolderBrowserDialog()
            {
                Description = "データ保存先を選択してください"
            };
            folderBrowserDialog.ShowDialog();
            path = folderBrowserDialog.SelectedPath;
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (DateTime.TryParse(StartTimeTextBox.Text, out var time))
            {
                startTime = time;
            }
            else
            {
                MessageBox.Show("開始時刻の形式が正しくありません");
                return;
            }

            if (int.TryParse(SyuyoEkiTimeTextBox.Text, out var n))
            {
                syuyoDelayTime = n;
            }
            else
            {
                MessageBox.Show("時刻の形式が正しくありません");
                return;
            }

            if (int.TryParse(TsuzyoEkiTimeTextBox.Text, out var m))
            {
                DelayTime = m;
            }
            else
            {
                MessageBox.Show("時刻の形式が正しくありません");
                return;
            }

            Lock();

            LogText.WriteLine($"{path}　にデータを保存します...");

            var waitTime = (startTime - DateTime.Now);
            LogText.WriteLine($"{waitTime.ToString()}秒待機します...");
            await Task.Delay(waitTime);

            LogText.WriteLine("指定時刻になりました。情報取得プロセスを開始します...");

            cancellationTokenSource = new CancellationTokenSource();
            await Task.WhenAll(GetSyuyoData(cancellationTokenSource.Token), GetAllData(cancellationTokenSource.Token), GetEreaData(cancellationTokenSource.Token));
        }

        private void Lock()
        {
            StartTimeTextBox.IsEnabled = false;
            SyuyoEkiTimeTextBox.IsEnabled = false;
            TsuzyoEkiTimeTextBox.IsEnabled = false;
            DoButton.IsEnabled = false;
        }

        private async Task DataGeter(CancellationToken cancellationToken, IEnumerable<int> id, string saveFolderPath)
        {
            while (true)
            {
                var savePath = saveFolderPath + $"\\{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}";
                Directory.CreateDirectory(savePath);

                var saveFileWriter = new SaveFileWriter(savePath);
                await Task.Run(() =>
                {
                    Parallel.ForEach(id, async staId =>
                    {
                        try
                        {
                            await saveFileWriter.SaveTrainData(staId);
                            if (staId < 5)
                            {
                                LogText.WriteLine($"{DateTime.Now}: 広域データID{staId}を保存しました...");
                            }
                            else
                            {
                                LogText.WriteLine($"{DateTime.Now}: {StationReader.GetStationById(staId).Name}を保存しました...");
                            }
                        }
                        catch (TrainDataGetException ex)
                        {
                            LogText.WriteLine("通信中に例外が発生しました...");
                            throw;
                        }
                    });
                });

                LogText.WriteLine("取得プロセス一時停止(取得完了)...");
                await Task.Delay(DelayTime * 60 * 1000, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }


        private async Task GetSyuyoData(CancellationToken cancellationToken)
        {
            /*
            var query = StationReader.GetAllEnds().Where(std => std.StationArea == 0).Select(std => std.StationID);
            var savePath = path + "\\主要駅";

            await DataGeter(cancellationToken, query, savePath);
            */
        }

        private async Task GetAllData(CancellationToken cancellationToken)
        {
            var query = StationReader.GetAllStations().Where(std => std.StationArea == 0).Select(std => std.StationId);
            var savePath = path + "\\すべての駅";

            await DataGeter(cancellationToken, query, savePath);
        }

        private async Task GetEreaData(CancellationToken cancellationToken)
        {
            /*
            var query = Enumerable.Range(1, 4);
            var savePath = path + "\\広域データ";

            await DataGeter(cancellationToken, query, savePath);
            */
        }
    }
}
