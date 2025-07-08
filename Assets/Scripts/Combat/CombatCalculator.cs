using UnityEngine;

public static class CombatCalculator
{
    public static int CalculateDamage(Unit attacker, Unit defender)
    {
        if (attacker == null || defender == null) return 0;
        
        // Get base damage from attack table
        int baseDamage = attacker.unitData.GetAttackDamage(defender.unitData.unitType);
        
        // If unit can't attack this target, return 0
        if (baseDamage <= 0) return 0;
        
        // Apply health multiplier (damaged units deal less damage)
        float healthMultiplier = attacker.GetHealthPercentage();
        float adjustedDamage = baseDamage * healthMultiplier;
        
        // Apply terrain defense bonus
        float defenseMultiplier = 1f;
        if (defender.currentTile != null)
        {
            float defenseBonus = defender.currentTile.GetDefenseBonus(defender) / 100f;
            defenseMultiplier = 1f - defenseBonus;
        }
        
        adjustedDamage *= defenseMultiplier;
        
        // Apply random variance (±10%)
        float variance = Random.Range(0.9f, 1.1f);
        adjustedDamage *= variance;
        
        // Round to nearest integer, minimum 1 damage
        int finalDamage = Mathf.Max(1, Mathf.RoundToInt(adjustedDamage));
        
        Debug.Log($"Combat: {attacker.unitData.unitName} vs {defender.unitData.unitName} - " +
                 $"Base: {baseDamage}, HP: {healthMultiplier:F2}, Defense: {defenseMultiplier:F2}, " +
                 $"Variance: {variance:F2}, Final: {finalDamage}");
        
        return finalDamage;
    }
    
    public static CombatResult CalculateCombatResult(Unit attacker, Unit defender)
    {
        CombatResult result = new CombatResult();
        
        // Calculate attacker damage to defender
        result.attackerDamage = CalculateDamage(attacker, defender);
        
        // Check if defender can counter-attack
        bool canCounterAttack = CanCounterAttack(attacker, defender);
        
        if (canCounterAttack)
        {
            // Calculate defender damage to attacker (counter-attack)
            result.defenderDamage = CalculateDamage(defender, attacker);
        }
        
        // Calculate resulting HP
        result.attackerFinalHP = Mathf.Max(0, attacker.currentHP - result.defenderDamage);
        result.defenderFinalHP = Mathf.Max(0, defender.currentHP - result.attackerDamage);
        
        // Determine winner
        if (result.attackerFinalHP <= 0 && result.defenderFinalHP <= 0)
        {
            result.winner = CombatWinner.Draw;
        }
        else if (result.attackerFinalHP <= 0)
        {
            result.winner = CombatWinner.Defender;
        }
        else if (result.defenderFinalHP <= 0)
        {
            result.winner = CombatWinner.Attacker;
        }
        else
        {
            result.winner = CombatWinner.None;
        }
        
        return result;
    }
    
    public static bool CanCounterAttack(Unit attacker, Unit defender)
    {
        if (defender == null || attacker == null) return false;
        
        // Defender must be able to attack the attacker
        if (defender.unitData.GetAttackDamage(attacker.unitData.unitType) <= 0)
            return false;
        
        // Defender must have ammo
        if (defender.currentAmmo <= 0)
            return false;
        
        // Check range - defender must be able to reach attacker
        Vector2Int defenderRange = defender.unitData.GetAttackRange();
        int distance = GridManager.Instance.GetDistance(defender.currentTile, attacker.currentTile);
        
        return distance >= defenderRange.x && distance <= defenderRange.y;
    }
    
    public static bool IsInAttackRange(Unit attacker, Unit target)
    {
        if (attacker == null || target == null) return false;
        
        Vector2Int attackRange = attacker.unitData.GetAttackRange();
        int distance = GridManager.Instance.GetDistance(attacker.currentTile, target.currentTile);
        
        return distance >= attackRange.x && distance <= attackRange.y;
    }
    
    public static float CalculateHitChance(Unit attacker, Unit defender)
    {
        // Basic hit chance calculation
        // In Advance Wars, most attacks have 100% hit chance, but we can add weather, CO powers, etc. later
        
        float baseHitChance = 1f; // 100%
        
        // TODO: Add factors like:
        // - Weather conditions
        // - CO powers
        // - Special abilities
        // - Terrain effects
        
        return Mathf.Clamp01(baseHitChance);
    }
    
    public static int CalculateExperience(Unit unit, Unit target, int damageDealt)
    {
        // Basic experience calculation
        int baseExp = damageDealt / 10; // 1 exp per 10 damage
        
        // Bonus for destroying enemy
        if (target.currentHP <= damageDealt)
        {
            baseExp += 10;
        }
        
        // TODO: Add factors like:
        // - Unit rank differences
        // - Special achievements
        // - CO powers
        
        return Mathf.Max(1, baseExp);
    }
}

[System.Serializable]
public class CombatResult
{
    public int attackerDamage;
    public int defenderDamage;
    public int attackerFinalHP;
    public int defenderFinalHP;
    public CombatWinner winner;
    public bool isCounterAttack;
}

public enum CombatWinner
{
    None,
    Attacker,
    Defender,
    Draw
} 