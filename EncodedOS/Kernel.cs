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
                string[] groupsLines = File.ReadAllLines(Variables.groupsFile);
                for (int i = 0; i < groupsLines.Length; i++)
                {
                    if (groupsLines[i].ToString().Contains(userName))
                    {
                        userPower = Int32.Parse(groupsLines[i].ToString().Split(':')[1].Split('=')[0]);
                    }
                }

                //Create new User Class
                User newUser = new User();
                newUser.InitilizeUser(userName, userPower);
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
                                string[] userLines = File.ReadAllLines(Variables.usersFile);
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
                                string[] groupLines = File.ReadAllLines(Variables.groupsFile);
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

                    default:
                        {
                            if (cmd.Contains("mkfile"))
                            {
                                //if (cmd.Contains(" "))
                                //{
                                //    string filename = cmd.Split(new string[] { " " }, StringSplitOptions.None)[1];

                                //    if (File.Exists(filename) == false)
                                //    {
                                //        try
                                //        {
                                //            Filesystem.CreateFile(@"0:\" + filename);
                                //        }
                                //        catch (Exception e)
                                //        {
                                //            Console.WriteLine("Error: " + e.Message.ToString());
                                //        }            
                                //    }
                                //    else
                                //    {
                                //        Console.WriteLine("> This file already exists!");
                                //    }
                                //}
                                //else
                                //{
                                //    Console.WriteLine("> You need to specific a filename!");
                                //}
                            } 
                            else if (cmd.Contains("su"))
                            {

                                if (cmd.Contains(" "))
                                {
                                    //Search user in the users.txt
                                    string userToSwitchTo = cmd.Split(new string[] { " " }, StringSplitOptions.None)[1];
                                    string[] allUsers = Filesystem.ReadAllLines(Variables.usersFile);
                                    if (allUsers[0] != "error" && allUsers.Length > 0)
                                    {
                                        bool foundUserInUserList = false;
                                        for (int i = 0; i < allUsers.Length; i++)
                                        {
                                            if (allUsers[i].Split('=')[0] == userToSwitchTo)
                                            {
                                                foundUserInUserList = true;
                                            }
                                        }
                                        if (foundUserInUserList == false)
                                        {
                                            Console.WriteLine("> This user was not found!");
                                            break;
                                        }                  
                                    }
                                    else if (allUsers[0] == "error")
                                    {
                                        Console.WriteLine("> " + allUsers[1].ToString());
                                        Sys.Power.Reboot();
                                    }
                                    else if (allUsers.Length <= 0)
                                    {
                                        Console.WriteLine("> No users were found, restarting the OS!");
                                        Sys.Power.Reboot();
                                    }

                                    //Need password for user
                                    bool correctUserPassword = false;
                                    while (correctUserPassword == false)
                                    {
                                        Console.Write("Password: ");
                                        string userPassword = Console.ReadLine();

                                        if (userPassword != "")
                                        {
                                            string[] userLines = File.ReadAllLines(Variables.usersFile);
                                            for (int i = 0; i < userLines.Length && correctUserPassword == false; i++)
                                            {
                                                if (userLines[i].Split('=')[1].ToString() == userPassword)
                                                {
                                                    correctUserPassword = true;
                                                }
                                            }
                                        }
                                        if (correctUserPassword == false) Console.WriteLine("> Wrong password for user: " + userToSwitchTo);
                                    }

                                    //Get the userpower from the groups.txt
                                    string[] groupsPower = Filesystem.ReadAllLines(Variables.groupsFile);
                                    int userGroupPower = 0;
                                    if (groupsPower[0] != "error" && groupsPower.Length > 0)
                                    {
                                        for (int i = 0; i < groupsPower.Length; i++)
                                        {
                                            if (groupsPower[i].Contains(userToSwitchTo))
                                            {
                                                userGroupPower = Int32.Parse(groupsPower[i].ToString().Split(':')[1].Split('=')[0]);
                                            }
                                        }
                                        if (userGroupPower == 0)
                                        {
                                            Console.WriteLine("> This group for the user: " + userToSwitchTo + " was not found!");
                                            break;
                                        }
                                    }
                                    else if (groupsPower[0] == "error")
                                    {
                                        Console.WriteLine("> " + groupsPower[1].ToString());
                                        Sys.Power.Reboot();
                                    }
                                    else if (groupsPower.Length <= 0)
                                    {
                                        Console.WriteLine("> No groups were found, restarting the OS!");
                                        Sys.Power.Reboot();
                                    }

                                    User switchedUser = new User();
                                    switchedUser.InitilizeUser(userToSwitchTo, userGroupPower);
                                    Variables.curUser = switchedUser;

                                    Console.WriteLine("> Switched to user: " + userToSwitchTo);
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
