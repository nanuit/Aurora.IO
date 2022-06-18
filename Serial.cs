using NLog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Aurora.IO
{
    public class Serial
    {
        private static SerialPort m_SerialPort;
        public System.IO.Ports.SerialPort SerialPort
        {
            get { return m_SerialPort; }
            set { m_SerialPort = value; }
        }
        private static readonly object m_SyncObject = new object();
        private readonly Logger m_Log;
        private readonly Logger m_LogRaw;
        private readonly BlockingCollection<string> m_ReceiverQueue = new BlockingCollection<string>();

        #region To Life and die in starlight
        public Serial()
        {
            m_Log = LogManager.GetCurrentClassLogger();
            m_LogRaw = LogManager.GetLogger("Helper.IO.SerialRaw");
            Task.Run(() => ReceiverMethod());
        }

        #endregion

        private void ReceiverMethod()
        {
            string previouslyReceivedData = string.Empty;
            do
            {
                try
                {
                    string receivedData = m_ReceiverQueue.Take();
                    m_Log.Trace("Received:{0}", receivedData.Replace("\r", "\\r").Replace("\n", "\\n").Replace("\0", "\\0").Replace("\a", "\\a").Replace("\t", "\\t"));
                    if (LineSeparator.Length > 0)
                        previouslyReceivedData = ReceiveEndingsSeparated(receivedData, previouslyReceivedData);
                    else
                        ReceivePlainData(previouslyReceivedData + receivedData);
                }
                catch (Exception ex)
                {
                    OnError(ex.Message);
                }
            } while (true);
        }

        private void ReceivePlainData(string receivedData)
        {
            OnLineReceived(receivedData);
        }

        private string ReceiveEndingsSeparated(string receivedData, string previouslyReceivedData)
        {
            bool receivedEndsWithNewline = LineSeparator.Any(lineEnding => receivedData.LastIndexOf(lineEnding) == receivedData.Length - 1);
            bool startsWithNewline = LineSeparator.Any(lineEnding => receivedData[0] == lineEnding);

            //separate the lines received
            string[] lines = receivedData.Split(LineSeparator, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length == 0)
                lines = new string[] {string.Empty};
            foreach (var ll in lines)
            {
                m_Log.Trace("line:{0}", ll);
            }
            List<string> receivedLines = new List<string>();
            if (startsWithNewline)
            {
                if (!string.IsNullOrEmpty(previouslyReceivedData))
                {
                    receivedLines.Add(previouslyReceivedData);
                    previouslyReceivedData = string.Empty;
                }
            }

            for (int lineCounter = 0; lineCounter < lines.Length - 1; lineCounter++)
            {
                receivedLines.Add(previouslyReceivedData + lines[lineCounter]);
                previouslyReceivedData = string.Empty;
            }
            if (receivedEndsWithNewline)
            {
                receivedLines.Add(previouslyReceivedData + lines[lines.Length - 1]);
                previouslyReceivedData = string.Empty;
            }

            else
                previouslyReceivedData = previouslyReceivedData + lines[lines.Length - 1];

            foreach (string line in receivedLines)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    m_Log.Trace("displayed:{0}", line);
                    OnLineReceived(line);
                }
            }
            m_Log.Trace("previously:{0}", previouslyReceivedData);
            return (previouslyReceivedData);
        }


        private async Task OpenSerialPort(string comPort, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        {
            await Task.Run(() =>
                           {
                               lock (m_SyncObject)
                               {
                                   if (SerialPort == null || SerialPort.BaudRate != baudrate || SerialPort.PortName != comPort)
                                   {
                                       m_Log.Trace(">> Init Serial Port: {0} baudrate {1} parity {2} dataBits {3} stopBits {4}", comPort, baudrate, parity, dataBits, stopBits);
                                       SerialPort = new SerialPort(comPort, baudrate, parity, dataBits, stopBits);
                                       SerialPort.DataReceived += SerialPortOnDataReceived;
                                       SerialPort.ErrorReceived += SerialPortOnErrorReceived;
                                       m_Log.Trace("<< Init Serial Port");
                                   }
                                   if (SerialPort != null && !SerialPort.IsOpen)
                                   {
                                       m_Log.Trace(">> Open Serial Port: {0} baudrate {1} parity {2} dataBits {3} stopBits {4}", comPort, baudrate, parity, dataBits, stopBits);
                                       SerialPort.Open();
                                       OnConnectionEstablished(SerialPort.PortName, SerialPort.BaudRate);
                                       m_Log.Trace("<< Open Serial Port");
                                   }
                               }
                           });
        }

        private void SerialPortOnErrorReceived(object sender, SerialErrorReceivedEventArgs serialErrorReceivedEventArgs)
        {
            OnError(serialErrorReceivedEventArgs.EventType.ToString());
        }

        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs serialDataReceivedEventArgs)
        {
            string receivedString = string.Empty;
            try
            {
                SerialPort sp = (SerialPort) sender;
                if (LineSeparator.Length == 0)
                    Thread.Sleep(LineTimeout);
                
                do
                {
                    int bytesToRead = sp.BytesToRead;
                    byte[] readbuffer = new byte[bytesToRead];
                    var read = sp.Read(readbuffer, 0, bytesToRead);
                    m_LogRaw.Trace(BitConverter.ToString(readbuffer));
                    receivedString += System.Text.Encoding.Default.GetString(readbuffer);

                } while (sp.BytesToRead > 0);
            }
            catch (Exception ex)
            {
                m_Log.Error(ex, "error reading com {0}", ex);
            }
            
            m_ReceiverQueue.Add(receivedString);

        }

        #region Properties

        public char[] LineSeparator { get; set; } = new char[] {'\r', '\n'};

        public bool Connected => SerialPort?.IsOpen ?? false;
        public string LastConnectedPort { get; set; } = string.Empty;
        public int LineTimeout { get; set; } = 200; 

        #endregion

        #region Public Methods
        public async Task<bool> Open(string comPort, int baudrate)
        {
            return (await Open(comPort, baudrate, Parity.None, 8, StopBits.One));
        }

        public async Task<bool> Open(string comPort, int baudrate, Parity parity, int dataBits, StopBits stopBits)
        {
            try
            {
                await OpenSerialPort(comPort, baudrate, parity, dataBits, stopBits);
                return (true);
            }
            catch (Exception ex)
            {
                m_Log.Warn(ex, "Error initializing Com Port {0}", ex);
                OnError(ex.Message);
                return (false);
            }
        }

        public bool Close()
        {
            lock (m_SyncObject)
            {
                if (SerialPort == null)
                    return (true);
                if (!SerialPort.IsOpen)
                    return (false);
                m_Log.Trace("<< Close Serial Port");
                SerialPort.Close();
            }
            OnConnectionLost();
            return (true);
        }

        public async void SendData(string dataToSend)
        {
            await Task.Run(() =>
                           {
                               try
                               {
                                   string dataSent;
                                   lock (m_SyncObject)
                                   {
                                       dataSent = $"{dataToSend}{new string(LineSeparator)}";
                                       SerialPort.Write(dataSent);
                                   }
                                   m_Log.Warn("Send Data:{0}", dataSent);
                                   OnDataSent(dataToSend);
                               }
                               catch (Exception ex)
                               {
                                   m_Log.Error(ex, "Writing exception {0}", ex);
                                   OnError(ex.Message);
                               }
                           });
        }

        #endregion

        #region Events

        public delegate void LineReceivedHandler(string line);

        public delegate void DataReceivedHandler(byte[] bata);

        public delegate void ConnectionEstablishedHandler(string comPort, int baudRate);

        public delegate void ConnectionLostHandler();

        public delegate void DataSentHandler(string dataSent);

        public delegate void ErrorHandler(string errorText);

        public event ConnectionLostHandler ConnectionLost;
        public event ConnectionEstablishedHandler ConnectionEstablished;
        public event LineReceivedHandler LinesReceived;
        public event DataReceivedHandler DataReceived;
        public event DataSentHandler DataSent;
        public event ErrorHandler Error;

        private void OnDataReceived(byte[] bytes)
        {
            DataReceived?.Invoke(bytes);
        }

        private void OnLineReceived(string line)
        {
            LinesReceived?.Invoke(line);
        }

        private void OnConnectionEstablished(string comPort, int baudRate)
        {
            LastConnectedPort = comPort;
            ConnectionEstablished?.Invoke(comPort, baudRate);
        }

        private void OnConnectionLost()
        {
            ConnectionLost?.Invoke();
            LastConnectedPort = string.Empty;
        }

        private void OnDataSent(string dataSent)
        {
            DataSent?.Invoke(dataSent);
        }

        private void OnError(string errorText)
        {
            Error?.Invoke(errorText);
            if (!SerialPort?.IsOpen ?? true)
                OnConnectionLost();
        }

        #endregion
    }
}
