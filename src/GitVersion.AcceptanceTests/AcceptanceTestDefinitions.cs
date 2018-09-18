using System;
using TechTalk.SpecFlow;
using System.IO;
using GitTools.Logging;
using GitTools.Testing;
using GitVersion;
using GitVersionCore.Tests;

namespace GitVersionCore.AcceptanceTests
{
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
            TextReader configReader = File.OpenText(configFilePath);
            Config = ConfigSerialiser.Read(configReader);
        }

        [When(@"I create a release branch named \(""(.*)""\)")]
        public void WhenICreateAReleaseBranchNamed(string branchName)
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
}
