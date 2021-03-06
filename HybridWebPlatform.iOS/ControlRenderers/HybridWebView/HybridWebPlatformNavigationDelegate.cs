﻿using System;
using Foundation;
using WebKit;

namespace HybridWebPlatform.iOS
{
	public class HybridWebPlatformNavigationDelegate : WKNavigationDelegate
	{
		public event Action<string, string, int> ReceivedError;
		public event Action<string> StartLoadingUrl;
		public event Action<string> FinishedLoadingUrl;
		public event Func<string, bool> ShouldStartPageLoading;
		public event Action<Uri> OpenExternalWindow;

		public HybridWebPlatformNavigationDelegate()
		{
		}

		public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
		{
			var action = WKNavigationActionPolicy.Allow;

			if (navigationAction.TargetFrame == null)
			{
				if (OpenExternalWindow != null)
				{
					OpenExternalWindow(new Uri(navigationAction.Request.Url.AbsoluteString));
				}
				decisionHandler(WKNavigationActionPolicy.Cancel);
			}

			if (ShouldStartPageLoading != null)
			{
				action = ShouldStartPageLoading(navigationAction.Request.Url.AbsoluteString) ? WKNavigationActionPolicy.Allow : WKNavigationActionPolicy.Cancel;
			}

			decisionHandler(action);
		}

		public override void DecidePolicy(WKWebView webView, WKNavigationResponse navigationResponse, Action<WKNavigationResponsePolicy> decisionHandler)
		{
			NSHttpUrlResponse response = navigationResponse.Response as NSHttpUrlResponse;
			NSHttpCookie[] cookies = NSHttpCookie.CookiesWithResponseHeaderFields(response.AllHeaderFields, response.Url);

			foreach (NSHttpCookie cookie in cookies)
			{
				NSHttpCookieStorage.SharedStorage.SetCookie(cookie);
			}

			decisionHandler(WKNavigationResponsePolicy.Allow);
		}

		public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
		{
			if (StartLoadingUrl != null)
			{
				StartLoadingUrl(webView.Url.AbsoluteString);
			}
		}

		public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
		{
			if (FinishedLoadingUrl != null)
			{
				FinishedLoadingUrl(webView.Url.AbsoluteString);
			}
		}

		public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
		{
			if (ReceivedError != null)
			{
				string description = "";
				int errorCode = 0;
				string url = "";

				if (error != null)
				{
					description = error.Description;
					errorCode = (int)error.Code;
				}

				if (webView.Url != null)
				{
					url = webView.Url.AbsoluteString;
				}

				ReceivedError(url, description, errorCode);
			}
		}

		public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
		{
			if (ReceivedError != null)
			{
				string description = "";
				int errorCode = 0;
				string url = "";

				if (error != null)
				{
					description = error.Description;
					errorCode = (int)error.Code;
				}

				if (webView.Url != null)
				{
					url = webView.Url.AbsoluteString;
				}

				ReceivedError(url, description, errorCode);
			}
		}
	}
}
