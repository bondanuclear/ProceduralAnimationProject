using Modules.Infrastructure.States;
using UnityEngine;
namespace Modules.Infrastructure.States
{
    public class RiseState : EnvironmentInteractionState
{
    private EEnvironmentInteractionState _stateKey;
    public RiseState(EnvironmentInteractionContext environmentInteractionContext, EEnvironmentInteractionState stateKey) : base(environmentInteractionContext, stateKey)
    {
       _stateKey = stateKey;
    }
}
}
