using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : SingletonMonoBehaviour<UIManager>
{
    private bool _showPauseMenu = false;
    [SerializeField] private PauseMenuInventoryManagement pauseMenuInventoryManagement = null;
    [SerializeField] private UIInventoryBar uIInventoryBar = null;
    [SerializeField] private GameObject pauseMenu = null;
    [SerializeField] private GameObject[] menuTabs = null;
    [SerializeField] private Button[] menuButtons = null;

    public bool showPauseMenu { get => _showPauseMenu; set => _showPauseMenu = value; }

    protected override void Awake()
    {
        base.Awake();

        pauseMenu.SetActive(false);
    }

    private void Update()
    {
        PauseMenu();  
    }

    private void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (showPauseMenu)
            {
                DisablePauseMenu();
            } else
            {
                EnablePauseMenu();
            }
        }
    }

    public void DisablePauseMenu() {

        pauseMenuInventoryManagement.DestroyCurrentlyDraggedItem();
        showPauseMenu = false;

        Player.Instance.playerInputIsDisabled = false;
        Time.timeScale = 1;
        pauseMenu.SetActive(false); 
    }

    public void EnablePauseMenu() {

        uIInventoryBar.DestroyCurrentDraggedItems();

        uIInventoryBar.ClearSelectedItems();

        showPauseMenu = true;

        Player.Instance.playerInputIsDisabled = true;
        Time.timeScale = 0;

        pauseMenu.SetActive(true);

        System.GC.Collect();

        HighlightButtonForSelectedTab();
    }

    private void HighlightButtonForSelectedTab()
    {
        for(int i = 0; i < menuTabs.Length; i ++)
        {
            if (menuTabs[i].activeSelf)
            {
                SetButtonColorToActive(menuButtons[i]);
            } else
            {
                SetButtonColorToInactive(menuButtons[i]);
            }
        }
    }

    private void SetButtonColorToActive(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.pressedColor;

        button.colors = colors;
    }

    private void SetButtonColorToInactive(Button button)
    {
        ColorBlock colors = button.colors;

        colors.normalColor = colors.disabledColor;

        button.colors = colors;
    }

    public void SwitchPauseMenuTab(int tabNum)
    {
        for (int i = 0; i < menuTabs.Length; i ++)
        {
            if (i == tabNum)
            {
                menuTabs[i].SetActive(true);
            } else
            {
                menuTabs[i].SetActive(false);
            }
        }

        HighlightButtonForSelectedTab();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
