using System;
using TechTalk.SpecFlow;
using System.IO;
using GitTools.Testing;
using GitVersion;
using GitVersionCore.Tests;

namespace GitVersionCore.AcceptanceTests
{
    using System.Linq;
    using EnumsNET;
    using GitVersion.AcceptanceTests;
    using GitVersion.Helpers;
    using LibGit2Sharp;
    using LogLevel = GitTools.Logging.LogLevel;

    [Binding]
    public class GenerateReleaseVersionNumberSteps
    {
        static GenerateReleaseVersionNumberSteps()
        {
            SetLogger(LogLevel.Warn);
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
            var destinationFileName = Path.Combine(RepositoryFixture.RepositoryPath, ConfigurationProvider.DefaultConfigFileName);
            File.Copy(configFilePath,destinationFileName);
            Config = ConfigurationProvider.Provide(new GitPreparer(RepositoryFixture.RepositoryPath), new FileSystem(), true);
            return;
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
                var actionRow = GetActionRow(row);
                Logger.WriteWarning($"{new String('-', 40)} <Begin> {new String('-', 40)}");
                Logger.WriteWarning($"Version before {actionRow.Idx} ({actionRow.SHA} {actionRow.Branch}) : {RepositoryFixture.GetVersion(Config).FullSemVer}");
                //var action = Enums.Parse<LogAction>(row["ACTION"], EnumFormat.Description);
                switch (actionRow.Action)
                {
                    case LogAction.Commit:
                        RepositoryFixture.ApplyCommit(actionRow);
                        break;
                    case LogAction.Merge:
                        RepositoryFixture.ApplyMerge(actionRow);
                        break;
                    case LogAction.Tag:
                        RepositoryFixture.ApplyTag(actionRow);
                        break;
                    case LogAction.Branch:
                        RepositoryFixture.ApplyBranch(actionRow);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                Logger.WriteWarning($"SemVer Version after {actionRow.Idx} - Version at commit {actionRow.SHA} ({actionRow.Branch}) : {RepositoryFixture.GetVersion(Config).FullSemVer}");
                Logger.WriteWarning($"Nuget package version after {actionRow.Idx} - Version at commit {actionRow.SHA} ({actionRow.Branch}) : {RepositoryFixture.GetVersion(Config).NuGetVersion}");
                Logger.WriteWarning($"{new String('-', 40)} <End> {new String('-', 40)}");
            }            
        }

        //private void ApplyCommit(ActionRow actionRow)
        //{
        //    createBranchIfNotExisits(actionRow);
        //    CheckoutBranch(actionRow.Branch);
        //    RepositoryFixture.MakeACommit();
        //}

        //private void ApplyMerge(ActionRow actionRow)
        //{
        //    CheckoutBranch(actionRow.Branch);
        //    RepositoryFixture.MergeNoFF(actionRow.MergeSourceBranch);
        //}
        //private void ApplyTag(ActionRow actionRow)
        //{
        //    CheckoutBranch(actionRow.Branch);
        //    RepositoryFixture.ApplyTag(actionRow.MergeSourceBranch);
        //}

        //private void CheckoutBranch(string branchName)
        //{
        //    if (!RepositoryFixture.Repository.Head.HasName(branchName))
        //    {
        //        RepositoryFixture.Checkout(branchName);
        //    }
        //}
        //private void createBranchIfNotExisits(ActionRow actionRow)
        //{
        //    if (!RepositoryFixture.Repository.Branches.Any(x => x.HasName(actionRow.Branch)))
        //    {
        //        //create branch from source
        //        RepositoryFixture.Checkout(actionRow.MergeSourceBranch);
        //        RepositoryFixture.BranchTo(actionRow.Branch);
        //    }
        //}

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

    internal static class GitHelperExtensions
    {
        public static bool HasName(this Branch branchToCheck, string nameToCheck)
        {
            return string.Compare(branchToCheck?.FriendlyName, nameToCheck, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
        public static void ApplyBranch(this RepositoryFixtureBase repositoryFixture, ActionRow actionRow)
        {
            repositoryFixture.createBranchIfNotExisits(actionRow);
            //repositoryFixture.CheckoutBranch(actionRow.Branch);
            //repositoryFixture.MakeACommit();
        }

        public static void ApplyCommit(this RepositoryFixtureBase repositoryFixture, ActionRow actionRow)
        {
            repositoryFixture.createBranchIfNotExisits(actionRow);
            repositoryFixture.CheckoutBranch(actionRow.Branch);
            repositoryFixture.MakeACommit();
        }

        public static void ApplyMerge(this RepositoryFixtureBase repositoryFixture, ActionRow actionRow)
        {
            //var result = (repositoryFixture.createBranchIfNotExisits(actionRow));
            repositoryFixture.CheckoutBranch(actionRow.Branch);
            //if (result)
            //{
            //    //If we needed to create a branch on merge (meaning that the branche creation have not in our dataset), force a commit
            //    repositoryFixture.ApplyCommit(actionRow);
            //}
            repositoryFixture.MergeNoFF(actionRow.MergeSourceBranch);
        }
        public static void ApplyTag(this RepositoryFixtureBase repositoryFixture, ActionRow actionRow)
        {
            repositoryFixture.CheckoutBranch(actionRow.Branch);
            repositoryFixture.ApplyTag(actionRow.MergeSourceBranch);
        }

        private static void CheckoutBranch(this RepositoryFixtureBase repositoryFixture, string branchName)
        {
            if (!repositoryFixture.Repository.Head.HasName(branchName))
            {
                repositoryFixture.Checkout(branchName);
            }
        }
        private static bool createBranchIfNotExisits(this RepositoryFixtureBase repositoryFixture, ActionRow actionRow)
        {
            if (!repositoryFixture.Repository.Branches.Any(x => x.HasName(actionRow.Branch)))
            {
                //create branch from source
                if (string.IsNullOrWhiteSpace(actionRow.MergeSourceBranch))
                {
                    throw new ArgumentNullException(nameof(actionRow.MergeSourceBranch), $"Idx {actionRow.Idx}: Impossible de créer la branche {actionRow.Branch} car aucune branche source n'est spécifiée");
                }
                repositoryFixture.Checkout(actionRow.MergeSourceBranch);
                repositoryFixture.BranchTo(actionRow.Branch);
                return true;
            }

            return false;
        }
    }
}
