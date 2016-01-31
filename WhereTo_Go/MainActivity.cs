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

namespace WhereTo_Go
{
	[Activity(Label = "WTG", MainLauncher = true)]
	public class MainActivity : Activity
	{
		private string userToken; 
		private OAuth2Authenticator auth;
		ISharedPreferences prefs;
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			SetContentView(Resource.Layout.Main);

			Button loginButton = FindViewById<Button>(Resource.Id.loginButton);
 			prefs = PreferenceManager.GetDefaultSharedPreferences(this);
			loginButton.Click += (object sender, EventArgs e) =>
			{
				if(!string.IsNullOrEmpty( prefs.GetString("token","")))
					{
						Intent intent= new Intent(this,typeof(SearchActivity));
						StartActivityForResult(intent, 0);
					}
				else
					{
						auth = Global.LogIn();
						auth.Completed += auth_Completed;
						StartActivity(auth.GetUI(this));
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
	
	}

}
