﻿using System;
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
	public class EventListAdapter: BaseAdapter<Events> {
		List<Events> items;
		Activity context;
		public EventListAdapter(Activity context, List<Events> items)
			: base()
		{
			this.context = context;
			this.items = items;
		}
		public override long GetItemId(int position)
		{
			return position;
		}
		public override Events this[int position]
		{
			get { return items[position]; }
		}
		public override int Count
		{
			get { return items.Count; }
		}
		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			var item = items[position];

			View view = convertView;
			if (view == null) // no view to re-use, create new
				view = context.LayoutInflater.Inflate(Resource.Layout.RowView, null);
			view.FindViewById<TextView>(Resource.Id.Text1).Text = item.Name.ToString();
			view.FindViewById<TextView>(Resource.Id.Text2).Text = string.Format("Place: {0}{1}Users Confirmed: {2}{3}Users Interested: {4}{5}Start time: {6}",item.PlaceName,System.Environment.NewLine,item.Attending_count.ToString(),System.Environment.NewLine,item.Maybe_count.ToString(),System.Environment.NewLine,item.Start_time);
			return view;
		}
	}
}