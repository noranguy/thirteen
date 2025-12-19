using Godot;
using System;
using System.Collections.Generic;

public abstract class Agent {
	public abstract void Init(List<Card> hand);
	
	public abstract List<Card> Move(List<Card> top);
}
