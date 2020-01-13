using System; // i added for save
using System.IO; // stands for inpup/output
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary; // i added
using UnityEngine;

public class GameControl : MonoBehaviour
{
    //saving for pc builds ----NOT WEB BUILDS----
    public static GameControl control; // define 1 control for the whole game

    public int levelsUnlocked;
    public int[] stars = new int[9];

    void Awake()
    {
        if (control == null) // if there is no control make it this and don't destroy
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if(control != this) // if there is control but not this one destroy it
            Destroy(gameObject);
    }
	void OnGUI()
    {
        GUI.Label(new Rect(10,10,100,30), "Levels " + levelsUnlocked);
    }
    public void Save() // saves the data out to a file
    {

    }
    public void Load() // loads the data from a file
    { }
}
