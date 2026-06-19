using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coachgun : Shotgun
{
    [Header("Coachgun")]
    public GameObject secondaryShootSfx;

    public Transform muzzleFlashPosA;
    public Transform muzzleFlashPosB;

    public float boostAmount;
    public override void Start()
    {
        base.Start();
        muzzleFlashPos = muzzleFlashPosA;
    }
    public override void OnMainAction()
    {
        if (currentClipAmount > 0) {
            if (currentReloadTime < (reloadTime - reloadDisableShootTime)){
                muzzleFlashPos = muzzleFlashPos == muzzleFlashPosA ? muzzleFlashPosB : muzzleFlashPosA;
                Instantiate(muzzleFlash, muzzleFlashPos.position, muzzleFlashPos.rotation, muzzleFlashPos);
                GameObject muzzle = muzzleFlash;
                muzzleFlash = null;
                base.OnMainAction();
                muzzleFlash = muzzle;
            }
        }
        else { NoAmmoShoot(); }
    }

    public override void OnSecondaryAction()
    {
        base.OnSecondaryAction();

        if (currentClipAmount > 0) {
            if (currentReloadTime < (reloadTime - reloadDisableShootTime))
            {
                Instantiate(shootSfx, transform.position, Quaternion.identity);
                Cache.moveData.Velocity += (Cache.vcam.transform.forward * -boostAmount) * Mathf.InverseLerp(0, 12, currentClipAmount);
                GameObject sfx = shootSfx;

                shootSfx = secondaryShootSfx;
                float spread = spreadAmount;

                spreadAmount *= 1.25f; //Values shouldn't be hardcoded : temp

                moveRecoilA *= 1.33f;
                moveRecoilB *= 1.33f;

                moveRecoilSmoothness *= 0.8f;
                moveRecoilSpeed *= 1.2f;

                shotgunSpreadMultiplier = 1.25f;
                OnMainAction(); OnMainAction();
                shotgunSpreadMultiplier = 1f;

                moveRecoilSpeed /= 1.2f;
                moveRecoilSmoothness /= 0.8f;

                spreadAmount = spread;

                shootSfx = sfx;

                moveRecoilA /= 1.33f;
                moveRecoilB /= 1.33f;
            }
        }
        else { NoAmmoShoot(); }
    }
}
