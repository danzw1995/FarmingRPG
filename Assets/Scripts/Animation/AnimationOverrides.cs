using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOverrides : MonoBehaviour
{
    [SerializeField] private GameObject character = null;
    [SerializeField] private SO_AnimationType[] sO_AnimationTypes = null;

    private Dictionary<AnimationClip, SO_AnimationType> animationTypeDictionaryByAnimation;
    private Dictionary<string, SO_AnimationType> animationTypeDictionaryByCompositeAttributeKey;
    // Start is called before the first frame update
    void Start()
    {

        animationTypeDictionaryByAnimation = new Dictionary<AnimationClip, SO_AnimationType>();
        animationTypeDictionaryByCompositeAttributeKey = new Dictionary<string, SO_AnimationType>();


        foreach (SO_AnimationType item in sO_AnimationTypes)
        {
            animationTypeDictionaryByAnimation.Add(item.animationClip, item);

            string key = item.characterPart.ToString() + item.partVariantColour.ToString() + item.partVariantType.ToString() + item.animationName.ToString();
            animationTypeDictionaryByCompositeAttributeKey.Add(key, item);
        }
    }

    /// <summary>
    /// 重写animator的动画帧
    /// </summary>
    /// <param name="characterAttributes"></param>
    public void ApplyCharacterCustomisationParameters(List<CharacterAttribute> characterAttributes)
    {
        foreach(CharacterAttribute characterAttribute in characterAttributes)
        {
            Animator currentAnimator = null;

            List<KeyValuePair<AnimationClip, AnimationClip>> animationKeyValuePairs = new List<KeyValuePair<AnimationClip, AnimationClip>>();

            string animationSOAssetName = characterAttribute.characterPart.ToString();

            Animator[] animators = character.GetComponentsInChildren<Animator>();
            
            // 找到当前的animator
            foreach(Animator animator in animators)
            {
                if (animator.name == animationSOAssetName)
                {
                    currentAnimator = animator;
                    break;
                }
            }

            AnimatorOverrideController aoc = new AnimatorOverrideController(currentAnimator.runtimeAnimatorController);
            List<AnimationClip> animationClips = new List<AnimationClip>(aoc.animationClips);
            
            foreach(AnimationClip animationClip in animationClips)
            {
                SO_AnimationType sO_AnimationType;
                // 找出预定义的动画
                bool foundAnimation = animationTypeDictionaryByAnimation.TryGetValue(animationClip, out sO_AnimationType);

                if (foundAnimation)
                {
                    // 通过characterPart + partVariantColour + animationName 找出对应的动画

                    string key = characterAttribute.characterPart.ToString() + characterAttribute.partVariantColour.ToString() + characterAttribute.partVariantType.ToString() + sO_AnimationType.animationName.ToString();

                    SO_AnimationType sO_SwapAnimationType;

                    bool foundSwapAnimation = animationTypeDictionaryByCompositeAttributeKey.TryGetValue(key, out sO_SwapAnimationType);

                    if (foundSwapAnimation)
                    {
                        AnimationClip swapAnimationClip = sO_SwapAnimationType.animationClip;
                        animationKeyValuePairs.Add(new KeyValuePair<AnimationClip, AnimationClip>(animationClip, swapAnimationClip));
                    }
                }
            }

            // 重写
            aoc.ApplyOverrides(animationKeyValuePairs);

            currentAnimator.runtimeAnimatorController = aoc;
        }

    
    }
}
