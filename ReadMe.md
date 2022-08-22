#Ace Gambler

This is a multiplayer project made in Unity with help of Mirror. I was able to create a card game that works as follows:

2 Players draw 8 cards, these cards are not random and rather all add up to the same number (For example, [7+6+7 = 20] for P1  and [9+8+3 =20] for P2)
this is to make the round-start balanced between both players. The types of the cards are also always the same (2 of each type, 2 swords, 2 clubs, 2 diamonds, etc)

Afterwards both players pick a card from their hand to put on the field. And it goes to the battle phase.

On the battle phase, the card with the highest number wins, however, there are type advantages, Swords>Clover>Diamonds>Hearts, if a type is better than the other,
it gets granted +2 to the value, if the cards have both types: [Swords and Diamonds] and [Hearts and Clover] A special reveal effect occurs.

After combat, the reveal phase starts, the losing player can pick a card from the opponent's hand to reveal it on the field, if this card is revealed,
the opponent can still use it. If the special reveal effect happens, then both players pick a card from their opponent's

Then the game loops for 10 rounds, when the players reach 4 cards in their hand, they draw 4 more (using the logic at the beginning)

The intention of this game is to both learn how to develop a game that's been networked and also put into practice simple game design,
its intention being that of information control, since both player start with the same resources (numerically speaking), then the game becomes about
playing the right cards to get type advantages knowing what was played and what is on the field, make an educated guess if your opponent is more likely to
use a card that's been revealed or one that's yet to be played.
