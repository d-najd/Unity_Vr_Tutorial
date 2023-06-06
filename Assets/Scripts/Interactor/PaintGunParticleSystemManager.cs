using UnityEngine;

public class PaintGunParticleSystemManager : MonoBehaviour
{
    [SerializeField] private GameObject particleSystemGameObject;
    [SerializeField] private new ParticleSystem particleSystem;

    public void ChangeParticleSystemColor(Color color)
    {
        var main = particleSystem.main;
        main.startColor = color;
    }

    public void StartParticleSystem()
    {
        particleSystemGameObject.SetActive(true);
    }

    public void StopParticleSystem()
    {
        particleSystemGameObject.SetActive(false);
    }

    public void ChangeParticleSystemActiveState()
    {
        particleSystemGameObject.SetActive(particleSystemGameObject.activeSelf);
    }
}
