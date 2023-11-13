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
    public static PlayerController playerControllerReference;
    
    /// <summary>
    /// How long each step should take
    /// </summary>
    private const float GridWalkDuration = .25f;
    /// <summary>
    /// The length of a single step 
    /// </summary>
    private const float GridStepSize = 1f;
    
    /// <summary>
    /// The duration to consider a key held down rather than pressed
    /// </summary>
    private const float KeyHeldMinDuration = .25f;

    /// <summary>
    /// Rigidbody attached to the player object
    /// </summary>
    public Rigidbody2D playerRigidbody;
    
    /// <summary>
    /// The transform of the Gameobject the interact collider is attachedd to
    /// </summary>
    public Transform interactColliderTransform;
    
    /// <summary>
    /// The speed the player moves when in free movement mode
    /// </summary>
    public float freeMoveSpeed = 5f;
    
    // Uses these keys for movement; public in case we have time to implement game options
    public KeyCode moveUpKey = KeyCode.W;
    public KeyCode moveDownKey = KeyCode.S;
    public KeyCode moveLeftKey = KeyCode.A;
    public KeyCode moveRightKey = KeyCode.D;

    /// <summary>
    /// Whether the player is in grid-based movement and currently walking
    /// </summary>
    [NonSerialized] public bool WalkingGrid = false;

    /// <summary>
    /// The total steps taken by the player (grid-based movement only).
    /// Currently untracked.
    /// </summary>
    [NonSerialized] public ulong StepsTaken = 0;
    
    private MovePlayerUpdate _movePlayerUpdate;
    private MovePlayerFixedUpdate _movePlayerFixedUpdate;
    private DirectionChangeEventHandler _updateColliderHandler;
    
    private delegate void MovePlayerUpdate();

    private delegate void MovePlayerFixedUpdate();

    private bool _isGridMovement;
    private float _xMovement;
    private float _yMovement;
    private float _tanSpeed;
    
    ///<summary>
    /// Determines if the <see cref="playerRigidbody"/> should keep moving once it reaches its position
    /// </summary>
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
    
    public void toggleMovement()
    {
        IsGridMovement = !IsGridMovement;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (playerControllerReference is not null)
        {
            Destroy(this.gameObject);
            return;
        }

        playerControllerReference = this;
        DontDestroyOnLoad(gameObject);
        
        // Initialize movement functions
        _movePlayerUpdate = GridMoveUpdate;
        _movePlayerFixedUpdate = GridMoveFixedUpdate;
        _isGridMovement = true;
        
        _updateColliderHandler = UpdateColliders;
        SubscribeEvents();
        
        // Initialization of free movement values
        _tanSpeed = freeMoveSpeed * .707f;
        
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

    private void OnDestroy()
    {
        UnsubscribeEvents();
        if (playerControllerReference == this)
            playerControllerReference = null;
    }

    private void FreeMoveUpdate()
    {
        // Intentionally empty
    }
    
    private void GridMoveUpdate()
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
                if (timeSincePress <= KeyHeldMinDuration)
                {
                    if (Player.FacingDirection == _walkDirection)
                    {
                        UpdateMovementVals();
                        _walkOnce = true;
                    }
                    else
                    {
                        Player.FacingDirection = _walkDirection;
                    }
                }
                else
                {
                    // do nothing? Key is considered held down at this point, but FixedUpdate turns off walking flag automatically
                    // probably not needed
                }

                /*else FacingDirection = _walkDirection;*/
            }
            //else we check to see if enough time has passed to consider the button held down
            else if (timeSincePress > KeyHeldMinDuration)
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
                _yMovement *= freeMoveSpeed;
            }
        }
        else if (absX > 0.5f)
        {
            Player.FacingDirection = _xMovement > 0f ? Direction.Right : Direction.Left;
            _xMovement *= freeMoveSpeed;
        }

        playerRigidbody.velocity = new Vector2(_xMovement, _yMovement);
    }
    
    private void GridMoveFixedUpdate()
    {
        if (WalkingGrid)
        {
            playerRigidbody.MovePosition(Vector2.Lerp(_startPos, _endPos, (Time.time - _walkStartTime) / GridWalkDuration));
            if (playerRigidbody.position == _endPos)
            {
                WalkingGrid = false;
                /*if (StepsTaken == UInt64.MaxValue)
                {
                    StepsTaken = 0;
                }
                else
                {
                    StepsTaken++;
                }*/
            }

            _walkOnce = false;
        }
        else if (_walkOn || _walkOnce)
        {
            WalkingGrid = true;
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
    
    void UpdateColliders(Direction oldDirection, Direction newDirection)
    {
        // For rotating the interact collider to the side the player is facing
        interactColliderTransform.transform.localRotation = Quaternion.Euler(0, 0, newDirection switch
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
    
    private void UpdateMovementVals()
    {
        _walkStartTime = Time.time;
        _startPos = transform.position;
        _endPos = _walkDirection switch
        {
            Direction.Up => _startPos + GridStepSize * Vector2.up,
            Direction.Down => _startPos + GridStepSize * Vector2.down,
            Direction.Left => _startPos + GridStepSize * Vector2.left,
            Direction.Right => _startPos + GridStepSize * Vector2.right,
            _ => _endPos
        };
    }

    // Subscribes functions to events
    private void SubscribeEvents()
    {
        Player.directionChangeEvent += _updateColliderHandler;
    }

    private void UnsubscribeEvents()
    {
        Player.directionChangeEvent -= _updateColliderHandler;
    }
} 
