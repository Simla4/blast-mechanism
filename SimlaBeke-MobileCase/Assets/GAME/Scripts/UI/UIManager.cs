using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Move Settings")]
    [SerializeField] private TextMeshProUGUI moveCountText;
    
    [Header("Goal Settings")]
    [SerializeField] private Transform goalsContainer;
    [SerializeField] private GoalUIElement goalPrefab;

    private List<GoalUIElement> _spawnedGoals = new List<GoalUIElement>();

    // 1. ADIM: Oyun başladığında hedefleri oluştur
    public void SetupUI(int moves, List<LevelGoals> goals)
    {
        moveCountText.text = moves.ToString();

        foreach (var goal in goals)
        {
            var newGoal = Instantiate(goalPrefab, goalsContainer);
            newGoal.Initialize(goal.goalType.tileId, goal.goalType.tileIcon, goal.count);
            _spawnedGoals.Add(newGoal);
        }
    }

    // 2. ADIM: EventBus'tan gelen veriye göre güncelle
    // LevelManager'dan "OnGoalUpdatedEvent" fırlatıldığında bunu çağır
    public void OnGoalChanged(string id, int remaining)
    {
        var element = _spawnedGoals.Find(x => x.TargetTileId == id);
        if (element != null)
        {
            element.UpdateUI(remaining);
        }
    }

    public void OnMoveCountChanged(int remainingMoves)
    {
        moveCountText.text = remainingMoves.ToString();
        
        // Kritik: Hamle sayısı azaldığında metni salla veya kırmızı yap
        if(remainingMoves <= 5) moveCountText.color = Color.red;
    }
}