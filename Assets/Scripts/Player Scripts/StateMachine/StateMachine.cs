using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Rougelike2D
{
    public class StateMachine
    {
        StateNode currentState;
        Dictionary<Type, StateNode> nodes = new();
        HashSet<ITransition> anyTransitions = new();

        public void Update() 
        {
            ITransition transition = GetTransition();
            if(transition != null)
            {
                ChangeState(transition.TargetState);
            }
            currentState.State.StateUpdate();
        }
        public void FixedUpdate() 
        {
            currentState.State.StateUpdate();
        }

        public void SetState(IState state)
        {
            currentState = nodes[state.GetType()];
            currentState.State.EnterState();
        }
        public void ChangeState(IState state)
        {
            if(state == currentState.State) return;

            var previousState = currentState.State;
            var nextState = nodes[state.GetType()].State;

            previousState.ExitState();
            nextState.EnterState();

            currentState = nodes[state.GetType()];
        }
        ITransition GetTransition()
        {
            foreach(ITransition transition in anyTransitions)
            {
                if(transition.Condition.Evaluate()) 
                {
                    return transition;
                }
            }
            foreach(ITransition transition in currentState.Transitions)
            {
                if(transition.Condition.Evaluate()) 
                {
                    return transition;
                }
            }
            return null;
        }
        public void AddTransition(IState from, IState to, IPredicate condition) 
        {
            GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
        }
        
        public void AddAnyTransition(IState to, IPredicate condition) 
        {
            anyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));
        }

        StateNode GetOrAddNode(IState state)
        {
            var node = nodes.GetValueOrDefault(state.GetType());
            
            if (node == null) {
                node = new StateNode(state);
                nodes.Add(state.GetType(), node);
            }
            
            return node;
        }
        public class StateNode
        {
            public IState State;
            public HashSet<ITransition> Transitions;

            public StateNode(IState state)
            {
                this.State = state;
                Transitions = new HashSet<ITransition>();
            }

            public void AddTransition(IState targetState, IPredicate condition)
            {
                Transitions.Add(new Transition(targetState, condition));
            }
        }
    }
}
