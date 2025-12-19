using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

class Deck {
	
	private readonly List<(string, string)> cards;
	private int nextIndex = 0;
	
	public Deck(IEnumerable<(string, string)> cards) {
		this.cards = cards.OrderBy(_ => GlobalState.Instance.Rand.Next()).ToList();
	}
	
	public List<(string, string)> Draw(int count) {
		if (nextIndex == cards.Count) {
			return new List<(string, string)>();
		}
		
		count = Math.Min(count, cards.Count - nextIndex);
		
		var hand = cards.GetRange(nextIndex, count);
		nextIndex += count;
		return hand;
	}
	
	public int Remaining => cards.Count - nextIndex;
}

public partial class GameManager : Node2D {
	private static readonly Func<List<(string, string)>> DeckSupplier = () =>
		new List<(string, string)>(
			GlobalState.Instance.RankOrder.SelectMany(rank => GlobalState.Instance.SuitOrder.Select(suit => (rank, suit)))
		);
	
	private HBoxContainer table;
	private TextureButton button;
	
	private Deck deck;
	private Hand playerHand;
	private Hand enemyHand;
	
	private List<Card> top;
	
	private int round = 0;
	
	public async override void _Ready() {
		GlobalState.Instance.allowCardSelect = false;
		
		table = GetNode<HBoxContainer>("Table");
		button = GetNode<TextureButton>("Button");
		
		deck = new Deck(DeckSupplier());
		playerHand = new Hand();
		enemyHand = new Hand();
		
		AddChild(playerHand);
		AddChild(enemyHand);
		
		var playerHandInfo = deck.Draw(13);
		var enemyHandInfo = deck.Draw(13);
		
		playerHand.Init(playerHandInfo, 0, 0.75f, true);
		enemyHand.Init(enemyHandInfo, -200, 0.3f, false);
		
		playerHand.Connect(Hand.SignalName.ActiveCardCheck, new Callable(this, nameof(ActiveCardCheck)));
		
		GlobalState.Instance.allowCardSelect = true;
	}
	
	private void UpdateButton(bool enable) {
		button.Disabled = !enable;
		button.Modulate = enable ? Colors.White : new Color(1, 1, 1, 0.4f);
	}
	
	private void ActiveCardCheck() {
		int tableCount = table.GetChildCount();
		int handCount = playerHand.activeCards.Count();
		
		if (tableCount == 0) {
			UpdateButton(handCount != 0);
		} else {
			List<Card> tableCards = table.GetChildren().OfType<Card>().ToList();
			
			//UpdateButton(CanBeat(playerHand.activeCards, tableCards));
		}
	}
	
	public enum ComboType {
		Invalid = 0,
		Single,
		Pair,
		Triple,
		FourKind,
		Sequence,
		DoubleSequence
	}

	record Combo(ComboType Type, int HighRank, int Length);

	//Dictionary<string,int> RankToIndex = GlobalState.Instance.RankOrder
		//.Select((r, i) => (r, i)).ToDictionary(x => x.r, x => x.i);

	//int RankIndex(string rank) => RankToIndex[rank];
//
	//Combo Classify(List<Card> cards) {
		//int n = cards.Count;
		//if (n == 0) {
			//return new Combo(ComboType.Invalid, -1, 0);
		//}
		//
		//var ranks = cards.Select(c => RankIndex(c.rank)).ToList();
		//var rankCounts = ranks.GroupBy(r => r).ToDictionary(g => g.Key, g => g.Count());
		//bool allSameRank = rankCounts.Count == 1;
		//
		//if (n == 1) return new Combo(ComboType.Single, ranks[0], 1);
		//if (n == 2 && allSameRank) return new Combo(ComboType.Pair, ranks[0], 2);
		//if (n == 3 && allSameRank) return new Combo(ComboType.Triple, ranks[0], 3);
		//if (n == 4 && allSameRank) return new Combo(ComboType.FourKind, ranks[0], 4);
		//
		//bool containsTwo = ranks.Any(r => r == RankToIndex["2"]);
		//var distinctRanks = rankCounts.Keys.OrderBy(r => r).ToList();
		//
		//bool isSequence = !containsTwo && n >= 3 && rankCounts.Values.All(c => c == 1) &&
			//AreConsecutive(distinctRanks);
		//
		//if (isSequence) {
			//int high = distinctRanks[^1];
			//return new Combo(ComboType.Sequence, high, distinctRanks.Count);
		//}
		//
		//bool isDoubleSeq = !containsTwo && n >= 6 && n % 2 == 0 &&
			//rankCounts.Values.All(c => c == 2) && AreConsecutive(distinctRanks) &&
			//distinctRanks.Count >= 3;
		//
		//if (isDoubleSeq) {
			//int high = distinctRanks[^1];
			//return new Combo(ComboType.DoubleSequence, high, distinctRanks.Count);
		//}
		//
		//return new Combo(ComboType.Invalid, -1, 0);
	//}
	//
	//bool AreConsecutive(List<int> sortedRanks) {
		//for (int i = 1; i < sortedRanks.Count; i++) {
			//if (sortedRanks[i] != sortedRanks[i - 1] + 1) {
				//return false;
			//}
		//}
		//return true;
	//}
	//
	//bool CanBeat(List<Card> player, List<Card> table) {
		//var a = Classify(player);
		//var b = Classify(table);
		//
		//if (a.Type == ComboType.Invalid || b.Type == ComboType.Invalid) {
			//return false;
		//}
		//if (a.Type != b.Type) {
			//return false;
		//}
		//
		//if ((a.Type == ComboType.Sequence || a.Type == ComboType.DoubleSequence) && a.Length != b.Length) {
			//return false;
		//}
		//
		//return a.HighRank > b.HighRank;
	//}
}
