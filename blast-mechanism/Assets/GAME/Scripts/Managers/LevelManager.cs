using System;
using System.Collections;
using System.Collections.Generic;
using sb.eventbus;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelManager : MonoSingleton<LevelManager>
{
    [Header("References")]
    [SerializeField] private List<LevelData> levels;
    
    private const string LEVEL_INDEX_KEY = "CURRENT_LEVEL_INDEX";

    
    private EventListener<OnMoveCountChnagedEvent> onBlockCollectedMoveCount;
    private EventListener<OnBlockCollected> onBlockCollectedGoals;
    private List<LevelGoals> remainingGoals = new List<LevelGoals>();
    private int remainingMoveCount;
    private int currentLevelIndex;

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
        currentLevelIndex = PlayerPrefs.GetInt(LEVEL_INDEX_KEY, 0); 
        LoadLevel(currentLevelIndex);
    }
    
    private void LoadLevel(int index)
    {
        remainingGoals.Clear();
        remainingGoals = new List<LevelGoals>();

        foreach (var goal in levels[index].levelGoals)
        {
            remainingGoals.Add(new LevelGoals 
            { 
                goalType = goal.goalType, 
                count = goal.count 
            });
        }

        remainingMoveCount = levels[index].moveCount;
        
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
    
        if (spawnedGoals == null || spawnedGoals.Count == 0 || spawnedGoals.Count != remainingGoals.Count)
        {
            return;
        }

        for(int i = 0; i < spawnedGoals.Count; i++)
        {
            if (spawnedGoals[i] == null || !spawnedGoals[i].IsDone())
            {
                return;
            }
        }
    
        EventBus<LevelCompletedEvent>.Emit(new LevelCompletedEvent(true));
    }
    
    public void LoadNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex >= levels.Count)
        {
            currentLevelIndex = 0;
        }

        PlayerPrefs.SetInt(LEVEL_INDEX_KEY, currentLevelIndex);
        LoadLevel(currentLevelIndex);
    }
    
    public void RetryLevel()
    {
        LoadLevel(currentLevelIndex);
    }



    public List<LevelGoals> GetGoals()
    {
        return levels[currentLevelIndex].levelGoals;
        
    }

    public LevelData GetLevelData()
    {
        return levels[currentLevelIndex];
    }
}
