using System;
using System.IO;
using System.Configuration;
using System.Linq;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplicationFileSync
{
    class Program
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public string sourcePath = "";
        public string destinationPath = "";
        public string fileName = "";
        public string destFile = "";
        const string ServiceBusConnectionString = "Endpoint=sb://servicebusqu.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=oGyl4NRX/0+u16SBaaHJBMMSrQXL0lwIi34I1xXZRhE=";
        const string QueueName = "firstqueue";


        public static void Main(string[] args)
        {
            try
            {

                Program c = new Program();

                if (args.Contains("nave1"))
                {
                    //Message for Log, do with LogMethod
                    log.Info("This is log file");
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();
                    
                    c.ConfigurationSourceDestinationPathAsync("sourcePath1", "destinationPath1");
                }
                else if (args.Contains("nave2"))
                {
                    //Message for Log, do with LogMethod
                    log.Info("This is log file");
                    Console.WriteLine("Press enter to continue");
                    Console.ReadLine();

                    c.ConfigurationSourceDestinationPathAsync("sourcePath2", "destinationPath2");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
            }
            

        }

        //method to seth sourcePath where take files and destinationPath where sent files
        public void ConfigurationSourceDestinationPathAsync(string source, string destination)
        {
             sourcePath = ConfigurationManager.AppSettings[source];
             destinationPath = ConfigurationManager.AppSettings[destination];

            //if destination directory not exist, create it
            if (!System.IO.Directory.Exists(destinationPath))
            {
                System.IO.Directory.CreateDirectory(destinationPath);
            }


            // To copy all the files in one directory to another directory:
            try
            {
                log.Debug("Source path: " + sourcePath);
                if (System.IO.Directory.Exists(sourcePath))
                {
                    string[] files = System.IO.Directory.GetFiles(sourcePath);
                    

                    // Copy the files and overwrite destination files if they already exist.
                    foreach (string f in files)
                    {
                        // Use static Path methods to extract only the file name from the path.
                        fileName = System.IO.Path.GetFileName(f);
                        destFile = System.IO.Path.Combine(destinationPath, fileName);
                        System.IO.File.Copy(f, destFile, true);
                        Console.WriteLine(fileName);
                        //send messages with name of files
                        SendMessagesAsync(fileName).GetAwaiter().GetResult();
 
                    }

                }
                else
                {
                    Console.WriteLine("Source path does not exist!");
                    Console.ReadKey();
                }

                Console.WriteLine("All files copied successfully on directory: " + destinationPath + ". Press any key to exit");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine($"{DateTime.Now} :: Exception: {e.Message}");
                Console.ReadKey();
            }
        }

        static async Task SendMessagesAsync(/*int numberOfMessagesToSend*/ string execution)
        {
            IMessageSender messageSender = new MessageSender(ServiceBusConnectionString, QueueName);
            try
            {
                    // Create a new message to send to the queue
                    string messageBody = execution;
                    var message = new Message(Encoding.UTF8.GetBytes(messageBody));
                    message.SessionId = "Test1";
                    // Write the body of the message to the console
                    Console.WriteLine($"Sending message: {messageBody}");

                    // Send the message to the queue
                    await messageSender.SendAsync(message);
                    messageSender.CloseAsync().GetAwaiter();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"{DateTime.Now} :: Exception: {exception.Message}");
            }
        }
    }
}
