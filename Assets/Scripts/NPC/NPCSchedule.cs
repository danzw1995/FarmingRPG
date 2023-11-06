using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCPath))]
public class NPCSchedule : MonoBehaviour
{
    [SerializeField] private SO_NPCScheduleEventList sO_NPCScheduleEventList = null;
    private SortedSet<NPCScheduleEvent> npcScheduleEventSet;
    private NPCPath npcPath;

    private void Awake()
    {
        npcPath = GetComponent<NPCPath>();
        npcScheduleEventSet = new SortedSet<NPCScheduleEvent>(new NPCScheduleEventSort());

        foreach(NPCScheduleEvent npcScheduleEvent in sO_NPCScheduleEventList.npcScheduleEventList)
        {
            npcScheduleEventSet.Add(npcScheduleEvent);
        }
    }

    private void OnEnable()
    {
        EventHandler.AdvanceGameMinuteEvent += GameTimeSystem_AdvanceMinute;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameMinuteEvent -= GameTimeSystem_AdvanceMinute;
    }

    private void GameTimeSystem_AdvanceMinute (int gameYear, Season gameSeason, int gameDay, string gameDayofWeek, int gameHour, int gameMinute, int gameSecond) 
    {
        int time = (gameHour * 100) + gameMinute;

        NPCScheduleEvent matchingScheduleEvent = null;
        foreach(NPCScheduleEvent npcScheduleEvent in npcScheduleEventSet)
        {
            if (npcScheduleEvent.Time == time)
            {
                if (npcScheduleEvent.day != 0 && npcScheduleEvent.day != gameDay)
                {
                    continue;
                }

                if (npcScheduleEvent.season != Season.none && npcScheduleEvent.season != gameSeason)
                {
                    continue;
                }

                if (npcScheduleEvent.weather != Weather.none && npcScheduleEvent.weather != GameManager.Instance.currentWeather)
                {
                    continue;
                }

                matchingScheduleEvent = npcScheduleEvent;
                break;
            } else if (npcScheduleEvent.Time > time)
            {
                break;
            }
        }

        if (matchingScheduleEvent != null)
        {
            npcPath.BuildPath(matchingScheduleEvent);
        }
    }

}
