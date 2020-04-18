using Android.Support.V4.App;
using Java.Lang;
using JrhTrainInfoAndroid.Resources.layout;
using Fragment = Android.Support.V4.App.Fragment;
using FragmentManager = Android.Support.V4.App.FragmentManager;

namespace JrhTrainInfoAndroid
{
    public class MyFragmentPagerAdapter : FragmentPagerAdapter
    {
        public MyFragmentPagerAdapter(FragmentManager fragmentManager) : base(fragmentManager)
        {

        }
        public override ICharSequence GetPageTitleFormatted(int position)
        {
            return null;
        }

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    return new AppHomeFragment();
                case 1:
                    return new TrainTimeStationSearchFragment();
                case 2:
                    return new TrainPositionLineSearchFragment();
                default:
                    return new SampleFragment();
            }
        }
        public override int Count => 4;
    }
}