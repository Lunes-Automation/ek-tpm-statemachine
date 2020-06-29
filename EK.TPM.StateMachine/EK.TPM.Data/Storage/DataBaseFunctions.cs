namespace EK.TPM.Data.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Odbc;
    using System.Data.OleDb;
    using System.Data.SqlClient;
    using System.IO;
    using System.Net;
    using System.Net.Mail;
    using System.Timers;
    

    /// <summary>
    /// Distributes a collection of Database functions.
    /// </summary>
    public class DataBaseFunctions : IDisposable
    {
        #region Fields

        /// <summary>
        /// Lock object to avoid double use of same code from more threads at the same time.
        /// </summary>
        private static readonly object LockStatusCache = new object();

        /// <summary>
        /// Cache for reducing the required changes to the table T_Status.
        /// </summary>
        private static Dictionary<string, string> statusCache = new Dictionary<string, string>();

        /// <summary>
        /// The timer reorganize or rebuild indices.
        /// </summary>
        private readonly Timer timerReorganizeOrRebuildIndices = new Timer(500);

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DataBaseFunctions" /> class.
        /// </summary>
        /// <param name="theDatabase">The database object <see cref="DataBase.Database" /> which is used to access the database.</param>
        /// <exception cref="System.Exception">Database connection failed! Terminating.</exception>
        public DataBaseFunctions(Database theDatabase)
        {
            Database = theDatabase;
            while (true)
            {
                if (!theDatabase.IsAvailable())
                {
                    int waitingTime = 5000;
                    System.Threading.Thread.Sleep(waitingTime);
                    FileOperation.LogError($"Database is not available. Retry in {waitingTime} ms", 128 * 1024 * 1024);
                }
                else
                {
                    break;
                }
            }

            FileOperation.MaxLogFileSize = GetConfigValue<int>("MaxLogFileSize");

            this.timerReorganizeOrRebuildIndices.Elapsed += this.OnReorganizeOrRebuildIndicesTimerElapsed;
            this.timerReorganizeOrRebuildIndices.AutoReset = false;
            this.timerReorganizeOrRebuildIndices.Start();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the database.
        /// </summary>
        /// <value>
        /// The database.
        /// </value>
        public static Database Database
        {
            get; set;
        }

        /// <summary>
        /// Gets a value indicating whether this instance is available.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is available; otherwise, <c>false</c>.
        /// </value>
        public bool IsAvailable
        {
            get
            {
                return Database.IsAvailable();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Builds the language object.
        /// </summary>
        /// <param name="item">
        /// The item.
        /// </param>
        /// <param name="standardText">
        /// The standard text.
        /// </param>
        /// <returns>
        /// If method was executed successfully.
        /// </returns>
        public static bool BuildLanguageObject(string item, string standardText)
        {
            DataBaseFunctions.NonQuery(
                "EXEC P_TPM_InsertOrUpdateAllLanguagesForLanguageObject @Object = '" + item + "', @StandardText = '"
                + standardText + "'");

            return true;
        }

        /// <summary>
        /// Disposes the data set.
        /// </summary>
        /// <param name="datasetToDispose">
        /// The dataset to dispose.
        /// </param>
        public static void DisposeDataSet(DataSet datasetToDispose)
        {
            if (datasetToDispose != null)
            {
                foreach (DataTable t in datasetToDispose.Tables)
                {
                    t.Clear();
                    t.Dispose();
                }

                datasetToDispose.Tables.Clear();
                datasetToDispose.Dispose();
                datasetToDispose = null;
            }
        }

        /// <summary>
        /// Does the action.
        /// </summary>
        /// <param name="actionName">
        /// Name of the action.
        /// </param>
        /// <param name="parameter1">
        /// The parameter1.
        /// </param>
        /// <param name="parameter2">
        /// The parameter2.
        /// </param>
        /// <param name="parameter3">
        /// The parameter3.
        /// </param>
        /// <param name="parameter4">
        /// The parameter4.
        /// </param>
        /// <param name="parameter5">
        /// The parameter5.
        /// </param>
        /// <param name="parameter6">
        /// The parameter6.
        /// </param>
        /// <param name="parameter7">
        /// The parameter7.
        /// </param>
        /// <param name="parameter8">
        /// The parameter8.
        /// </param>
        /// <param name="who">
        /// The who value.
        /// </param>
        /// <param name="timeoutInMs">
        /// The timeout in milliseconds.
        /// </param>
        /// <param name="reply">
        /// The reply.
        /// </param>
        /// <returns>
        /// 0 success (reply is valid)
        /// -1 action insert in DB failed
        /// -2 DataRow with reply not found in DB
        /// -3 Timeout.
        /// </returns>
        public static int DoAction(
            string actionName,
            string parameter1,
            string parameter2,
            string parameter3,
            string parameter4,
            string parameter5,
            string parameter6,
            string parameter7,
            string parameter8,
            string who,
            int timeoutInMs,
            out string reply)
        {
            DateTime now = DateTime.Now;
            string sql = "INSERT INTO T_TPM_VIS_Action " + "([ActionName]" + ", [Parameter1]" + ", [Parameter2]"
                         + ", [Parameter3]" + ", [Parameter4]" + ", [Parameter5]" + ", [Parameter6]" + ", [Parameter7]"
                         + ", [Parameter8]" + ", [Who]" + ", [Time]" + ", [Reply])" + " VALUES " + "('" + actionName
                         + "'," + " '" + parameter1 + "'," + " '" + parameter2 + "'," + " '" + parameter3 + "'," + " '"
                         + parameter4 + "'," + " '" + parameter5 + "'," + " '" + parameter6 + "'," + " '" + parameter7
                         + "'," + " '" + parameter8 + "'," + " '" + who + "'," + " '" + now.ToString("s") + "','')";

            if (DataBaseFunctions.NonQuery(sql) > 0)
            {
                if (timeoutInMs > 0)
                {
                    while (now.AddMilliseconds(timeoutInMs) > DateTime.Now)
                    {
                        System.Threading.Thread.Sleep(100);
                        DataSet result =
                            DataBaseFunctions.Query("SELECT Reply FROM T_TPM_VIS_Action WHERE ActionName = '" + actionName + "' AND Time = '" + now.ToString("s") + "'");
                        if (DataBaseFunctions.TestDataSet(result, 1))
                        {
                            reply = result.Tables[0].Rows[0]["Reply"].ToString();
                            if (reply != string.Empty)
                            {
                                DataBaseFunctions.NonQuery("DELETE FROM T_TPM_VIS_Action WHERE ActionName = '" + actionName + "' AND Time = '" + now.ToString("s") + "'");
                                return 0;
                            }
                        }
                        else
                        {
                            DataBaseFunctions.NonQuery("DELETE FROM T_TPM_VIS_Action WHERE ActionName = '" + actionName + "' AND Time = '" + now.ToString("s") + "'");
                            reply = string.Empty;
                            return -2;
                        }
                    }

                    DataBaseFunctions.NonQuery("DELETE FROM T_TPM_VIS_Action WHERE ActionName = '" + actionName + "' AND Time = '" + now.ToString("s") + "'");
                    reply = string.Empty;
                    return -3;
                }

                reply = string.Empty;
                return 0;
            }

            reply = string.Empty;
            return -1;
        }

        /// <summary>
        /// Gets the config value.
        /// </summary>
        /// <typeparam name="T">The expected type to be returned.</typeparam>
        /// <param name="item">The requesting item.</param>
        /// <returns>
        /// The config value of item.
        /// </returns>
        /// <exception cref="InvalidDataException">Requested type (" + type of(T).FullName + ") does not match stored type (" + result.Tables[0].Rows[0]["DataTypeName"].ToString() + ")!.</exception>
        public static T GetConfigValue<T>(string item)
        {
            DataSet result;
            if (item == string.Empty)
            {
                return default(T);
            }

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "SELECT TOP 1 [ItemValue], [DataTypeName] FROM [dbo].[T_TPM_Config] WHERE [Item] = @item";
            SqlParameter p1 = new SqlParameter("@item", item);
            cmd.Parameters.Add(p1);
            result = Database.Query(cmd);

            if (TestDataSet(result, 1))
            {
                if (result.Tables[0].Rows[0]["DataTypeName"].ToString() == typeof(T).AssemblyQualifiedName)
                {
                    if (DataOperation.TryConvertValue(result.Tables[0].Rows[0]["ItemValue"].ToString(), out T returnValue))
                    {
                        return returnValue;
                    }
                }
                else
                {
                    throw new InvalidDataException("Requested type (" + typeof(T).AssemblyQualifiedName + ") does not match stored type (" + result.Tables[0].Rows[0]["DataTypeName"].ToString() + ")!");
                }

                return default(T);
            }

            return default(T);
        }

        /// <summary>
        /// Gets the text.
        /// </summary>
        /// <param name="objectName">
        /// Name of the object.
        /// </param>
        /// <param name="language">
        /// The language.
        /// </param>
        /// <param name="userLevel">
        /// The user level.
        /// </param>
        /// <param name="toolTipText">
        /// The tool tip text.
        /// </param>
        /// <returns>
        /// The text in requested language of objectName or a empty string if not found.
        /// </returns>
        public static string GetText(string objectName, string language, out int userLevel, out string toolTipText)
        {
            userLevel = -1;
            toolTipText = string.Empty;
            DataSet result =
                Database.Query("SELECT [Authorizationlevel], [ToolTipText], [Text]  FROM [dbo].[V_TPM_Language] WHERE [Object] = '" + objectName + "' AND [Language] = '" + language + "'");
            if (result != null && result.Tables.Count > 0 && result.Tables[0].Rows.Count > 0)
            {
                userLevel = (int)result.Tables[0].Rows[0]["Authorizationlevel"];
                toolTipText = (string)result.Tables[0].Rows[0]["ToolTipText"];
                return (string)result.Tables[0].Rows[0]["Text"];
            }

            return string.Empty;
        }

        /// <summary>
        /// Initializes the configuration value if not exist.
        /// </summary>
        /// <typeparam name="T">Type of the item value.</typeparam>
        /// <param name="item">The item.</param>
        /// <param name="itemGroup">The item group.</param>
        /// <param name="itemValue">The item value.</param>
        /// <param name="description">The description.</param>
        /// <param name="lastChangedBy">The last changed by.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="minValue">The minimum value.</param>
        /// <param name="maxValue">The maximum value.</param>
        /// <param name="regularExpression">The regular expression.</param>
        /// <param name="notificationRequired">if set to <c>true</c> [notification required].</param>
        /// <exception cref="InvalidDataException">Is thrown if the data type is not valid.</exception>
        public static void InitializeConfigValueIfNotExist<T>(string item, string itemGroup, T itemValue, string description, string lastChangedBy, T defaultValue, double? minValue = null, double? maxValue = null, string regularExpression = null, bool notificationRequired = false)
        {
            string fullNameOfT = typeof(T).AssemblyQualifiedName;
            bool typeOfTIsValid = fullNameOfT == typeof(string).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(decimal).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(double).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(float).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(ulong).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(long).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(uint).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(int).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(ushort).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(short).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(byte).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(sbyte).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(DateTime).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(TimeSpan).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(bool).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(IPAddress).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(MailAddress).AssemblyQualifiedName ||
                                  typeof(T).IsEnum;
            if (!typeOfTIsValid)
            {
                throw new InvalidDataException(fullNameOfT + " is an invalide data type to be stored!");
            }

            string sql = "EXECUTE[dbo].[P_TPM_TryInsertConfig] @Item, @ItemGroup, @ItemValue, @Description, @LastChangedBy, @DefaultValue, @DataTypeName, @MinValue, @MaxValue, @RegEx, @NotificationRequired";

            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.Add("@Item", SqlDbType.NVarChar, 128).SqlValue = item;
            cmd.Parameters.Add("@ItemGroup", SqlDbType.NVarChar, 50).SqlValue = itemGroup;
            cmd.Parameters.Add("@ItemValue", SqlDbType.NVarChar, -1).SqlValue = itemValue.ToString();
            cmd.Parameters.Add("@Description", SqlDbType.NVarChar, 512).SqlValue = description;
            cmd.Parameters.Add("@LastChangedBy", SqlDbType.NVarChar, 256).SqlValue = lastChangedBy;
            cmd.Parameters.Add("@DefaultValue", SqlDbType.NVarChar, -1).SqlValue = defaultValue.ToString();
            cmd.Parameters.Add("@DataTypeName", SqlDbType.NVarChar, 512).SqlValue = fullNameOfT;
            cmd.Parameters.Add("@MinValue", SqlDbType.Float).SqlValue = minValue.HasValue ? (double)minValue.Value : System.Data.SqlTypes.SqlDouble.Null;
            cmd.Parameters.Add("@MaxValue", SqlDbType.Float).SqlValue = maxValue.HasValue ? (double)maxValue.Value : System.Data.SqlTypes.SqlDouble.Null;
            cmd.Parameters.Add("@RegEx", SqlDbType.NVarChar, -1).SqlValue = !string.IsNullOrEmpty(regularExpression) ? regularExpression : System.Data.SqlTypes.SqlString.Null;
            cmd.Parameters.Add("@NotificationRequired", SqlDbType.TinyInt).SqlValue = notificationRequired ? "0" : "1";

            NonQuery(cmd);
        }

        /// <summary>
        /// Inserts the change request.
        /// </summary>
        /// <param name="valueString">
        /// The SQL string.
        /// </param>
        /// <returns>
        /// True if change was successful, else false.
        /// </returns>
        public static int InsertChangeRequest(string valueString)
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = "INSERT INTO T_TPM_VIS_ChangeRequest (SQLStatement) VALUES (@p1)";
            SqlParameter p1 = new SqlParameter("@p1", valueString);
            cmd.Parameters.Add(p1);
            return NonQuery(cmd);
        }

        /// <summary>
        /// Logins the specified username.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <returns>
        /// true if login was successful, else false.
        /// </returns>
        public static int Login(string username, string password)
        {
            if (username == "Service" && (DataOperation.DecryptMessage(password, "ek2402ek") == "ek2402ek"))
            {
                DataSet result = Database.Query("SELECT [UserGroupID] FROM [dbo].[T_TPM_User] WHERE [Username] = 'Service'");
                if (TestDataSet(result, 1))
                {
                    return (int)result.Tables[0].Rows[0]["UserGroupID"];
                }

                return 0;
            }
            else
            {
                DataSet result = Database.Query("SELECT [UserGroupID] FROM [dbo].[T_TPM_User] WHERE [Username] = '" + username + "' AND [Password] = '" + password + "'");

                if (TestDataSet(result, 1))
                {
                    return (int)result.Tables[0].Rows[0]["UserGroupID"];
                }

                return 0;
            }
        }

        /// <summary>
        /// The Non Query.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <returns>
        /// The result of the Non Query.
        /// </returns>
        public static int NonQuery(object command)
        {
            if (command is SqlCommand)
            {
                return Database.NonQuery((SqlCommand)command);
            }

            if (command is OdbcCommand)
            {
                return Database.NonQuery((OdbcCommand)command);
            }

            if (command is OleDbCommand)
            {
                return Database.NonQuery((OleDbCommand)command);
            }

            // TODO: was sinnvolles ?!
            return -1;
        }

        /// <summary>
        /// The non query operation.
        /// </summary>
        /// <param name="sqlCommand">
        /// The SQL command.
        /// </param>
        /// <returns>
        /// The result of database non query.
        /// </returns>
        public static int NonQuery(string sqlCommand)
        {
            return Database.NonQuery(sqlCommand);
        }

        /// <summary>
        /// The NonQuery with transaction.
        /// </summary>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <returns>The result dataset of database.Query.</returns>
        public static int NonQueryWithTransaction(string sqlCommand)
        {
            return Database.NonQueryWithTransaction(sqlCommand);
        }

        /// <summary>
        /// Queries the specified SQL command.
        /// </summary>
        /// <param name="sqlCommand">The SQL command.</param>
        /// <param name="querySchema">if set to <c>true</c> [query schema].</param>
        /// <param name="schemaType">Type of the schema.</param>
        /// <returns>The result dataset of database.Query.</returns>
        public static DataSet Query(
                                    string sqlCommand,
                                    bool querySchema = false,
                                    SchemaType schemaType = SchemaType.Source)
        {
            DataSet result = Database.Query(sqlCommand, querySchema, schemaType);
            return result;
        }

        /// <summary>
        /// Query a command object.
        /// </summary>
        /// <param name="command">The SQL command.</param>
        /// <returns>
        /// The result.<see cref="DataSet" />.
        /// </returns>
        public static DataSet Query(object command)
        {
            if (command is SqlCommand)
            {
                return Database.Query((SqlCommand)command);
            }

            if (command is OdbcCommand)
            {
                return Database.Query((OdbcCommand)command);
            }

            if (command is OleDbCommand)
            {
                return Database.Query((OleDbCommand)command);
            }

            return null;
        }

        /// <summary>
        /// Queries the specified SQL query.
        /// </summary>
        /// <param name="sql_query">
        /// The SQL query.
        /// </param>
        /// <returns>
        /// The result dataset of database.Query.
        /// </returns>
        public static DataSet Query(string sql_query)
        {
            DataSet result = Database.Query(sql_query);
            return result;
        }

        /// <summary>
        /// Reorganize the indices of the connected database.
        /// </summary>
        public static void ReOrganizeIndices()
        {
            NonQuery("EXEC [dbo].[P_TPM_ReorganizeIndizes]");
        }

        /// <summary>
        /// SQL scalar.
        /// </summary>
        /// <param name="sqlScalar">
        /// The SQL scalar.
        /// </param>
        /// <returns>
        /// True if successful, otherwise false.
        /// </returns>
        public static object Scalar(string sqlScalar)
        {
            return Database.Scalar(sqlScalar);
        }

        /// <summary>
        /// Sets the action reply.
        /// </summary>
        /// <param name="actionName">
        /// Name of the action.
        /// </param>
        /// <param name="requestTime">
        /// The request time.
        /// </param>
        /// <param name="reply">
        /// The reply.
        /// </param>
        public static void SetActionReply(string actionName, DateTime requestTime, string reply)
        {
            DataBaseFunctions.NonQuery(
                "UPDATE T_TPM_VIS_Action SET Reply = '" + reply + "' WHERE ActionName = '" + actionName
                + "' AND Time = '" + requestTime.ToString("s") + "'");
        }

        /// <summary>
        /// Sets all rights of service.
        /// Executes P_TPM_SetAllRightsOfService.
        /// </summary>
        public static void SetAllRightsOfService()
        {
            NonQuery("EXEC P_TPM_SetAllRightsOfService");
        }

        /// <summary>
        /// Sets the configuration value.
        /// </summary>
        /// <typeparam name="T">Type of value to save.</typeparam>
        /// <param name="item">The item.</param>
        /// <param name="itemValue">The item value.</param>
        /// <param name="changedBy">The changed by.</param>
        /// <returns>
        /// true if value was saved.
        /// </returns>
        /// <exception cref="InvalidDataException">Is thrown if the data type is not valid.</exception>
        public static bool SetConfigValue<T>(string item, T itemValue, string changedBy)
        {
            if (item == string.Empty)
            {
                return false;
            }

            string fullNameOfT = typeof(T).AssemblyQualifiedName;
            bool typeOfTIsValid = fullNameOfT == typeof(string).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(decimal).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(double).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(float).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(ulong).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(long).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(uint).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(int).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(ushort).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(short).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(byte).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(sbyte).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(DateTime).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(TimeSpan).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(bool).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(IPAddress).AssemblyQualifiedName ||
                                  fullNameOfT == typeof(MailAddress).AssemblyQualifiedName ||
                                  typeof(T).IsEnum;
            if (!typeOfTIsValid)
            {
                throw new InvalidDataException(fullNameOfT + " is an invalide data type to be stored!");
            }

            string sql = "EXECUTE[dbo].[P_TPM_UpdateConfig] @Item, @ItemValue, @LastChangedBy, @DataTypeName";
            SqlCommand cmd = new SqlCommand();
            cmd.CommandText = sql;
            cmd.Parameters.Add("@Item", SqlDbType.NVarChar, 128).SqlValue = item;
            cmd.Parameters.Add("@ItemValue", SqlDbType.NVarChar, -1).SqlValue = itemValue.ToString();
            cmd.Parameters.Add("@LastChangedBy", SqlDbType.NVarChar, 256).SqlValue = changedBy;
            cmd.Parameters.Add("@DataTypeName", SqlDbType.NVarChar, 256).SqlValue = fullNameOfT;
            NonQuery(cmd);

            return true;
        }

        ///// <summary>
        ///// Sets the status value. Language-KeyValue is the combination of "MES-CON OS TPM.Status.[item][itemStatus]".
        ///// </summary>
        ///// <param name="item">The item to update.</param>
        ///// <param name="itemStatus">The status of the item to update.</param>
        ///// <param name="titelStandardText">The title standard text.</param>
        ///// <param name="textStandardText">The text standard text.</param>
        ///// <param name="color">The color.</param>
        ///// <param name="millisecondsToExpire">The milliseconds to expire.</param>
        ///// <param name="parameter">Parameter to be inserted in the TextReference.Text. Will be inserted for %P in oder of appearance.</param>
        ///// <returns>
        ///// True if successful, else false.
        ///// </returns>
        ///// <exception cref="System.Exception">Item must not be Empty
        ///// or
        ///// Item must start with \MES-CON OS TPM.Status.\.</exception>
        //public static bool SetStatusValue(
        //                                  string item,
        //                                  string itemStatus,
        //                                  string titelStandardText,
        //                                  string textStandardText,
        //                                  System.Drawing.Color color,
        //                                  int millisecondsToExpire,
        //                                  params string[] parameter)
        //{
        //    if (item == string.Empty)
        //    {
        //        throw new Exception("Item must not be Empty");
        //    }
        //    else if (!item.StartsWith("MES-CON OS TPM.Status."))
        //    {
        //        throw new Exception("Item must start with \"MES-CON OS TPM.Status.\"");
        //    }
        //    else
        //    {
        //        lock (LockStatusCache)
        //        {
        //            bool titleOkay = false;
        //            bool textOkay = false;
        //            string keyValue = string.Concat(item, itemStatus);

        //            titleOkay = statusCache.ContainsKey(keyValue + ".Title") && statusCache[keyValue + ".Title"].Equals(titelStandardText);
        //            if (!titleOkay)
        //            {
        //                if (BuildLanguageObject(keyValue + ".Title", titelStandardText))
        //                {
        //                    statusCache.Add(keyValue + ".Title", titelStandardText);
        //                    titleOkay = true;
        //                }
        //            }

        //            string textStandard = textStandardText.Substring(0, textStandardText.Length > 4000 ? 4000 : textStandardText.Length);
        //            textOkay = statusCache.ContainsKey(keyValue + ".Text") && statusCache[keyValue + ".Text"].Equals(textStandard);
        //            if (!textOkay)
        //            {
        //                if (BuildLanguageObject(keyValue + ".Text", textStandard))
        //                {
        //                    statusCache.Add(keyValue + ".Text", textStandard);
        //                    textOkay = true;
        //                }
        //            }

        //            if (titleOkay && textOkay)
        //            {
        //                DataTable parameters = new DataTable();
        //                parameters.Columns.Add("Item", typeof(string));
        //                parameters.Columns.Add("ParameterNumber", typeof(int));
        //                parameters.Columns.Add("ParameterValue", typeof(string));

        //                for (int i = 0; i < parameter.Length; i++)
        //                {
        //                    parameters.Rows.Add(item, i, parameter[i].Substring(0, parameter[i].Length > 4000 ? 4000 : parameter[i].Length));
        //                }

        //                SqlCommand sqlCommand = new SqlCommand("P_TPM_InsertOrUpdateT_TPM_Status");
        //                sqlCommand.CommandType = CommandType.StoredProcedure;
        //                sqlCommand.Parameters.Add("Item", SqlDbType.NVarChar, 128);
        //                sqlCommand.Parameters.Add("ItemStatus", SqlDbType.NVarChar, 50);
        //                sqlCommand.Parameters.Add("TitelStandardText", SqlDbType.NVarChar, 256);
        //                sqlCommand.Parameters.Add("TextStandardText", SqlDbType.NVarChar, 512);
        //                sqlCommand.Parameters.Add("DisplayColorAlpha", SqlDbType.TinyInt);
        //                sqlCommand.Parameters.Add("DisplayColorRed", SqlDbType.TinyInt);
        //                sqlCommand.Parameters.Add("DisplayColorGreen", SqlDbType.TinyInt);
        //                sqlCommand.Parameters.Add("DisplayColorBlue", SqlDbType.TinyInt);
        //                sqlCommand.Parameters.Add("Description", SqlDbType.NVarChar, 256);
        //                sqlCommand.Parameters.Add("MSToStatusExpires", SqlDbType.Int);
        //                sqlCommand.Parameters.Add("StandardValue", SqlDbType.TinyInt);
        //                sqlCommand.Parameters.Add("AllowPush", SqlDbType.TinyInt);
        //                sqlCommand.Parameters.Add("Show", SqlDbType.TinyInt);
        //                sqlCommand.Parameters.Add("Position", SqlDbType.TinyInt);
        //                sqlCommand.Parameters.Add("ParameterTable", SqlDbType.Structured);

        //                sqlCommand.Parameters["Item"].Value = item;
        //                sqlCommand.Parameters["ItemStatus"].Value = itemStatus;
        //                sqlCommand.Parameters["TitelStandardText"].Value = titelStandardText;
        //                sqlCommand.Parameters["TextStandardText"].Value = textStandardText;
        //                sqlCommand.Parameters["DisplayColorAlpha"].Value = color.A;
        //                sqlCommand.Parameters["DisplayColorRed"].Value = color.R;
        //                sqlCommand.Parameters["DisplayColorGreen"].Value = color.G;
        //                sqlCommand.Parameters["DisplayColorBlue"].Value = color.B;
        //                sqlCommand.Parameters["Description"].Value = "Status " + item;
        //                sqlCommand.Parameters["MSToStatusExpires"].Value = millisecondsToExpire;
        //                sqlCommand.Parameters["StandardValue"].Value = 0;
        //                sqlCommand.Parameters["AllowPush"].Value = 1;
        //                sqlCommand.Parameters["Show"].Value = 1;
        //                sqlCommand.Parameters["Position"].Value = 0;
        //                sqlCommand.Parameters["ParameterTable"].Value = parameters;

        //                DataBaseFunctions.NonQuery(sqlCommand);

        //                return true;
        //            }
        //        }
        //    }

        //    return false;
        //}

        /// <summary>
        /// Sets the config value.
        /// </summary>
        /// <param name="userId">
        /// The user Id.
        /// </param>
        /// <param name="item">
        /// The item to update.
        /// </param>
        /// <param name="value">
        /// The value to update.
        /// </param>
        /// <returns>
        /// True if successful, else false.
        /// </returns>
        public static bool SetVisUserConfigValue(int userId, string item, string value)
        {
            if (item == string.Empty)
            {
                return false;
            }

            if (
                DataBaseFunctions.NonQuery(
                    "UPDATE [dbo].[T_VIS_UserConfig] SET [ItemValue] = '" + value + "' WHERE [UserID] = " + userId
                    + " AND [Item] = '" + item + "'") == 0)
            {
                if (
                    DataBaseFunctions.NonQuery(
                        "INSERT INTO [dbo].[T_VIS_UserConfig] ([UserID], [Item], [ItemValue]) VALUES (" + userId + ", '"
                        + item + "', '" + value + "')") == 1)
                {
                    return true;
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Tests the data set.
        /// </summary>
        /// <param name="dataSetToTest">
        /// The data set to test.
        /// </param>
        /// <param name="tableNumber">
        /// The table number.
        /// </param>
        /// <param name="amountOfDataRowsForTrue">
        /// The amount of data rows for true.
        /// </param>
        /// <param name="maxAmountOfDataRowsForTrue">
        /// The max amount of data rows for true.
        /// </param>
        /// <returns>
        /// True if there are in table with index tableNumber of dataset dataSetToTest at least amountOfDataRowsForTrue rows but not more than maxAmountOfDataRowsForTrue rows, else false.
        /// </returns>
        public static bool TestDataSet(
            DataSet dataSetToTest,
            byte tableNumber,
            int amountOfDataRowsForTrue,
            int maxAmountOfDataRowsForTrue)
        {
            return dataSetToTest != null && dataSetToTest.Tables.Count > tableNumber
                   && dataSetToTest.Tables[tableNumber].Rows.Count >= amountOfDataRowsForTrue
                   && dataSetToTest.Tables[tableNumber].Rows.Count <= maxAmountOfDataRowsForTrue;
        }

        /// <summary>
        /// Tests the data set.
        /// </summary>
        /// <param name="dataSetToTest">
        /// The data set to test.
        /// </param>
        /// <param name="amountOfDataRowsForTrue">
        /// The amount of data rows for true.
        /// </param>
        /// <param name="maxAmountOfDataRowsForTrue">
        /// The max amount of data rows for true.
        /// </param>
        /// <returns>
        /// True if there are in table with index 0 of dataset dataSetToTest at least amountOfDataRowsForTrue rows but not more than maxAmountOfDataRowsForTrue rows, else false.
        /// </returns>
        public static bool TestDataSet(
            DataSet dataSetToTest,
            int amountOfDataRowsForTrue,
            int maxAmountOfDataRowsForTrue)
        {
            return dataSetToTest != null && dataSetToTest.Tables.Count > 0
                   && dataSetToTest.Tables[0].Rows.Count >= amountOfDataRowsForTrue
                   && dataSetToTest.Tables[0].Rows.Count <= maxAmountOfDataRowsForTrue;
        }

        /// <summary>
        /// Tests the data set.
        /// </summary>
        /// <param name="dataSetToTest">
        /// The data set to test.
        /// </param>
        /// <param name="tableNumber">
        /// The table number.
        /// </param>
        /// <param name="amountOfDataRowsForTrue">
        /// The amount of data rows for true.
        /// </param>
        /// <returns>
        /// True if there are in table with index tableNumber of dataset dataSetToTest at least amountOfDataRowsForTrue rows, else false.
        /// </returns>
        public static bool TestDataSet(DataSet dataSetToTest, byte tableNumber, int amountOfDataRowsForTrue)
        {
            return dataSetToTest != null && dataSetToTest.Tables.Count > tableNumber
                   && dataSetToTest.Tables[tableNumber].Rows.Count >= amountOfDataRowsForTrue;
        }

        /// <summary>
        /// Tests the data set.
        /// </summary>
        /// <param name="dataSetToTest">
        /// The data set to test.
        /// </param>
        /// <param name="amountOfDataRowsForTrue">
        /// The amount of data rows for true.
        /// </param>
        /// <returns>
        /// True if there are in table with index 0 of dataset dataSetToTest at least amountOfDataRowsForTrue rows, else false.
        /// </returns>
        public static bool TestDataSet(DataSet dataSetToTest, int amountOfDataRowsForTrue)
        {
            return dataSetToTest != null && dataSetToTest.Tables.Count > 0
                   && dataSetToTest.Tables[0].Rows.Count >= amountOfDataRowsForTrue;
        }

        /// <summary>
        /// Versions the specified program name.
        /// </summary>
        /// <param name="programName">
        /// Name of the program.
        /// </param>
        /// <returns>
        /// The version as string.
        /// </returns>
        public static string Version(string programName)
        {
            return GetConfigValue<string>(programName);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.timerReorganizeOrRebuildIndices?.Stop();
            this.timerReorganizeOrRebuildIndices?.Dispose();
        }

        /// <summary>
        /// Called when [reorganize or rebuild indices timer elapsed].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ElapsedEventArgs"/> instance containing the event data.</param>
        private void OnReorganizeOrRebuildIndicesTimerElapsed(object sender, ElapsedEventArgs e)
        {
            this.timerReorganizeOrRebuildIndices.Stop();
            this.timerReorganizeOrRebuildIndices.Interval = 3600000;
            try
            {
                ReOrganizeIndices();
            }
            catch (Exception ex)
            {
                FileOperation.LogError(ex, FileOperation.MaxLogFileSize);
            }

            this.timerReorganizeOrRebuildIndices.Start();
        }

        #endregion
    }
}