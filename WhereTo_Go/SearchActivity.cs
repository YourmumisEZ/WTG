
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

		protected override void OnCreate (Bundle savedInstanceState)
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
			string localizationSetting = prefs.GetString("localization","");
			GetPlacesList (token,pBar,loading,localizationSetting);
		}
		private void GetPlacesList(string token,ProgressBar pBar, TextView loading,string localizationSetting)
		{
			List<Events> todaysEvents = new List<Events> ();
			string locationId = Global.GetLocationID (token);
			Coords coords = Global.GetLongitudeAndLatitude (token, locationId,localizationSetting);
			JsonObject allPlaces = Global.GetAllPlaces (token, coords);
			List<string> allPlacesIds=Global.QueryParser (allPlaces);
			ThreadPool.QueueUserWorkItem (o => GetEventList (token, allPlacesIds,pBar,loading));
		}

		public void GetEventList(string token,List<string> allPlacesIds,ProgressBar pBar,TextView loading)
		{
			DateTime now = DateTime.Now;
			DateTime tomorrow = DateTime.Now.AddDays(1);
			int count = 0;
			string dateNow = string.Format ("{0}-{1}-{2}",now.Year,now.Month,now.Day);
			string dateTomorrow = string.Format ("{0}-{1}-{2}",tomorrow.Year,tomorrow.Month,tomorrow.Day);
			List<Events> todaysEvents = new List<Events> ();
			FacebookClient fb= new FacebookClient(token);	
			foreach (var item in allPlacesIds) 
			{
				RunOnUiThread (() =>loading.Text = string.Format ("Loading {0} possible events out of {1}",count,allPlacesIds.Count));
				string query = string.Format ("{0}?&fields=id,name,events.fields(id,name,description,start_time,attending_count,declined_count,maybe_count,noreply_count).since({1}).until({2})", item,dateNow,dateTomorrow);
				JsonObject result=(JsonObject)fb.Get (query, null);
				try
				{
					JsonArray allEvents= (JsonArray)((JsonObject) result ["events"])["data"];

					foreach (var events in allEvents)
					{
						Events theEvent= new Events(((JsonObject)events) ["id"].ToString(),
										           ((JsonObject)events) ["name"].ToString(),
												   ((JsonObject)events) ["description"].ToString(),
												   ((JsonObject)events) ["start_time"].ToString(),
												   int.Parse(((JsonObject)events) ["attending_count"].ToString()),
												   int.Parse(((JsonObject)events) ["declined_count"].ToString()),
												   int.Parse(((JsonObject)events) ["maybe_count"].ToString()),
												   int.Parse(((JsonObject)events) ["noreply_count"].ToString()));
											
						todaysEvents.Add(theEvent);
					}
				}
				catch(Exception ex) 
				{
				}
				count++;
			}
			Global.TodaysEvents= todaysEvents;
			Intent intent= new Intent(this,typeof(EventListActivity));
			StartActivity(intent);
		}


	}
}

