
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Facebook;
using Xamarin.Facebook;
using Android.Preferences;
using Android.Locations;

namespace WhereTo_Go
{
	[Activity (Label = "WTG", ConfigurationChanges=Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]			
	public class SearchActivity : Activity
	{
		TextView loading;
		ProgressBar pBar;
		public string userToken;
		ISharedPreferences prefs;
		string localizationSetting;
		protected override void OnCreate (Bundle savedInstanceState)
		{
			try
			{
				base.OnCreate (savedInstanceState);
				SetContentView(Resource.Layout.Search);
				loading = FindViewById<TextView>(Resource.Id.textView1);
				pBar = FindViewById<ProgressBar>(Resource.Id.searchPB);	
				pBar.Visibility = ViewStates.Visible;
				loading.Visibility = ViewStates.Visible;
				loading.Text="Loading events";
				prefs = PreferenceManager.GetDefaultSharedPreferences(this);
				string token = prefs.GetString("token","");
				localizationSetting = prefs.GetString("localization","");
				if (localizationSetting == "gps" && Global.GPSCoords == null) 
					{
						AlertDialog.Builder alert = new AlertDialog.Builder (this);
						alert.SetTitle ("GPS error");
						alert.SetMessage ("Turn gps on");
						alert.SetPositiveButton ("Ok", (senderAlert, args) => 
							{
								Intent intent = new Intent (this, typeof(MainActivity));
								StartActivityForResult (intent, 0);
							}
						);
					Dialog dialog = alert.Create ();
					dialog.Show ();
					} 
					else 
					{
						GetPlacesList (token,localizationSetting);
					}
			}
			catch(Exception ex)
			{
				throw ex;
			}
		}
		private void GetPlacesList(string token,string localizationSetting)
		{
			try
			{
			List<Events> todaysEvents = new List<Events> ();
			string locationId = "";
			Coords coords;
			if (localizationSetting == "gps") 
			{
				 coords = Global.GetLongitudeAndLatitude ("", "",localizationSetting);

			}
			else 
			{
				 locationId = Global.GetLocationID (token);
				 if(locationId=="Error")
					{
						AlertDialog.Builder alert = new AlertDialog.Builder (this);
						alert.SetTitle ("Localization Error");
						alert.SetMessage ("You have not set the city you live in, on facebook. Either set it or use GPS localization");
						alert.SetPositiveButton ("Ok", (senderAlert, args) => 
							{
								Intent intent = new Intent (this, typeof(MainActivity));
								StartActivityForResult (intent, 0);
							}
						);
						Dialog dialog = alert.Create ();
						dialog.Show ();
						return;
					}

				 coords = Global.GetLongitudeAndLatitude (token, locationId,localizationSetting);
			}
			JsonObject allPlaces = Global.GetAllPlaces (token, coords);
			List<string> allPlacesIds=Global.QueryParser (allPlaces);
			ThreadPool.QueueUserWorkItem (o => GetEventList (token, allPlacesIds));
			}
			catch(Exception ex){
				throw ex;
			}
		}
		public void GetEventList(string token,List<string> allPlacesIds)
		{
			DateTime now = DateTime.Now;
			DateTime tomorrow = DateTime.Now.AddDays(1);
			int count = 0;
			string dateNow = string.Format ("{0}-{1}-{2}",now.Year,now.Month,now.Day);
			string dateTomorrow = string.Format ("{0}-{1}-{2}",tomorrow.Year,tomorrow.Month,tomorrow.Day);
			List<Events> todaysEvents = new List<Events> ();
			FacebookClient fb= new FacebookClient(token);

			for(int i=0;i<allPlacesIds.Count;i=i+50)
			{
				string ids = allPlacesIds.Skip(i).Take(50).GenerateFBQuery ();
				string query = string.Format ("{0}&fields=id,name,events.fields(id,name,description,start_time,attending_count,declined_count,maybe_count,noreply_count).since({1}).until({2})", ids,dateNow,dateTomorrow);
				JsonObject result=(JsonObject)fb.Get (query, null);
				try
				{
					for( int j=0;j<50;j++)
					{
							if(((JsonObject)result[j]).ToString().Contains("events"))
							{
								JsonObject auxResult=(JsonObject)result[j];
								JsonArray allEvents= (JsonArray)((JsonObject) auxResult ["events"])["data"];
								foreach (var events in allEvents)
								{
									Events theEvent= new Events(((JsonObject)events) ["id"].ToString(),
																((JsonObject)events) ["name"].ToString(),
																((JsonObject)events) ["description"].ToString(),
																((JsonObject)events) ["start_time"].ToString(),
																int.Parse(((JsonObject)events) ["attending_count"].ToString()),
																int.Parse(((JsonObject)events) ["declined_count"].ToString()),
																int.Parse(((JsonObject)events) ["maybe_count"].ToString()),
																int.Parse(((JsonObject)events) ["noreply_count"].ToString()),
																((JsonObject)auxResult) ["name"].ToString());

									todaysEvents.Add(theEvent);
								}
							}
					}
				}
				catch(Exception ex) 
				{
				}
			}
			Global.TodaysEvents= todaysEvents;
			Intent intent= new Intent(this,typeof(EventListActivity));
			StartActivity(intent);
		}

	}
}

