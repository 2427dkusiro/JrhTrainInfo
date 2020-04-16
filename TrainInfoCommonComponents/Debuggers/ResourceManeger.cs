using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TrainInfo.ExtensionMethods;
using TrainInfo.Stations;

namespace TrainInfo.Debuggers
{
    /// <summary>
    /// 駅データの管理を提供します。
    /// </summary>
    public static class StationResourceManeger
    {
        /// <summary>
        /// 路線を指定して駅データをCSVに書き出します。
        /// </summary>
        /// <param name="jrhLine"></param>
        /// <returns></returns>
        public static string ToCsvData(JrhLine jrhLine)
        {
            var stringBuilder = new StringBuilder();

            foreach (var station in LineDataReader.GetStations(jrhLine))
            {
                stringBuilder.Append($"{station.StationID},{station.Name},{station.HiraName},{station.KataName},{station.EnglishName},{station.IsEndStation},{station.StationArea},{jrhLine.GetName()},{station.Position[jrhLine]}\n");
            }

            return stringBuilder.ToString();
        }

        /* 大して早くならないので手間が見合わない
        public static string GenerateStationInitCode(IEnumerable<Station> stations)
        {
            stations = StationData.Stations;

            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("var std = new[] {");

            foreach (var station in stations)
            {
                if (station is null)
                {
                    stringBuilder.Append("null,");
                }
                else
                {

                    StringBuilder dicBuilder = new StringBuilder();
                    dicBuilder.Append("new Dictionary<JrhLine, int>() {");

                    var dic = station.Position;

                    //Part1
                    foreach (var kvp in dic)
                    {
                        dicBuilder.Append($"{{JrhLine.{kvp.Key},{kvp.Value}}},");
                    }

                    //Part2
                    foreach (var kvp in dic)
                    {
                        dicBuilder.Append($"[JrhLine.{kvp.Key}]={kvp.Value},");
                    }

                    dicBuilder.Append("}");

                    stringBuilder.Append($" new Station({station.StationID},\"{station.Name}\",\"{station.HiraName}\",\"{station.KataName}\",\"{station.EnglishName}\"" +
                        $",{station.IsEndStation.ToString().ToLower()},{station.StationArea},{dicBuilder.ToString()}),");
                }
            }

            stringBuilder.Append("}");

            var result = stringBuilder.ToString();
            return result;
        }
        */

        /// <summary>
        /// すべての駅データを路線ごとに、指定したフォルダーにCSVとして書き出します。
        /// </summary>
        /// <param name="folder"></param>
        public static void ExportAllStaionsToCsvFile(string folder)
        {
            foreach (var lineObj in Enum.GetValues(typeof(JrhLine)))
            {
                var line = (JrhLine)lineObj;
                var path = $"{folder}\\{line.ToString()}.csv";

                using (var streamWriter = new StreamWriter(path, false, Encoding.UTF8))
                {
                    streamWriter.Write(ToCsvData(line));
                }
            }
        }

        /// <summary>
        /// CSV形式の駅データを読み取ります。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Station[] ReadCsvData(string[] data)
        {
            var stations = new Station[data.Length];
            for (var i = 0; i < data.Length; i++)
            {
                var split = data[i].Split(',');

                var id = int.Parse(split[0]);
                var isEnd = bool.Parse(split[5]);
                var area = int.Parse(split[6]);
                var jrhLine = JrhLineCreater.FromString(split[7]);
                var pos = int.Parse(split[8]);

                stations[i] = new Station(id, split[1], split[2], split[3], split[4], area, isEnd, jrhLine, pos);
            }
            return stations;
        }

        /// <summary>
        /// フォルダー内にあるCSV形式の駅データをすべて読み取ります。
        /// </summary>
        /// <param name="folderPath"></param>
        /// <returns></returns>
        public static Station[][] ReadCsvFiles(string folderPath)
        {
            var files = Directory.GetFiles(folderPath, "*.csv");
            var stationDatas = new List<Station[]>();

            foreach (var file in files)
            {
                using (var streamReader = new StreamReader(file, Encoding.UTF8))
                {
                    var lines = new List<string>();
                    while (true)
                    {
                        var line = streamReader.ReadLine();
                        if (line is null)
                        {
                            break;
                        }
                        else
                        {
                            lines.Add(line);
                        }
                    }
                    stationDatas.Add(ReadCsvData(lines.ToArray()));
                }
            }
            return stationDatas.ToArray();
        }

        /// <summary>
        /// 指定したフォルダーに、駅リソースをjson形式で書き出します。
        /// </summary>
        /// <param name="folderPath"></param>
        public static void MakeResources(string folderPath)
        {
            var data = ReadCsvFiles(folderPath);

            var stationArray = MapStationDatas(data);

            var stations = stationArray.Where(std => std != null);

            var StationNameDic = MakeDictionary(stations, std => std.Name);
            var StationKanaDic = MakeDictionary(stations, std => std.KataName);
            var StationHiraDic = MakeDictionary(stations, std => std.HiraName);
            var StationRomanDic = MakeDictionary(stations, std => std.EnglishName);
            var LineDataDic = data.Select(x => GetLineData(x)).ToDictionary(sel => sel.Item1, sel => sel.Item2);

            ResourceWriter(folderPath, stationArray, nameof(stationArray));
            ResourceWriter(folderPath, StationNameDic, nameof(StationNameDic));
            ResourceWriter(folderPath, StationKanaDic, nameof(StationKanaDic));
            ResourceWriter(folderPath, StationHiraDic, nameof(StationHiraDic));
            ResourceWriter(folderPath, StationRomanDic, nameof(StationRomanDic));
            ResourceWriter(folderPath, LineDataDic, nameof(LineDataDic));
        }

        private static void ResourceWriter<T>(string folder, T obj, string name)
        {
            var path = $"{folder}\\{name}.json";
            var str = JsonConvert.SerializeObject(obj);

            using (var streamWriter = new StreamWriter(path, false, Encoding.UTF8))
            {
                streamWriter.Write(str);
            }
        }

        private static Station[] MapStationDatas(IEnumerable<IEnumerable<Station>> data)
        {
            var stationList = new List<Station>();
            foreach (var stations in data)
            {
                foreach (var station in stations)
                {
                    Station st = default;
                    if (stationList.Any())
                    {
                        st = stationList.FirstOrDefault(sta => sta == station);
                    }
                    if (st == null)
                    {
                        stationList.Add(station);
                    }
                    else
                    {
                        st.Position.Add(station.Position.First().Key, station.Position.First().Value);
                    }
                }
            }

            var maxId = stationList.Max(std => std.StationID);
            var result = new Station[maxId + 1];

            foreach (var std in stationList)
            {
                result[std.StationID] = std;
            }

            return result;
        }

        private static Dictionary<T, int> MakeDictionary<T>(IEnumerable<Station> stations, Func<Station, T> func)
        {
            var result = new Dictionary<T, int>();
            foreach (var station in stations)
            {
                var key = func.Invoke(station);
                result.Add(key, station.StationID);
            }
            return result;
        }

        private static (JrhLine, int[]) GetLineData(Station[] data)
        {

            var jrhLine = data.First().Position.First().Key;
            if (data.Any(std => !std.Position.ContainsKey(jrhLine)))
            {
                throw new ArgumentException();
            }
            else
            {
                var result = data.Select(std => std.StationID).ToArray();
                return (jrhLine, result);
            }
        }
    }
}
