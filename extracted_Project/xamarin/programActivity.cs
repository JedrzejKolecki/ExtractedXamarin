using System;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.Graphics;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Graphics.Drawables;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;

namespace xamarin
{
    [Activity(Label = "programActivity", ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
    public class programActivity : Activity
    {
        private Bitmap bitmap;

        private Button firstColor;
        private Button secondColor;
        private Button thirdColor;
        private Button fourthColor;

        private FloatingActionButton saveButton;

        private Color firstBackgroundColor;
        private Color secondBackgroundColor;
        private Color thirdBackgroundColor;
        private Color fourthBackgroundColor;

        private Color getColor;
        private ColorWithText scheme;

        private string json;
        private string storagePath;
        private string file;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.secondLayout);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);
            
            List<ColorWithText> listColor = new List<ColorWithText>();

            string path = Intent.GetStringExtra("path");

            //resize image if too big (Android will crash if file loaded into bitmap is too big)
            BitmapFactory.Options options = new BitmapFactory.Options();
            options.InJustDecodeBounds = true;
            BitmapFactory.DecodeFile(path,options);
            options.InSampleSize = calculateInSampleSize(options, 1000, 1000); //max size 1000x1000, else - scale
            options.InJustDecodeBounds = false;

            bitmap = BitmapFactory.DecodeFile(path, options);
            Toast.MakeText(this, "Resized sample by: " + options.InSampleSize, ToastLength.Long).Show();

            //set bitmap for image
            var image = FindViewById<ImageView>(Resource.Id.imageChoose);
            image.SetImageBitmap(bitmap);

            image.DrawingCacheEnabled=true;
            var drawable =  image.Drawable as BitmapDrawable;
            if (drawable != null)
            {
                bitmap = drawable.Bitmap;
            }

            //get buttons
            firstColor = FindViewById<Button>(Resource.Id.color1);
            secondColor = FindViewById<Button>(Resource.Id.color2);
            thirdColor = FindViewById<Button>(Resource.Id.color3);
            fourthColor = FindViewById<Button>(Resource.Id.color4);

            //get fab
            saveButton = FindViewById<FloatingActionButton>(Resource.Id.saveBtn);

            //default color - white for all
            firstBackgroundColor = Color.White;
            firstColor.SetBackgroundColor(firstBackgroundColor);

            secondBackgroundColor = Color.White;
            secondColor.SetBackgroundColor(secondBackgroundColor);

            thirdBackgroundColor = Color.White;
            thirdColor.SetBackgroundColor(thirdBackgroundColor);

            fourthBackgroundColor = Color.White;
            fourthColor.SetBackgroundColor(fourthBackgroundColor);
            
            //if color button clicked then set it to active and deactivate other buttons so you can change only 1 color
            firstColor.Click += delegate
            {
                try
                {
                    firstColor.Activated = true;
                    
                    secondColor.Activated = false;
                    thirdColor.Activated = false;
                    fourthColor.Activated = false;
                }

                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };

            secondColor.Click += delegate
            {
                try
                {
                    secondColor.Activated = true;

                    firstColor.Activated = false;
                    thirdColor.Activated = false;
                    fourthColor.Activated = false;
                }

                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };

            thirdColor.Click += delegate
            {
                try
                {
                    thirdColor.Activated = true;

                    firstColor.Activated = false;
                    secondColor.Activated = false;
                    fourthColor.Activated = false;
                }

                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };

            fourthColor.Click += delegate
            {
                try
                {
                    fourthColor.Activated = true;

                    firstColor.Activated = false;
                    secondColor.Activated = false;
                    thirdColor.Activated = false;
                }

                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };
            
            //touch point on image and get color 
            image.Touch += (s, e) =>
            {
                var handled = false;
                
                try
                {
                    if (e.Event.Action == MotionEventActions.Down)
                    { 
                        bitmap = image.GetDrawingCache(true);
                        getColor = new Color(bitmap.GetPixel((int)e.Event.GetX(), (int)e.Event.GetY()));

                        //set color to activated button
                        if (firstColor.Activated == true)
                        {
                            firstBackgroundColor = getColor;
                            firstColor.SetBackgroundColor(getColor);
                        }

                        if (secondColor.Activated == true)
                        {
                            secondBackgroundColor = getColor;
                            secondColor.SetBackgroundColor(getColor);
                        }

                        if (thirdColor.Activated == true)
                        {
                            thirdBackgroundColor = getColor;
                            thirdColor.SetBackgroundColor(getColor);
                        }

                        if (fourthColor.Activated == true)
                        {
                            fourthBackgroundColor = getColor;
                            fourthColor.SetBackgroundColor(getColor);
                        }
                        handled = true;
                    }
                    e.Handled = handled;
                }

                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };

            //save 4-color scheme 
            saveButton.Click += delegate
            {
                try
                {
                    //popout window with OK and cancel buttons that will ask to enter scheme name 
                    Android.App.AlertDialog.Builder alert = new Android.App.AlertDialog.Builder(this);
                    alert.SetTitle("Name");
                    EditText input = new EditText(this);
                    alert.SetMessage("Enter name for your new scheme.");
                    alert.SetView(input);
                    
                    alert.SetPositiveButton("OK", (senderAlert, args) => 
                    {
                        storagePath = Android.OS.Environment.ExternalStorageDirectory.Path;
                        file = System.IO.Path.Combine(storagePath, "schemes.json");

                        if (new FileInfo(file).Exists)
                        {
                            json = File.ReadAllText(file);
                            listColor = JsonConvert.DeserializeObject<List<ColorWithText>>(json);
                        }

                        if (input.Text.Length == 0)
                            input.Text = "defaultNameScheme";

                        scheme = new ColorWithText(firstBackgroundColor, secondBackgroundColor, thirdBackgroundColor, fourthBackgroundColor, input.Text, listColor.Count);
                        listColor.Add(scheme);
                        
                        using (StreamWriter sw = new StreamWriter(file, false))
                        {
                            JsonSerializer serializer = new JsonSerializer();
                            serializer.Formatting = Formatting.Indented;
                            serializer.Serialize(sw, listColor);
                        }
                        Toast.MakeText(this, "Scheme succesfully saved.", ToastLength.Short).Show();
                        
                        //go to listview of schemes
                        Intent nextActivity = new Intent(this, typeof(listActivity));
                        StartActivity(nextActivity);
                        Finish();
                    });

                    alert.SetNegativeButton("Cancel", (senderAlert, args) => 
                    {
                        //go back to main menu
                        Toast.MakeText(this, "Scheme dropped.", ToastLength.Short).Show();
                        Intent nextActivity = new Intent(this, typeof(MainActivity));
                        StartActivity(nextActivity);
                        Finish();
                    });

                    Dialog dialog = alert.Create();
                    dialog.Show();
                }
                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };
        }

        //if physical back pressed
        public override void OnBackPressed()
        {
            Intent nextActivity = new Intent(this, typeof(MainActivity));
            StartActivity(nextActivity);
            Finish();
        }

        //resizing image (bitmapfactory InSampleSize)
        public static int calculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            int height = options.OutHeight;
            int width = options.OutWidth;
            int inSampleSize = 1;

            if (height > reqHeight || width > reqWidth)
            {

                int halfHeight = height / 2;
                int halfWidth = width / 2;

                // Calculate the largest inSampleSize value that is a power of 2 and keeps both
                // height and width larger than the requested height and width.
                while ((halfHeight / inSampleSize) > reqHeight
                        && (halfWidth / inSampleSize) > reqWidth)
                {
                    inSampleSize *= 2;
                }
            }
            return inSampleSize;
        }
    }
}
