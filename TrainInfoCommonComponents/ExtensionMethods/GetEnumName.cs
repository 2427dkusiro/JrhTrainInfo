using System;
using TrainInfo.Stations;

namespace TrainInfo.ExtensionMethods
{
    /// <summary>
    /// ライブラリ中の<c>Enum</c>の名前取得を提供します。
    /// </summary>
    public static class GetEnumName
    {
        /// <summary>
        /// <see cref="TrainData.TrainTypes"/> の日本語名を取得します。
        /// </summary>
        /// <param name="trainTypes">取得対象の列車種別。</param>
        /// <returns>一般的な日本語の列車種別名。</returns>
        /// <exception cref="NotSupportedException">路線がサポートされない場合にスローされる例外。</exception>
        public static string GetName(this TrainData.TrainTypes trainTypes)
        {
            switch (trainTypes)
            {
                case TrainData.TrainTypes.Local:
                    return "普通";
                case TrainData.TrainTypes.Semi_Rapid:
                    return "区間快速";
                case TrainData.TrainTypes.Rapid:
                    return "快速";
                case TrainData.TrainTypes.Express:
                    return "急行";
                case TrainData.TrainTypes.Ltd_Exp:
                    return "特急";
                case TrainData.TrainTypes.Extra:
                    return "臨時";
                case TrainData.TrainTypes.Other:
                    return "不明";
                default:
                    throw new NotSupportedException("この列車種別はサポートされていません。");
            }
        }

        /// <summary>
        /// <see cref="TrainData.TrainConditions"/> の日本語名を取得します。
        /// </summary>
        /// <param name="trainConditions">取得対象の運行状況。</param>
        /// <returns>列車状況を表す文字列。</returns>
        /// <exception cref="NotSupportedException">運行状況がサポートされない場合にスローされる例外。</exception>
        public static string GetName(this TrainData.TrainConditions trainConditions)
        {
            switch (trainConditions)
            {
                case TrainData.TrainConditions.OutsideArea:
                    return "表示区間外";
                case TrainData.TrainConditions.NotDeparted:
                case TrainData.TrainConditions.NotDepartedArrive:
                    return "出発前";
                case TrainData.TrainConditions.OnSchedule:
                    return "通常運行";
                case TrainData.TrainConditions.Delayed:
                    return "列車遅延";
                case TrainData.TrainConditions.PartiallySuspended:
                    return "部分運休";
                case TrainData.TrainConditions.Suspended:
                    return "運休";
                default:
                    throw new NotSupportedException("この運行状況はサポートされていません。");
            }
        }

