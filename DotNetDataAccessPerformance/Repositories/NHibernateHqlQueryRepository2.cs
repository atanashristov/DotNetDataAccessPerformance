using System.Collections.Generic;
using System.Linq;
using DotNetDataAccessPerformance.Domain;
using DotNetDataAccessPerformance.Helpers;

namespace DotNetDataAccessPerformance.Repositories
{
	public class NHibernateHqlQueryRepository2 : IRepository
	{
		public void AddArtist(Artist artist)
		{
			throw new System.NotImplementedException();
		}

		public void UpdateArtist(Artist artist)
		{
			throw new System.NotImplementedException();
		}

		public void DeleteArtist(Artist artist)
		{
			throw new System.NotImplementedException();
		}

		public Artist GetArtistById(int id)
		{
			using (var session = NHibernateHelper.OpenSession())
			{
				var query = session.CreateQuery("from Artist artist where artist.ArtistId=" + id);
				return query.List<Artist>().FirstOrDefault();
			}
		}

		public IEnumerable<Song> GetSongsByArtist(string name)
		{
			using (var session = NHibernateHelper.OpenSession())
			{
				var qry = session.CreateQuery(@"select album.Title as AlbumName, track.Name as SongName, artist.Name as ArtistName
												from Track track 
												join track.Album as album
												join album.Artist as artist
												where artist.Name='" + name + "'");
				var songs = (from object[] item in qry.List()
				             select new Song
				                    	{
				                    		AlbumName = (string)item[0],
				                    		SongName = (string)item[1],
				                    		ArtistName = (string)item[2]
				                    	}).ToList();

				return songs;
			}
		}
	}
}