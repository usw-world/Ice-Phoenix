using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pool;

public class GiveExperience : MonoBehaviour {
    [SerializeField] private int expAmount = 5;
    [SerializeField] private GameObject expParticleObject;
    const string EXP_PARTICLE_NAME = "Exp Particle";

    private void OnEnable() {
        if(expParticleObject == null)
            Debug.LogError("Game object might throw 'NullReferenceException'." + expParticleObject);
        
        if(ParticleManager.instance != null && !ParticleManager.instance.ContainParticle(EXP_PARTICLE_NAME)) {
            ParticleManager.instance.InitializeParticle(EXP_PARTICLE_NAME, expParticleObject, 10, 5);
        }
    }
    public void ReleaseExp() {
        GameObject particle = ParticleManager.instance.Call(EXP_PARTICLE_NAME, transform.position, null);
        var em = particle.GetComponent<ParticleSystem>().emission;
        int count = expAmount / 10;
        em.SetBursts(new ParticleSystem.Burst[]{ new ParticleSystem.Burst(0, count, 5, 0.05f) });
        
        ParticleManager.instance.Release(particle, 3);
        StartCoroutine(IncreaseExpCoroutine());
    }
    private IEnumerator IncreaseExpCoroutine() {
        yield return new WaitForSeconds(1);
        Player.playerInstance.IncreaseExp(expAmount);
    }
}