        /// <summary>
        /// <see cref="TrainData.ArrivalTypes"/>の日本語名を取得します。
        /// </summary>
        /// <param name="arrivalTypes"></param>
        /// <returns></returns>
        public static string GetName(this TrainData.ArrivalTypes arrivalTypes)
        {
            switch (arrivalTypes)
            {
                case TrainData.ArrivalTypes.Arrival:
                    return "出発";
                case TrainData.ArrivalTypes.Departure:
                    return "到着";
                case TrainData.ArrivalTypes.Through:
                    return "通過";
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// <c>JrhLine</c>から路線名を表す文字列を取得します。
        /// </summary>
        /// <param name="jrhLine"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">路線がサポートされない場合にスローされる例外。</exception>
        public static string GetName(this JrhLine jrhLine)
        {
            switch (jrhLine)
            {
                case JrhLine.Hakodate_Iwamizawa:
                    return "函館本線岩見沢方面";
                case JrhLine.Hakodate_Otaru:
                    return "函館本線小樽方面";
                case JrhLine.Chitose_Tomakomai:
                    return "千歳線苫小牧方面";
                case JrhLine.Sassyo:
                    return "学園都市線";
                case JrhLine.Sekisyo:
                    return "石勝線";
                case JrhLine.Nemuro_Furano:
                    return "根室本線富良野方面";
                case JrhLine.Nemuro_Obihiro:
                    return "根室本線帯広方面";
                case JrhLine.Muroran:
                    return "室蘭本線";
                default:
                    throw new NotSupportedException("路線が存在しません");
            }
        }

        /// <summary>
        /// <see cref="JrhDestType">JrhDestType</see>から行先の方面を表す文字列を取得します。
        /// </summary>
        /// <param name="jrhDistType"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException">行先がサポートされない場合にスローされる例外。</exception>
        public static string GetName(this JrhDestType jrhDistType)
        {
            switch (jrhDistType)
            {
                case JrhDestType.Sapporo_Hakodate:
                    return "函館本線[札幌方面]";
                case JrhDestType.Sapporo_Chitose:
                    return "千歳線[札幌方面]";
                case JrhDestType.Sapporo_Hakodate_Chitose:
                    return "函館本線・千歳線[札幌方面]";
                case JrhDestType.Sapporo_Sassyo:
                    return "学園都市線[札幌方面]";
                case JrhDestType.Sapporo_Hakodate_Sassyo:
                    return "函館本線・学園都市線[札幌方面]";
                case JrhDestType.Hakodate_Otaru:
                    return "函館本線[小樽方面]";
                case JrhDestType.Muroran_Iwamizawa:
                    return "室蘭本線[岩見沢方面]";
                case JrhDestType.Hakodate_Iwamizawa:
                    return "函館本線[岩見沢方面]";
                case JrhDestType.Hakodate_Yoichi:
                    return "函館本線[余市方面]";
                case JrhDestType.Hakodate_Takikawa:
                    return "函館本線[滝川・旭川方面]";
                case JrhDestType.Chitose_Chitose:
                    return "千歳線[千歳方面]";
                case JrhDestType.Chitose_AP:
                    return "千歳線[新千歳空港]";
                case JrhDestType.Chitose_Rapid_AP:
                    return "千歳線[（快速）新千歳空港]";
                case JrhDestType.Chitose_LocalRapid_Sapporo:
                    return "千歳線[（普通・快速）札幌方面]";
                case JrhDestType.Chitose_LimExp_Chitose_Sekisyo:
                    return "千歳線[（特急）帯広・釧路方面]";
                case JrhDestType.Chitose_LimExp_Sapporo:
                    return "千歳線[（特急）札幌]";
                case JrhDestType.Muroran_Tomakomai:
                    return "室蘭本線[苫小牧方面]";
                case JrhDestType.chitose_Tomakomai:
                    return "千歳線[苫小牧・東室蘭・函館方面]";
                case JrhDestType.Chitose_Muroran:
                    return "千歳線[室蘭・函館方面]";
                case JrhDestType.Sassyo_IshikariTobetsu:
                    return "学園都市線[石狩当別方面]";
                case JrhDestType.Sassyo_Urausu:
                    return "学園都市線[浦臼・新十津川方面]";
                case JrhDestType.Muroran_Oiwake:
                    return "室蘭本線[追分方面]";
                case JrhDestType.Sekisyo_MinamiChitose_Sapporo:
                    return "石勝線[南千歳・札幌方面]";
                case JrhDestType.Sekisyo_Oiwake_Obihiro:
                    return "石勝線[追分・帯広・釧路方面]";
                case JrhDestType.Sekisyo_Oiwake_Sapporo:
                    return "石勝線[追分・新千歳空港・札幌方面]";
                case JrhDestType.Sekisyo_Shiyubari:
                    return "石勝線[新夕張方面]";
                case JrhDestType.Sekisyo_Yubari:
                    return "石勝線[夕張方面]";
                case JrhDestType.Sekisyo_Obihiro:
                    return "石勝線[帯広方面]";
                case JrhDestType.Nemuro_Takikawa:
                    return "根室本線[滝川方面]";
                case JrhDestType.Nemuro_Hurano:
                    return "根室本線[富良野方面]";
                case JrhDestType.Nemuro_Shintoku:
                    return "根室本線[新得方面]";
                case JrhDestType.Nemuro_Obihiro:
                    return "根室本線[帯広方面]";
                case JrhDestType.Nemuro_Kusiro:
                    return "根室本線[釧路方面]";
                case JrhDestType.Hanasaki_Kusiro:
                    return "花咲線[釧路方面]";
                case JrhDestType.Senmo_Kusiro:
                    return "釧網本線[釧路方面]";
                case JrhDestType.Hanasaki_Nemuro:
                    return "釧網本線[根室方面]";
                case JrhDestType.Senmo_Abashiri:
                    return "釧網本線[網走方面]";
                case JrhDestType.Sekihoku_Asahikawa:
                    return "石北本線[旭川方面]";
                case JrhDestType.Hurano_Asahikawa:
                    return "富良野線[旭川方面]";

                default:
                    throw new NotSupportedException("路線が存在しません");
            }
        }
    }
}
