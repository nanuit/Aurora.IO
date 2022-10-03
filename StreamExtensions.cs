using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.IO
{
    /// <summary>
    /// Extension Methods enhancing the System.IO.Steam class
    /// </summary>
    public static class StreamExtensions
    {
        /// <summary>
        /// Read a number of bytes from a given stream an convert it into an uint
        /// </summary>
        /// <param name="stream">stream to read the bytes from</param>
        /// <param name="bytesToRead">number ob bytes to read. Must not exceed 4</param>
        /// <returns>uint value converted from the stream</returns>
        /// <exception cref="Exception">Exception if not all the specified bytes could be read from the stream </exception>
        /// <exception cref="ArgumentException">if length is larger than 4</exception>
        public static uint GetUintFromStream(this Stream stream,  int bytesToRead)
        {
            byte[] intToRead = new byte[4];
            if (bytesToRead > 4)
                throw (new ArgumentException("bytesToRead"));
            if (stream.Read(intToRead, 4 - bytesToRead, bytesToRead) != bytesToRead)
                throw (new Exception($"data could not be read"));
            return (System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(intToRead));
        }
        /// <summary>
        /// Read a number of bytes from a stream into an ByteArray
        /// </summary>
        /// <param name="stream">stream to read the bytes from</param>
        /// <param name="bytesToRead">number of bytes to read</param>
        /// <returns></returns>
        /// <exception cref="Exception">Exception if not all the specified bytes could be read from the stream </exception>
        public static byte[] GetByteArrayFromStream(this Stream stream, int bytesToRead)
        {
            byte[] readBytes = new byte[bytesToRead];
            if (stream.Read(readBytes, 0, bytesToRead) != bytesToRead)
                throw (new Exception($"data could not be read"));
            return readBytes;
        }

        /// <summary>
        /// read a given amoutn of bytes from a stream and convert it to a string with the default text encoding
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="bytesToRead"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static string GetStringFromStream(this Stream stream, int bytesToRead)
        {
            byte[] buffer = new byte[bytesToRead];
            if (stream.Read(buffer, 0, bytesToRead) != bytesToRead)
                throw (new Exception($"data could not be read"));
            return System.Text.Encoding.Default.GetString(buffer);
        }
        

    }
}
