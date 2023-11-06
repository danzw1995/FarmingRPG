using UnityEngine;

public class PlayerAnimationTest : MonoBehaviour
{
    public  int inputX;
    public  int inputY;
    public  bool isWalking;
    public  bool isRunning;
    public  bool isIdle;
    public  bool isCarrying;
    public  ToolEffect toolEffect;
    public  bool isUsingToolRight;
    public  bool isUsingToolLeft;
    public  bool isUsingToolUp;
    public  bool isUsingToolDown;
    public  bool isLiftingToolRight;
    public  bool isLiftingToolLeft;
    public  bool isLiftingToolUp;
    public  bool isLiftingToolDown;
    public  bool isPickingRight;
    public  bool isPickingLeft;
    public  bool isPickingUp;
    public  bool isPickingDown;
    public  bool isSwingingToolRight;
    public  bool isSwingingToolLeft;
    public  bool isSwingingToolUp;
    public  bool isSwingingToolDown;


    public  bool idleRight;
    public  bool idleLeft;
    public  bool idleUp;
    public  bool idleDown;

    // Update is called once per frame
    void Update()
    {
        EventHandler.CallMoventEvent(inputX, inputY, isWalking, isRunning, isIdle, isCarrying,
           toolEffect,
           isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
           isLiftingToolRight, isLiftingToolLeft, isLiftingToolUp, isLiftingToolDown,
           isPickingRight, isPickingLeft, isPickingUp, isPickingDown,
           isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp,  isSwingingToolDown,
            idleRight,  idleLeft,  idleUp,  idleDown);
    }
}
