using FreeSql.DataAnnotations;
using System;
using System.Data.Odbc;
using Xunit;

namespace FreeSql.Tests.Odbc.Default
{
    public class OdbcAdoTest
    {
        [Fact]
        public void Pool()
        {
            var t1 = g.odbc.Ado.MasterPool.StatisticsFullily;
        }

        [Fact]
        public void SlavePools()
        {
            var t2 = g.odbc.Ado.SlavePools.Count;
        }

        [Fact]
        public void ExecuteTest()
        {
            Assert.True(g.odbc.Ado.ExecuteConnectTest());
        }
        [Fact]
        public void ExecuteReader()
        {

        }
        [Fact]
        public void ExecuteArray()
        {

        }
        [Fact]
        public void ExecuteNonQuery()
        {

        }
        [Fact]
        public void ExecuteScalar()
        {

        }

        [Fact]
        public void Query()
        {

            //var tt1 = g.odbc.Select<xxx>()
            //    .LeftJoin(a => a.ParentId == a.Parent.Id)
            //    .ToSql(a => new { a.Id, a.Title });

            //var tt2result = g.odbc.Select<xxx>()
            //    .LeftJoin(a => a.ParentId == a.Parent.Id)
            //    .ToList(a => new { a.Id, a.Title });

            //var tt = g.odbc.Select<xxx>()
            //    .LeftJoin<xxx>((a, b) => b.Id == a.Id)
            //    .ToSql(a => new { a.Id, a.Title });

            //var ttresult = g.odbc.Select<xxx>()
            //    .LeftJoin<xxx>((a, b) => b.Id == a.Id)
            //    .ToList(a => new { a.Id, a.Title });

            var tnsql1 = g.odbc.Select<xxx>().Where(a => a.Id > 0).Where(b => b.Title != null).Page(1, 3).ToSql(a => a.Id);

            var tn1 = g.odbc.Select<xxx>().Where(a => a.Id > 0).Where(b => b.Title != null).Page(1, 3).ToList(a => a.Id);

            var t3 = g.odbc.Ado.Query<xxx>("select * from xxx");

            var t4 = g.odbc.Ado.Query<(int, int, string, string DateTime)>("select * from xxx");

            var t5 = g.odbc.Ado.Query<dynamic>(System.Data.CommandType.Text, "select * from xxx where Id = ?",
                new OdbcParameter("@Id", 1));
        }

        [Fact]
        public void QueryMultipline()
        {
            var tnsql1 = g.odbc.Select<xxx>().Where(a => a.Id > 0).Where(b => b.Title != null).Page(1, 3).ToSql(a => a.Id);

            var t3 = g.odbc.Ado.Query<xxx, (int, string, string), dynamic>("select * from xxx; select * from xxx; select * from xxx");
        }

        class xxx
        {
            public int Id { get; set; }
            public int ParentId { get; set; }
            public xxx Parent { get; set; }
            public string Title { get; set; }
            public string Url { get; set; }
            public DateTime Create_time { get; set; }
        }
    }
}
