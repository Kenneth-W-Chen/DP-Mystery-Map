using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Rigidbody2D playerRigidbody;
    public Transform interactColliderTransform;
    public float speed = 5f;
    public KeyCode moveUpKey = KeyCode.W;
    public KeyCode moveDownKey = KeyCode.S;
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;

    private float _tanSpeed;

    private float xMovement;
    private float yMovement;
    
    public enum Direction
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }

    [NonSerialized] public Direction FacingDirection;
    // Start is called before the first frame update
    void Start()
    {
        _tanSpeed = speed * .707f;
    }

    void Update()
    {
        // For rotating the interact collider to the side the player is facing
        interactColliderTransform.transform.localRotation = Quaternion.Euler(0, 0, FacingDirection switch
        {
            Direction.Up => 0,
            Direction.Down => 180,
            Direction.Left => 90,
            Direction.Right => 270,
            _ => 0
        });
    }

    void FixedUpdate()
    {
        xMovement = 0;
        yMovement = 0;
        if (Input.GetKey(moveUpKey)) yMovement += 1f;
        if (Input.GetKey(moveDownKey)) yMovement -= 1f;
        if (Input.GetKey(moveRightKey)) xMovement += 1f;
        if (Input.GetKey(moveLeftKey)) xMovement -= 1f;
        
        // Check if the player is moving diagonally
        var absX = Mathf.Abs(xMovement);
        var absY = Mathf.Abs(yMovement);
        if (absY > 0.5f)
        {
            FacingDirection = yMovement > 0f ? Direction.Up : Direction.Down;
            if (absX > 0.5f)
            {
                xMovement *= _tanSpeed;
                yMovement *= _tanSpeed;
            }
            else
            {
                yMovement *= speed;
            }
        }
        else if (absX > 0.5f)
        {
            FacingDirection = xMovement > 0f ? Direction.Right : Direction.Left;
            xMovement *= speed;
        }

        playerRigidbody.velocity = new Vector2(xMovement, yMovement);
    }
}
