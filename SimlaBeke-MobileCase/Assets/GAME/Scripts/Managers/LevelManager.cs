using System;
using System.Collections;
using System.Collections.Generic;
using sb.eventbus;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Refferances")]
    [SerializeField] private LevelData levelData;
    
    private EventListener<OnMoveCountChnagedEvent> onBlockCollectedMoveCount;
    private EventListener<OnBlockCollected> onBlockCollectedGoals;
    private List<LevelGoals> remainingGoals = new List<LevelGoals>();
    private int remainingMoveCount;

    private void OnEnable()
    {
        onBlockCollectedMoveCount = new EventListener<OnMoveCountChnagedEvent>(CheckMoveCount);
        EventBus<OnMoveCountChnagedEvent>.AddListener(onBlockCollectedMoveCount);
        
        onBlockCollectedGoals = new EventListener<OnBlockCollected>(CheckGoals);
        EventBus<OnBlockCollected>.AddListener(onBlockCollectedGoals);
    }

    private void OnDisable()
    {
        EventBus<OnMoveCountChnagedEvent>.RemoveListener(onBlockCollectedMoveCount);
        EventBus<OnBlockCollected>.RemoveListener(onBlockCollectedGoals);
    }

    private void Start()
    {
        remainingGoals = new List<LevelGoals>();

        foreach (var goal in levelData.levelGoals)
        {
            remainingGoals.Add(new LevelGoals 
            { 
                goalType = goal.goalType, 
                count = goal.count 
            });
        }

        remainingMoveCount = levelData.moveCount;
        EventBus<OnGameStartEvent>.Emit(new OnGameStartEvent(remainingMoveCount, remainingGoals));
    }

    private void CheckMoveCount(OnMoveCountChnagedEvent e)
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
                // 1. Önce hafızadaki değeri azalt
                remainingGoals[i].count--; 
            
                // 2. Azalmış olan yeni değeri bir değişkene al
                int updatedCount = remainingGoals[i].count;

                // 3. UI'ya bu taze değeri gönder
                EventBus<ChangeGoalsUIEvent>.Emit(new ChangeGoalsUIEvent(updatedCount, remainingGoals[i].goalType));
            
                break;
            }
        }
    }
}
