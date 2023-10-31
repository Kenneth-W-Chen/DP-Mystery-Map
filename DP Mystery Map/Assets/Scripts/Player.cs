using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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
    [Flags, Serializable]
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

        public static string SaveFilePath = $"{Path.Combine(PlayerSave.defaultSavePath, "save1")}";

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

        public static void loadData(PlayerSave save)
        {
            
        }
    }
    
    /// <summary>
    /// This class holds information for player info to be serialized.
    /// For class member documentation, see <see cref="Player"/> members of same names
    /// </summary>
    [System.Serializable]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class PlayerSave
    {
        /// <summary>
        /// The default path for save game files
        /// </summary>
        public static readonly string defaultSavePath = $"{Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),"DP-Map-Saves")}";
        /// <summary>
        /// BinaryFormatter used for serializing data
        /// </summary>
        public static BinaryFormatter bf = new BinaryFormatter();
        
        /*public KeyCode MoveUpKey;
        public KeyCode MoveDownKey;
        public KeyCode MoveLeftKey;
        public KeyCode MoveRightKey;*/
        
        public Item _collectedItems;
        public Direction _facingDirection;
        
        public Vector2 position;
        public string SaveFilePath;
        
        /// <summary>
        /// Saves player information to file
        /// </summary>
        /// <exception cref="Exception">No data able to be saved</exception>
        public void Save()
        {
            // consider removing, the save button should not be accessible outside of the game
            if (PlayerController.playerControllerReference is null)
                throw new Exception("No player object instantiated.");
            SaveData();
            SerializeData();
        }
        
        /// <summary>
        /// Loads the save file information from the disk to the object.
        /// Does not load the data into the game state. Use <see cref="loadData"/> to load game data.
        /// </summary>
        /// <param name="saveFileName">Name of the save file.</param>
        /// <param name="fullPath">Set to true if <see cref="saveFileName"/> is the full path of the save file. Defaults to only filename.</param>
        public void loadFile(string saveFileName, bool fullPath = false)
        {
            
        }

        public void loadData()
        {
            Player.loadData(this);
        }

        /// <summary>
        /// Saves current data to the object
        /// </summary>
        private void SaveData()
        {
            /*MoveUpKey = Player.MoveUpKey;
            MoveDownKey = Player.MoveDownKey;
            MoveLeftKey = Player.MoveDownKey;
            MoveRightKey = Player.MoveRightKey;*/
            SaveFilePath = Player.SaveFilePath;
            _collectedItems = Player.collectedItems;
            _facingDirection = Player.FacingDirection;
            position = (Vector2) PlayerController.playerControllerReference.transform.position;
        }

        private void SerializeData()
        {
            using (FileStream fs = new FileStream(SaveFilePath, FileMode.Create, FileAccess.Write))
            {
                bf.Serialize(fs, this);
            }
        }
    }
}