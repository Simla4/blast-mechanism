using System;
using System.Collections;
using System.Collections.Generic;
using sb.eventbus;
using UnityEngine;

public class LevelManager : MonoSingleton<LevelManager>
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

        if (remainingMoveCount <= 0)
        {
            EventBus<LevelCompletedEvent>.Emit(new LevelCompletedEvent(false));
        }
    }

    private void CheckGoals(OnBlockCollected e)
    {
        for (int i = 0; i < remainingGoals.Count; i++)
        {
            if (e.tileData.tileId == remainingGoals[i].goalType.tileId)
            {
                remainingGoals[i].count--; 
            
                int updatedCount = remainingGoals[i].count;

                EventBus<ChangeGoalsUIEvent>.Emit(new ChangeGoalsUIEvent(updatedCount, remainingGoals[i].goalType));
                CheckLevelSuccess();
            
                break;
            }
        }
    }

    private void CheckLevelSuccess()
    {
        var spawnedGoals = UIManager.Instance.GetSpawnedGoals();
        
        for(int i = 0; i < spawnedGoals.Count; i++)
        {
            if (!spawnedGoals[i].IsDone())
            {
                return;
            }
        }
        
        EventBus<LevelCompletedEvent>.Emit(new LevelCompletedEvent(true));
    }

    public List<LevelGoals> GetGoals()
    {
        return levelData.levelGoals;
        
    }

    public LevelData GetLevelData()
    {
        return levelData;
    }
}
