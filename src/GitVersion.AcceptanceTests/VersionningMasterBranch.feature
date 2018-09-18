Feature: Versionning master branche
	In order to track master version number
	As a committer
	I want git version to automatically generate the master version number

@mytag
Scenario: Do a commit
	Given A master branch at version ("1.0.0")
    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
	When I create a commit
	Then The version should be ("1.0.1")

Scenario: Create a tag
	Given A master branch at version ("1.0.0")
    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
	When I create a tag named ("v1.2.1")
	Then The version should be ("1.2.1")

#  We cannot go back. The highest version number still used
Scenario: Create a tag lower than current version
	Given A master branch at version ("1.4.5")
    And An external configuration at path ("Asset/TestGJCConfiguration.yml")
	When I create a tag named ("v0.8.1")
	Then The version should be ("1.4.5")
