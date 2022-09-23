Port of Wolfenstein 3D to the Godot engine

Buy a copy of Wolfenstein 3D, or download the shareware version, and place the WL1/WL6 data files in a subfolder called Data.
Be sure to double check game.cfg if you're using the registered version of Wolf3D and change the extensions to WL6.

It is still very work in progress and not exactly elegant but it works for now.
This is also a learning project for me in regard to the Godot engine.

Contributions are welcome!

Some things that are needed in no particular order:
* Digitized sound effects are available but some of them have a "beep" on the end. (Please look at the VSWAP.cs wolfread library and see if you can figure it out. I'm probably determining the chunk length wrong.)
* Adlib sound effects are absent which are basically short MIDI sequences. I plan to use the added Nuked-OPL3-dotnet submodule to digitize the sound effects but haven't really worked on it yet.
* There is no music. I plan to use the added Nuked-OPL3-dotnet submodule to digitize the music but haven't really worked on it yet.
* Some enumerations and hard coded values (e.g. Assets.DigitalSoundList) could be moved to be configurable into the game.cfg to allow for more flexibility and game modding.
* There is no heads up display.
* There are no weapons.
* There are no stats (health, ammo, keys, etc..) for the player.
* AI is very basic at the moment; only the basic guard is implemented and they just chase you around. It is basically a port of the AI from the original C code.
* World geometry generation could be much better to reduce triangle count.
