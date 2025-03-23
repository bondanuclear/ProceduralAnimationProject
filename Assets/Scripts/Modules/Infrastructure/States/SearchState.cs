using Modules.Infrastructure.States;
using UnityEngine;
namespace Modules.Infrastructure.States
{
    public class SearchState : EnvironmentInteractionState
{
    private EEnvironmentInteractionState _stateKey;
    public SearchState(EnvironmentInteractionContext environmentInteractionContext, EEnvironmentInteractionState stateKey) : base(environmentInteractionContext, stateKey)
    {
       _stateKey = stateKey;
    }
}
}
