using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using TrainInfo.Models;
using TrainInfo.Spliters;
using TrainInfo.Stations;

namespace TrainInfo
{
    /// <summary>
    /// 列車データを表します。
    /// </summary>
    public class TrainData
    {
        /// <summary>
        /// 列車名に関わる情報を取得します。
        /// </summary>
        public TrainName Name { get; private set; }

        /// <summary>
        /// 列車の出発または到着の時刻を取得します。
        /// </summary>
        public DateTime Time { get; private set; }

        /// <summary>
        /// 列車の行先を取得します。
        /// </summary>
        public Station Destination { get; private set; }

        /// <summary>
        /// 列車の運行状況を取得します。
        /// </summary>
        public TrainCondition Condition { get; private set; }

        /// <summary>
        /// 列車の現在位置を取得します。
        /// </summary>
        public LineRange NowPosition { get; private set; }

        /// <summary>
        /// 列車名に関わる情報を設定します。
        /// </summary>
        /// <param name="trainNameString">列車名を表す文字列。</param>
        /// <param name="trainTypeString">列車種別を表す文字列。</param>
        public void SetTrainName(string trainNameString, string trainTypeString)
        {
            Name = TrainNameSpliter.SetTrainName(trainNameString, trainTypeString);
        }

        /// <summary>
        /// 列車の出発または到着の時刻を設定します。
        /// </summary>
        /// <param name="timeString">時刻を表す文字列。</param>
        /// <param name="nowHour">現在の時間。日付の補正に使用します。</param>
        public void SetTime(string timeString, int nowHour)
        {
            DateTime result = DateTime.Parse(timeString);
            if (nowHour < 5 && result.Hour > 5)
            {
                result = result.AddDays(-1);
            }
            else if (nowHour > 5 && result.Hour < 5)
            {
                result = result.AddDays(1);
            }
            Time = result;
        }

        /// <summary>
        /// 列車の行先を設定します。
        /// </summary>
        /// <param name="destinationString">行先を表す文字列。</param>
        public void SetDestination(string destinationString)
        {
            if (destinationString.Contains("("))
                destinationString = destinationString.GetRange('(', ')');
            if (destinationString.EndsWith("行"))
                Destination = StationReader.GetOrCreateStationByName(destinationString.Substring(0, destinationString.Length - 1));
            else
                Destination = StationReader.GetOrCreateStationByName(destinationString);
        }

        /// <summary>
        /// 列車の運行状況を設定します。
        /// </summary>
        /// <param name="conditionString">運行状況を表す番号。</param>
        /// <param name="suspendRangeStating">運休区間を表す文字列。</param>
        /// <param name="delayStatusString">遅延状況を表す文字列。</param>
        [Obsolete("Html版用メソッドです")]
        public void SetCondition(string conditionString, string suspendRangeStating = null, string delayStatusString = null)
        {
            Condition = TrainConditionSpliter.SetCondition(conditionString, suspendRangeStating, delayStatusString);
        }

        /// <summary>
        /// 列車の運行状況を設定します。
        /// </summary>
        /// <param name="condition">運行状況を表す番号。</param>
        /// <param name="suspendRangeStating">運休区間を表す文字列。</param>
        /// <param name="delayStatusString">遅延状況を表す文字列。</param>
        public void SetCondition(int condition, string suspendRangeStating = null, string delayStatusString = null)
        {
            Condition = TrainConditionSpliter.SetCondition(condition, suspendRangeStating, delayStatusString);
        }

        /// <summary>
        /// 列車の現在位置を設定します。
        /// </summary>
        /// <param name="trainPositionString">現在位置を表す文字列。</param>
        public void SetNowPosition(string trainPositionString)
        {
            if (trainPositionString == null)
                NowPosition = null;
            else
                NowPosition = TrainPositionSpliter.GetTrainPosition(trainPositionString);
        }

        /// <summary>
        /// このインスタンスを表す文字列を取得します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (NowPosition == null)
            {
                return $"{Name.ToString()} {Destination.Name}行 出発前";
            }
            else
            {
                return $"{Name.ToString()} {Destination.Name}行 {NowPosition.ToString()}走行中";
            }
        }
    }
}
