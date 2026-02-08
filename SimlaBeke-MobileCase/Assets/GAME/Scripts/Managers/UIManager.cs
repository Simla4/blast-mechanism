using System;
using System.Collections.Generic;
using sb.eventbus;
using TMPro;
using UnityEngine;

public class UIManager : MonoSingleton<UIManager>
{
    [Header("Move Settings")]
    [SerializeField] private TextMeshProUGUI moveCountText;
    [SerializeField] private GameObject uiPrefab;
    
    [Header("Goal Settings")]
    [SerializeField] private Transform goalsContainer;
    [SerializeField] private GoalUIElement goalPrefab;

    private List<GoalUIElement> spawnedGoals = new List<GoalUIElement>();
    private EventListener<OnGameStartEvent> onGameStart;
    private EventListener<ChangeGoalsUIEvent> onChangeGoals;
    private EventListener<ChangeMoveCountUIEvent> onChangeMoveCount;
    public Dictionary<PanelID, IPanel> PanelsByID { get; private set; } = new();

    private void OnEnable()
    {
        onGameStart = new EventListener<OnGameStartEvent>(SetupUI);
        EventBus<OnGameStartEvent>.AddListener(onGameStart);

        onChangeGoals = new EventListener<ChangeGoalsUIEvent>(OnGoalChanged);
        EventBus<ChangeGoalsUIEvent>.AddListener(onChangeGoals);

        onChangeMoveCount = new EventListener<ChangeMoveCountUIEvent>(OnMoveCountChanged);
        EventBus<ChangeMoveCountUIEvent>.AddListener(onChangeMoveCount);
    }

    private void OnDisable()
    {
        EventBus<OnGameStartEvent>.RemoveListener(onGameStart);
        EventBus<ChangeGoalsUIEvent>.RemoveListener(onChangeGoals);
        EventBus<ChangeMoveCountUIEvent>.RemoveListener(onChangeMoveCount);
    }
    
    private void Awake()
    {
        DontDestroyOnLoad(uiPrefab);
    }

    public void SetupUI(OnGameStartEvent e)
    {
        moveCountText.text = e.moves.ToString();

        foreach (var goal in e.levelGoals)
        {
            var newGoal = Instantiate(goalPrefab, goalsContainer);
            newGoal.Initialize(goal.goalType.tileId, goal.goalType.tileIcon, goal.count);
            spawnedGoals.Add(newGoal);
        }
    }

    public void OnGoalChanged(ChangeGoalsUIEvent e)
    {
        var element = spawnedGoals.Find(x => x.TargetTileId == e.TileData.tileId);
        if (element != null)
        {
            element.UpdateUI(e.newGoal);
        }
    }

    public void OnMoveCountChanged(ChangeMoveCountUIEvent e)
    {
        moveCountText.text = e.newValue.ToString();
    }

    public GoalUIElement GetGoalUIElement(string id)
    {
        foreach (var goal in spawnedGoals)
        {
            if (goal.TargetTileId == id)
            {
                return goal;
            }
        }
        
        return null;
    }
    
    public void ShowPanel(PanelID panelID)
    {
        if (!PanelsByID.ContainsKey(panelID))
            return;

        PanelsByID[panelID].ShowPanelAnimated();
    }

    public void HidePanel(PanelID panelID)
    {
        if (!PanelsByID.ContainsKey(panelID))
            return;

        PanelsByID[panelID].HidePanelAnimated();
    }

    public void HideAllPanels()
    {
        foreach (var panel in PanelsByID.Values)
        {
            panel.HidePanelAnimated();
        }
    }

    public void AddPanel(IPanel panel)
    {
        if (PanelsByID.ContainsKey(panel.PanelID))
            return;

        PanelsByID.Add(panel.PanelID, panel);
    }

    public void RemovePanel(IPanel panel)
    {
        if (!PanelsByID.ContainsKey(panel.PanelID))
            return;

        PanelsByID.Remove(panel.PanelID);
    }
    
}


public enum PanelID
{
    WinPanel = 4,
    LosePanel = 5
}
public enum PanelAnimationTypes
{
    Fade,
    Scale,
}