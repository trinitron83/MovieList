using System;
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
		MovieListResponse Movies;

		public MasterViewController()
		{
			Movies = new MovieListResponse();
		}

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();

			await GetMovies();

			int rowOffset = 0;
			double posterWidth = UIScreen.MainScreen.Bounds.Width/3;
			int posterHeight = 200;

			var verticalScroll = new UIScrollView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Width, UIScreen.MainScreen.Bounds.Height));

			for (int i = 0; i < 5; i++)
			{
				var row = new UIScrollView(new CGRect(0, rowOffset, UIScreen.MainScreen.Bounds.Width, posterHeight));
				row.ContentSize = new CGSize(UIScreen.MainScreen.Bounds.Width * 3, 50);
				row.PagingEnabled = true;

				double posterOffset = 0;

				foreach (Movie m in Movies.results)
				{
					if (!string.IsNullOrEmpty(m.poster_path))
					{
						var poster = new UIImageView(new CGRect(posterOffset, 0, posterWidth, posterHeight));

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

						posterOffset += posterWidth;
					}
				}

				verticalScroll.Add(row);
				rowOffset += posterHeight;
			}
			View.Add(verticalScroll);
		}

		async Task GetMovies()
		{
			Movies = await new API().SendGetRequest<MovieListResponse>("http://api.themoviedb.org/3/movie/now_playing?api_key=ab41356b33d100ec61e6c098ecc92140&sort_by=popularity.des");
		}
	}
}

