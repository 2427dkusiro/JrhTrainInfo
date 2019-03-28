using TrainInfo.Models;

namespace TrainInfo.Stations
{ 
    /// <summary>
    /// 路線名と行先の方面の組を表します。
    /// </summary>
    public struct LineDestPair
    {
        /// <summary>
        /// 路線名を取得します。
        /// </summary>
        public JrhLine JrhLine { get; }

        /// <summary>
        /// 行先の方面を取得します。
        /// </summary>
        public JrhDestType JrhDestType { get; }

        /// <summary>
        /// この構造体の新しいインスタンスを路線名と行先の方面情報から初期化します。
        /// </summary>
        /// <param name="jrhLine"></param>
        /// <param name="jrhDestType"></param>
        public LineDestPair(JrhLine jrhLine, JrhDestType jrhDestType) : this()
        {
            JrhLine = jrhLine;
            JrhDestType = jrhDestType;
        }
    }  
}
