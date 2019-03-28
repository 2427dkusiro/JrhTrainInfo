using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using TrainInfo;
using TrainInfo.Models;
using TrainInfo.Stations;

namespace TrainInfoWPF.Debuger
{
    /// <summary>
    /// DataManager.xaml の相互作用ロジック
    /// </summary>
    public partial class DataManager : Window
    {
        public DataManager()
        {
            InitializeComponent();
        }

        private void OpenJsonButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFileDialog = new System.Windows.Forms.OpenFileDialog();
            openFileDialog.Filter = "Jsonファイル|*.json";
            openFileDialog.ShowDialog();
            string fileName = openFileDialog.FileName;
            string text;
            using (StreamReader streamReader = new StreamReader(fileName, Encoding.GetEncoding("Shift_JIS")))
            {
                text = streamReader.ReadToEnd();
            }

            var rawData = TrainInfoReader.DeserializeTrainInfo(text);
            var data = TrainInfoReader.ParseRawStationTrainData(rawData);
            TrainDataFileViewer trainDataFileViewer = new TrainDataFileViewer();
            trainDataFileViewer.RenderData(data);
            trainDataFileViewer.Show();
        }

        private async void SaveJsonButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(StationIdTextBox.Text, out var n))
            {
                string text = await TrainInfoReader.GetTrainDataJsonById(n);

                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog.Filter = "jsonファイル|*.json";
                saveFileDialog.ShowDialog();

                using (StreamWriter streamWriter = new StreamWriter(saveFileDialog.FileName))
                {
                    streamWriter.Write(text);
                }
            }
            else
            {
                MessageBox.Show("駅IDの入力が正しくありません。");
            }
        }

        private void StationIdTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            StationIdTextBox.Text = "";
        }

        private async void PreviewCurrentButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(StationIdTextBox.Text, out var n))
            {
                try
                {
                    if (n > 5 || n == 1)
                    {
                        var data = await TrainInfoReader.GetTrainDataById(n);
                        TrainDataFileViewer trainDataFileViewer = new TrainDataFileViewer();
                        trainDataFileViewer.RenderData(data);
                        trainDataFileViewer.Show();
                    }
                    else
                    {
                        var data = await TrainInfoReader.GetSpecialTrainDataById(n);
                        foreach(var td in data)
                        {
                            /*
                            TrainDataFileViewer trainDataFileViewer = new TrainDataFileViewer();
                            trainDataFileViewer.RenderData(td);
                            trainDataFileViewer.Show();
                            */
                        }
                    }
                }
                catch (System.Net.Http.HttpRequestException ex)
                {
                    var result = MessageBox.Show($"データ取得中にエラーが発生しました。駅IDが不正な可能性があります。{Environment.NewLine}エラーの種類：{ex.ToString()}{Environment.NewLine}処理を続行しますか?", "エラー処理", MessageBoxButton.YesNo, MessageBoxImage.Error);
                    if (result == MessageBoxResult.No)
                        throw;
                }
            }
            else
            {
                var station = StationReader.GetStationByName(StationIdTextBox.Text);
                if (station is null)
                    MessageBox.Show("駅IDの入力が正しくありません。");
                else
                {
                    var data = await TrainInfoReader.GetTrainDataById(station.StationID);
                    TrainDataFileViewer trainDataFileViewer = new TrainDataFileViewer();
                    trainDataFileViewer.RenderData(data);
                    trainDataFileViewer.Show();
                }
            }
        }
    }
}
