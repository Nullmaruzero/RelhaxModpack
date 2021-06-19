﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelhaxModpack.Settings
{
    /// <summary>
    /// Defines settings used in the database automation runner window
    /// </summary>
    public class AutomationRunnerSettings : ISettingsFile
    {
        /// <summary>
        /// The name of the xml file on disk
        /// </summary>
        public string Filename { get; } = "AutomationRunnerSettings.xml";

        /// <summary>
        /// A list of properties and fields to exclude from saving/loading to and from xml
        /// </summary>
        public string[] MembersToExclude { get { return new string[] { nameof(MembersToExclude), nameof(Filename), nameof(RepoDefaultBranch) }; } }

        /// <summary>
        /// The name of the branch on github that the user specifies to download the automation scripts from
        /// </summary>
        public string SelectedBranch { get; set; } = "master";

        public const string RepoDefaultBranch = "master";

        public bool OpenLogWindowOnStartup { get; set; } = true;

        /// <summary>
        /// Toggle to dump the parsed macros to the log file before every sequence run
        /// </summary>
        public bool DumpParsedMacrosPerSequenceRun { get; set; } = false;
    }
}
