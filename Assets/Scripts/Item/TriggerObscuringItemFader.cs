using UnityEngine;

public class TriggerObscuringItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ObscuringItemFader[] obscuringItemFaders = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        if (obscuringItemFaders.Length > 0)
        {
            for(int i = 0; i < obscuringItemFaders.Length; i ++)
            {
                // 植物淡出（通过设置透明度）
                obscuringItemFaders[i].FadeOut();
            }
        }
    }

    private void  OnTriggerExit2D (Collider2D collision)
    {
        ObscuringItemFader[] obscuringItemFaders = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();
        if (obscuringItemFaders.Length > 0)
        {
            for(int i = 0; i < obscuringItemFaders.Length; i ++ )
            {
                // 植物淡出效果恢复
                obscuringItemFaders[i].FadeIn();
            }
        }
    }
}
