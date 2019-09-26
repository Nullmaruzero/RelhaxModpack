﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RelhaxModpack
{
    /// <summary>
    /// An object that represents cache files generated by mods that should be backed up and restored as to not loose cache data (like session stats or auto equip data)
    /// </summary>
    public class UserFile
    {
        /// <summary>
        /// The path or pattern to a file or files to backup to a temporary directory
        /// </summary>
        public string Pattern = string.Empty;

        /// <summary>
        /// Speed up the restore backup function in case of ClanIcons, the "backup folder" will be pushed back at once (and not file by file)
        /// </summary>
        [Obsolete("This is for legacy database compatibility and will be ignored in Relhax V2")]
        public bool PlaceBeforeExtraction { get; set; } = false;

        /// <summary>
        /// Entry will be processed in any case (if package is checked), even if "save user data" option is "false"
        /// </summary>
        [Obsolete("This is for legacy database compatibility and will be ignored in Relhax V2")]
        public bool SystemInitiated { get; set; } = false;

        /// <summary>
        /// The list of actual files saved to the temporary backup directory. Contains the full path and file name
        /// </summary>
        public List<string> Files_saved { get; set; } = new List<string>();

        /// <summary>
        /// The string representation of the UserFile object
        /// </summary>
        /// <returns>The string pattern</returns>
        public override string ToString()
        {
            return Pattern;
        }

        /// <summary>
        /// Create a copy of the UserFile object
        /// </summary>
        /// <param name="userFileToCopy">The object to copy</param>
        /// <returns>A new UserFile object with the same values</returns>
        public static UserFile Copy(UserFile userFileToCopy)
        {
            UserFile file =  new UserFile()
            {
                Pattern = userFileToCopy.Pattern,
#pragma warning disable CS0618 // Type or member is obsolete
                PlaceBeforeExtraction = userFileToCopy.PlaceBeforeExtraction,
                SystemInitiated = userFileToCopy.SystemInitiated,
                Files_saved = new List<string>()
            };

            return file;
        }

        /// <summary>
        /// Create a deep copy of the UserFile object
        /// </summary>
        /// <param name="userFileToCopy">The object to copy</param>
        /// <returns>A new UserFile object with the same values and new list elements with the same values</returns>
        public static UserFile DeepCopy(UserFile userFileToCopy)
        {
            UserFile file = new UserFile()
            {
                Pattern = userFileToCopy.Pattern,
                PlaceBeforeExtraction = userFileToCopy.PlaceBeforeExtraction,
                SystemInitiated = userFileToCopy.SystemInitiated,
#pragma warning restore CS0618 // Type or member is obsolete
                Files_saved = new List<string>()
            };

            foreach (string s in userFileToCopy.Files_saved)
                file.Files_saved.Add(s);

            return file;
        }
    }
}
