using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using NLog;
using ServiceStack.Text;

namespace Aurora.IO.NamedPipe
{
    public class Server<T>
    {
        #region Events                                
        public delegate void ClientMessageReceivedHandler(T message);
        public event ClientMessageReceivedHandler ClientMessageReceived;
        private void OnClientMessageReceived(T message)
        {
            m_Log.Debug($"** Event: {message}");
            ClientMessageReceived?.Invoke(message);
        }
        #endregion
        #region Properties

        public bool IsRunning => m_Server != null;
        #endregion

        #region Private Members

        private Logger m_Log = LogManager.GetLogger("IpcServer");
        private string m_PipeName;
        private static NamedPipeServerStream m_Server;
        private bool m_ToRun = false;
        #endregion                                    
        public Server(string pipeName)
        {
            m_Log.Warn($">> Server ctor");
            m_PipeName = pipeName;
            m_Log.Warn($"<< Server ctor");
        }

        public bool Start()
        {
            bool retVal = false;
            try
            {
                m_Log.Warn($">> Start");
                m_ToRun = true;
                m_Server = new NamedPipeServerStream(m_PipeName, PipeDirection.In, 1, PipeTransmissionMode.Message);
                Task.Run(() => Worker(m_Server));
                retVal = true;
            }
            catch (IOException ioEx)
            {
                m_Log.Debug($"** IOExcp must likely already running {ioEx}");
            }
            catch (Exception ex)
            {
                m_Log.Debug($"** Start error {ex}");
            }
            finally
            {
                m_Log.Warn($"<< Start {retVal}");
            }
            return (retVal);
        }

        private void Worker(NamedPipeServerStream m_Server)
        {
            try
            {
                m_Log.Warn($">> Worker");
                do
                {
                    try
                    {
                        if (m_Server == null)
                            m_Server = new NamedPipeServerStream(m_PipeName, PipeDirection.In, 1, PipeTransmissionMode.Message);
                        m_Log.Warn($">> Waiting for Connection");
                        m_Server.WaitForConnection();
                        m_Log.Warn($"<< Waiting for Connection");
                        int readCount;
                        do
                        {
                            m_Log.Warn($">> Read Stream");
                            byte[] seBuffer = new byte[1024];
                            readCount = m_Server.Read(seBuffer, 0, seBuffer.Length);
                            string clientMessage = Encoding.UTF8.GetString(seBuffer, 0, readCount);
                            m_Log.Debug($"** Client message completed?{m_Server.IsMessageComplete} {readCount} long -> {clientMessage}#");
                            var clientData = JsonSerializer.DeserializeFromString<T>(clientMessage);
                            m_Log.Debug($"** Client data: {clientData}");
                            if (readCount > 0 && !object.Equals(clientData, default(T)))
                                OnClientMessageReceived(clientData);
                        } while (!m_Server.IsMessageComplete && readCount > 0);

                        m_Server.Dispose();
                        m_Server = null;
                    }
                    catch (Exception ex)
                    {
                        m_Log.Warn($"** Exception {ex}");
                        m_ToRun = false;
                    }
                } while (m_ToRun);
            }
            catch (Exception ex)
            {
                m_Log.Warn($"** Worker abort exception {ex}");
            }
            finally
            {
                m_Log.Warn($"<< Worker");
            }

        }
    }
}
