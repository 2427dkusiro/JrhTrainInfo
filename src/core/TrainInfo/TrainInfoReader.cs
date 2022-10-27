using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrainInfo.Stations;
using TrainInfo.TrainDatas;
using TrainInfo.TrainPositions;

namespace TrainInfo
{
    /// <summary>
    /// 列車運行情報の取得を提供します。
    /// </summary>
    public static class TrainInfoReader
    {
        static TrainInfoReader() { }


        public static async Task<string> GetTrainDataJsonAsync(int id)
        {
            return await TrainDataGeter.GetTrainDataJsonAsync(id);
        }

        /// <summary>
        /// 駅 ID を指定して列車情報を取得します。エリア外の駅の場合広域データから列車情報を返します。
        /// </summary>
        /// <param name="id">取得対象の駅 ID。</param>
        /// <returns>解析済みの列車情報。</returns>
        public static async Task<TrainDataFile> GetTrainDataAsync(int id)
        {
            var station = StationReader.GetStationById(id);
            if (station.StationArea != 0)
            {
                return await GetSpecialTrainDataAsync(station);
            }

            var data = await TrainDataGeter.GetTrainDataJsonAsync(id);

            return await Task.Run(() =>
            {
                var rawTrainDataFile = JsonDeserializer.DeserializeTrainInfo(data);
                return TrainFilePaeser.ParseRawTrainDataFile(rawTrainDataFile);
            });
        }

        /// <summary>
        /// 保存済み駅データから列車情報を取得します。エリア外駅の場合の呼び分けは行われないことに留意してください。
        /// </summary>
        /// <param name="savedData"></param>
        /// <returns></returns>
        public static async Task<TrainDataFile> GetTrainDataAsync(string savedData)
        {
            return await Task.Run(() =>
            {
                var rawTrainDataFile = JsonDeserializer.DeserializeTrainInfo(savedData);
                return TrainFilePaeser.ParseRawTrainDataFile(rawTrainDataFile);
            });
        }

        internal static async Task<TrainDataFile> GetSpecialTrainDataAsync(Station station)
        {
            var data = await TrainDataGeter.GetTrainDataJsonAsync(station.StationArea);
            return await Task.Run(() =>
            {
                var rawTrainDataFiles = JsonDeserializer.DeserializeSpecialTrainInfo(data);
                return TrainFilePaeser.ParseRawTrainDataFile(rawTrainDataFiles.First(rtdf => rtdf.StationId == station.StationId));
            });
        }

        /// <summary>
        /// Kitacaエリア外の一括形式の駅データを読み取ります。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static async Task<TrainDataFile[]> GetSpecialTrainDatasAsync(int id)
        {
            var data = await TrainDataGeter.GetTrainDataJsonAsync(id);
            return await Task.Run(() =>
            {
                var rawTrainDataFiles = JsonDeserializer.DeserializeSpecialTrainInfo(data);
                return rawTrainDataFiles.Select(rstd => TrainFilePaeser.ParseRawTrainDataFile(rstd)).ToArray();
            });
        }

        internal static async Task<TrainDataFile[]> GetSpecialTrainDatasAsync(string savedData)
        {
            return await Task.Run(() =>
            {
                var rawTrainDataFiles = JsonDeserializer.DeserializeSpecialTrainInfo(savedData);
                return rawTrainDataFiles.Select(rstd => TrainFilePaeser.ParseRawTrainDataFile(rstd)).ToArray();
            });
        }

        /// <summary>
        /// 路線を指定してその路線上を走行するすべての列車データを取得します。
        /// </summary>
        /// <param name="jrhLine"></param>
        /// <returns>走行中の列車データ。</returns>
        public static async Task<(TrainPositionData arrivalData, TrainPositionData departureData)> GetTrainPositionDataAsync(JrhLine jrhLine)
        {
            return await TrainPositionGeter.GetTrainPosition(jrhLine, null);
        }

        /// <summary>
        /// 路線を指定してその路線上を走行するすべての列車データを取得します。
        /// </summary>
        /// <param name="jrhLine"></param>
        /// <returns>走行中の列車データ。</returns>
        public static async Task<(TrainPositionData arrivalData, TrainPositionData departureData)> GetTrainPositionDataAsync(JrhLine jrhLine, Dictionary<int, TrainDataFile> dataFile)
        {
            return await TrainPositionGeter.GetTrainPosition(jrhLine, dataFile);
        }

        /// <summary>
        /// 情報の取得処理を置き換えます。このメソッドはデバッグ用です
        /// </summary>
        public static void SetRedirect(ITrainDataSource trainDataSource)
        {
            TrainDataGeter.trainDataSource = trainDataSource ?? throw new ArgumentNullException(nameof(trainDataSource));
        }

        /// <summary>
        /// 情報の取得処理の置換をもとに戻します。
        /// </summary>
        public static void ClearRedirect()
        {
            TrainDataGeter.trainDataSource = null;
        }

        /// <summary>
        /// 列車データの取得ソースを表します。
        /// </summary>
        public interface ITrainDataSource
        {
            string GetTrainDataAsync(int id);
        }
    }
}
