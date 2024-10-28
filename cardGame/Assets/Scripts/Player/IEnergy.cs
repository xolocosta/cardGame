public interface IEnergy 
{
    int EnergyValue { get; }
    bool SpendEnergy(int energyValue);
}
