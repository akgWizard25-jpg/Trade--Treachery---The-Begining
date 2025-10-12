using System;
using System.Collections;
using System.Collections.Generic;
using jy_util;
using UnityEngine;

public class _GameAssets : MonoSingleton<_GameAssets>
{
    [Header("Player")]
    public static readonly string PlayerTag = "Player";
    public Transform playerShipTranform;
    

    [Space]
    [Header("Damage Pop ups")]
    public Transform pfDamagePopup;
    public Color ciriticalHitColor;
    public Color normalHitColor;
}
