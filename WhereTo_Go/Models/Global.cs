using System;
using System.Collections.Generic;
using Xamarin.Auth;
using Facebook;
using System.Threading.Tasks;
using System.Linq;
using Android.Widget;
using System.Xml.Serialization;
using System.IO;
using Android.Content;
using Android.Preferences;
using Android.Locations;

namespace WhereTo_Go
{
	public static class Global
	{
		public const string AppID = "188354251504525";
		public const string AppSecret = "e7c753704db8947d6a40ce4cd3a1833c";

		public static string Token {
			get;
			set;
		}

		public static List<Events> TodaysEvents {
			get;
			set;
		}	
		public static Coords GPSCoords {
			get;
			set;
		}
		public static OAuth2Authenticator LogIn()
		{
			return new OAuth2Authenticator (
				clientId: Global.AppID,
				scope: "publish_actions,user_location,user_about_me",
				authorizeUrl: new Uri ("https://m.facebook.com/dialog/oauth/"),
				redirectUrl: new Uri ("http://www.facebook.com/connect/login_success.html"));
		} 
		public static void PostToWall(string token)
		{

			FacebookClient fb= new FacebookClient(token);
				var postParams = new
				{
					
					name = "application_testing_name",
					caption = "application_testing_caption",
					description = "application_testing_description",
					link = "http://www.google.com",
				};
				//var orice=fb.Post ("me/feed", postParams);
			List<string> allPlaces= new List<string>();
	

		}
		public static string GetLocationID(string token)
		{
			
				FacebookClient fb = new FacebookClient (token);
				JsonObject result = (JsonObject)fb.Get ("me?fields=location", null);
				return (((JsonObject)result ["location"]) ["id"]).ToString (); 


		}

		public static Coords GetLongitudeAndLatitude(string token,string id,string localizationSetting)
		{if (localizationSetting == "facebook") 
			{
				FacebookClient fb = new FacebookClient (token);
				string query = id + "?fields=location";
				JsonObject result = (JsonObject)fb.Get (query, null);
				Coords coords = new Coords ();
				coords.Longitude = (((JsonObject)result ["location"]) ["longitude"]).ToString ();
				coords.Latitutde = (((JsonObject)result ["location"]) ["latitude"]).ToString ();
				return coords;
			}
			return GPSCoords;
		}



		public static JsonObject GetAllPlaces (string token, Coords coords)
		{
			FacebookClient fb= new FacebookClient(token);
			string query = string.Format ("search?limit=1000&type=place&center={0},{1}&distance=10000",coords.Latitutde, coords.Longitude);
			JsonObject result=(JsonObject)fb.Get (query, null);
			return result;

		}

		public static List<string> QueryParser(JsonObject fbQuery)
		{
			List<string> allPlacesIds = new List<string> ();
			JsonArray allPlaces= (JsonArray )fbQuery ["data"];
			foreach (var item in allPlaces)
				{
					allPlacesIds.Add(((JsonObject)item) ["id"].ToString());
				}

			return allPlacesIds;
		}

		public static string SerializeObject<T>(this T toSerialize)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());

			using(StringWriter textWriter = new StringWriter())
			{
				xmlSerializer.Serialize(textWriter, toSerialize);
				return textWriter.ToString();
			}
		}
			


	}
}

