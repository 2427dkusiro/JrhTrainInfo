using Android.Content;
using Android.Views;
using System;
using System.Collections.Generic;

namespace JrhTrainInfoAndroid
{
    public partial class HierarchyButtonLayout
    {
        public abstract class HierarchyButton
        {
            public abstract event EventHandler Click;

            /// <summary>
            /// ボタンが押されたことを通知します。
            /// </summary>
            /// <param name="e"></param>
            public abstract void OnClick(HierarchyButtonClickEventArgs e);

            public abstract ViewGroup BuildView(Context context);

            public abstract ViewGroup GetView(Context context, out bool IsCache);

            public abstract string Description { get; set; }

            /// <summary>
            /// 親要素のボタンを取得します。
            /// </summary>
            public HierarchyButton Parent { get; protected set; }


            public IReadOnlyList<HierarchyButton> Children => children;

            private List<HierarchyButton> children = new List<HierarchyButton>();

            /// <summary>
            /// 子要素を追加します。子要素には親要素が設定されます。
            /// </summary>
            /// <param name="hierarchyButton"></param>
            public void AddChild(HierarchyButton hierarchyButton)
            {
                hierarchyButton.Parent = this;
                children.Add(hierarchyButton);
            }

            /// <summary>
            /// 複数の子要素を追加します。子要素には親要素が設定されます。
            /// </summary>
            /// <param name="hierarchyButtons"></param>
            public void AddChildren(IEnumerable<HierarchyButton> hierarchyButtons)
            {
                foreach (var hierarchyButton in hierarchyButtons)
                {
                    hierarchyButton.Parent = this;
                }
                children.AddRange(hierarchyButtons);
            }

            /// <summary>
            /// 子要素を挿入します。子要素には親要素が設定されます。
            /// </summary>
            /// <param name="hierarchyButton"></param>
            public void InsertChild(int index, HierarchyButton hierarchyButton)
            {
                hierarchyButton.Parent = this;
                children.Insert(index, hierarchyButton);
            }
        }
    }
}