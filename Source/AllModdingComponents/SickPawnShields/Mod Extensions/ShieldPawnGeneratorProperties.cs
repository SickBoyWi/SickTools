using System.Collections.Generic;
using Verse;

namespace SickPawnShields
{
    /// <summary>
    /// Contains extra information for the patched pawn generator to give a appropiate shield to the pawn.
    /// </summary>
    public class ShieldPawnGeneratorProperties : DefModExtension
    {
        /// <summary>
        /// How much "money" the pawn has for their shield.
        /// </summary>
        public FloatRange shieldMoney = FloatRange.Zero;

        /// <summary>
        /// The shields with any of these tags can be used.
        /// </summary>
        public List<string> shieldTags;

        /// <summary>
        /// The chance that a pawn will have a shield.
        /// </summary>
        public float shieldChance = 0.0f;

        public override IEnumerable<string> ConfigErrors()
        {
            if (shieldTags.NullOrEmpty())
                yield return nameof(shieldTags) + " cannot be null or empty";
        }
    }
}
