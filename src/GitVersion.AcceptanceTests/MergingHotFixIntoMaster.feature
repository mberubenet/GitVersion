Feature: Merging Hotfix Into Master
	In order to do a hotfix in production
	As a committer
	I want git version to automatically generate the master version after the merge

@mytag
Scenario: Create hotfix branch and merge back to master
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("hotfix/crash")
    And  I create a commit
    And  I merge ("hotfix/crash") to ("master")
	Then The version should be ("1.0.1")

Scenario: Create hotfix branch with two commits and merge back to master
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("hotfix/crash")
    And  I create a commit
    And  I create a commit
    And  I merge ("hotfix/crash") to ("master")
	Then The version should be ("1.0.1")

# Version doesn't increase on merge if hotfix is already versioned
Scenario: Create versioned hotfix branch and merge back to master
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("hotfix-1.0.2")
    And  I create a commit
    And  I merge ("hotfix-1.0.2") to ("master")
	Then The version should be ("1.0.2")
