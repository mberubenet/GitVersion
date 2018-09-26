using System;
using TechTalk.SpecFlow;
using System.IO;
using GitTools.Testing;
using GitVersion;
using GitVersionCore.Tests;
using System.Collections.Generic;
using System.Text;
using EnumsNET;
using GitVersion.AcceptanceTests;
using LogLevel = GitTools.Logging.LogLevel;
using static GitVersion.AcceptanceTests.SpecFlowHelper;

namespace GitVersionCore.AcceptanceTests
{

    [Binding]
    public class GenerateReleaseVersionNumberSteps
    {
        private static VerbosityLevel _verbosityLevel;
        private bool _produceCSV;
        private string _csvFileName;
        private List<string> _csvLines;
        private Dictionary<string, int> _indentDictionary;
        const int Indent_depth = 4;

        static GenerateReleaseVersionNumberSteps()
        {
            SetLogger(LogLevel.Warn);
            _verbosityLevel = VerbosityLevel.Debug;
        }

        public GenerateReleaseVersionNumberSteps()
        {
            _produceCSV = false;
            var now = DateTime.Now.ToString("yyyyMMddHHmmss");
            _csvFileName = $@"c:\temp\cuillette-{now}.csv";
            _csvLines = new List<string>();
            _indentDictionary = new Dictionary<string, int>() { { "master", 0 } };
        }

        [Given(@"A master branch at version \(""(.*)""\)")]
        public void GivenAMasterBranchAtVersion(string startingVersion)
        {
            RepositoryFixture.MakeATaggedCommit(startingVersion);
        }

        [Given(@"An external configuration at path \(""(.*)""\)")]
        public void GivenAnExternalConfigurationAtPath(string relativePath)
        {
            Config = ConfigurationHelper.GetConfiguration(relativePath, RepositoryFixture.RepositoryPath);
        }

        [Given(@"GitVersion configured and a master branch at version \(""(.*)""\)")]
        public void GivenGitVersionConfiguredAndAMasterBranchAtVersion(string startingVersion)
        {
            GivenAMasterBranchAtVersion(startingVersion);
            GivenAnExternalConfigurationAtPath("Asset/TestGJCConfiguration.yml");
        }

        [Given(@"GitVersion configured and a release branch named \(""(.*)""\)")]
        public void GivenGitVersionConfiguredAndAReleaseBranchNamed(string branchName)
        {
            GivenAMasterBranchAtVersion("1.0.0");
            GivenAnExternalConfigurationAtPath("Asset/TestGJCConfiguration.yml");
            WhenICreateABranchNamed(branchName);
            WhenICreateACommit();
        }


        [When(@"I create a branch named \(""(.*)""\)")]
        public void WhenICreateABranchNamed(string branchName)
        {
            RepositoryFixture.BranchTo(branchName);
        }

        [When(@"I create a commit")]
        public void WhenICreateACommit()
        {
            RepositoryFixture.MakeACommit();
        }

        [When(@"I merge \(""(.*)""\) to \(""(.*)""\)")]
        public void WhenIMergeTo(string sourceBranch, string destinationBranch)
        {
            RepositoryFixture.Checkout(destinationBranch);
            RepositoryFixture.MergeNoFF(sourceBranch);
        }

        [When(@"I create a tag named \(""(.*)""\)")]
        public void WhenICreateATagNamed(string tagName)
        {
            RepositoryFixture.ApplyTag(tagName);
        }

        [When(@"I checkout branch\(""(.*)""\)")]
        public void WhenICheckoutBranch(string branchName)
        {
            RepositoryFixture.Checkout(branchName);
        }

