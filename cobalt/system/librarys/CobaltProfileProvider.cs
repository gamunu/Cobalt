using System.Web.Profile;
using System.Configuration.Provider;
using System.Collections.Specialized;
using System;
using System.Data;
using System.Data.Odbc;
using System.Configuration;
using System.Diagnostics;
using System.Web;
using System.Collections;
using System.Data.SqlClient;

/*

CREATE TABLE Profiles
(
  UniqueID Int NOT NULL PRIMARY KEY IDENTITY(1,1),
  Username nvarchar (255) NOT NULL,
  ApplicationName nvarchar (255) NOT NULL,
  IsAnonymous Bit, 
  LastActivityDate DateTime,
  LastUpdatedDate DateTime,
    CONSTRAINT PKProfiles UNIQUE (Username, ApplicationName)
)

CREATE TABLE StockSymbols
(
  UniqueID Int,
  StockSymbol Nvarchar (10),
    CONSTRAINT FKProfiles1 FOREIGN KEY (UniqueID)
      REFERENCES Profiles
)

CREATE TABLE ProfileData
(
  UniqueID Int,
  ZipCode nvarchar (10),
    CONSTRAINT FKProfiles2 FOREIGN KEY (UniqueID)
      REFERENCES Profiles
)

*/


namespace cobalt.system.librarys
{

    public sealed class CobaltProfileProvider : ProfileProvider
    {
        //
        // Global connection string, generic exception message, event log info.
        //

        private string eventSource = "CobaltProfileProvider";
        private string eventLog = "Application";
        private string exceptionMessage = "An exception occurred. Please check the event log.";
        private string connectionString;


        //
        // If false, exceptions are thrown to the caller. If true,
        // exceptions are written to the event log.
        //

        private bool pWriteExceptionsToEventLog;

        public bool WriteExceptionsToEventLog
        {
            get { return pWriteExceptionsToEventLog; }
            set { pWriteExceptionsToEventLog = value; }
        }



        //
        // System.Configuration.Provider.ProviderBase.Initialize Method
        //

        public override void Initialize(string name, NameValueCollection config)
        {

            //
            // Initialize values from web.config.
            //

            if (config == null)
                throw new ArgumentNullException("config");

            if (name == null || name.Length == 0)
                name = "CobaltProfileProvider";

            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Cobalt Profile provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);


            if (config["applicationName"] == null || config["applicationName"].Trim() == "")
            {
                pApplicationName = System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath;
            }
            else
            {
                pApplicationName = config["applicationName"];
            }


            //
            // Initialize connection string.
            //

            ConnectionStringSettings pConnectionStringSettings = ConfigurationManager.
                ConnectionStrings[config["connectionStringName"]];

            if (pConnectionStringSettings == null ||
                pConnectionStringSettings.ConnectionString.Trim() == "")
            {
                throw new ProviderException("Connection string cannot be blank.");
            }

            connectionString = pConnectionStringSettings.ConnectionString;
        }


        //
        // System.Configuration.SettingsProvider.ApplicationName
        //

        private string pApplicationName;

        public override string ApplicationName
        {
            get { return pApplicationName; }
            set { pApplicationName = value; }
        }



        //
        // System.Configuration.SettingsProvider methods.
        //

        //
        // SettingsProvider.GetPropertyValues
        //

        public override SettingsPropertyValueCollection
              GetPropertyValues(SettingsContext context,
                    SettingsPropertyCollection ppc)
        {
            string username = (string)context["UserName"];
            bool isAuthenticated = (bool)context["IsAuthenticated"];

            // The serializeAs attribute is ignored in this provider implementation.

            SettingsPropertyValueCollection svc =
                new SettingsPropertyValueCollection();

            foreach (SettingsProperty prop in ppc)
            {
                SettingsPropertyValue pv = new SettingsPropertyValue(prop);

                switch (prop.Name)
                {
                    case "StockSymbols":
                        pv.PropertyValue = GetStockSymbols(username, isAuthenticated);
                        break;
                    case "ZipCode":
                        pv.PropertyValue = GetZipCode(username, isAuthenticated);
                        break;
                    default:
                        throw new ProviderException("Unsupported property.");
                }

                svc.Add(pv);
            }

            UpdateActivityDates(username, isAuthenticated, true);

            return svc;
        }



        //
        // SettingsProvider.SetPropertyValues
        //

