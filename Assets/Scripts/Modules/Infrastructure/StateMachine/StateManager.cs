using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Modules.Infrastructure.States;
using UnityEngine;

public abstract class StateManager<TState> : MonoBehaviour where TState : Enum
{
    protected Dictionary<TState, BaseState<TState>> States = new Dictionary<TState, BaseState<TState>>();
    protected TState currentState;
    void Start() {}
    void Update() {}
    void OnTriggerEnter(Collider other){}
    void OnTriggerExit(Collider other){}
    void OnTriggerStay(Collider other){}
}