        [When(@"I have the following events")]
        public void WhenIHaveTheFollowingEvents(Table table)
        {
           
            Logger.WriteWarning(new String('=', 30));
            foreach (var row in table.Rows)
            {                
                var actionRow = GetActionRow(row);
                //logVersionInfo(actionRow, "Version before", true);
                switch (actionRow.Action)
                {
                    case LogAction.Commit:
                        RepositoryFixture.ApplyCommit(actionRow, OnBranchCreated);
                        break;
                    case LogAction.Merge:
                        RepositoryFixture.ApplyMerge(actionRow);
                        break;
                    case LogAction.Tag:
                        RepositoryFixture.ApplyTag(actionRow);
                        break;
                    case LogAction.Branch:
                        RepositoryFixture.ApplyBranch(actionRow, OnBranchCreated);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                LogActionInformations(actionRow, "Version after",LogArtefact.End);
            }
            Logger.WriteWarning(new String('=', 30));
            writeCSVFile();
        }

        private void OnBranchCreated(string sourceBranch, string newBranch, ActionRow row)
        {
            AddBranchToIndentation(sourceBranch,newBranch);
            //If action is branch, the action will be logged elsewhere
            if (row.Action != LogAction.Branch)
            {
                var clonedActionRow = row.Clone() as ActionRow;
                clonedActionRow.Action = LogAction.Branch;
                LogActionInformations(clonedActionRow);
            }
        }

        private void writeCSVFile()
        {
            if (_produceCSV)
            {
                File.WriteAllLines(_csvFileName,_csvLines);
            }
        }

        private void LogActionInformations(ActionRow row, string prefixText=null,LogArtefact? logArtefact=null)
        {
            int indentLevel = GetIndentLevel(row);

            if (_verbosityLevel != VerbosityLevel.None)
            {
                if (_verbosityLevel == VerbosityLevel.Debug)
                {
                    LogVerbose(row,prefixText, logArtefact, indentLevel);
                }
                else
                {
                    LogShort(row, indentLevel);
                }
            }

            formatCSVLine(row);
        }

        private void formatCSVLine(ActionRow row)
        {
            if (_produceCSV)
            {
                var lastCommit = RepositoryFixture.Repository.Head.Tip;
                var currentBranch = RepositoryFixture.Repository.Head;
                var versionInformation = RepositoryFixture.GetVersion(Config);
                var text = $"{row.Idx};{row.Action};{lastCommit.ShortSHA()};{currentBranch.FriendlyName};{versionInformation.SemVer};{row.Idx} - {row.Action.Format(row, lastCommit, currentBranch)}. Current version : {versionInformation.SemVer}.";
                _csvLines.Add(text);
            }
        }

        private int GetIndentLevel(ActionRow row)
        {
            var branch = RepositoryFixture.Repository.Head.FriendlyName;
            if (!_indentDictionary.ContainsKey(row.Branch))
            {
                return 0;
            }

            var result = _indentDictionary[branch];
            return row.Action == LogAction.Branch ? Math.Max(0, result - Indent_depth) : result;
        }

        private void AddBranchToIndentation(string sourceBranch, string destinationBranch)
        {
            
            int indent = 0;
            if (_indentDictionary.ContainsKey(sourceBranch))
            {
                indent = _indentDictionary[sourceBranch] + Indent_depth ;
            }
            _indentDictionary.Add(destinationBranch,indent);
        }

        private void LogShort(ActionRow row, int indentLevel)
        {
            var lastCommit = RepositoryFixture.Repository.Head.Tip;
            var currentBranch = RepositoryFixture.Repository.Head;
            var versionInformation = RepositoryFixture.GetVersion(Config);
            var text = $"{new String(' ',indentLevel)}{row.Idx} - {row.Action.Format(row, lastCommit, currentBranch)}. Current version : {versionInformation.SemVer}.";
            Logger.WriteWarning(text);
        }

        private void LogVerbose(ActionRow row, string prefixText, LogArtefact? logArtefact, int indentLevel)
        {
            const int lineLength = 15;
            if (logArtefact==LogArtefact.Begin)
            {
                Logger.WriteWarning($"{new String('-', lineLength)} <Begin> {new String('-', lineLength)}");
            }
            var lastCommit = RepositoryFixture.Repository.Head.Tip;
            var currentBranch = RepositoryFixture.Repository.Head;
            var versionInformation = RepositoryFixture.GetVersion(Config);
            prefixText = prefixText + " ";
            var sb = new StringBuilder();
            sb.AppendLine("");
            sb.AppendLine($"{nameof(versionInformation.FullSemVer)} : {versionInformation.FullSemVer}");
            sb.AppendLine($"{nameof(versionInformation.SemVer)} : {versionInformation.SemVer}");
            sb.AppendLine($"{nameof(versionInformation.AssemblySemVer)} : {versionInformation.AssemblySemVer}");
            sb.AppendLine($"{nameof(versionInformation.AssemblySemFileVer)} : {versionInformation.AssemblySemFileVer}");
            sb.AppendLine($"{nameof(versionInformation.InformationalVersion)} : {versionInformation.InformationalVersion}");
            sb.AppendLine($"{nameof(versionInformation.NuGetVersion)} : {versionInformation.NuGetVersion}");
            sb.AppendLine($"{nameof(versionInformation.Major)} : {versionInformation.Major}");
            sb.AppendLine($"{nameof(versionInformation.Minor)} : {versionInformation.Minor}");
            sb.AppendLine($"{nameof(versionInformation.Patch)} : {versionInformation.Patch}");
            var text = $"{new String(' ', indentLevel)}{prefixText}{row.Idx} - {row.Action.Format(row, lastCommit, currentBranch)}. {sb.ToString().TrimEnd(Environment.NewLine.ToCharArray())}";
            Logger.WriteWarning(text);
            if (logArtefact == LogArtefact.End)
            {
                Logger.WriteWarning($"{new String('-', lineLength)} <End> {new String('-', lineLength)}{Environment.NewLine}");
            }
        }

        private ActionRow GetActionRow(TableRow row)
        {
            return new ActionRow()
            {
                Idx = row["IDX"],
                SHA = row["SHA"],
                Branch = row["BRANCH"],
                Action = Enums.Parse<LogAction>(row["ACTION"], EnumFormat.Description),
                MergeSourceBranch=row["MERGE_SOURCE"],
                Message=row["MESSAGE"]
            };
        }

        [Then(@"The version should be \(""(.*)""\)")]
        public void ThenTheVersionShouldBe(string expectedVersion)
        {
            if (Config != null)
            {
                RepositoryFixture.AssertSemver(Config, expectedVersion);
            }
            else
            {
                RepositoryFixture.AssertSemver(expectedVersion);
            }
        }

        [AfterScenario]
        public void AfterEachScenario()
        {
            RepositoryFixture.Dispose();
        }

        private RepositoryFixtureBase RepositoryFixture
        {
            get
            {
                return GetOrCreate(() => new EmptyRepositoryFixture());
            }
        }

        private Config Config
        {
            get => Get<Config>();
            set => Set(value);
        }

        private static void SetLogger(LogLevel logLevel)
        {
            GitVersion.Logger.SetLoggers(
                s => Log(s,LogLevel.Debug,logLevel),
                s => Log(s,LogLevel.Info,logLevel),
                s => Log(s,LogLevel.Warn,logLevel),
                s => Log(s,LogLevel.Error,logLevel));
        }

        private static void Log(string message,LogLevel currentLogLevel, LogLevel minimumLogLevel)
        {
            if (currentLogLevel >= minimumLogLevel)
            {
                Console.WriteLine(message);
            }
        }
    }
}
