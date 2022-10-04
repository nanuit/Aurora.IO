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
        /// <summary>
        /// Write a Uint to a stream with BigEndian and only write the given number of bytes
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="uIntToWrite"></param>
        /// <param name="bytesToWrite"></param>
        public static void WriteUint(this Stream stream, uint uIntToWrite, byte bytesToWrite)
        {
            if (bytesToWrite > 4)
                throw (new ArgumentException("bytesToRead"));
            byte[] bytes = new byte[4];
            System.Buffers.Binary.BinaryPrimitives.WriteUInt32BigEndian(bytes, uIntToWrite);
            stream.Write(bytes, 4 - bytesToWrite, bytesToWrite);
        }
        /// <summary>
        /// Read a number of bytes from a stream into an ByteArray
        /// </summary>
        /// <param name="stream">stream to read the bytes from</param>
        /// <param name="bytesToRead">number of bytes to read</param>
        /// <returns></returns>
        /// <exception cref="Exception">Exception if not all the specified bytes could be read from the stream </exception>
        public static byte[] ReadByteArray(this Stream stream, int bytesToRead)
        {
            byte[] readBytes = new byte[bytesToRead];
            if (stream.Read(readBytes, 0, bytesToRead) != bytesToRead)
                throw (new Exception($"data could not be read"));
            return readBytes;
        }
        /// <summary>
        /// Read a string with a given length from a stream with Default Encoding
        /// </summary>
        /// <param name="stream">stream to read the bytes from</param>
        /// <param name="bytesToRead">number of bytes to read</param>
        /// <param name="encoding">Encoding the read string should be generated with</param>
        /// <returns>string read from stream</returns>
        /// <exception cref="Exception">Exception if not all the specified bytes could be read from the stream </exception>
        public static string ReadString(this Stream stream, int bytesToRead, Encoding? encoding = null)
        {
            encoding ??= Encoding.Default;
            return encoding.GetString(stream.ReadByteArray(bytesToRead));
        }
        /// <summary>
        /// Read a bytearray from the stream and convert it to a Base64String
        /// </summary>
        /// <param name="stream">stream to read the bytes from</param>
        /// <param name="bytesToRead">number of bytes to read</param>
        /// <returns>Base64string from stream</returns>
        /// <exception cref="Exception">Exception if not all the specified bytes could be read from the stream </exception>
        public static string ReadBase64(this Stream stream, int bytesToRead)
        {
            return Convert.ToBase64String(stream.ReadByteArray(bytesToRead));
        }
        /// <summary>
        /// Read a number of bytes from a given stream an convert it into an uint
        /// </summary>
        /// <param name="stream">stream to read the bytes from</param>
        /// <param name="bytesToRead">number ob bytes to read. Must not exceed 4</param>
        /// <returns>uint value converted from the stream</returns>
        /// <exception cref="Exception">Exception if not all the specified bytes could be read from the stream </exception>
        /// <exception cref="ArgumentException">if length is larger than 4</exception>
        public static uint ReadUint(this Stream stream, int bytesToRead)
        {
            byte[] intToRead = new byte[4];
            if (bytesToRead > 4)
                throw (new ArgumentException("bytesToRead"));
            if (stream.Read(intToRead, 4 - bytesToRead, bytesToRead) != bytesToRead)
                throw (new Exception($"data could not be read"));
            return (System.Buffers.Binary.BinaryPrimitives.ReadUInt32BigEndian(intToRead));
        }


    }
}
