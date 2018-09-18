Feature: Versioning master branch
	In order to track master version number
	As a committer
	I want git version to automatically generate the master version number

@mytag
Scenario: Do a commit
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a commit
	Then The version should be ("1.0.1")

Scenario: Create a tag
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a tag named ("v1.2.1")
	Then The version should be ("1.2.1")

#  We cannot go back. The highest version number still used
Scenario: Create a tag lower than current version
	Given GitVersion configured and a master branch at version ("1.4.5")
	When I create a tag named ("v0.8.1")
	Then The version should be ("1.4.5")
