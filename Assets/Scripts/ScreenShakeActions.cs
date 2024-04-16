using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private void Start() 
    {
        ShootAction.OnAnyShoot += ShootAction_OnAnyShoot;
        Grenade.OnAnyGrenadeExplode += ExplosionAction_OnAnyGrenadeExplode;
        SwordAction.OnSwordHit += SwordAction_OnSwordHit;
    }

    private void SwordAction_OnSwordHit(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(1.5f);
    }

    private void ExplosionAction_OnAnyGrenadeExplode(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(2.5f);
    }

    private void ShootAction_OnAnyShoot(object sender, ShootAction.OnShootEventArgs  e)
    {
        ScreenShake.Instance.Shake(0.2f);
    }
}
