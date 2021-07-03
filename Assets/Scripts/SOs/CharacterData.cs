using UnityEngine;

public class CharacterData : ScriptableObject
{
    [SerializeField] private Char_Type charType;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _hitDamage;
    [SerializeField] private string _details;

    public int MaxHealth => _maxHealth;
    public int HitDamage => _hitDamage;
    public string Details => _details;

}
