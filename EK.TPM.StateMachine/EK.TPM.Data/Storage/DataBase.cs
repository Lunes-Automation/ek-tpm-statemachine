// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DataBase.cs" company="E&amp;K Automation GmbH">
//   Copyright (c) E&amp;K Automation GmbH. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace EK.TPM.Data.Storage
{
    using System;
    using System.Data;
    using System.Data.Odbc;
    using System.Data.OleDb;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using System.Threading;
    

    /// <summary>
    /// This Class distributes some kinds of Database Connections.
    /// You have the possibility to use ADO.Net, ODBC or OLEDB Connections.
    /// They are chosen by the Constructor.
    /// </summary>
    public class Database
    {
        #region Constants

        private const double LongWaitSeconds = 5;
        private const int MaxRetry = 10;
        private const double ShortWaitSeconds = 0.5;
        private const int SqlCmdTimeout = 60000;

        #endregion

        #region Fields

        private static readonly TimeSpan LongWait = TimeSpan.FromSeconds(LongWaitSeconds);
        private static readonly TimeSpan ShortWait = TimeSpan.FromSeconds(ShortWaitSeconds);
        private readonly string appPath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        private readonly OdbcConnectionStringBuilder connectionStringBuilderForOdbc;
        private readonly OleDbConnectionStringBuilder connectionStringBuilderForOledb;
        private readonly SqlConnectionStringBuilder connectionStringBuilderForSql;
        private readonly string strDsn = string.Empty;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="Database" /> class.
        ///     If you want to use OLEDB use this constructor.
        ///     The connection string must be stored in the Executable Path in the filename 'dbConnection.UDL'.
        /// </summary>
        public Database()
        {
            this.ConnectionType = ConnType.OLEDB;
            this.connectionStringBuilderForOledb = new OleDbConnectionStringBuilder { FileName = this.appPath + "\\dbConnection.udl" };
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        ///     If you want to use ODBC  use this constructor.
        /// </summary>
        /// <param name="dsn">
        /// System or user DNS.
        /// </param>
        /// <param name="user">
        /// Username used for Identification.
        /// </param>
        /// <param name="password">
        /// Password for the user.
        /// </param>
        public Database(string dsn, string user, string password)
        {
            this.ConnectionType = ConnType.ODBC;
            this.DatabaseUser = user;
            this.DatabasePassword = password;
            this.strDsn = dsn;
            this.connectionStringBuilderForOdbc = new OdbcConnectionStringBuilder();
            this.connectionStringBuilderForOdbc.Dsn = dsn;
            this.connectionStringBuilderForOdbc.Add("UID", user);
            this.connectionStringBuilderForOdbc.Add("PWD", password);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Database" /> class.
        /// </summary>
        /// <param name="ip">The IP.</param>
        /// <param name="user">The user.</param>
        /// <param name="password">The password.</param>
        /// <param name="databasename">The database name.</param>
        /// <param name="failoverPartner">The failover partner.</param>
        public Database(string ip, string user, string password, string databasename, string failoverPartner = "")
        {
            this.ConnectionType = ConnType.MSSQL;
            this.ServerIp = ip;
            this.DatabaseUser = user;
            this.DatabasePassword = password;
            this.DatabaseName = databasename;
            this.connectionStringBuilderForSql = new SqlConnectionStringBuilder();
            this.connectionStringBuilderForSql.UserID = this.DatabaseUser;
            this.connectionStringBuilderForSql.DataSource = this.ServerIp;
            this.connectionStringBuilderForSql.Password = this.DatabasePassword;
            this.connectionStringBuilderForSql.InitialCatalog = this.DatabaseName;
            this.connectionStringBuilderForSql.FailoverPartner = failoverPartner;

            this.connectionStringBuilderForSql["Trusted_Connection"] = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Database"/> class.
        ///     If you want to use OLEDB use this constructor.
        /// </summary>
        /// <param name="oleDbConnectionstring">
        /// The connection string.
        /// </param>
        public Database(string oleDbConnectionstring)
        {
            this.ConnectionType = ConnType.OLEDB;
            this.connectionStringBuilderForOledb = new OleDbConnectionStringBuilder();
            this.connectionStringBuilderForOledb.ConnectionString = oleDbConnectionstring;
        }

        #endregion

        #region Enums

        /// <summary>
        ///     Defines the type of Database connection.
        /// </summary>
        [Serializable]
        public enum ConnType
        {
            /// <summary>
            ///     Undefined connection type.
            /// </summary>
            Undefined = 0,

            /// <summary>
            ///     The Database is accessed via ODBC.
            /// </summary>
            ODBC = 1,

            /// <summary>
            ///     The Database is accessed via OLEDB.
            /// </summary>
            OLEDB = 2,

            /// <summary>
            ///     The Database is accessed via ADO.Net.
            /// </summary>
            MSSQL = 3,
        }

        /// <summary>
        ///     Defines the possible Database connection states.
        /// </summary>
        [Serializable]
        public enum DBState
        {
            /// <summary>
            ///     The Database connection is closed.
            /// </summary>
            DisConnected = 0,

            /// <summary>
            ///     The Database connection is open.
            /// </summary>
            Connected = 1,

            /// <summary>
            ///     The Database connection is connecting.
            /// </summary>
            Connecting = 2,

            /// <summary>
            ///     The Database connection is closing.
            /// </summary>
            DisConnecting = 3,
        }

        /// <summary>
        ///     SQL exception types.
        /// </summary>
        private enum RetryableSqlErrors
        {
            /// <summary>
            ///     The connection broken.
            /// </summary>
            ConnectionBroken = -1,

            /// <summary>
            ///     The timeout.
            /// </summary>
            Timeout = -2,

            /// <summary>
            ///     The out of memory.
            /// </summary>
            OutOfMemory = 701,

            /// <summary>
            ///     The out of locks.
            /// </summary>
            OutOfLocks = 1204,

            /// <summary>
            ///     The deadlock victim.
            /// </summary>
            DeadlockVictim = 1205,

            /// <summary>
            ///     The lock request timeout.
            /// </summary>
            LockRequestTimeout = 1222,

            /// <summary>
            ///     The timeout waiting for memory resource.
            /// </summary>
            TimeoutWaitingForMemoryResource = 8645,

            /// <summary>
            ///     The low memory condition.
            /// </summary>
            LowMemoryCondition = 8651,

            /// <summary>
            ///     The word breaker timeout.
            /// </summary>
            WordbreakerTimeout = 30053,
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the type of the connection.
        /// </summary>
        /// <value>
        ///     The type of the connection.
        /// </value>
        public ConnType ConnectionType { get; } = ConnType.Undefined;

        /// <summary>
        ///     Gets or sets the Database name used for Data operations.
        /// </summary>
        /// <value>
        ///     The name of the database.
        /// </value>
        public string DatabaseName { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the user password.
        /// </summary>
        /// <value>
        ///     The database password.
        /// </value>
        public string DatabasePassword { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the Username used for DB authentication.
        /// </summary>
        /// <value>
        ///     The database user.
        /// </value>
        public string DatabaseUser { get; set; } = string.Empty;

        /// <summary>
        ///     Gets or sets the IP Address of the Primary Database Server.
        /// </summary>
        /// <value>
        ///     The server IP.
        /// </value>
        public string ServerIp { get; set; } = string.Empty;

        #endregion

        #region Methods

        /// <summary>
        /// Attaches the specified database name into server.
        /// </summary>
        /// <param name="databaseName">
        /// Name of the database.
        /// </param>
        /// <param name="error">
        /// The error.
        /// </param>
        /// <returns>
        /// True if successful, otherwise false.
        /// </returns>
        public bool Attach(string databaseName, ref int error)
        {
            bool success = true;
            string currentPath = Directory.GetCurrentDirectory();
            string dataFile = currentPath + "\\databases\\" + databaseName + "\\" + databaseName + ".mdf";
            string logFile = currentPath + "\\databases\\" + databaseName + "\\" + databaseName + "_log.ldf";
            bool dataFileOk = File.Exists(dataFile);
            bool logFileOk = File.Exists(logFile);
            if (dataFileOk && logFileOk)
            {
                string theSql = "CREATE DATABASE [" + databaseName + "] ON \n" + "( FILENAME = N'" + dataFile + "' ),\n"
                                + "( FILENAME = N'" + logFile + "' )\n" + "FOR ATTACH";
                int result = this.NonQuery(theSql);
            }
            else
            {
                error = dataFileOk ? 0 : 0x02;
                error |= logFileOk ? 0 : 0x04;
                success = false;
            }

            return success;
        }

        /// <summary>
        ///     Test if the SQL server is available.
        /// </summary>
        /// <returns>True if available.</returns>
        public bool IsAvailable()
        {
            bool result = false;
            try
            {
                switch (this.ConnectionType)
                {
                    case ConnType.MSSQL:
                        using (var con = new SqlConnection(this.connectionStringBuilderForSql.ConnectionString))
                        {
                            con.Open();
                            result = con.State == ConnectionState.Open;
                        }

                        break;
                    case ConnType.ODBC:
                        using (var con = new OdbcConnection(this.connectionStringBuilderForOdbc.ConnectionString))
                        {
                            con.Open();
                            result = con.State == ConnectionState.Open;
                        }

                        break;
                    case ConnType.OLEDB:
                        using (var con = new OleDbConnection(this.connectionStringBuilderForOledb.ConnectionString))
                        {
                            con.Open();
                            result = con.State == ConnectionState.Open;
                        }

                        break;
                }
            }
            catch (Exception ex)
            {
                string m = ex.Message;
            }

            return result;
        }

        /// <summary>
        /// Query that returns no data but number of affected rows.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// Return value of query.
        /// </returns>
        public int NonQuery(OdbcCommand command)
        {
            int result = 0;
            try
            {
                using (var con = new OdbcConnection(this.connectionStringBuilderForOdbc.ConnectionString))
                {
                    con.Open();
                    var adapter = new OdbcDataAdapter();
                    command.Connection = con;
                    adapter.SelectCommand = command;
                    adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                    result = adapter.SelectCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                this.WriteErrorLog(ex.ToString(), command.CommandText);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Query that returns no data but number of affected rows.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// Return value of query.
        /// </returns>
        public int NonQuery(OleDbCommand command)
        {
            int result = 0;
            try
            {
                using (var con = new OleDbConnection(this.connectionStringBuilderForOledb.ConnectionString))
                {
                    con.Open();
                    var adapter = new OleDbDataAdapter();
                    command.Connection = con;
                    adapter.SelectCommand = command;
                    adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                    result = adapter.SelectCommand.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                this.WriteErrorLog(ex.ToString(), command.CommandText);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Query that returns no data but number of affected rows.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// Return value of query.
        /// </returns>
        public int NonQuery(SqlCommand command)
        {
            int retryCount = 0;
            int result = 0;
            while (true)
            {
                try
                {
                    using (var con = new SqlConnection(this.connectionStringBuilderForSql.ConnectionString))
                    {
                        con.Open();
                        var adapter = new SqlDataAdapter();
                        command.Connection = con;
                        adapter.SelectCommand = command;
                        adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                        result = adapter.SelectCommand.ExecuteNonQuery();
                    }

                    break;
                }
                catch (Exception ex)
                {
                    if (ex is SqlException &&
                        Enum.IsDefined(typeof(RetryableSqlErrors), ((SqlException)ex).Number) &&
                        retryCount < MaxRetry)
                    {
                        retryCount++;
                        Thread.Sleep(((SqlException)ex).Number == (int)RetryableSqlErrors.Timeout ? LongWait : ShortWait);
                        continue;
                    }
                    else
                    {
                        this.WriteErrorLog(ex.ToString(), command.CommandText);
                        throw;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Query that returns no data but number of affected rows.
        /// </summary>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <returns>
        /// The result of NonQuery.
        /// </returns>
        public int NonQuery(string sqlCommand)
        {
            int result = 0;
            int retryCount = 0;

            if (this.ConnectionType == ConnType.MSSQL)
            {
                while (true)
                {
                    try
                    {
                        using (var con = new SqlConnection(this.connectionStringBuilderForSql.ConnectionString))
                        {
                            con.Open();
                            var adapter = new SqlDataAdapter();
                            adapter.SelectCommand = new SqlCommand(sqlCommand, con);
                            adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                            result = adapter.SelectCommand.ExecuteNonQuery();
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (ex is SqlException &&
                            Enum.IsDefined(typeof(RetryableSqlErrors), ((SqlException)ex).Number) &&
                            retryCount < MaxRetry)
                        {
                            retryCount++;
                            Thread.Sleep(((SqlException)ex).Number == (int)RetryableSqlErrors.Timeout ? LongWait : ShortWait);
                            continue;
                        }
                        else
                        {
                            this.WriteErrorLog(ex.ToString(), sqlCommand);
                            throw;
                        }
                    }
                }
            }
            else if (this.ConnectionType == ConnType.ODBC)
            {
                try
                {
                    using (var con = new OdbcConnection(this.connectionStringBuilderForOdbc.ConnectionString))
                    {
                        con.Open();
                        var adapter = new OdbcDataAdapter();
                        adapter.SelectCommand = new OdbcCommand(sqlCommand, con);
                        adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                        result = adapter.SelectCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    this.WriteErrorLog(ex.ToString(), sqlCommand);
                    throw;
                }
            }
            else if (this.ConnectionType == ConnType.OLEDB)
            {
                try
                {
                    using (var con = new OleDbConnection(this.connectionStringBuilderForOledb.ConnectionString))
                    {
                        con.Open();
                        var adapter = new OleDbDataAdapter();
                        adapter.SelectCommand = new OleDbCommand(sqlCommand, con);
                        adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                        result = adapter.SelectCommand.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    this.WriteErrorLog(ex.ToString(), sqlCommand);
                    throw;
                }
            }

            return result;
        }

        /// <summary>
        /// Query that returns no data but number of affected rows. This query works transactional, changes done are will be
        /// rolled back in case of errors.
        /// </summary>
        /// <param name="sqlStatement">The SQL statement.</param>
        /// <returns>
        /// The result of NonQuery. -4 general exception, -3 exception on rollback, -2 rollback done, all other depend on the
        /// SQL
        /// statement itself.
        /// </returns>
        public int NonQueryWithTransaction(string sqlStatement)
        {
            int retryCount = 0;
            int result = 0;

            if (this.ConnectionType == ConnType.MSSQL)
            {
                while (true)
                {
                    try
                    {
                        using (var con = new SqlConnection(this.connectionStringBuilderForSql.ConnectionString))
                        {
                            con.Open();
                            var adapter = new SqlDataAdapter();
                            SqlTransaction transaction = con.BeginTransaction();

                            try
                            {
                                adapter.SelectCommand = new SqlCommand(sqlStatement, con);
                                adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                                adapter.SelectCommand.Transaction = transaction;

                                result = adapter.SelectCommand.ExecuteNonQuery();

                                transaction.Commit();
                            }
                            catch (Exception ex)
                            {
                                if (ex is SqlException &&
                                    Enum.IsDefined(typeof(RetryableSqlErrors), ((SqlException)ex).Number) &&
                                    retryCount < MaxRetry)
                                {
                                    retryCount++;
                                    Thread.Sleep(((SqlException)ex).Number == (int)RetryableSqlErrors.Timeout ? LongWait : ShortWait);
                                    continue;
                                }
                                else
                                {
                                    result = -2;
                                    this.WriteErrorLog(ex.ToString(), sqlStatement);

                                    // Attempt to roll back the transaction.
                                    try
                                    {
                                        transaction.Rollback();
                                        throw;
                                    }
                                    catch (Exception ex2)
                                    {
                                        result = -3;

                                        // This catch block will handle any errors that may have occurred
                                        // on the server that would cause the rollback to fail, such as
                                        // a closed connection.
                                        FileOperation.LogError(ex2, FileOperation.MaxLogFileSize);
                                        throw;
                                    }
                                }
                            }
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        if (ex is SqlException &&
                            Enum.IsDefined(typeof(RetryableSqlErrors), ((SqlException)ex).Number) &&
                            retryCount < MaxRetry)
                        {
                            retryCount++;
                            Thread.Sleep(((SqlException)ex).Number == (int)RetryableSqlErrors.Timeout ? LongWait : ShortWait);
                            continue;
                        }

                        this.WriteErrorLog(ex.ToString(), sqlStatement);
                        throw;
                    }
                }
            }
            else if (this.ConnectionType == ConnType.ODBC)
            {
                using (var con = new OdbcConnection(this.connectionStringBuilderForOdbc.ConnectionString))
                {
                    con.Open();
                    var adapter = new OdbcDataAdapter();
                    OdbcTransaction transaction = con.BeginTransaction();

                    try
                    {
                        adapter.SelectCommand = new OdbcCommand(sqlStatement, con);
                        adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                        adapter.SelectCommand.Transaction = transaction;

                        result = adapter.SelectCommand.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        result = -2;

                        this.WriteErrorLog(ex.ToString(), sqlStatement);

                        // Attempt to roll back the transaction.
                        try
                        {
                            transaction.Rollback();
                            throw;
                        }
                        catch (Exception ex2)
                        {
                            result = -3;

                            // This catch block will handle any errors that may have occurred
                            // on the server that would cause the rollback to fail, such as
                            // a closed connection.
                            FileOperation.LogError(ex2, FileOperation.MaxLogFileSize);
                            throw;
                        }
                    }
                }
            }
            else if (this.ConnectionType == ConnType.OLEDB)
            {
                using (var con = new OleDbConnection(this.connectionStringBuilderForOledb.ConnectionString))
                {
                    con.Open();
                    var adapter = new OleDbDataAdapter();
                    OleDbTransaction transaction = con.BeginTransaction();

                    try
                    {
                        adapter.SelectCommand = new OleDbCommand(sqlStatement, con);
                        adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                        adapter.SelectCommand.Transaction = transaction;

                        result = adapter.SelectCommand.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        result = -2;

                        this.WriteErrorLog(ex.ToString(), sqlStatement);

                        // Attempt to roll back the transaction.
                        try
                        {
                            transaction.Rollback();
                            throw;
                        }
                        catch (Exception ex2)
                        {
                            result = -3;

                            // This catch block will handle any errors that may have occurred
                            // on the server that would cause the rollback to fail, such as
                            // a closed connection.
                            FileOperation.LogError(ex2, FileOperation.MaxLogFileSize);
                            throw;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Used for select query.
        /// </summary>
        /// <param name="sqlCommand">The SQL string.</param>
        /// <param name="querySchema">if set to <c>true</c> [query schema].</param>
        /// <param name="schemaType">Type of the schema.</param>
        /// <returns>
        /// A Data Set with the selected Data tables and Data Rows.
        /// </returns>
        public DataSet Query(
            string sqlCommand,
            bool querySchema = false,
            SchemaType schemaType = SchemaType.Source)
        {
            var result = new DataSet();

            if (this.ConnectionType == ConnType.MSSQL)
            {
                int retryCount = 0;
                while (true)
                {
                    try
                    {
                        using (var con = new SqlConnection(this.connectionStringBuilderForSql.ConnectionString))
                        {
                            con.Open();
                            var adapter = new SqlDataAdapter();
                            adapter.SelectCommand = new SqlCommand(sqlCommand, con);
                            adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                            if (querySchema)
                            {
                                adapter.FillSchema(result, schemaType);
                            }

                            adapter.Fill(result);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex is SqlException &&
                            Enum.IsDefined(typeof(RetryableSqlErrors), ((SqlException)ex).Number) &&
                            retryCount < MaxRetry)
                        {
                            retryCount++;
                            Thread.Sleep(((SqlException)ex).Number == (int)RetryableSqlErrors.Timeout ? LongWait : ShortWait);
                            continue;
                        }
                        else
                        {
                            this.WriteErrorLog(ex.ToString(), sqlCommand);
                            throw;
                        }
                    }
                }
            }
            else if (this.ConnectionType == ConnType.ODBC)
            {
                try
                {
                    using (var con = new OdbcConnection(this.connectionStringBuilderForOdbc.ConnectionString))
                    {
                        con.Open();
                        var adapter = new OdbcDataAdapter();
                        adapter.SelectCommand = new OdbcCommand(sqlCommand, con);
                        adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                        if (querySchema)
                        {
                            adapter.FillSchema(result, schemaType);
                        }

                        adapter.Fill(result);
                    }
                }
                catch (Exception ex)
                {
                    this.WriteErrorLog(ex.ToString(), sqlCommand);
                    throw;
                }
            }
            else if (this.ConnectionType == ConnType.OLEDB)
            {
                try
                {
                    using (var con = new OleDbConnection(this.connectionStringBuilderForOledb.ConnectionString))
                    {
                        con.Open();
                        var adapter = new OleDbDataAdapter();
                        adapter.SelectCommand = new OleDbCommand(sqlCommand, con);
                        adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                        if (querySchema)
                        {
                            adapter.FillSchema(result, schemaType);
                        }

                        adapter.Fill(result);
                    }
                }
                catch (Exception ex)
                {
                    this.WriteErrorLog(ex.ToString(), sqlCommand);
                    throw;
                }
            }

            return result;
        }

        /// <summary>
        /// Query that returns no data but number of affected rows.
        /// </summary>
        /// <param name="command">
        /// SQL command object.
        /// </param>
        /// <returns>
        /// result as dataset.
        /// </returns>
        public DataSet Query(OdbcCommand command)
        {
            var result = new DataSet();
            try
            {
                using (var con = new OdbcConnection(this.connectionStringBuilderForOdbc.ConnectionString))
                {
                    con.Open();
                    var adapter = new OdbcDataAdapter();
                    command.Connection = con;
                    adapter.SelectCommand = command;
                    adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                    adapter.Fill(result);
                }
            }
            catch (Exception ex)
            {
                this.WriteErrorLog(ex.ToString(), command.CommandText);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Query that returns no data but number of affected rows.
        /// </summary>
        /// <param name="command">
        /// SQL command object.
        /// </param>
        /// <returns>
        /// result as dataset.
        /// </returns>
        public DataSet Query(OleDbCommand command)
        {
            var result = new DataSet();
            try
            {
                using (var con = new OleDbConnection(this.connectionStringBuilderForOledb.ConnectionString))
                {
                    con.Open();
                    var adapter = new OleDbDataAdapter();
                    command.Connection = con;
                    adapter.SelectCommand = command;
                    adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                    adapter.Fill(result);
                }
            }
            catch (Exception ex)
            {
                this.WriteErrorLog(ex.ToString(), command.CommandText);
                throw;
            }

            return result;
        }

        /// <summary>
        /// Query that returns no data but number of affected rows.
        /// </summary>
        /// <param name="command">SQL command object.</param>
        /// <returns>
        /// result as dataset.
        /// </returns>
        public DataSet Query(SqlCommand command)
        {
            var result = new DataSet();
            int retryCount = 0;
            while (true)
            {
                try
                {
                    using (var con = new SqlConnection(this.connectionStringBuilderForSql.ConnectionString))
                    {
                        con.Open();
                        var adapter = new SqlDataAdapter();
                        command.Connection = con;
                        adapter.SelectCommand = command;
                        adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                        adapter.Fill(result);
                    }
                }
                catch (Exception ex)
                {
                    if (ex is SqlException &&
                        Enum.IsDefined(typeof(RetryableSqlErrors), ((SqlException)ex).Number) &&
                        retryCount < MaxRetry)
                    {
                        retryCount++;
                        Thread.Sleep(((SqlException)ex).Number == (int)RetryableSqlErrors.Timeout ? LongWait : ShortWait);
                        continue;
                    }
                    else
                    {
                        this.WriteErrorLog(ex.ToString(), command.CommandText);
                        throw;
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// Scalars the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns>
        /// The result dataset of database.Query.
        /// </returns>
        public object Scalar(SqlCommand command)
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    using (var con = new SqlConnection(this.connectionStringBuilderForSql.ConnectionString))
                    {
                        con.Open();
                        var adapter = new SqlDataAdapter();
                        command.Connection = con;
                        return command.ExecuteScalar();
                    }
                }
                catch (Exception ex)
                {
                    if (ex is SqlException &&
                        Enum.IsDefined(typeof(RetryableSqlErrors), ((SqlException)ex).Number) &&
                        retryCount < MaxRetry)
                    {
                        retryCount++;
                        Thread.Sleep(((SqlException)ex).Number == (int)RetryableSqlErrors.Timeout ? LongWait : ShortWait);
                        continue;
                    }
                    else
                    {
                        this.WriteErrorLog(ex.ToString(), command.CommandText);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// SQL command with return type scalar.
        /// </summary>
        /// <param name="sqlScalar">The SQL scalar.</param>
        /// <returns>
        /// True if successful, otherwise false.
        /// </returns>
        public object Scalar(string sqlScalar)
        {
            int retryCount = 0;

            switch (this.ConnectionType)
            {
                case ConnType.MSSQL:
                    {
                        while (true)
                        {
                            try
                            {
                                using (var con = new SqlConnection(this.connectionStringBuilderForSql.ConnectionString))
                                {
                                    con.Open();
                                    var adapter = new SqlDataAdapter();
                                    adapter.SelectCommand = new SqlCommand(sqlScalar, con);
                                    adapter.SelectCommand.CommandTimeout = SqlCmdTimeout;
                                    return adapter.SelectCommand.ExecuteScalar();
                                }
                            }
                            catch (Exception ex)
                            {
                                if (ex is SqlException &&
                                          Enum.IsDefined(typeof(RetryableSqlErrors), ((SqlException)ex).Number) &&
                                          retryCount < MaxRetry)
                                {
                                    retryCount++;
                                    Thread.Sleep(((SqlException)ex).Number == (int)RetryableSqlErrors.Timeout ? LongWait : ShortWait);
                                    continue;
                                }
                                else
                                {
                                    this.WriteErrorLog(ex.ToString(), sqlScalar);
                                    throw;
                                }
                            }
                        }
                    }

                case ConnType.ODBC:
                    {
                        try
                        {
                            using (var con = new OdbcConnection(this.connectionStringBuilderForOdbc.ConnectionString))
                            {
                                con.Open();
                                var adapter = new OdbcDataAdapter();
                                adapter.SelectCommand = new OdbcCommand(sqlScalar, con);
                                adapter.SelectCommand.CommandTimeout = 60000;
                                return adapter.SelectCommand.ExecuteScalar();
                            }
                        }
                        catch (Exception ex)
                        {
                            this.WriteErrorLog(ex.ToString(), sqlScalar);
                            throw;
                        }
                    }

                case ConnType.OLEDB:
                    {
                        try
                        {
                            using (var con = new OleDbConnection(this.connectionStringBuilderForOledb.ConnectionString))
                            {
                                con.Open();
                                var adapter = new OleDbDataAdapter();
                                adapter.SelectCommand = new OleDbCommand(sqlScalar, con);
                                adapter.SelectCommand.CommandTimeout = 60000;
                                return adapter.SelectCommand.ExecuteScalar();
                            }
                        }
                        catch (Exception ex)
                        {
                            this.WriteErrorLog(ex.ToString(), sqlScalar);
                            throw;
                        }
                    }

                default:
                    {
                        return false;
                    }
            }
        }

        /// <summary>
        /// Writes an error into 'ErrorLog.txt' text file.
        /// </summary>
        /// <param name="message">
        /// The Error message.
        /// </param>
        /// <param name="strCommand">
        /// SQL command.
        /// </param>
        private void WriteErrorLog(string message, string strCommand)
        {
            FileOperation.LogError("Message: \r\n" + message + "\r\n Command: \r\n" + strCommand, FileOperation.MaxLogFileSize);
        }

        #endregion
    }
}