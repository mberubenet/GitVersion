Feature: Simplified full scenario
	In order to verify the impact of GitVersion
	As a developper
	I want to test full scenarios

@mytag
Scenario: 01 - One versioned release branch
	Given GitVersion configured and a master branch at version ("1.13.0")
	When I have the following events
| IDX  | SHA       | BRANCH            | ACTION | MERGE_SOURCE       | MESSAGE                                                                                                                                                                                                           |
| 0    | 300e86576 | release/1.14      | B      | master             | |
| 1    | 5884caf6c | release/1.14      | C      |                    | |
    Then The version should be ("1.14.0-release.1+1")


Scenario: 02 - One versioned release branch with a feature branch
	Given GitVersion configured and a master branch at version ("1.13.0")
	When I have the following events
| IDX  | SHA       | BRANCH            | ACTION | MERGE_SOURCE       | MESSAGE                                                                                                                                                                                                           |
| 0    | 300e86576 | release/1.14      | B      | master             | |
| 1    | 5884caf6c | release/1.14      | C      |                    | |
| 2    | bf6dbbd68 | feature/homepage  | B      | release/1.14       | |
    Then The version should be ("1.13.0-feature.homepage.1+1")

Scenario: 03 - One versioned release branch and a feature branch no commit
	Given GitVersion configured and a master branch at version ("1.13.0")
	When I have the following events
| IDX  | SHA       | BRANCH            | ACTION | MERGE_SOURCE       | MESSAGE                                                                                                                                                                                                           |
| 0    | 300e86576 | release/1.14      | B      | master             | |
| 1    | bf6dbbd68 | feature/homepage  | B      | release/1.14       | |
    Then The version should be ("1.13.0")

Scenario: 04 - One versioned release branch with a feature branch plus a commit
	Given GitVersion configured and a master branch at version ("1.13.0")
	When I have the following events
| IDX  | SHA       | BRANCH            | ACTION | MERGE_SOURCE       | MESSAGE                                                                                                                                                                                                           |
| 0    | 300e86576 | release/1.14      | B      | master             | |
| 1    | 5884caf6c | release/1.14      | C      |                    | |
| 2    | bf6dbbd68 | feature/homepage  | B      | release/1.14       | |
| 3    | be24f9508 | feature/homepage  | C      |                    | |
    Then The version should be ("1.14.0-feature.homepage.1+1")

Scenario: 05 - One versioned release branch with a feature branch a commit and merge
	Given GitVersion configured and a master branch at version ("1.13.0")
	When I have the following events
| IDX  | SHA       | BRANCH            | ACTION | MERGE_SOURCE       | MESSAGE                                                                                                                                                                                                           |
| 0    | 300e86576 | release/1.14      | B      | master             | |
| 1    | 5884caf6c | release/1.14      | C      |                    | |
| 2    | bf6dbbd68 | feature/homepage  | B      | release/1.14       | |
| 3    | be24f9508 | feature/homepage  | C      |                    | |
| 4    | dedd43461 | release/1.14      | M      | feature/homePage   | |
    Then The version should be ("1.14.0-release.1+3")

Scenario: 06 - One versioned release branch with a feature branch a commit and merge back to master
	Given GitVersion configured and a master branch at version ("1.13.0")
	When I have the following events
| IDX  | SHA       | BRANCH            | ACTION | MERGE_SOURCE       | MESSAGE                                                                                                                                                                                                           |
| 0    | 300e86576 | release/1.14      | B      | master             | |
| 1    | 5884caf6c | release/1.14      | C      |                    | |
| 2    | bf6dbbd68 | feature/homepage  | B      | release/1.14       | |
| 3    | be24f9508 | feature/homepage  | C      |                    | |
| 4    | dedd43461 | release/1.14      | M      | feature/homePage   | |
| 5    | 4f76edb18 | master            | M      | release/1.14       | |
    Then The version should be ("1.14.0")

Scenario: 07 - Start a new release after master merge
	Given GitVersion configured and a master branch at version ("1.13.0")
	When I have the following events
| IDX  | SHA       | BRANCH            | ACTION | MERGE_SOURCE       | MESSAGE                                                                                                                                                                                                           |
| 0    | 300e86576 | release/1.14      | B      | master             | |
| 1    | 5884caf6c | release/1.14      | C      |                    | |
| 2    | bf6dbbd68 | feature/homepage  | B      | release/1.14       | |
| 3    | be24f9508 | feature/homepage  | C      |                    | |
| 4    | dedd43461 | release/1.14      | M      | feature/homePage   | |
| 5    | 4f76edb18 | master            | M      | release/1.14       | |
| 6    | 74810224b | release/1.15      | B      | master             | |
| 7    | 5963b7b2b | feature/loginpage | B      | release/1.14       | |
| 8    | 969fbd708 | feature/loginpage | C      |                    | |

    Then The version should be ("1.15.0-feature.loginpage.1+1")

Scenario: 08 - One named release branch with a feature branch a commit and merge back to master
	Given GitVersion configured and a master branch at version ("1.13.0")
	When I have the following events
| IDX  | SHA       | BRANCH            | ACTION | MERGE_SOURCE       | MESSAGE                                                                                                                                                                                                           |
| 0    | 300e86576 | release/promo     | B      | master             | |
| 1    | 5884caf6c | release/promo     | C      |                    | |
| 2    | bf6dbbd68 | feature/homepage  | B      | release/promo      | |
| 3    | be24f9508 | feature/homepage  | C      |                    | |
| 4    | dedd43461 | release/promo     | M      | feature/homePage   | |
| 5    | 5963b7b2b | release/promo     | T      | v1.14.0-RTM        | |
| 6    | 4f76edb18 | master            | M      | release/promo      | |
| 7    | 969fbd708 | master            | T      | v1.14.0            | |
    Then The version should be ("1.14.0")

Scenario: 09 - Hotfix and merge back to master
	Given GitVersion configured and a master branch at version ("1.13.0")
	When I have the following events
| IDX  | SHA       | BRANCH            | ACTION | MERGE_SOURCE       | MESSAGE                                                                                                                                                                                                           |
| 0    | 300e86576 | hotfix/bug1234    | B      | master             | |
| 1    | 5884caf6c | hotfix/bug1234    | C      |                    | |
| 2    | bf6dbbd68 | master            | M      | hotfix/bug1234     | |
    Then The version should be ("1.13.1")

Scenario: 10 - Two named release branch merged in one and merge back to master
	Given GitVersion configured and a master branch at version ("1.13.0")
	When I have the following events
| IDX  | SHA       | BRANCH            | ACTION | MERGE_SOURCE       | MESSAGE                                                                                                                                                                                                           |
| 0    | 300e86576 | release/promo     | B      | master             | |
| 1    | db089c66d | release/promo     | C      |                    | |
| 2    | 5884caf6c | feature/homepage  | B      | release/promo      | |
| 3    | be24f9508 | feature/homepage  | C      |                    | |
| 4    | dedd43461 | release/concours  | B      | master             | |
| 5    | 4f76edb18 | feature/concours  | B      | release/concours   | |
| 6    | 74810224b | feature/concours  | C      |                    | |
| 7    | 5963b7b2b | release/promo     | M      | feature/homePage   | |
| 8    | 969fbd708 | release/concours  | M      | feature/concours   | |
| 9    | 529047b7a | release/1.14      | B      | master             | |
| 10   | 5e7215e0f | release/1.14      | M      | release/promo      | |
| 11   | 75e8cdbcf | release/1.14      | M      | release/concours   | |
| 12   | 4c231dde6 | master            | M      | release/1.14       | |
    Then The version should be ("1.14.0")

