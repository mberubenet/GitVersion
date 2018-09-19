Feature: Versioning feature branches
	In order to track feature version number
	As a committer
	I want git version to automatically generate the feature version number

@mytag
Scenario: Create a feature branch from versioned release
	Given GitVersion configured and a release branch named ("release/1.1.0")
	When I create a branch named ("feature/priceCalculation")
	Then The version should be ("1.1.0")

Scenario: Create a feature branch from versioned release and commit
	Given GitVersion configured and a release branch named ("release/1.1.0")
	When I create a branch named ("feature/priceCalculation")
    And I create a commit
	Then The version should be ("1.1.1-feature.priceCalculation.1+1")

Scenario: Create a feature branch from versioned release without tag and commit
	Given GitVersion configured and a release branch named ("release/1.1.0")
	When I create a branch named ("feature/priceCalculation")
    And I create a commit
	Then The version should be ("1.1.1-feature.priceCalculation.1+1")

Scenario: Create a tag
	Given GitVersion configured and a release branch named ("release/1.1.0")
	When I create a branch named ("feature/priceCalculation")
    And I create a tag named ("v1.2.1")
	Then The version should be ("1.2.1")

#  We cannot go back. The highest version number still used
Scenario: Create a tag lower than current version
	Given GitVersion configured and a release branch named ("release/1.1.0")
	When I create a tag named ("v1.4.5")
    And I create a branch named ("feature/priceCalculation")
    And I create a tag named ("v0.2.1")
	Then The version should be ("1.4.5")

Scenario: Create a feature branch from named release
	Given GitVersion configured and a release branch named ("1.1.0")
	When I create a tag named ("v1.1.0")
    And I create a branch named ("feature/priceCalculation")
	Then The version should be ("1.1.0")

Scenario: Create a feature branch from named release and commit
	Given GitVersion configured and a release branch named ("1.1.0")
	When I create a tag named ("v1.1.0")
    And I create a branch named ("feature/priceCalculation")
    And I create a commit
	Then The version should be ("1.1.1-feature.priceCalculation.1+1")
