using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using System.Diagnostics;
using System.Threading;
using DotNetDataAccessPerformance.Helpers;
using DotNetDataAccessPerformance.Repositories;
using DotNetDataAccessPerformance.Domain;

namespace DotNetDataAccessPerformanceTests
{
    class Program
    {
        static void Main(string[] args)
        {
			var repository = RepositoryFactory.Create(typeof(NHibernateHqlQueryStrongTypeRepository2));
			List<Song> songs = repository.GetSongsByArtist("Pearl Jam").ToList();


            Tester tester;

            Console.WriteLine("Start: DataReaderNativeQueryTester");
            tester = new DataReaderNativeQueryTester();
            tester.Test();
            Console.WriteLine("ElapsedMilliseconds: " + tester.ElapsedMilliseconds);


            Console.WriteLine("Start: NHibernateHqlQueryTester1");
            tester = new NHibernateHqlQueryTester1();
            tester.Test();
            Console.WriteLine("ElapsedMilliseconds: " + tester.ElapsedMilliseconds);


            Console.WriteLine("Start: NHibernateHqlQueryTester2");
            tester = new NHibernateHqlQueryTester2();
            tester.Test();
            Console.WriteLine("ElapsedMilliseconds: " + tester.ElapsedMilliseconds);


            Console.WriteLine("Press <Enter> to finish.");
            Console.ReadLine();
        }
    }



    public abstract class Tester
    {
        protected Stopwatch stopwatch;
        protected IRepository repo;

        public Tester()
        {
            stopwatch = new Stopwatch();
        }

        public Int64 ElapsedMilliseconds { get { return stopwatch.ElapsedMilliseconds; } }

        public void Test()
        {
            stopwatch.Start();

            for (int i = 0; i < 1000; i++)
            {
                IEnumerable<Song> songs = repo.GetSongsByArtist("Aerosmith");
                songs.ToList<Song>();
            }

            stopwatch.Stop();
        }
    }




    public class DataReaderNativeQueryTester : Tester
    {
        public DataReaderNativeQueryTester() 
            : base() 
        {
            repo = new DataReaderNativeQueryRepository();
        }
    }




    public abstract class NHibernateHqlQueryTester : Tester
    {
        public NHibernateHqlQueryTester()
            : base()
        {
            NHibernateHelper.OpenSession();
        }
    }

    public class NHibernateHqlQueryTester1 : NHibernateHqlQueryTester
    {
        public NHibernateHqlQueryTester1() 
            : base() 
        {
            repo = new NHibernateHqlQueryStrongTypeRepository();
        }


    }

    public class NHibernateHqlQueryTester2 : NHibernateHqlQueryTester
    {
        public NHibernateHqlQueryTester2() 
            : base() 
        {
            repo = new NHibernateHqlQueryStrongTypeRepository2();
        }

    }


}
