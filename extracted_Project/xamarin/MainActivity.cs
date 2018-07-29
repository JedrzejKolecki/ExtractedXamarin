using System;
using Android.App;
using Android.Widget;
using Android.Content.PM;
using Android.OS;
using Android.Support.V7.App;
using Plugin.Media;
using Android.Content;
using Android;

namespace xamarin
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true, ConfigurationChanges = ConfigChanges.Orientation, ScreenOrientation = ScreenOrientation.Portrait)]
	public class MainActivity : AppCompatActivity
	{
        private string path;

        private Button photoBtn;
        private Button imageBtn;
        private Button listBtn;

        protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);

            // check write external storage permission (for browsing schemes and gallery) - on start to be sure 
            if (CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Permission.Granted)
            {
                RequestPermissions(new String[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, 1);
            }
            
            //main menu buttons
            photoBtn = FindViewById<Button>(Resource.Id.button2);
            imageBtn = FindViewById<Button>(Resource.Id.button1);
            listBtn = FindViewById<Button>(Resource.Id.listButton);

            //take photo button - Xamarin Media Plugin
            photoBtn.Click += async (sender, args) =>
            {
                try
                {
                    await CrossMedia.Current.Initialize();
                    
                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        Toast.MakeText(this, "Camera not avaible. Check permissions.", ToastLength.Long).Show();
                        return;
                    }

                    //awaiting taking photo and saving to file in Sample directory
                    var file = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                    {
                        Directory = "Sample", 
                        Name = $"{DateTime.Now}_FULL|\\?*<\":>/'.jpg".Replace(" ", string.Empty),
                        SaveToAlbum = true,
                        PhotoSize = Plugin.Media.Abstractions.PhotoSize.Full,
                        DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Rear
                    });
                    
                    if (file == null)
                        return;

                    path = file.Path;
                    
                    //show path
                    Toast.MakeText(this, path, ToastLength.Long).Show();

                    //go to next activity 
                    Intent nextActivity = new Intent(this, typeof(programActivity));
                    
                    //path to file, for loading bitmap
                    nextActivity.PutExtra("path", path);
                    StartActivity(nextActivity);
                    Finish();
                }

                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };

            //choose image button
            imageBtn.Click += async delegate
            {
                try
                {
                    if (!CrossMedia.Current.IsPickPhotoSupported)
                    {
                        Toast.MakeText(this, "Picking images not avaible. Check permissions.", ToastLength.Long).Show();
                        return;
                    }

                    var file = await CrossMedia.Current.PickPhotoAsync();

                    if (file == null)
                        return;

                    path = file.Path;
                    
                    Intent nextActivity = new Intent(this, typeof(programActivity));
                    nextActivity.PutExtra("path", path);

                    StartActivity(nextActivity);
                    file.Dispose();
                    Finish();
                }

                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };

            //saved schemes list button
            listBtn.Click += delegate
            {
                try
                {
                    Intent nextActivity = new Intent(this, typeof(listActivity));
                    StartActivity(nextActivity);
                    Finish();
                }

                catch (Exception ex)
                {
                    Toast.MakeText(this, ex.Message, ToastLength.Long).Show();
                }
            };
        }

        //plugin media permissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
          Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}