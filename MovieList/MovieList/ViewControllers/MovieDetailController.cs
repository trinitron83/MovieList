using System.Collections.Generic;
using System.Threading.Tasks;
using UIKit;
using Foundation;
using CoreGraphics;
using MovieListPCL;
using SDWebImage;

namespace MovieList
{
	public class MovieDetailController : UIViewController
	{
		Movie Movie { get; set; }
		List<Movie> Favorites { get; set; }
		MovieListResponse SimilarMovies { get; set; }
		Storage storage { get; set; }
		bool isFavorite;

		public MovieDetailController(Movie m)
		{
			Movie = m;
			View.BackgroundColor = UIColor.Clear;
			storage = new Storage();
		}

		public override async void ViewDidLoad()
		{
			base.ViewDidLoad();

			await GetSimilarMovies();

			Favorites = storage.GetFavorites();

			var posterView = new UIImageView(new CGRect(10, 50, 150, 250));
			var xView = new UIImageView(new CGRect(AppDelegate.ScreenWidth - 50, 20, 50, 30));
			xView.Image = new UIImage("PS_X");
			xView.ContentMode = UIViewContentMode.ScaleAspectFit;
			xView.UserInteractionEnabled = true;
			xView.AddGestureRecognizer(new UITapGestureRecognizer(() => {
				DismissViewController(true, null);
			}));

			var title = new UILabel(new CGRect(170, 50, AppDelegate.ScreenWidth - 180, 50));
			title.Text = Movie.title;
			title.Font = UIFont.FromName("Arial-BoldMT", 17);
			title.TextColor = UIColor.White;
			title.Lines = 3;
			title.SizeToFit();

			var dateLabel = new UILabel(new CGRect(170, title.Frame.Height + 65, AppDelegate.ScreenWidth - 180, 20));
			dateLabel.Font = UIFont.FromName("Arial", 15);
			dateLabel.Text = Strings.ReleaseDate + Movie.release_date.Replace("-", "/");
			dateLabel.TextColor = UIColor.White;

			var ratingLabel = new UILabel(new CGRect(170, title.Frame.Height + 85, AppDelegate.ScreenWidth - 180, 20));
			ratingLabel.Font = UIFont.FromName("Arial", 15);
			ratingLabel.Text = "Rating: " + Movie.popularity.ToString("F2");
			ratingLabel.TextColor = UIColor.White;

			var playButton = new UIButton(new CGRect(170, 210, 120, 35));
			playButton.BackgroundColor = UIColor.FromRGB(146, 195, 21);
			playButton.SetTitle(Strings.Play, UIControlState.Normal);

			var saveButton = new UIButton(new CGRect(170, 260, 160, 35));
			saveButton.BackgroundColor = UIColor.FromRGB(208, 163, 8);
			saveButton.SetTitle(Strings.Save, UIControlState.Normal);

			if (Favorites != null)
			{
				foreach (Movie favorite in Favorites)
				{
					if (favorite.id == Movie.id)
					{
						saveButton.SetTitle(Strings.Remove, UIControlState.Normal);
						saveButton.SizeToFit();
						isFavorite = true;
					}
				}
			}

			saveButton.TouchUpInside += (sender, e) => {
				if (!isFavorite) {
					saveButton.SetTitle(Strings.Remove, UIControlState.Normal);
					saveButton.SizeToFit();
					storage.AddToFavorites(Movie);
					isFavorite = true;
				}
				else {
					saveButton.SetTitle(Strings.Save, UIControlState.Normal);
					saveButton.SizeToFit();
					storage.RemoveFromFavorites(Movie);
					isFavorite = false;
				}
			};

			var descriptionLabel = new UILabel(new CGRect(15, 50 + posterView.Frame.Height + 15, AppDelegate.ScreenWidth - 30, 50));
			descriptionLabel.Text = Movie.overview;
			descriptionLabel.Font = UIFont.FromName("Arial", 13);
			descriptionLabel.TextColor = UIColor.White;
			descriptionLabel.Lines = 10;
			descriptionLabel.SizeToFit();

			var similarLabel = new UILabel(new CGRect(15, AppDelegate.ScreenHeight - 200, AppDelegate.ScreenWidth - 30, 20));
			similarLabel.Text = Strings.Similar;
			similarLabel.Font = UIFont.FromName("Arial", 16);
			similarLabel.TextColor = UIColor.White;
			similarLabel.SizeToFit();

			var manager = SDWebImageManager.SharedManager.ImageDownloader;
			var imageNSUrl = new NSUrl("https://image.tmdb.org/t/p/w600_and_h900_bestv2/" + Movie.poster_path.Replace("\\", ""));

			manager.DownloadImage(imageNSUrl, SDWebImageDownloaderOptions.UseNSUrlCache, (s, e) =>
				{
				},
				((UIImage image, NSData data, NSError error, bool finished) =>
				{
					if (image != null && finished)
					{
						BeginInvokeOnMainThread(() =>
						{
							posterView.Image = image;
						});
					}
				})
			);

			var similarRow = new UIScrollView(new CGRect(15, AppDelegate.ScreenHeight - 170, AppDelegate.ScreenWidth, 200));
			similarRow.CanCancelContentTouches = true;
			similarRow.ContentSize = new CGSize(AppDelegate.ScreenWidth * SimilarMovies.results.Count, 200);
			similarRow.PagingEnabled = true;

			double xOffset = 0;
			double xPadding = 10;
			double posterWidth = AppDelegate.ScreenWidth / 3 - 2 * xPadding;
			double posterHeight = 160;

			foreach (Movie m in SimilarMovies.results)
			{
				if (!string.IsNullOrEmpty(m.poster_path))
				{
					imageNSUrl = new NSUrl("https://image.tmdb.org/t/p/w600_and_h900_bestv2/" + m.poster_path.Replace("\\", ""));

					var poster = new UIImageView(new CGRect(xOffset, 0, posterWidth, posterHeight));
					poster.UserInteractionEnabled = true;
					poster.AddGestureRecognizer(new UITapGestureRecognizer(() =>
					{
						PresentViewController(new MovieDetailController(m), true, null);
					}));

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

					similarRow.Add(poster);
					xOffset += posterWidth + 2 * xPadding;
				}
			}

			var blurView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark));
			blurView.Frame = View.Frame;

			View.AddSubviews(new UIView[] {
				blurView, 
				xView, 
				posterView, 
				title, 
				dateLabel,
				ratingLabel,
				playButton, 
				saveButton, 
				descriptionLabel,
				similarLabel,
				similarRow
			});
		}

		async Task GetSimilarMovies()
		{
			SimilarMovies = await new API().SendGetRequest<MovieListResponse>(APIEndpoints.SimilarURL.Replace("##ID##", Movie.id.ToString()));
		}
	}
}

