
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
namespace WhereTo_Go
{
	[Activity (Label = "WTG")]			
	public class EventListActivity : Activity
	{
		List<Events> orderdEvents = new List<Events> ();
		protected override void OnCreate (Bundle savedInstanceState)
		{
			base.OnCreate (savedInstanceState);
			SetContentView (Resource.Layout.EventsList);
			SetUpLabel();
			orderdEvents= Global.TodaysEvents.OrderByDescending(o=> o.Attending_count).ThenByDescending(o=>o.Maybe_count).ThenByDescending(o=>o.TotalCount).ToList();
			ListView listView = FindViewById<ListView>(Resource.Id.listView1);
			listView.Adapter = new EventListAdapter (this, orderdEvents);
			listView.ItemClick += OnListItemClick;
		}

		public void SetUpLabel()
		{
			DateTime now = DateTime.Now;
			string dateNow = string.Format ("{0}-{1}-{2}",now.Year,now.Month,now.Day);
			TextView label = FindViewById<TextView> (Resource.Id.textView1);
			label.Text = string.Format ("Events for {0} :", dateNow);
		}

		protected void OnListItemClick(object sender, Android.Widget.AdapterView.ItemClickEventArgs e)
		{
			var listView = sender as ListView;
			var t = orderdEvents[e.Position];

			Android.Net.Uri uri = Android.Net.Uri.Parse(string.Format("http://www.facebook.com/{0}",t.Id));
			Intent intent = new Intent (Intent.ActionView);
			intent.SetData (uri);

			Intent chooser = Intent.CreateChooser (intent, "Open with");

			this.StartActivity(chooser);
		}

	}
}

