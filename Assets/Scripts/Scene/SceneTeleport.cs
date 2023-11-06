using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SceneTeleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneName;
    [SerializeField] private Vector3 sceneGotoPosition = new Vector3();


    private void OnTriggerStay2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player != null)
        {
            SceneControllerManager.Instance.FadeAndLoadScene(sceneName.ToString(), sceneGotoPosition);
        }
    }
}
