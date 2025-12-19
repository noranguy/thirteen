import random
from collections import Counter

RANKS = ["3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A", "2"]
SUITS = ["c", "d", "h"]
RANK_VALUE = {r: i for i, r in enumerate(RANKS)}
SUIT_VALUE = {s: i for i, s in enumerate(SUITS)}

class Card:
    __slots__ = ("rank", "suit")
    def __init__(self, rank: str, suit: str):
        self.rank = rank
        self.suit = suit

    def __str__(self) -> str:
        return f"Card(rank={self.rank}, suit={self.suit})"

    def _cmp_key(self) -> tuple[int]:
        return (RANK_VALUE(self.rank), SUIT_VALUE(self.suit))
    
    def __eq__(self, other: object) -> bool:
        if not isinstance(other, Card):
            return NotImplemented
        
        return self.rank == other.rank and self.suit == other.suit
    
    def __lt__(self, other: object) -> bool:
        if not isinstance(other, Card):
            return NotImplemented
        
        return self._cmp_key() < other._cmp_key()
    
    def __hash__(self) -> int:
        return hash((self.rank, self.suit))

class Agent:
    def __init__(self):
        self.deck = []

    def move(self, top: list[Card], round: int) -> str:
        return ""

class Human(Agent):
    def __init__(self):
        self.deck = [[Card(r, s) for r in RANKS] for s in SUITS]

    def _parse_cards(s: str) -> list[Card]:
        s = s.strip()
        tokens = s.replace(',', ' ').split()
        out = []

        for t in tokens:
            suit = t[-1].lower()
            rank = t[:-1].upper()
            out.append(Card(rank, suit))

        return out

    def move(self, top: list[Card], round: int) -> str:
        s = input("Move: ")
        cards = self._parse_cards(s)
        self.deck[round] = [x for x in self.deck[round] if x not in cards]
        return cards
    
class Agent0(Agent):
    def __init__(self):
        self.deck = [[Card(r, s) for r in RANKS] for s in SUITS]
    
    def move(self, top: list[Card], round: int) -> str:
        n = len(self.deck[round])

        order = random.sample(range(n), n)

        out = []
        for i in order:
            curr = self.deck[round][order[i]]
            if all(curr > c for c in top):
                out.append(curr)
            else: continue

            if len(out) >= len(top): break
            
        return out
    
class Game:
    __slots__ = ("round", "score", "agent1", "agent2")
    def __init__(self, agent1: Agent, agent2: Agent):
        self.round_num = 0
        self.score = [0, 0]
        self.agents = [agent1, agent2]

    def round(self):
        top = []
        first, second = self.round_num % 2, (self.round_num + 1) % 2
    
        # while True:
        #     cards = self.agents[first].move(top, self.round_num)

        #     if not cards:
        #         self.score[second
        # while agent1_playing or agent2_playing:
        #     if agent1_playing:
        #         cards = self.agent1.move(top, self.round_num)
                
        #         if not cards:
        #             agent1_playing = False
        #             self.score += 1
        #         else:
        #             top = cards
            
        #     if agent2_playing:
        #         cards = self.agent2.move(top, self.round_num)
                
        #         if not cards:
        #             agent2_playing = False
    
        #             if agent1_playing:
        #                 self.score -= 1
        #         else:
        #             top = cards
        
        # self.round_num += 1