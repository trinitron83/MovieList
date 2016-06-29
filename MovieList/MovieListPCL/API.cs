using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace MovieListPCL
{
	public class API
	{
		public async Task<TReturn> SendGetRequest<TReturn>(string route)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(route);
			request.ContentType = "application/json";
			request.Method = "GET";
			string received = "";

			using (var response = (HttpWebResponse)(await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null)))
			{
				using (var responseStream = response.GetResponseStream())
				{
					using (var sr = new StreamReader(responseStream))
					{
						received = await sr.ReadToEndAsync();
					}
				}
			}

			TReturn responseObject = JsonConvert.DeserializeObject<TReturn>(received);
			return responseObject;
		}
	}
}

