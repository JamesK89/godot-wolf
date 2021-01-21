Port of Wolfenstein 3D to the Godot engine

Buy a copy of Wolfenstein 3D, or download the shareware version, and place the WL1/WL6 data files in a subfolder called Data.
Be sure to double check game.cfg if you're using the registered version of Wolf3D and change the extensions to WL6.

It is still very work in progress and not exactly elegant but it works for now.
This is also a learning project for me in regard to the Godot engine.

Contributions are welcome!

Some things that are needed in no particular order:
* Digitized sound effects are available but some of them have a "beep" on the end. (Please look at the VSWAP.cs wolfread library and see if you can figure it out. I'm probably determining the chunk length wrong.)
* Adlib sound effects are absent which are basically short MIDI sequences. It'll need to be figured out how to synthesize those into digital samples for playback in Godot.
* There is no music. Again, we'll need to figure out how to synthesize those into digital samples for playback in Godot.
* The Level class is used as a place holder for a lot of enumerations, so some refactoring needs to be done.
* Some enumerations and hard coded values (e.g. Assets.DigitalSoundList) could be moved to be configurable into the game.cfg to allow for more flexibility and game modding.
* There is no heads up display.
* There are no weapons.
* There are no stats (health, ammo, keys, etc..) for the player.
* No AI characters are implemented yet.
* World geometry generation could be much better to reduce triangle count.