using System;
using TechTalk.SpecFlow;
using System.IO;
using GitTools.Logging;
using GitTools.Testing;
using GitVersion;
using GitVersionCore.Tests;

namespace GitVersionCore.AcceptanceTests
{
    using EnumsNET;
    using GitVersion.AcceptanceTests;

    [Binding]
    public class GenerateReleaseVersionNumberSteps
    {
        static GenerateReleaseVersionNumberSteps()
        {
            SetLogger(LogLevel.Info);
        }

        [Given(@"A master branch at version \(""(.*)""\)")]
        public void GivenAMasterBranchAtVersion(string startingVersion)
        {
            RepositoryFixture.MakeATaggedCommit(startingVersion);
        }

        [Given(@"An external configuration at path \(""(.*)""\)")]
        public void GivenAnExternalConfigurationAtPath(string relativePath)
        {
            var currentAssembly = this.GetType().Assembly;
            var assemblyLocation = currentAssembly.Location;
            var assemblyName = currentAssembly.GetName().Name;
            var posIndex = assemblyLocation.IndexOf(assemblyName,StringComparison.InvariantCultureIgnoreCase);
            var projectPath = assemblyLocation.Substring(0, posIndex + assemblyName.Length);
            var configFilePath = Path.Combine(projectPath, relativePath);
            TextReader configReader = File.OpenText(configFilePath);
            Config = ConfigSerialiser.Read(configReader);
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
            foreach (var row in table.Rows)
            {
                var actionRow = GetActionRow(row)
                var action = Enums.Parse<LogAction>(row["ACTION"], EnumFormat.Description);
                switch (actionRow.Action)
                {
                    case LogAction.Commit:
                        ApplyCommit(GetActionRow(row));
                        break;
                    case LogAction.Merge:
                        ApplyMerge(GetActionRow(row));
                        break;
                    case LogAction.Tag:
                        ApplyTag(GetActionRow(row));
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private ActionRow GetActionRow(TableRow row)
        {
            return new ActionRow()
            {
                SHA= row["SHA"],
                Branch = row["BRANCH"],
                Action = Enums.Parse<LogAction>(row["ACTION"], EnumFormat.Description),
                MergeSourceBranch=row["MERGE_SOURCE"],
                Message=row["Message"]
            };
        }

        [Then(@"The version should be \(""(.*)""\)")]
        public void ThenTheVersionShouldBe(string expectedVersion)
        {
            if (Config != null)
            {
                RepositoryFixture.AssertFullSemver(Config, expectedVersion);
            }
            else
            {
                RepositoryFixture.AssertFullSemver(expectedVersion);
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
                return GetOrCreate("RepositoryFixture", () => new EmptyRepositoryFixture());
            }
        }

        private Config Config { get; set; }

        private T GetOrCreate<T>(string propertyName, Func<T> createInstanceFunc) where T : class
        {

            T currentProperty = null;
            if (ScenarioContext.Current.ContainsKey(propertyName))
            {
                currentProperty = ScenarioContext.Current[propertyName] as T;
            }

            if (currentProperty == null)
            {
                currentProperty = createInstanceFunc();
                ScenarioContext.Current.Add(propertyName,currentProperty);
            }
            return currentProperty;
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

    internal class ActionRow
    {
        public string SHA { get; set; }
        public string Branch { get; set; }
        public LogAction Action { get; set; }
        public string MergeSourceBranch { get; set; }
        public string Message { get; set; }
    }
}
