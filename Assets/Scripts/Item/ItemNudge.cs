using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemNudge : MonoBehaviour
{
    private WaitForSeconds pause;
    private bool isAnimating = false;

    private void Awake()
    {
        pause = new WaitForSeconds(0.04f);
    }

    /// <summary>
    ///风景 与player的碰撞
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAnimating == false)
        {
            // 角色在左侧
            if (gameObject.transform.position.x < collision.gameObject.transform.position.x)
            {
                StartCoroutine(RotateAntiClock());
             } else
            {
                StartCoroutine(RotateClock());
            }

            if (collision.gameObject.tag == "Player")
            {
                AudioManager.Instance.PlaySound(SoundName.effectRustle);
            }
        }
    }

    private IEnumerator RotateAntiClock()
    {
        isAnimating = true;
        for (int i = 0; i < 4; i ++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);

            yield return pause;
        }

        for (int j = 0; j < 5; j ++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);
            yield return pause;

        }


        gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);

        yield return pause;

        isAnimating = false;

    }

    private IEnumerator RotateClock()
    {
        isAnimating = true;

         for (int i = 0; i < 4; i ++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);

            yield return pause;
        }

         for (int j = 0; j < 5; j ++)
        {
            gameObject.transform.GetChild(0).Rotate(0f, 0f, 2f);

            yield return pause;
        }

        gameObject.transform.GetChild(0).Rotate(0f, 0f, -2f);

        yield return pause;
        isAnimating = false;
    }
}
