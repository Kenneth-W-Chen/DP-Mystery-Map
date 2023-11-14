using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace PlayerInfo
{
    public static class StageData
    {
        public static readonly ReadOnlyCollection<PlayerPosition> FloorOneToFloorTwoPos = new ReadOnlyCollection<PlayerPosition>(new List<PlayerPosition>()
        {
            new PlayerPosition(new Vector2(-250.5f, 160.5f), Direction.Down),
            new PlayerPosition(new Vector2(144.5f, 206.5f), Direction.Down),
            new PlayerPosition(new Vector2(142.5f, 136.5f), Direction.Up),
            new PlayerPosition(new Vector2(313.5f,-22.5f), Direction.Down),
            new PlayerPosition(new Vector2(18.5f, 2.5f), Direction.Up)
        });
        
        public static readonly ReadOnlyCollection<PlayerPosition> FloorTwoToFloorOnePos = new ReadOnlyCollection<PlayerPosition>(new List<PlayerPosition>()
        {
            new PlayerPosition(new Vector2(-208.5f, 28.5f), Direction.Down),
            new PlayerPosition(new Vector2(117.5f, 142.5f), Direction.Left),
            new PlayerPosition(new Vector2(118.5f, 15.5f), Direction.Up),
            new PlayerPosition(new Vector2(255.5f, -119.5f), Direction.Down),
            new PlayerPosition(new Vector2(54.5f, -125.5f), Direction.Left),
        });
    }
}