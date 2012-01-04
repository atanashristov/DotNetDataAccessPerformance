using System.Collections.Generic;
using System.Linq;
using DotNetDataAccessPerformance.Domain;
using DotNetDataAccessPerformance.Helpers;

namespace DotNetDataAccessPerformance.Repositories
{
	public class NHibernateHqlQueryStrongTypeRepository2 : IRepository
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
                qry.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<Song>());
                return qry.List<Song>();
            }
        }



        public IEnumerable<Song> GetSongsByArtist2(string name)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                NHibernate.ICriteria qry = session.CreateCriteria<Track>("track")
                    .CreateAlias("Album", "album")
                    .CreateAlias("album.Artist", "artist")
                    .Add(NHibernate.Criterion.Expression.Eq("artist.Name", name))
                    .SetProjection(NHibernate.Criterion.Projections.ProjectionList()
                        .Add(NHibernate.Criterion.Projections.Property("album.Title"), "AlbumName")
                        .Add(NHibernate.Criterion.Projections.Property("track.Name"), "SongName")
                        .Add(NHibernate.Criterion.Projections.Property("artist.Name"), "ArtistName")
                    );

                qry.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<Song>());
                return qry.List<Song>();
            }
        }


        public IEnumerable<Song> GetSongsByArtist3(string name)
        {
            using (var session = NHibernateHelper.OpenSession())
            {
                var qry = session.CreateQuery(@"select album.Title as AlbumName, track.Name as SongName, artist.Name as ArtistName
												from Track track 
												join track.Album as album
												join album.Artist as artist
												where artist.Name='" + name + "'");
                qry.SetResultTransformer(NHibernate.Transform.Transformers.AliasToBean<Song>());
                return qry.List<Song>();
            }
        }

	}
}