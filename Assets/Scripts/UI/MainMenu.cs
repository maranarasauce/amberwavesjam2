using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public enum MenuScreen
//{
//    Title,
//    Options,
//}

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject startScreen;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider resolutionSlider;

    private GameObject currentScreen;

    // MenuScreen stuff intended to be filled out in the editor, 
    //  but would require a package or writing editor scripts :(
    //  -Parz
    //private Dictionary<MenuScreen, GameObject> screenDict = new Dictionary<MenuScreen, GameObject>();
    //private MenuScreen currentScreen = MenuScreen.Title;


    //public void OpenScreen(MenuScreen screen)
    //{
    //    if(currentScreen == screen) { return; }

    //    screenDict[currentScreen].SetActive(false);
    //    screenDict[screen].SetActive(true);

    //    currentScreen = screen;
    //}

    //private void Start()
    //{
    //    screenDict.Add(MenuScreen.Title, titleScreen);
    //    screenDict.Add(MenuScreen.Options, optionsScreen);
    //}

    private void Start()
    {
        OpenScreen(startScreen);
    }

    public void OpenScreen(GameObject screen)
    {
        if (currentScreen != null && currentScreen == screen) { return; }

        if (currentScreen != null)
            currentScreen.SetActive(false);

        screen.SetActive(true);
        currentScreen = screen;
    }
    
    public void OnVolumeChanged()
    {
        GameManager.inst.SetGlobalVolume(volumeSlider.value / volumeSlider.maxValue);
    }

    public void OnResolutionChanged()
    {
        GameManager.inst.ScaleRenderResolution(resolutionSlider.value);
    }


    public void PlayGame()
    {
        startScreen.SetActive(false);
        GameManager.inst.LoadGame();
    }
    
    public void ReturnToTitle()
    {
        startScreen.SetActive(false);
        GameManager.inst.LoadTitleScreen();
    }

    public void ExitGame()
    {
        if (!GameManager.inst.IsLoading)
            Application.Quit();
    }
}

