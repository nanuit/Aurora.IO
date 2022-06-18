using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using NLog;
using ServiceStack.Text;

namespace Aurora.IO.NamedPipe
{
    public class Client<T>
    {
        private Logger m_Log = LogManager.GetLogger("IpcClient");
        private NamedPipeClientStream m_Client;
        private StreamWriter m_StremWriter;
        public Client(string pipeName, string pipeServer = ".")
        {
            m_Log.Warn($">> ctor");
            m_Client = new NamedPipeClientStream(pipeServer, pipeName, PipeDirection.Out);
            m_Log.Warn($"<< ctor");
        }

        public void Stop()
        {
            m_Log.Warn($">> Stop");
            m_Client.Close();
            m_Log.Warn($"<< Stop");
        }
        public bool Start()
        {
            bool retVal = false;
            try
            {
                m_Log.Warn($">> Start");
                m_Client.Connect(100);
                m_StremWriter = new StreamWriter(m_Client);
                retVal = true;
            }
            catch (Exception ex)
            {
                m_Log.Error($"** Error {ex}");
            }
            finally
            {
                m_Log.Warn($"<< Start {retVal}");
            }

            return (retVal);
        }

        public void Send(T dataToSent)
        {
            m_Log.Warn($">> Send");
            string jsonied = JsonSerializer.SerializeToString<T>(dataToSent);
            byte[] buffer = Encoding.UTF8.GetBytes(JsonSerializer.SerializeToString<T>(dataToSent));
            m_Log.Trace($"** Send {jsonied}");
            m_Client.Write(buffer, 0, buffer.Length);//Write(JsonSerializer.SerializeToString<T>(dataToSent));
            m_Log.Warn($"<< Send");
        }
    }
}
