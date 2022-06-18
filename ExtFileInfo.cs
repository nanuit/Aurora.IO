using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Shell;
using NLog;

namespace Aurora.IO
{
    
    public class ExtFileInfo
    {
        private static Logger m_Log = LogManager.GetCurrentClassLogger();
        #region Properties
        public string PropertyName { get; set; }
        public string PropertyValueAsString { get; set; }
        public object PropertyValue { get; set; }
        public string PropertyKey { get; set; }
        #endregion
        
        public static List<ExtFileInfo> GetExtendedFileInfo(string fileWithPath)
        {
            List<ExtFileInfo> retVal = new List<ExtFileInfo>();
            var fill = ShellFile.FromFilePath(fileWithPath);
            foreach (var prop in fill.Properties.DefaultPropertyCollection)
            {
                if (prop.ValueAsObject != null)
                {
                    ExtFileInfo newInfo = new ExtFileInfo();
                    newInfo.PropertyKey = prop.PropertyKey.ToString();
                    newInfo.PropertyName = prop.CanonicalName;
                    newInfo.PropertyValue = prop.ValueAsObject;
                    
                    string value = string.Empty;
                    if (prop.ValueType == typeof(string[]))
                    {
                        string[] arrays = prop.ValueAsObject as string[];
                        value = string.Join(";", arrays);
                    }
                    else
                    {
                        value = string.Format("{0}", prop.ValueAsObject ?? "null");
                    }
                    newInfo.PropertyValueAsString = value;
                    m_Log.Trace("File:{0} {1}={2}", Path.GetFileName(fileWithPath), prop.CanonicalName, value);
                }
            }
            return (retVal);

        }
    }
}
