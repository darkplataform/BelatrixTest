using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    public class JobLogger
    {


        public MessageType[] allowedLoggedTypes { get;  set; }
        public OutputType[] requiredOutputTypes { get; set; }
        private string LoggerFile;
        private string connectionString;

        public JobLogger(MessageType[] messageTypes, OutputType[] outputTypes)
        {
            allowedLoggedTypes = messageTypes;
            requiredOutputTypes = outputTypes;
            LoggerFile = string.Format(ConfigurationManager.AppSettings["LoggerFile"],DateTime.Now.ToString("yyyyMMdd"));
            connectionString = ConfigurationManager.ConnectionStrings["logDatabase"].ConnectionString;

        }

        public  void LogMessage(string message, MessageType messageType)
        {
            if (!isAllowed(messageType))
                return;
            
            foreach(var outputType in requiredOutputTypes)
            {
                switch(outputType)
                {
                    case OutputType.toConsole:
                        logConsole(messageFormat(message),messageType);
                        break;
                    case OutputType.toFile:
                        logFile(messageFormat(message));
                        break;
                    case OutputType.toDatabase:
                        logDatabase(message,messageType);
                        break;
                }
            }
            
        }

        public void logFile(string message)
        {
            using (StreamWriter sw = File.AppendText(LoggerFile))
            {
                sw.WriteLine(message);
            }
        }

        public void logConsole(string message, MessageType messageType)
        {
            var messageColor = ConsoleColor.White;

            switch (messageType)
            {
                case MessageType.Warning: messageColor=ConsoleColor.Yellow; break;
                case MessageType.Error: messageColor = ConsoleColor.Red; break;
                default:messageColor = ConsoleColor.White; break;
            }

            Console.ForegroundColor = messageColor;

            Console.WriteLine(message);

            Console.ForegroundColor = ConsoleColor.White;
        }

        public void logDatabase(string message, MessageType messageType)
        {
            using (SqlConnection connection = new SqlConnection(this.connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {
                    connection.Open();

                    int type = (int)messageType;

                    command.CommandText = "insert into log(date, level, message) VALUES(@date, @level, @message)";
                    command.Parameters.AddWithValue("date", DateTime.Now);
                    command.Parameters.AddWithValue("level", type);
                    command.Parameters.AddWithValue("message", message);

                    command.ExecuteNonQuery();
                }
            }
        }

        public string messageFormat(string message)
        {
            return string.Format("{0} : {1}", DateTime.Now, message);
        }

        

        public bool isAllowed(MessageType type)
        {
            return allowedLoggedTypes.Contains(type);
        }

       /* public static void LogMessage(string message, MessageType messageType)
        {
            message.Trim();
            if (message == null || message.Length == 0)
            {
                return;
            }
            if (!_logToConsole && !_logToFile && !LogToDatabase)
            {
                throw new Exception("Invalid configuration");
            }
            if ((!_logError && !_logMessage && !_logWarning) || (!message && !warning
            && !error))
            {
                throw new Exception("Error or Warning or Message must be specified");
            }
            System.Data.SqlClient.SqlConnection connection = new
            System.Data.SqlClient.SqlConnection(System.Configuration.ConfigurationManager.AppS
            ettings["ConnectionString"]);
            connection.Open();
            int t;
            if (message && _logMessage)
            {
                t = 1;
            }
            if (error && _logError)
            {
                t = 2;
            }
            if (warning && _logWarning)
            {
                t = 3;
            }
            System.Data.SqlClient.SqlCommand command = new
            System.Data.SqlClient.SqlCommand("Insert into Log Values('" + message + "', " +
            t.ToString() + ")");
            command.ExecuteNonQuery();
            string l;
            if
            (!System.IO.File.Exists(System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt"))
            {
                l =
                System.IO.File.ReadAllText(System.Configuration.ConfigurationManager.AppSettings["LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt");
            }
            if (error && _logError)
            {
                l = l + DateTime.Now.ToShortDateString() + message;
            }
            if (warning && _logWarning)
            {
                l = l + DateTime.Now.ToShortDateString() + message;
            }
            if (message && _logMessage)
            {
                l = l + DateTime.Now.ToShortDateString() + message;
            }

            System.IO.File.WriteAllText(System.Configuration.ConfigurationManager.AppSettings[
            "LogFileDirectory"] + "LogFile" + DateTime.Now.ToShortDateString() + ".txt", l);
            if (error && _logError)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if (warning && _logWarning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            if (message && _logMessage)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine(DateTime.Now.ToShortDateString() + message);
        }*/
    }
}
