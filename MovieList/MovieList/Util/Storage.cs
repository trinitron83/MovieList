using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using SQLite.Net;
using SQLiteNetExtensions.Extensions;
using SQLite.Net.Platform.XamarinIOS;

namespace MovieListPCL
{
	public class Storage
	{
		public readonly string dbPath;
		public readonly SQLiteConnection conn;

		public Storage()
		{
			dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "database.db3");
			conn = new SQLiteConnection(new SQLitePlatformIOS(), dbPath);
			if (!conn.GetTableInfo("Movie").Any())
				conn.CreateTable<Movie>();
		}

		public List<Movie> GetFavorites()
		{
			if (conn.GetTableInfo("Movie").Any())
			{
				return conn.GetAllWithChildren<Movie>();
			}
			else
				return new List<Movie>();
		}

		public bool AddToFavorites(Movie m)
		{
			try
			{
				conn.Insert(m);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public bool RemoveFromFavorites(Movie m)
		{
			try
			{
				conn.Delete(m);
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}
	}
}

