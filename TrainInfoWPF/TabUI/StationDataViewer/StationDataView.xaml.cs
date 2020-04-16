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

namespace TrainInfoWPF.TabUI.StationDataViewer
{
    /// <summary>
    /// StationDataView.xaml の相互作用ロジック
    /// </summary>
    public partial class StationDataView : UserControl
    {
        public TrainDataFile TrainDataFile { get; }

        public StationDataView(TrainDataFile trainDataFile)
        {
            TrainDataFile = trainDataFile;
            InitializeComponent();
        }

        private void SetData()
        {
            var depGrid = BuildGridData(TrainDataFile.DepartureTrainDatas, false);
            DeparturesGrid = depGrid;

            var arrGrid = BuildGridData(TrainDataFile.ArrivalTrainDatas, true);
            ArrivalsGrid = arrGrid;
        }

        private static Grid BuildGridData(IEnumerable<KeyValuePair<JrhDestType, IReadOnlyList<TrainData>>> data, bool isArrival)
        {
            var i = 0;
            Grid grid = new Grid();

            foreach (var (dest, trainData) in data)
            {
                var textBlock = new TextBlock()
                {
                    Text = $"{dest.GetName()}",
                    VerticalAlignment = VerticalAlignment.Center,
                };

                DataTable dataTable = BuildDataTable(trainData, isArrival);

                DataGrid dataGrid = new DataGrid();
                dataGrid.Columns.Add(new DataGridTextColumn() { Width = 2 });
                dataGrid.Columns.Add(new DataGridTextColumn() { Width = 1 });
                dataGrid.Columns.Add(new DataGridTextColumn() { Width = 1 });
                dataGrid.Columns.Add(new DataGridTextColumn() { Width = 2 });
                dataGrid.Columns.Add(new DataGridTextColumn() { Width = 2 });


                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });
                grid.RowDefinitions.Add(new RowDefinition());

                Grid.SetRow(textBlock, i);
                Grid.SetRow(dataGrid, i + 1);

                grid.Children.Add(textBlock);
                grid.Children.Add(dataGrid);

                i += 2;
            }

            return grid;
        }

        private static DataTable BuildDataTable(IEnumerable<TrainData> trainData, bool isArrival)
        {
            DataTableBuilder dataTableBuilder = new DataTableBuilder();

            var arrString = $"{(isArrival ? "到着" : "出発")}時刻";

            dataTableBuilder.AddColumn("列車名", trainData.Select(td => td.Name.ToString()).ToArray());
            dataTableBuilder.AddColumn("行先", trainData.Select(td => td.Destination.Name).ToArray());
            dataTableBuilder.AddColumn(arrString, trainData.Select(td => td.Time.ToString("HH:mm")).ToArray());
            dataTableBuilder.AddColumn("現在地", trainData.Select(td => td.NowPosition?.ToString() ?? "出発前").ToArray());
            dataTableBuilder.AddColumn("運行状況", trainData.Select(td => td.Condition.ToString()).ToArray());

            return dataTableBuilder.Build();
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
