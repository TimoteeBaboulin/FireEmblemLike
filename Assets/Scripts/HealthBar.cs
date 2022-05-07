using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    public void UpdateHealth(Character character)
    {
        Vector3 scale = transform.localScale;
        scale.x = (float) character.health / character.MaxHealth;
        scale.x *= 0.1f;
        transform.localScale = scale;
    }
}
