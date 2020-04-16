using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using TrainInfo;
using TrainInfo.ExtensionMethods;
using TrainInfo.Stations;

namespace TrainInfoWPF.Debuger
{
    /// <summary>
    /// StationDataViewer.xaml の相互作用ロジック
    /// </summary>
    public partial class TrainDataFileViewer : Window
    {
        public TrainDataFileViewer() => InitializeComponent();

        public void RenderData(TrainDataFile trainDataFile)
        {
            Title = $"{trainDataFile.Station} - {trainDataFile.GetedDateTime.ToString()}";

            RenderGridData(trainDataFile.DepartureTrainDatas, false, DiparturesGrid);

            RenderGridData(trainDataFile.ArrivalTrainDatas, true, ArrivalsGrid);
        }


        private static void RenderGridData(IEnumerable<KeyValuePair<JrhDestType, IReadOnlyList<TrainData>>> data, bool isArrival, Grid grid)
        {
            var i = 0;
            foreach (var (dest, trainData) in data)
            {
                var label = new Label()
                {
                    Content = $"{dest.GetName()}",
                    VerticalAlignment = VerticalAlignment.Center,
                };

                var arrString = $"{(isArrival ? "到着" : "出発")}時刻";

                var dataGridGenerator = new DataGridGenerator();
                dataGridGenerator.AddColumn("列車名", trainData.Select(td => td.Name.ToString()).ToArray(), 2);
                dataGridGenerator.AddColumn("行先", trainData.Select(td => td.Destination.Name).ToArray(), 1);
                dataGridGenerator.AddColumn(arrString, trainData.Select(td => td.Time.ToString("HH:mm")).ToArray(), 1);
                dataGridGenerator.AddColumn("現在地", trainData.Select(td => td.NowPosition?.ToString() ?? "出発前").ToArray(), 2);
                dataGridGenerator.AddColumn("運行状況", trainData.Select(td => td.Condition.ToString()).ToArray(), 2);

                var dataGrid = dataGridGenerator.Finalize();

                grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(40) });
                grid.RowDefinitions.Add(new RowDefinition());

                Grid.SetRow(label, i);
                Grid.SetRow(dataGrid, i + 1);

                grid.Children.Add(label);
                grid.Children.Add(dataGrid);

                i += 2;
            }
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

    internal class DataGridGenerator
    {
        private readonly DataGrid dataGrid;
        private readonly DataTable dataTable;

        public DataGridGenerator()
        {
            dataGrid = new DataGrid()
            {
                AutoGenerateColumns = false,
                IsReadOnly = true,
            };
            dataTable = new DataTable();
        }

        private List<string[]> columns = new List<string[]>();
        public void AddColumn(string title, string[] column)
        {
            dataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = title,
                Binding = new Binding(title)
            });
            dataTable.Columns.Add(title, typeof(string));

            columns.Add(column);
        }

        public void AddColumn(string title, string[] column, double size)
        {
            dataGrid.Columns.Add(new DataGridTextColumn()
            {
                Header = title,
                Width = new DataGridLength(size, DataGridLengthUnitType.Star),
                Binding = new Binding(title)
            });
            dataTable.Columns.Add(title, typeof(string));

            columns.Add(column);
        }


        public DataGrid Finalize()
        {
            if (columns != null)
            {
                for (var i = 0; i < columns[0].Length; i++)
                {
                    var dataRow = dataTable.NewRow();
                    for (var j = 0; j < columns.Count; j++)
                    {
                        dataRow[j] = columns[j][i];
                    }
                    dataTable.Rows.Add(dataRow);
                }
            }

            dataGrid.DataContext = dataTable;
            dataGrid.SetBinding(DataGrid.ItemsSourceProperty, new Binding());
            return dataGrid;
        }
    }
}
