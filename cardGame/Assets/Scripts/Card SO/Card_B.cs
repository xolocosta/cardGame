using UnityEngine;

[CreateAssetMenu(fileName = "Card_B", menuName = "Scriptable Objects/Card_B")]
public class Card_B : Card_Abstract, IEnergy, IFriendly, IBlock
{
    public bool SpendEnergy(int energyValue)
        => false;
}
