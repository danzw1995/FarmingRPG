using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCManager : SingletonMonoBehaviour<NPCManager>
{
    [SerializeField] private SO_SceneRouteList sO_SceneRouteList = null;
    private Dictionary<string, SceneRoute> sceneRouteDictionary;

    [HideInInspector]
    public NPC[] npcs;

    private AStar aStar;

    protected override void Awake()
    {
        base.Awake();

        sceneRouteDictionary = new Dictionary<string, SceneRoute>();

        if (sO_SceneRouteList.sceneRouteList.Count > 0)
        {
            foreach(SceneRoute sceneRoute in sO_SceneRouteList.sceneRouteList)
            {
                string key = sceneRoute.fromSceneName.ToString() + "_" + sceneRoute.toSceneName.ToString();
                if (sceneRouteDictionary.ContainsKey(key))
                {
                    Debug.Log("*** Duplicate Scene Route Key Found, Please Check Your SceneRouteList ***");
                    continue;
                }

                sceneRouteDictionary.Add(key, sceneRoute);
            }
        }

        aStar = GetComponent<AStar>();

        npcs = FindObjectsOfType<NPC>();
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterLoad;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterLoad;
    }

    private void AfterLoad()
    {
        SetNpcsActiveStatus();
    }

    private void SetNpcsActiveStatus()
    {
        foreach(NPC npc in npcs)
        {
            NPCMovement npcMovement = npc.GetComponent<NPCMovement>();

            if (npcMovement.npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
            {
                npcMovement.SetNPCActiveInScene();
            } else
            {
                npcMovement.SetNPCInactiveInScene();
            }
        }
    }

    public bool BuildPath(SceneName sceneName, Vector2Int startGridPosition, Vector2Int endGridPosition, Stack<NPCMovementStep> npcMovementStack)
    {
        return aStar.BuildPath(sceneName, startGridPosition, endGridPosition, npcMovementStack);
      
        
    }

    public SceneRoute GetSceneRoute(string fromScene, string toScene)
    {
        SceneRoute sceneRoute;
        if (sceneRouteDictionary.TryGetValue(fromScene + "_" + toScene, out sceneRoute)) {
            return sceneRoute;
        } else
        {
            return null;
        }
    }
}
