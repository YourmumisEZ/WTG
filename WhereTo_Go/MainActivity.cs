using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using System;
using Xamarin.Auth;
using System.Collections.Generic;
using System.Linq;
using Facebook;
using Android.Preferences;
using Android.Locations;
using Android.Net;

namespace WhereTo_Go
{
	[Activity(Label = "WTG", MainLauncher = true)]
	public class MainActivity : Activity,ILocationListener
	{
		private string userToken; 
		private OAuth2Authenticator auth;
		ISharedPreferences prefs;
		LocationManager _locationManager;
		IList<string> acceptableLocationProviders;
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);
			ConnectivityManager connectivityManager = (ConnectivityManager) GetSystemService(ConnectivityService);
			NetworkInfo activeConnection = connectivityManager.ActiveNetworkInfo;


			Button loginButton = FindViewById<Button>(Resource.Id.loginButton);
			Button filteButton = FindViewById<Button> (Resource.Id.button1);
 			prefs = PreferenceManager.GetDefaultSharedPreferences(this);
			_locationManager = GetSystemService (Context.LocationService) as LocationManager;
			Criteria criteriaForLocationService = new Criteria
			{
				Accuracy = Accuracy.Fine
			};
			acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

				
				filteButton.Click += (object sender, EventArgs e) => {
					Intent intent = new Intent (this, typeof(FilterActivity));
					StartActivityForResult (intent, 0);
				};

				loginButton.Click += (object sender, EventArgs e) => {
				bool isOnline = (activeConnection != null) && activeConnection.IsConnected;
				bool wifiIsOnline = (connectivityManager.GetNetworkInfo(ConnectivityType.Wifi)).IsConnected;
				if (isOnline || wifiIsOnline) {
					
					if (!string.IsNullOrEmpty (prefs.GetString ("token", ""))) {
						Intent intent = new Intent (this, typeof(SearchActivity));
						StartActivityForResult (intent, 0);
					} else {
						auth = Global.LogIn ();
						auth.Completed += auth_Completed;
						StartActivity (auth.GetUI (this));
					}
				}
				else 
				{
					AlertDialog.Builder alert = new AlertDialog.Builder (this);
					alert.SetTitle ("Internet connection error");
					alert.SetMessage ("Turn wifi or mobile data on");
					alert.SetPositiveButton ("Ok", (senderAlert, args) => {

					});
					Dialog dialog = alert.Create();
					dialog.Show();
				}
			};
		
			} 
			

		void auth_Completed(object sender, AuthenticatorCompletedEventArgs e)
		{
			if (e.Account != null)
			{
				
				userToken = e.Account.Properties["access_token"];
				ISharedPreferencesEditor editor = prefs.Edit();
				editor.PutString ("token", userToken);
				editor.Apply ();
				Intent intent= new Intent(this,typeof(SearchActivity));
				StartActivityForResult(intent, 0);

			}
		}
		protected override void OnResume ()
		{
			base.OnResume ();
			if (acceptableLocationProviders.Count != 0) {
				string Provider = acceptableLocationProviders.First ();

				if (_locationManager.IsProviderEnabled (Provider)) {
					_locationManager.RequestLocationUpdates (Provider, 2000, 0, this);
					Coords thisCoords = new Coords (_locationManager.GetLastKnownLocation (Provider).Longitude.ToString (), _locationManager.GetLastKnownLocation (Provider).Latitude.ToString ());
					Global.GPSCoords = thisCoords;

				}
			}

		}


		public void OnLocationChanged (Location location)
		{
			string Provider = acceptableLocationProviders.First();
			Coords thisCoords = new Coords (_locationManager.GetLastKnownLocation(Provider).Longitude.ToString(),_locationManager.GetLastKnownLocation(Provider).Latitude.ToString());
			Global.GPSCoords = thisCoords;
		}
		public void OnProviderDisabled (string provider)
		{
	
		}
		public void OnProviderEnabled (string provider)
		{
			
		}
		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
			
		}
		protected override void OnPause ()
		{
			base.OnPause ();
			_locationManager.RemoveUpdates (this);
		}
	
	}

}
