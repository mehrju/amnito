using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Services.Logging;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Data;

namespace Nop.Plugin.Misc.ShippingSolutions.Services.Interfaces
{
    public class common
    {
        public static void LogException(Exception ex)
        {
            DataSettingsManager p = new DataSettingsManager();
            var dataSettings = p.LoadSettings();
            string connectionString = dataSettings.DataConnectionString;
            var fullMessage = ex?.ToString() ?? string.Empty;

            // define INSERT query with parameters
            string query = $@"INSERT INTO dbo.Log
		                        (
			                        LogLevelId
			                        , ShortMessage
			                        , FullMessage
			                        , IpAddress
			                        , CustomerId
			                        , PageUrl
			                        , ReferrerUrl
			                        , CreatedOnUtc
		                        )
		                        VALUES
		                        (	40 -- LogLevelId - int
			                        , N'{ex.Message}' -- ShortMessage - nvarchar(max)
			                        , N'{fullMessage}' -- FullMessage - nvarchar(max)
			                        , N'127.0.0.1' -- IpAddress - nvarchar(200)
			                        , NULL -- CustomerId - int
			                        , N'خطا های ثبت شده در زمان ارتباط با سرویس های همکار' -- PageUrl - nvarchar(max)
			                        , N'' -- ReferrerUrl - nvarchar(max)
			                        , GETDATE() -- CreatedOnUtc - datetime
			                        )";

            // create connection and command
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public static void Log(string shortMessage, string fullMessage)
        {
            DataSettingsManager p = new DataSettingsManager();
            var dataSettings = p.LoadSettings();
            string connectionString = dataSettings.DataConnectionString;


            // define INSERT query with parameters
            string query = $@"INSERT INTO dbo.Log
		                        (
			                        LogLevelId
			                        , ShortMessage
			                        , FullMessage
			                        , IpAddress
			                        , CustomerId
			                        , PageUrl
			                        , ReferrerUrl
			                        , CreatedOnUtc
		                        )
		                        VALUES
		                        (	40 -- LogLevelId - int
			                        , N'{shortMessage}' -- ShortMessage - nvarchar(max)
			                        , N'{fullMessage}' -- FullMessage - nvarchar(max)
			                        , N'127.0.0.1' -- IpAddress - nvarchar(200)
			                        , NULL -- CustomerId - int
			                        , N'خطا های ثبت شده در زمان ارتباط با سرویس های همکار' -- PageUrl - nvarchar(max)
			                        , N'' -- ReferrerUrl - nvarchar(max)
			                        , GETDATE() -- CreatedOnUtc - datetime
			                        )";

            // create connection and command
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
        public static void InsertOrderNote(string Note, int orderId)
        {
            DataSettingsManager p = new DataSettingsManager();
            var dataSettings = p.LoadSettings();
            string connectionString = dataSettings.DataConnectionString;
            string query = $@"INSERT INTO  dbo.OrderNote
                    (
	                    OrderId
	                    , Note
	                    , DownloadId
	                    , DisplayToCustomer
	                    , CreatedOnUtc
                    )
                    VALUES
                    (	{orderId} -- OrderId - int
	                    , N'{Note}' -- Note - nvarchar(max)
	                    , 0 -- DownloadId - int
	                    , 0 -- DisplayToCustomer - bit
	                    , GETDATE() -- CreatedOnUtc - datetime
                    )";
            using (SqlConnection cn = new SqlConnection(connectionString))
            using (SqlCommand cmd = new SqlCommand(query, cn))
            {
                cn.Open();
                cmd.ExecuteNonQuery();
                cn.Close();
            }
        }
    }
}
