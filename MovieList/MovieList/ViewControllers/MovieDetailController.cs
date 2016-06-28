using System;
using UIKit;
using Foundation;
using CoreGraphics;
using MovieListPCL;
using SDWebImage;

namespace MovieList
{
	public class MovieDetailController : UIViewController
	{
		Movie Movie { get; set;}

		public MovieDetailController(Movie m)
		{
			Movie = m;
			View.BackgroundColor = UIColor.Clear;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			var posterView = new UIImageView(new CGRect(10, 50, 150, 250));
			var xView = new UIImageView(new CGRect(AppDelegate.ScreenWidth - 50, 20, 50, 30));
			xView.Image = new UIImage("PS_X");
			xView.ContentMode = UIViewContentMode.ScaleAspectFit;
			xView.UserInteractionEnabled = true;
			xView.AddGestureRecognizer(new UITapGestureRecognizer(() => {
				DismissViewController(true, null);
			}));

			var title = new UILabel(new CGRect(170, 50, AppDelegate.ScreenWidth - 180, 60));
			title.Text = Movie.title;
			title.TextColor = UIColor.White;
			title.Lines = 3;

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

			var blurView = new UIVisualEffectView(UIBlurEffect.FromStyle(UIBlurEffectStyle.Dark));
			blurView.Frame = View.Frame;

			View.AddSubviews(new UIView[] {blurView, xView, posterView, title });
		}
	}
}

