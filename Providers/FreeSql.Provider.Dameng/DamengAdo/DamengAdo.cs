﻿using Dm;
using FreeSql.Internal;
using FreeSql.Internal.CommonProvider;
using FreeSql.Internal.Model;
using FreeSql.Internal.ObjectPool;
using System;
using System.Collections;
using System.Data.Common;
using System.Linq;
using System.Threading;

namespace FreeSql.Dameng
{
    class DamengAdo : FreeSql.Internal.CommonProvider.AdoProvider
    {
        public DamengAdo() : base(DataType.Dameng, null, null) { }
        public DamengAdo(CommonUtils util, string masterConnectionString, string[] slaveConnectionStrings, Func<DbConnection> connectionFactory) : base(DataType.Dameng, masterConnectionString, slaveConnectionStrings)
        {
            base._util = util;
            if (connectionFactory != null)
            {
                var pool = new FreeSql.Internal.CommonProvider.DbConnectionPool(DataType.Dameng, connectionFactory);
                ConnectionString = pool.TestConnection?.ConnectionString;
                MasterPool = pool;
                return;
            }

            var isAdoPool = masterConnectionString?.StartsWith("AdoConnectionPool,") ?? false;
            if (isAdoPool) masterConnectionString = masterConnectionString.Substring("AdoConnectionPool,".Length);
            if (!string.IsNullOrEmpty(masterConnectionString))
                MasterPool = isAdoPool ?
                    new DbConnectionStringPool(base.DataType, CoreStrings.S_MasterDatabase, () => new DmConnection(masterConnectionString)) as IObjectPool<DbConnection> :
                    new DamengConnectionPool(CoreStrings.S_MasterDatabase, masterConnectionString, null, null);

            slaveConnectionStrings?.ToList().ForEach(slaveConnectionString =>
            {
                var slavePool = isAdoPool ?
                        new DbConnectionStringPool(base.DataType, $"{CoreStrings.S_SlaveDatabase}{SlavePools.Count + 1}", () => new DmConnection(slaveConnectionString)) as IObjectPool<DbConnection> :
                        new DamengConnectionPool($"{CoreStrings.S_SlaveDatabase}{SlavePools.Count + 1}", slaveConnectionString, () => Interlocked.Decrement(ref slaveUnavailables), () => Interlocked.Increment(ref slaveUnavailables));
                SlavePools.Add(slavePool);
            });
        }
        public override object AddslashesProcessParam(object param, Type mapType, ColumnInfo mapColumn)
        {
            if (param == null) return "NULL";
            if (mapType != null && mapType != param.GetType() && (param is IEnumerable == false))
                param = Utils.GetDataReaderValue(mapType, param);

            if (param is byte[])
                return $"hextoraw('{CommonUtils.BytesSqlRaw(param as byte[])}')";
            else if (param is bool || param is bool?)
                return (bool)param ? 1 : 0;
            else if (param is string)
                return string.Concat("'", param.ToString().Replace("'", "''"), "'");
            else if (param is char)
                return string.Concat("'", param.ToString().Replace("'", "''").Replace('\0', ' '), "'");
            else if (param is Enum)
                return AddslashesTypeHandler(param.GetType(), param) ?? ((Enum)param).ToInt64();
            else if (decimal.TryParse(string.Concat(param), out var trydec))
                return param;

            else if (param is DateTime)
                return AddslashesTypeHandler(typeof(DateTime), param) ?? string.Concat("to_timestamp('", ((DateTime)param).ToString("yyyy-MM-dd HH:mm:ss.ffffff"), "','YYYY-MM-DD HH24:MI:SS.FF6')");
            else if (param is DateTime?)
                return AddslashesTypeHandler(typeof(DateTime?), param) ?? string.Concat("to_timestamp('", ((DateTime)param).ToString("yyyy-MM-dd HH:mm:ss.ffffff"), "','YYYY-MM-DD HH24:MI:SS.FF6')");

            else if (param is TimeSpan || param is TimeSpan?)
                return $"numtodsinterval({((TimeSpan)param).Ticks * 1.0 / 10000000},'second')";
            else if (param is IEnumerable)
                return AddslashesIEnumerable(param, mapType, mapColumn);

            return string.Concat("'", param.ToString().Replace("'", "''"), "'");
            //if (param is string) return string.Concat('N', nparms[a]);
        }

        public override DbCommand CreateCommand()
        {
            return new DmCommand();
        }

        public override void ReturnConnection(IObjectPool<DbConnection> pool, Object<DbConnection> conn, Exception ex)
        {
            var rawPool = pool as DamengConnectionPool;
            if (rawPool != null) rawPool.Return(conn, ex);
            else pool.Return(conn);
        }

        public override DbParameter[] GetDbParamtersByObject(string sql, object obj) => _util.GetDbParamtersByObject(sql, obj);
    }
}
