using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using System.IO;
using EncodedOS.System;
using EncodedOS.Tools;

namespace EncodedOS
{
    public class Kernel: Sys.Kernel
    {
        //Just make it Global
        Sys.FileSystem.CosmosVFS fs = new Sys.FileSystem.CosmosVFS();

        protected override void BeforeRun()
        {
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            Console.Clear();

            //Filesystem.SetToBegin();

            //Check for settings
            if (File.Exists(Variables.settingsFile) == false)
            {
                //Create Settings File
                File.Create(Variables.settingsFile);
                File.WriteAllText(Variables.settingsFile, "first_start=true");
            }

            //Check if the Filesystem includes everything
            if (Filesystem.CheckFileSystem() == false)
            {
                if (File.Exists(Variables.settingsFile) == true)
                {
                    string[] sLines = File.ReadAllLines(Variables.settingsFile);
                    if (sLines[0].ToString() == "first_start=true")
                    {
                        //First OS start, let's create our Filesystem
                        Console.WriteLine("Creating Filesystem, this could take a moment....");
                        Filesystem.CreateFileSystem();
                    }
                    else
                    {
                        Console.WriteLine("Error while checking the Filesystem!");
                        Sys.Global.mFileSystemDebugger.Break();
                    }
                }
            }

            //Loading Filesystem was successfully
            Console.WriteLine("Successfully loaded important files!");

            //User Login
            bool foundUserName = false;
            bool foundPassword = false;

            string userName = "";
            string userPassword = "";

            try
            {
                while (foundUserName == false)
                {
                    Console.Write(Environment.NewLine + Environment.NewLine + Environment.NewLine + "Username: ");
                    userName = Console.ReadLine();

                    if (userName != "")
                    {
                        string[] userLines = File.ReadAllLines(Variables.usersFile);
                        for (int i = 0; i < userLines.Length && foundUserName == false; i++)
                        {
                            if (userLines[i].Split('=')[0].ToString() == userName)
                            {
                                foundUserName = true;
                            }
                        }
                    }
                    if (foundUserName == false) Console.WriteLine("This user does not exists!");
                }

                while (foundPassword == false)
                {
                    Console.Write("Password: ");
                    userPassword = Console.ReadLine();

                    if (userPassword != "")
                    {
                        string[] userLines = File.ReadAllLines(Variables.usersFile);
                        for (int i = 0; i < userLines.Length && foundPassword == false; i++)
                        {
                            if (userLines[i].Split('=')[1].ToString() == userPassword)
                            {
                                foundPassword = true;
                            }
                        }
                    }
                    if (foundPassword == false) Console.WriteLine("Wrong password for user: " + userName);
                }

                //Search for UserPower in groups.txt
                int userPower = 0;
                string userGroup = "";
                string[] groupsLines = File.ReadAllLines(Variables.groupsFile);
                for (int i = 0; i < groupsLines.Length; i++)
                {
                    if (groupsLines[i].ToString().Contains(userName))
                    {
                        userGroup = groupsLines[i].ToString().Split(':')[0];
                        userPower = Int32.Parse(groupsLines[i].ToString().Split(':')[1].Split('=')[0]);
                    }
                }

                //Create new User Class
                User newUser = new User();
                newUser.InitilizeUser(userName, userPower, userGroup);
                Variables.curUser = newUser;

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message.ToString());
            }

            Console.WriteLine("Successfully logged in!");
        }

