using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using Android.Content;
using Android.Views;
using Android.Widget;

namespace App1
{
    public partial class HierarchyButtonLayout
    {
        /// <summary>
        /// <see cref="HierarchyButtonLayout"/>のボタン要素を表します。
        /// </summary>
        public class HierarchyTextButton : HierarchyButton
        {
            private static RelativeLayout.LayoutParams rightArrowSignTextLayoutParams;
            private static RelativeLayout.LayoutParams leftArrowSignTextLayoutParams;
            private static RelativeLayout.LayoutParams rightArrowSignImageLayoutParams;
            private static RelativeLayout.LayoutParams leftArrowSignLayoutParams;
            private static readonly int leftArrowSignImageId;

            /// <summary>
            /// <see cref="HierarchyTextButton"/>クラスの新しいインスタンスを初期化します。
            /// </summary>
            public HierarchyTextButton()
            {

            }

            static HierarchyTextButton()
            {
                leftArrowSignImageId = View.GenerateViewId();

                rightArrowSignTextLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                rightArrowSignTextLayoutParams.AddRule(LayoutRules.AlignParentLeft);
                rightArrowSignTextLayoutParams.AddRule(LayoutRules.CenterVertical);
                rightArrowSignTextLayoutParams.LeftMargin = 40;

                leftArrowSignTextLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                leftArrowSignTextLayoutParams.AddRule(LayoutRules.EndOf, leftArrowSignImageId);
                leftArrowSignTextLayoutParams.AddRule(LayoutRules.CenterVertical);

                rightArrowSignImageLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                rightArrowSignImageLayoutParams.AddRule(LayoutRules.AlignParentRight);
                rightArrowSignImageLayoutParams.AddRule(LayoutRules.CenterVertical);
                rightArrowSignImageLayoutParams.Height = 70;
                rightArrowSignImageLayoutParams.Width = 70;
                rightArrowSignImageLayoutParams.RightMargin = 40;

                leftArrowSignLayoutParams = new RelativeLayout.LayoutParams(RelativeLayout.LayoutParams.MatchParent, RelativeLayout.LayoutParams.MatchParent);
                leftArrowSignLayoutParams.AddRule(LayoutRules.AlignParentLeft);
                leftArrowSignLayoutParams.AddRule(LayoutRules.CenterVertical);
                leftArrowSignLayoutParams.Height = 70;
                leftArrowSignLayoutParams.Width = 70;
                leftArrowSignLayoutParams.RightMargin = 20;
                leftArrowSignLayoutParams.LeftMargin = 30;
            }

            private bool NeedRendering = true;
            private ViewGroup RenderCache = null;

            private string description;
            public override string Description
            {
                get { return description ?? Text; }
                set { description = value; }
            }

            /// <summary>
            /// このボタンのテキストを取得または設定します。
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            /// 文字サイズを取得または設定します。
            /// </summary>
            public int TextSize { get; set; } = 18;

            /// <summary>
            /// 矢印アイコンの向きを取得または設定します。
            /// </summary>
            public ArrowSignDirections ArrowSignDirection { get; set; } = ArrowSignDirections.Right;

            public override event EventHandler Click;

            public override ViewGroup BuildView(Context context)
            {
                if (!(RenderCache is null))
                {
                    //Debug.WriteLine("キャッシュデータを返しました");
                    return RenderCache;
                }

                var buttonRelativeLayout = new RelativeLayout(context);

                var textView = new TextView(context)
                {
                    Text = Text,
                    TextSize = TextSize,
                    Gravity = GravityFlags.CenterVertical,
                };

                if (this.ArrowSignDirection == ArrowSignDirections.Left)
                {
                    var imageView = new ImageView(context);
                    imageView.SetImageResource(Resource.Drawable.LeftArrowSignIcon);
                    imageView.SetScaleType(ImageView.ScaleType.FitXy);
                    imageView.SetAdjustViewBounds(true);
                    imageView.Id = leftArrowSignImageId;
                    buttonRelativeLayout.AddView(imageView, leftArrowSignLayoutParams);
                    buttonRelativeLayout.AddView(textView, leftArrowSignTextLayoutParams);
                }
                else if (this.ArrowSignDirection == HierarchyTextButton.ArrowSignDirections.Right)
                {
                    var imageView = new ImageView(context);
                    imageView.SetImageResource(Resource.Drawable.RightArrowSignIcon);
                    imageView.SetScaleType(ImageView.ScaleType.FitXy);
                    imageView.SetAdjustViewBounds(true);
                    buttonRelativeLayout.AddView(imageView, rightArrowSignImageLayoutParams);
                    buttonRelativeLayout.AddView(textView, rightArrowSignTextLayoutParams);
                }
                else
                {
                    buttonRelativeLayout.AddView(textView);
                }

                RenderCache = buttonRelativeLayout;
                return buttonRelativeLayout;
            }

            /// <summary>
            /// ボタンが押されたことを通知します。
            /// </summary>
            /// <param name="e"></param>
            public override void OnClick(HierarchyButtonClickEventArgs e)
            {
                if (Children.Any())
                {
                    e.NextCurrentHierarchyButton = this;
                    e.LayoutChanged = true;
                }
                Click?.Invoke(this, e);
            }

            /// <summary>
            /// 矢印アイコンの向きを表します。
            /// </summary>
            public enum ArrowSignDirections
            {
                None,
                Right,
                Left,
            }
        }
    }
}