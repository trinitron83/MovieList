using System;
using ObjCRuntime;
using System.Collections.Generic;
using UIKit;
using Foundation;
using CoreGraphics;
using System.Threading.Tasks;
using MovieListPCL;
using SDWebImage;

namespace MovieList
{
	public class MasterViewController : UIViewController
	{
		List<MovieListResponse> Movies;
		List<string> APIUrls;

		public MasterViewController()
		{
			APIUrls = new List<string> { APIEndpoints.NowPlayingURL, APIEndpoints.PopularURL, APIEndpoints.TopRatedURL };
			Movies = new List<MovieListResponse>();
		}

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();

			//pull movie lists from api
			await GetMovies();

			double xOffset = 0, yOffset = 25;
			double xPadding = 10, yPadding = 10;
			double posterWidth = AppDelegate.ScreenWidth/3 - 2*xPadding;
			double posterHeight = AppDelegate.ScreenHeight/3 - 2*yPadding;

			//vertical scroll to hold multiple rows of movies
			var verticalScroll = new UIScrollView(new CGRect(0, 0, AppDelegate.ScreenWidth, AppDelegate.ScreenHeight));
			verticalScroll.ContentSize = new CGSize(AppDelegate.ScreenWidth, posterHeight * APIUrls.Count);
			verticalScroll.CanCancelContentTouches = false;

			foreach (MovieListResponse response in Movies)
			{
				//layout each row of movies
				var row = new UIScrollView(new CGRect(xPadding, yOffset, AppDelegate.ScreenWidth, posterHeight));
				row.CanCancelContentTouches = true;
				row.ContentSize = new CGSize(AppDelegate.ScreenWidth * response.results.Count, posterHeight);
				row.PagingEnabled = true;

				xOffset = 0;

				foreach (Movie m in response.results)
				{
					if (!string.IsNullOrEmpty(m.poster_path))
					{
						var poster = new UIImageView(new CGRect(xOffset, 0, posterWidth, posterHeight));
						poster.UserInteractionEnabled = true;
						poster.AddGestureRecognizer(new UITapGestureRecognizer(() => {
							PresentViewController(new MovieDetailController(m), true, null);
						}));

						var manager = SDWebImageManager.SharedManager.ImageDownloader;
						var imageNSUrl = new NSUrl("https://image.tmdb.org/t/p/w600_and_h900_bestv2/" + m.poster_path.Replace("\\", ""));

						manager.DownloadImage(imageNSUrl, SDWebImageDownloaderOptions.UseNSUrlCache, (s, e) =>
							{
							},
							((UIImage image, NSData data, NSError error, bool finished) =>
							{
								if (image != null && finished)
								{
									BeginInvokeOnMainThread(() =>
									{
										poster.Image = image;
									});
								}
							})
						);

						row.Add(poster);
						xOffset += posterWidth + 2*xPadding;
					}
				}

				verticalScroll.Add(row);
				yOffset += posterHeight + 2*yPadding;
			}
			View.Add(verticalScroll);
		}

		async Task GetMovies()
		{
			foreach (string url in APIUrls)
			{
				Movies.Add(await new API().SendGetRequest<MovieListResponse>(url));
			}
		}
	}
}