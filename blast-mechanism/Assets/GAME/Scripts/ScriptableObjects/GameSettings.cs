using System;
 using System.Collections.Generic;
 using UnityEngine;
 using UnityEngine.Tilemaps;
 
 [CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/GameSettings", order = 1)]
 public class GameSettings : ScriptableObject
 {
     [Header("PowerUp Rules")]
     public List<PowerUpThreshold> powerUpThresholds;
 }

 [System.Serializable]
 public class PowerUpThreshold
 {
     public int requiredCount;
     public TileData powerUpData; 
 }