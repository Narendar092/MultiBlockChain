using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XIDNA.Models
{
    /// <summary>
    /// Zip and Unzip in memory using System.IO.Compression.
    /// </summary>
    /// <remarks>
    /// Include System.IO.Compression in your project.
    /// </remarks>
    public static class ZipHelper
    {
        

        /// <summary>
        /// Unzip a zipped byte array into a string.
        /// </summary>
        /// <param name="zippedBuffer">The byte array to be unzipped</param>
        /// <returns>string representing the original stream</returns>
        public static ZipArchive Unzip(byte[] zippedBuffer)
        {
            Stream stream = new MemoryStream(zippedBuffer);
            ZipArchive archive = new ZipArchive(stream);
            return archive;
        }
    }
}
