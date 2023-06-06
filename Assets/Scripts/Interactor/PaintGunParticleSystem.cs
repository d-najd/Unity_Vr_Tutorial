using UnityEngine;

public class PaintGunParticleSystem : MonoBehaviour
{
    [SerializeField] private GameObject particleSystemGameObject;
    [SerializeField] private ParticleSystem particleSystem;

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
