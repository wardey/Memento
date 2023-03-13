using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour {   //game control class

    //keeps track of game data at run-time, maybe should just leave it to storyProgress instead
    //information for main story file progress
    public int fileLength;
    public static SaveFile.Mode currMode;
    public VNModeData currVNData;
    public CourtRoomModeData currCourtRoomData;
    public InvestigationModeData currInvestigationData;
    public FreeTimeModeData currFreeTimeData;
    public LogicFrenzyData currLogicFrenzyData;
    public PeaceMakerData currPeaceMakerData;

    public static List<Evidence> evidenceBag = new List<Evidence>();    //instead of having multiple copies, all refer to master bag, used for all modes, also more consistent
    public List<Clue> clueBag = new List<Clue>();
    //all major uis that is controlled by this particular script
    public GameObject dialogueUIObject; //the actual object for dialogueUI
    public GameObject selectionUIObject;    //for options
    public GameObject courtRoomUIObject;    //for courtroom mode
    public GameObject debateDialogueUIObject; //for debate VN
    public GameObject investigationUIObject;    //for investigation mode
    public GameObject freeTimeUIObject; //for free time
    public GameObject logicFrenzyUIObject; // for logic frenzy
    public GameObject peaceMakerUIObject;

    public GameObject pauseMenu;    //basically the same as main menu, but with a save button
    public GameObject startMenu;

    private List<string> fileContents;  //lines of the current file

    //separate set of dialogue information for displaying "side" files, mostly when player want more information on something


    //flags for interpreting commands, mostly for following script, when to pause.
    private bool skipUntilCont; //after reading exit command, set to true, and skip all commands until after the first cont
    private bool skipUntilExit; //after failing a cond, set to true, skip all commands until after the first exit
    public bool skipUntilPresent;
    private bool waitingforClick;
    private bool waitingforTransition;
    private bool waitingforChoose;
    private bool waitforpresent;    
    private bool debateVNMode;
    private bool dedeuceMode;

    public bool inDebateVN()
    {
        return debateVNMode;
    }

    public bool inDedeuce()
    {
        return dedeuceMode;
    }

    // Use this for initialization
    void Start() {
        storyProgress.instance.onLoadGame += init;
        storyProgress.instance.onNewGame += newGame;
        storyProgress.instance.onSaveGame += updateData;
        DialogueUI.instance.onEffectFinish += effectFinish;
        DialogueUI.instance.onClickDialogue += clickDialogue;
        SelectionUI.instance.onChooseOption += chooseOption;
        CourtRoomUI.instance.onRoundEnd += setNewFile;
        PeaceMakerUI.instance.onRoundEnd += setNewFile;
        DebateDialogueUI.instance.onClickDialogue += clickDialogue;
        InvestigationUI.instance.onRoundEnd += setNewFile;
        FreeTimeUI.instance.onRoundEnd += setNewFile;
        LogicFrenzyController.instance.clickDialogue += LFclickDialogue;
        LogicFrenzyController.instance.OnRoundEnd += setNewFile;
        skipUntilCont = false;
        skipUntilExit = false;
        skipUntilPresent = false;
        waitingforClick = false;
        waitingforChoose = false;
        waitforpresent = false;
        waitingforTransition = false;
    }

    void OnDestroy()
    {
        storyProgress.instance.onLoadGame -= init;
        storyProgress.instance.onNewGame -= newGame;
        storyProgress.instance.onSaveGame -= updateData;
        DialogueUI.instance.onEffectFinish -= effectFinish;
        DialogueUI.instance.onClickDialogue -= clickDialogue;
        SelectionUI.instance.onChooseOption -= chooseOption;
        CourtRoomUI.instance.onRoundEnd -= setNewFile;
        PeaceMakerUI.instance.onRoundEnd -= setNewFile;
        DebateDialogueUI.instance.onClickDialogue -= clickDialogue;
        InvestigationUI.instance.onRoundEnd -= setNewFile;
        FreeTimeUI.instance.onRoundEnd -= setNewFile;
        LogicFrenzyController.instance.clickDialogue -= LFclickDialogue;
        LogicFrenzyController.instance.OnRoundEnd -= setNewFile;
    }

    // Update is called once per frame
    void Update() {
        //on escape key, opens "pause menu" (doesn't actually pause), escape again to exit
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (startMenu.activeInHierarchy)
            {
                return;
            }
            if (pauseMenu.activeInHierarchy)
            {
                Time.timeScale = 1.0f;
                BGMController.instance.unPauseSong();
                pauseMenu.GetComponent<MainMenuUI>().onBack();
                pauseMenu.SetActive(false);
            }
            else
            {
                Time.timeScale = 0.0f;
                BGMController.instance.pauseSong();
                pauseMenu.SetActive(true);
            }
        }
    }

    public void loadScene()     //after all information is loaded from savefile/new game, etc..., decide on what mode it should be in
                                //and load appropriate UIs, try to not leave layers peeking through
    {
        //DialogueParser.instance.LoadDialogue(currVNData);
    }

    //flags will just remain in story progress, and can still be accessed from controller
    public void updateData()        //on save button, storyprogress will trigger onSave to get updated data from game controller
    {
        storyProgress.instance.mainVNData = currVNData;
        storyProgress.instance.courtRoomData = currCourtRoomData;
        storyProgress.instance.investigationData = currInvestigationData;
        storyProgress.instance.freeTimeData = currFreeTimeData;
        storyProgress.instance.currMode = currMode;
        //storyProgress.instance.evidenceBag = evidenceBag;
    }

    public SaveFile.Mode getMode()
    {
        return currMode;
    }

    public void init()          //on load button, story progress will trigger onLoad to tell game controller to get new data, and play from currLine
    {   //sets all the backgrounds, music, character sprites, etc...

        currVNData = storyProgress.instance.mainVNData;
        currCourtRoomData = storyProgress.instance.courtRoomData;
        currInvestigationData = storyProgress.instance.investigationData;
        currFreeTimeData = storyProgress.instance.freeTimeData;
        currMode = storyProgress.instance.currMode;
        //evidenceBag = storyProgress.instance.evidenceBag;

        loadScene();      //do something based on the current mode
        //closeAllUI(); //close all unneeded ui

        /*
        DialogueUI.instance.changeChar(currChar);
        DialogueUI.instance.changeSpeaker(currSpeaker);
        BGMController.instance.playSong(currBGM);
        BackGroundController.instance.setBackground(currBG);
        ForegroundController.instance.setForeground(currFG);

        DialogueParser.instance.LoadDialogue(currFile);
        fileContents = DialogueParser.instance.getLines();
        fileLength = fileContents.Count;
        //if load during an option, need to close it, also any other UIs that may be open at the time, should write a function to close all later
        if (selectionUIObject.activeInHierarchy)    
        {
            selectionUIObject.SetActive(false);
        }
        readCommand();
        */
    }

    public void newGame()
    {
        //starts from default file, assuming it starts in VN mode 
        currVNData = storyProgress.instance.mainVNData;
        DialogueParser.instance.LoadDialogue(currVNData.File);
        fileContents = DialogueParser.instance.getLines();
        fileLength = fileContents.Count;
        readCommand();      //for reading commands only while in vn mode 
    }

    public void setNewFile(string filename, string nextmode) //sets the new file to be read when the end of a file is reached
    {
        Debug.Log("reading file : " + filename);
        //read the lines in first anyways, then decide what to do with them
        DialogueParser.instance.LoadDialogue(filename);
        fileContents = DialogueParser.instance.getLines();
        fileLength = fileContents.Count;
        //called on next keyword, so filename and next mode is already given and decided before going into the next file
        //case by case based on mode

        if (nextmode == "VN")
        {
            VNModeData tmp = new VNModeData();
            tmp.File = filename;
            tmp.lineNumber = 1;
            currVNData = tmp;
            currMode = SaveFile.Mode.VN;
            debateVNMode = false;
            dialogueUIObject.SetActive(true);
            readCommand();
        }
        else if (nextmode == "debate") 
        {
            CourtRoomModeData tmp = new CourtRoomModeData();
            tmp.lineNumber = 1;
            currCourtRoomData = tmp;
            currMode = SaveFile.Mode.CourtRoom;
                    dedeuceMode = false;
            courtRoomUIObject.SetActive(true);
            debate();
        }
        else if(nextmode == "dedeuce")  //dedeuce and pacemaker same thing
        {
            PeaceMakerData tmp = new PeaceMakerData();
            tmp.lineNumber = 1;
            currPeaceMakerData = tmp;
            currMode = SaveFile.Mode.PeaceMaker;
            dedeuceMode = true;
            peaceMakerUIObject.SetActive(true);
            peacemaker();
        }
        else if (nextmode == "investigation")
        {
            InvestigationModeData tmp = new InvestigationModeData();
            tmp.lineNumber = 1;
            currInvestigationData = tmp;
            currMode = SaveFile.Mode.Investigation;
            investigationUIObject.SetActive(true);
            investigate();
        }
        else if (nextmode == "freetime")
        {
            FreeTimeModeData tmp = new FreeTimeModeData();
            tmp.lineNumber = 1;
            currFreeTimeData = tmp;
            currMode = SaveFile.Mode.FreeTime;
            freeTimeUIObject.SetActive(true);
            freetime();
        }
        //trigger logic frenzy mode, should give control to the logic frenzy controller? or keep in this one
        //better control here, but... logic frenzy needs to read file real time. should be fine to keep control in same class
        else if (nextmode == "logicfrenzy")
        {
            //does logic frenzy need a data? is there anything to preprocess... probably
            LogicFrenzyData tmp = new LogicFrenzyData();
            tmp.lineNumber = 1;
            tmp.filename = filename;
            LogicFrenzyController.instance.logicFrenzyCamera.enabled = true;//should be disabled by logic frenzy controller after the mode is over
            currLogicFrenzyData = tmp;
            currMode = SaveFile.Mode.LogicFrenzy;
            //need to change cameras too.
            logicFrenzyUIObject.SetActive(true);
            logicfrenzy();
        }
        else if (nextmode == "logicfrenzydialogue")//dialogue files that are triggered in logic frenzy mode, so needs to be played in logic frenzy's UI
        {
            //just get the file and read it, should return afterwards and have no effect on rest of the game
            currLogicFrenzyData.lineNumber = 1;
            //handles keywords for dialogue related things in logic frenzy, but limited support, and must have return instead of end for termination
            logicfrenzy();
        }
        else if (nextmode == "debateVN")//do same as regular VN, but just flip an internal switch that says its in debate mode
        {
            Debug.Log("starting a debateVN");
            VNModeData tmp = new VNModeData();
            tmp.File = filename;
            tmp.lineNumber = 1;
            currVNData = tmp;
            //the mode could be anything, but since theres no specific debatevn mode, we'll just leave it as is, whether courtroom or dedeuce
            //currMode = SaveFile.Mode.CourtRoom;
            //dialogueUIObject.SetActive(true);
            //after making a debate VN UI, set that to be true and use that one instead
            debateDialogueUIObject.SetActive(true);
            debateVNMode = true;
            readCommand();
        }
    }

    /*shouldnt need this functioncality anymore due to use of next keyword, no need to return to a location in a particular file
    public void setTempFile(string filename)       //for side dialogue that needs to come back to the same spot in the main dialogue
    {
        
    }
    */

    //when dialogue ui is clicked after dialogue appears, continue reading commands
    public void clickDialogue() //vn mode method    (dialogue ui)
    {
        //the addition of waiting for transition caused infinite loops/recursion not sure why
        if (waitingforTransition)
        {
            Debug.Log("not waiting for transition anymore");
            waitingforTransition = false;
            currVNData.lineNumber += 1;
            readCommand();
            return;
        }
        if (waitingforClick)
        {
            waitingforClick = false;
            currVNData.lineNumber += 1;
            readCommand();
            return;
        }
        if (waitforpresent)
        {
            waitforpresent = false;
            currVNData.lineNumber += 1;
            readCommand();
            return;
        }
    }

    public void LFclickDialogue()
    {
        logicfrenzy();
    }

    public void effectFinish()  //vn mode method    (dialogue ui)
    {
        currVNData.lineNumber += 1;
        readCommand();
    }

    //when an option is chose by player
    public void chooseOption(int index) //vn mode method    (dialogue ui)
    {
        if (waitingforChoose)
        {
            waitingforChoose = false;
            setFlags(index);
            currVNData.lineNumber += 1;
            readCommand();
        }
    }
    
    void setFlags(int index)    //the option at index was chosen
    {
        string[] line = fileContents[currVNData.lineNumber].Split('=');
        for (int i = 1; i < line.Length; i++)
        {
            if(i == index)
            {
                storyProgress.instance.flags[line[i]] = 1;
            }
            else
            {
                storyProgress.instance.flags[line[i]] = 0;
            }
        }
    }

    void backtoStartMenu()
    {
        startMenu.SetActive(true);
        courtRoomUIObject.SetActive(false);
        DialogueUI.instance.clearAll();
    }

    //This method is the preprocessing for "VN" mode, which is just progressive dialogue
    //for debate-vn could just pass in another argument that indicates that its for debate and not normal vn, but would have to change it to fit the debate ui anyways.
    //need to change so that data is captured in VNmode class, and set flag for current mode, need more variables
    void readCommand() //reads in command at linenumber and pauses at either dialogue line or options line to wait for click
    {                                //some commands will continue reading after being executed, some will wait for a click, some will skip to a certain part
        //now uses an iterative approach, but keywords that need to wait for input will break out of the loop and wait for the corresponding function to call readCommand again
        while (true) {
            if (fileContents[currVNData.lineNumber] == "")   //empty line, skip
            {
                Debug.Log("reading command at line " + currVNData.lineNumber.ToString());
                Debug.Log(fileContents[currVNData.lineNumber]);
                currVNData.lineNumber += 1;
            }
            Debug.Log("reading command at line " + currVNData.lineNumber.ToString());
            Debug.Log(fileContents[currVNData.lineNumber]);
            string[] line = fileContents[currVNData.lineNumber].Split('=');
            string keyword = line[0];
            if (line.Length == 1)        //keywords exit, cont, or a dialogue line, i.e. keywords with no parameters
            {
                if (keyword == "exit")
                {
                    if (skipUntilExit) //for escaping cond
                    {
                        skipUntilExit = false;
                    }
                    else
                    {
                        skipUntilCont = true;  //actually an exit command, so skip until cont
                    }
                    currVNData.lineNumber += 1;
                }
                else if(keyword == "choiceredo")
                {
                    currVNData.lineNumber = currVNData.lastChoice;
                }
                else if(keyword == "presentsuccess")
                {
                    if(!skipUntilPresent)  //the player hit the present prompt, tried, and got it wrong, so go back
                    {
                        currVNData.lineNumber = currVNData.presentprompt;
                        skipUntilPresent = false;
                    }
                    else
                    {
                        skipUntilPresent = false;
                        currVNData.lineNumber += 1;
                    }
                }
                else if (keyword == "cont")
                {
                    skipUntilCont = false;
                    //continue reading from next line
                    currVNData.lineNumber += 1;
                }
                else if (keyword == "return")
                {
                    //for debate vn, should transition back to the character that started the dialogue sequence and turn on debate UI again
                    if (!skipUntilExit && !skipUntilCont)
                    {
                        if (debateVNMode)
                        {
                            //after a debate-vn segment ends, we must return to the current debate mode that instigated the debate-vn.
                            //since the information is still saved, we just need to call a refresh to update all the visuals.
                            DebateDialogueUI.instance.clearAll();
                            debateDialogueUIObject.SetActive(false);
                            if (dedeuceMode)
                            {
                                PeaceMakerUI.instance.enableEverything();
                                PeaceMakerUI.instance.postDebateVN();
                            }
                            else
                            {
                                CourtRoomUI.instance.enableEverything();
                                CourtRoomUI.instance.postDeabteVN();
                                CourtRoomUI.instance.refreshCircle();
                            }
                        }
                        DialogueUI.instance.clearAll();
                        dialogueUIObject.SetActive(false);
                        //depending on the current mode as indicated by which object is active, call the appropriate afterinteract methodk
                        if (investigationUIObject.activeInHierarchy)
                        {
                            if (InvestigationUI.instance.allComplete) InvestigationUI.instance.PlayAllComplete();   //same as endround
                            else InvestigationUI.instance.afterInteract();
                        }
                        if (freeTimeUIObject.activeInHierarchy)
                        {
                            FreeTimeUI.instance.afterInteract();
                        }
                        return;
                    }
                    else
                    {
                        currVNData.lineNumber += 1;
                    }
                }
                else if (keyword == "giveup")
                {
                    if (!skipUntilExit && !skipUntilCont)
                    {
                        backtoStartMenu();
                        break;
                    }
                    else
                    {
                        currVNData.lineNumber += 1;
                    }
                }
                else
                {
                    //actual dialogue, read until second " ?
                    //need to wait for click before going to next command
                    //both flags to skip are down
                    if (!skipUntilExit && !skipUntilCont && !skipUntilPresent)
                    {
                        //the dialogue line could be "say" or (thought), so need to distinguish between double quotes and brackets
                        if(keyword[0] == '"')// || keyword[0] == '“')
                        {
                            if (debateVNMode)
                            {
                                waitingforClick = true;
                                //make a debate VN script
                                //DebateDialogueUI.instance.changeSpeaker(currVNData.Speaker);
                                DebateDialogueUI.instance.displayLine(keyword.Substring(1, keyword.Length - 2));
                                break;
                            }
                            //use the current speaker
                            waitingforClick = true;
                            DialogueUI.instance.changeSpeaker(currVNData.Speaker);
                            DialogueUI.instance.displayLine(keyword.Substring(1, keyword.Length - 2));    //keyword is a dialogue line here
                            break;
                            //check is debateVNMode, then use the debate UI instead
                        }
                        else if(keyword[0] == '(')
                        {
                            if (debateVNMode)
                            {
                                waitingforClick = true;
                                DebateDialogueUI.instance.changeSpeaker(currVNData.MC);
                                DebateDialogueUI.instance.displayThought(keyword.Substring(1, keyword.Length - 2));
                                break;
                            }
                            //use mc as speaker(thinker)
                            waitingforClick = true;
                            DialogueUI.instance.changeSpeaker(currVNData.MC);
                            DialogueUI.instance.displayThought(keyword.Substring(1, keyword.Length - 2));    //keyword is a thought line here
                            break;
                            //check is debateVNMode, then use the debate UI instead
                        }
                    }
                    else        //one of the flags are up, so skip
                    {
                        currVNData.lineNumber += 1;
                    }
                }
            }
            else if (skipUntilExit || skipUntilCont)      //skips the keywords below if either exit or cont flags are up
            {
                currVNData.lineNumber += 1;
            }
            else                        //all other keywords with some kind of argument
            {
                if (keyword == "bg") //change background picture
                {
                    currVNData.BG = line[1];
                    BackGroundController.instance.setBackground(currVNData.BG);
                    currVNData.lineNumber += 1;
                }
                else if (keyword == "fg")    //set/unset foreground image
                {
                    currVNData.FG = line[1];
                    ForegroundController.instance.setForeground(currVNData.FG);
                    currVNData.lineNumber += 1;
                }
                else if (keyword == "bgm")    //change bgm
                {
                    currVNData.BGM = line[1];
                    BGMController.instance.playSong(currVNData.BGM);
                    currVNData.lineNumber += 1;
                }
                else if (keyword == "c")  //change characters being displayed in dialogue ui, and speaker, different rules...
                {
                    //format c=name=emote=position(optional)
                    if (line.Length == 3)    //no position argument, just add it in and shift
                    {
                        currVNData.Speaker = line[1];
                        currVNData.CharList = DialogueUI.instance.addChar(line[1] + "_" + line[2]);   //all logic done in dialogue ui class
                        DialogueUI.instance.changeSpeaker(currVNData.Speaker);
                    }
                    else if (line.Length == 4)   //position argument given, strictly place char in position
                    {
                        currVNData.Speaker = line[1];
                        currVNData.CharList = DialogueUI.instance.setChar(line[1] + "_" + line[2], line[3]);   //all logic done in dialogue ui class
                        DialogueUI.instance.changeSpeaker(currVNData.Speaker);
                    }
                    storyProgress.instance.mostRecentSprite[NameAtlas.instance.getName(line[1])] = line[1] + "_" + line[2];
                    currVNData.lineNumber += 1;
                }
                else if(keyword == "actors")//debate-vn only
                {
                    if(line[1] == "carryover")
                    {
                        //get the information from the existing debate mode
                        if (dedeuceMode)
                        {
                            DebateDialogueUI.instance.allchars = new List<string>(PeaceMakerUI.instance.roundData.allchars);
                            DebateDialogueUI.instance.characters = new List<string>(PeaceMakerUI.instance.roundData.characters);
                        }
                        else
                        {
                            DebateDialogueUI.instance.allchars = new List<string>(CourtRoomUI.instance.roundData.allchars);
                            DebateDialogueUI.instance.characters = new List<string>(CourtRoomUI.instance.roundData.characters);
                        }
                    }
                    else
                    {
                        DebateDialogueUI.instance.allchars = new List<string>(new string[15]);
                        for (int i = 1; i < line.Length; i++)
                        {
                            DebateDialogueUI.instance.allchars[(2 * (i - 1)) % 15] = NameAtlas.instance.getName(line[i]);
                        }
                    }
                    currVNData.lineNumber += 1;
                }
                else if (keyword == "emotes")
                {
                    if(line[1] == "carryover")
                    {
                        //the actors carryover are using the same list as a reference, but this one is a new one
                        if (dedeuceMode)
                        {
                            DebateDialogueUI.instance.allSprites = new List<string>(PeaceMakerUI.instance.roundData.allSprites);
                            DebateDialogueUI.instance.sprites = new List<string>(PeaceMakerUI.instance.roundData.sprites);
                        }
                        else
                        {
                            DebateDialogueUI.instance.allSprites = new List<string>(CourtRoomUI.instance.roundData.allSprites);
                            DebateDialogueUI.instance.sprites = new List<string>(CourtRoomUI.instance.roundData.sprites);
                        }
                        //this line is causing it to go into an infinite loop and freezing the program
                        //DebateDialogueUI.instance.repopulate(0, DebateDialogueUI.instance.allchars[0]);
                    }
                    else
                    {
                        DebateDialogueUI.instance.allSprites = new List<string>(new string[15]);
                        for (int i = 1; i < line.Length; i++)
                        {
                            DebateDialogueUI.instance.allSprites[(2 * (i - 1)) % 15] = line[i];
                        }
                    }
                    currVNData.lineNumber += 1;
                }
                else if(keyword == "mv")
                {
                    //format mv=char=emote=transition...
                    //keyword for transitions, if called, do the transition and after its done, call back to this method
                    if(line.Length == 3)
                    {
                        Transition currtransition = new Transition("defaultTransition");
                        DebateDialogueUI.instance.transition(line[1], line[2], currtransition);
                    }
                    else if(line.Length == 4)
                    {
                        //preset for entire transition
                        Transition currtransition = new Transition(line[3]);
                        DebateDialogueUI.instance.transition(line[1], line[2], currtransition);
                    }
                    else if(line.Length == 6)
                    {
                        //3 parts are given for transition
                        Transition currtransition = new Transition(line[3], line[4], line[5]);
                        DebateDialogueUI.instance.transition(line[1], line[2], currtransition);
                    }
                    waitingforTransition = true;
                    break;
                }
                else if (keyword == "t") //changes speaker, possible argument for changing emote
                {
                    if (line.Length == 2)    //no sprite change
                    {
                        if(line[1] == "mc")
                        {
                            currVNData.Speaker = currVNData.MC;
                            DialogueUI.instance.changeSpeaker(currVNData.MC);
                        }
                        else
                        {
                            currVNData.Speaker = line[1];
                            if (debateVNMode)
                                DebateDialogueUI.instance.changeSpeaker(currVNData.Speaker);
                            else
                                DialogueUI.instance.changeSpeaker(currVNData.Speaker);
                        }
                    }
                    else if (line.Length == 3)   //sprite change for current char, only gives emotion for same character
                    {
                        currVNData.Speaker = line[1];
                        if (debateVNMode)
                            DebateDialogueUI.instance.changeSpeaker(currVNData.Speaker, line[2]);
                        else
                            DialogueUI.instance.changeSpeaker(currVNData.Speaker, line[2]);
                    }
                    currVNData.lineNumber += 1;
                }
                else if(keyword == "e")
                {
                    //format e=emote
                    //changes the sprite for the current speaker
                    if (debateVNMode)
                    {
                        //DebateDialogueUI.instance.changeSpeakerSprite(currVNData.Speaker, line[1]);
                        Debug.Log("change emote in debate-vn");
                    }
                    else
                    {
                        DialogueUI.instance.changeSpeaker(currVNData.Speaker, line[1]);
                    }
                    currVNData.lineNumber += 1;
                }
                else if(keyword == "presentprompt")
                {
                    //presentprompt=statement=evidence
                    if (debateVNMode)
                    {
                        currVNData.presentprompt = currVNData.lineNumber;                    
                        DebateDialogueUI.instance.presentprompt(line[1], line[2]);
                        waitforpresent = true;  //skip until present will be decided based on the answer
                        break;
                    }
                }
                else if (keyword == "rm")    //removing a character from screen and adjusting others positions
                {
                    DialogueUI.instance.removeChar(line[1]);
                    currVNData.lineNumber += 1;
                }
                //remember to use nameatlas once its setup for character names (line[1])
                else if (keyword == "mc")    //change mc to a character, whoever it is, no sprite changes
                {
                    currVNData.MC = line[1];
                    currVNData.lineNumber += 1;
                }
                else if (keyword == "cfx")   //character effects
                {
                    if (debateVNMode)
                    {
                        //do something for this
                    }
                    DialogueUI.instance.charEffect(line[1], line[2]);
                    //should continue to next line after effect plays, called by effectFinish listener on dialogue UI
                    //currVNData.lineNumber += 1;
                    //readCommand(currVNData.lineNumber);
                    break;
                }
                else if (keyword == "fx")    //some kind of effect, 
                {
                    if (debateVNMode)
                    {
                        DebateDialogueUI.instance.playEffect(line[1], float.Parse(line[2]));
                    }
                    else
                    //calls a method in dialogue ui to run a specific coroutine based on the name of the effect passed in
                        DialogueUI.instance.playEffect(line[1], float.Parse(line[2]));
                    //should continue to next line after effect plays, called by effectFinish listener on dialogue UI
                    //currVNData.lineNumber += 1;
                    //readCommand(currVNData.lineNumber);
                    break;
                }
                else if (keyword == "sf")    //play a sound effect 
                {
                    SFController.instance.playSF(line[1]);
                    currVNData.lineNumber += 1;
                }
                else if (keyword == "cutscene")  //play a video, can skip if player wants to 
                {
                    //havent figured this one out yet
                }
                else if (keyword == "cond")      //if all conditions after the cond are met, i.e. flags set to 1, read commands after cond until exit
                {                               //otherwise skip all until after first exit
                    if (storyProgress.instance.flags.ContainsKey(line[1]) && storyProgress.instance.flags[line[1]] == 1)
                    {
                        //nothing happens, continue as usual if cond condition is met                  
                    }
                    else
                    {
                        skipUntilExit = true;
                    }
                    currVNData.lineNumber += 1;
                }
                else if (keyword == "set")       //manually sets a flag value, sometimes useless in cond statements
                {
                    storyProgress.instance.flags[line[1]] = 1;
                    currVNData.lineNumber += 1;
                }
                else if (keyword == "show")  //show something on the dialogue ui, an item
                {
                    if (debateVNMode)
                    {
                        //do something
                        DebateDialogueUI.instance.showItem(line[1]);
                    }
                    else
                        DialogueUI.instance.showItem(line[1]);
                    currVNData.lineNumber += 1;
                }
                else if (keyword == "unshow")    //unshow the thing shown above, fade out smoothly
                {
                    if (debateVNMode)
                    {
                        //do something
                    }
                    DialogueUI.instance.unshowItem();
                    currVNData.lineNumber += 1;
                }
                else if (keyword == "addev")
                {
                    //add evidence to bag,
                    //format: addev=name
                    //name, desc, image
                    int l = evidenceBag.Count;
                    bool found = false;
                    for(int i = 0; i < l; i++)
                    {
                        if (evidenceBag[i].name == line[1]) found = true;
                    }
                    if(!found) evidenceBag.Add(EvidenceAtlas.instance.loadEvidence(line[1]));
                    currVNData.lineNumber += 1;
                    //maybe play a little thing that says "something added to evidence"
                }
                else if(keyword == "rmevidence")
                {
                    //removes evidence from the bag, 
                    //format rmevidence=name/all , remove either by name or all
                    if (line[1] == "all") evidenceBag.Clear();
                    else
                    {
                        int l = evidenceBag.Count;
                        for (int i = 0; i < l; i++)
                        {
                            if (evidenceBag[i].name == line[1])
                            {
                                evidenceBag.RemoveAt(i);
                                break;
                            }
                        }
                    }
                    currVNData.lineNumber += 1;
                }
                //remember to use nameatlas once its setup for character names (line[1])
                else if (keyword == "affectionup")  //increase affection level of certain character
                {
                    string name = NameAtlas.instance.getName(line[1]);  //convert to formal name
                    if (storyProgress.instance.flags.ContainsKey(name))
                        storyProgress.instance.flags[name] += 1;
                    else
                        storyProgress.instance.flags[name] = 1;
                    Debug.Log(storyProgress.instance.flags[name]);
                    currVNData.lineNumber += 1;
                }
                //remember to use nameatlas once its setup for character names (line[1])
                else if (keyword == "affectionof")   //similar to cond, but with a indicated operator, and based on a charcters affection level      
                {
                    //affectionof=character=operator=num
                    string name = NameAtlas.instance.getName(line[1]);  //convert to formal name
                    if (line[2] == "greaterthan")
                    {
                        if (storyProgress.instance.flags[name] > int.Parse(line[3])) ;//same as fulfilling a cond statement, proceed until exit/cont
                        else
                        {
                            skipUntilExit = true;
                        }
                    }
                    else if (line[2] == "equals")
                    {
                        if (storyProgress.instance.flags[name] == int.Parse(line[3])) Debug.Log("affection of equals");//same as fulfilling a cond statement, proceed until exit/cont
                        else
                        {
                            skipUntilExit = true;
                        }
                    }
                    else
                    {
                        if (storyProgress.instance.flags[name] < int.Parse(line[3])) ; //same as fulfilling a cond statement, proceed until exit/cont
                        else
                        {
                            skipUntilExit = true;
                        }
                    }
                    currVNData.lineNumber += 1;
                }
                else if (keyword == "choice")   //initiates a option for player to choose between 2-4 choices
                {
                    //will this be used in debate? perhaps. should still work if i just bring this up over debate UI
                    currVNData.lastChoice = currVNData.lineNumber;
                    waitingforChoose = true;
                    for (int i = 1; i < line.Length; i++)
                    {
                        SelectionUI.instance.setOption(line[i], i);
                    }
                    selectionUIObject.SetActive(true);
                    break;
                }
                else if (keyword == "next")
                {
                    //indicates end of file, proceed to next file and change mode if necessary
                    DialogueUI.instance.clearAll();
                    dialogueUIObject.SetActive(false);
                    setNewFile(line[2], line[1]);
                    break;
                }
                else //skips if does not match any keyword, also works for comments
                {
                    currVNData.lineNumber += 1;
                }
            }
        }
    }

    //starts debate mode
    //reads in all necessary information, sets it into a courtroom class, pass it to courtroom ui, flag current mode to courtroom
    public void debate()
    {
        //iterate through file until next keyword, must have next in order to terminate, same as before, but no recursion now
        while (true)
        {
            if (fileContents[currCourtRoomData.lineNumber] == "")   //empty line, skip
            {
                Debug.Log("reading command at line " + currCourtRoomData.lineNumber.ToString());
                Debug.Log(fileContents[currCourtRoomData.lineNumber]);
                currCourtRoomData.lineNumber += 1;
            }
            Debug.Log("reading command at line " + currCourtRoomData.lineNumber.ToString());
            Debug.Log(fileContents[currCourtRoomData.lineNumber]);
            string[] line = fileContents[currCourtRoomData.lineNumber].Split('=');
            string keyword = line[0];
            //********************* need to modify to incoporate transitions ***********************//
            if (keyword == "statement")  //add to currCourtRoomData's list of statements, then transfer it over at next keyword
            {
                //format: statement=#=char=emote=line=transition
                Statement tmp = new Statement(line[4], line[2], line[3], line[5]);
                storyProgress.instance.mostRecentSprite[NameAtlas.instance.getName(line[2])] = line[2] + "_" + line[3];
                currCourtRoomData.statements.Add(tmp);
                currCourtRoomData.lineNumber += 1;
            }
            //deprecated
            //no more mv? merge into statement keyword
            else if(keyword == "mv")    //this is one transition, and add to a list, transition[0] will be played when going from statement[0] to statement[1]
            {
                //format mv=transition=shot=movement or mv=preset
                if(line.Length == 2)
                {
                    //preset
                    Transition tmp = new Transition(line[1]);
                    currCourtRoomData.transitions.Add(tmp);
                    currCourtRoomData.lineNumber += 1;
                }
                else if(line.Length == 4)
                {
                    //indicated 3 parts, which are also presets to a certian degree
                    Transition tmp = new Transition(line[1], line[2], line[3]);
                    currCourtRoomData.transitions.Add(tmp);
                    currCourtRoomData.lineNumber += 1;
                }
            }
            else if(keyword == "actors")
            {
                currCourtRoomData.allchars = new List<string>(new string[15]);
                for (int i = 1; i < line.Length; i++)
                {
                    Debug.Log((2 * (i - 1)) % 15);
                    currCourtRoomData.allchars[(2 * (i - 1)) % 15] = NameAtlas.instance.getName(line[i]);
                }
                currCourtRoomData.lineNumber += 1;
            }
            else if (keyword == "emotes")
            {
                currCourtRoomData.allSprites = new List<string>(new string[15]);
                for (int i = 1; i < line.Length; i++)
                {
                    currCourtRoomData.allSprites[(2 * (i - 1)) % 15] = line[i];
                }
                currCourtRoomData.lineNumber += 1;
            }
            else if(keyword == "correct")   //format: correct=#=evidence
            {
                currCourtRoomData.correctStatement = currCourtRoomData.statements[int.Parse(line[1])];
                currCourtRoomData.correctEvidence = EvidenceAtlas.instance.loadEvidence(line[2]);
                currCourtRoomData.lineNumber += 1;
            }
            else if (keyword == "bgm")
            {
                currCourtRoomData.bgm = line[1];
                BGMController.instance.playSong(line[1]);
                currCourtRoomData.lineNumber += 1;
            }
            else if (keyword == "incorrect") //sets the file to be played when an incorrect objection is made
            {
                currCourtRoomData.incorrectFile = line[1];
                currCourtRoomData.lineNumber += 1;
            }
            else if (keyword == "loop") //sets the file to be played when player reaches the end of statements and clicks loop button
            {
                currCourtRoomData.loopFile = line[1];
                currCourtRoomData.lineNumber += 1;
            }
            else if (keyword == "next")  //sets the information for the next file/mode after this one
            {
                currCourtRoomData.nextMode = line[1];
                currCourtRoomData.nextFile = line[2];
                //currCourtRoomData.evidenceBag = evidenceBag;
                CourtRoomUI.instance.init(currCourtRoomData);
                CourtRoomUI.instance.startRound();
                break;
            }
            else //not a valid command in this mode, skip
            {
                currCourtRoomData.lineNumber += 1;
            }
        }
    }

    //do later after the file format is settled
    public void investigate()
    {
        while (true)
        {
            if (fileContents[currInvestigationData.lineNumber] == "")   //empty line, skip
            {
                Debug.Log("reading investigate command at line " + currInvestigationData.lineNumber.ToString());
                Debug.Log(fileContents[currInvestigationData.lineNumber]);
                currInvestigationData.lineNumber += 1;
            }
            string[] line = fileContents[currInvestigationData.lineNumber].Split('=');
            string keyword = line[0];
            Debug.Log("reading investigate command at line " + currInvestigationData.lineNumber.ToString());
            Debug.Log(fileContents[currInvestigationData.lineNumber]);
            if (keyword == "locations")  //set all locations required to complete round
            {
                int length = line.Length;
                for (int i = 1; i < length; i++)
                {
                    currInvestigationData.completeScenes.Add(line[i]);
                }
                currInvestigationData.lineNumber += 1;
            }
            else if (keyword == "bgm")
            {
                currInvestigationData.bgm = line[1];
                BGMController.instance.playSong(line[1]);
                currInvestigationData.lineNumber += 1;
            }
            else if (keyword == "location") //sets details of one location
            {
                //format: location=name, read until end
                Location tmp = new Location(line[1]);
                makeLocation(tmp);
                return;
            }
            else if (keyword == "character")    //sets details for one character
            {
                //format: character=name, read until end
                InteractableObject tmp = new InteractableObject(NameAtlas.instance.getName(line[1]));   //just in case script uses a short form
                makeChar(tmp);
                return;
                //let investigation ui temporarily hold all characters until construction complete
            }
            else if (keyword == "noleaving") //file for if attempting travel but not allowed
            {
                currInvestigationData.noleaveFile = line[1];
                currInvestigationData.lineNumber += 1;
            }
            else if (keyword == "startat")  //file to play at beginning of round
            {
                //format: startat=hanamura=start.txt
                currInvestigationData.startLocation = line[1];
                currInvestigationData.startScript = line[2];
                currInvestigationData.lineNumber += 1;
            }
            else if(keyword == "shortcut")//after this file is played, end investigation mode
            {
                //format: shortcut=filename
                currInvestigationData.shorcutFile = line[1];
                currInvestigationData.lineNumber += 1;
            }
            else if(keyword == "allcomplete")
            {
                //format: allcomplete=filename
                currInvestigationData.allCompleteFile = line[1];
                currInvestigationData.lineNumber += 1;
            }
            else if (keyword == "connectclues") //for adding in possible clue connections
            {
                //clues not implemented yet
                currInvestigationData.lineNumber += 1;
            }
            else if (keyword == "next")  //sets the information for the next file/mode after this one
            {
                currInvestigationData.nextMode = line[1];
                currInvestigationData.nextFile = line[2];
                //currInvestigationData.evidenceBag = evidenceBag;
                InvestigationUI.instance.currData = currInvestigationData;
                InvestigationUI.instance.startRound();
                return;
            }
            else //not a valid command in this mode, skip
            {
                currInvestigationData.lineNumber += 1;
            }
        }
    }

    void makeChar(InteractableObject obj) //hits a character block, extract information and put into obj, modifies obj, since pass by reference. call investigate() on end keyword
    {
        while (true)
        {
            //reading from same file, just treated a bit different now, editing obj by reference
            if (fileContents[currInvestigationData.lineNumber] == "")   //empty line, skip
            {
                Debug.Log("reading investigate command at line " + currInvestigationData.lineNumber.ToString());
                Debug.Log(fileContents[currInvestigationData.lineNumber]);
                currInvestigationData.lineNumber += 1;
            }
            Debug.Log("reading investigate command at line " + currInvestigationData.lineNumber.ToString());
            Debug.Log(fileContents[currInvestigationData.lineNumber]);
            string[] line = fileContents[currInvestigationData.lineNumber].Split('=');
            string keyword = line[0];
            if (keyword == "end")    //only keyword with no argument, shows that character construction is complete, go back to investigate
            {
                InvestigationUI.instance.chars.Add(obj);
                currInvestigationData.lineNumber += 1;
                investigate();
                return;
            }
            else if (keyword == "topic")    //convo option with no requirements
            {
                //format : topic=display name=file name
                obj.addConvo("none", "", line[1], line[2]);
                currInvestigationData.lineNumber += 1;
            }
            else if (keyword == "show") //dialogue when presented a certain evidence
            {
                //format: show=bloodyknife=script
                obj.addResponse(line[1], line[2]);
                currInvestigationData.lineNumber += 1;
            }
            else if (keyword == "showbad")   //file for when character has nothing to say for evidence
            {
                //format : showbad=script
                obj.setBadResponse(line[1]);
                currInvestigationData.lineNumber += 1;
            }
            else if (keyword == "addtopic") //convo option with requirement
            {
                //format: addtopic=own/present=brokenclock=The Broken Clock=script
                if (line[1] == "own")
                {
                    obj.addConvo("own", line[2], line[3], line[4], "add");
                }
                else if (line[1] == "present")
                {
                    obj.addConvo("present", line[2], line[3], line[4], "add");
                }
                currInvestigationData.lineNumber += 1;
            }
            else if (keyword == "updatetopic")   //updating an existing convo option after a requirement is reached
            {
                //format : updatetopic=own/present=brokenclock=The Broken Clock=script=number in sequence
                if (line[1] == "own")
                {
                    obj.addConvo("own", line[2], line[3], line[4], "update", int.Parse(line[5]));
                }
                else if (line[1] == "present")
                {
                    obj.addConvo("present", line[2], line[3], line[4], "update", int.Parse(line[5]));
                }
                currInvestigationData.lineNumber += 1;
            }
            else //not valid command, skip
            {
                currInvestigationData.lineNumber += 1;
            }
        }
    }

    void makeLocation(Location loc) //modify location based on script, call investigate() on end keyword
    {
        //iterative now, returns if certain keyword requires waiting or calling another method
        while (true)
        {
            //reading from same file, just treated a bit different now, editing obj by reference
            if (fileContents[currInvestigationData.lineNumber] == "")   //empty line, skip
            {
                Debug.Log("reading investigate command at line " + currInvestigationData.lineNumber.ToString());
                Debug.Log(fileContents[currInvestigationData.lineNumber]);
                currInvestigationData.lineNumber += 1;
            }
            Debug.Log("reading investigate command at line " + currInvestigationData.lineNumber.ToString());
            Debug.Log(fileContents[currInvestigationData.lineNumber]);
            string[] line = fileContents[currInvestigationData.lineNumber].Split('=');
            string keyword = line[0];
            if (keyword == "end")    //only keyword with no argument, shows that character construction is complete, go back to investigate
            {
                currInvestigationData.scenes.Add(loc);
                currInvestigationData.lineNumber += 1;
                investigate();
                return;
            }
            else if (keyword == "addchar")    //add character
            {
                //format : addchar=Kiri=xmin=xmax=ymin=ymax=sprite=need/want
                //need to get formal name from nameatlas
                string name = NameAtlas.instance.getName(line[1]);
                InvestigationUI.instance.findChar(name).setChar(line[6], float.Parse(line[2]), float.Parse(line[3]), float.Parse(line[4]), float.Parse(line[5]));
                loc.objects.Add(InvestigationUI.instance.findChar(name));
                loc.objectsRequired.Add(line[7] == "need");
                storyProgress.instance.mostRecentSprite[name] = line[6];
                currInvestigationData.lineNumber += 1;
            }
            else if (keyword == "object") //add object
            {
                //format: addchar=bloodyknife=xmin=xmax=ymin=ymax=examine script=need/want    (evidence name must match name in file)
                Evidence obj = EvidenceAtlas.instance.loadEvidence(line[1]);
                InteractableObject tmp = new InteractableObject(obj.name, obj.image, float.Parse(line[2]), float.Parse(line[3]), float.Parse(line[4]), float.Parse(line[5]), line[6]);
                loc.objects.Add(tmp);
                loc.objectsRequired.Add(line[7] == "need");
                currInvestigationData.lineNumber += 1;
            }
            else if (keyword == "complete")  //file for when location is complete
            {
                //format: complete=filename
                loc.locationCompleteFile = line[1];
                currInvestigationData.lineNumber += 1;
            }
            else if (keyword == "firsttime") //file for first time traveling to a location
            {
                //format : firsttime=filename
                loc.firstTimeFile = line[1];
                currInvestigationData.lineNumber += 1;
            }
            else if (keyword == "revisit") //file for revisiting a location
            {
                //format: revisited=filename
                loc.revisitFile = line[1];
                currInvestigationData.lineNumber += 1;
            }
            else //not valid command, skip
            {
                currInvestigationData.lineNumber += 1;
            }
        }
    }

    public void freetime()
    {
        while (true)
        {
            if (fileContents[currFreeTimeData.lineNumber] == "")   //empty line, skip
            {
                Debug.Log("reading freetime command at line " + currFreeTimeData.lineNumber.ToString());
                Debug.Log(fileContents[currFreeTimeData.lineNumber]);
                currFreeTimeData.lineNumber += 1;
            }
            string[] line = fileContents[currFreeTimeData.lineNumber].Split('=');
            string keyword = line[0];
            Debug.Log("reading freetime command at line " + currFreeTimeData.lineNumber.ToString());
            Debug.Log(fileContents[currFreeTimeData.lineNumber]);
            if (keyword == "bgm")
            {
                currFreeTimeData.bgm = line[1];
                BGMController.instance.playSong(line[1]);
                currFreeTimeData.lineNumber += 1;
            }
            else if (keyword == "location") //sets details of one location
            {
                //format: location=name, read until end
                Location tmp = new Location(line[1]);
                FTmakeLocation(tmp);
                return;
            }
            else if (keyword == "character")    //sets details for one character
            {
                //format: character=name, read until end
                InteractableObject tmp = new InteractableObject(NameAtlas.instance.getName(line[1]));   //just in case script uses a short form
                tmp.isChar = true;
                FTmakeChar(tmp);
                return;
                //let investigation ui temporarily hold all characters until construction complete
            }
            else if (keyword == "startat")  //file to play at beginning of round
            {
                //format: startat=hanamura=start.txt
                currFreeTimeData.startLocation = line[1];
                currFreeTimeData.startScript = line[2];
                currFreeTimeData.lineNumber += 1;
            }
            else if (keyword == "next")  //sets the information for the next file/mode after this one
            {
                currFreeTimeData.nextMode = line[1];
                currFreeTimeData.nextFile = line[2];
                //currInvestigationData.evidenceBag = evidenceBag;
                FreeTimeUI.instance.currData = currFreeTimeData;
                FreeTimeUI.instance.startRound();
                return;
            }
            else //not a valid command in this mode, skip
            {
                currFreeTimeData.lineNumber += 1;
            }
        }
    }

    public void FTmakeChar(InteractableObject obj)//same objects as investigation mode, but only use the freetimeconvos field
    {
        //iterative now, returns if certain keyword requires waiting or calling another method
        while (true)
        {
            //reading from same file, just treated a bit different now, editing obj by reference
            if (fileContents[currFreeTimeData.lineNumber] == "")   //empty line, skip
            {
                Debug.Log("reading freetime command at line " + currFreeTimeData.lineNumber.ToString());
                Debug.Log(fileContents[currFreeTimeData.lineNumber]);
                currFreeTimeData.lineNumber += 1;
            }
            Debug.Log("reading freetime command at line " + currFreeTimeData.lineNumber.ToString());
            Debug.Log(fileContents[currFreeTimeData.lineNumber]);
            string[] line = fileContents[currFreeTimeData.lineNumber].Split('=');
            string keyword = line[0];
            if (keyword == "end")    //only keyword with no argument, shows that character construction is complete, go back to investigate
            {
                FreeTimeUI.instance.chars.Add(obj);
                currFreeTimeData.lineNumber += 1;
                freetime();
                return;
            }
            else if (keyword == "freetimetopic")
            {
                //format : freetimetopic=affectionlvl=filename
                //assuming the files are given in order of increasing affection, i dont really need to figure out the number itself
                obj.freetimeConvos.Add(line[2]);
                //loc.objectsRequired.Add(line[7] == "need");
                currFreeTimeData.lineNumber += 1;
            }
            else //not valid command, skip
            {
                currFreeTimeData.lineNumber += 1;
            }
        }
    }

    public void FTmakeLocation(Location loc)   //same as investigation
    {
        //iterative now, returns if certain keyword requires waiting or calling another method
        while (true)
        {
            //reading from same file, just treated a bit different now, editing obj by reference
            if (fileContents[currFreeTimeData.lineNumber] == "")   //empty line, skip
            {
                Debug.Log("reading freetime command at line " + currFreeTimeData.lineNumber.ToString());
                Debug.Log(fileContents[currFreeTimeData.lineNumber]);
                currFreeTimeData.lineNumber += 1;
            }
            Debug.Log("reading freetime command at line " + currFreeTimeData.lineNumber.ToString());
            Debug.Log(fileContents[currFreeTimeData.lineNumber]);
            string[] line = fileContents[currFreeTimeData.lineNumber].Split('=');
            string keyword = line[0];
            if (keyword == "end")    //only keyword with no argument, shows that character construction is complete, go back to investigate
            {
                currFreeTimeData.scenes.Add(loc);
                currFreeTimeData.lineNumber += 1;
                freetime();
                return;
            }
            else if (keyword == "addchar")    //add character
            {
                //format : addchar=Kiri=xmin=xmax=ymin=ymax=sprite
                //need to get formal name from nameatlas
                string name = NameAtlas.instance.getName(line[1]);
                FreeTimeUI.instance.findChar(name).setChar(line[6], float.Parse(line[2]), float.Parse(line[3]), float.Parse(line[4]), float.Parse(line[5]));
                loc.objects.Add(FreeTimeUI.instance.findChar(name));
                storyProgress.instance.mostRecentSprite[name] = line[6];
                //loc.objectsRequired.Add(line[7] == "need");
                currFreeTimeData.lineNumber += 1;
            }
            else if (keyword == "object") //add object
            {
                //format: object=sprite name=xmin=xmax=ymin=ymax=examine script    (evidence name must match name in file)
                //Evidence obj = EvidenceAtlas.instance.loadEvidence(line[1]);    //probbaly only used for testing purposes
                InteractableObject tmp = new InteractableObject(line[1], line[1], float.Parse(line[2]), float.Parse(line[3]), float.Parse(line[4]), float.Parse(line[5]), line[6]);
                loc.objects.Add(tmp);
                //loc.objectsRequired.Add(line[7] == "need");
                currFreeTimeData.lineNumber += 1;
            }
            else //not valid command, skip
            {
                currFreeTimeData.lineNumber += 1;
            }
        }
    }

    //logic frenzy mode reader
    public void logicfrenzy()
    {
        while (true)
        {
            //need to be changed for logic frenzy
            if (fileContents[currLogicFrenzyData.lineNumber] == "")   //empty line, skip
            {
                Debug.Log("reading logicfrenzy command at line " + currLogicFrenzyData.lineNumber.ToString());
                Debug.Log(fileContents[currLogicFrenzyData.lineNumber]);
                currLogicFrenzyData.lineNumber += 1;
            }
            Debug.Log("reading freetime command at line " + currLogicFrenzyData.lineNumber.ToString());
            Debug.Log(fileContents[currLogicFrenzyData.lineNumber]);
            string[] line = fileContents[currLogicFrenzyData.lineNumber].Split('=');
            string keyword = line[0];
            if(line.Length == 1 && keyword != "end")    //only one possibility, which is a line of dialogue that needs to be displayed
            {
                //display the dialogue
                //return and wait for click from logic frenzy side
                LogicFrenzyController.instance.displayLine(line[0]);
                currLogicFrenzyData.lineNumber += 1;
                return;
            }
            else if (keyword == "end")    //only keyword with no argument, shows that character construction is complete, go back to investigate
            {
                //all the information for one round has been preprocessed, start the round and pause reading for now
                LogicFrenzyController.instance.currData = currLogicFrenzyData;
                LogicFrenzyController.instance.startRound();
                currLogicFrenzyData.lineNumber += 1;
                return;
            }
            else if (keyword == "bgm")
            {
                //format bgm=songname
                currLogicFrenzyData.bgm = line[1];
                BGMController.instance.playSong(line[1]);
                currLogicFrenzyData.lineNumber += 1;
            }
            else if (keyword == "gameover")
            {
                //format gameover=filename
                currLogicFrenzyData.gameoverfile = line[1];
                currLogicFrenzyData.lineNumber += 1;
            }
            else if (keyword == "c") //setting a character to speak
            {
                //format c=character=emotion=pos(l,r)
                //do it in logic frenzy controller, mc is left, enemy is right, or just listen to what the script says
                LogicFrenzyController.instance.setChar(line[1], line[2], line[3]);
                storyProgress.instance.mostRecentSprite[NameAtlas.instance.getName(line[1])] = line[1] + "_" + line[2];
                currLogicFrenzyData.lineNumber += 1;
            }
            else if (keyword == "opponent")//indicates start of a round within the whole thing, terminated by an end
            {
                //format opponent=name=emotion
                currLogicFrenzyData.opponent = line[1];
                currLogicFrenzyData.sprite = line[2];
                storyProgress.instance.mostRecentSprite[NameAtlas.instance.getName(line[1])] = line[2];
                currLogicFrenzyData.lineNumber += 1;
            }
            else if (keyword == "question")
            {
                //format question=text
                currLogicFrenzyData.question = line[1];
                currLogicFrenzyData.lineNumber += 1;
            }
            else if(keyword == "response")
            {
                //format response=#=text
                currLogicFrenzyData.currResponses.Add(line[2]);
                currLogicFrenzyData.lineNumber += 1;
            }
            else if(keyword == "correct")
            {
                //format correct=#(corresponding to the index of the response in the list)
                currLogicFrenzyData.correctResponse = int.Parse(line[1]);
                currLogicFrenzyData.lineNumber += 1;
            }
            else if(keyword == "detectivefx")
            {
                //not sure yet
                currLogicFrenzyData.lineNumber += 1;
            }
            else if(keyword == "culpritfx")
            {
                //not sure yet
                currLogicFrenzyData.lineNumber += 1;
            }
            else if(keyword == "fail")
            {
                //format fail=filename
                currLogicFrenzyData.failfile = line[1];
                currLogicFrenzyData.lineNumber += 1;
            }
            else if(keyword == "success")
            {   
                //scrapped this one, if succeed just keep reading, no need for special file
                //format success=filename
                currLogicFrenzyData.successfile = line[1];
                currLogicFrenzyData.lineNumber += 1;
            }
            else if(keyword == "pattern")
            {
                //format pattern=type(string)=bullet type(int)=post delay time(float)
                Pattern tmp = new Pattern();
                tmp.name = line[1];
                tmp.type = int.Parse(line[2]);
                tmp.delay = float.Parse(line[3]);
                currLogicFrenzyData.patterns.Add(tmp);
                currLogicFrenzyData.lineNumber += 1;
            }
            else if(keyword == "next")
            {
                LogicFrenzyController.instance.reset();
                logicFrenzyUIObject.SetActive(false);
                setNewFile(line[2], line[1]);
                break;
            }
            else //not valid command, skip
            {
                currFreeTimeData.lineNumber += 1;
            }
        }
    }

    public void peacemaker()
    {
        DedeuceChar currentChar = new DedeuceChar();
        Statement currStatement = new Statement();
        while (true)
        {
            if (fileContents[currPeaceMakerData.lineNumber] == "")   //empty line, skip
            {
                Debug.Log("reading pacemaker command at line " + currPeaceMakerData.lineNumber.ToString());
                Debug.Log(fileContents[currPeaceMakerData.lineNumber]);
                currPeaceMakerData.lineNumber += 1;
            }
            Debug.Log("reading peacemaker command at line " + currPeaceMakerData.lineNumber.ToString());
            Debug.Log(fileContents[currPeaceMakerData.lineNumber]);
            string[] line = fileContents[currPeaceMakerData.lineNumber].Split('=');
            string keyword = line[0];
            if(keyword == "character")
            {
                //make a new character and upadte current char
                //character=name=emote
                currentChar = new DedeuceChar();
                currentChar.name = line[1];
                currentChar.emote = line[2];
                currPeaceMakerData.lineNumber += 1;
            }
            else if (keyword == "endchar")
            {
                //add the completed character to a list
                currPeaceMakerData.dChars.Add(currentChar);
                currPeaceMakerData.lineNumber += 1;
            }
            else if(keyword == "bgm")
            {
                //bgm=Swordland
                currPeaceMakerData.bgm = line[1];
                currPeaceMakerData.lineNumber += 1;
                //causing infinite for some reason, even with the += 1
            }
            else if (keyword == "actors")
            {
                currPeaceMakerData.allchars = new List<string>(new string[15]);
                for (int i = 1; i < line.Length; i++)
                {
                    currPeaceMakerData.allchars[(2 * (i - 1)) % 15] = NameAtlas.instance.getName(line[i]);
                }
                currPeaceMakerData.lineNumber += 1;
            }
            else if (keyword == "emotes")
            {
                currPeaceMakerData.allSprites = new List<string>(new string[15]);
                for (int i = 1; i < line.Length; i++)
                {
                    currPeaceMakerData.allSprites[(2 * (i - 1)) % 15] = line[i];
                }
                currPeaceMakerData.lineNumber += 1;
            }
            else if(keyword == "statement")
            {
                //make a statement and add it to current char
                //format statement=#=text
                Statement tmp = new Statement(line[2], currentChar.name, currentChar.emote, "");
                currentChar.statements.Add(tmp);
                currPeaceMakerData.statements.Add(tmp);
                currPeaceMakerData.lineNumber += 1;
            }
            else if (keyword == "correctmatch")
            {
                //format correct=#1=#2
                currPeaceMakerData.correctOne = int.Parse(line[1]);
                currPeaceMakerData.correctTwo = int.Parse(line[2]);
                currPeaceMakerData.lineNumber += 1;
            }
            else if (keyword == "incorrect")
            {
                //format incorrect=filename
                currPeaceMakerData.incorrectFile = line[1];
                currPeaceMakerData.lineNumber += 1;
            }
            else if(keyword == "next")
            {
                //format next=mode=filename
                currPeaceMakerData.nextMode = line[1];
                currPeaceMakerData.nextFile = line[2];
                //do something to start the mode
                PeaceMakerUI.instance.init(currPeaceMakerData);
                PeaceMakerUI.instance.startRound();
                break;
            }
            else
            {
                currPeaceMakerData.lineNumber += 1;
            }
        }
    }

    //useless for now...
    IEnumerator waitforseconds(float secs)
    {
        yield return new WaitForSeconds(secs);
    }
}
