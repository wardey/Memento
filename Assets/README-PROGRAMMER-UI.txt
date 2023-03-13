HAI HAI, This is a few notes for people trying to add to the code/edit the project in unity for implementation purposes

1. To make UI changes, if you want to see the particular ui that youre working on, you have to disable all the other ones below it in the hierarchy, since they overlap it. I don't really have a a good solution for this, just disabling them is fine for now i guess.

ex: if you want to work on CourtRoomUI, you'll find the CourtRoomUI object in the hierarchy, I recommend disabling everything else under "Game" and disabling "Menus" so taht you only see the CourtRoomUI in the editor

This is a bit messy, I'll clean it up later

2.When you run the project, everything that you just disabled has to be reenabled.

NOTE: To run the game normally without errors, since I didn't really use null checks.
You need to have the following things enabled(E)/disabled(D) unless we've removed them from the implementation. You'll find these things in the Hierarchy window
Everything at the top level needs to be enabled
-logic frenzy should be removed soon,
-dont touch debate normally
Under the "Canvas" object:
	-Game (E)
		-everything under Game must be enabled
	-Menus (E)
		-StartMenu(E)
		-PauseMenu(D)

3.Code for UI usually is named the same as the UI object that hosts it, also it shows up in the inspector, so you cna find that there
	-if youre looking for gamecontroller.cs, its attached to Game

4.A lot fo the UI objects aren't actually being used according to the current implementation, but I would leave them for now until we decide to do something about it.

5.Try to save the editor with the things enabled, 

6. Quick Rundown of the Objects in the hierarchy, as of when I wrote this (only noting the high level objects, not their children)
-InitScripts: object for a lot of script initialization, the various resource atlas's are attached to it, as well as the script parser. Atlas' are for adding assets/script referenced resources to the project
-SFPlayer: it's just an object with an audio player attached, used for playing sound effects
-BGMPlayer: same as above, but for background music
-Logic Frenzy: (deprecated), the implementation for the bullet hell mode idea
-Debate: The object used for debate/peacemaker(called somehting else now i thing) modes, includes the 3d environment and character circle and the camera for those modes, must be enabled. (You probably wont need to touch this)
-Canvas: parent for all the UI stuff, 
	-Game: parent for all the in game UI, gamecontroller is attached to this
		-LogicFrenzyUI: Deprecated, Ui for logic frenzy
		-the rest are quite self explanatory, they usually correspond to the mode that they're for, some are deprecated
		-DebateDialogueUI: The UI used for debate dialogue sections, only triggered through variants of debate mode, very similar to dialogueUI, but not the same
		-DialogueUI: the normal VN UI, what you see at the beginning
		-SelectionUI: This one's a bit special, this is ntoa  standalone UI for a mode, it's used for player selection during Dialogue based modes.
	-Menus: The parent for all the menu objects, basically just the starting menu, options, setting, those kind of stuff, not all implemented
		-PauseMenu: the pause menu..., you cna press esc to bring this up in game, only works in regular vn mode though i think
		-StartMenu: the menu you see at the start, 

7.If you have question about gamecontroller.cs or any other scripts, just ask me, theres too much to write, I'm not writing a whole documentation for this
