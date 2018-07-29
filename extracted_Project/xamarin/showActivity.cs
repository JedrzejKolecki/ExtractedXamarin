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
    //listview item pressed Activity
    [Activity(Label = "showActivity",ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class showActivity : Activity
    {
        private TextView hexText;

        private Button showButtonOne;
        private Button showButtonTwo;
        private Button showButtonThree;
        private Button showButtonFour;

        private string json;
        private string storagePath;
        private string file;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.showLayout);

            hexText = FindViewById<TextView>(Resource.Id.textColors);
            ColorWithText colorScheme;

            //buttons that represent color scheme
            showButtonOne = FindViewById<Button>(Resource.Id.showFirstBTN);
            showButtonTwo = FindViewById<Button>(Resource.Id.showSecondBTN);
            showButtonThree = FindViewById<Button>(Resource.Id.showThirdBTN);
            showButtonFour = FindViewById<Button>(Resource.Id.showFourthBTN);
           
            storagePath = Android.OS.Environment.ExternalStorageDirectory.Path;
            file = System.IO.Path.Combine(storagePath, "schemes.json");

            //if json file exists then read it else load empty list
            if (new FileInfo(file).Exists)
            {
                //read json
                json = File.ReadAllText(file);

                //deserialize json into List<ColorWithText>
                var listColor = JsonConvert.DeserializeObject<List<ColorWithText>>(json);
                
                int findID = Intent.GetIntExtra("idScheme", 0) ;
                colorScheme = listColor.Find(x => x.id == findID);

                showButtonOne.SetBackgroundColor(colorScheme.color_First);
                showButtonTwo.SetBackgroundColor(colorScheme.color_Second);
                showButtonThree.SetBackgroundColor(colorScheme.color_Third);
                showButtonFour.SetBackgroundColor(colorScheme.color_Fourth);

                //RGB + HEX display
                hexText.SetTextColor(Color.White);
                hexText.Text = "RGB:\n\n" + colorScheme.GetRGB() + "\n\nHEX:\n\n" + colorScheme.GetHex();
            }

            else
            {
                Toast.MakeText(this, "Scheme not found.", ToastLength.Long).Show();
                Finish();
            }
            
        }

        //on physical back pressed
        public override void OnBackPressed()
        {
            Intent nextActivity = new Intent(this, typeof(listActivity));
            StartActivity(nextActivity);
            Finish();
        }
    }
}