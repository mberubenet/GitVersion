using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitVersion.AcceptanceTests
{
    using System.IO;
    using GitVersion.Helpers;


    public static class ConfigurationHelper
    {

        public static Config GetConfiguration(string configFileRelativePath, string repositoryPath)
        {
            string configFilePath = GetConfigurationPath(configFileRelativePath);
            CopyConfigFilToRepositoryDirectory(configFilePath, repositoryPath);
            var result = ConfigurationProvider.Provide(new GitPreparer(repositoryPath), new FileSystem(), true);
            RemoveUnusedBranchConfigurations(configFilePath, result);
            return result;
        }

        private static void RemoveUnusedBranchConfigurations(string configFilePath, Config loadedConfiguration)
        {
            Config customConfig = LoadCustomConfiguration(configFilePath);
            var keyToRemove = loadedConfiguration.Branches.Keys.Where(x => !customConfig.Branches.Keys.Any(y => string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase) == 0)).ToList();
            keyToRemove.ForEach(x => loadedConfiguration.Branches.Remove(x));
            foreach (var branch in loadedConfiguration.Branches.Values)
            {
                var customBranchConfiguration = customConfig.Branches[branch.Name];
                branch.SourceBranches = customBranchConfiguration.SourceBranches;
                branch.IsSourceBranchFor = customBranchConfiguration.IsSourceBranchFor;
            }
        }

        private static Config LoadCustomConfiguration(string configFilePath)
        {
            TextReader configReader = File.OpenText(configFilePath);
            var customConfig = ConfigSerialiser.Read(configReader);
            return customConfig;
        }

        private static void CopyConfigFilToRepositoryDirectory(string configFilePath,string repositoryPath)
        {
            var destinationFileName = Path.Combine(repositoryPath, ConfigurationProvider.DefaultConfigFileName);
            File.Copy(configFilePath, destinationFileName);
        }

        private static string GetConfigurationPath(string relativePath)
        {
            var currentAssembly = typeof(ConfigurationHelper).Assembly;
            var assemblyLocation = currentAssembly.Location;
            var assemblyName = currentAssembly.GetName().Name;
            var posIndex = assemblyLocation.IndexOf(assemblyName, StringComparison.InvariantCultureIgnoreCase);
            var projectPath = assemblyLocation.Substring(0, posIndex + assemblyName.Length);
            var configFilePath = Path.Combine(projectPath, relativePath);
            return configFilePath;
        }
    }
}