        protected override void Run()
        {
            Console.Clear();

            Console.WriteLine("Welcome to EncodedOS. Type help to show avaiable commands!");

            while (Variables.shutdown == false)
            {
                Console.Write(Environment.NewLine + Variables.curDir + "> ");
                string cmd = Console.ReadLine();

                switch (cmd)
                {
                    case "help":
                        {
                            Console.WriteLine(Variables.commands);
                            break;
                        }

                    case "list user":
                        {
                            if (File.Exists(Variables.usersFile) == true)
                            {
                                string[] userLines = File.ReadAllLines(Variables.usersFile); //TODO: Change File.ReadAllLines with Filesystem.ReadAllLines
                                for (int i = 0; i < userLines.Length; i++)
                                {
                                    Console.WriteLine("> " + userLines[i].Split('=')[0].ToString());
                                }
                            }
                            else
                            {
                                Console.WriteLine("> The users.txt file wasn't found! The filesystem must be new created!");
                                Filesystem.CreateFileSystem();
                                Console.WriteLine("> Filesystem was created. Restarting OS now....");
                                for (int i = 0; i < 700000; i++) ;
                                Sys.Power.Reboot();
                            }
                            break;
                        }

                    case "list groups":
                        {
                            if (File.Exists(Variables.groupsFile) == true)
                            {
                                string[] groupLines = File.ReadAllLines(Variables.groupsFile); //TODO: Change File.ReadAllLines with Filesystem.ReadAllLines
                                for (int i = 0; i < groupLines.Length; i++)
                                {
                                    if (groupLines[i].ToString().Contains("=") == true)
                                    {
                                        Console.WriteLine("> " + groupLines[i].ToString().Split('=')[0].Split(':')[0]);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("> The groups.txt file wasn't found! The filesystem must be new created!");
                                Filesystem.CreateFileSystem();
                                Console.WriteLine("> Filesystem was created. Restarting OS now....");
                                for (int i = 0; i < 700000; i++) ;
                                Sys.Power.Reboot();
                            }
                            break;
                        }

                    case "reboot":
                        {
                            Sys.Power.Reboot();
                            break;
                        }

                    case "format":
                        {
                            Format.SetToBegin();
                            break;
                        }

                    case "shutdown":
                        {
                            Variables.shutdown = true;
                            break;
                        }

                    case "whoami":
                        {
                            Console.WriteLine("> " + Variables.curUser.userName);
                            break;
                        }

                    case "cls":
                        {
                            Console.Clear();
                            break;
                        }

                    case "clear":
                        {
                            Console.Clear();
                            break;
                        }

                    case "ls":
                        {
                            string[] filesInDir = Directory.GetFiles(Variables.rootDir);
                            string[] dirsInDir = Directory.GetDirectories(Variables.rootDir);

                            Console.WriteLine("Listing for Folder: " + Variables.curDir);

                            for(int i = 0; i < filesInDir.Length; i++)
                            {
                                Console.WriteLine("<File> " + filesInDir[i].ToString());
                            }

                            for (int i = 0; i < dirsInDir.Length; i++)
                            {
                                Console.WriteLine("<Folder> " + dirsInDir[i].ToString());
                            }

                            break;
                        }

                    case "dir":
                        {
                            string[] filesInDir = Directory.GetFiles(Variables.curDir);
                            string[] dirsInDir = Directory.GetDirectories(Variables.curDir);

                            Console.WriteLine("Listing for Folder: " + Variables.curDir);

                            for (int i = 0; i < filesInDir.Length; i++)
                            {
                                Console.WriteLine("<File> " + filesInDir[i].ToString());
                            }

                            for (int i = 0; i < dirsInDir.Length; i++)
                            {
                                Console.WriteLine("<Folder> " + dirsInDir[i].ToString());
                            }

                            break;
                        }

                    default:
                        {
                            if (cmd.Contains("mkfile")) //Create File TODO: Check if Path contains rootDir
                            {
                                if (cmd.Contains(" "))
                                {
                                    string filename = cmd.Split(new string[] { " " }, StringSplitOptions.None)[1];
                                    Filesystem.CreateFile(filename);
                                }
                                else
                                {
                                    Console.WriteLine("> You need to specific a filename!");
                                }
                            }
                            else if (cmd.Contains("rm")) //Delete File TODO: Check whether the fileToDelete is a full path or in the current directory
                            {
                                if (cmd.Contains(" "))
                                {
                                    string fileToDelete = cmd.Split(new string[] { " " }, StringSplitOptions.None)[1];

                                    if (Variables.curDir == Variables.rootDir)
                                    {
                                        Filesystem.DeleteFile(Variables.rootDir + fileToDelete);
                                    }
                                    else
                                    {
                                        Filesystem.DeleteFile(Variables.curDir + @"\" + fileToDelete);
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("You need to specific a filename!");
                                }
                            }
                            else if (cmd.Contains("cd")) //Change Directory
                            {
                                if (cmd.Contains(" "))
                                {
                                    string dirToSwitchTo = cmd.Split(new string[] { " " }, StringSplitOptions.None)[1]; //Userinput
                                    
                                    if (dirToSwitchTo.Contains(Variables.rootDir))
                                    {
                                        Variables.curDir = dirToSwitchTo;
                                    }
                                    else if (dirToSwitchTo == "..")
                                    {
                                        if (Variables.curDir.Substring(0, Variables.curDir.LastIndexOf('\\')) == "0:")
                                        {
                                            Variables.curDir = Variables.rootDir;
                                        }
                                        else
                                        {
                                            Variables.curDir = Variables.curDir.Substring(0, Variables.curDir.LastIndexOf('\\'));
                                        }                              
                                    }
                                    else
                                    {
                                        if (Variables.curDir == Variables.rootDir)
                                        {
                                            string[] dirsInCurDir = Directory.GetDirectories(Variables.curDir);
                                            for (int i = 0; i < dirsInCurDir.Length; i++)
                                            {
                                                if (dirsInCurDir[i].ToString() == dirToSwitchTo || dirsInCurDir[i].ToString().ToLower() == dirToSwitchTo || dirsInCurDir[i].ToString().ToUpper() == dirToSwitchTo) //Use this to get the correct Directory name
                                                {
                                                    Variables.curDir += dirsInCurDir[i].ToString();
                                                }
                                            }   
                                        }
                                        else
                                        {
                                            string[] dirsInCurDir = Directory.GetDirectories(Variables.curDir);
                                            for (int i = 0; i < dirsInCurDir.Length; i++)
                                            {
                                                if (dirsInCurDir[i].ToString() == dirToSwitchTo || dirsInCurDir[i].ToString().ToLower() == dirToSwitchTo || dirsInCurDir[i].ToString().ToUpper() == dirToSwitchTo) //Use this to get the correct Directory name
                                                {
                                                    Variables.curDir += @"\" + dirsInCurDir[i].ToString();
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Console.WriteLine("> You must enter a folder to change to!");
                                }
                            }
                            else if (cmd.Contains("su")) //Switch User
                            {
                                if (cmd.Contains(" "))
                                {
                                    string userToSwitchTo = cmd.Split(new string[] { " " }, StringSplitOptions.None)[1];
                                    SwitchUser.su(userToSwitchTo);
                                }
                                else
                                {
                                    Console.WriteLine("> You must enter a user to switch to!");
                                }
                            }
                            else
                            {
                                Console.WriteLine("> Unrecognized command: " + cmd + Environment.NewLine + "> Type help to get a list of avaiable commands!");
                            }
                            break;
                        }
                }
            }
            return;
        }
    }
}
