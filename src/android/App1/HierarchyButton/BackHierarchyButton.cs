using System;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace App1
{
    public partial class HierarchyButtonLayout
    {
        public class BackHierarchyButton : HierarchyButton
        {
            private static RelativeLayout.LayoutParams leftArrowSignTextLayoutParams;
            private static RelativeLayout.LayoutParams leftArrowSignLayoutParams;
            private static readonly int leftArrowSignImageId;

            /// <summary>
            /// <see cref="HierarchyTextButton"/>クラスの新しいインスタンスを初期化します。
            /// </summary>
            public BackHierarchyButton()
            {

            }

            /// <summary>
            /// <see cref="HierarchyTextButton"/>クラスの新しいインスタンスを初期化します。
            /// </summary>
            public BackHierarchyButton(HierarchyButton parent)
            {
                Parent = parent;
            }

            static BackHierarchyButton()
            {
                leftArrowSignImageId = View.GenerateViewId();

                leftArrowSignTextLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                leftArrowSignTextLayoutParams.AddRule(LayoutRules.EndOf, leftArrowSignImageId);
                leftArrowSignTextLayoutParams.AddRule(LayoutRules.CenterVertical);

                leftArrowSignLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                leftArrowSignLayoutParams.AddRule(LayoutRules.AlignParentLeft);
                leftArrowSignLayoutParams.AddRule(LayoutRules.CenterVertical);
                leftArrowSignLayoutParams.Height = 70;
                leftArrowSignLayoutParams.Width = 70;
                leftArrowSignLayoutParams.RightMargin = 20;
                leftArrowSignLayoutParams.LeftMargin = 30;
            }

            /// <summary>
            /// このボタンのテキストを取得または設定します。
            /// </summary>
            public string Text => $"{Parent.Parent.Description}へ戻る";

            private string description;
            public override string Description
            {
                get { return description ?? Text; }
                set { description = value; }
            }

            /// <summary>
            /// 文字サイズを取得または設定します。
            /// </summary>
            public int TextSize { get; set; } = 18;

            public override ViewGroup BuildView(Context context)
            {
                var buttonRelativeLayout = new RelativeLayout(context);

                var textView = new TextView(context)
                {
                    Text = Text,
                    TextSize = TextSize,
                    Gravity = GravityFlags.CenterVertical,
                };

                var imageView = new ImageView(context);
                imageView.SetImageResource(Resource.Drawable.LeftArrowSignIcon);
                imageView.SetScaleType(ImageView.ScaleType.FitXy);
                imageView.SetAdjustViewBounds(true);
                imageView.Id = leftArrowSignImageId;
                buttonRelativeLayout.AddView(imageView, leftArrowSignLayoutParams);
                buttonRelativeLayout.AddView(textView, leftArrowSignTextLayoutParams);

                return buttonRelativeLayout;
            }

            public override event EventHandler Click;

            /// <summary>
            /// ボタンが押されたことを通知します。
            /// </summary>
            /// <param name="e"></param>
            public override void OnClick(HierarchyButtonClickEventArgs e)
            {
                e.NextCurrentHierarchyButton = this.Parent.Parent;
                e.LayoutChanged = true;
                Click?.Invoke(this, e);
            }
        }
    }
}