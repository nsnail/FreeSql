﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace FreeSql
{
    public interface IInsertOrUpdate<T1> where T1 : class
    {

        /// <summary>
        /// 指定事务对象
        /// </summary>
        /// <param name="transaction"></param>
        /// <returns></returns>
        IInsertOrUpdate<T1> WithTransaction(DbTransaction transaction);
        /// <summary>
        /// 指定事务对象
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        IInsertOrUpdate<T1> WithConnection(DbConnection connection);
        /// <summary>
        /// 命令超时设置(秒)
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        IInsertOrUpdate<T1> CommandTimeout(int timeout);

        /// <summary>
        /// 添加或更新，设置实体
        /// </summary>
        /// <param name="source">实体</param>
        /// <returns></returns>
        IInsertOrUpdate<T1> SetSource(T1 source);
        /// <summary>
        /// 添加或更新，设置实体
        /// </summary>
        /// <param name="source">实体</param>
        /// <param name="tempPrimarys">
        /// 根据临时主键插入或更新，a => a.Name | a => new{a.Name,a.Time} | a => new[]{"name","time"}<para></para>
        /// 注意：不处理自增，因某些数据库依赖主键或唯一键，所以指定临时主键仅对 SqlServer/PostgreSQL/Firebird/达梦/南大通用/金仓/神通 有效
        /// </param>
        /// <returns></returns>
        IInsertOrUpdate<T1> SetSource(T1 source, Expression<Func<T1, object>> tempPrimarys);
        /// <summary>
        /// 添加或更新，设置实体集合
        /// </summary>
        /// <param name="source">实体集合</param>
        /// <param name="tempPrimarys">
        /// 根据临时主键插入或更新，a => a.Name | a => new{a.Name,a.Time} | a => new[]{"name","time"}<para></para>
        /// 注意：不处理自增，因某些数据库依赖主键或唯一键，所以指定临时主键仅对 SqlServer/PostgreSQL/Firebird/达梦/南大通用/金仓/神通 有效
        /// </param>
        /// <returns></returns>
        IInsertOrUpdate<T1> SetSource(IEnumerable<T1> source, Expression<Func<T1, object>> tempPrimarys = null);
        /// <summary>
        /// 添加或更新，设置SQL
        /// </summary>
        /// <param name="sql">查询SQL</param>
        /// <param name="tempPrimarys">
        /// 根据临时主键插入或更新，a => a.Name | a => new{a.Name,a.Time} | a => new[]{"name","time"}<para></para>
        /// 注意：不处理自增，因某些数据库依赖主键或唯一键，所以指定临时主键仅对 SqlServer/PostgreSQL/Firebird/达梦/南大通用/金仓/神通 有效
        /// </param>
        /// <returns></returns>
        IInsertOrUpdate<T1> SetSource(string sql, Expression<Func<T1, object>> tempPrimarys = null);

        /// <summary>
        /// 当记录存在时，什么都不做<para></para>
        /// 换句话：只有记录不存在时才插入
        /// </summary>
        /// <returns></returns>
        IInsertOrUpdate<T1> IfExistsDoNothing();

        /// <summary>
        /// 当记录存在时，指定只更新的字段，UpdateColumns(a => a.Name) | UpdateColumns(a => new{a.Name,a.Time}) | UpdateColumns(a => new[]{"name","time"})
        /// </summary>
        /// <param name="columns">lambda选择列</param>
        /// <returns></returns>
        IInsertOrUpdate<T1> UpdateColumns(Expression<Func<T1, object>> columns);
        /// <summary>
        /// 当记录存在时，指定只更新的字段
        /// </summary>
        /// <param name="columns">属性名，或者字段名</param>
        /// <returns></returns>
        IInsertOrUpdate<T1> UpdateColumns(string[] columns);

        /// <summary>
        /// 设置列的联表值，格式：<para></para>
        /// UpdateSet((a, b) => a.Clicks == b.xxx)<para></para>
        /// UpdateSet((a, b) => a.Clicks == a.Clicks + 1)
        /// </summary>
        /// <typeparam name="TMember"></typeparam>
        /// <param name="exp"></param>
        /// <returns></returns>
        IInsertOrUpdate<T1> UpdateSet<TMember>(Expression<Func<T1, T1, TMember>> exp);

        /// <summary>
        /// 批量执行选项设置，一般不需要使用该方法<para></para>
        /// 各数据库 rows 限制不一样，默认设置：200<para></para>
        /// 若没有事务传入，内部(默认)会自动开启新事务，保证拆包执行的完整性。
        /// </summary>
        /// <param name="rowsLimit">指定根据 rows 上限数量拆分执行</param>
        /// <param name="autoTransaction">是否自动开启事务</param>
        /// <returns></returns>
        IInsertOrUpdate<T1> BatchOptions(int rowsLimit, bool autoTransaction = true);

        /// <summary>
        /// 设置表名规则，可用于分库/分表，参数1：默认表名；返回值：新表名；
        /// </summary>
        /// <param name="tableRule"></param>
        /// <returns></returns>
        IInsertOrUpdate<T1> AsTable(Func<string, string> tableRule);
        /// <summary>
        /// 设置表名
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        IInsertOrUpdate<T1> AsTable(string tableName);
        /// <summary>
        /// 动态Type，在使用 Update&lt;object&gt; 后使用本方法，指定实体类型
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        IInsertOrUpdate<T1> AsType(Type entityType);
        /// <summary>
        /// 返回即将执行的SQL语句
        /// </summary>
        /// <returns></returns>
        string ToSql();
        /// <summary>
        /// 执行SQL语句，返回影响的行数
        /// </summary>
        /// <returns></returns>
        int ExecuteAffrows();

#if net40
#else
        Task<int> ExecuteAffrowsAsync(CancellationToken cancellationToken = default);
#endif
    }
}