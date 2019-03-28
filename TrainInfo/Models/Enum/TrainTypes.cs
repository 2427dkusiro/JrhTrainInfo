namespace TrainInfo.Models
{
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
}
