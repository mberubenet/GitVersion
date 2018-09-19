Feature: Merging Release Into Master
	In order to release a new version in production
	As a committer
	I want git version to automatically generate the master version after the merge

@mytag
Scenario: Create versioned release branch and merge back to master
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release-1.1")
    And  I create a commit
    And  I merge ("release-1.1") to ("master")
	Then The version should be ("1.1.0")

Scenario: Create versioned release branch, tag deploy version and merge back to master
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release-1.1")
    And  I create a commit
    And I create a tag named ("v1.1.0.Deploy51")
    And  I merge ("release-1.1") to ("master")
	Then The version should be ("1.1.0")

Scenario: Create versioned release branch, tag with named version and merge back to master
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release-1.1")
    And  I create a commit
    And I create a tag named ("v1.1.0-production")
    And  I merge ("release-1.1") to ("master")
	Then The version should be ("1.1.0")

Scenario: Create versioned release branch, tag numbered version and merge back to master
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release-1.1")
    And  I create a commit
    And I create a tag named ("v1.1.0")
    And  I merge ("release-1.1") to ("master")
	Then The version should be ("1.1.1")

Scenario: Create named release branch, tag version and merge back to master
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release-Magasinez")
    And  I create a commit
    And I create a tag named ("v1.1.0")
    And  I merge ("release-Magasinez") to ("master")
	Then The version should be ("1.1.1")

#Non supporté dans cette version (work around possible)
@ignore
Scenario: Create named release branch, tag with named version and merge back to master
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release-Magasinez")
    And  I create a commit
    And I create a tag named ("v1.1.0-Production")
    And  I merge ("release-Magasinez") to ("master")
	Then The version should be ("1.1.0")

# Scénario le plus probable pour le site internet
Scenario: Create named release branch, merge it to a versioned branch then merge back to master
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release-Magasinez")
    And  I create a commit
    And I checkout branch("master")
    And I create a branch named ("release-1.1")
    And  I merge ("release-Magasinez") to ("release-1.1")
    And I create a commit
    And I create a tag named ("v1.1.0-Production")
    And  I merge ("release-1.1") to ("master")
	Then The version should be ("1.1.0")

Scenario: Create versioned release branch, merge it to master, add commit to release and merge again
	Given GitVersion configured and a master branch at version ("1.0.0")
	When I create a branch named ("release-1.1")
    And  I create a commit
    And  I merge ("release-1.1") to ("master")
    And I checkout branch("release-1.1")
    And  I create a commit
    And  I merge ("release-1.1") to ("master")
	Then The version should be ("1.1.1")
