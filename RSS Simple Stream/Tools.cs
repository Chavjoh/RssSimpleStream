using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSS_Simple_Stream
{
    public class Tools
    {
        /// <summary>
        /// Hash input with the given algorithm
        /// </summary>
        /// <param name="input">String to hash</param>
        /// <param name="algorithm">Algorithm to use</param>
        /// <returns>Hash</returns>
        public static string ComputeHash(string input, HashAlgorithm algorithm)
        {
            // Convert string to byte array
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            // Hash with given algorithm
            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);

            // Convert to string and return
            return BitConverter.ToString(hashedBytes);
        }
    }
}
