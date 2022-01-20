using System;
using System.Collections.Generic;
using System.Data;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrainInfo;
using TrainInfo.Stations;
using TrainInfo.ExtensionMethods;
using System.Diagnostics;

namespace TrainInfoWPF.TabUI.StationDataViewer
{
    /// <summary>
    /// StationDataView.xaml の相互作用ロジック
    /// </summary>
    public partial class StationDataView : UserControl
    {
        public TrainDataFile TrainDataFile { get; }

        public async static Task<StationDataView> InitializeWithDialog()
        {
            StationDataSelectDialog stationDataSelectDialog = new StationDataSelectDialog();
            stationDataSelectDialog.ShowDialog();

            var station = stationDataSelectDialog.SelectedStation;
            if (station is null)
            {
                return null;
            }

            var trainData = await TrainInfoReader.GetTrainDataAsync(station.StationID);
            StationDataView stationDataView = new StationDataView(trainData);
            return stationDataView;
        }

        public StationDataView(TrainDataFile trainDataFile)
        {
            TrainDataFile = trainDataFile;
            InitializeComponent();
            SetData();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void SetData()
        {
            BuildGridData(TrainDataFile.DepartureTrainDatas, false, DeparturesGrid);
            BuildGridData(TrainDataFile.ArrivalTrainDatas, true, ArrivalsGrid);
        }

        private static void BuildGridData(IEnumerable<KeyValuePair<JrhDestType, IReadOnlyList<TrainData>>> data, bool isArrival, Grid grid)
        {
            var i = 0;
            grid.Children.Clear();

            foreach (var (dest, trainData) in data)
            {
                var textBlock = new TextBlock()
                {
                    Text = $"{dest.GetName()}",
                    VerticalAlignment = VerticalAlignment.Center,
                };

                DataGrid dataGrid = BuildDataGrid(trainData, isArrival);

                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });
                grid.RowDefinitions.Add(new RowDefinition());

                Grid.SetRow(textBlock, i);
                Grid.SetRow(dataGrid, i + 1);

                grid.Children.Add(textBlock);
                grid.Children.Add(dataGrid);

                i += 2;
            }
        }

        private static DataGrid BuildDataGrid(IEnumerable<TrainData> trainData, bool isArrival)
        {
            DataGridBuilder dataGridBuilder = new DataGridBuilder();

            var arrString = $"{(isArrival ? "到着" : "出発")}時刻";

            dataGridBuilder.AddColumn("列車名", trainData.Select(td => td.Name.ToString()).ToArray(), 2);
            dataGridBuilder.AddColumn("行先", trainData.Select(td => td.Destination.Name).ToArray(), 1);
            dataGridBuilder.AddColumn(arrString, trainData.Select(td => td.Time.ToString("HH:mm")).ToArray(), 1);
            dataGridBuilder.AddColumn("現在地", trainData.Select(td => td.NowPosition?.ToString() ?? "出発前").ToArray(), 2);
            dataGridBuilder.AddColumn("運行状況", trainData.Select(td => td.Condition.ToString()).ToArray(), 2);

            return dataGridBuilder.Build();
        }
    }

    internal static class ExtMethod
    {
        public static void Deconstruct<TKey, TValue>(this KeyValuePair<TKey, TValue> kvp, out TKey key, out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }
    }
}
