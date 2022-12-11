using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pool;

public class FireRing : MonoBehaviour {
    public Ability_FireRing owner;
    [SerializeField] GameObject[] fireballs;
    [SerializeField] SoundPlayer soundPlayer;

    [SerializeField] AudioClip flameSound;
    [SerializeField] AudioClip explosionSound;
    [SerializeField] GameObject explosionParticle;

    [SerializeField] ParticlePool explostionPool;
    
    public void Start() {
        explostionPool = new ParticlePool("Explosion", explosionParticle, 10, 5, this.transform);
        soundPlayer = GetComponent<SoundPlayer>();
        for(int i=0; i<fireballs.Length; i++) {
            Fireball fireball = fireballs[i].GetComponent<Fireball>();
            if(fireball != null) {
                fireball.damage = owner.Damage;
                int n = i;
                fireball.explosionEvent = () => {
                    NoticeExplosion(n);
                    soundPlayer.StopSound();
                    soundPlayer.PlayClip(explosionSound);
                    GameObject particle = explostionPool.OutPool(fireball.transform.position);
                    StartCoroutine(Utility.TimeoutTask(() => {
                        explostionPool.InPool(particle);
                    }, 4f));
                };
            } else {
                Debug.LogWarning("Fireball is miss matched.");
            }
        }
    }
    public void NoticeExplosion(int index) {
        StartCoroutine(ActiveFireball(index));
    }
    private IEnumerator ActiveFireball(int index) {
        yield return new WaitForSeconds(owner.RespawnTime);
        try {
            fireballs[index].GetComponent<Fireball>().damage = owner.Damage;
            fireballs[index].SetActive(true);
            soundPlayer.PlayClip(flameSound);
        } catch(System.Exception e) {
            Debug.LogWarning(e);
        }
    }
}
