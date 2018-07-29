using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.Graphics;
using Android.Content.PM;
using Android.OS;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace xamarin
{
    [Activity(Label = "listActivity", ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class listActivity : Activity
    {
        private string json;
        private string storagePath;
        private string file;

        private ListView list;
        private SimpleListItem2_adapter listAdapter;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.listLayout);
            
            list = FindViewById<ListView>(Resource.Id.listScheme);

            list.SetBackgroundColor(Color.White);
            list.CacheColorHint = Color.Transparent;
            
            //path to external storage with json
            storagePath = Android.OS.Environment.ExternalStorageDirectory.Path;
            file = System.IO.Path.Combine(storagePath, "schemes.json");

            //if there is already a schemes.json file
            if (new FileInfo(file).Exists)
            {
                //read json
                json = File.ReadAllText(file);
                
                //deserialize into List<ColorWithText>
                var listColor = JsonConvert.DeserializeObject<List<ColorWithText>>(json);

                //pass list to the SimpleListItem2 adapter
                listAdapter = new SimpleListItem2_adapter(this, listColor);
                list.Adapter = listAdapter;
            }

            list.ItemClick += (sender, e) =>
            {
                //if item clicked start showActivity (show 4color scheme)
                try
                {
                    Intent nextActivity = new Intent(this, typeof(showActivity));
                    int item = (int)list.GetItemIdAtPosition(e.Position);
                    nextActivity.PutExtra("idScheme", item);
                    StartActivity(nextActivity);
                    Finish();
                }
                
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };
        }

        //on physical back pressed
        public override void OnBackPressed()
        {
            Intent nextActivity = new Intent(this, typeof(MainActivity));
            StartActivity(nextActivity);
            Finish();
        }
    }
}