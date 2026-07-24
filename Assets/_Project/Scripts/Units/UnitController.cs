using UnityEngine;
using TWR.Battle;
using TWR.Data;

namespace TWR.Units
{
    public abstract class UnitController : MonoBehaviour
    {
        public enum UnitState { Idle, Move, Attack, Dead }

        public UnitState      State     { get; protected set; } = UnitState.Idle;
        public WarriorFaction Faction   { get; protected set; }
        public bool           IsPlayer  { get; protected set; }
        public bool           IsAlive   => State != UnitState.Dead;
        public float          CurrentATK => _currentATK;

        protected float _currentHP;
        protected float _currentATK;
        protected float _currentRange;
        protected float _currentAttackSpeed;
        protected float _moveSpeed = 2f;
        protected float _attackTimer;

        public virtual void InitializeStats(
            float hp, float atk, float range, float attackSpeed,
            WarriorFaction faction, bool isPlayer, float moveSpeed = 2f)
        {
            _currentHP          = hp;
            _currentATK         = atk;
            _currentRange       = range;
            _currentAttackSpeed = attackSpeed;
            _moveSpeed          = moveSpeed;
            Faction             = faction;
            IsPlayer            = isPlayer;
            State               = UnitState.Idle;
            _attackTimer        = 0f;
        }

        protected virtual void Update()
        {
            if (State == UnitState.Dead) return;

            var target = FindTarget();

            if (target == null || !target.IsAlive)
            {
                State = UnitState.Move;
                MoveForward();
                return;
            }

            float dist = Vector3.Distance(transform.position, target.transform.position);
            if (dist <= _currentRange)
            {
                State = UnitState.Attack;
                _attackTimer -= Time.deltaTime;
                if (_attackTimer <= 0f)
                {
                    _attackTimer = 1f / _currentAttackSpeed;
                    PerformAttack(target);
                }
            }
            else
            {
                State = UnitState.Move;
                MoveToward(target.transform.position);
            }
        }

        protected abstract UnitController FindTarget();

        protected virtual void MoveForward()
        {
            float dir = IsPlayer ? 1f : -1f;
            transform.Translate(0f, dir * _moveSpeed * Time.deltaTime, 0f);
        }

        protected virtual void MoveToward(Vector3 position)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, position, _moveSpeed * Time.deltaTime);
        }

        protected virtual void PerformAttack(UnitController target)
        {
            float multiplier = CounterSystem.GetDamageMultiplier(Faction, target.Faction);
            target.TakeDamage(_currentATK * multiplier);
        }

        public virtual void TakeDamage(float damage)
        {
            if (!IsAlive) return;
            _currentHP -= damage;
            if (_currentHP <= 0f) Die();
        }

        protected virtual void Die()
        {
            State = UnitState.Dead;
            OnDeath();
        }

        protected abstract void OnDeath();

        public void ReturnToPool()
        {
            gameObject.SetActive(false);
        }
    }
}