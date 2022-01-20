using System;
using System.Collections.Generic;
using System.Text;

using TrainInfo.ExtensionMethods;
using TrainInfo.Stations;

namespace TrainInfo
{
    public partial class TrainData
    {
        /// <summary>
        /// 列車の運行状況を表します。
        /// </summary>
        public class TrainCondition
        {
            /// <summary>
            /// <see cref="TrainCondition"/> クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="condition">列車運行ステータス。</param>
            /// <param name="suspendRange">運休区間。</param>
            /// <exception cref="ArgumentException">引数の組がサポートされない場合にスローされる例外。</exception> 
            public TrainCondition(TrainConditions condition, LineRange suspendRange = null)
            {
                Condition = condition;
                SuspendRange = suspendRange;
                DelayTimeMin = null;
                DelayTimeMax = null;
            }

            /// <summary>
            /// <see cref="TrainCondition"/> クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="condition">列車運行ステータス。</param>
            /// <param name="delayTimeMin">遅延時間の下限。</param>
            /// <param name="delayTimeMax">遅延時間の上限。</param>
            /// <exception cref="ArgumentException">引数の組がサポートされない場合にスローされる例外。</exception> 
            public TrainCondition(TrainConditions condition, int delayTimeMin, int delayTimeMax)
            {
                Condition = condition;
                SuspendRange = null;
                DelayTimeMin = delayTimeMin;
                DelayTimeMax = delayTimeMax;
            }

            /// <summary>
            /// 運行ステータスを取得または設定します。
            /// </summary>
            public TrainConditions Condition { get; set; }

            /// <summary>
            /// 運休している区間を取得または設定します。
            /// </summary>
            public LineRange SuspendRange { get; set; } = null;

            /// <summary>
            /// 遅延時間の下限を取得または設定します。
            /// 遅延がない場合<c>null</c>。
            /// </summary>
            public int? DelayTimeMin { get; set; } = null;

            /// <summary>
            /// 遅延時間の上限を取得または設定します。。
            /// 遅延がない場合<c>null</c>。
            /// </summary>
            public int? DelayTimeMax { get; set; } = null;


            /// <summary>
            /// 運行状況の概要を表す文字列を取得します。
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                if (SuspendRange != null)
                {
                    return $"{Condition.GetName()}({SuspendRange.ToString()}間)";
                }
                if (DelayTimeMin != null)
                {
                    return $"{Condition.GetName()}(約{DelayTimeMin}分)";
                }
                return Condition.GetName();
            }
        }

        /// <summary>
        /// 列車名に関わる情報を表します。
        /// </summary>
        public class TrainName
        {
            /// <summary>
            /// <see cref="TrainName"/> クラスの新しいインスタンスを初期化します。
            /// </summary>
            /// <param name="trainType">列車種別。</param>
            /// <param name="name">列車名。</param>
            /// <param name="number">列車番号。</param>
            /// <param name="subTrainType">サブ列車種別。</param>
            /// <param name="subTrainTypeRange">サブ列車種別が適用される区間。</param>
            public TrainName(TrainTypes trainType, string name = null, int? number = null, TrainTypes? subTrainType = null, LineRange subTrainTypeRange = null)
            {
                TrainType = trainType;
                Name = name;
                Number = number;
                SubTrainType = subTrainType;
                SubTrainTypeRange = subTrainTypeRange;
            }

            /// <summary>
            /// 列車の種別を取得します。
            /// </summary>
            public TrainTypes TrainType { get; private set; }

            /// <summary>
            /// 列車名を取得します。
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// 列車番号を取得します。
            /// </summary>
            public int? Number { get; private set; }

            /// <summary>
            /// 区間により複数の種別を持つ場合のサブ種別を取得します。
            /// </summary>
            public TrainTypes? SubTrainType { get; private set; }

            /// <summary>
            /// サブ種別が適用される区間を取得します。
            /// </summary>
            public LineRange SubTrainTypeRange { get; private set; }

            /// <summary>
            /// 指定の駅における列車種別を取得します。区間快速等の種別変更が考慮されます。
            /// </summary>
            /// <param name="station">対象となる駅</param>
            /// <returns></returns>
            public TrainTypes GetTypesByStation(Station station)
            {
                if (SubTrainTypeRange == null)
                {
                    return TrainType;
                }
                else
                {
                    if (SubTrainTypeRange.Contains(station))
                    {
                        return (TrainTypes)SubTrainType;
                    }
                    else
                    {
                        return TrainType;
                    }
                }
            }

            /// <summary>
            /// このインスタンスを表す文字列を取得します。
            /// </summary>
            /// <returns>列車名を表す文字列。</returns>
            public override string ToString()
            {
                return TrainType.GetName() + (string.IsNullOrWhiteSpace(Name) || Name == TrainType.GetName() ? "" : Name) + (Number is null ? "" : (Number.ToString() + "号"));
            }

