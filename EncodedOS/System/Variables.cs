using System;
using System.Collections.Generic;
using System.Text;

namespace EncodedOS.System
{
    class Variables
    {
        public static string rootDir = @"0:\";
        public static string usersFile = @"0:\users.txt";
        public static string groupsFile = @"0:\groups.txt";
        public static string settingsFile = @"0:\settings.txt";
        public static string directorySettingsFile = @"0:\rootDir.txt";

        public static bool shutdown = false;

        public static string commands = "> list user" + Environment.NewLine + "> list groups" + Environment.NewLine + "> reboot" + Environment.NewLine + "> format" + Environment.NewLine + 
            "> whoami" + Environment.NewLine + "> mkfile" + Environment.NewLine + "> ls / dir" + Environment.NewLine + "> rm" + Environment.NewLine + "> cd"  +  Environment.NewLine + "> su" + Environment.NewLine + "> cls / clear" +
            Environment.NewLine;

        public static User curUser;
        public static string curDir = @"0:\";
    }
}
