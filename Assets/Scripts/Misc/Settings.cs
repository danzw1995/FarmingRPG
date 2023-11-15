

using UnityEngine;

public static class Settings {
    public static string persistentScene = "PersistentScene";
    public static float fadeInSeconds = 0.25f;
    public static float fadeOutSeconds = 0.35f;
    public static float targetAlpla = 0.45f;

    public const float gridCellSize = 1f;
    public const float gridCellDiagonalSize = 1.41f;
    public const int maxGridWidth = 99999;
    public const int maxGridHeight = 99999;
    public static Vector2 cursorSize = Vector2.zero;

    public static float playerCentreYOffset = 0.875f;

    // Player Movement
    // public static float runningSpeed = 5.333f;
    public static float runningSpeed = 8.333f;
    public static float walkingSpeed = 2.666f;
    public static float useToolAnimationPause = 0.25f;
    public static float afterUseToolAnimationPause = 0.2f;

    public static float useLiftToolAnimationPause = 0.4f;
    public static float afterUseLiftToolAnimationPause = 0.4f;

    public static float pickAnimationPause = 1f;
    public static float afterPickAnimationPause = 0.2f;

    public static float pixelSize = 0.0625f;

    // Inventory
    public static int playerInitialInventoryCapacity = 24;
    public static int playerMaximumInventoryCapacity = 48;


    public static int walkUp;
    public static int walkDown;
    public static int walkRight;
    public static int walkLeft;
    public static int eventAnimation;

    public static int xInput;
    public static int yInput; 
    public static int isWalking;
    public static int isRunning;
    public static int isIdle;
    public static int isCarrying;
    public static int toolEffect;
    public static int isUsingToolRight;
    public static int isUsingToolLeft;
    public static int isUsingToolUp;
    public static int isUsingToolDown;
    public static int isLiftingToolRight;
    public static int isLiftingToolLeft;
    public static int isLiftingToolUp;
    public static int isLiftingToolDown;
    public static int isPickingRight;
    public static int isPickingLeft;
    public static int isPickingUp;
    public static int isPickingDown;
    public static int isSwingingToolRight;
    public static int isSwingingToolLeft;
    public static int isSwingingToolUp;
    public static int isSwingingToolDown;


    public static int idleRight;
    public static int idleLeft;
    public static int idleUp;
    public static int idleDown;

    // 工具
    public const string HoeingTool = "Hoe";
    public const string ChoppingTool = "Axe";
    public const string BreakingTool = "Pickaxe";
    public const string ReapingTool = "Scythe";
    public const string WateringTool = "Watering Can";
    public const string CollectingTool = "Basket";

    public const int maxCollidersToTestPerReapSwing = 15;
    public const int maxTargetComponentsToDestroyPerReapSwing = 2;


    // 时间相关
    public const float secondsPerGameSecond = 0.012f;
    public const int maxSecond = 59;
    public const int maxMinute = 59;
    public const int maxHour = 23;
    public const int maxDay = 29;
    public const int maxSeason = 3;


    static Settings()
    {

        walkDown = Animator.StringToHash("walkDown");
        walkUp = Animator.StringToHash("walkUp");
        walkLeft = Animator.StringToHash("walkLeft");
        walkRight = Animator.StringToHash("walkRight");
        eventAnimation = Animator.StringToHash("eventAnimation");

        xInput = Animator.StringToHash("xInput");
        yInput = Animator.StringToHash("yInput");
        isWalking = Animator.StringToHash("isWalking");
        isRunning = Animator.StringToHash("isRunning");
        isIdle = Animator.StringToHash("isIdle");
        isCarrying = Animator.StringToHash("isCarrying");
        toolEffect = Animator.StringToHash("toolEffect");
        isUsingToolRight = Animator.StringToHash("isUsingToolRight");
        isUsingToolLeft = Animator.StringToHash("isUsingToolLeft");
        isUsingToolUp = Animator.StringToHash("isUsingToolUp");
        isUsingToolDown = Animator.StringToHash("isUsingToolDown");
        isLiftingToolRight = Animator.StringToHash("isLiftingToolRight");
        isLiftingToolLeft = Animator.StringToHash("isLiftingToolLeft");
        isLiftingToolUp = Animator.StringToHash("isLiftingToolUp");
        isLiftingToolDown = Animator.StringToHash("isLiftingToolDown");
        isPickingRight = Animator.StringToHash("isPickingRight");
        isPickingLeft = Animator.StringToHash("isPickingLeft");
        isPickingUp = Animator.StringToHash("isPickingUp");
        isPickingDown = Animator.StringToHash("isPickingDown");
        isSwingingToolRight = Animator.StringToHash("isSwingingToolRight");
        isSwingingToolLeft = Animator.StringToHash("isSwingingToolLeft");
        isSwingingToolUp = Animator.StringToHash("isSwingingToolUp");
        isSwingingToolDown = Animator.StringToHash("isSwingingToolDown");
        idleRight = Animator.StringToHash("idleRight");
        idleLeft = Animator.StringToHash("idleLeft");
        idleUp = Animator.StringToHash("idleUp");
        idleDown = Animator.StringToHash("idleDown");
    }

}