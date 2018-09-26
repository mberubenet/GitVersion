namespace GitVersionCore.AcceptanceTests
{
    using System;
    using System.Linq;
    using GitTools;
    using GitTools.Testing;
    using GitVersion;
    using GitVersionCore.Tests;
    using LibGit2Sharp;
    using Shouldly;

    internal static class GitHelperExtensions
    {
        //internal enum ActionResult
        //{
        //    Ok,
        //    Error,
        //    BranchCreated
        //}

        public static bool HasName(this Branch branchToCheck, string nameToCheck)
        {
            return string.Compare(branchToCheck?.FriendlyName, nameToCheck, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
        public static void ApplyBranch(this RepositoryFixtureBase repositoryFixture, ActionRow actionRow, Action<string, string, ActionRow> branchCreated)
        {
            repositoryFixture.createBranchIfNotExisits(actionRow, branchCreated);
            //repositoryFixture.CheckoutBranch(actionRow.Branch);
            //repositoryFixture.MakeACommit();
        }

        public static void ApplyCommit(this RepositoryFixtureBase repositoryFixture, ActionRow actionRow, Action<string, string, ActionRow> branchCreated)
        {
            repositoryFixture.createBranchIfNotExisits(actionRow, branchCreated);
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
            return;
        }
        public static void ApplyTag(this RepositoryFixtureBase repositoryFixture, ActionRow actionRow)
        {
            repositoryFixture.CheckoutBranch(actionRow.Branch);
            repositoryFixture.ApplyTag(actionRow.MergeSourceBranch);
            return;
        }

        private static void CheckoutBranch(this RepositoryFixtureBase repositoryFixture, string branchName)
        {
            if (!repositoryFixture.Repository.Head.HasName(branchName))
            {
                repositoryFixture.Checkout(branchName);
            }
        }
        private static void createBranchIfNotExisits(this RepositoryFixtureBase repositoryFixture, ActionRow actionRow, Action<string,string,ActionRow> branchCreated)
        {
            if (!repositoryFixture.Repository.Branches.Any(x => HasName(x, actionRow.Branch)))
            {
                //create branch from source
                if (string.IsNullOrWhiteSpace(actionRow.MergeSourceBranch))
                {
                    throw new ArgumentNullException(nameof(actionRow.MergeSourceBranch), $"Idx {actionRow.Idx}: Impossible de créer la branche {actionRow.Branch} car aucune branche source n'est spécifiée");
                }
                repositoryFixture.Checkout(actionRow.MergeSourceBranch);
                repositoryFixture.BranchTo(actionRow.Branch);
                branchCreated?.Invoke(actionRow.MergeSourceBranch, actionRow.Branch, actionRow);
            }
        }

        public static void AssertSemver(this RepositoryFixtureBase fixture, string semver, IRepository repository = null, string commitId = null, bool isForTrackedBranchOnly = true, string targetBranch = null)
        {
            fixture.AssertSemver(new Config(), semver, repository, commitId, isForTrackedBranchOnly, targetBranch);
        }

        public static void AssertSemver(this RepositoryFixtureBase fixture, Config configuration, string semver, IRepository repository = null, string commitId = null, bool isForTrackedBranchOnly = true, string targetBranch = null)
        {
            ConfigurationProvider.ApplyDefaultsTo(configuration);
            Console.WriteLine("---------");

            try
            {
                var variables = fixture.GetVersion(configuration, repository, commitId, isForTrackedBranchOnly, targetBranch);
                variables.SemVer.ShouldBe(semver);
            }
            catch (Exception)
            {
                (repository ?? fixture.Repository).DumpGraph();
                throw;
            }
            if (commitId == null)
            {
                fixture.SequenceDiagram.NoteOver(semver, fixture.Repository.Head.FriendlyName, color: "#D3D3D3");
            }
        }
    }
}