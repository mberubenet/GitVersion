using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitVersion.AcceptanceTests
{
    using System.ComponentModel;
    using GitVersionCore.AcceptanceTests;
    using LibGit2Sharp;

    public enum LogAction
    {
        [Description("C")] Commit,
        [Description("M")] Merge,
        [Description("T")] Tag,
        [Description("B")] Branch
    }

    public static class LogActionExtensions
    {
        private static Dictionary<LogAction, Func<ActionRow, Commit, Branch, string>>
            _formatFunctionDictionary = new Dictionary<LogAction, Func<ActionRow, Commit, Branch, string>>()
            {
                {LogAction.Commit, FormatCommit},
                {LogAction.Merge, FormatMerge},
                {LogAction.Tag, FormatTag},
                {LogAction.Branch, FormatBranch},
            };

        internal static string Format(this LogAction action, ActionRow row, Commit lastCommit, Branch currentBranch)
        {
            return _formatFunctionDictionary[action](row, lastCommit, currentBranch);
        }

        private static string FormatCommit(ActionRow row, Commit lastCommit, Branch currentBranch)
        {
            return $"Commit {lastCommit.ShortSHA()} in branch {currentBranch.FriendlyName}";
        }
        private static string FormatMerge(ActionRow row, Commit lastCommit, Branch currentBranch)
        {
            return $"Merging {row.MergeSourceBranch} into {currentBranch.FriendlyName}";
        }
        private static string FormatTag(ActionRow row, Commit lastCommit, Branch currentBranch)
        {
            return $"Tag {row.MergeSourceBranch} created on commit {lastCommit.ShortSHA()} in branch {currentBranch.FriendlyName}";
        }
        private static string FormatBranch(ActionRow row, Commit lastCommit, Branch currentBranch)
        {
            return $"Branch {currentBranch.FriendlyName} created from {row.MergeSourceBranch}";
        }

        public static string ShortSHA(this Commit commit)
        {
            return commit.Sha.Substring(0, 8);
        }
    }


}
