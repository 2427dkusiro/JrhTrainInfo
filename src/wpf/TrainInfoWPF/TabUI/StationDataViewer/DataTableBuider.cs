
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TrainInfoWPF.TabUI.StationDataViewer
{
    /// <summary>
    /// <see cref="DataTable"/> の作成を補助する機能を提供します。
    /// </summary>
    public class DataTableBuilder
    {
        private readonly List<string> titleList = new List<string>();
        private readonly List<Type> typeList = new List<Type>();
        private readonly List<IReadOnlyList<object>> contentList = new List<IReadOnlyList<object>>();

        /// <summary>
        /// 作成中の <see cref="DataTable"/> に新しい列をデータと共に追加します。
        /// </summary>
        /// <typeparam name="T">追加する要素の型。共変性を利用する為参照型である必要があります。</typeparam>
        /// <param name="name">追加する列の名前。</param>
        /// <param name="value">追加する要素。</param>
        public void AddColumn<T>(string name, IReadOnlyList<T> value) where T : class
        {
            titleList.Add(name);
            typeList.Add(typeof(T));
            contentList.Add(value);
        }

        /// <summary>
        /// 作成中の <see cref="DataTable"/> に新しい列をデータと共に追加します。
        /// </summary>
        /// <param name="name">追加する列の名前。</param>
        /// <param name="type">追加する要素の型。</param>
        /// <param name="value">追加する要素。</param>
        public void AddColumn(string name, Type type, IReadOnlyList<object> value)
        {
            titleList.Add(name);
            typeList.Add(type);
            contentList.Add(value);
        }

        /// <summary>
        /// 現在編集中の <see cref="DataTable"/> を構築して出力します。
        /// </summary>
        /// <returns></returns>
        public DataTable Build()
        {
            var dataTable = new DataTable();
            for (var i = 0; i < titleList.Count; i++)
            {
                dataTable.Columns.Add(titleList[i], typeList[i]);
            }

            var count = contentList.First().Count;

            for (var i = 0; i < count; i++)
            {
                DataRow dataRow = dataTable.NewRow();
                for (var j = 0; j < contentList.Count; j++)
                {
                    dataRow[j] = contentList[j][i];
                }
                dataTable.Rows.Add(dataRow);
            }
            return dataTable;
        }
    }
}
