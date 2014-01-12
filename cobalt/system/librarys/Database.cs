using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Cobalt.system.librarys
{
    public class Database
    {
        private static string _conntectionString = ConfigurationManager.ConnectionStrings["Cobalt"].ToString();

        ///<summary>
        ///Close current sql connection
        ///<para>
        ///Check the condition of the connection and close the connection
        ///</para>
        ///</summary>
        public static void sqlClose(SqlConnection connection)
        {
            try
            {
                if (!(connection == null && connection.State == ConnectionState.Closed))
                {
                    connection.Close();
                    connection.Dispose();
                }
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
        }

        ///<summary>
        ///Connect to sql server
        ///<para>
        ///Create new sql connection and return connection, if connection failed it will throw an error.
        ///</para>
        ///</summary>
        public static SqlConnection sqlConnect()
        {
            SqlConnection connection = null;
            try
            {
                connection = new SqlConnection(_conntectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }

            return connection;
        }

        public static int sqlCreateTable(string tablename)
        {
            string query = "CREATE TABLE " + tablename + "";
            SqlCommand command = null;
            SqlConnection connection = sqlConnect();
            int rws = -1;

            try
            {
                command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                command.Dispose();
                sqlClose(connection);
            }

            return rws;
        }

        public static int sqlDropTable(string tablename)
        {
            string query = "DROP TABLE " + tablename + "";
            SqlCommand command = null;
            SqlConnection connection = sqlConnect();
            int rws = -1;

            try
            {
                command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                command.Dispose();
                sqlClose(connection);
            }

            return rws;
        }

        public static int sqlCustomQuery(string query)
        {
            SqlCommand command = null;
            SqlConnection connection = sqlConnect();
            int rws = -1;

            try
            {
                command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                command.Dispose();
                sqlClose(connection);
            }

            return rws;
        }

        ///<summary>
        ///Perform sql update
        ///<para>
        ///This function return sql affected rows
        ///</para>
        ///Sample uses
        ///<para>
        /// SqlParameterCollection parameters = new SqlParameterCollection();
        /// parameters.Add(new SqlParameter("@name", SqlDbType.VarChar, 32).Value = "OFF sue");
        /// parameters.AddRange(..
        /// parameters.AddWidthValue(..
        /// ... sqlUpdate("UPDATE table SET name = "@name"  WHERE id = 2", parameters)
        /// ... sqlUpdate("UPDATE table SET name = "name"  WHERE id = 2")
        ///</para>
        ///</summary>
        public static int sqlUpdate(string query, List<SqlParameter> paras)
        {
            SqlCommand command = null;
            SqlConnection connection = sqlConnect();
            int rws = -1;

            try
            {
                command = new SqlCommand(query, connection);
                command.Parameters.AddRange(paras.ToArray());
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                command.Dispose();
                sqlClose(connection);
            }

            return rws;
        }

        public static int sqlUpdate(string query)
        {
            SqlCommand command = null;
            SqlConnection connection = sqlConnect();
            int rws = -1;

            try
            {
                command = new SqlCommand(query, connection);
                rws = (Int32)command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                command.Dispose();
                sqlClose(connection);
            }

            return rws;
        }

        ///<summary>
        ///Perform sql Insert
        ///<para>
        ///This function return sql affected rows
        ///</para>
        ///Sample uses
        ///<para>
        /// SqlParameterCollection parameters = new SqlParameterCollection();
        /// parameters.Add(new SqlParameter("@name", SqlDbType.VarChar, 32).Value = "OFF sue");
        /// parameters.AddRange(..
        /// parameters.AddWidthValue(..
        /// ... sqlInsert("INSERT INTO table VALUES("@name")", parameters)
        /// ... sqlInsert("INSERT INTO table VALUES("name")")
        ///</para>
        ///</summary>
        public static int sqlInsert(string query, List<SqlParameter> paras)
        {
            SqlCommand command = null;
            SqlConnection connection = sqlConnect();
            int rws = -1;

            try
            {
                command = new SqlCommand(query, connection);
                command.Parameters.AddRange(paras.ToArray());
                rws = (Int32)command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                command.Dispose();
                sqlClose(connection);
            }

            return rws;
        }

        public static int sqlInsert(string query)
        {
            SqlCommand command = null;
            SqlConnection connection = sqlConnect();
            int rws = -1;

            try
            {
                command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                command.Dispose();
                sqlClose(connection);
            }

            return rws;
        }

        ///<summary>
        ///Perform sql Delete
        ///<para>
        ///This function return sql affected rows
        ///</para>
        ///Sample uses
        ///<para>
        /// SqlParameterCollection parameters = new SqlParameterCollection();
        /// parameters.Add(new SqlParameter("@name", SqlDbType.VarChar, 32).Value = "OFF sue");
        /// parameters.AddRange(..
        /// parameters.AddWidthValue(..
        /// ... sqlDelete("DELETE FROM table WHERE name = "@name"", parameters)
        /// ... sqlDelete("DELETE FROM table WHERE name = "name"")
        ///</para>
        ///</summary>
        public static int sqlDelete(string query, List<SqlParameter> paras)
        {
            SqlCommand command = null;
            SqlConnection connection = sqlConnect();
            int rws = -1;

            try
            {
                command = new SqlCommand(query, connection);
                command.Parameters.AddRange(paras.ToArray());
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                command.Dispose();
                sqlClose(connection);
            }

            return rws;
        }

        public static int sqlDelete(string query)
        {
            SqlCommand command = null;
            SqlConnection connection = sqlConnect();
            int rws = -1;

            try
            {
                command = new SqlCommand(query, connection);
                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                command.Dispose();
                sqlClose(connection);
            }

            return rws;
        }

        ///<summary>
        ///Return SqlData data
        ///<para>
        ///This function return SqlData as DataRecord
        ///</para>
        ///Sample uses
        ///<para>
        /// SqlParameterCollection parameters = new SqlParameterCollection();
        /// parameters.Add(new SqlParameter("@name", SqlDbType.VarChar, 32).Value = "OFF sue");
        /// parameters.AddRange(..
        /// parameters.AddWidthValue(..
        /// ... sqlSelect("SELECT * FROM table WHERE name = "@name"", parameters)
        /// ... sqlSelect("SELECT * FROM table WHERE name = "name"")
        ///</para>
        ///</summary>
        public static IEnumerable<IDataReader> sqlSelect(string query, List<SqlParameter> paras)
        {
            SqlConnection connection = sqlConnect();
            SqlDataReader reader = null;
            SqlCommand command = null;

            using (command = new SqlCommand(query, connection))
            {
                command.Parameters.AddRange(paras.ToArray());
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //object[] values = new object[reader.FieldCount];
                    //reader.GetValues(values);
                    //data.Add(values);
                    yield return reader;
                }
            }

            // finally
            //  {
            // command.Dispose();
            // reader.Close();
            // reader.Dispose();
            // sqlClose(connection);
            //    }
        }

        public static IEnumerable<IDataReader> sqlSelect(string query)
        {
            SqlConnection connection = sqlConnect();
            SqlDataReader reader = null;
            SqlCommand command = null;

            using (command = new SqlCommand(query, connection))
            {
                reader = command.ExecuteReader();

                while (reader.Read())
                {
                    //object[] values = new object[reader.FieldCount];
                    //reader.GetValues(values);
                    //data.Add(values);
                    yield return reader;
                }
            }

            // finally
            //  {
            // command.Dispose();
            // reader.Close();
            // reader.Dispose();
            // sqlClose(connection);
            //    }
        }

        //Sql Data Adapter Functions

        public static int adapterInsert(string queryString, List<SqlParameter> paras)
        {
            SqlConnection connection = sqlConnect();
            SqlDataAdapter adapter = null;
            int rws = -1;
            try
            {
                adapter = new SqlDataAdapter();
                adapter.InsertCommand = new SqlCommand(queryString, connection);
                adapter.InsertCommand.Parameters.AddRange(paras.ToArray());
                adapter.InsertCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }

            finally
            {
                sqlClose(connection);
                adapter.Dispose();
            }
            return rws;
        }

        public static int adapterInsert(string queryString)
        {
            SqlConnection connection = sqlConnect();
            SqlDataAdapter adapter = null;
            int rws = -1;
            try
            {
                adapter = new SqlDataAdapter();
                adapter.InsertCommand = new SqlCommand(queryString, connection);
                adapter.InsertCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }

            finally
            {
                sqlClose(connection);
                adapter.Dispose();
            }
            return rws;
        }

        public static void adapterSelect(string queryString, ref DataSet dataset, List<SqlParameter> paras)
        {
            SqlConnection connection = sqlConnect();
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(queryString, connection);
                if (paras != null)
                {
                    adapter.SelectCommand.Parameters.AddRange(paras.ToArray());
                }
                adapter.Fill(dataset);
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                sqlClose(connection);
            }
        }

        public static void adapterSelect(string queryString, ref DataSet dataset)
        {
            SqlConnection connection = sqlConnect();
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(queryString, connection);
                adapter.Fill(dataset);
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                sqlClose(connection);
            }
        }

        public static void adapterSelect(string queryString, ref DataTable datatable, List<SqlParameter> paras)
        {
            SqlConnection connection = sqlConnect();
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(queryString, connection);
                adapter.SelectCommand.Parameters.AddRange(paras.ToArray());

                adapter.Fill(datatable);
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                sqlClose(connection);
            }
        }

        public static void adapterSelect(string queryString, ref DataTable datatable)
        {
            SqlConnection connection = sqlConnect();
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(queryString, connection);
                adapter.Fill(datatable);
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                sqlClose(connection);
            }
        }

        public static int adapterDelete(string queryString, List<SqlParameter> paras)
        {
            SqlConnection connection = sqlConnect();
            int rws = -1;
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.DeleteCommand = new SqlCommand(queryString, connection);
                adapter.DeleteCommand.Parameters.AddRange(paras.ToArray());
                adapter.DeleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                sqlClose(connection);
            }
            return rws;
        }

        public static int adapterDelete(string queryString)
        {
            SqlConnection connection = sqlConnect();
            int rws = -1;
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.DeleteCommand = new SqlCommand(queryString, connection);
                adapter.DeleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                sqlClose(connection);
            }
            return rws;
        }

        public static int adapterUpdate(string queryString, List<SqlParameter> paras)
        {
            SqlConnection connection = sqlConnect();
            int rws = -1;
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.UpdateCommand = new SqlCommand(queryString, connection);
                adapter.UpdateCommand.Parameters.AddRange(paras.ToArray());
                adapter.UpdateCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                sqlClose(connection);
            }
            return rws;
        }

        public static int adapterUpdate(string queryString)
        {
            SqlConnection connection = sqlConnect();
            int rws = -1;
            try
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.UpdateCommand = new SqlCommand(queryString, connection);
                adapter.UpdateCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CobaltException.getExeption(ex);
            }
            finally
            {
                sqlClose(connection);
            }
            return rws;
        }

        public static void BuildSqlParameter(string ParameterName, SqlDbType dbtype,
    string Value, ParameterDirection dir, ref List<SqlParameter> paras)
        {
            SqlParameter temp = new SqlParameter(ParameterName, dbtype);
            temp.Value = Value;
            temp.Direction = dir;
            paras.Add(temp);
        }

        public static void BuildSqlParameter(string ParameterName, SqlDbType dbtype,
    bool Value, ParameterDirection dir, ref List<SqlParameter> paras)
        {
            SqlParameter temp = new SqlParameter(ParameterName, dbtype);
            temp.Value = Value;
            temp.Direction = dir;
            paras.Add(temp);
        }

        internal static void BuildSqlParameter(string ParameterName, SqlDbType dbtype,
    Int32 Value, ParameterDirection dir, ref List<SqlParameter> paras)
        {
            SqlParameter temp = new SqlParameter(ParameterName, dbtype);
            temp.Value = Value;
            temp.Direction = dir;
            paras.Add(temp);
        }
    }
}