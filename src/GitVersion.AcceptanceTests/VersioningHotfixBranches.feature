Feature: Versioning hotfix branches
	In order to track hotfix version number
	As a committer
	I want git version to automatically generate the hotfix version number

@mytag
Scenario: Create a hotfix branch
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("hotfix/crash")
	Then The version should be ("1.0.0")

Scenario: Create a hotfix branch and commit
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("hotfix/crash")
    And I create a commit
	Then The version should be ("1.0.1-hotfix.crash.1+1")

Scenario: Create a tag
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("hotfix/crash")
    And I create a tag named ("v1.2.1")
	Then The version should be ("1.2.1")

#  We cannot go back. The highest version number still used
Scenario: Create a tag lower than current version
	Given GitVersion configured and a master branch at version ("1.4.5")
	When I create a branch named ("hotfix/crash")
    And I create a tag named ("v0.2.1")
	Then The version should be ("1.4.5")
