using UnityEngine;

namespace PlayerInfo
{
    /// <summary>
    /// Enum that defines player's facing direction
    /// </summary>
    public enum Direction
    {
        None = 0,
        Up = 1,
        Down = 2,
        Left = 3,
        Right = 4
    }

    /// <summary>
    /// Event handler delegate that is fired when the player's direction changes.
    /// </summary>
    /// <param name="oldValue">The <see cref="Direction"/> before the change</param>
    /// <param name="newValue">The <see cref="Direction"/> after the change</param>
    /// <seealso cref="DirectionChangeEventHandler"/>
    public delegate void DirectionChangeEventHandler(Direction oldValue, Direction newValue);
    
    /// <summary>
    /// This class holds settings and statuses for the player. Does not inherit MonoBehavior and should not use any Unity objects.
    /// </summary>
    public static class Player
    {
        
        public static KeyCode MoveUpKey = KeyCode.W;
        public static KeyCode MoveDownKey = KeyCode.S;
        public static KeyCode MoveLeftKey = KeyCode.A;
        public static KeyCode MoveRightKey = KeyCode.D;
        
        /// <summary>
        /// The direction the player is facing
        /// </summary>
        private static Direction _facingDirection = Direction.Up;
        
        /// <summary>
        /// Subscribe to this event to execute a command when the direction the player faces changes.
        /// This only fires when the direction changes. It does not fire whenever the FacingDirection variable is set. 
        /// </summary>
        public static event DirectionChangeEventHandler DirectionChangeEvent;
        /// <summary>
        /// Gets or sets the player's facing direction information. Fires a <see cref="DirectionChangeEvent"/> if the value is set to a different value.
        /// </summary>
        public static Direction FacingDirection
        {
            get => _facingDirection;
            set
            {
                if (_facingDirection == value)
                    return;
                Direction oldValue = _facingDirection;
                _facingDirection = value;
                DirectionChangeEvent?.Invoke(oldValue, _facingDirection);
            }
        }
    }
}