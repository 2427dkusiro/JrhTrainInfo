namespace TrainInfo.Models
{
    /// <summary>
    /// 列車運行ステータスを表します。
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
}
