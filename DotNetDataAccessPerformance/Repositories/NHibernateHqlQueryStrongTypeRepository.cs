using System.Collections.Generic;
using System.Linq;
using DotNetDataAccessPerformance.Domain;
using DotNetDataAccessPerformance.Helpers;

namespace DotNetDataAccessPerformance.Repositories
{
	public class NHibernateHqlQueryStrongTypeRepository : IRepository
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
				var query = session.CreateQuery(@"select track
												from Track track 
												join track.Album as album
												join album.Artist as artist
												where artist.Name='" + name + "'");

				var songs = (from track in query.List<Track>()
				             select new Song
				                    	{
				                    		AlbumName = track.Album.Title,
				                    		SongName = track.Name,
				                    		ArtistName = track.Album.Artist.Name
				                    	}).ToList();

				return songs;
			}
		}


        /*

        Added two versions of NHibernateHqlQueryStrongTypeRepository.GetSongsByArtist() - GetSongsByArtist2() and GetSongsByArtist3()

        the idea is:
        1. the original version does 3 trips to DB running 3 queries (at very minimum) due to the dot (.) notation / lazy loading of the graphs
        2. we can select all the  Song data in one trip to DB running one query.

        Simple output:

        Loading from the original GetSongsByArtist.
        NHibernate: select track0_.TrackId as TrackId2_, track0_.AlbumId as AlbumId2_, track0_.Bytes as Bytes2_, track0_.Composer as Composer2_, track0_.GenreId as GenreId2_, track0_.MediaTypeId as MediaTyp6_2_, track0_.Milliseconds as Millisec7_2_, track0_.Name as Name2_, track0_.UnitPrice as UnitPrice2_ from Track track0_ inner join Album album1_ on track0_.AlbumId=album1_.AlbumId inner join Artist artist2_ on album1_.ArtistId=artist2_.ArtistId where artist2_.Name='Aerosmith'
        NHibernate: SELECT album0_.AlbumId as AlbumId1_0_, album0_.Title as Title1_0_, album0_.ArtistId as ArtistId1_0_ FROM Album album0_ WHERE album0_.AlbumId=@p0;@p0 = 5 [Type: Int32 (0)]
        NHibernate: SELECT artist0_.ArtistId as ArtistId0_0_, artist0_.Name as Name0_0_ FROM Artist artist0_ WHERE artist0_.ArtistId=@p0;@p0 = 3 [Type: Int32 (0)]
        Aerosmith - Big Ones - Walk On Water
        Aerosmith - Big Ones - Love In An Elevator
        Aerosmith - Big Ones - Rag Doll
        Aerosmith - Big Ones - What It Takes
        Aerosmith - Big Ones - Dude (Looks Like A Lady)
        Aerosmith - Big Ones - Janie's Got A Gun
        Aerosmith - Big Ones - Cryin'
        Aerosmith - Big Ones - Amazing
        Aerosmith - Big Ones - Blind Man
        Aerosmith - Big Ones - Deuces Are Wild
        Aerosmith - Big Ones - The Other Side
        Aerosmith - Big Ones - Crazy
        Aerosmith - Big Ones - Eat The Rich
        Aerosmith - Big Ones - Angel
        Aerosmith - Big Ones - Livin' On The Edge
        Loading from the new GetSongsByArtist2.
        NHibernate: SELECT album1_.Title as y0_, this_.Name as y1_, artist2_.Name as y2_ FROM Track this_ inner join Album album1_ on this_.AlbumId=album1_.AlbumId inner join Artist artist2_ on album1_.ArtistId=artist2_.ArtistId WHERE artist2_.Name = @p0;@p0 = 'Aerosmith' [Type: String (4000)]
        Aerosmith - Big Ones - Walk On Water
        Aerosmith - Big Ones - Love In An Elevator
        Aerosmith - Big Ones - Rag Doll
        Aerosmith - Big Ones - What It Takes
        Aerosmith - Big Ones - Dude (Looks Like A Lady)
        Aerosmith - Big Ones - Janie's Got A Gun
        Aerosmith - Big Ones - Cryin'
        Aerosmith - Big Ones - Amazing
        Aerosmith - Big Ones - Blind Man
        Aerosmith - Big Ones - Deuces Are Wild
        Aerosmith - Big Ones - The Other Side
        Aerosmith - Big Ones - Crazy
        Aerosmith - Big Ones - Eat The Rich
        Aerosmith - Big Ones - Angel
        Aerosmith - Big Ones - Livin' On The Edge
        Loading from the new GetSongsByArtist3.
        NHibernate: select album1_.Title as col_0_0_, track0_.Name as col_1_0_, artist2_.Name as col_2_0_ from Track track0_ inner join Album album1_ on track0_.AlbumId=album1_.AlbumId inner join Artist artist2_ on album1_.ArtistId=artist2_.ArtistId where artist2_.Name='Aerosmith'
        Aerosmith - Big Ones - Walk On Water
        Aerosmith - Big Ones - Love In An Elevator
        Aerosmith - Big Ones - Rag Doll
        Aerosmith - Big Ones - What It Takes
        Aerosmith - Big Ones - Dude (Looks Like A Lady)
        Aerosmith - Big Ones - Janie's Got A Gun
        Aerosmith - Big Ones - Cryin'
        Aerosmith - Big Ones - Amazing
        Aerosmith - Big Ones - Blind Man
        Aerosmith - Big Ones - Deuces Are Wild
        Aerosmith - Big Ones - The Other Side
        Aerosmith - Big Ones - Crazy
        Aerosmith - Big Ones - Eat The Rich
        Aerosmith - Big Ones - Angel
        Aerosmith - Big Ones - Livin' On The Edge
        Press <Enter> to finish.
 
        */


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