using _Scripts.Quantum.Views;
using UnityEngine;
using UnityEngine.UI;

public class ActorModel : PoolableObject
{
    [Header("UI Link")] public Image image_HealthBar;
    public UI_ObjectHealthBar ObjectHealthBar;
    public RendererEffect RendererEffect;
    public Transform ShootingPosition;
    public Transform HitPosition;
    public Transform TopEffectPosition;
    public Animator Animator;

    private void Awake()
    {
        ObjectHealthBar = GetComponentInChildren<UI_ObjectHealthBar>();
        RendererEffect = new RendererEffect(gameObject);
    }

    public void Initialize(bool isAlly)
    {
        RendererEffect.Reset();
        ObjectHealthBar?.SetColor(isAlly);
    }
}
