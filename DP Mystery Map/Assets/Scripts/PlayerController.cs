using System;
using System.Collections.Generic;
using System.Linq;
using PlayerInfo;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script includes movement and interact colliders.
/// </summary>
public class PlayerController : GameplayScript
{
    private bool wait = false;

    /// <summary>
    /// Flags for what is preventing movement and/or turning
    /// </summary>
    [Flags]
    public enum WalkBlockedFlags : short
    {
        /// <summary>
        /// The player's movement is unrestricted
        /// </summary>
        CanWalk = 0,

        /// <summary>
        /// The player can't move due to the pause menu being opened
        /// </summary>
        Paused = 1,

        /// <summary>
        /// The player can't walk due to an object being in front of them
        /// </summary>
        DirectionBlocked = 2,

        /// <summary>
        /// The player can't move because they are interacting with something
        /// </summary>
        Interacting = 4,

        /// <summary>
        /// An event is executing, so player can't move
        /// </summary>
        Event = 8,

        Dialogue = 16,

        GameLoad = 32,

        /// <summary>
        /// Flag combination indicating that the player can't walk or change facing direction
        /// Do not set something to this value. 
        /// </summary>
        CantMove = Paused | Interacting | Event | Dialogue | GameLoad,

        /// <summary>
        /// Flag combination indicating that the player can't walk. The player can still change facing direction.
        /// Do not set something to this value.
        /// </summary>
        CantWalk = DirectionBlocked | GameLoad
    }

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

    private const float DefaultStepSpeed = 1f;

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

    /// <summary>
    /// Alternative speed. Speed is adjusted by this value when one of the Player.MovementModifierKeys is held down
    /// </summary>
    public float speedModifier = 2f;

    private float _currentStepSpeed = DefaultStepSpeed;

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

    /// <summary>
    /// Whether the player is able to walk. This will not stop any executing movement (i.e., grid movement)
    /// This should be set by the paused function
    /// </summary>
    [NonSerialized] public WalkBlockedFlags WalkBlocked = WalkBlockedFlags.CanWalk;
    /*[NonSerialized] public bool canWalkPaused = true;*/

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
    private float _currentStepTime;

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
    private bool _stopWalking = false;

    public Animator playerAnimator;

    /// <summary>
    /// 2D Vector Storing Variables for The Player's Animation Controller
    /// </summary>
    Vector2 playerMovementController;

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

    public void StopWalking(bool waitForAnimFinish = false)
    {
        if (_isGridMovement)
        {
            if (waitForAnimFinish)
            {
                _stopWalking = true;
            }
            else
            {
                WalkingGrid = false;
                _walkOn = false;
                _walkOnce = false;
                _startPos = _endPos = transform.position;
                _currentStepTime = GridWalkDuration;
            }
        }
        else
        {
        }
    }

