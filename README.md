# README #

### To Do ###
* Timer Display, Round win count display (not as ongui)
* Specials
* Character select screen. First three characters, Sweep, Command Grab, Armor.
* Creating some simple sprites
* Use ReWired for inputs, set up so players can select side, start testing with mobile touchscreen joystick

### Bugs/QOL ###
## TO DO BEFORE CANADA CUP ##
* Fix score bug. I think it resets on throw or timeout. Make it declare a winner after 5 or 10 rounds
* Main menu, choose number of rounds (5, 10, 20, int max), maybe test inputs to see who's p1 and p2
* Special, meter stays between rounds, 1 gauge per blocked attack, 3 gauges to use special, it's a sweep with more range than the normal. Not sure how much recovery. Maybe also a way to disable Special

### Fixed stuff ###
* Make hitbox color change for both players, change when in startup/recovery, needs more feedback for what's happening, blockstun too - Done
* Limit sides of screen a bit more - Done
* Investigate bug where sometimes a held direction isn't registered after changing inputs fast - Possible hitbox bug, seems fine on sticks and kb, needs more testing.
* Fix and increase throw tech pushback, show visual feedback too. - Done. No feedback besides pushback, but maybe it's enough
* Tech throw automatically on throw startup. - Done
* I think slightly reducing walk back speed would be worth trying. - Done but needs to be tested in matches to see how it is. maybe make it modifyable somehow
