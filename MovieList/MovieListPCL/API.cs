using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Collections.Generic;

namespace MovieListPCL
{
	public class API
	{
		public API()
		{
		}

		public async Task<TReturn> SendPostRequest<T, TReturn>(T messageObject, string route)
		{
			string json = JsonConvert.SerializeObject(messageObject);
			if (json == null)
				json = "[{}]";

			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(route);

			request.ContentType = "application/json";
			request.Method = "POST";
			string received = "";

			using (var stream = await Task.Factory.FromAsync(request.BeginGetRequestStream, request.EndGetRequestStream, null))
			{
				using (var writer = new StreamWriter(stream))
				{
					writer.Write(json);
					writer.Flush();
					writer.Dispose();
				}
			}

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

		public TReturn SendGetRequest<TReturn>(string route)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(route);
			request.ContentType = "application/json";
			request.Method = "GET";
			string received = "";

			using (var response = (HttpWebResponse)(Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse, request.EndGetResponse, null)).Result)
			{
				using (var responseStream = response.GetResponseStream())
				{
					using (var sr = new StreamReader(responseStream))
					{
						received = sr.ReadToEndAsync().Result;
					}
				}
			}

			TReturn responseObject = JsonConvert.DeserializeObject<TReturn>(received);
			return responseObject;
		}
	}
}

