using System.Collections.Generic;

namespace MovieListPCL
{
	public class TrailerResponse
	{
		public int id { get; set; }
		public List<Trailer> results { get; set; }
	}
}

