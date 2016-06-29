namespace MovieListPCL
{
	public static class APIEndpoints
	{
		public static string NowPlayingURL = "http://api.themoviedb.org/3/movie/now_playing?api_key=ab41356b33d100ec61e6c098ecc92140&sort_by=popularity.des";
		public static string TopRatedURL = "http://api.themoviedb.org/3/movie/top_rated?api_key=ab41356b33d100ec61e6c098ecc92140&sort_by=popularity.des";
		public static string PopularURL = "http://api.themoviedb.org/3/movie/popular?api_key=ab41356b33d100ec61e6c098ecc92140&sort_by=popularity.des";
		public static string SimilarURL = "http://api.themoviedb.org/3/movie/##ID##/similar?api_key=ab41356b33d100ec61e6c098ecc92140";
		public static string trailerURL = "http://api.themoviedb.org/3/movie/##ID##/videos?api_key=ab41356b33d100ec61e6c098ecc92140";
	}
}

