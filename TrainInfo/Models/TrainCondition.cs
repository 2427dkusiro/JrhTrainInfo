using System;
using System.Collections.Generic;
using System.Text;

namespace TrainInfo.Models
{
    /// <summary>
    /// 列車の運行状況を表します。
    /// </summary>
    public class TrainCondition
    {
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
        /// <see cref="TrainCondition"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="condition">列車運行ステータス。</param>
        /// <param name="suspendRange">運休区間。</param>
        /// <param name="delayTimeMin">遅延時間の下限。</param>
        /// <param name="delayTimeMax">遅延時間の上限。</param>
        public TrainCondition(TrainConditions condition, LineRange suspendRange = null, int? delayTimeMin = null, int? delayTimeMax = null)
        {
            if (!((delayTimeMin == null && delayTimeMax == null) || (delayTimeMin != null && delayTimeMax != null)))
            {
                throw new FormatException("遅延時間の上限と下限は同時に設定されなくてはなりません");
            }
            Condition = condition;
            SuspendRange = suspendRange;
            DelayTimeMin = delayTimeMin;
            DelayTimeMax = delayTimeMax;
        }
        
        /// <summary>
        /// 運行状況の概要を表す文字列を取得します。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Condition.GetName();
        }
    }
}