    public void toggleMovement()
    {
        IsGridMovement = !IsGridMovement;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        if (playerControllerReference is not null)
        {
            Destroy(this.gameObject);
            return;
        }

        base.Start();

        playerControllerReference = this;

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
            { Direction.Up, Player.MoveUpKey },
            { Direction.Down, Player.MoveDownKey },
            { Direction.Left, Player.MoveLeftKey },
            { Direction.Right, Player.MoveRightKey }
        };
    }

    // Update is called once per frame
    void Update()
    {
        //Only allow character movement when dialogue isn't active
        //Set player status to interacting once merged with main
        _movePlayerUpdate();
    }


    protected override void OnLevelLoad(Scene scene, LoadSceneMode mode)
    {
        base.OnLevelLoad(scene, mode);
        WalkBlocked &= ~WalkBlockedFlags.GameLoad;
        UpdateMovementVals();
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
        if ((WalkBlocked & WalkBlockedFlags.CantMove) != 0) return;
        CheckKeys();
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
            }

            if (!_keyHeld)
                return;

            //calculate time since the key was pressed
            var timeSincePressed = Time.time - _walkDirection switch
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
                if (!(timeSincePress <= KeyHeldMinDuration))
                    return;
                if (Player.FacingDirection != _walkDirection)
                {
                    Player.FacingDirection = _walkDirection;
                    return;
                }

                CheckKeys();
                // prevent the _endPos value from being updated in the middle of a step
                if (!WalkingGrid && CanWalk())
                {
                    UpdateMovementVals();
                    _walkOnce = true;
                }
            }
            //else we check to see if enough time has passed to consider the button held down
            else if (timeSincePress > KeyHeldMinDuration)
            {
                if (_stopWalking)
                {
                    _stopWalking = false;
                    UpdateMovementVals();
                    _walkOnce = false;
                    _walkOn = false;
                    return;
                }

                if (Player.FacingDirection != _walkDirection)
                {
                    Player.FacingDirection = _walkDirection;
                    wait = true;
                    return;
                }

                if (wait) return;
                if (CanWalk())

                    _walkOn = true;
            }

            playerAnimator.SetFloat("Speed", playerMovementController.sqrMagnitude);

            //Enable running animation if shift is held
            if (Input.GetKey(KeyCode.LeftShift))
                playerAnimator.SetBool("isRunning", true);
            else
                playerAnimator.SetBool("isRunning", false);
        }
        else
        {
            //Handle direction faced while moving and idle
            Player.SetIdleFacingDirection(playerAnimator);
            playerAnimator.SetFloat("Speed", 0.0f);
            playerAnimator.SetBool("isRunning", false);
        }

        switch (Player.FacingDirection)
        {
            case Direction.Left:
                playerMovementController.x = -1.0f;
                playerMovementController.y = 0.0f;
                break;
            case Direction.Right:
                playerMovementController.x = 1.0f;
                playerMovementController.y = 0.0f;
                break;
            case Direction.Up:
                playerMovementController.x = 0.0f;
                playerMovementController.y = 1.0f;
                break;
            case Direction.Down:
                playerMovementController.x = 0.0f;
                playerMovementController.y = -1.0f;
                break;
        }

        playerAnimator.SetFloat("Horizontal", playerMovementController.x);
        playerAnimator.SetFloat("Vertical", playerMovementController.y);
    }


    private bool CanWalk()
    {
        return ((WalkBlocked & WalkBlockedFlags.CantWalk) == WalkBlockedFlags.CanWalk);
    }

    private void FreeMoveFixedUpdate()
    {
        if (WalkBlocked != WalkBlockedFlags.CanWalk)
            return;
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
            //Player.FacingDirection = _yMovement > 0f ? Direction.Up : Direction.Down;
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
            //Player.FacingDirection = _xMovement > 0f ? Direction.Right : Direction.Left;
            _xMovement *= freeMoveSpeed;
        }

        if (Player.MovementModifierKeys.Any(key => Input.GetKey(key)))
        {
            _xMovement *= speedModifier;
            _yMovement *= speedModifier;
        }

        playerRigidbody.velocity = new Vector2(_xMovement, _yMovement);
    }

    private void GridMoveFixedUpdate()
    {
        wait = false;
        if (!WalkingGrid)
        {
            if ((_walkOn || _walkOnce) && WalkBlocked == WalkBlockedFlags.CanWalk)
            {
                WalkingGrid = true;
                UpdateMovementVals();
            }

            return;
        }

        _currentStepTime += Time.fixedDeltaTime;
        playerRigidbody.MovePosition(Vector2.Lerp(_startPos, _endPos,
            _currentStepTime * _currentStepSpeed / GridWalkDuration));
        _walkOnce = false;
        if (playerRigidbody.position == _endPos)
        {
            if ((_walkOn || _walkOnce) && WalkBlocked == WalkBlockedFlags.CanWalk)
            {
                WalkingGrid = true;
                UpdateMovementVals();
            }
            else
            {
                WalkingGrid = false;
            }
        }
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
        else if (Input.GetKeyDown(moveLeftKey))
        {
            _walkDirection = Direction.Left;
            _keyHeld = true;
            _leftKeyHeldStartTime = Time.time;
        }
        else if (Input.GetKeyDown(moveDownKey))
        {
            _walkDirection = Direction.Down;
            _keyHeld = true;
            _downKeyHeldStartTime = Time.time;
        }
        else if (Input.GetKeyDown(moveUpKey))
        {
            _walkDirection = Direction.Up;
            _keyHeld = true;
            _upKeyHeldStartTime = Time.time;
        }
    }

    private void UpdateMovementVals()
    {
        _currentStepTime = 0f;
        _startPos = transform.position;
        _endPos = _walkDirection switch
        {
            Direction.Up => _startPos + GridStepSize * Vector2.up,
            Direction.Down => _startPos + GridStepSize * Vector2.down,
            Direction.Left => _startPos + GridStepSize * Vector2.left,
            Direction.Right => _startPos + GridStepSize * Vector2.right,
            _ => _endPos
        };
        _currentStepSpeed =
            Player.MovementModifierKeys.Any(key => Input.GetKey(key)) ? speedModifier : DefaultStepSpeed;
    }

    // Subscribes functions to events
    private void SubscribeEvents()
    {
        Player.directionChangeEvent += _updateColliderHandler;
    }

    private void UnsubscribeEvents()
    {
        Player.directionChangeEvent -= _updateColliderHandler;
        SceneManager.sceneLoaded -= OnLevelLoad;
    }
}