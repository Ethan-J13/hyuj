using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour
{

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = 0.4f;
    float accelerationTimeAirborne = 0.1f;
    float accelerationTimeGrounded = 0.1f;
    public float moveSpeed = 7;

    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;

    public float wallSlideSpeedMax = 3;
    public float wallStickTime = 0.25f;
    float timeToWallUnstick;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    Vector3 moveDistance;
    float velocityXSmoothing;

    Controller2D controller;

    void Start()
    {
        controller = GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }

    void Update()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        int wallDirX = (controller.collisions.left) ? -1 : 1;

        float targetVelocityX = input.x * moveSpeed;
        moveDistance.x = Mathf.SmoothDamp(moveDistance.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

        bool wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && moveDistance.y < 0) {
            wallSliding = true;

            if (moveDistance.y < -wallSlideSpeedMax) {
                moveDistance.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0) {
                velocityXSmoothing = 0;
                moveDistance.x = 0;

                if (input.x != wallDirX && input.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = wallStickTime;
            }
        }



        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (wallSliding)
            {
                if (wallDirX == input.x)
                {
                    moveDistance.x = -wallDirX * wallJumpClimb.x;
                    moveDistance.y = wallJumpClimb.y;
                }
                else if (input.x == 0)
                {
                    moveDistance.x = -wallDirX * wallJumpOff.x;
                    moveDistance.y = wallJumpOff.y;
                }
                else
                {
                    moveDistance.x = -wallDirX * wallLeap.x;
                    moveDistance.y = wallLeap.y;
                }
            }
            if (controller.collisions.below && input.y != -1)
            {
                moveDistance.y = maxJumpVelocity;
            }
        }
        if (Input.GetKeyUp (KeyCode.Space)) { 
            if (moveDistance.y > minJumpVelocity) {
                moveDistance.y = minJumpVelocity;
            }
        }

        moveDistance.y += gravity * Time.deltaTime;
        controller.Move(moveDistance * Time.deltaTime, input);

        if (controller.collisions.above || controller.collisions.below) {
            moveDistance.y = 0;
        }
    }
}
