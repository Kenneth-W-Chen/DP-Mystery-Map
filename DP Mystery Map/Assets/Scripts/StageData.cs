using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace PlayerInfo
{
    public static class StageData
    {
        public static readonly ReadOnlyCollection<PlayerPosition> FloorOneToFloorTwoPos = new ReadOnlyCollection<PlayerPosition>(new List<PlayerPosition>()
        {
            
        });
        
        public static readonly ReadOnlyCollection<PlayerPosition> FloorTwoToFloorOnePos = new ReadOnlyCollection<PlayerPosition>(new List<PlayerPosition>()
        {
            new PlayerPosition(new Vector2(-208.5f, 28.5f), Direction.Down),
            new PlayerPosition(new Vector2(117.5f, 142.5f), Direction.Left),
            new PlayerPosition(new Vector2(118.5f, 15.5f), Direction.Up),
            new PlayerPosition(new Vector2(255.5f, -119.5f), Direction.Down),
        });
    }
}