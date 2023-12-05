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

*Version 0.15.1*
- Fixed wall shaders
- Reworked game start system
- Now you can see current lap on spectated player
- Fixed PS4 drifting
- Added keyboard controls (Gamepad is heavily reccomended)
- Added controls screen

*Version 0.16*
- Added audio system
- Added music and sound volume variables
<details>
<summary>Synthetised sound effects:</summary>
<ul>
<ul>
	<li>Vehicle motor
	<li>Boost
	<li>Heal
	<li>Gravel
	<li>Ice
	<li>Item box spawn
	<li>Item box take
	<li>Item rolling
	<li>Final item
	<li>Drifting
	<li>Missile sound
	<li>Missile followed ping
	<li>Missile explosion
	<li>Out of bounds
	<li>Multiple hits with the walls
	<li>Hits with other players
	<li>Different horns for the different vehicles
	<li>Jump
	<li>Rolling
	<li>Rolling end
	<li>Laser get
	<li>Laser idle
	<li>Laser shoot
	<li>Laser hit
	<li>Race countdown
	<li>Lap
	<li>Race end
<ul>
</ul>
</details>
<details>
<summary>Composed music tracks:</summary>
<ul>
<ul>
	<li>Heavy Light
	<li>Infinite Frost
	<li>Lobby
	<li>No Barriers
<ul>
</ul>
</details>
- Added world setting to the levels:
	- Lobby
	- City
	- Desert
	- Ice
	- Water
	- Fire
	- Lightning
	- Sky
	- Illusion
- Made backgrounds and ambience for Water, Lightning and Illusion settings
- Created new level: Fake Pathway

*Version 0.16.1*
- Composed new music track:
	- Deep Dive
	- Dry Oasis
- Made backgrounds and ambience for Snow and Sand settings
- Created new level: Permafrost

*Version 0.16.2*
- Composed new music track:
	- Scorched Future
- Made background and ambience for Fire setting
- Made new levels:
	- Heat Wave
	- Shifting Highway
	
*Version 0.17*
- Fixed tunneling on players and rebounders
- Improved missile AI
- Added level thumbnails

*Version 0.17.1*
- Added new item: Shockwave
- Made background and ambience for City and Clouds settings
- Made new levels: 
	- Delta
	- Anticyclone
	- Converge
- Composed new music track: Hyperdrive
- Added sound effects:
	- Death
	- ItemHit
	- Shockwave
	- Shield
	- FireFloor
	
*Version 0.18*
- Buffed Laser: Now autofires when a player is in the line of fire
	- Nerfed aim duration to compensate; Now it lasts 0.3 seconds before firing
- Traps now can be thrown forward
- Rebounders now can be thrown backwards
- Added new item: Equalizer

*Version 0.18.1*
- Added controller rumble support

*Version 0.19*
- Made background and ambience for Lobby
- Added new illusion level: Deadlock
- Revamped controls to work with Unity's new Input System
- Implemented a locked camera system
- Made a tutorial

*Version 0.19.1*
- Fixed a bug where the player wouldn't die off track
- Fixed a bug where the traps wouldn't work on client side

*Version 0.20*
- Added spectator mode when entering a lobby with a race in progress
- Remade the player interface with shaders
- Made a new main menu with new functionalities:
	- Host game: Host a game
	- Direct connection: Connect to the chosen IP Address
	- Find servers: Search for servers in the LAN
	- Exit game: Exit the game
- Alternative strafing now inverts strafing controls
- Reworked checkpoint system

*Version 0.21*
- Added account and online data saving system
- Added posibility to activate camera rotation
- Now rolling slows your speed, and gives it back once finished
- Reworked jumping physics
- Reworked ice physics
- Added new lightning level: Helix
- Added fan props and fan physics
- Added bouncy walls
- Added hit announcements
- Added record system and display
- Added leaderboard pannel in the lobby
- Added post-processing effects
- Now boosting deals aditional contact damage, and ignores collision if it kills the receiver

##ALPHA

*Version 1.0*
- Added full controller support for menuing
- Updated options menu
- Lots of bugfixes

*Version 1.1*
- Fixed a bug with specific username formattings