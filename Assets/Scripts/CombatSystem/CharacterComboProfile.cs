using System.Collections.Generic;
using UnityEngine;

namespace Combat
{

    /// <summary>
    /// One ScriptableObject per playable character.
    /// Holds all combos available to that character + base combat stats.
    /// </summary>
    [CreateAssetMenu(fileName = "CharacterComboProfile", menuName = "Scriptable Objects/CharacterComboProfile")]
    public class CharacterComboProfile : ScriptableObject
    {
        [Header("Character info")]
        public string characterId = "Mechanic Girl";

        [Header("Combat Stats")]
        public float attackMultiplier = 1f;
        public float moveSpeedDuringCombo = 2f;

        [Header("Combo")]
        [Tooltip("All combos this character can perform. Order = priority for input matching")]
        public List<ComboData> availableCombos = new();

        public ComboData GetComboById(string id)
        {
            foreach (ComboData combo in availableCombos)
            {
                if (combo.comboName == id) return combo;
            }
            return null;
        }

    }
}

