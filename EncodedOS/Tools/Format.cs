using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using EncodedOS.System;
using Sys = Cosmos.System;

namespace EncodedOS.Tools
{
    class Format
    {
        public static void SetToBegin()
        {
            try
            {
                if (File.Exists(Variables.usersFile) == true)
                {
                    File.Delete(Variables.usersFile);
                }

                if (File.Exists(Variables.settingsFile) == true)
                {
                    File.Delete(Variables.settingsFile);
                }

                if (File.Exists(Variables.groupsFile) == true)
                {
                    File.Delete(Variables.groupsFile);
                }

                if (File.Exists(Variables.directorySettingsFile) == true)
                {
                    File.Delete(Variables.directorySettingsFile);
                }

                Console.WriteLine("Files deleted");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message.ToString());
                Sys.Global.mDebugger.Break();
            }
        }
    }
}
