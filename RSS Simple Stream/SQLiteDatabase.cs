﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Windows;

namespace RSS_Simple_Stream
{
    /// <summary>
    /// SQLite database management 
    /// Modified from brennydoogles tutorial
    /// http://www.dreamincode.net/forums/topic/157830-using-sqlite-with-c%23/
    /// </summary>
    public class SQLiteDatabase
    {
        private String file;
        private String dbConnection;
        private SQLiteConnection connection;

        private static Dictionary<String, SQLiteDatabase> instances = new Dictionary<string,SQLiteDatabase>();

        /// <summary>
        ///     Single Param Constructor for specifying the DB file.
        /// </summary>
        /// <param name="inputFile">The File containing the DB</param>
        private SQLiteDatabase(String inputFile)
        {
            this.file = inputFile;
            this.dbConnection = String.Format("Data Source={0}", inputFile);

            connection = new SQLiteConnection(dbConnection);
            connection.Open();
        }

        /// <summary>
        /// Get instance of SQLite Database manager for file given
        /// </summary>
        /// <param name="file">The File containing the DB</param>
        /// <returns>Corresponding instance of SQL Database management</returns>
        public static SQLiteDatabase getInstance(string file)
        {
            if (!instances.ContainsKey(file))
            {
                instances.Add(file, new SQLiteDatabase(file));
            }

            return instances[file];
        }

        /// <summary>
        /// Close current SQLite Connection
        /// </summary>
        public void closeConnection()
        {
            if (instances.ContainsKey(this.file))
            {
                instances.Remove(this.file);
            }

            connection.Close();
        }

        /// <summary>
        ///     Allows the programmer to run a query against the Database.
        /// </summary>
        /// <param name="sql">The SQL to run</param>
        /// <returns>A DataTable containing the result set.</returns>
        public DataTable GetDataTable(string sql)
        {
            DataTable dt = new DataTable();

            try
            {
                SQLiteCommand mycommand = new SQLiteCommand(connection);
                mycommand.CommandText = sql;

                SQLiteDataReader reader = mycommand.ExecuteReader();
                dt.Load(reader);

                reader.Close();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }

            return dt;
        }

        /// <summary>
        ///     Allows the programmer to interact with the database for purposes other than a query.
        /// </summary>
        /// <param name="sql">The SQL to be run.</param>
        /// <returns>An Integer containing the number of rows updated.</returns>
        public int ExecuteNonQuery(string sql)
        {
            SQLiteCommand mycommand = new SQLiteCommand(connection);
            mycommand.CommandText = sql;
            int rowsUpdated = mycommand.ExecuteNonQuery();

            return rowsUpdated;
        }

        /// <summary>
        ///     Allows the programmer to retrieve single items from the DB.
        /// </summary>
        /// <param name="sql">The query to run.</param>
        /// <returns>A string.</returns>
        public string ExecuteScalar(string sql)
        {
            SQLiteCommand mycommand = new SQLiteCommand(connection);
            mycommand.CommandText = sql;
            object value = mycommand.ExecuteScalar();

            if (value != null)
            {
                return value.ToString();
            }

            return "";
        }

        /// <summary>
        ///     Allows the programmer to easily update rows in the DB.
        /// </summary>
        /// <param name="tableName">The table to update.</param>
        /// <param name="data">A dictionary containing Column names and their new values.</param>
        /// <param name="where">The where clause for the update statement.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Update(String tableName, Dictionary<String, String> data, String where)
        {
            String vals = "";
            Boolean returnCode = true;

            if (data.Count >= 1)
            {
                foreach (KeyValuePair<String, String> val in data)
                {
                    vals += String.Format(" {0} = '{1}',", val.Key.ToString(), val.Value.ToString());
                }
                vals = vals.Substring(0, vals.Length - 1);
            }

            try
            {
                this.ExecuteNonQuery(String.Format("UPDATE {0} SET {1} WHERE {2};", tableName, vals, where));
            }
            catch
            {
                returnCode = false;
            }

            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily delete rows from the DB.
        /// </summary>
        /// <param name="tableName">The table from which to delete.</param>
        /// <param name="where">The where clause for the delete.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool Delete(String tableName, String where)
        {
            Boolean returnCode = true;
            try
            {
                this.ExecuteNonQuery(String.Format("DELETE FROM {0} WHERE {1};", tableName, where));
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message);
                returnCode = false;
            }
            
            return returnCode;
        }

        /// <summary>
        ///     Allows the programmer to easily insert into the DB
        /// </summary>
        /// <param name="tableName">The table into which we insert the data.</param>
        /// <param name="data">A dictionary containing the column names and data for the insert.</param>
        /// <returns>The last inserted ID.</returns>
        public int Insert(String tableName, Dictionary<String, String> data)
        {
            String columns = "";
            String values = "";

            foreach (KeyValuePair<String, String> val in data)
            {
                columns += String.Format(" {0},", val.Key.ToString());
                values += String.Format(" '{0}',", val.Value);
            }

            columns = columns.Substring(0, columns.Length - 1);
            values = values.Substring(0, values.Length - 1);
            
            try
            {
                this.ExecuteNonQuery(String.Format("INSERT INTO {0}({1}) VALUES({2});", tableName, columns, values));
                string id = this.ExecuteScalar("SELECT last_insert_rowid()");
                return int.Parse(id);
            }
            catch (Exception fail)
            {
                MessageBox.Show(fail.Message);
                return 0;
            }
        }

        /// <summary>
        ///     Allows the programmer to easily delete all data from the DB.
        /// </summary>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearDB()
        {
            DataTable tables;
            try
            {
                tables = this.GetDataTable("select NAME from SQLITE_MASTER where type='table' order by NAME;");
                
                foreach (DataRow table in tables.Rows)
                {
                    this.ClearTable(table["NAME"].ToString());
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///     Allows the user to easily clear all data from a specific table.
        /// </summary>
        /// <param name="table">The name of the table to clear.</param>
        /// <returns>A boolean true or false to signify success or failure.</returns>
        public bool ClearTable(String table)
        {
            try
            {
                this.ExecuteNonQuery(String.Format("DELETE FROM {0};", table));
                return true;
            }
            catch
            {
                return false;
            }
        }
    }

}
