using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering.Universal;


public class LightingControl : MonoBehaviour
{
    [SerializeField] private LightingSchedule lightingSchedule;
    [SerializeField] private bool isLightFlicker = false;
    [SerializeField] [Range(0f, 1f)] private float lightFlickerIntensity;
    [SerializeField] [Range(0f, 0.2f)] private float lightFlickerTimeMin;
    [SerializeField] [Range(0f, 0.2f)] private float lightFlickerTimeMax;

    private Light2D light2D;
    private Dictionary<string, float> lightingBrightnessDictionary = new Dictionary<string, float>();
    private float currentLightIntensity;
    private float lightFlickerTimer = 0f;
    private Coroutine fadeInLightRoutine;

    private void Awake()
    {
        light2D = GetComponent<Light2D>();

        if (light2D == null)
        {
            enabled = false;
        }

        foreach(LightingBrightness lightingBrightness in lightingSchedule.lightingBrightnessArray)
        {
            string key = lightingBrightness.season.ToString() + lightingBrightness.hour.ToString();

            lightingBrightnessDictionary.Add(key, lightingBrightness.lightIntensity);
        }
    }

    private void OnEnable()
    {
        EventHandler.AdvanceGameHourEvent += AdvanceGameHour;
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
    }

    private void OnDisable()
    {
        EventHandler.AdvanceGameHourEvent -= AdvanceGameHour;
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
    }

    private void Update()
    {
        if (isLightFlicker)
        {
            lightFlickerTimer = Time.deltaTime;
        }
    }

    private void LateUpdate()
    {
        if (lightFlickerTimer <= 0f && isLightFlicker)
        {
            LightFlicker();
        } else
        {
            light2D.intensity = currentLightIntensity;
        }
    }

    private void AdvanceGameHour (int gameYear, Season gameSeason, int gameDay, string gameDayofWeek, int gameHour, int gameMinute, int gameSecond)
    {
        SetLightingIntensity(gameSeason, gameHour, true);
    }

    private void AfterSceneLoad () {

        SetLightingAfterSceneLoaded();
    }
    private void SetLightingAfterSceneLoaded()
    {
        Season gameSeason = TimeManager.Instance.GetGameSeason();
        int gameHour = TimeManager.Instance.GetGameTime().Hours;

        SetLightingIntensity(gameSeason, gameHour, false);
    }

    private void SetLightingIntensity(Season gameSeason, int gameHour, bool fadein)
    {
        int i = 0;
        while (i < 23)
        {
            string key = gameSeason.ToString() + gameHour.ToString();

            if (lightingBrightnessDictionary.TryGetValue(key, out float targetLightIntensity))
            {
                if (fadein)
                {
                    if (fadeInLightRoutine != null)
                    {
                        StopCoroutine(fadeInLightRoutine);
                    }
                    fadeInLightRoutine = StartCoroutine(FadeLightRoutine(targetLightIntensity));

                } else
                {
                    currentLightIntensity = targetLightIntensity;

                }
                break;
            }

            if (gameHour == 0)
            {
                gameHour = 23;
            }
            else
            {
                gameHour--;
            }
            i++;

        }
    }

    private IEnumerator  FadeLightRoutine(float targetLightIntensity)
    {
        float fadeDuration = 5f;

        float fadeSpeed = Mathf.Abs(currentLightIntensity - targetLightIntensity) / fadeDuration;

        while(!Mathf.Approximately(currentLightIntensity, targetLightIntensity))
        {
            currentLightIntensity = Mathf.MoveTowards(currentLightIntensity, targetLightIntensity, fadeSpeed * Time.deltaTime);

            yield return null;
        }

        currentLightIntensity = targetLightIntensity;
    }

    private void LightFlicker()
    {
        light2D.intensity = Random.Range(currentLightIntensity, currentLightIntensity + (currentLightIntensity * lightFlickerIntensity));

        lightFlickerTimer = Random.Range(lightFlickerTimeMin, lightFlickerTimeMax);
    }
}
