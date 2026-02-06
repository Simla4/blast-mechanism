using UnityEngine;
using sb.eventbus;


public class OnRocketActivated : IEvent
    {
        public Vector2Int position;
        public RocketDirections direction;
        public float animationDuration;

        public OnRocketActivated(Vector2Int position, RocketDirections direction, float animationDuration)
        {
            this.position = position;
            this.direction = direction;
            this.animationDuration = animationDuration;
        }
    }