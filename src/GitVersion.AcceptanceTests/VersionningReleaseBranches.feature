Feature: Versionning release branches
	In order to track release number
	As a committer
	I want git version to automatically generate the release number

@mytag
Scenario: Create versionned release branch
	Given A master branch at version ("1.0.0")
    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
	When I create a release branch named ("release-1.1")
	Then The version should be ("1.0.0")

Scenario: Create versionned release branch and commit
	Given A master branch at version ("1.0.0")
    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
	When I create a release branch named ("release/1.1")
    And  I create a commit
	Then The version should be ("1.1.0-release.1+1")

Scenario: Create versionned release branch and commit twice
	Given A master branch at version ("1.0.0")
    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
	When I create a release branch named ("release/1.1")
    And  I create a commit
    And  I create a commit
	Then The version should be ("1.1.0-release.1+2")

Scenario: Create versionned release branch, commit and tag
	Given A master branch at version ("1.0.0")
    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
	When I create a release branch named ("release/1.1")
    And  I create a commit
    And  I create a tag named ("v1.2.1")
	Then The version should be ("1.2.1")

Scenario: Create named release branch
	Given A master branch at version ("1.0.0")
    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
	When I create a release branch named ("release/Magasinez")
	Then The version should be ("1.0.0")

Scenario: Create named release branch and commit
	Given A master branch at version ("1.0.0")
    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
	When I create a release branch named ("release/Magasinez")
    And  I create a commit
	Then The version should be ("1.0.1-Magasinez.1+1")

Scenario: Create named release branch and commit twice
	Given A master branch at version ("1.0.0")
    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
	When I create a release branch named ("release/Magasinez")
    And  I create a commit
    And  I create a commit
	Then The version should be ("1.0.1-Magasinez.1+2")

Scenario: Create named release branch, commit and tag
	Given A master branch at version ("1.0.0")
    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
	When I create a release branch named ("release/Magasinez")
    And  I create a commit
    And  I create a tag named ("v1.2.1-Magasinez")
	Then The version should be ("1.2.1-Magasinez")

Scenario: Create named and versionned release branch and commit
	Given A master branch at version ("1.0.0")
    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
	When I create a release branch named ("release/1.1-Magasinez")
    And  I create a commit
	Then The version should be ("1.1.0-Magasinez.1+1")

Scenario: Create named and versionned release branch and commit twice
	Given A master branch at version ("1.0.0")
    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
	When I create a release branch named ("release/1.1-Magasinez")
    And  I create a commit
    And  I create a commit
	Then The version should be ("1.1.0-Magasinez.1+2")

#Scenario: Create versionned release branch and merge back to master
#	Given A master branch at version ("1.0.0")
#    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
#	When I create a release branch named ("release-1.1")
#    And  I create a commit
#    And  I merge ("release-1.1") to ("master")
#	Then The version should be ("1.1.0")
#
#Scenario: Create versionned release branch, tag version and merge back to master
#	Given A master branch at version ("1.0.0")
#    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
#	When I create a release branch named ("release-1.1")
#    And  I create a commit
#    And I create a tag named ("v1.1.0")
#    And  I merge ("release-1.1") to ("master")
#	Then The version should be ("1.1.0")
#
#Scenario: Create named release branch, tag version and merge back to master
#	Given A master branch at version ("1.0.0")
#    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
#	When I create a release branch named ("release-Magasinez")
#    And  I create a commit
#    And I create a tag named ("v1.1.0")
#    And  I merge ("release-Magasinez") to ("master")
#	Then The version should be ("1.1.0")
#
#Scenario: Create named release branch, merge back to master
#	Given A master branch at version ("1.0.0")
#    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
#	When I create a release branch named ("release-Magasinez")
#    And  I create a commit
#    And  I merge ("release-Magasinez") to ("master")
#	Then The version should be ("1.1.0")
#
#Scenario: Create named release branch, merge it to a versionned branch then merge back to master
#	Given A master branch at version ("1.0.0")
#    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
#	When I create a release branch named ("release-Magasinez")
#    And  I create a commit
#    And I create a release branch named ("release-1.1")
#    And  I merge ("release-Magasinez") to ("release-1.1")
#    And I create a commit
#    And  I merge ("release-1.1") to ("master")
#	Then The version should be ("1.1.0")

#Scenario: Create named hotfix branch, merge back to master
#	Given A master branch at version ("1.0.0")
#    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
#	When I create a release branch named ("hotfix/Crash")
#    And  I create a commit
#    And  I merge ("hotfix/Crash") to ("master")
#	Then The version should be ("1.0.1")
#
#Scenario: Create named hotfix branch, merge back to master
#	Given A master branch at version ("1.0.0")
#    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
#	When I create a release branch named ("hotfix/Crash")
#    And  I create a commit
#    And  I merge ("hotfix/Crash") to ("master")
#	Then The version should be ("1.0.1")
