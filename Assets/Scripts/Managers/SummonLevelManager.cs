using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SummonLevelManager : MonoBehaviour
{
    public class LevelAndSpell
    {
        public GameObject level;
        public string spell;

        public LevelAndSpell(GameObject level, string spell)
        {
            this.level = level;
            this.spell = spell;
        }
    }

    public GameObject officeLevel;
    public GameObject[] summonLevelCorrectPrefabs;
    public GameObject[] summonLevelIncorrectPrefabs;
    public float spellMistakeProbability;
    public string[] correctSpells;
    public string[] incorrectSpells;


    public bool IsSpellCorrect()
    {
        if (UnityEngine.Random.Range(0.0f, 1.0f) < spellMistakeProbability) {
            return false;
        }
        return true;
    }

    public GameObject GenerateSummonLevel(bool correct)
    {
        GameObject prefab;
        if (correct) {
            prefab = summonLevelCorrectPrefabs[UnityEngine.Random.Range(0, summonLevelCorrectPrefabs.Length - 1)];
        } else {
            prefab = summonLevelIncorrectPrefabs[UnityEngine.Random.Range(0, summonLevelIncorrectPrefabs.Length - 1)];
        }
        var summonLevel = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity);
        summonLevel.SetActive(false);
        return summonLevel;
    }

    public string GenerateSpell(bool correct)
    {
        if (correct) {
            return correctSpells[UnityEngine.Random.Range(0, correctSpells.Length - 1)];
        } else {
            return incorrectSpells[UnityEngine.Random.Range(0, incorrectSpells.Length - 1)];
        }
    }

    public LevelAndSpell GenerateLevelAndSpell(bool correct)
    {
        bool isSpellCorrect;
        bool isLevelCorrect;
        if (correct) {
            isSpellCorrect = true;
            isLevelCorrect = true;
        } else {
            isSpellCorrect = IsSpellCorrect();
            isLevelCorrect = !isSpellCorrect;
        }

        return new LevelAndSpell(GenerateSummonLevel(isLevelCorrect), GenerateSpell(isSpellCorrect));
    }
}
