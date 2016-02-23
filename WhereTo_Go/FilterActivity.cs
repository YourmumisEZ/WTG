
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Preferences;

namespace WhereTo_Go
{
	[Activity (Label = "FilterActivity")]			
	public class FilterActivity : Activity
	{
		ISharedPreferences prefs;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView(Resource.Layout.Filter);
			prefs = PreferenceManager.GetDefaultSharedPreferences(this);
			RadioButton radio_gps = FindViewById<RadioButton>(Resource.Id.radioButton1);
			RadioButton radio_fb = FindViewById<RadioButton>(Resource.Id.radioButton2);
			prefs = PreferenceManager.GetDefaultSharedPreferences(this);
			string localizationSetting = prefs.GetString("localization","");
			if (localizationSetting == "facebook") {
				radio_fb.Checked = true;
				radio_gps.Checked = false;
			} 
			else 
			{
				radio_fb.Checked = false;
				radio_gps.Checked = true;
			}
			radio_gps.Click += RadioButtonClick;
			radio_fb.Click += RadioButtonClick;

			Button backButton = FindViewById<Button>(Resource.Id.button1);
			backButton.Click += (object sender, EventArgs e) => {
				Intent intent = new Intent (this, typeof(MainActivity));
				StartActivityForResult (intent, 0);
			};
		}
		private void RadioButtonClick (object sender, EventArgs e)
		{
			string localizationSetting = "";
			RadioButton rb = (RadioButton)sender;
			if (rb.Text.Contains ("Facebook")) {
				localizationSetting="facebook";
			}
			if (rb.Text.Contains ("GPS")) {
				localizationSetting="gps";
			}
			ISharedPreferencesEditor editor = prefs.Edit();
			editor.PutString ("localization", localizationSetting);
			editor.Apply ();
		}
	}

}

