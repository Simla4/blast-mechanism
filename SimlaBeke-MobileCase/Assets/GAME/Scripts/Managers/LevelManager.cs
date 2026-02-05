using System;
using System.Collections;
using System.Collections.Generic;
using sb.eventbus;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Refferances")]
    [SerializeField] private LevelData levelData;
    
    private EventListener<OnBlockCollected> onBlockCollectedMoveCount;
    private EventListener<OnBlockCollected> onBlockCollectedGoals;
    private List<LevelGoals> remainingGoals = new List<LevelGoals>();
    private int remainingMoveCount;

    private void OnEnable()
    {
        onBlockCollectedMoveCount = new EventListener<OnBlockCollected>(CheckMoveCount);
        EventBus<OnBlockCollected>.AddListener(onBlockCollectedMoveCount);
        
        onBlockCollectedGoals = new EventListener<OnBlockCollected>(CheckGoals);
        EventBus<OnBlockCollected>.AddListener(onBlockCollectedGoals);
    }

    private void OnDisable()
    {
        EventBus<OnBlockCollected>.RemoveListener(onBlockCollectedMoveCount);
        EventBus<OnBlockCollected>.RemoveListener(onBlockCollectedGoals);
    }

    private void Start()
    {
        remainingGoals = levelData.levelGoals;
        remainingMoveCount = levelData.moveCount;
    }

    private void CheckMoveCount(OnBlockCollected e)
    {
        remainingMoveCount--;
        EventBus<ChangeMoveCountUIEvent>.Emit(new ChangeMoveCountUIEvent( remainingMoveCount));
    }

    private void CheckGoals(OnBlockCollected e)
    {
        for (int i = 0; i < remainingGoals.Count; i++)
        {
            if (e.tileData.tileId == remainingGoals[i].goalType.tileId)
            {
                var newGoal = remainingGoals[i].count--;
                EventBus<ChangeGoalsUIEvent>.Emit(new ChangeGoalsUIEvent(newGoal, remainingGoals[i].goalType));
                break;
            }
        }
    }
}
