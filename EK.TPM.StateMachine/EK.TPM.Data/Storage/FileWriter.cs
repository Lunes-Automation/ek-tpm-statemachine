// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileWriter.cs" company="E&amp;K Automation GmbH">
//   Copyright (c) E&amp;K Automation GmbH. All rights reserved.
// </copyright>
// <summary>
//   Defines the public class FileWriter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EK.TPM.Data.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Defines the public class FileWriter.
    /// </summary>
    public class FileWriter
    {
        #region Fields

        /// <summary>
        /// The lock object for the list.
        /// </summary>
        private static object lockList = new object();

        /// <summary>
        /// The write.
        /// </summary>
        private Thread write;

        /// <summary>
        /// The file writer object.
        /// </summary>
        private List<FileWriterObject> fileWriterObjects = new List<FileWriterObject>(10);

        /// <summary>
        /// The enabled.
        /// </summary>
        private bool enabled;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWriter"/> class.
        /// </summary>
        public FileWriter()
        {
            this.write = new System.Threading.Thread(this.Run);
            this.enabled = true;
            this.write.Start();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the file writer object list count.
        /// </summary>
        /// <value>
        /// The file writer object list count.
        /// </value>
        public int FileWriterObjectListCount
        {
            get
            {
                return this.fileWriterObjects.Count;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public void Start()
        {
            this.enabled = true;
            this.write.Start();
        }

        /// <summary>
        /// Stops this instance.
        /// </summary>
        public void Stop()
        {
            this.enabled = false;
        }

        /// <summary>
        /// Adds the specified File Writer Object to the file writer object list.
        /// </summary>
        /// <param name="fileWriterObject">
        /// The File Writer Object.
        /// </param>
        public void Add(FileWriterObject fileWriterObject)
        {
            lock (lockList)
            {
                this.fileWriterObjects.Add(fileWriterObject);
            }
        }

        /// <summary>
        /// The run.
        /// </summary>
        private void Run()
        {
            while (this.enabled)
            {
                try
                {
                    System.Threading.Thread.Sleep(2000);
                    List<FileWriterObject> fwol_write;

                    lock (lockList)
                    {
                        fwol_write = this.fileWriterObjects;
                        this.fileWriterObjects = new List<FileWriterObject>(100);
                    }

                    int counter = fwol_write.Count;

                    for (int i = 0; i < counter; i++)
                    {
                        if (fwol_write[i] != null)
                        {
                            FileWriterObject fwo = fwol_write[i];
                            string path = fwo.Path;
                            string fileName = fwo.FileName;
                            StringBuilder logText = new StringBuilder(512 * 1024);
                            logText.Append(fwo.Text);
                            int maxFileSize = fwo.MaxFileSize;
                            i++;

                            while (i < counter && fwol_write[i].Path == path && fwol_write[i].FileName == fileName
                                   && logText.Length < 512 * 1024)
                            {
                                logText.Append(fwol_write[i].Text);
                                i++;
                            }

                            if (i < counter)
                            {
                                i--;
                            }

                            this.WriteTextToFile(path, fileName, logText.ToString(), maxFileSize);
                        }
                    }
                }
                catch (Exception ex)
                {
                    FileOperation.LogError(ex, FileOperation.MaxLogFileSize);
                }
            }
        }

        /// <summary>
        /// The write text to file.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <param name="fileName">
        /// The file name.
        /// </param>
        /// <param name="text">
        /// The text.
        /// </param>
        /// <param name="maxFileSize">
        /// The max file size.
        /// </param>
        private void WriteTextToFile(string path, string fileName, string text, int maxFileSize)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (File.Exists(path + fileName) && maxFileSize > 0)
                {
                    if (new FileInfo(path + fileName).Length > maxFileSize)
                    {
                        if (File.Exists(path + fileName + ".bak"))
                        {
                            File.Delete(path + fileName + ".bak");
                            File.Move(path + fileName, path + fileName + ".bak");
                        }
                        else
                        {
                            File.Move(path + fileName, path + fileName + ".bak");
                        }
                    }
                }

                FileStream file = new FileStream(path + fileName, FileMode.Append, FileAccess.Write);
                using (StreamWriter sw = new StreamWriter(file))
                {
                    sw.Write(text);
                }
            }
            catch
            {
            }
        }

        #endregion
    }
}
