using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UpGame
{

	// 状态基类
	public class FiniteState
	{
		public string Name
		{
			get
			{
				return GetType().Name;
			}
		}

		public virtual void OnEnter(object context){}
		public virtual void OnExit(object context){}
		public virtual void OnFixUpdate(object context){}

		private FiniteStateMachine _fsm;
		public FiniteStateMachine Owner
		{
			get {
				return _fsm;
			}
			set {
				_fsm = value;
			}
		}
	}
	
	// 状态迁移基类
	public class FiniteStateTransition
	{
		public string EventName;
		public string LastStateName;
		public string NextStateName;

		public FiniteStateTransition(string eventName, string lastStateName, string nextStateName)
		{
			EventName = eventName;
			LastStateName = lastStateName;
			NextStateName = nextStateName;
		}
	}

	// 有限状态机基类
	public class FiniteStateMachine 
	{
		class InitState : FiniteState
		{
		}

		private static FiniteState _initState = new InitState();
		protected object _context; // 上下文内容
		public string CurrentStateName = "EmptyState";

		private Dictionary<string, FiniteState> _allStates = new Dictionary<string, FiniteState>(); // 状态集合
		private List<FiniteStateTransition> _allTransitions = new List<FiniteStateTransition>(); // 状态迁移列表

		protected FiniteState _defaultState = _initState;
		private FiniteState _currentState;
		protected FiniteState CurrentState
		{
			get
			{
				return _currentState;
			}
			set
			{
				_currentState = value;
				CurrentStateName = _currentState.Name;
			}
		}

		private FiniteState _nextState = null;
		protected FiniteState NextState
		{
			get {
				return _nextState;
			}
			set {
				_nextState = value;
			}
		}

		protected virtual void Init(){}

		public void SetContext(object context)
		{
			_context = context;
		}

		public void SetDefaultToCurrent()
		{
//			CurrentState = defaultState;
			ChangeState(_defaultState);
		}

		public FiniteStateMachine(object context = null)
		{
			SetContext (context);
			CurrentState = _initState;
			Init();
			SetDefaultToCurrent();
		}
			

		protected void AddState(FiniteState state, bool bDefault = false)
		{
			_allStates[state.Name] = state;
			state.Owner = this;
			if (bDefault) {
				_defaultState = state;
			}
		}

		protected void RemoveState(FiniteState state)
		{
			if (_allStates.ContainsKey(state.Name))
			{
				_allStates.Remove(state.Name);
			}
		}

		protected void AddTransition<T1, T2>(string eventName)
		{
			string lastStateName = typeof(T1).Name;
			string nextStateName = typeof(T2).Name;
			FiniteStateTransition transition = new FiniteStateTransition(eventName, lastStateName, nextStateName);
			_allTransitions.Add(transition);
		}

		protected void AddTransition(FiniteStateTransition transition)
		{
			_allTransitions.Add(transition);
		}

		protected void RemoveTranstion(FiniteStateTransition transition)
		{
			if (_allTransitions.Contains(transition))
		    {
				_allTransitions.Remove(transition);
			}
		}

		private bool FindTransitionWithEventName(string eventName, out FiniteStateTransition transition)
		{
			transition = null;
			if (CurrentState == null) return false;

			foreach(FiniteStateTransition trans in _allTransitions)
			{
				if (0 == eventName.CompareTo(trans.EventName) && 0 == CurrentState.Name.CompareTo(trans.LastStateName))
				{
					transition = trans;
					return true;
				}
			}
			return false;
		}

		// 根据事件名称，从状态迁移表中，取出下一个状态
		private bool TryGetNextStateByEventName(string eventName, FiniteStateTransition transition, out FiniteState state)
		{
			state = null;
			if (!eventName.Equals(transition.EventName))
				return false;
			if (CurrentState == null || !CurrentState.Name.Equals(transition.LastStateName)) 
				return false;

			return (_allStates.TryGetValue(transition.NextStateName, out state));
		}

		// 状态迁移
		private void ChangeState(FiniteState newState)
		{
			if (newState == CurrentState) {
				return;
			}

			if (CurrentState != null)
			{
				CurrentState.OnExit(_context);
			}
			CurrentState = newState;
			if (CurrentState != null)
			{
				CurrentState.OnEnter(_context);
			}
		}

		// 发送事件，触发状态迁移
		public void SendEvent(string eventName)
		{
			Debug.Assert (NextState == null, "Next State already exist!");
			foreach (FiniteStateTransition transition in _allTransitions)
			{
				FiniteState nextState;
				if (TryGetNextStateByEventName(eventName, transition, out nextState))
			    {
//					ChangeState(nextState);
					NextState = nextState;
					break;
				}
			}
		}

		public void OnFixUpdate()
		{
			if (NextState != null) {
				ChangeState (NextState);
				NextState = null;
			}
			if (CurrentState != null)
			{
				CurrentState.OnFixUpdate(_context);
			}
		}
	}

}