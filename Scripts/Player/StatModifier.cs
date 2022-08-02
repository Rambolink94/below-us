
namespace BelowUs.CharacterStats
{
    public class StatModifier
    {
        public readonly float value;
        public readonly StatModType statType;
        public readonly int order;
        public readonly object source;

        public StatModifier(float value, StatModType statType, int order, object source)
        {
            this.value = value;
            this.statType = statType;
            this.order = order;
            this.source = source;
        }

        public StatModifier(float value, StatModType statType) : this(value, statType, (int)statType, null) { }
        public StatModifier(float value, StatModType statType, int order) : this(value, statType, order, null) { }
        public StatModifier(float value, StatModType statType, object source) : this(value, statType, (int)statType, source) { }
    }

    public enum StatModType
    {
        Flat = 100,
        PercentAdd = 200,
        PercentMult = 300,
    }
}
