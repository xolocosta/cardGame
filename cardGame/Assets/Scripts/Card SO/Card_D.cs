using UnityEngine;

[CreateAssetMenu(fileName = "Card_D", menuName = "Scriptable Objects/Card_D")]
public class Card_D : Card_Abstract, IEnergy, IFriendly, IDamage
{
    public bool SpendEnergy(int energyValue)
        => false;
}