            public static bool operator ==(TrainName left, TrainName right)
            {
                if (left is null)
                {
                    if (right is null)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return left.TrainType == right.TrainType && left.Name == right.Name && left.Number == right.Number
                        && left.SubTrainType == right.SubTrainType && left.SubTrainTypeRange == right.SubTrainTypeRange;
                }
            }

            public static bool operator !=(TrainName left, TrainName right)
            {
                return !(left == right);
            }
        }


        /// <summary>
        /// 到着タイプを表します。
        /// </summary>
        public enum ArrivalTypes
        {
            /// <summary>
            /// 到着
            /// </summary>
            Arrival,

            /// <summary>
            /// 出発
            /// </summary>
            Departure,

            /// <summary>
            /// 通過
            /// </summary>
            Through,
        }

        public class ArrivalTypesCreater
        {
            public ArrivalTypes FromString(string str)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 列車の運行ステータスを表します。
        /// </summary>
        public enum TrainConditions
        {
            /// <summary>
            /// 提供範囲外。
            /// </summary>
            OutsideArea = 7,

            /// <summary>
            /// 出発前。
            /// </summary>
            NotDeparted = 5,

            /// <summary>
            /// 出発前の到着列車。
            /// </summary>
            NotDepartedArrive = 6,

            /// <summary>
            /// 通常運行。
            /// </summary>
            OnSchedule = 0,

            /// <summary>
            /// 列車遅延。
            /// </summary>
            Delayed = 1,

            /// <summary>
            /// 部分運休。
            /// </summary>
            PartiallySuspended = 2,

            /// <summary>
            /// 運休。
            /// </summary>
            Suspended = 3
        }

        /// <summary>
        /// <see cref="TrainConditions"/>の作成を提供します。
        /// </summary>
        public class TrainConditionsCreater
        {
            /// <summary>
            /// 運行状況を表す文字列から運行状況を返します
            /// </summary>
            /// <param name="condition">運行状況を表す文字列</param>
            /// <returns></returns>
            /// <exception cref="NotSupportedException">対応していない運行状況のときにスローされる例外。</exception>
            public static TrainConditions CreateTrainConditions(string condition)
            {
                switch (condition)
                {
                    case "表示区間外":
                        return TrainConditions.OutsideArea;
                    case "出発前":
                        return TrainConditions.NotDeparted;
                    case "通常運行":
                        return TrainConditions.OnSchedule;
                    case "列車遅延":
                        return TrainConditions.Delayed;
                    case "部分運休":
                        return TrainConditions.PartiallySuspended;
                    case "全区間運休":
                        return TrainConditions.Suspended;
                    default:
                        throw new NotSupportedException("この運行状況はサポートされていません。");
                }
            }
        }

        /// <summary>
        /// 列車種別を表します。
        /// </summary>
        public enum TrainTypes
        {
            /// <summary>
            /// 普通。
            /// </summary>
            Local,

            /// <summary>
            /// 区間快速。
            /// </summary>
            Semi_Rapid,

            /// <summary>
            /// 区間快速(各停区間走行)。
            /// </summary>
            Become_Semi_Rapid,

            /// <summary>
            /// 快速。
            /// </summary>
            Rapid,

            /// <summary>
            /// 急行。
            /// </summary>
            Express,

            /// <summary>
            /// 特急。
            /// </summary>
            Ltd_Exp,

            /// <summary>
            /// 臨時。
            /// </summary>
            Extra,

            /// <summary>
            /// その他。
            /// </summary>
            Other,
        }

        /// <summary>
        /// <see cref="TrainTypes"/>の作成を提供します。
        /// </summary>
        public class TrainTypesCreater
        {
            /// <summary>
            /// 列車種別を表す文字列から列車種別を返します
            /// </summary>
            /// <param name="text"></param>
            /// <returns></returns>
            public static TrainTypes FromString(string text)
            {
                switch (text)
                {
                    case "普通":
                        return TrainTypes.Local;
                    case "区快":
                        return TrainTypes.Semi_Rapid;
                    case "区間快速":
                        return TrainTypes.Semi_Rapid;
                    case "快速":
                        return TrainTypes.Rapid;
                    case "特急":
                        return TrainTypes.Ltd_Exp;
                    default:
                        return TrainTypes.Other;
                }
            }

