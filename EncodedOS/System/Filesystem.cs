using System;
using System.Collections.Generic;
using System.Text;
using EncodedOS.System;
using System.IO;
using Sys = Cosmos.System;

namespace EncodedOS.System
{
    class Filesystem
    {
        public static bool CheckFileSystem()
        {
            try
            {
                //Check if the Filesystem includes everything
                if (File.Exists(Variables.usersFile) == false)
                {
                    return false;
                }
                else if (File.Exists(Variables.groupsFile) == false)
                {
                    return false;
                }
                else if (File.Exists(Variables.directorySettingsFile) == false)
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message.ToString());
                Sys.Global.mDebugger.Break();
            }

            return true;
        }

        public static void CreateFileSystem()
        {
            try
            {
                //Create the standard User
                if (File.Exists(Variables.usersFile) == false)
                {
                    File.Create(Variables.usersFile);
                    File.WriteAllText(Variables.usersFile, "admin=admin" + Environment.NewLine + "guest=guest" + Environment.NewLine + "john=john");
                }
                else
                {
                    File.WriteAllText(Variables.usersFile, "admin=admin" + Environment.NewLine + "guest=guest" + Environment.NewLine + "john=john");
                }

                //Add them to the correct group
                if (File.Exists(Variables.groupsFile) == false)
                {
                    File.Create(Variables.groupsFile);
                    File.WriteAllText(Variables.groupsFile, "administrator:100=admin" + Environment.NewLine + "benutzer:50=john" + Environment.NewLine + "guest:25=guest");
                }
                else
                {
                    File.WriteAllText(Variables.groupsFile, "administrator:100=admin" + Environment.NewLine + "benutzer:50=john" + Environment.NewLine + "guest:25=guest");
                }

                //Create the Root Directory file
                if (File.Exists(Variables.directorySettingsFile) == false)
                {
                    File.Create(Variables.directorySettingsFile);
                    File.WriteAllText(Variables.directorySettingsFile, "null");
                }
                else
                {
                    File.WriteAllText(Variables.directorySettingsFile, "null");
                }

                //Now the OS started for the first time, so we change that Value
                if (File.Exists(Variables.settingsFile) == false)
                {
                    File.Create(Variables.settingsFile);
                    File.WriteAllText(Variables.settingsFile, "first_start=false");
                }
                else
                {
                    File.WriteAllText(Variables.settingsFile, "first_start=false");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message.ToString());
                Sys.Global.mDebugger.Break();
            }
        }

        public static string[] ReadAllLines(string filePath)
        {
            if (File.Exists(filePath) == false)
            {
                return "error:File does not exists!".Split(':');
            }

            string[] fileContent = File.ReadAllLines(filePath);
            return fileContent;
        }

        //public static void CreateFile(string location)
        //{
        //    File.Create(location);
        //}
    }
}
