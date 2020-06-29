// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileOperation.cs" company="E&amp;K Automation GmbH">
//   Copyright (c) E&amp;K Automation GmbH. All rights reserved.
// </copyright>
// <summary>
//   Defines the public class FileOperation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EK.TPM.Data.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.IO;
    using System.Xml;
    using Ionic.Zip;

    /// <summary>
    /// Defines the public class FileOperation.
    /// </summary>
    [Serializable]
    public class FileOperation : IDisposable
    {
        #region Fields

        /// <summary>
        /// The file writer.
        /// </summary>
        private static FileWriter fileWriter = new FileWriter();

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the maximum size of the log file.
        /// </summary>
        /// <value>
        /// The maximum size of the log file.
        /// </value>
        public static int MaxLogFileSize { get; set; } = 128 * 1024 * 1024;

        /// <summary>
        /// Gets the application path.
        /// </summary>
        /// <value>
        /// The application path.
        /// </value>
        public static string ApplicationPath { get; } = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        #endregion

        #region Methods

        /// <summary>
        /// Writes the text to file.
        /// </summary>
        /// <param name="path">
        /// The path including the file.
        /// </param>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <param name="text">
        /// The text to write.
        /// </param>
        /// <param name="maxFileSize">
        /// Max. size of file.
        /// </param>
        public static void WriteTextToFile(string path, string fileName, string text, int maxFileSize = 128 * 1024 * 1024)
        {
            if (fileWriter == null)
            {
                fileWriter = new FileWriter();
            }

            FileWriterObject fwo = new FileWriterObject(path, fileName, text, maxFileSize);
            fileWriter.Add(fwo);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="maxFileSize">Maximum size of the file.</param>
        public static void LogError(Exception ex, int maxFileSize = 128 * 1024 * 1024)
        {
            FileOperation.WriteTextToFile(ApplicationPath + @"\LogFiles\", "Errorlog.txt", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "\r\n" + ex.ToString() + "\r\n", maxFileSize);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="additionalText">The additional text.</param>
        /// <param name="maxFileSize">Maximum size of the file.</param>
        public static void LogError(Exception ex, string additionalText, int maxFileSize = 128 * 1024 * 1024)
        {
            FileOperation.WriteTextToFile(ApplicationPath + @"\LogFiles\", "Errorlog.txt", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "\r\n" + ex.ToString() + "\r\n" + additionalText + "\r\n", maxFileSize);
        }

        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="errorText">The error text.</param>
        /// <param name="maxFileSize">Maximum size of the file.</param>
        public static void LogError(string errorText, int maxFileSize = 128 * 1024 * 1024)
        {
            FileOperation.WriteTextToFile(ApplicationPath + @"\LogFiles\", "Errorlog.txt", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "\r\n" + errorText + "\r\n", maxFileSize);
        }

        /// <summary>
        /// Logs the interface data.
        /// </summary>
        /// <param name="interfaceName">Name of the interface.</param>
        /// <param name="text">The text.</param>
        /// <param name="maxFileSize">Maximum size of the file.</param>
        public static void LogInterfaceData(string interfaceName, string text, int maxFileSize = 128 * 1024 * 1024)
        {
            FileOperation.LogInterfaceData(interfaceName, interfaceName, text, maxFileSize);
        }

        /// <summary>
        /// Logs the interface error.
        /// </summary>
        /// <param name="interfaceName">Name of the interface.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="additionalText">The additional text.</param>
        /// <param name="maxFileSize">Maximum size of the file.</param>
        public static void LogInterfaceError(string interfaceName, Exception ex, string additionalText, int maxFileSize = 128 * 1024 * 1024)
        {
            FileOperation.LogInterfaceError(interfaceName, interfaceName, ex, additionalText, maxFileSize);
        }

        /// <summary>
        /// Logs the interface data.
        /// </summary>
        /// <param name="logfileName">Name of the logfile.</param>
        /// <param name="interfaceName">Name of the interface.</param>
        /// <param name="text">The text.</param>
        /// <param name="maxFileSize">Maximum size of the file.</param>
        public static void LogInterfaceData(string logfileName, string interfaceName, string text, int maxFileSize = 128 * 1024 * 1024)
        {
            FileOperation.WriteTextToFile(ApplicationPath + @"\InterfaceLog\", "LOG_" + logfileName + ".txt", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "\r\n" + interfaceName + "\r\n" + text + "\r\n", maxFileSize);
        }

        /// <summary>
        /// Logs the interface error.
        /// </summary>
        /// <param name="logfileName">Name of the logfile.</param>
        /// <param name="interfaceName">Name of the interface.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="additionalText">The additional text.</param>
        /// <param name="maxFileSize">Maximum size of the file.</param>
        public static void LogInterfaceError(string logfileName, string interfaceName, Exception ex, string additionalText, int maxFileSize = 128 * 1024 * 1024)
        {
            FileOperation.WriteTextToFile(ApplicationPath + @"\InterfaceLog\", "LOG_" + logfileName + ".txt", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "\r\n" + interfaceName + "\r\n" + ex.ToString() + "\r\n" + additionalText + "\r\n", maxFileSize);
        }

        /// <summary>
        /// Logs the io interface error.
        /// </summary>
        /// <param name="interfaceDeviceId">The interface device identifier.</param>
        /// <param name="text">The text.</param>
        /// <param name="maxFileSize">Maximum size of the file.</param>
        public static void LogIoInterfaceError(int interfaceDeviceId, string text, int maxFileSize = 128 * 1024 * 1024)
        {
            FileOperation.WriteTextToFile(ApplicationPath + @"\InterfaceLog\", "LOG_IoDevice_" + interfaceDeviceId + ".txt", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "\r\n" + text + "\r\n", maxFileSize);
        }

        /// <summary>
        /// Logs the event data.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="text">The text.</param>
        /// <param name="maxFileSize">Maximum size of the file.</param>
        public static void LogEventData(string path, string filename, string text, int maxFileSize = 128 * 1024 * 1024)
        {
            FileOperation.WriteTextToFile(path, filename, text, maxFileSize);
        }

        /// <summary>
        /// Logs the event data.
        /// </summary>
        /// <param name="orderNumber">The order number.</param>
        /// <param name="messageText">The message text.</param>
        /// <param name="orderInformation">The order information.</param>
        /// <param name="maxFileSize">Maximum size of the file.</param>
        public static void LogOrderData(int orderNumber, string messageText, string orderInformation = "", int maxFileSize = 128 * 1024 * 1024)
        {
            FileOperation.WriteTextToFile(ApplicationPath + @"\OrderLog\", orderNumber + ".txt", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "\r\n" + messageText + "\r\n" + orderInformation + "\r\n\r\n", maxFileSize);
            FileOperation.WriteTextToFile(ApplicationPath + @"\OrderLog\", orderNumber + "_Short.txt", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss.fff") + "\r\n" + messageText + "\r\n\r\n", maxFileSize);
        }

        /// <summary>
        /// Writes the text to file.
        /// </summary>
        /// <param name="path">
        /// The path including file.
        /// </param>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <param name="byteText">
        /// The byte text to write.
        /// </param>
        /// <param name="maxFileSize">
        /// Max. size of file.
        /// </param>
        public static void WriteTextToFile(string path, string fileName, byte[] byteText, int maxFileSize = 128 * 1024 * 1024)
        {
            if (fileWriter == null)
            {
                fileWriter = new FileWriter();
            }

            FileWriterObject fwo = new FileWriterObject(path, fileName, byteText, maxFileSize);
            fileWriter.Add(fwo);
        }

        /// <summary>
        /// Reads all text from file.
        /// </summary>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <returns>
        /// The read text as string.
        /// </returns>
        public static string ReadAllTextFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            using (StreamReader sr = new StreamReader(file))
            {
                string s = sr.ReadToEnd();
                return s;
            }
        }

        /// <summary>
        /// Reads all text from file to line lineNumber.
        /// </summary>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <param name="lineNumber">
        /// The line number.
        /// </param>
        /// <returns>
        /// The read text as string.
        /// </returns>
        public static string ReadAllTextFromFile(string fileName, int lineNumber)
        {
            string s = string.Empty;
            FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read);
            using (StreamReader sr = new StreamReader(file))
            {
                for (int i = 1; i < lineNumber || sr.EndOfStream; i++)
                {
                    sr.ReadLine();
                }

                if (!sr.EndOfStream)
                {
                    s = sr.ReadLine();
                }
            }

            return s;
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <param name="folder">
        /// The folder.
        /// </param>
        /// <param name="searchPattern">
        /// The search pattern.
        /// </param>
        /// <param name="searchoption">
        /// The search option.
        /// </param>
        /// <returns>
        /// A List of matching files.
        /// </returns>
        public static List<string> GetFiles(string folder, string searchPattern, SearchOption searchoption)
        {
            List<string> files = new List<string>();
            try
            {
                files.AddRange(Directory.GetFiles(folder, searchPattern, SearchOption.TopDirectoryOnly));
            }
            catch
            {
            }

            if (searchoption == SearchOption.AllDirectories)
            {
                foreach (string subDir in Directory.GetDirectories(folder))
                {
                    try
                    {
                        files.AddRange(GetFiles(subDir, searchPattern, searchoption));
                    }
                    catch
                    {
                    }
                }
            }

            return files;
        }

        ///// <summary>
        ///// Changes the config file value.
        ///// </summary>
        ///// <param name="name">
        ///// The name.
        ///// </param>
        ///// <param name="newvalue">
        ///// The new value.
        ///// </param>
        ///// <returns>
        ///// A value indicating weather the change was successful or not.
        ///// </returns>
        //public static bool ChangeConfigFileValue(string name, string newvalue)
        //{
        //    try
        //    {
        //        XmlDocument xmlDoc = new XmlDocument();

        //        xmlDoc.Load(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

        //        foreach (XmlElement element in xmlDoc.DocumentElement)
        //        {
        //            if (element.Name.Equals("applicationSettings"))
        //            {
        //                foreach (XmlNode node in element.ChildNodes)
        //                {
        //                    foreach (XmlNode node2 in node.ChildNodes)
        //                    {
        //                        if (node2.Attributes.Count > 0 && node2.Attributes[0].Value.Equals(name))
        //                        {
        //                            foreach (XmlNode node3 in node2.ChildNodes)
        //                            {
        //                                if (node3.Name == "value")
        //                                {
        //                                    node3.InnerText = newvalue;
        //                                    xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

        //                                    ConfigurationManager.RefreshSection("applicationSettings");
        //                                    return true;
        //                                }
        //                            }

        //                            XmlElement newelement2 = xmlDoc.CreateElement("value");
        //                            newelement2.InnerText = newvalue;
        //                            node2.AppendChild(newelement2);
        //                            xmlDoc.Save(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);

        //                            ConfigurationManager.RefreshSection("applicationSettings");
        //                            return true;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        FileOperation.LogError(ex, FileOperation.MaxLogFileSize);
        //    }

        //    return false;
        //}

        /// <summary>
        /// Compresses the file to ZIP.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="fileToCompress">
        /// The file to compress.
        /// </param>
        /// <param name="compressedFileName">
        /// Name of the compressed file.
        /// </param>
        /// <returns>
        /// A boolean indicating that the file was zipped was successful.
        /// </returns>
        public static bool CompressFileToZIP(string path, string fileToCompress, string compressedFileName)
        {
            string srcFile = path.EndsWith("\\") ? path + fileToCompress : path + "\\" + fileToCompress;
            string dstFile = path.EndsWith("\\") ? path + compressedFileName : path + "\\" + compressedFileName;

            try
            {
                var zipfile = new ZipFile(dstFile);
                zipfile.CompressionLevel = Ionic.Zlib.CompressionLevel.Default;
                zipfile.CompressionMethod = CompressionMethod.Deflate;

                if (zipfile.ContainsEntry(srcFile))
                {
                    zipfile.RemoveEntry(srcFile);
                    zipfile.AddFile(srcFile);
                }
                else
                {
                    zipfile.AddFile(srcFile, string.Empty);
                }

                zipfile.Save();
                zipfile = null;
                return true;
            }
            catch (Exception ex)
            {
                FileOperation.LogError(ex, FileOperation.MaxLogFileSize);
            }

            return false;
        }

        /// <summary>
        /// Determines whether [is file locked] [the specified file name].
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <returns>
        ///   <c>true</c> if [is file locked] [the specified file name]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsFileLocked(string fileName)
        {
            FileStream stream = null;

            try
            {
                FileInfo file = new FileInfo(fileName);
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                // the file is unavailable because it is:
                // still being written to
                // or being processed by another thread
                // or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }
            }

            // file is not locked
            return false;
        }

        #region IDisposable Member

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            while (fileWriter.FileWriterObjectListCount > 0)
            {
                System.Threading.Thread.Sleep(1);
            }

            fileWriter.Stop();
        }

        #endregion

        #endregion
    }
}
