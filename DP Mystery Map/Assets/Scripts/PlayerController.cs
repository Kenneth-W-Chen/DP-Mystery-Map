using System;
using System.Collections;
using System.Collections.Generic;
using PlayerInfo;
using UnityEngine;

/// <summary>
/// This script includes movement and interact colliders.
/// </summary>
public class PlayerController : MonoBehaviour
{
    public Rigidbody2D playerRigidbody;
    public Transform interactColliderTransform;
    public float speed = 5f;
    
    private MovePlayerUpdate _movePlayerUpdate;
    private MovePlayerFixedUpdate _movePlayerFixedUpdate;

    private delegate void MovePlayerUpdate();

    private delegate void MovePlayerFixedUpdate();

    private bool _isGridMovement;
    private float _xMovement;
    private float _yMovement;
    private float _tanSpeed;

    public bool IsGridMovement
    {
        get => _isGridMovement;
        set
        {
            if (value == _isGridMovement) return;
            if (value)
            {
                _movePlayerUpdate = GridMoveUpdate;
                _movePlayerFixedUpdate = GridMoveFixedUpdate;
                InitializeGridPosition();
                _isGridMovement = true;
            }
            else
            {
                _movePlayerUpdate = FreeMoveUpdate;
                _movePlayerFixedUpdate = FreeMoveFixedUpdate;
                _isGridMovement = false;
            }
        }
    }
    
    //Todo sort these
    private const float WalkDuration = .25f;
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
    
    public KeyCode moveUpKey = KeyCode.W;
    public KeyCode moveDownKey = KeyCode.S;
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;

    [NonSerialized] public bool Walking = false;

    [NonSerialized] public ulong StepsTaken = 0;
//End todo
    
    // Start is called before the first frame update
    void Start()
    {
        _movePlayerUpdate = GridMoveUpdate;
        _movePlayerFixedUpdate = GridMoveFixedUpdate;
        _isGridMovement = true;
        
        // Initialization of free movement values
        _tanSpeed = speed * .707f;
        
        //initialization of
        InitializeGridPosition();
        _directionToKeyCode = new Dictionary<Direction, KeyCode>()
        {
            { Direction.Up, Player.MoveUpKey},
            { Direction.Down, Player.MoveDownKey},
            { Direction.Left, Player.MoveLeftKey},
            { Direction.Right, Player.MoveRightKey }
        };
    }

    // Update is called once per frame
    void Update()
    {
        _movePlayerUpdate();
    }

    private void FixedUpdate()
    {
        _movePlayerFixedUpdate();
    }
    
    private void FreeMoveUpdate()
    {
        // Intentionally empty
    }
    
    private void GridMoveUpdate()
    {
        Debug.Log("called");
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
                    if (Player.FacingDirection == _walkDirection)
                    {
                        UpdateMovementVals();
                        _walkOnce = true;
                    }
                }
                else
                {
                }

                /*else FacingDirection = _walkDirection;*/
            }
            //else we check to see if enough time has passed to consider the button held down
            else if (timeSincePress > KeyPushedDuration)
            {
                /*Debug.Log("Turning on walk");*/
                _walkOn = true;
                Player.FacingDirection = _walkDirection;
            }

            CheckKeys();
        }
        //else check for key being pressed down
        else
        {
            CheckKeys();
        }
    }

    private void FreeMoveFixedUpdate()
    {
        _xMovement = 0;
        _yMovement = 0;
        if (Input.GetKey(Player.MoveUpKey)) _yMovement += 1f;
        if (Input.GetKey(Player.MoveDownKey)) _yMovement -= 1f;
        if (Input.GetKey(Player.MoveRightKey)) _xMovement += 1f;
        if (Input.GetKey(Player.MoveLeftKey)) _xMovement -= 1f;
        
        // Check if the player is moving diagonally
        var absX = Mathf.Abs(_xMovement);
        var absY = Mathf.Abs(_yMovement);
        if (absY > 0.5f)
        {
            Player.FacingDirection = _yMovement > 0f ? Direction.Up : Direction.Down;
            if (absX > 0.5f)
            {
                _xMovement *= _tanSpeed;
                _yMovement *= _tanSpeed;
            }
            else
            {
                _yMovement *= speed;
            }
        }
        else if (absX > 0.5f)
        {
            Player.FacingDirection = _xMovement > 0f ? Direction.Right : Direction.Left;
            _xMovement *= speed;
        }

        playerRigidbody.velocity = new Vector2(_xMovement, _yMovement);
    }
    
    private void GridMoveFixedUpdate()
    {
        if (Walking)
        {
            playerRigidbody.MovePosition(Vector2.Lerp(_startPos, _endPos, (Time.time - _walkStartTime) / WalkDuration));
            if (playerRigidbody.position == _endPos)
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
        }*/
    }
    
    void UpdateColliders()
    {
        // For rotating the interact collider to the side the player is facing
        interactColliderTransform.transform.localRotation = Quaternion.Euler(0, 0, Player.FacingDirection switch
        {
            Direction.Up => 0,
            Direction.Down => 180,
            Direction.Left => 90,
            Direction.Right => 270,
            _ => 0
        });
    }

    private void InitializeGridPosition()
    {
        var position = transform.position;
        position = new Vector3(Mathf.Floor(position.x) + 0.5f, Mathf.Floor(position.y) + 0.5f,
            position.z);
        transform.position = position;
        _startPos = _endPos = position;
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

    */
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
    }
} 
