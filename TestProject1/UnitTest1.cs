using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            //FileStream file = new FileStream(@"C:\Projects\CODit\Riziv\CharReplacer\unicode.txt", FileMode.Open);
            FileStream file = new FileStream(@"C:\Projects\CODit\Riziv\CharReplacer\utf8.txt", FileMode.Open);
            //FileStream file = new FileStream(@"C:\Projects\CODit\Riziv\CharReplacer\utf8nobom.txt", FileMode.Open);

            string searchString = "PETER";
            string replaceSTring = "replaced";

            byte[] buffer = new byte[1024];
            
            byte[] pattern = new byte[Encoding.UTF8.GetBytes(searchString).Length];
            byte[] replacement = new byte[Encoding.UTF8.GetBytes(replaceSTring).Length];

            Encoding.UTF8.GetBytes(searchString, 0, searchString.Length, pattern, 0);
            Encoding.UTF8.GetBytes(replaceSTring, 0, replaceSTring.Length, replacement, 0);
            
            int count = file.Read(buffer, 0, 1024);

            string strBuffer = Encoding.UTF8.GetString(buffer);


            List<int> occurences = new List<int>();


            occurences = IndexOfSequence(buffer, pattern, 0);

            byte[] replaced = ReplaceBytePattern(buffer, pattern, replacement, 0);

            string res = Encoding.Default.GetString(replaced);

#region encoding conversion
            //Encoding coversion
            //string unicodeString = "This string contains the unicode character Pi(\u03a0)";

            //// Create two different encodings.
            //Encoding ascii = Encoding.ASCII;
            //Encoding unicode = Encoding.Unicode;

            //// Convert the string into a byte[].
            //byte[] unicodeBytes = unicode.GetBytes(unicodeString);

            //// Perform the conversion from one encoding to the other.
            //byte[] asciiBytes = Encoding.Convert(unicode, ascii, unicodeBytes);

            //// Convert the new byte[] into a char[] and then into a string.
            //// This is a slightly different approach to converting to illustrate
            //// the use of GetCharCount/GetChars.
            //char[] asciiChars = new char[ascii.GetCharCount(asciiBytes, 0, asciiBytes.Length)];
            //ascii.GetChars(asciiBytes, 0, asciiBytes.Length, asciiChars, 0);
            //string asciiString = new string(asciiChars);

            //// Display the strings created before and after the conversion.
            //Console.WriteLine("Original string: {0}", unicodeString);
            //Console.WriteLine("Ascii converted string: {0}", asciiString);
#endregion

           
            Assert.IsTrue(occurences.Count == 3);
        }

        
        /// <summary>
        /// Find a pattern of bytes in a byte array
        /// </summary>
        /// <param name="buffer">The byte array to search in</param>
        /// <param name="pattern">The pattern to look for in 'buffer'</param>
        /// <param name="startIndex">Index where to start searching</param>
        /// <returns></returns>
        public List<int> IndexOfSequence(byte[] buffer, byte[] pattern, int startIndex)
        {
            List<int> positions = new List<int>();
            int i = Array.IndexOf<byte>(buffer, pattern[0], startIndex);
            while (i >= 0 && i <= buffer.Length - pattern.Length)
            {
                byte[] segment = new byte[pattern.Length];
                Buffer.BlockCopy(buffer, i, segment, 0, pattern.Length);
                if (segment.SequenceEqual<byte>(pattern))
                    positions.Add(i);
                i = Array.IndexOf<byte>(buffer, pattern[0], i + pattern.Length);
            }
            return positions;
        }

        public byte[] ReplaceBytePattern(byte[] buffer, byte[] pattern, byte[] replacement, int startIndex)
        {
            List<int> positions = new List<int>();
            int i = Array.IndexOf<byte>(buffer, pattern[0], startIndex);
            while (i >= 0 && i <= buffer.Length - pattern.Length)
            {
                byte[] segment = new byte[pattern.Length];
                Buffer.BlockCopy(buffer, i, segment, 0, pattern.Length);
                if (segment.SequenceEqual<byte>(pattern))
                    positions.Add(i);
                i = Array.IndexOf<byte>(buffer, pattern[0], i + pattern.Length);
            }

            //first the positions of the string to replace are searched, because byte arrays have to be initialized with a fixed length
            byte[] replaced = new byte[buffer.Length - pattern.Length * positions.Count + replacement.Length * positions.Count];
            int offset = 0;
            int srcOffset = 0;
            for(int j = 0; j < positions.Count; j++)
            {
                if(j>0)
                    srcOffset = positions[j -1] + pattern.Length;
                int length = positions[j] - srcOffset;

                Buffer.BlockCopy(buffer, srcOffset, replaced, offset, length);
                offset += length;
                Buffer.BlockCopy(replacement, 0, replaced, offset, replacement.Length);
                offset += replacement.Length;
            }
            //copy what is left after the last search position (or if nothing found)
            if(positions.Count > 0)
                srcOffset = positions[positions.Count -1] + pattern.Length;
            else
                srcOffset = 0;
            Buffer.BlockCopy(buffer, srcOffset, replaced, offset, buffer.Length - srcOffset);

            return replaced;
        }

    }
}
