using UnityEngine;

[CreateAssetMenu(fileName = "Card_D_all", menuName = "Scriptable Objects/Card_D_all")]
public class Card_D_all : Card_Abstract, IEnergy, IFriendly, IDamageAll
{
    public bool SpendEnergy(int energyValue)
        => false;
}
