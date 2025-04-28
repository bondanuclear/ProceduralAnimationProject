using UnityEngine;


namespace Modules.Infrastructure.States
{
    public abstract class MovementState : BaseState<EMovementState>
    {
        protected MovementContext _context;

        public MovementState(MovementContext context, EMovementState stateKey) : base(stateKey)
        {
            _context = context;
        }

        public override void OnTriggerEnter(Collider other)
        {
            // Optional: Override in derived classes if needed
        }

        public override void OnTriggerExit(Collider other)
        {
            // Optional: Override in derived classes if needed
        }

        public override void OnTriggerStay(Collider other)
        {
            // Optional: Override in derived classes if needed
        }
    }
} 