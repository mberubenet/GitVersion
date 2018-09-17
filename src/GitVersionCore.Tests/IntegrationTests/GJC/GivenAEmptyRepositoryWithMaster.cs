using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using GitTools.Testing;
using GitVersion;
using GitVersionCore.Tests;
using LibGit2Sharp;
using NUnit.Framework;

[TestFixture]
public class GivenADynamicConfiguration : TestBase
{
    private readonly string AssemblyPath = Assembly.GetExecutingAssembly().Location;
    private Config _config;

    [SetUp]
    public void BeforeEachTest()
    {
        var assemblyName = this.GetType().Assembly.GetName().Name.ToLowerInvariant();
        var posIndex = AssemblyPath.ToLowerInvariant().IndexOf(assemblyName);
        var projectPath = AssemblyPath.Substring(0, posIndex + assemblyName.Length);
        var configFilePath = Path.Combine(projectPath, "IntegrationTests", "GJC", "Asset", "TestGJCConfiguration.yml");
        TextReader configReader = File.OpenText(configFilePath);
        _config = ConfigSerialiser.Read(configReader);
    }

    [TestFixture]
    public class WhenEmptyMasterWithVersion1_0_0 : GivenADynamicConfiguration
    {
        private const string baseVersion = "1.0.0";

        [Test]
        public void Release_branch_shoud_be_1_1_0_rc1_1()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.MakeATaggedCommit(baseVersion);
                fixture.BranchTo("release-1.1");
                fixture.MakeACommit();

                fixture.AssertFullSemver(_config,"1.1.0-rc.1+1", fixture.Repository);
            }
        }
    }
}