            /// <summary>
            /// 列車種別を表すURLから列車種別を返します
            /// </summary>
            /// <param name="text"></param>
            /// <returns></returns>
            [Obsolete("このメソッドは現在のバージョンでは使用されていません")]
            public static TrainTypes FromImgTag(string text)
            {
                switch (text)
                {
                    case @"img/common/icon-train-futsu.png":
                        return TrainTypes.Local;
                    case @"img/common/icon-train-kukai.png":
                        return TrainTypes.Semi_Rapid;
                    case @"img/common/icon-train-kaisoku.png":
                        return TrainTypes.Rapid;
                    case @"img/common/icon-train-tokkyu.png":
                        return TrainTypes.Ltd_Exp;
                    default:
                        return TrainTypes.Other;
                }
            }

        }
    }
}
/*


/// <summary>
/// 列車の運行状況を表します。
/// </summary>
public class TrainCondition
{
    /// <summary>
    /// <see cref="TrainCondition"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="condition">列車運行ステータス。</param>
    /// <param name="suspendRange">運休区間。</param>
    /// <exception cref="ArgumentException">引数の組がサポートされない場合にスローされる例外。</exception> 
    public TrainCondition(TrainConditions condition, LineRange suspendRange = null)
    {
        Condition = condition;
        SuspendRange = suspendRange;
        DelayTimeMin = null;
        DelayTimeMax = null;
    }

    /// <summary>
    /// <see cref="TrainCondition"/> クラスの新しいインスタンスを初期化します。
    /// </summary>
    /// <param name="condition">列車運行ステータス。</param>
    /// <param name="delayTimeMin">遅延時間の下限。</param>
    /// <param name="delayTimeMax">遅延時間の上限。</param>
    /// <exception cref="ArgumentException">引数の組がサポートされない場合にスローされる例外。</exception> 
    public TrainCondition(TrainConditions condition, int delayTimeMin, int delayTimeMax)
    {
        Condition = condition;
        SuspendRange = null;
        DelayTimeMin = delayTimeMin;
        DelayTimeMax = delayTimeMax;
    }

    /// <summary>
    /// 運行ステータスを取得または設定します。
    /// </summary>
    public TrainConditions Condition { get; set; }

    /// <summary>
    /// 運休している区間を取得または設定します。
    /// </summary>
    public LineRange SuspendRange { get; set; } = null;

    /// <summary>
    /// 遅延時間の下限を取得または設定します。
    /// 遅延がない場合<c>null</c>。
    /// </summary>
    public int? DelayTimeMin { get; set; } = null;

    /// <summary>
    /// 遅延時間の上限を取得または設定します。。
    /// 遅延がない場合<c>null</c>。
    /// </summary>
    public int? DelayTimeMax { get; set; } = null;


    /// <summary>
    /// 運行状況の概要を表す文字列を取得します。
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return Condition.GetName();
    }
}

/// <summary>
/// 列車名に関わる情報を表します。
/// </summary>
public class TrainName
{
/// <summary>
/// <see cref="TrainName"/> クラスの新しいインスタンスを初期化します。
/// </summary>
/// <param name="trainType">列車種別。</param>
/// <param name="name">列車名。</param>
/// <param name="number">列車番号。</param>
/// <param name="subTrainType">サブ列車種別。</param>
/// <param name="subTrainTypeRange">サブ列車種別が適用される区間。</param>
public TrainName(TrainTypes trainType, string name = null, int? number = null, TrainTypes? subTrainType = null, LineRange subTrainTypeRange = null)
{
TrainType = trainType;
Name = name;
Number = number;
SubTrainType = subTrainType;
SubTrainTypeRange = subTrainTypeRange;
}

/// <summary>
/// 列車の種別を取得します。
/// </summary>
public TrainTypes TrainType { get; private set; }

/// <summary>
/// 列車名を取得します。
/// </summary>
public string Name { get; private set; }

/// <summary>
/// 列車番号を取得します。
/// </summary>
public int? Number { get; private set; }

/// <summary>
/// 区間により複数の種別を持つ場合のサブ種別を取得します。
/// </summary>
public TrainTypes? SubTrainType { get; private set; }

/// <summary>
/// サブ種別が適用される区間を取得します。
/// </summary>
public LineRange SubTrainTypeRange { get; private set; }

/// <summary>
/// 指定の駅における列車種別を取得します。区間快速等の種別変更が考慮されます。
/// </summary>
/// <param name="station">対象となる駅</param>
/// <returns></returns>
public TrainTypes GetTypesByStation(Station station)
{
if (SubTrainTypeRange == null)
{
return TrainType;
}
else
{
if (SubTrainTypeRange.Contains(station))
{
    return (TrainTypes)SubTrainType;
}
else
{
    return TrainType;
}
}
}

/// <summary>
/// このインスタンスを表す文字列を取得します。
/// </summary>
/// <returns>列車名を表す文字列。</returns>
public override string ToString()
{
return TrainType.GetName() + (string.IsNullOrWhiteSpace(Name) || Name == TrainType.GetName() ? "" : Name) + (Number is null ? "" : (Number.ToString() + "号"));
}

public static bool operator ==(TrainName left, TrainName right)
{
if (left is null)
{
if (right is null)
{
    return true;
}
else
{
    return false;
}
}
else
{
return left.TrainType == right.TrainType && left.Name == right.Name && left.Number == right.Number
    && left.SubTrainType == right.SubTrainType && left.SubTrainTypeRange == right.SubTrainTypeRange;
}
}

public static bool operator !=(TrainName left, TrainName right)
{
return !(left == right);
}
}


/// <summary>
/// 到着タイプを表します。
/// </summary>
public enum ArrivalTypes
{
/// <summary>
/// 到着
/// </summary>
Arrival,

/// <summary>
/// 出発
/// </summary>
Departure,

/// <summary>
/// 通過
/// </summary>
Through,
}

public class ArrivalTypesCreater
{
public ArrivalTypes FromString(string str)
{
throw new NotImplementedException();
}
}

/// <summary>
/// 列車の運行ステータスを表します。
/// </summary>
public enum TrainConditions
{
/// <summary>
/// 提供範囲外。
/// </summary>
OutsideArea = 7,

/// <summary>
/// 出発前。
/// </summary>
NotDeparted = 5,

/// <summary>
/// 出発前の到着列車。
/// </summary>
NotDepartedArrive = 6,

/// <summary>
/// 通常運行。
/// </summary>
OnSchedule = 0,

/// <summary>
/// 列車遅延。
/// </summary>
Delayed = 1,

/// <summary>
/// 部分運休。
/// </summary>
PartiallySuspended = 2,

/// <summary>
/// 運休。
/// </summary>
Suspended = 3
}

/// <summary>
/// <see cref="TrainConditions"/>の作成を提供します。
/// </summary>
public class TrainConditionsCreater
{
/// <summary>
/// 運行状況を表す文字列から運行状況を返します
/// </summary>
/// <param name="condition">運行状況を表す文字列</param>
/// <returns></returns>
/// <exception cref="NotSupportedException">対応していない運行状況のときにスローされる例外。</exception>
public static TrainConditions CreateTrainConditions(string condition)
{
switch (condition)
{
case "表示区間外":
    return TrainConditions.OutsideArea;
case "出発前":
    return TrainConditions.NotDeparted;
case "通常運行":
    return TrainConditions.OnSchedule;
case "列車遅延":
    return TrainConditions.Delayed;
case "部分運休":
    return TrainConditions.PartiallySuspended;
case "全区間運休":
    return TrainConditions.Suspended;
default:
    throw new NotSupportedException("この運行状況はサポートされていません。");
}
}
}

/// <summary>
/// 列車種別を表します。
/// </summary>
public enum TrainTypes
{
/// <summary>
/// 普通。
/// </summary>
Local,

/// <summary>
/// 区間快速。
/// </summary>
Semi_Rapid,

/// <summary>
/// 区間快速(各停区間走行)。
/// </summary>
Become_Semi_Rapid,

/// <summary>
/// 快速。
/// </summary>
Rapid,

/// <summary>
/// 急行。
/// </summary>
Express,

/// <summary>
/// 特急。
/// </summary>
Ltd_Exp,

/// <summary>
/// 臨時。
/// </summary>
Extra,

/// <summary>
/// その他。
/// </summary>
Other,
}

/// <summary>
/// <see cref="TrainTypes"/>の作成を提供します。
/// </summary>
public class TrainTypesCreater
{
/// <summary>
/// 列車種別を表す文字列から列車種別を返します
/// </summary>
/// <param name="text"></param>
/// <returns></returns>
public static TrainTypes FromString(string text)
{
switch (text)
{
case "普通":
    return TrainTypes.Local;
case "区快":
    return TrainTypes.Semi_Rapid;
case "区間快速":
    return TrainTypes.Semi_Rapid;
case "快速":
    return TrainTypes.Rapid;
case "特急":
    return TrainTypes.Ltd_Exp;
default:
    return TrainTypes.Other;
}
}

/// <summary>
/// 列車種別を表すURLから列車種別を返します
/// </summary>
/// <param name="text"></param>
/// <returns></returns>
[Obsolete("このメソッドは現在のバージョンでは使用されていません")]
public static TrainTypes FromImgTag(string text)
{
switch (text)
{
case @"img/common/icon-train-futsu.png":
    return TrainTypes.Local;
case @"img/common/icon-train-kukai.png":
    return TrainTypes.Semi_Rapid;
case @"img/common/icon-train-kaisoku.png":
    return TrainTypes.Rapid;
case @"img/common/icon-train-tokkyu.png":
    return TrainTypes.Ltd_Exp;
default:
    return TrainTypes.Other;
}
}

}
*/
