using UnityEngine;

public class GameManager : SingletonMonoBehaviour<GameManager>
{
    public Weather currentWeather;
    protected override void Awake()
    {
        base.Awake();

        Screen.SetResolution(1920, 1080, FullScreenMode.FullScreenWindow, 0);

        currentWeather = Weather.dry;
    }
}
