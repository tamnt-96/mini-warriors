using LayerLab.ArtMakerUnity;
using UnityEngine;

namespace TWR.Units
{
    [RequireComponent(typeof(UnitController))]
    public class UnitAnimator : MonoBehaviour
    {
        static class Anim
        {
            public const string Idle   = "Idle";
            public const string Walk   = "Walk";
            public const string Attack = "Attack";
            public const string Dead   = "Dead1";
        }

        UnitController          _unit;
        PartsManager            _parts;
        AnimationEventReceiver  _animEvents;
        UnitController.UnitState _lastState = (UnitController.UnitState)(-1);

        public event System.Action OnAttackHit;

        void Awake()
        {
            _unit       = GetComponent<UnitController>();
            _parts      = GetComponentInChildren<PartsManager>();
            _animEvents = GetComponentInChildren<AnimationEventReceiver>();

            if (_animEvents != null)
                _animEvents.OnAttackHitEvent += HandleAttackHit;
        }

        void OnEnable()
        {
            // Force state re-apply when the pooled object is re-activated.
            _lastState = (UnitController.UnitState)(-1);
        }

        void OnDestroy()
        {
            if (_animEvents != null)
                _animEvents.OnAttackHitEvent -= HandleAttackHit;
        }

        void Update()
        {
            if (_unit == null || _parts == null) return;

            var state = _unit.State;
            if (state == _lastState) return;
            _lastState = state;

            switch (state)
            {
                case UnitController.UnitState.Idle:
                    _parts.PlayAnimation(Anim.Idle);
                    break;
                case UnitController.UnitState.Move:
                    _parts.PlayAnimation(Anim.Walk);
                    break;
                case UnitController.UnitState.Attack:
                    _parts.PlayAnimation(Anim.Attack);
                    break;
                case UnitController.UnitState.Dead:
                    _parts.PlayAnimation(Anim.Dead);
                    break;
            }
        }

        void HandleAttackHit() => OnAttackHit?.Invoke();
    }
}