        public override void SetPropertyValues(SettingsContext context,
                       SettingsPropertyValueCollection ppvc)
        {
            // The serializeAs attribute is ignored in this provider implementation.

            string username = (string)context["UserName"];
            bool isAuthenticated = (bool)context["IsAuthenticated"];
            int uniqueID = GetUniqueID(username, isAuthenticated, false);
            if (uniqueID == 0)
                uniqueID = CreateProfileForUser(username, isAuthenticated);

            foreach (SettingsPropertyValue pv in ppvc)
            {
                switch (pv.Property.Name)
                {
                    case "StockSymbols":
                        SetStockSymbols(uniqueID, (ArrayList)pv.PropertyValue);
                        break;
                    case "ZipCode":
                        SetZipCode(uniqueID, (string)pv.PropertyValue);
                        break;
                    default:
                        throw new ProviderException("Unsupported property.");
                }
            }

            UpdateActivityDates(username, isAuthenticated, false);
        }


        //
        // UpdateActivityDates
        // Updates the LastActivityDate and LastUpdatedDate values 
        // when profile properties are accessed by the
        // GetPropertyValues and SetPropertyValues methods. 
        // Passing true as the activityOnly parameter will update
        // only the LastActivityDate.
        //

        private void UpdateActivityDates(string username, bool isAuthenticated, bool activityOnly)
        {
            DateTime activityDate = DateTime.Now;


            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;

            if (activityOnly)
            {
                cmd.CommandText = "UPDATE Profiles Set LastActivityDate = @LastActivityDate " +
                    "WHERE Username = @Username AND ApplicationName = @ApplicationName AND IsAnonymous = @IsAnonymous";
                cmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = activityDate;
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
                cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = ApplicationName;
                cmd.Parameters.Add("@IsAnonymous", SqlDbType.Bit).Value = !isAuthenticated;

            }
            else
            {
                cmd.CommandText = "UPDATE Profiles Set LastActivityDate = @LastActivityDate, LastUpdatedDate = @LastUpdatedDate " +
                      "WHERE Username = @Username AND ApplicationName = @ApplicationName AND IsAnonymous = @IsAnonymous";
                cmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = activityDate;
                cmd.Parameters.Add("@LastUpdatedDate", SqlDbType.DateTime).Value = activityDate;
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
                cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = ApplicationName;
                cmd.Parameters.Add("@IsAnonymous", SqlDbType.Bit).Value = !isAuthenticated;
            }

            try
            {
                conn.Open();

                cmd.ExecuteNonQuery();
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "UpdateActivityDates");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
        }


        //
        // GetStockSymbols
        //   Retrieves stock symbols from the database during the call to GetPropertyValues.
        //

        private ArrayList GetStockSymbols(string username, bool isAuthenticated)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new
              SqlCommand("SELECT StockSymbol FROM Profiles " +
                "INNER JOIN StockSymbols ON Profiles.UniqueID = StockSymbols.UniqueID " +
                "WHERE Username = @Username AND ApplicationName = @ApplicationName And IsAnonymous = @IsAnonymous", conn);
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = ApplicationName;
            cmd.Parameters.Add("@IsAnonymous", SqlDbType.Bit).Value = !isAuthenticated;

            ArrayList outList = new ArrayList();

            SqlDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    outList.Add(reader.GetString(0));
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetStockSymbols");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }

