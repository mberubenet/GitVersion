namespace GitVersionCore.AcceptanceTests
{
    using System;
    using EnumsNET;
    using GitVersion.AcceptanceTests;

    internal class ActionRow:ICloneable
    {
        private string _message;
        public string Idx { get; set; }
        public string SHA { get; set; }
        public string Branch { get; set; }
        public LogAction Action { get; set; }
        public string MergeSourceBranch { get; set; }
        public string Message {
            get
            {
                if (string.IsNullOrWhiteSpace(_message))
                {
                    return FormatMessage();
                }

                return _message;
            }

            set { _message = value; }
        }

        private string FormatMessage()
        {
            switch (Action)
            {
                case LogAction.Commit:
                    return FormatCommit();
                case LogAction.Merge:
                    return FormatMerge();
                case LogAction.Tag:
                    return FormatTag();
            }

            return "No Comment";
        }

        private string FormatCommit()
        {
            var message = $"Commit created on branch {Branch}.";
            if (!string.IsNullOrWhiteSpace(MergeSourceBranch))
            {
                message += $" (Branch {Branch} from {MergeSourceBranch}).";
            }

            return message;
        }

        private string FormatMerge()
        {
            return $"Branch {MergeSourceBranch} merged into {Branch}.";
        }

        private string FormatTag()
        {
            return $"Tag {MergeSourceBranch} created on branch {Branch}.";
        }

        public object Clone()
        {
            var newRow = new ActionRow
            {
                Branch = this.Branch,
                Action = this.Action,
                MergeSourceBranch = this.MergeSourceBranch,
                Idx = this.Idx,
                Message = this.Message,
                SHA = this.SHA
            };
            return newRow;
        }
    }
}