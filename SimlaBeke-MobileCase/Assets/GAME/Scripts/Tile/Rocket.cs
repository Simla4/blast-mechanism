using System;
using DG.Tweening;
using UnityEngine;
using sb.eventbus;

  public class Rocket : TileBase, IClickable
  {
      [Header("References")]
      [SerializeField] private RocketDirections direction;
      [SerializeField] private GameObject visualPartA;
      [SerializeField] private GameObject visualPartB;
      [SerializeField] private GameObject rocketParticleA;
      [SerializeField] private GameObject rocketParticleB;

      [Header("Settings")]
      [SerializeField] private float duration = 1f;
      
      private Vector3 visualPartAStartPosition;
      private Vector3 visualPartBStartPosition;
      private Pool<TileBase> tilePool;
      private Tween visualPartATween;
      private Tween visualPartBTween;

      public RocketDirections Direction => direction;


      private void Start()
      {
          visualPartAStartPosition = visualPartA.transform.localPosition;
          visualPartBStartPosition = visualPartB.transform.localPosition;
      }


      public void Init(RocketDirections dir)
      {
          direction = dir;
          SetupVisuals();
      }

      public void OnClickedTileEvent()
      {
          float timePerUnit = duration / GetDistance();
          
          EventBus<OnRocketActivated>.Emit(new OnRocketActivated(TilePosition, direction, duration, timePerUnit));
          EventBus<OnMoveCountChnagedEvent>.Emit(new OnMoveCountChnagedEvent());

          AnimateRocket();
      }

      private void SetupVisuals()
      {
          if (direction == RocketDirections.Vertical)
          {
              transform.localRotation = Quaternion.Euler(0, 0, 90);
          }
          else
          {
              transform.localRotation = Quaternion.identity;
          }
      }

      private void AnimateRocket()
      {
          rocketParticleA.SetActive(true);
          rocketParticleB.SetActive(true);

          if (visualPartATween != null)
          {
              visualPartATween.Kill();
          }

          if (visualPartBTween != null)
          {
              visualPartBTween.Kill();
          }
          
          if (direction == RocketDirections.Horizontal)
          {
              visualPartATween = visualPartA.transform.DOLocalMoveX(GetDistance(), duration);
              visualPartBTween = visualPartB.transform.DOLocalMoveX(-GetDistance(), duration).OnComplete(OnAnimationComplete);
          }
          else
          {
              visualPartATween = visualPartA.transform.DOLocalMoveX(GetDistance(), duration);
              visualPartBTween = visualPartB.transform.DOLocalMoveX(-GetDistance(), duration).OnComplete(OnAnimationComplete);
          }
      }

      private float GetDistance()
      {
          
          Camera mainCam = Camera.main;
          float screenHeight = 2f * mainCam.orthographicSize;
          float screenWidth = screenHeight * mainCam.aspect;

          float dynamicDistance = (direction == RocketDirections.Horizontal) ? screenWidth : screenHeight;
          
          return dynamicDistance;
      }
      
      private void OnAnimationComplete()
      {
          visualPartA.transform.localPosition = visualPartAStartPosition;
          visualPartB.transform.localPosition = visualPartBStartPosition;
          
          rocketParticleA.SetActive(false);
          rocketParticleB.SetActive(false);
          
          tilePool = PoolManager.Instance.GetPool(tileData.tileId);
          tilePool.ReturnToPool(this);
      }
  }


public enum RocketDirections 
{ 
    Horizontal, 
    Vertical 
}