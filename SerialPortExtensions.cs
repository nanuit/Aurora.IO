using System;
using System.IO.Ports;
using System.Threading.Tasks;

namespace Aurora.IO
{
    public static class SerialPortExtensions
    {
        public  static async Task<int> ReadAsync(this SerialPort serialPort, byte[] buffer, int offset, int count)
        {
            var bytesToRead = count;
            var temp = new byte[count];
            Console.WriteLine($"Read:{bytesToRead}:{serialPort.BytesToRead}");
            while (bytesToRead > 0 && serialPort.BytesToRead > 0)
            {
                var readBytes = await serialPort.BaseStream.ReadAsync(temp, 0, bytesToRead);
                Array.Copy(temp, 0, buffer, offset + count - bytesToRead, readBytes);
                bytesToRead -= readBytes;
            }
            return count - bytesToRead;
        }
        public static async Task<int> ReadAsync(this SerialPort serialPort, byte[] buffer, int offset)
        {
            var temp = new byte[8192];
            int overallReadBytes = 0;
            do
            {
                var readBytes = await serialPort.BaseStream.ReadAsync(temp, 0, 8192);
                Array.Copy(temp, 0, buffer, offset + overallReadBytes, readBytes);
                overallReadBytes += readBytes;
            } while (serialPort.BytesToRead > 0);
            return overallReadBytes;
        }

        public static async Task<byte[]> ReadAsync(this SerialPort serialPort, int count)
        {
            
            var temp = new byte[count];
            int read = await serialPort.ReadAsync(temp, 0, count);
            var buffer = new byte[read];
            Array.Copy(temp, 0, buffer, 0, read);
            return buffer;
        }
    }
}
