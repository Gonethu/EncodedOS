using System;
using System.Collections.Generic;
using System.Text;
using EncodedOS.System;
using Sys = Cosmos.System;

namespace EncodedOS.Tools
{
    class SwitchUser
    {
        public static void su(string user)
        {
            //Search user in the users.txt
            string[] allUsers = Filesystem.ReadAllLines(Variables.usersFile);
            if (allUsers[0] != "error" && allUsers.Length > 0)
            {
                bool foundUserInUserList = false;
                for (int i = 0; i < allUsers.Length; i++)
                {
                    if (allUsers[i].Split('=')[0] == user)
                    {
                        foundUserInUserList = true;
                    }
                }
                if (foundUserInUserList == false)
                {
                    Console.WriteLine("> This user was not found!");
                    return;
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
                    string[] userLines = Filesystem.ReadAllLines(Variables.usersFile);
                    for (int i = 0; i < userLines.Length && correctUserPassword == false; i++)
                    {
                        if (userLines[i].Split('=')[1].ToString() == userPassword)
                        {
                            correctUserPassword = true;
                        }
                    }
                }
                if (correctUserPassword == false) Console.WriteLine("> Wrong password for user: " + user);
            }

            //Get the userpower from the groups.txt
            string[] groupsPower = Filesystem.ReadAllLines(Variables.groupsFile);
            int userGroupPower = 0;
            string userGroup = "";
            if (groupsPower[0] != "error" && groupsPower.Length > 0)
            {
                for (int i = 0; i < groupsPower.Length; i++)
                {
                    if (groupsPower[i].Contains(user))
                    {
                        userGroup = groupsPower[i].ToString().Split(':')[0];
                        userGroupPower = Int32.Parse(groupsPower[i].ToString().Split(':')[1].Split('=')[0]);
                    }
                }
                if (userGroupPower == 0)
                {
                    Console.WriteLine("> This group for the user: " + user + " was not found!");
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
            switchedUser.InitilizeUser(user, userGroupPower, userGroup);
            Variables.curUser = switchedUser;

            Console.WriteLine("> Switched to user: " + user);
        }
    }
}
