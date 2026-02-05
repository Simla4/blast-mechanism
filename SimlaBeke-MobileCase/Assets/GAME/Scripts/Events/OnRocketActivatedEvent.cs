using UnityEngine;
using sb.eventbus;


public class OnRocketActivated : IEvent
    {
        public Vector2Int position;
        public RocketDirections direction;

        public OnRocketActivated(Vector2Int position, RocketDirections direction)
        {
            this.position = position;
            this.direction = direction;
        }
    }