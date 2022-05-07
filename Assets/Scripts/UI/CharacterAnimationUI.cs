using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationUI : MonoBehaviour
{
    [SerializeField] private Animator Animator;
    
    public void SetCharacterAnimation(Character character)
    {
        string path = "Animation/UI/";

        switch (character.team)
        {
            case Character.Team.Player:
                path += "Player";
                break;
            
            case Character.Team.Enemy:
                path += "Enemy";
                break;
        }

        switch (character._Weapon.type)
        {
            case Weapon.WeaponType.Axe:
                path += "Axe";
                break;
            
            case Weapon.WeaponType.Spear:
                path += "Spear";
                break;
            
            case Weapon.WeaponType.Sword:
                path += "Sword";
                break;
        }

        path += "IdleUI";
        
        AnimatorOverrideController animation = Resources.Load<AnimatorOverrideController>(path);

        Animator.runtimeAnimatorController = animation;
    }
}
