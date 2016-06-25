using System.Collections.Generic;

namespace MovieListPCL
{
	public class MovieListResponse
	{
		public int page { get; set; }

		public List<Movie> results { get; set; }

		public MovieDate dates { get; set; }

		public int total_pages { get; set; }

		public int total_results { get; set; }
	}
}

