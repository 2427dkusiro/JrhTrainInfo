using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
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
using TrainInfo;
using TrainInfo.Debuggers;
using TrainInfo.SaveDatas;
using TrainInfo.Stations;

namespace TrainInfoWPF.TabUI.StationDataViewer
{
    /// <summary>
    /// StationDataSelectDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class StationDataSelectDialog : Window
    {
        public StationDataSelectDialog()
        {
            InitializeComponent();
        }

        public TrainDataFile SelectedSource { get; set; }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var station = StationReader.GetStationByName(StationSerchTextBox.Text);
            if (station is null)
            {
                MessageBox.Show($"{StationSerchTextBox.Text}は見つかりませんでした");
            }
            else
            {
                SelectedSource = await TrainInfoReader.GetTrainDataAsync(station.StationId);
                this.Close();
            }
        }

        private async void OpenLocalFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Jsonファイル(*.json)|*.json";
            openFileDialog.Title = "開く列車情報を選択";
            openFileDialog.ShowDialog();

            var file = openFileDialog.FileName;
            var trainDataFile = TryPaeseTrainDataFile(file);
            if (trainDataFile != null)
            {
                this.SelectedSource = trainDataFile;
                this.Close();
            }
            else
            {
                var data = await SaveFileReader.ReadJsonDataAsync(file);
                this.SelectedSource = data;
                this.Close();
            }
        }

        private static TrainDataFile TryPaeseTrainDataFile(string filePath)
        {
            try
            {
                string str = "";
                using (StreamReader streamReader = new StreamReader(filePath, Encoding.UTF8))
                {
                    str = streamReader.ReadToEnd();
                }
                var result = JsonConvert.DeserializeObject<AnalyzedTrainFile>(str);
                var res = result.ToTrainDataFile();
                return res;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool? IsAreaData(string filePath)
        {
            if (SaveFileReader.TryGetIdInFileName(filePath, out var id))
            {
                return (id > 0 && id < 6);
            }
            else
            {
                return null;
            }
        }
    }

    /*
    /// <summary>
    /// 列車データの取得元を表現する抽象クラスです。
    /// </summary>
    public abstract class TrainDataSourse
    {
        public abstract Task<TrainDataFile> GetTrainDataFile();
    }

    /// <summary>
    /// 取得対象の <see cref="TrainInfo.Stations.Station"/> の指定による列車データ取得元を示します。 
    /// </summary>
    public class StationDataTrainDataSource : TrainDataSourse
    {
        public StationDataTrainDataSource(Station station) => Station = station ?? throw new ArgumentNullException(nameof(station));

        public Station Station { get; }

        public override Task<TrainDataFile> GetTrainDataFile()
        {
            return TrainInfoReader.GetTrainDataAsync(Station.StationID);
        }
    }

    /// <summary>
    /// ローカルに保存されたファイルによる列車データ取得元を示します。
    /// </summary>
    public class LocalFileTrainDataSource : TrainDataSourse
    {
        public LocalFileTrainDataSource(string filePath) => FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

        public string FilePath { get; }

        public override Task<TrainDataFile> GetTrainDataFile() => SaveFileReader.ReadJsonDataAsync(FilePath);
    }

    public class LocalAreaFileTrainDataSource : TrainDataSourse
    {
        public LocalAreaFileTrainDataSource(string filePath) => FilePath = filePath ?? throw new ArgumentNullException(nameof(filePath));

        public string FilePath { get; }

        public override Task<TrainDataFile> GetTrainDataFile() => Task.Run(async () => (await SaveFileReader.ReadSpecialJsonDataAsync(FilePath)).First());
    }

    public class TrainDataFileObjectDataSource : TrainDataSourse
    {
        public TrainDataFileObjectDataSource(TrainDataFile trainDataFile) => TrainDataFile = trainDataFile ?? throw new ArgumentNullException(nameof(trainDataFile));

        public TrainDataFile TrainDataFile { get; }

        public override Task<TrainDataFile> GetTrainDataFile()
        {
            return Task.Run(() => TrainDataFile);
        }
    }
    */
}
