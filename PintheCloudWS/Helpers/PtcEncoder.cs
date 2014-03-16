﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PintheCloudWS.Helpers
{
    public class PtcEncoder
    {

        /// <summary>
        /// Encodes the given string
        /// </summary>
        /// <param name="str">The string to encode</param>
        /// <returns>The encoded string</returns>
        public static string Encode(string str)
        {
            return (System.Uri.EscapeDataString(str)).Replace("%2F", "/").Replace("%", ";");
            /*
            byte[] NameEncodein = new byte[str.Length];
            NameEncodein = System.Text.Encoding.UTF8.GetBytes(str);
            string EcodedName = Convert.ToBase64String(NameEncodein);
            return EcodedName;
            */
        }


        /// <summary>
        /// Decodes the given string
        /// </summary>
        /// <param name="str">The string to decode</param>
        /// <returns>The decoded string</returns>
        public static string Decode(string str)
        {
            return System.Uri.UnescapeDataString(str.Replace(";", "%").Replace("/", "%2F"));
            /*
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder strDecoder = encoder.GetDecoder();
            byte[] to_DecodeByte = Convert.FromBase64String(str);
            int charCount = strDecoder.GetCharCount(to_DecodeByte, 0, to_DecodeByte.Length);
            char[] decoded_char = new char[charCount];
            strDecoder.GetChars(to_DecodeByte, 0, to_DecodeByte.Length, decoded_char, 0);
            string Name = new string(decoded_char);
            return Name;
            */
        }    
    }
}
