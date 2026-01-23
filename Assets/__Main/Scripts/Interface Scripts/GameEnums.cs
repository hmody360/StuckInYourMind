using UnityEngine;

public static class GameEnums
{
    public enum CharacterType { Omar, Mohammed };
    public enum PlayerMovementState { Disabled, Movement, Dashing, Bashing };

    public enum PlayerOffenseState { Disabled, Neutral, Attacking, Shooting };

    public enum CollectibleType { NormalCollectible, SecretCollectible, HealthPoint, LifePoint};


}
