using System;

namespace Aurora.IO
{
    public class Directory
    {
        /// <summary>
        /// Ensure that the directory part of the given filename with path is existing, created if needed
        /// </summary>
        /// <param name="fileDirectory">full path to file to create the directory for</param>
        /// <returns>true if the directory had to be created</returns>
        public static bool EnsureFileDirectory(string fileDirectory)
        {
            bool retVal = false;
            if (!System.IO.Directory.Exists(PathLinuxed.GetDirectoryName(fileDirectory)))
            {
                System.IO.Directory.CreateDirectory(PathLinuxed.GetDirectoryName(fileDirectory));
                retVal = true;
            }
            return (retVal);
        }
        public static bool EnsureDirectory(string directory)
        {
            bool retVal = false;
            if (!System.IO.Directory.Exists(PathLinuxed.GetDirectoryName(directory)))
            {
                System.IO.Directory.CreateDirectory(PathLinuxed.GetDirectoryName(directory));
                retVal = true;
            }
            return (retVal);
        }
        /// <summary>
        /// Ensure that the directory parameter end with the given subdirectory and that the directory exists
        /// </summary>
        /// <param name="directory">directory to be checked for subdirectory and created if not exists</param>
        /// <param name="subDirectory"></param>
        /// <returns>the directory combined with the subDirectory or null if an error occured</returns>
        public static string EnsureSubDirectory(string directory, string subDirectory)
        {
            string retVal = string.Empty;
            if (!string.IsNullOrEmpty(directory) && !string.IsNullOrEmpty(subDirectory))
            {
                if (!directory.EndsWith(subDirectory, StringComparison.InvariantCultureIgnoreCase))
                    directory = System.IO.Path.Combine(directory, subDirectory);
                EnsureDirectory(directory);
                retVal = directory;
            }                          
            return (retVal);
        }

    }
}