                conn.Close();
            }

            return outList;
        }



        //
        // SetStockSymbols
        // Inserts stock symbol values into the database during 
        // the call to SetPropertyValues.
        //

        private void SetStockSymbols(int uniqueID, ArrayList stocks)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("DELETE FROM StockSymbols WHERE UniqueID = @UniqueID", conn);
            cmd.Parameters.Add("@UniqueID", SqlDbType.Int).Value = uniqueID;

            SqlCommand cmd2 = new SqlCommand("INSERT INTO StockSymbols (UniqueID, StockSymbol) " +
                       "Values(@UniqueID, @StockSymbol)", conn);
            cmd2.Parameters.Add("@UniqueID", SqlDbType.Int).Value = uniqueID;
            cmd2.Parameters.Add("@StockSymbol", SqlDbType.NVarChar, 10);

            SqlTransaction tran = null;

            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                cmd.Transaction = tran;
                cmd2.Transaction = tran;

                // Delete any existing values;
                cmd.ExecuteNonQuery();
                foreach (object o in stocks)
                {
                    cmd2.Parameters["@StockSymbol"].Value = o.ToString();
                    cmd2.ExecuteNonQuery();
                }

                tran.Commit();
            }
            catch (SqlException e)
            {
                try
                {
                    tran.Rollback();
                }
                catch
                {
                }

                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "SetStockSymbols");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
        }

        //
        // GetZipCode
        // Retrieves ZipCode value from the database during 
        // the call to GetPropertyValues.
        //

        private string GetZipCode(string username, bool isAuthenticated)
        {
            string zipCode = "";

            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT ZipCode FROM Profiles " +
                  "INNER JOIN ProfileData ON Profiles.UniqueID = ProfileData.UniqueID " +
                  "WHERE Username = @Username AND ApplicationName = @ApplicationName And IsAnonymous = @IsAnonymous", conn);
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = ApplicationName;
            cmd.Parameters.Add("@IsAnonymous", SqlDbType.Bit).Value = !isAuthenticated;

            try
            {
                conn.Open();

                zipCode = (string)cmd.ExecuteScalar();
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetZipCode");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }

            return zipCode;
        }

        //
        // SetZipCode
        // Inserts the zip code value into the database during 
        // the call to SetPropertyValues.
        //

        private void SetZipCode(int uniqueID, string zipCode)
        {
            if (zipCode == null) { zipCode = String.Empty; }

            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("DELETE FROM ProfileData WHERE UniqueID = @UniqueID", conn);
            cmd.Parameters.Add("@UniqueID", SqlDbType.Int).Value = uniqueID;

            SqlCommand cmd2 = new SqlCommand("INSERT INTO ProfileData (UniqueID, ZipCode) " +
                       "Values(@UniqueID, @ZipCode)", conn);
            cmd2.Parameters.Add("@UniqueID", SqlDbType.Int).Value = uniqueID;
            cmd2.Parameters.Add("@ZipCode", SqlDbType.NVarChar, 10).Value = zipCode;

            SqlTransaction tran = null;

            try
            {
                conn.Open();
                tran = conn.BeginTransaction();
                cmd.Transaction = tran;
                cmd2.Transaction = tran;

                // Delete any existing values.
                cmd.ExecuteNonQuery();
                cmd2.ExecuteNonQuery();

                tran.Commit();
            }
            catch (SqlException e)
            {
                try
                {
                    tran.Rollback();
                }
                catch
                {
                }

                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "SetZipCode");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }
        }


        //
        // GetUniqueID
        //   Retrieves the uniqueID from the database for the current user and application.
        //

        private int GetUniqueID(string username, bool isAuthenticated, bool ignoreAuthenticationType)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT UniqueID FROM Profiles " +
                    "WHERE Username = @Username AND ApplicationName = @ApplicationName", conn);
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = ApplicationName;

            if (!ignoreAuthenticationType)
            {
                cmd.CommandText += " AND IsAnonymous = @IsAnonymous";
                cmd.Parameters.Add("@IsAnonymous", SqlDbType.Bit).Value = !isAuthenticated;
            }

            int uniqueID = 0;
            SqlDataReader reader = null;

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader(CommandBehavior.SingleRow);
                if (reader.HasRows)
                    uniqueID = reader.GetInt32(0);
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetUniqueID");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }

            return uniqueID;
        }


        //
        // CreateProfileForUser
        // If no user currently exists in the database, 
        // a user record is created during
        // the call to the GetUniqueID private method.
        //

        private int CreateProfileForUser(string username, bool isAuthenticated)
        {
            // Check for valid user name.

            if (username == null)
                throw new ArgumentNullException("User name cannot be null.");
            if (username.Length > 255)
                throw new ArgumentException("User name exceeds 255 characters.");
            if (username.Contains(","))
                throw new ArgumentException("User name cannot contain a comma (,).");


            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("INSERT INTO Profiles (Username, " +
                    "ApplicationName, LastActivityDate, LastUpdatedDate, " +
                    "IsAnonymous) Values(@Username, @ApplicationName, @LastActivityDate, @LastUpdatedDate, @IsAnonymous)", conn);
            cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = ApplicationName;
            cmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = DateTime.Now;
            cmd.Parameters.Add("@LastUpdatedDate", SqlDbType.NVarChar).Value = DateTime.Now;
            cmd.Parameters.Add("@IsAnonymous", SqlDbType.Bit).Value = !isAuthenticated;

            SqlCommand cmd2 = new SqlCommand("SELECT @@IDENTITY", conn);

            int uniqueID = 0;

            try
            {
                conn.Open();

                cmd.ExecuteNonQuery();

                uniqueID = (int)cmd2.ExecuteScalar();
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "CreateProfileForUser");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }

            return uniqueID;
        }


        //
        // ProfileProvider.DeleteProfiles(ProfileInfoCollection)
        //

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            int deleteCount = 0;

            SqlConnection conn = new SqlConnection(connectionString);
            SqlTransaction tran = null;

            try
            {
                conn.Open();
                tran = conn.BeginTransaction();

                foreach (ProfileInfo p in profiles)
                {
                    if (DeleteProfile(p.UserName, conn, tran))
                        deleteCount++;
                }

                tran.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    tran.Rollback();
                }
                catch
                {
                }

                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteProfiles(ProfileInfoCollection)");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }

            return deleteCount;
        }


        //
        // ProfileProvider.DeleteProfiles(string[])
        //

        public override int DeleteProfiles(string[] usernames)
        {
            int deleteCount = 0;

            SqlConnection conn = new SqlConnection(connectionString);
            SqlTransaction tran = null;

            try
            {
                conn.Open();
                tran = conn.BeginTransaction();

                foreach (string user in usernames)
                {
                    if (DeleteProfile(user, conn, tran))
                        deleteCount++;
                }

                tran.Commit();
            }
            catch (Exception e)
            {
                try
                {
                    tran.Rollback();
                }
                catch
                {
                }

                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteProfiles(String())");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                conn.Close();
            }

            return deleteCount;
        }



        //
        // ProfileProvider.DeleteInactiveProfiles
        //

        public override int DeleteInactiveProfiles(
          ProfileAuthenticationOption authenticationOption,
          DateTime userInactiveSinceDate)
        {
            SqlConnection conn = new SqlConnection(connectionString);
            SqlCommand cmd = new SqlCommand("SELECT Username FROM Profiles " +
                    "WHERE ApplicationName = @ApplicationName AND " +
                    " LastActivityDate <= @LastActivityDate", conn);
            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = ApplicationName;
            cmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = userInactiveSinceDate;

            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    cmd.CommandText += " AND IsAnonymous = @IsAnonymous";
                    cmd.Parameters.Add("@IsAnonymous", SqlDbType.Bit).Value = true;
                    break;
                case ProfileAuthenticationOption.Authenticated:
                    cmd.CommandText += " AND IsAnonymous = @IsAnonymous";
                    cmd.Parameters.Add("@IsAnonymous", SqlDbType.Bit).Value = false;
                    break;
                default:
                    break;
            }

            SqlDataReader reader = null;
            string usernames = "";

            try
            {
                conn.Open();

                reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    usernames += reader.GetString(0) + ",";
                }
            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "DeleteInactiveProfiles");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }

                conn.Close();
            }

            if (usernames.Length > 0)
            {
                // Remove trailing comma.
                usernames = usernames.Substring(0, usernames.Length - 1);
            }


            // Delete profiles.

            return DeleteProfiles(usernames.Split(','));
        }


        //
        // DeleteProfile
        // Deletes profile data from the database for the 
        // specified user name.
        //

        private bool DeleteProfile(string username, SqlConnection conn, SqlTransaction tran)
        {
            // Check for valid user name.
            if (username == null)
                throw new ArgumentNullException("User name cannot be null.");
            if (username.Length > 255)
                throw new ArgumentException("User name exceeds 255 characters.");
            if (username.Contains(","))
                throw new ArgumentException("User name cannot contain a comma (,).");


            int uniqueID = GetUniqueID(username, false, true);

            SqlCommand cmd1 = new SqlCommand("DELETE * FROM ProfileData WHERE UniqueID = @UniqueID", conn);
            cmd1.Parameters.Add("@UniqueID", SqlDbType.Int).Value = uniqueID;
            SqlCommand cmd2 = new SqlCommand("DELETE * FROM StockSymbols WHERE UniqueID = @UniqueID", conn);
            cmd2.Parameters.Add("@UniqueID", SqlDbType.Int).Value = uniqueID;
            SqlCommand cmd3 = new SqlCommand("DELETE * FROM Profiles WHERE UniqueID = @UniqueID", conn);
            cmd3.Parameters.Add("@UniqueID", SqlDbType.Int).Value = uniqueID;

            cmd1.Transaction = tran;
            cmd2.Transaction = tran;
            cmd3.Transaction = tran;

            int numDeleted = 0;

            // Exceptions will be caught by the calling method.
            numDeleted += cmd1.ExecuteNonQuery();
            numDeleted += cmd2.ExecuteNonQuery();
            numDeleted += cmd3.ExecuteNonQuery();

            if (numDeleted == 0)
                return false;
            else
                return true;
        }


        //
        // ProfileProvider.FindProfilesByUserName
        //

        public override ProfileInfoCollection FindProfilesByUserName(
          ProfileAuthenticationOption authenticationOption,
          string usernameToMatch,
          int pageIndex,
          int pageSize,
          out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            return GetProfileInfo(authenticationOption, usernameToMatch,
                null, pageIndex, pageSize, out totalRecords);
        }


        //
        // ProfileProvider.FindInactiveProfilesByUserName
        //

        public override ProfileInfoCollection FindInactiveProfilesByUserName(
          ProfileAuthenticationOption authenticationOption,
          string usernameToMatch,
          DateTime userInactiveSinceDate,
          int pageIndex,
          int pageSize,
          out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            return GetProfileInfo(authenticationOption, usernameToMatch, userInactiveSinceDate,
                  pageIndex, pageSize, out totalRecords);
        }


        //
        // ProfileProvider.GetAllProfiles
        //

        public override ProfileInfoCollection GetAllProfiles(
          ProfileAuthenticationOption authenticationOption,
          int pageIndex,
          int pageSize,
          out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            return GetProfileInfo(authenticationOption, null, null,
                  pageIndex, pageSize, out totalRecords);
        }


        //
        // ProfileProvider.GetAllInactiveProfiles
        //

        public override ProfileInfoCollection GetAllInactiveProfiles(
          ProfileAuthenticationOption authenticationOption,
          DateTime userInactiveSinceDate,
          int pageIndex,
          int pageSize,
          out int totalRecords)
        {
            CheckParameters(pageIndex, pageSize);

            return GetProfileInfo(authenticationOption, null, userInactiveSinceDate,
                  pageIndex, pageSize, out totalRecords);
        }



        //
        // ProfileProvider.GetNumberOfInactiveProfiles
        //

        public override int GetNumberOfInactiveProfiles(
          ProfileAuthenticationOption authenticationOption,
          DateTime userInactiveSinceDate)
        {
            int inactiveProfiles = 0;

            ProfileInfoCollection profiles =
              GetProfileInfo(authenticationOption, null, userInactiveSinceDate,
                  0, 0, out inactiveProfiles);

            return inactiveProfiles;
        }



        //
        // CheckParameters
        // Verifies input parameters for page size and page index. 
        // Called by GetAllProfiles, GetAllInactiveProfiles, 
        // FindProfilesByUserName, and FindInactiveProfilesByUserName.
        //

        private void CheckParameters(int pageIndex, int pageSize)
        {
            if (pageIndex < 0)
                throw new ArgumentException("Page index must 0 or greater.");
            if (pageSize < 1)
                throw new ArgumentException("Page size must be greater than 0.");
        }


        //
        // GetProfileInfo
        // Retrieves a count of profiles and creates a 
        // ProfileInfoCollection from the profile data in the 
        // database. Called by GetAllProfiles, GetAllInactiveProfiles,
        // FindProfilesByUserName, FindInactiveProfilesByUserName, 
        // and GetNumberOfInactiveProfiles.
        // Specifying a pageIndex of 0 retrieves a count of the results only.
        //

        private ProfileInfoCollection GetProfileInfo(
          ProfileAuthenticationOption authenticationOption,
          string usernameToMatch,
          object userInactiveSinceDate,
          int pageIndex,
          int pageSize,
          out int totalRecords)
        {
            SqlConnection conn = new SqlConnection(connectionString);


            // Command to retrieve the total count.

            SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM Profiles WHERE ApplicationName = ? ", conn);
            cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = ApplicationName;


            // Command to retrieve the profile data.

            SqlCommand cmd2 = new SqlCommand("SELECT Username, LastActivityDate, LastUpdatedDate, " +
                    "IsAnonymous FROM Profiles WHERE ApplicationName = @ApplicationName ", conn);
            cmd2.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = ApplicationName;


            // If searching for a user name to match, add the command text and parameters.

            if (usernameToMatch != null)
            {
                cmd.CommandText += " AND Username LIKE @Username ";
                cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = usernameToMatch;

                cmd2.CommandText += " AND Username LIKE @Username ";
                cmd2.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = usernameToMatch;
            }


            // If searching for inactive profiles, 
            // add the command text and parameters.

            if (userInactiveSinceDate != null)
            {
                cmd.CommandText += " AND LastActivityDate <= @LastActivityDate ";
                cmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = (DateTime)userInactiveSinceDate;

                cmd2.CommandText += " AND LastActivityDate <= @LastActivityDate ";
                cmd2.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = (DateTime)userInactiveSinceDate;
            }


            // If searching for a anonymous or authenticated profiles,    
            // add the command text and parameters.

            switch (authenticationOption)
            {
                case ProfileAuthenticationOption.Anonymous:
                    cmd.CommandText += " AND IsAnonymous = @IsAnonymous";
                    cmd.Parameters.Add("@IsAnonymous", SqlDbType.Bit).Value = true;
                    cmd2.CommandText += " AND IsAnonymous = @IsAnonymous";
                    cmd2.Parameters.Add("@IsAnonymous", SqlDbType.Bit).Value = true;
                    break;
                case ProfileAuthenticationOption.Authenticated:
                    cmd.CommandText += " AND IsAnonymous = @IsAnonymous";
                    cmd.Parameters.Add("@IsAnonymous", SqlDbType.Bit).Value = false;
                    cmd2.CommandText += " AND IsAnonymous = @IsAnonymous";
                    cmd2.Parameters.Add("@IsAnonymous", SqlDbType.Bit).Value = false;
                    break;
                default:
                    break;
            }


            // Get the data.

            SqlDataReader reader = null;
            ProfileInfoCollection profiles = new ProfileInfoCollection();

            try
            {
                conn.Open();
                // Get the profile count.
                totalRecords = (int)cmd.ExecuteScalar();
                // No profiles found.
                if (totalRecords <= 0) { return profiles; }
                // Count profiles only.
                if (pageSize == 0) { return profiles; }

                reader = cmd2.ExecuteReader();

                int counter = 0;
                int startIndex = pageSize * (pageIndex - 1);
                int endIndex = startIndex + pageSize - 1;

                while (reader.Read())
                {
                    if (counter >= startIndex)
                    {
                        ProfileInfo p = GetProfileInfoFromReader(reader);
                        profiles.Add(p);
                    }

                    if (counter >= endIndex)
                    {
                        cmd.Cancel();
                        break;
                    }

                    counter++;
                }

            }
            catch (SqlException e)
            {
                if (WriteExceptionsToEventLog)
                {
                    WriteToEventLog(e, "GetProfileInfo");
                    throw new ProviderException(exceptionMessage);
                }
                else
                {
                    throw e;
                }
            }
            finally
            {
                if (reader != null) { reader.Close(); }

                conn.Close();
            }

            return profiles;
        }

        //
        // GetProfileInfoFromReader
        //  Takes the current row from the SqlDataReader
        // and populates a ProfileInfo object from the values. 
        //

        private ProfileInfo GetProfileInfoFromReader(SqlDataReader reader)
        {
            string username = reader.GetString(0);

            DateTime lastActivityDate = new DateTime();
            if (reader.GetValue(1) != DBNull.Value)
                lastActivityDate = reader.GetDateTime(1);

            DateTime lastUpdatedDate = new DateTime();
            if (reader.GetValue(2) != DBNull.Value)
                lastUpdatedDate = reader.GetDateTime(2);

            bool isAnonymous = reader.GetBoolean(3);

            // ProfileInfo.Size not currently implemented.
            ProfileInfo p = new ProfileInfo(username,
                isAnonymous, lastActivityDate, lastUpdatedDate, 0);

            return p;
        }


        //
        // WriteToEventLog
        // A helper function that writes exception detail to the event 
        // log. Exceptions are written to the event log as a security 
        // measure to prevent private database details from being 
        // returned to the browser. If a method does not return a 
        // status or Boolean value indicating whether the action succeeded 
        // or failed, the caller also throws a generic exception.
        //

        private void WriteToEventLog(Exception e, string action)
        {
            EventLog log = new EventLog();
            log.Source = eventSource;
            log.Log = eventLog;

            string message = "An exception occurred while communicating with the data source.\n\n";
            message += "Action: " + action + "\n\n";
            message += "Exception: " + e.ToString();

            log.WriteEntry(message);
        }
    }
}
