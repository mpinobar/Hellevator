using UnityEngine;
using System.Collections;

public enum Figures
{
    WHOLE, MINIM, CROTCHET, TRIPLET, QUAVER, SEMIQUAVER, TRIPLET_SEMIQUAVER, TRIPLET_QUAVER, TRIPLET_CROCHET, TRIPLET_MINIM, TRIPLET_WHOLE, 
    DOTTED_SEMIQUAVER, DOTTED_QUAVER, DOTTED_CROTCHET, DOTTED_MINIM, DOTTED_WHOLE, SILENCE, NONE
} 

public enum  GameType
{
    PLAYER_VS_AI,
    PLAYER_VS_PLAYER
} 

public enum TypeOfPreassurePlate
{
	PaltformRaiser, PlatformLowerer, ButtonLike, Elevator, None
}
public enum EnemyState
{
	Chasing, Patrol, GoingBack, None
}
public enum BalanzaState
{
	Equal, RightIsHeavier, LeftIsHeavier
}
public enum FadeState
{
	FadingIn, FadingOut, None
}

