using Android.Content;
using Android.Views;
using Android.Widget;
using System;
using System.Linq;
using Debug = System.Diagnostics.Debug;

namespace JrhTrainInfoAndroid
{
    public partial class HierarchyButtonLayout
    {
        /// <summary>
        /// <see cref="HierarchyButtonLayout"/>のボタン要素を表します。
        /// </summary>
        public class HierarchyTextButton : HierarchyButton
        {
            private static readonly RelativeLayout.LayoutParams rightArrowSignTextLayoutParams;
            private static readonly RelativeLayout.LayoutParams leftArrowSignTextLayoutParams;
            private static readonly RelativeLayout.LayoutParams rightArrowSignImageLayoutParams;
            private static readonly RelativeLayout.LayoutParams leftArrowSignLayoutParams;
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

            private bool needReflesh = true;
            private ViewGroup renderCache = null;

            private string description;
            private string text;
            private int textSize = 18;
            private ArrowSignDirections arrowSignDirection = ArrowSignDirections.Right;


            public override string Description
            {
                get => description ?? Text;
                set => description = value;
            }

            /// <summary>
            /// このボタンのテキストを取得または設定します。
            /// </summary>
            public string Text
            {
                get => text;
                set { needReflesh = true; text = value; }
            }

            /// <summary>
            /// 文字サイズを取得または設定します。
            /// </summary>
            public int TextSize
            {
                get => textSize;
                set { needReflesh = true; textSize = value; }
            }

            /// <summary>
            /// 矢印アイコンの向きを取得または設定します。
            /// </summary>
            public ArrowSignDirections ArrowSignDirection
            {
                get => arrowSignDirection;
                set { needReflesh = true; arrowSignDirection = value; }
            }

            public override event EventHandler Click;

            public override ViewGroup GetView(Context context, out bool IsCache)
            {
                if (needReflesh)
                {
                    Debug.WriteLine("return new rendered button!");
                    IsCache = false;
                    needReflesh = false;
                    return BuildView(context);
                }
                else
                {
                    try
                    {
                        if (renderCache.Class is null)
                        {
                            throw new InvalidOperationException();
                        }
                    }
                    catch (ObjectDisposedException ex)
                    {
                        Debug.WriteLine("Error!\n" +
                                        "-----------------------------\n" +
                                           "Cached view was disposed!\n" +
                                        "-----------------------------");

                        IsCache = false;
                        return BuildView(context);
                    }

                    Debug.WriteLine("return cache button!");
                    IsCache = true;
                    return renderCache;
                }
            }

            public override ViewGroup BuildView(Context context)
            {
                var buttonRelativeLayout = new RelativeLayout(context);

                var textView = new TextView(context)
                {
                    Text = Text,
                    TextSize = TextSize,
                    Gravity = GravityFlags.CenterVertical,
                };

                if (ArrowSignDirection == ArrowSignDirections.Left)
                {
                    var imageView = new ImageView(context);
                    imageView.SetImageResource(Resource.Drawable.LeftArrowSignIcon);
                    imageView.SetScaleType(ImageView.ScaleType.FitXy);
                    imageView.SetAdjustViewBounds(true);
                    imageView.Id = leftArrowSignImageId;
                    buttonRelativeLayout.AddView(imageView, leftArrowSignLayoutParams);
                    buttonRelativeLayout.AddView(textView, leftArrowSignTextLayoutParams);
                }
                else if (ArrowSignDirection == HierarchyTextButton.ArrowSignDirections.Right)
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

                renderCache = buttonRelativeLayout;
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