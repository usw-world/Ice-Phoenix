using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyMonster : ChaseMonster {
    [SerializeField] GiveExperience experience;
    protected override void Start() {
        base.Start();
        chaseState.OnStay += () => {
            transform.Translate(new Vector2(targetDirection.x, 0) * Time.deltaTime * moveSpeed);
        };
        if(experience == null && TryGetComponent<GiveExperience>(out experience)) {
            Debug.LogWarning($"There is any 'GiveExperience' component in {gameObject.name}");
        }
    }
    protected override void Update() {
        base.Update();
        DetectTarget();
    }
    protected override void DetectTarget() {
        Collider2D inner = Physics2D.OverlapCircle((Vector2)transform.position + detectRange.center, detectRange.radius, Player.DEFAULT_PLAYER_LAYERMASK);
        if(inner != null && !isDead) {
            targetTransform = inner.transform;
            remainingDistance = Vector2.Distance(new Vector2(transform.position.x, 0), new Vector2(targetTransform.position.x, 0));
            monsterStateMachine.ChangeState(chaseState);
        } else {
            MissTarget();
        }
    }
    protected override void MissTarget() {
        targetTransform = null;
    }
    public override void OnDamage(float damage, Vector2 force, float duration = 0.25F) {
        monsterRigidbody.AddForce(force);
        base.OnDamage(damage, force, duration);
        if(hp <= 0)
            Die();
    }
    protected override void Die() {
        if(isDead) return;
        base.Die();
        monsterStateMachine.ChangeState(dieState);
        GetComponent<SpriteRenderer>().color = Color.white;
        transform.localScale = new Vector3(.5f, .5f, .5f);
        monsterSideUI.gameObject.SetActive(false);
        Destroy(this.gameObject, 1f);
    }
}
