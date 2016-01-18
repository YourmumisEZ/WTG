
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
using Facebook;
using Android.Webkit;


namespace WhereTo_Go
{
	[Activity(Label = "FBLoginActivity")]
	public class FBLoginActivity : Activity
	{
		private FacebookClient fb;
		public string url { get; set; }
		public string permissions { get; set; }
		public string appId { get; set; }
		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);
			fb = new FacebookClient();
			appId = Intent.GetStringExtra("AppId");
			permissions = Intent.GetStringExtra("Permissions");
			url = GetFacebookLoginUrl(appId, permissions);

			SetContentView (Resource.Layout.Login);
			WebView wv = FindViewById<WebView> (Resource.Id.FBWebView);
			wv.SetWebViewClient (new WebViewClient ());
			wv.Settings.JavaScriptEnabled = true;
			wv.LoadUrl (url);
//			WebView wv = new WebView (this);
//			wv.Settings.JavaScriptEnabled = true;
//			wv.Settings.SetSupportZoom(true);
//			wv.Settings.BuiltInZoomControls = true;
//			wv.Settings.LoadWithOverviewMode = true;
//			wv.ScrollBarStyle = ScrollbarStyles.OutsideOverlay;
//			wv.ScrollbarFadingEnabled = true;
//			wv.VerticalScrollBarEnabled = true;
//			wv.HorizontalScrollBarEnabled = true;			
//			//wv.SetWebViewClient (new FBWebClient (this));
//			//AddContentView(wv,new ViewGroup.LayoutParams(ViewGroup.LayoutParams.FillParent));
//			wv.LoadUrl(url);				
	}

		private string GetFacebookLoginUrl(string appId,string permissions)
		{
			var parameters = new Dictionary<string, object>();
			parameters["client_id"] = appId;
			parameters["redirect_uri"] = "https://www.facebook.com/connect/login_success.html";
			parameters["response_type"] = "token";
			parameters["display"] = "touch";
			parameters["scope"] = permissions;
			return fb.GetLoginUrl(parameters).AbsoluteUri;

		}
	}
}