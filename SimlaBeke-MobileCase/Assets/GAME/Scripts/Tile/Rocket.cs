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
      [SerializeField] private float distance = 15f;
      [SerializeField] private float duration = 0.5f;

      public RocketDirections Direction => direction;

      public void Init(RocketDirections dir)
      {
          direction = dir;
          SetupVisuals();
      }

      public void OnClickedTileEvent()
      {
          // 1) Board temizliği – görselden bağımsız, hemen
          EventBus<OnRocketActivated>.Emit(new OnRocketActivated(TilePosition, direction));

          // 2) Roket animasyonu
          AnimateRocket();
      }

      private void SetupVisuals()
      {
          // Burada sprite’ları ve yönü ayarlıyorsun:
          // - Horizontal: A sol sprite, B sağ sprite, localPosition’lar x ekseninde yakın
          // - Vertical: parent veya çocukları 90° döndür, A yukarı, B aşağı baksın
      }

      private void AnimateRocket()
      {
          if (direction == RocketDirections.Horizontal)
          {
              visualPartA.transform.DOMoveX(
                  visualPartA.transform.position.x - distance, duration);
              visualPartB.transform.DOMoveX(
                  visualPartB.transform.position.x + distance, duration);
          }
          else
          {
              visualPartA.transform.DOMoveY(
                  visualPartA.transform.position.y + distance, duration);
              visualPartB.transform.DOMoveY(
                  visualPartB.transform.position.y - distance, duration);
          }

          DOVirtual.DelayedCall(duration, () =>
          {
              PoolManager.Instance
                  .GetPool(GetTileID())
                  .ReturnToPool(this);
          });
      }
  }

public enum RocketDirections 
{ 
    Horizontal, 
    Vertical 
}