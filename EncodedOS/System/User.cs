using System;
using System.Collections.Generic;
using System.Text;

namespace EncodedOS.System
{
    class User
    {
        public string userName = "";
        public int userPower = 0;
        public string userGroup = "";

        public void InitilizeUser(string name, int power)
        {
            userName = name;
            userPower = power;
            //userGroup = group; TODO
        }
    }
}
