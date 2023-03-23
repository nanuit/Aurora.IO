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
        /// <summary>
        /// check the existence of the directory <paramref name="directory"/> and creates it if not
        /// </summary>
        /// <param name="directory"></param>
        /// <returns>true if the directory has been created</returns>
        public static bool EnsureDirectory(string directory)
        {
            bool retVal = false;
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
                retVal = true;
            }
            return (retVal);
        }
        /// <summary>
        /// Ensure the existence of a sub directory <paramref name="subDirectory" />  in the directory <paramref name="directory"/>
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
