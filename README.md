Port of Wolfenstein 3D to the Godot engine

Buy a copy of Wolfenstein 3D, or download the shareware version, and place the WL1/WL6 data files in a subfolder called Data.
Be sure to double check game.cfg if you're using the registered version of Wolf3D and change the extensions to WL6.

It is still very work in progress and not exactly elegant but it works for now.
This is also a learning project for me in regard to the Godot engine.

Contributions are welcome!

Some things that are needed in no particular order:
* There is not yet audio nor any support for loading audio in the wolfread library.
* Secret doors (aka Push blocks) are not implemented yet.
* Regular doors do not check if there is something inside their "cell" before automatically closing.
* Pickups are non-functional at the moment.
* There is no heads up display.
* No AI characters are implemented yet.
* World geometry generation could be much better to reduce triangle count.
