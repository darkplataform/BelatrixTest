using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logger
{
    class Program
    {
        static void Main(string[] args)
        {
            //message types allowed
            MessageType[] msgTypesAllowed = new MessageType[] { MessageType.Error, MessageType.Warning, MessageType.Message };
            OutputType[] outputTypesAllowed = new OutputType[] { OutputType.toConsole, OutputType.toFile, OutputType.toDatabase };
            JobLogger logger = new JobLogger(msgTypesAllowed, outputTypesAllowed);

            logger.LogMessage("MESSAGE", MessageType.Message);
            logger.LogMessage("WARNING", MessageType.Warning);
            logger.LogMessage("ERROR", MessageType.Error);

            Console.ReadLine();
        }
    }
}
