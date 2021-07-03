using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Slime", menuName = "Character/Slime", order = 51)]
public class SlimeData : CharacterData
{
    [SerializeField] private Slime_Type _slimeType;
    [SerializeField] private int _maxCloneCreate;
    [SerializeField] private GameObject _slimePrefab;

    public Slime_Type Slime_Type => _slimeType;
    public int MaxCloneCreate => _maxCloneCreate;
    
}
