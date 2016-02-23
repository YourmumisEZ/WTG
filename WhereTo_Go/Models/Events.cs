using System;
using Android.OS;
using Java.Interop;

namespace WhereTo_Go
{
	public class Events 
	{

		public string Id {
			get;
			set;
		}
		public string Name {
			get;
			set;
		}
		public string Description {
			get;
			set;
		}
		public string Start_time {
			get;
			set;
		}
		public int Attending_count {
			get;
			set;
		}
		public int Declined_count {
			get;
			set;
		}
		public int Maybe_count {
			get;
			set;
		}
		public int Noreply_count{
			get;
			set;
		}
		public int TotalCount{
			get;
			set;
		}
		public string PlaceName {
			get;
			set;
		}
		public Events (string id,string name,string description,string start_time,int attending_Count,int declined_count,int maybe_count,int noreply_count,string placeName)
		{

			this.Id = id;
			this.Name = name;
			this.Description = description;
			this.Start_time = start_time;
			this.Attending_count = attending_Count;
			this.Declined_count = Declined_count;
			this.Maybe_count = maybe_count;
			this.Noreply_count = noreply_count;
			this.TotalCount = attending_Count + declined_count + maybe_count + noreply_count;
			this.PlaceName = placeName;
		}
	}
}

