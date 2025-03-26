using Modules.Infrastructure.States;
using UnityEngine;
namespace Modules.Infrastructure.States
{
    public class TouchState : EnvironmentInteractionState
{
    private EEnvironmentInteractionState _stateKey;
    public TouchState(EnvironmentInteractionContext environmentInteractionContext, EEnvironmentInteractionState stateKey) : base(environmentInteractionContext, stateKey)
    {
       //_stateKey = stateKey;
    }
}
}
