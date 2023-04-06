# GradWing

*Version 0.3*
- Implemented multiplayer via Mirror

*Version 0.4*
- Created personalized connection screen
- Tweaked synchronization
- Added jump pads

*Version 0.5*
- Implemented personalized UI:
  - Username display
  - Current item display
  - Health bar display
- Added healing zones
- Added item boxes

*Version 0.6*
- Implemented item box functionality and weight values
- Added _Shield_ item
- Added _Jump_ item
- Added _Trap_ item
- Added _Rebounder_ item

*Version 0.6.1*
- Added _Laser_ item

*Version 0.7*
- Created sample level
- Added boosters
- Added gravel
- Added damaging floor

*Version 0.8*
- Now out of bounds is checked with a floor texture
- Changed object synchronization system to client-sided simulations

*Version 0.9*
- Added checkpoint system
- Added lap system
- Added user interface ingame

*Version 0.9.1*
- Fixed checkpoint functionality
- Added HP and current object to interface

*Version 0.10*
- Added statistic system:
	- Velocity increments max speed
	- Acceleration increments vehicle acceleration
	- Weight:
		- Increased max HP
		- Increased contact damage against players
		- Increased knockback against players
		- Reduced self damage from contact with players
		- Reduced self knockback from contact with players
	- Handling:
		- Increased rotation speed
		- Increased max speed limit break while drifting
		- Increased air handling
- Added Lobby

*Version 0.10.1*
- Added level vote system for lobby
- Minor upgrades in interface

*Version 0.10.2*
- Added support for Playstation controller

*Version 0.10.3*
- Added different control system:
	- Classic mode: Right trigger aims wing to the right, left trigger aims wing to the left
	- Alternative mode: Right trigger closes curves, left trigger opens them
	
*Version 0.11*
- Added rudimentary lap and placement system
- Added new items:
	- Boost
	- Missile

*Version 0.11.1*
- Added diferent item weights for different placements:

|Placement|Shield|Jump|Trap|Rebounder|Laser|Boost|Missile|
|---------|------|----|----|---------|-----|-----|-------|
|1st      |25%   |0%  |45% |20%      |0%   |10%  |0%     |
|2nd      |20%   |5%  |30% |25%      |10%  |10%  |0%     |
|3rd      |30%   |5%  |20% |20%      |10%  |15%  |0%     |
|4th      |30%   |10% |10% |20%      |10%  |15%  |5%     |
|5th      |10%   |15% |10% |20%      |20%  |20%  |5%     |
|6th      |10%   |20% |10% |15%      |20%  |20%  |5%     |
|7th      |0%    |25% |0%  |30%      |30%  |20%  |10%    |
|8th      |0%    |30% |0%  |30%      |30%  |20%  |5%     |

*Version 0.12*
- Added stylistic trails
- Created new level: Figure ∞
- Fixed camera movement in boost panels
- Added magnetic ground
- Added minimap system

*Version 0.13*
- Added bloom
- Now player trails change colors depending on state and speed
- Created new level: Volute

*Version 0.14*
- Added treadmill
- Added results screen and finished the game loop

*Version 0.14.1*
- Added race start countdown

*Version 0.14.2*
- Added spectator mode
- Now Boost protects from slowing down over gravel
- Now Boost makes the player deal more and receive less contact damage
- Added VFX to boost
- Added boost at the start of the race (Lerp(1 sec left - start))
- Now missiles follow first player ignoring finished ones
- Stilyzed level borders

*Version 0.15*
- Added vehicle type selection
- Added dynamic vehicle color based on stats