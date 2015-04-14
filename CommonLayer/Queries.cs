using SQLite.Net.Async;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLayer
{
    public class Queries
    {
        public static Task<int> InsertAsync(SQLiteAsyncConnection connection, object item)
        {
            return connection.InsertAsync(item);
        }

        public static Task<int> InsertAllAsync(SQLiteAsyncConnection connection, IEnumerable items)
        {
            return connection.InsertAllAsync(items);
        }

        public static Task<int> UpdateAsync(SQLiteAsyncConnection connection, object item)
        {
            return connection.UpdateAsync(item);
        }

        public static Task<int> UpdateAllAsync(SQLiteAsyncConnection connection, IEnumerable items)
        {
            return connection.UpdateAllAsync(items);
        }

        public static Task<int> DeleteAsync<T>(SQLiteAsyncConnection connection, object item)
        {
            return connection.DeleteAsync<T>(item);
        }

        public static Task<int> DeleteAllAsync<T>(SQLiteAsyncConnection connection, IEnumerable items)
        {
            return connection.DeleteAllAsync<T>();
        }

        public static Task<List<T>> QueryAsync<T>(SQLiteAsyncConnection connection, string sql, params object[] args) where T : new()
        {
            return connection.QueryAsync<T>(sql, args);
        } 
    }
}
