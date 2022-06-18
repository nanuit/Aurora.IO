using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aurora.IO
{
    public class PathLinuxed
    {
        public static string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path)?.Replace('\\', '/');
        }
        
        public static string Combine(string path1, string path2, string path3)
        {
            return Path.Combine(path1, path2, path3)?.Replace('\\', '/');
        }
        public static string Combine(string path1, string path2, string path3, string path4)
        {
            return Path.Combine(path1, path2, path3, path4)?.Replace('\\', '/');
        }
        
        public static string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2)?.Replace('\\', '/');
        }
    }
}
