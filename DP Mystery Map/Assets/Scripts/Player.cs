using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    /// Enum describing which major the player has
    /// </summary>
    public enum Major:short
    {
        None = 0,
        ComputerEngineering = 1,
        ComputerScience = 2,
        ElectricalEngineering = 3,
        MechanicalEngineering = 4
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

        public static readonly Dictionary<Major, string> majorToString = new Dictionary<Major, string>()
        {
            { Major.None, "None" },
            { Major.ComputerScience, "Computer Science" },
            { Major.ComputerEngineering, "Computer Engineering" },
            { Major.ElectricalEngineering, "Electrical Engineering" },
            { Major.MechanicalEngineering, "Mechanical Engineering" }
        };
        
        public static KeyCode MoveUpKey = KeyCode.W;
        public static KeyCode MoveDownKey = KeyCode.S;
        public static KeyCode MoveLeftKey = KeyCode.A;
        public static KeyCode MoveRightKey = KeyCode.D;
        public static KeyCode InteractKey = KeyCode.P;
        public static KeyCode PauseKey = KeyCode.Escape;

        public static string SaveFilePath = $"{Path.Combine(PlayerSave.defaultSavePath, "save1")}";

        
        
        private static readonly GameObject PlayerPrefab = (GameObject) UnityEngine.Resources.Load("prefabs/player", typeof(GameObject));
        /// <summary>
        /// The items the player has collected
        /// </summary>
        private static Item _collectedItems = Item.None;
        
        /// <summary>
        /// The direction the player is facing
        /// </summary>
        private static Direction _facingDirection = Direction.Up;

        /// <summary>
        /// The player's major
        /// </summary>
        private static Major _major = Major.None;
        
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

        public static Major major
        {
            get=>_major;
            set => _major = value;
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
            SaveFilePath = save.SaveFilePath;
            _collectedItems = save._collectedItems;
            _facingDirection = save._position.direction;
            _major = save._major;
            _health = MaxHealth;
            if (PlayerController.playerControllerReference is not null)
                PlayerController.playerControllerReference.transform.position = save._position.Position;
            else
            {
                UnityEngine.Object.Instantiate(PlayerPrefab);
            }
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
        public Major _major;
        public PlayerPosition _position;
        public bool isFloorTwo;
        
        [NonSerialized]
        public string SaveFilePath;

        /// <summary>
        /// Loads the save file information from the disk to the object.
        /// Does not load the data into the game state. Use <see cref="loadData"/> to load game data.
        /// </summary>
        /// <param name="saveFileName">Name of the save file.</param>
        /// <param name="fullPath">Set to true if <see cref="saveFileName"/> is the full path of the save file. Defaults to only filename.</param>
        /// <returns>A <see cref="PlayerSave"/> object if a save file was found. Returns null otherwise</returns>
        public static PlayerSave GetSaveFile(string saveFileName, bool fullPath = false)
        {
            if (!fullPath)
                saveFileName = Path.Combine(defaultSavePath, saveFileName);
            if(!File.Exists(saveFileName))
                return null;
            PlayerSave s = new PlayerSave();
            using (FileStream fs = new FileStream(saveFileName, FileMode.Open, FileAccess.Read))
            {
                object deserialized = bf.Deserialize(fs);
                if (deserialized is not PlayerSave temp)
                {
                    return null;
                }
                else
                {
                    s._collectedItems = temp._collectedItems;
                    s._position = new PlayerPosition(ref temp._position);
                    s._major = temp._major;
                    s.isFloorTwo = temp.isFloorTwo;
                    s.SaveFilePath = saveFileName;
                }
            }

            return s;
        }
        
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
            if (!fullPath)
                saveFileName = Path.Combine(defaultSavePath, saveFileName);
            
            using (FileStream fs = new FileStream(saveFileName, FileMode.Open, FileAccess.Read))
            {
                object deserialized = bf.Deserialize(fs);
                if (deserialized is not PlayerSave temp)
                {
                    throw new Exception("Save file read exception");
                }
                else
                {
                    this._collectedItems = temp._collectedItems;
                    this._position = new PlayerPosition(ref temp._position);
                    this._major = temp._major;
                    this.isFloorTwo = temp.isFloorTwo;
                    this.SaveFilePath = saveFileName;
                }
            }
        }

        /// <summary>
        /// Loads the objects fields into <see cref="Player"/>'s static fields
        /// </summary>
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
            isFloorTwo = SceneManager.GetActiveScene().buildIndex == 2;
            _position = new PlayerPosition(PlayerController.playerControllerReference.transform.position, Player.FacingDirection);
        }

        private void SerializeData()
        {
            if(!Directory.Exists(Path.GetDirectoryName(SaveFilePath)))
                Directory.CreateDirectory(Path.GetDirectoryName(SaveFilePath)!);
            try
            {
                //temporarily save data to a -temp file, in case serialization doesn't work
                using (FileStream fs = new FileStream(SaveFilePath + "-temp", FileMode.Create, FileAccess.Write))
                {
                    bf.Serialize(fs, this);
                }
                // remove old save file
                if(File.Exists(SaveFilePath))
                    File.Delete(SaveFilePath);
                //rename temp file
                File.Move(SaveFilePath+"-temp",SaveFilePath);
            }
            catch (Exception e)
            {
                // do not save the file or keep the temp file
                if(File.Exists(SaveFilePath+"-temp"))
                    File.Delete(SaveFilePath+"-temp");
                return;
            }
        }
    }

    [Serializable]
    public struct PlayerPosition
    {
        public SerializableVector2 Position;
        public Direction direction;

        /// <summary>
        /// Parameter constructor
        /// </summary>
        /// <param name="position">The player's position relative to parent</param>
        /// <param name="direction">The direction the player is facing</param>
        public PlayerPosition(Vector2 position, Direction direction)
        {
            this.Position = new SerializableVector2(position);
            this.direction = direction;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="source">The source PlayerPosition object to copy</param>
        public PlayerPosition(ref PlayerPosition source)
        {
            this.Position = source.Position;
            this.direction = source.direction;
        }
    }

    [Serializable]
    public struct SerializableVector2
    {
        public float x, y;

        public SerializableVector2(Vector2 vector2)
        {
            this.x = vector2.x;
            this.y = vector2.y;
        }

        public SerializableVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2 ToVector2()
        {
            return new Vector2(x, y);
        }

        public static implicit operator Vector2(SerializableVector2 s) => new Vector2(s.x, s.y);
        public static implicit operator Vector3(SerializableVector2 s) => new Vector3(s.x, s.y, 0f);
        public static implicit operator SerializableVector2(Vector2 v) => new SerializableVector2(v);
    }
}