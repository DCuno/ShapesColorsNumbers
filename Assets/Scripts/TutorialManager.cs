using UnityEngine;

public class TutorialManager : Singleton<TutorialManager>
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Get PlayerPrefs tutorial bool
        // If tutorial, then pull the TitleManager. 
        // Use TitleManager methods to interface with the menus.
        // TODO: Create message objects with Next button to move along tutorial. Mask behind message objects to block interaction 
        // Maybe: Block all interaction and only allow pressing Next button?
        // When Options is entered, pull OptionsManager.
        // Use OptionsManager methods to interface with menus.
        // Maybe: Need to pull scroll rect and impart force like in FunMode to show bottom of screen, or set the value explicitly
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
