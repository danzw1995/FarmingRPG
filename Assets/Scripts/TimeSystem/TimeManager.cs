using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : SingletonMonoBehaviour<TimeManager>, ISaveable
{
    private int gameYear = 1;
    private Season gameSeason = Season.Spring;
    private int gameDay = 1;
    private string gameDayOfWeek = "Mon";
    private int gameHour = 6;
    private int gameMinute = 30;
    private int gameSecond = 0;

    private bool gameClockPaused = false;

    private float gameTick = 0f;

    private string _iSaveableUniqueID;
    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }
    private GameObjectSave _gameObjectSave;
    public GameObjectSave gameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }


    protected override void Awake()
    {
        base.Awake();
        ISaveableUniqueID = GetComponent<GenerateGuid>().GUID;
        gameObjectSave = new GameObjectSave();
    }
    // Start is called before the first frame update
    void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameClockPaused)
        {
            GameTick();
        }
    }

    private void OnEnable()
    {
        ISaveableRegister();

        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
    }

    private void BeforeSceneUnloadFadeOut ()
    {
        gameClockPaused = true;
    }

    private void AfterSceneLoadFadeIn ()
    {
        gameClockPaused = false;
    }

    private void GameTick()
    {
        gameTick += Time.deltaTime;
        if (gameTick >= Settings.secondsPerGameSecond)
        {
            gameTick -= Settings.secondsPerGameSecond;

            UpdateGameSecond();
        }
    }

    private void UpdateGameSecond()
    {
        gameSecond++;
        if (gameSecond > Settings.maxSecond)
        {
            gameSecond = 0;
            gameMinute++;
            if (gameMinute > Settings.maxMinute)
            {
                gameMinute = 0;
                gameHour++;
                if (gameHour > Settings.maxHour)
                {
                    gameHour = 0;
                    gameDay++;
                    if (gameDay > Settings.maxDay)
                    {
                        gameDay = 1;

                        int gs = (int)gameSeason;
                        gs++;
                        gameSeason = (Season)gs;

                        if (gs > Settings.maxSeason)
                        {
                            gameSeason = Season.Spring;

                            gameYear++;
                            EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);


                        }
                        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

                    }
                    gameDayOfWeek = GetGameDayOfWeek();

                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

                }
                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
            }
            EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);


        }
    }

    public string GetGameDayOfWeek()
    {
        int totoalDay = (int)gameSeason * 30 + gameDay;
        int dayOfWeek = totoalDay % 7;
        switch (dayOfWeek)
        {
            case 0:
                return "Sun";
            case 1:
                return "Mon";
            case 2:
                return "Tue";
            case 3:
                return "Wed";
            case 4:
                return "Thu";
            case 5:
                return "Fri";
            case 6:
                return "Sat";
            default:
                return "";
        }
    }

    public void TestAdvanceGameMinute()
    {
        for (int i = 0; i < 60; i++)
        {
            UpdateGameSecond();
        }
    }

    public void TestAdvanceGameDay()
    {
        for (int i = 0; i < 86400; i ++)
        {
            UpdateGameSecond();
        }
    }

    public void ISaveableRegister() {
        SaveLoadManager.Instance.iSaveableList.Add(this);
    }

    public void ISaveableDeregister() {

        SaveLoadManager.Instance.iSaveableList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        gameObjectSave.sceneData.Remove(Settings.persistentScene);
        SceneSave sceneSave = new SceneSave();

        sceneSave.intDictionary = new Dictionary<string, int>();
        sceneSave.stringDictionary = new Dictionary<string, string>();

        sceneSave.intDictionary.Add("gameYear", gameYear);
        sceneSave.intDictionary.Add("gameDay", gameDay);
        sceneSave.intDictionary.Add("gameHour", gameHour);
        sceneSave.intDictionary.Add("gameMinute", gameMinute);
        sceneSave.intDictionary.Add("gameSecond", gameSecond);

        sceneSave.stringDictionary.Add("gameSeason", gameSeason.ToString());
        sceneSave.stringDictionary.Add("gameDayOfWeek", gameDayOfWeek);

        gameObjectSave.sceneData.Add(Settings.persistentScene, sceneSave);


        return gameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        if (gameSave.gameSaveData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            if (gameObjectSave.sceneData.TryGetValue(Settings.persistentScene, out SceneSave sceneSave))
            {
                if (sceneSave.intDictionary != null && sceneSave.stringDictionary != null)
                {
                    if (sceneSave.intDictionary.TryGetValue("gameYear", out int year))
                    {
                        gameYear = year;
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameDay", out int day))
                    {
                        gameDay = day;
                        
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameHour", out int hour))
                    {
                        gameHour = hour;
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameMinute", out int minute))
                    {
                        gameMinute = minute;
                    }
                    if (sceneSave.intDictionary.TryGetValue("gameSecond", out int second))
                    {
                        gameSecond = second;
                    }
                    if (sceneSave.stringDictionary.TryGetValue("gameDayOfWeek", out string dayOfWeek))
                    {
                        gameDayOfWeek = dayOfWeek;
                    }

                    if (sceneSave.stringDictionary.TryGetValue("gameSeason", out string seasonStr))
                    {
                        if (Enum.TryParse<Season>(seasonStr, out Season season))
                        {
                            gameSeason = season;
                        }

                    }

                }
                    

            }
        }
    }

    public TimeSpan GetGameTime()
    {
        TimeSpan timeSpan = new TimeSpan(gameHour, gameMinute, gameSecond);
        return timeSpan;
    }

    public Season GetGameSeason()
    {
        return gameSeason;
    }

    public void ISaveableStoreScene(string sceneName) { }

    public void ISaveableReStoreScene(string sceneName) { }
}
