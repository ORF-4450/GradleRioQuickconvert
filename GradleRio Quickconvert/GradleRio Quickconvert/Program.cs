using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.IO.Compression;

namespace GradleRio_Quickconvert
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the GradleRio Project Quickconvert tool!");
            Console.WriteLine("Created by MoSadie for/from FRC Team 4450.");

            Console.WriteLine();
            Console.WriteLine("Checking for an internet connection.");

            if (!internetCheck())
            {
                Console.WriteLine("Unable to connect to the internet.");
                Console.WriteLine("Please try again later. Press any key to exit.");
                Console.ReadKey(true);
                System.Environment.Exit(0);
            }

            Console.WriteLine("Internet connection established.");
            Console.WriteLine("Looking for eclipse robot project.");

            if (!robotProjectExists())
            {
                Console.WriteLine("Unable to find eclipse-based robot project.");
                Console.WriteLine("Make sure this program is in the root directory of your project.");
                Console.WriteLine("Please try again later. Press any key to exit.");
                Console.ReadKey(true);
                System.Environment.Exit(0);
            }

            Console.WriteLine("Project found!");
            Console.WriteLine();
            Console.WriteLine("Presetup complete!");
            Console.WriteLine("What is the main package of your robot code? (ex. Team4450.Robot11)");

            string robotPackage = Console.ReadLine();

            Console.WriteLine();

            Console.WriteLine("What is the main robot class? (ex. Robot)");

            string robotClass = Console.ReadLine();

            Console.WriteLine();

            DirectoryInfo projectRoot = new DirectoryInfo(System.AppDomain.CurrentDomain.BaseDirectory);

            Console.WriteLine("Getting GradleRio Quickstart for Java.");

            if (!getGradleRioQuickstart())
            {
                Console.WriteLine("Something went wrong getting the quickstart zip.");
                Console.WriteLine("Please try again later. Press any key to exit.");
                Console.ReadKey(true);
                System.Environment.Exit(0);
            }

            Console.ReadKey(true);
        }

        static bool internetCheck()
        {
            try
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp("https://github.com/MoSadie/"); //TODO UPDATE THIS
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return (response.StatusCode == HttpStatusCode.OK);
            } catch (WebException e)
            {
                return false;
            }
        }

        static bool robotProjectExists()
        {
            if (!Directory.Exists(System.AppDomain.CurrentDomain.BaseDirectory))
            {
                return false;
            }

            string pathToRoot = System.AppDomain.CurrentDomain.BaseDirectory;

            bool result = true;

            string[] dirsToCheck = new string[] { pathToRoot + "/src" };

            for (int i = 0; i < dirsToCheck.Length && result; i++)
            {
                if (!Directory.Exists(dirsToCheck[i])) result = false;
            }

            string[] filesToCheck = new string[] { ".classpath", ".project" };

            for (int i = 0; i < filesToCheck.Length && result; i++)
            {
                if (!File.Exists(filesToCheck[i])) result = false;
            }

            return result;
        }

        static bool getGradleRioQuickstart()
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadFile("https://github.com/Open-RIO/GradleRIO/blob/master/Quickstart.zip?raw=true", "tmp/Quickstart.zip");
                }

                using (var zip = new ZipArchive("tmp/Quickstart.zip"))
                {

                }
            } catch
            {
                return false;
            }
        }
    }
}
