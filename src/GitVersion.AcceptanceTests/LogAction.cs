using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitVersion.AcceptanceTests
{
    using System.ComponentModel;

    public enum LogAction
    {
        [Description("C")]
        Commit,
        [Description("M")]
        Merge,
        [Description("T")]
        Tag,
        [Description("B")]
        Branch
    }
}
