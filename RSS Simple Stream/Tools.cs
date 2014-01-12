using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace RSS_Simple_Stream
{
    class Tools
    {
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
