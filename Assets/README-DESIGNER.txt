So some simple things about using the project in the editor

1. To add a sprite that will be referenced in the script
	a.Add sprite to assets, in some folder, doesn't really matter, but keep it organized
	
	b.look in the hierarchy for the InitScripts object
	
	c.find the SpriteAtlas script attached to the object (in the inspector)
	
	d.In the Sprites list in the script, increase Size by the number of sprites that you want to add (x), and drag and drop those sprites to the bottom of the list (can expand if not expanded already), replacing the last x entries with the new ones.
	
	e. Should be good to use now in the text scripts. Can only be referenced using the sprite's exact name

2. Adding a sound file to be used through text scripts
	a.Add sound file to assets, Unity will convert it and do some importing stuff
	
	b.look in the hierarchy for the InitScripts object
	
	c.find the SoundAtlas script attached to the object (in the inspector)

	d.In the Songs(if file is for bgm)/Sounds(if file is for sfx) list in the script, increase Size by the number of files that you want to add (x), and drag and drop those files to the bottom of the list (can expand if not expanded already), replacing the last x entries with the new ones.
	
	e. Should be good to use now in the text scripts. Can only be referenced using the files exact name without extensions

3. Adding Evidences to the project (this is for creating the list of evidences that will exist in the game so that we can load them with ease)
	a.In the Resources folder, theres a file called Evidences, just follow the format in the file to add new evidences, one per line

4.Adding Naming references of characters (this one is a bit unreliable, since not all the code uses it, but basically its a list of strings that all reference the same character, and can be converted to their full name)
	a.In the resources folder, theres a file called Names, follow the format and add a new naming reference, one character per line. 

5. If any more questions, ask me