Feature: Versioning release branches
	In order to track release number
	As a committer
	I want git version to automatically generate the release number

@mytag
Scenario: Create versionned release branch
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release-1.1")
	Then The version should be ("1.0.0")

Scenario: Create versionned release branch and commit
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release/1.1")
    And  I create a commit
	Then The version should be ("1.1.0-release.1+1")

Scenario: Create versionned release branch and commit twice
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release/1.1")
    And  I create a commit
    And  I create a commit
	Then The version should be ("1.1.0-release.1+2")

Scenario: Create versionned release branch, commit and tag
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release/1.1")
    And  I create a commit
    And  I create a tag named ("v1.2.1")
	Then The version should be ("1.2.1")

Scenario: Create named release branch
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release/Magasinez")
	Then The version should be ("1.0.0")

Scenario: Create named release branch and commit
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release/Magasinez")
    And  I create a commit
	Then The version should be ("1.0.1-Magasinez.1+1")

Scenario: Create named release branch and commit twice
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release/Magasinez")
    And  I create a commit
    And  I create a commit
	Then The version should be ("1.0.1-Magasinez.1+2")

Scenario: Create named release branch, commit and tag
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release/Magasinez")
    And  I create a commit
    And  I create a tag named ("v1.2.1-Magasinez")
	Then The version should be ("1.2.1-Magasinez")

Scenario: Create named and versionned release branch and commit
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release/1.1-Magasinez")
    And  I create a commit
	Then The version should be ("1.1.0-Magasinez.1+1")

Scenario: Create named and versionned release branch and commit twice
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release/1.1-Magasinez")
    And  I create a commit
    And  I create a commit
	Then The version should be ("1.1.0-Magasinez.1+2")
