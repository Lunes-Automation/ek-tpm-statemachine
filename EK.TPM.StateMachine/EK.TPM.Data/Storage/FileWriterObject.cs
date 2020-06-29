// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FileWriterObject.cs" company="E&amp;K Automation GmbH">
//   Copyright (c) E&amp;K Automation GmbH. All rights reserved.
// </copyright>
// <summary>
//   Defines the public class FileWriterObject.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace EK.TPM.Data.Storage
{
    using System;

    /// <summary>
    /// Defines the public class FileWriterObject.
    /// </summary>
    [Serializable]
    public class FileWriterObject
    {
        #region Fields

        /// <summary>
        /// The max file size.
        /// </summary>
        private int maxFileSize;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWriterObject"/> class.
        /// </summary>
        /// <param name="path">
        /// The path including file.
        /// </param>
        /// <param name="fileName">
        /// Name of the file.
        /// </param>
        /// <param name="text">
        /// The text to write.
        /// </param>
        /// <param name="maxFileSize">
        /// Size of the max file.
        /// </param>
        public FileWriterObject(string path, string fileName, string text, int maxFileSize)
        {
            this.Path = path;
            this.FileName = fileName;
            this.Text = text;
            this.maxFileSize = maxFileSize;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileWriterObject"/> class.
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
        /// Size of the max file.
        /// </param>
        public FileWriterObject(string path, string fileName, byte[] byteText, int maxFileSize)
        {
            this.Path = path;
            this.FileName = fileName;
            this.maxFileSize = maxFileSize;
            for (int i = 0; i < byteText.Length; i++)
            {
                this.Text += "[" + byteText[i] + "]";
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        /// <value>
        /// The path of file.
        /// </value>
        public string Path { get; set; } = "C:\\";

        /// <summary>
        /// Gets or sets the name of the file.
        /// </summary>
        /// <value>
        /// The name of the file.
        /// </value>
        public string FileName { get; set; } = "Default.txt";

        /// <summary>
        /// Gets or sets the text to write.
        /// </summary>
        /// <value>
        /// The text to write.
        /// </value>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the max. size of the file.
        /// </summary>
        /// <value>
        /// The max. size of the file.
        /// </value>
        public int MaxFileSize
        {
            get
            {
                return this.maxFileSize;
            }

            set
            {
                this.maxFileSize = value;
            }
        }

        #endregion
    }
}
