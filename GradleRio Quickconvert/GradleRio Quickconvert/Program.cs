using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Diagnostics;

namespace GradleRio_Quickconvert
{
    class Program
    {
        public const string version = "2018.1.1";
        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Welcome to the GradleRio Project Quickconvert tool!");
            Console.WriteLine("Created by MoSadie for/from FRC Team 4450.");
            Console.WriteLine();
            Console.WriteLine("Checking for an internet connection.");

            if (!InternetCheck())
            {
                Console.WriteLine("Unable to connect to the internet.");
                Console.WriteLine("Please try again later. Press any key to exit.");
                Console.ReadKey(true);
                System.Environment.Exit(0);
            }

            Console.WriteLine("Internet connection established.");
            Console.WriteLine("Looking for eclipse robot project.");

            if (!RobotProjectExists())
            {
                Console.WriteLine("Unable to find eclipse-based robot project.");
                Console.WriteLine("Make sure this program is in the root directory of your project.");
                Console.WriteLine("Please try again later. Press any key to exit.");
                Console.ReadKey(true);
                System.Environment.Exit(0);
            }

            Console.WriteLine("Project found!");
            Console.WriteLine("Presetup checks complete!");
            Console.WriteLine();
            Console.WriteLine("What is your team number? (ex. 4450)");

            string teamNumber = Console.ReadLine();

            Console.WriteLine();

            Console.WriteLine("What is the main robot class? (ex. Team4450.Robot11.Robot)");

            string robotClass = Console.ReadLine();

            Console.WriteLine();

            Console.WriteLine("Moving source files");

            Directory.Move("src", "java");
            Directory.CreateDirectory("src/main");
            Directory.Move("java", "src/main/java");

            Console.WriteLine("Source files moved!");

            Console.WriteLine();

            Console.WriteLine("Getting GradleRio setup.");

            if (!GetGradleRioSetup(teamNumber, robotClass))
            {
                Console.WriteLine("Something went wrong getting the quickstart zip.");
                Console.WriteLine("Please try again later. Press any key to exit.");
                Console.ReadKey(true);
                System.Environment.Exit(0);
            }

            Console.WriteLine("GradleRio setup complete!");
            Console.WriteLine("Now cleaning up eclipse project files.");

            if (!CleanEclipseProject())
            {
                Console.WriteLine("Something went wrong cleaning up the eclipse project.");
                Console.WriteLine("Please try again later. Press any key to exit.");
                Console.ReadKey(true);
                System.Environment.Exit(0);
            }

            Console.WriteLine("Cleanup complete!");
            Console.WriteLine("Attempting to build the project.");

            if (!BuildProject())
            {
                Console.WriteLine("Something went wrong attempting to building the project.");
                Console.WriteLine("Please try again later. Press any key to exit.");
                Console.ReadKey(true);
                System.Environment.Exit(0);
            }

            Console.WriteLine("Test build complete! Now creating new eclipse project.");

            if (!CreateEclipseProject())
            {
                Console.WriteLine("Something went wrong creating the new eclipse project.");
                Console.WriteLine("Please try again later. Press any key to exit.");
                Console.ReadKey(true);
                System.Environment.Exit(0);
            }

            Console.WriteLine("Eclipse project created!");

            if (File.Exists(".gitignore"))
            {
                Console.WriteLine("Adding cache folders to .gitignore");
                File.AppendAllLines(".gitignore", new string[] { "", "# Gradle cache", ".gradle", "", "# Eclipse settings", ".settings" });
            }

            Console.WriteLine("Everything's all setup! Cleaning up temporary files and deleting this program.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);

            Directory.Delete("tmp", true);
            try
            {
                Process.Start("cmd.exe", "/c ping localhost -n 3 > nul & del " + System.Reflection.Assembly.GetEntryAssembly().Location);
            }
            catch { /* Do nothing */ }
        }

        static bool InternetCheck()
        {
            try
            {
                HttpWebRequest request = HttpWebRequest.CreateHttp("https://github.com/MoSadie/GradleRioQuickconvert");
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                return (response.StatusCode == HttpStatusCode.OK);
            } catch
            {
                return false;
            }
        }

        static bool RobotProjectExists()
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

        static bool GetGradleRioSetup(string teamNumber, string robotClass)
        {
            try
            {
                Directory.CreateDirectory("tmp");

                using (var client = new WebClient())
                {
                    client.DownloadFile("https://github.com/MoSadie/GradleRioQuickconvert/blob/" + version + "/DownloadPackage.zip?raw=true", "tmp/DownloadPackage.zip");
                }

                ZipFile.ExtractToDirectory("tmp/DownloadPackage.zip","tmp");

                Directory.Move("tmp/DownloadPackage/.vscode", ".vscode");
                Directory.Move("tmp/DownloadPackage/gradle", "gradle");

                foreach (string file in Directory.GetFiles("tmp/DownloadPackage"))
                {
                    string newFile = file.Replace("tmp/DownloadPackage\\", "");
                    File.Move(file, newFile);
                }

                if (File.Exists("build.gradle"))
                {
                    File.Move("build.gradle", "build.gradle.backup");
                }

                using (var stream = File.CreateText("build.gradle"))
                {
                    for (int i = 0; i < Util.buildGradleArray.Length; i++)
                    {
                        if (i == Util.RobotClassIndex)
                        {
                            stream.WriteLine(Util.buildGradleArray[Util.RobotClassIndex] + robotClass + "\"");
                        } else if (i == Util.TeamNumberIndex)
                        {
                            stream.WriteLine(Util.buildGradleArray[Util.TeamNumberIndex] + teamNumber);
                        } else if (i == Util.RobotLibIndex) {
                            if (teamNumber == "4450") stream.WriteLine(Util.buildGradleArray[Util.RobotLibIndex]);
                        } else
                        {
                            stream.WriteLine(Util.buildGradleArray[i]);
                        }
                    }
                }

                return true;

            } catch
            {
                return false;
            }
        }

        static bool CleanEclipseProject()
        {
            try
            {
                if (File.Exists(".project")) File.Delete(".project");
                if (File.Exists(".classpath")) File.Delete(".classpath");
                if (File.Exists("build.xml")) File.Delete("build.xml");
                if (File.Exists("build.properties")) File.Delete("build.properties");

                return true;
            } catch
            {
                return false;
            }
        }

        static bool BuildProject()
        {
            try
            {
                var process = Process.Start("gradlew.bat", "build");
                process.WaitForExit();
                return true;
            } catch
            {
                return false;
            }
        }

        static bool CreateEclipseProject()
        {
            try
            {
                var process = Process.Start("gradlew.bat", "eclipse");
                process.WaitForExit();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
