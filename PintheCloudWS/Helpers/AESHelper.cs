using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PintheCloudWS.Helpers
{
    public class AESHelper
    {
        private const string PASSWORD = "!PiN_tHe_ClOud_At_Here_PassWORd2";
        private const string SALT = "@At_HeRe_pIN_ThE_cLOud_sAlT3";



        public static string Encrypt(string dataToEncrypt, string password = PASSWORD, string salt = SALT)
        {
            return null;
        }

        public static string Decrypt(string dataToDecrypt, string password = PASSWORD, string salt = SALT)
        {
            return null;
        }
    }
}
