using UnityEngine;

[CreateAssetMenu(fileName = "Card_H", menuName = "Scriptable Objects/Card_H")]
public class Card_H : Card_Abstract, IEnergy, IFriendly, IHeal
{
    public bool SpendEnergy(int energyValue)
        => false;
}
