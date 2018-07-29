using System.Collections.Generic;
using Android.App;
using Android.Views;
using Android.Widget;

namespace xamarin
{
    public class SimpleListItem2_adapter : BaseAdapter<ColorWithText>
    {
        //SimpleListItem2 adapter
        List<ColorWithText> localList;
        Activity context;

        public SimpleListItem2_adapter(Activity currentContext, List<ColorWithText> colorWithText) : base()
        {
            this.localList = colorWithText;
            this.context = currentContext;
        }

        public override ColorWithText this[int position]
        {
            get
            {
                return localList.ToArray()[position];
            }
        }

        public override int Count
        {
            get
            {
                return localList.Count;
            }
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = convertView;
            if (view == null)
            {
                string Hex = localList.ToArray()[position].GetHex();
                
                view = context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem2, null);
                //first line
                view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = localList.ToArray()[position].schemeName;
                //second line
                view.FindViewById<TextView>(Android.Resource.Id.Text2).Text = Hex;
            }
            return view;
        }
    }
}