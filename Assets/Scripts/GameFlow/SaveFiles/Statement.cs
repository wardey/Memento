using UnityEngine;
using System.Collections;

[System.Serializable]
public class Statement{
    public string statement;
    public string character;
    public string emote;
    public string descfile; //deprecated
    public string soundeffect;  //not sure where this is gonna go
    public string shift;    //deprecated
    public Transition transition;

    public Statement(string statement, string character, string emote, string transition)
    {
        this.statement = statement;
        this.character = character;
        this.emote = emote;
        this.transition = new Transition(transition);
        //this.descfile = descfile;
        //this.soundeffect = soundeffect; //what does this do?
    }

    public Statement()
    {

    }

    public string getSprite()
    {
        return character + "_" + emote;
    }
}

[System.Serializable]
public class Transition
{
    //all the parts of a transition would also be a "preset" of some sort
    public string preset = "";   //not always applicable, and default to "" if not a preset
    public string transition;
    public string shot;
    public string movement;

    public Transition(string preset)
    {
        this.preset = preset;
    }

    public Transition(string transition, string shot, string movement)
    {
        this.transition = transition;
        this.shot = shot;
        this.movement = movement;
    }
}
