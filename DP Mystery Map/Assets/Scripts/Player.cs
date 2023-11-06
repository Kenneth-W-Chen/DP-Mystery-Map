using System;
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
    /// Bitflag for items
    /// </summary>
    [Flags]
    public enum Item:short
    {
       None = 0,
       Keys = 1,
       Phone = 2,
       Backpack = 4,
       Book = 8,
       Pen = 16,
       CreamCheese = 32,
       All = 63
    }

    /// <summary>
    /// Event handler delegate that is fired when the player collects all items.
    /// </summary>
    public delegate void ALlItemsCollectedEventHandler();
    
    /// <summary>
    /// Event handler delegate that is fired when the player's direction changes.
    /// </summary>
    /// <param name="oldValue">The <see cref="Direction"/> before the change</param>
    /// <param name="newValue">The <see cref="Direction"/> after the change</param>
    public delegate void DirectionChangeEventHandler(Direction oldValue, Direction newValue);

    /// <summary>
    /// Event handler delegate that is fired when the player's health changes.
    /// </summary>
    /// <param name="oldValue">The health before the change</param>
    /// <param name="newValue">The health after the change</param>
    public delegate void HealthChangeEventHandler(int oldValue, int newValue);
    
    /// <summary>
    /// This class holds settings and statuses for the player. Does not inherit MonoBehavior and should not use any Unity objects.
    /// </summary>
    public static class Player
    {
        public const int MaxHealth = 100;
        
        public static KeyCode MoveUpKey = KeyCode.W;
        public static KeyCode MoveDownKey = KeyCode.S;
        public static KeyCode MoveLeftKey = KeyCode.A;
        public static KeyCode MoveRightKey = KeyCode.D;

        /// <summary>
        /// The items the player has collected
        /// </summary>
        private static Item _collectedItems = Item.None;
        
        /// <summary>
        /// The direction the player is facing
        /// </summary>
        private static Direction _facingDirection = Direction.Up;

        /// <summary>
        /// Player health
        /// </summary>
        private static int _health = MaxHealth;

        /// <summary>
        /// Subscribe to this event to execute a command when the player collects all items.
        /// This event should only fire once.
        /// </summary>
        public static event ALlItemsCollectedEventHandler allItemsCollectedEvent;
        
        /// <summary>
        /// Subscribe to this event to execute a command when the direction the player faces changes.
        /// This only fires when the direction changes. It does not fire whenever the FacingDirection variable is set. 
        /// </summary>
        public static event DirectionChangeEventHandler directionChangeEvent;

        /// <summary>
        /// Subscribe to this event to execute a command when the player's health changes.
        /// </summary>
        public static event HealthChangeEventHandler healthChangeEvent;
        
        /// <summary>
        /// Gets or sets the player's facing direction information. Fires a <see cref="directionChangeEvent"/> if the value is set to a different value.
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
                directionChangeEvent?.Invoke(oldValue, _facingDirection);
            }
        }

        /// <summary>
        /// Gets or sets the player's health. Fires a <see cref="healthChangeEvent"/> if health changes.
        /// </summary>
        public static int Health
        {
            get => _health;
            set
            {
                if (_health == value)
                    return;
                int oldValue = _health;
                // prevents the player from having health points over max value
                _health = value>MaxHealth?MaxHealth:value;
                healthChangeEvent?.Invoke(oldValue, _health);
            }
        }

        /// <summary>
        /// Gets or sets the items the player has collected. Fires a <see cref="allItemsCollectedEvent"/> when all items are collected.
        /// Collected items cannot be removed.
        /// </summary>
        public static Item collectedItems
        {
            get => _collectedItems;
            set
            {
                // prevents items from being removed, ignores setting the value to itself, and prevents setting invalid values
                if ((_collectedItems & value) != _collectedItems || _collectedItems == value || (short)value > 63)
                    return;
                _collectedItems = value;
                if((short)value == 63)
                    allItemsCollectedEvent?.Invoke();
            }
        }

        /// <summary>
        /// Resets control bindings to the defaults.
        /// </summary>
        public static void resetControls()
        {
            MoveUpKey = KeyCode.W;
            MoveDownKey = KeyCode.S;
            MoveLeftKey = KeyCode.A;
            MoveRightKey = KeyCode.D;            
        }

        /// <summary>
        /// Resets player values
        /// </summary>
        public static void resetValues()
        {
            _collectedItems = Item.None;
            _facingDirection = Direction.Up;
            _health = MaxHealth;
        }
    }
    
}