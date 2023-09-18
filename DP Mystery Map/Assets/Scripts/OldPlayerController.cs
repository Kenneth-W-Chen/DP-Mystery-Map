using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldPlayerController : MonoBehaviour
{
    
    /*private const float WalkDuration = .25f;
    private const float StepSize = 1f;
    private const float KeyPushedDuration = .25f;

    //_walkOn determines if the rigidbody should keep moving once it reaches its position
    private bool _walkOn = false;
    private bool _walkOnce = false;
    private float _walkStartTime;

    // The direction the player will start walking in. Not necessarily the direction they're facing, until they start walking
    private Direction _walkDirection;
    private Vector2 _startPos;
    private Vector2 _endPos;
    private bool _keyHeld = false;
    private float _upKeyHeldStartTime;
    private float _downKeyHeldStartTime;
    private float _leftKeyHeldStartTime;
    private float _rightKeyHeldStartTime;
    private Dictionary<Direction, KeyCode> _directionToKeyCode;

    private Direction _facingDirection;

    public KeyCode moveUpKey = KeyCode.W;
    public KeyCode moveDownKey = KeyCode.S;
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;

    public GameObject interactColliderObject;

    // The direction the player is facing. Property so we can override what happens when setting the value
    public Direction FacingDirection
    {
        get => _facingDirection;
        set
        {
            if (value == _facingDirection) return;
            _facingDirection = value;
            interactColliderObject.transform.localRotation = Quaternion.Euler(0, 0, value switch
            {
                Direction.Up => 0,
                Direction.Down => 180,
                Direction.Left => 90,
                Direction.Right => 270,
                _ => 0
            });
        }
    }

    public Rigidbody2D rigidBody;

    [NonSerialized] public bool Walking = false;

    [NonSerialized] public ulong StepsTaken = 0;

    // Start is called before the first frame update
    void Start()
    {
        var position = transform.position;
        position = new Vector3(Mathf.Floor(position.x) + 0.5f, Mathf.Floor(position.y) + 0.5f,
            position.z);
        transform.position = position;
        _startPos = _endPos = position;
        _directionToKeyCode = new Dictionary<Direction, KeyCode>()
        {
            { Direction.Up, moveUpKey },
            { Direction.Down, moveDownKey },
            { Direction.Left, moveLeftKey },
            { Direction.Right, moveRightKey }
        };
        _facingDirection = Direction.Up;
    }

    // Update is called once per frame
    void Update()
    {
        //check if key was pressed down
        if (_keyHeld)
        {
            //calculate time since the key was pressed
            var timeSincePress = Time.time - _walkDirection switch
            {
                Direction.Up => _upKeyHeldStartTime,
                Direction.Down => _downKeyHeldStartTime,
                Direction.Left => _leftKeyHeldStartTime,
                Direction.Right => _rightKeyHeldStartTime,
                _ => 0
            };
            
            //check if the key has been released
            if (Input.GetKeyUp(_directionToKeyCode[_walkDirection]))
            {
                _keyHeld = false;
                _walkOn = false;
                if (timeSincePress <= KeyPushedDuration)
                {
                    if (FacingDirection == _walkDirection)
                    {
                        UpdateMovementVals();
                        _walkOnce = true;
                    }
                }
                else
                {
                }

                /*else FacingDirection = _walkDirection;#1#
            }
            //else we check to see if enough time has passed to consider the button held down
            else if (timeSincePress > KeyPushedDuration)
            {
                /*Debug.Log("Turning on walk");#1#
                _walkOn = true;
                FacingDirection = _walkDirection;
            }

            CheckKeys();
        }
        //else check for key being pressed down
        else
        {
            CheckKeys();
        }
    }

    private void FixedUpdate()
    {
        if (Walking)
        {
            rigidBody.MovePosition(Vector2.Lerp(_startPos, _endPos, (Time.time - _walkStartTime) / WalkDuration));
            if (rigidBody.position == _endPos)
            {
                Walking = false;
                StepsTaken++;
            }

            _walkOnce = false;
        }
        else if (_walkOn || _walkOnce)
        {
            Walking = true;
            UpdateMovementVals();
        }
        /*if(_walkOn)
        // sets the player's position and then checks if they're at the end position
        {
            rigidBody.MovePosition(Vector2.Lerp(_startPos, _endPos, (Time.time - _startTime) / WalkDuration));
            if (rigidBody.position == _endPos)
            {
                _walkOn = false;
            }
        }#1#
    }

    //checks if movement keys are pressed down
    private void CheckKeys()
    {
        if (Input.GetKeyDown(moveRightKey))
        {
            _walkDirection = Direction.Right;
            _keyHeld = true;
            _rightKeyHeldStartTime = Time.time;
        }

        if (Input.GetKeyDown(moveLeftKey))
        {
            _walkDirection = Direction.Left;
            _keyHeld = true;
            _leftKeyHeldStartTime = Time.time;
        }

        if (Input.GetKeyDown(moveDownKey))
        {
            _walkDirection = Direction.Down;
            _keyHeld = true;
            _downKeyHeldStartTime = Time.time;
        }

        if (Input.GetKeyDown(moveUpKey))
        {
            _walkDirection = Direction.Up;
            _keyHeld = true;
            _upKeyHeldStartTime = Time.time;
        }
    }

    /*private Direction CheckKeys()
    {
        if (Input.GetKey(moveUpKey) || Input.GetKeyDown(moveUpKey))
        {
            //move up
            return Direction.Up;
        }
        if (Input.GetKey(moveDownKey) || Input.GetKeyDown(moveDownKey))
        {
            return Direction.Down;
        }
        if (Input.GetKey(moveLeftKey) || Input.GetKeyDown(moveLeftKey))
        {
         return Direction.Left;
        }
        if (Input.GetKey(moveRightKey) || Input.GetKeyDown(moveRightKey))
        {
            
            return Direction.Right;
        }
        else
        {
            return Direction.None;
        }
    }

    #1#
    private void UpdateMovementVals()
    {
        _walkStartTime = Time.time;
        _startPos = transform.position;
        _endPos = _walkDirection switch
        {
            Direction.Up => _startPos + StepSize * Vector2.up,
            Direction.Down => _startPos + StepSize * Vector2.down,
            Direction.Left => _startPos + StepSize * Vector2.left,
            Direction.Right => _startPos + StepSize * Vector2.right,
            _ => _endPos
        };
    }*/
}