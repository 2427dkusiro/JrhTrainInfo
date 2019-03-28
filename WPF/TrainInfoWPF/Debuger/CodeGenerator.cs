using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using TrainInfo;
using TrainInfo.Stations;
using TrainInfo.Models;
using Newtonsoft.Json;

namespace TrainInfoWPF.Debuger
{
    static class CodeGenerator
    {
        /// <summary>
        /// 駅データを初期化するコードを生成します。
        /// </summary>
        /// <param name="inputStations">生成元の駅データ。</param>
        /// <returns>駅データを初期化する代入式のC#コード。</returns>
        public static string GenerateStationCode(Station[] inputStations)
        {
            string result = "";
            result += "= new Station[]{";

            foreach (Station st in inputStations)
            {
                result += $"new Station({st.StationID},\"{st.Name}\",\"{st.HiraName}\",\"{st.KataName}\",\"{st.EnglishName}\",{st.IsEndStation.ToString().ToLower()},JrhLine.{st.Position.First().Key},{st.Position.First().Value}),";
            }
            result += "};";
            return result;
        }

        public static void ReadStationCsvFile(string path)
        {
            using (StreamReader streamReader = new StreamReader(path, Encoding.GetEncoding("Shift_JIS")))
            {
                List<Station> stations = new List<Station>();
                while (true)
                {
                    var line = streamReader.ReadLine()?.Split(',');

                    if (line == null)
                        break;

                    if (int.TryParse(line[0], out int n))
                    {
                        var lineString = line[1];
                        var StationName = line[2];
                        if (lineString.Contains("石勝"))
                            stations.Add(new Station(n, StationName, null, null, null, null));
                    }
                }
                stations.Insert(0, StationReader.GetStationByName("南千歳"));
                AddPositionData(stations.ToArray(), JrhLine.Sekisyo);
                string code = GenerateStationCode(stations.ToArray());
            }
        }

        /// <summary>
        /// 駅データに位置情報データを付加します。
        /// </summary>
        /// <param name="stations"></param>
        /// <returns></returns>
        public static void AddPositionData(Station[] stations, JrhLine jrhLine)
        {
            for (int i = 0; i < stations.Length; i++)
            {
                if (!stations[i].Position.ContainsKey(jrhLine))
                    stations[i].Position.Add(jrhLine, i + 1);
            }
        }
    }
}
