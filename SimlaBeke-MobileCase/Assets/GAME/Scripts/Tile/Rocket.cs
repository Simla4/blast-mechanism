using System;
using DG.Tweening;
using UnityEngine;
using sb.eventbus;

  public class Rocket : TileBase, IClickable
  {
      [Header("References")]
      [SerializeField] private RocketDirections direction;
      [SerializeField] private GameObject visualPartA; // sol/üst
      [SerializeField] private GameObject visualPartB; // sağ/alt

      [Header("Settings")]
      [SerializeField] private float duration = 1f;
      
      private Vector3 visualPartAStartPosition;
      private Vector3 visualPartBStartPosition;
      private Pool<TileBase> tilePool;

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
          // 1) Board temizliği – görselden bağımsız, hemen
          EventBus<OnRocketActivated>.Emit(new OnRocketActivated(TilePosition, direction, duration));

          AnimateRocket();
      }

      private void SetupVisuals()
      {
          if (direction == RocketDirections.Vertical)
          {
              // Roketi dikey yapmak için Z ekseninde 90 derece döndür
              transform.localRotation = Quaternion.Euler(0, 0, 90);
          }
          else
          {
              // Yatay ise rotasyonu sıfırla (0,0,0)
              transform.localRotation = Quaternion.identity;
          }
      }

      private void AnimateRocket()
      {
          Camera mainCam = Camera.main;
          float screenHeight = 2f * mainCam.orthographicSize;
          float screenWidth = screenHeight * mainCam.aspect;

          float dynamicDistance = (direction == RocketDirections.Horizontal) ? screenWidth : screenHeight;

          if (direction == RocketDirections.Horizontal)
          {
              visualPartB.transform.DOLocalMoveX(-dynamicDistance, duration);
              visualPartA.transform.DOLocalMoveX(dynamicDistance, duration).OnComplete(OnAnimationComplete);
          }
          else
          {
              visualPartB.transform.DOLocalMoveX(-dynamicDistance, duration);
              visualPartA.transform.DOLocalMoveX(dynamicDistance, duration).OnComplete(OnAnimationComplete);
          }
      }

      private void OnAnimationComplete()
      {
          visualPartA.transform.localPosition = visualPartAStartPosition;
          visualPartB.transform.localPosition = visualPartBStartPosition;
          
          tilePool = PoolManager.Instance.GetPool(tileData.tileId);
          tilePool.ReturnToPool(this);
      }
  }


public enum RocketDirections 
{ 
    Horizontal, 
    Vertical 
}