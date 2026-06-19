using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firearm : Item
{
    [Header("Stats")]
    public float damage;
    public float range;

    public float impactForce;

    float timeAfterLastShot;

    [Header("Spread")]
    public float spreadAmount;
    public float movementSpreadMultiplier = 1f;

    public float startShootSpread;
    public float shootSpreadIncrease;
    public float shootSpreadIncreaseDecaySpeed;
    float currentShootSpread;

    float spreadNoisePointer;

    public static float globalCurrentSpread { get; private set; }
    public static float globalCurrentInaccuracy { get; private set; }

    public float spreadCrosshairSizeMultiplier;
    public static float globalCurrentSpreadCrosshairSizeMultiplier { get; private set; }

    //public LayerMask layerMask;

    [Header("Reloading")]
    public int ammoClipSize;
    [HideInInspector] public int currentClipAmount;
    public PlaySound reloadSfx;

    public float reloadTime;
    public float reloadDisableShootTime;
    bool reloadRequiredComplete;
    [HideInInspector] public float currentReloadTime;

    public static bool globalCurrentlyReloading;

    [Header("Graphics")]

    public Transform muzzleFlashPos;
    public GameObject muzzleFlash;

    public GameObject shootSfx;

    public static Transform curentMuzzleFlashPos;

    [Header("Sound Effects")]
    public PlaySound noAmmoClickSfx;

    [Header("Model Recoil")]

    public LocalOffsetManager localOffsetManager;
    public Vector3 moveRecoilA;
    public Vector3 moveRecoilB;
    Vector3 targetMoveRecoil;
    Vector3 currentMoveRecoil;
    public float moveRecoilSpeed;
    public float moveRecoilSmoothness;

    public LocalRotationOffsetManager localRotationOffsetManager;
    public Vector3 rotRecoilA;
    public Vector3 rotRecoilB;
    Vector3 targetRotRecoil;
    Vector3 currentRotRecoil;
    public float rotRecoilSpeed;
    public float rotRecoilSmoothness;

    [Header("Run Bob")]

    public Vector2 runBobMultiplier = Vector2.one;
    //public static ObjectRunBob handheldRunBobManager;

    [Header("Camera Recoil")]

    public float wCamRecoilA;
    public float wCamRecoilB;
    float currentWCamRecoil;
    float targetWCamRecoil;
    public float wCamRecoilSpeed;
    public float wCamRecoilSmoothness;

    public Vector3 camRecoilA;
    public Vector3 camRecoilB;
    Vector3 currentCamRecoil;
    [HideInInspector] public Vector3 targetCamRecoil;
    public static Vector3 globalCurrentCamRecoilAmount;
    public float camRecoilSpeed;
    public float camRecoilSmoothness;

    int shotsThisFrame;

    [Header("Agro")]
    public float shootDetectionIncrease;

    [Header("Crosshair")]
    public float crosshairDipAmount;
    public float crosshairDipMultiplier;

    [Header("Fixed Recoil")]
    public Vector3 fixedRecoilA;
    public Vector3 fixedRecoilB;
    public Vector3[] fixedRecoils;
    public int fixedRecoilSeed;
    public int fixedRecoilPointer;
    public float fixedRecoilPointerDecayTime;
    float currentFixedRecoilPointerDecayTime;
    public float fixedRecoilPointerDecaysUntilMultiplier;
    int currentFixedRecoilPointerDecaysUntilMultiplier;
    public float fixedRecoilPointerSpeedupMultiplier;

    Vector3 targetCamRecoilVelocity;
    Vector3 currentCamRecoilVelocity;

    [HideInInspector] public float shotgunSpreadMultiplier = 1f;

    [Header("Recoil Inaccuracy Spread")]
    public float movementRecoilInaccuracy;
    public float airborneRecoilInaccuracy;
    public override void Start()
    {
        globalCurrentlyReloading = false;
        base.Start();
        currentShootSpread = startShootSpread;
        //currentClipAmount = ammoClipSize;
        ///if (handheldRunBobManager == null) { handheldRunBobManager = GameObject.Find("HandheldRunBobManager").GetComponent<ObjectRunBob>(); }
        //if (!handheldRunBobManager.amountMultipliers.ContainsKey(transform.name + "ItemBob")) { handheldRunBobManager.amountMultipliers.Add(transform.name + "ItemBob", Vector2.zero); }
        localOffsetManager.AddValue(transform.name + "MoveRecoil", Vector3.zero);
        localRotationOffsetManager.AddValue(transform.name + "RotRecoil", Vector3.zero);
        Cache.cameraRotationOffsetManager.AddOffset(transform.name + "Recoil", new CameraRotationOffset{offset = Vector4.zero});
        reloadRequiredComplete = true;
        GenerateFixedRecoil();
    }
    public void GenerateFixedRecoil()
    {
        System.Random rand = new System.Random(fixedRecoilSeed);
        fixedRecoils = new Vector3[ammoClipSize];
        for (int i = 0; i < fixedRecoils.Length; i++){
            fixedRecoils[i] = new Vector3(Utilities.NextFloat(rand, fixedRecoilA.x, fixedRecoilB.x), Utilities.NextFloat(rand, fixedRecoilA.y, fixedRecoilB.y), Utilities.NextFloat(rand, fixedRecoilA.z, fixedRecoilB.z));
        }
    }
    public override void OnMainAction()
    {
        base.OnMainAction();

        if (currentClipAmount > 0){
            if (currentReloadTime < (reloadTime - reloadDisableShootTime)){
                Shoot();
            }
        }
        else{
            NoAmmoShoot();
        }
    }
    public void NoAmmoShoot()
    {
        if (InputManager.GetMainActionKeyDown()){
            Cache.ammo.NoAmmoInClipIndication();
            noAmmoClickSfx.Play();
        }
    }
    public void Shoot()
    {
        if (muzzleFlash != null) { Instantiate(muzzleFlash, muzzleFlashPos.position, muzzleFlashPos.rotation, muzzleFlashPos); }
        targetMoveRecoil = Utilities.RandomRangeVector3(moveRecoilA, moveRecoilB);
        targetRotRecoil = Utilities.RandomRangeVector3(rotRecoilA, rotRecoilB);
        targetWCamRecoil = Random.RandomRange(wCamRecoilA, wCamRecoilB) * Utilities.CoinFlipn();
        //targetCamRecoil = Utilities.RandomRangeVector3(camRecoilA, camRecoilB);
        targetCamRecoil += fixedRecoils[fixedRecoilPointer] * (Cache.crouch.crouching ? 0.66f : 1f);
        if (fixedRecoilPointer < fixedRecoils.Length-1){
            fixedRecoilPointer++;
        }
        currentFixedRecoilPointerDecaysUntilMultiplier = 0;
        currentFixedRecoilPointerDecayTime = fixedRecoilPointerDecayTime * 2f;

        bool shotgun = this is Shotgun;

        if (shotgun)
        {
            if (shotsThisFrame == 0)
            {
                Instantiate(shootSfx, transform.position, Quaternion.identity);
            }
        }
        else { Instantiate(shootSfx, transform.position, Quaternion.identity); }

        shotsThisFrame++;

        //Vector3 Spread = GetSpread();
        Vector3 Spread = GetRecoilInaccuracy() + (shotgun ? fixedRecoils[fixedRecoilPointer] * (Cache.crouch.crouching ? 0.66f : 1f) * shotgunSpreadMultiplier : Vector3.zero);
        //currentShootSpread += shootSpreadIncrease;
        //currentShootSpread = Mathf.Clamp(currentShootSpread, 0f, 1f);
        //spreadNoisePointer += 1f / ammoClipSize;
        //print(Spread);

        FirearmAgroManager.Shot(shootDetectionIncrease);

        RaycastHit hit;
        if (Physics.Raycast(Cache.vcam.transform.position, Cache.vcam.transform.forward + Spread, out hit, range, Cache.references.bulletLayerMask))
        {
            ImpactManager.Hit(this, hit);
        }

        currentClipAmount--;
        Cache.ammo.currentClipCounterGraphicManager.SetCounter(currentClipAmount);
        timeAfterLastShot = 0f;
        //Cache.ammo.ChangeAmmo(-1);
    }
    /*
    public void Shoot()
    {
        if (muzzleFlash != null) { Instantiate(muzzleFlash, muzzleFlashPos.position, muzzleFlashPos.rotation, muzzleFlashPos); }
        targetMoveRecoil = Utilities.RandomRangeVector3(moveRecoilA,moveRecoilB);
        targetRotRecoil = Utilities.RandomRangeVector3(rotRecoilA,rotRecoilB);
        targetWCamRecoil = Random.RandomRange(wCamRecoilA, wCamRecoilB) * Utilities.CoinFlipn();
        targetCamRecoil = Utilities.RandomRangeVector3(camRecoilA, camRecoilB);

        if (this is Shotgun){
            if (shotsThisFrame == 0){
                Instantiate(shootSfx, transform.position, Quaternion.identity);
            }
        }
        else { Instantiate(shootSfx, transform.position, Quaternion.identity); }

        shotsThisFrame++;

        Vector3 Spread = GetSpread();
        currentShootSpread += shootSpreadIncrease;
        currentShootSpread = Mathf.Clamp(currentShootSpread, 0f, 1f);
        spreadNoisePointer += 1f/ammoClipSize;
        //print(Spread);

        FirearmAgroManager.Shot(shootDetectionIncrease);

        RaycastHit hit;
        if (Physics.Raycast(Cache.vcam.transform.position, Cache.mainCam.transform.forward+ Spread, out hit, range, Cache.references.bulletLayerMask)){
            ImpactManager.Hit(this, hit);
        }

        currentClipAmount--;
        Cache.ammo.currentClipCounterGraphicManager.SetCounter(currentClipAmount);
        timeAfterLastShot = 0f;
        //Cache.ammo.ChangeAmmo(-1);
    }
    */
    /*
    int seed = 12345;  // A fixed seed for this example, replace with something dynamic if needed

    Vector3 GetSpread()
    {
        // Initialize the random state with the fixed seed
        //Random.InitState(seed);

        // You can also use a Perlin noise function for more natural spread behavior
        // Perlin noise takes a 1D or 2D value, but we can use different coordinates for X, Y, Z.

        // Example for generating 3D spread values (X, Y, Z components of spread)
        float noiseX = Mathf.PerlinNoise(seed * 0.1f, 0f) * 2f - 1f;  // X spread
        float noiseY = Mathf.PerlinNoise(seed * 0.2f, 0f) * 2f - 1f;  // Y spread
        float noiseZ = Mathf.PerlinNoise(seed * 0.3f, 0f) * 2f - 1f;  // Z spread

        // Scale and apply any multipliers based on movement, crouch, etc.
        float movementMultiplier = 1 + ((FragMovementListener.hSpeedPercentage / 1.5f) * movementSpreadMultiplier);
        float midAirMultiplier = FragMovementListener.grounded ? 1f : 2f;
        float crouchMultiplier = Cache.crouch.crouching ? 0.75f : 1f;

        // Calculate the final spread with multipliers applied
        Vector3 spread = new Vector3(noiseX, noiseY, noiseZ) * (spreadAmount * movementMultiplier) * midAirMultiplier * crouchMultiplier * currentShootSpread;

        return spread;
    }
    */
    /*
    Vector3 GetSpread() // Used to calculate randomness in recoil.
    {
        float movementMultiplier = 1+((FragMovementListener.hSpeedPercentage/1.5f)*movementSpreadMultiplier);
        float midAirMultiplier = FragMovementListener.grounded ? 1f : 2f;
        float crouchMultiplier = Cache.crouch.crouching  ? 0.75f : 1f;
        //globalCurrentSpread = ((spreadAmount * movementMultiplier) * midAirMultiplier) * crouchMultiplier * currentShootSpread;
        //print((Mathf.PerlinNoise1D(spreadNoisePointer)).ToString() +"     "+ spreadNoisePointer.ToString());
        //return (((spreadAmount * movementMultiplier) * midAirMultiplier) * crouchMultiplier * currentShootSpread) * new Vector3((Mathf.PerlinNoise1D(spreadNoisePointer) * 20) - 0.5f, (Mathf.PerlinNoise1D(-spreadNoisePointer) * 20) - 0.5f, (Mathf.PerlinNoise1D(spreadNoisePointer * 20) * 0.5f) - 1);
        return Utilities.RandomRangeV3All(-(((spreadAmount * movementMultiplier) * midAirMultiplier) * crouchMultiplier * currentShootSpread), ((spreadAmount * movementMultiplier) * midAirMultiplier) * crouchMultiplier* currentShootSpread);
    }
    */
    Vector3 GetSpread() // Deprecated - Used for old spread system
    {
        float movementMultiplier = FragMovementListener.hSpeedPercentage * movementSpreadMultiplier;
        float midAirMultiplier = FragMovementListener.grounded ? 0f : 2f;
        //float crouchMultiplier = Cache.crouch.crouching ? 0.75f : 1f;
        //globalCurrentSpread = ((spreadAmount * movementMultiplier) * midAirMultiplier) * crouchMultiplier * currentShootSpread;
        //print((Mathf.PerlinNoise1D(spreadNoisePointer)).ToString() +"     "+ spreadNoisePointer.ToString());
        //return (((spreadAmount * movementMultiplier) * midAirMultiplier) * crouchMultiplier * currentShootSpread) * new Vector3((Mathf.PerlinNoise1D(spreadNoisePointer) * 20) - 0.5f, (Mathf.PerlinNoise1D(-spreadNoisePointer) * 20) - 0.5f, (Mathf.PerlinNoise1D(spreadNoisePointer * 20) * 0.5f) - 1);
        return Utilities.RandomRangeV3All(-(movementMultiplier + midAirMultiplier), movementMultiplier + midAirMultiplier);
    }
    Vector3 GetRecoilInaccuracy() // Used to calculate randomness in recoil
    {
        float inaccuracy = GetInaccuracy();
        return Utilities.RandomRangeV3All(-(inaccuracy), inaccuracy);
    }
    float GetInaccuracy()
    {
        float movementMultiplier = (FragMovementListener.hSpeedPercentage * movementRecoilInaccuracy) * (Cache.stabilize.stabilized?0.5f:1f);
        float midAirMultiplier = FragMovementListener.grounded || Mathf.Round(Cache.moveData.Velocity.y) == 0f ? 0f : 1f * airborneRecoilInaccuracy;
        return movementMultiplier + midAirMultiplier;
    }
    public virtual void OnReloadComplete()
    {
        int newAmmo = 0;
        while (currentClipAmount < ammoClipSize && Cache.ammo.amount > 0)
        {
            currentClipAmount++;
            Cache.ammo.amount--;
            newAmmo++;
            if (newAmmo > 999) { break; }
        }
        Cache.ammo.currentClipCounterGraphicManager.SetCounter(currentClipAmount);
        Cache.ammo.counterGraphicManager.SetCounter(Cache.ammo.amount);
        if (Cache.ammo.amount<=0) { Cache.ammo.NoAmmoInClipIndication(); }
        //globalCurrentlyReloading = false;
        reloadRequiredComplete = true;
    }
    public virtual void OnReloadStart()
    {
        currentReloadTime = reloadTime;
        if (reloadSfx != null) { reloadSfx.Play(); }
        globalCurrentlyReloading = true;
        reloadRequiredComplete = false;
    }
    public virtual void OnReloadCancel()
    {
        currentReloadTime = 0;
        globalCurrentlyReloading = false;
    }
    public override void OnEquip()
    {
        base.OnEquip();
        //Cache.firearmSpreadCrosshairIndicator.currentItemMultiplier = crosshairSpreadIndicatorMultiplier;
        curentMuzzleFlashPos = muzzleFlashPos;
        //handheldRunBobManager.amountMultipliers[transform.name + "ItemBob"] = runBobMultiplier;
        Cache.itemRunBobManager.baseAmountMultiplier = runBobMultiplier;
        Cache.firearmSpreadCrosshairIndicator.currentFirearmDipAmount = crosshairDipAmount;
        Cache.firearmSpreadCrosshairIndicator.currentFirearmDipMultiplier = crosshairDipMultiplier;
        StartCoroutine(OnEquipDelay());
    }
    IEnumerator OnEquipDelay()
    {
        yield return new WaitForSeconds(0.1f);
        ///handheldRunBobManager.amountMultipliers[transform.name + "ItemBob"] = runBobMultiplier;
        Cache.ammo.currentClipCounterGraphicManager.SetCounter(currentClipAmount);
    }
    public override void OnUnequip()
    {
        base.OnUnequip();
        //Cache.firearmSpreadCrosshairIndicator.currentItemMultiplier = 1f;
        Cache.ammo.currentClipCounterGraphicManager.HideCounter();
        OnReloadCancel();
        //handheldRunBobManager.amountMultipliers[transform.name + "ItemBob"] = Vector2.zero;
        Cache.itemRunBobManager.baseAmountMultiplier = Vector2.one;
        Cache.firearmSpreadCrosshairIndicator.currentFirearmDipMultiplier = crosshairDipMultiplier;
        Cache.firearmSpreadCrosshairIndicator.currentFirearmDipAmount = crosshairDipAmount;
        globalCurrentSpread = 0f;
    }
    public override void Update()
    {
        base.Update();
        targetMoveRecoil = Vector3.Lerp(targetMoveRecoil, Vector3.zero, Time.deltaTime * moveRecoilSpeed);
        currentMoveRecoil = Vector3.Lerp(currentMoveRecoil, targetMoveRecoil, Time.deltaTime * moveRecoilSmoothness);
        localOffsetManager.UpdateValue("MoveRecoil", currentMoveRecoil);

        targetRotRecoil = Vector3.Lerp(targetRotRecoil, Vector3.zero, Time.deltaTime * rotRecoilSpeed);
        currentRotRecoil = Vector3.Lerp(currentRotRecoil, targetRotRecoil, Time.deltaTime * rotRecoilSmoothness);
        localRotationOffsetManager.UpdateValue("RotRecoil", currentRotRecoil);

        targetWCamRecoil = Mathf.Lerp(targetWCamRecoil, 0f, Time.deltaTime * wCamRecoilSpeed);
        currentWCamRecoil = Mathf.Lerp(currentWCamRecoil, targetWCamRecoil, Time.deltaTime * wCamRecoilSmoothness);

        targetCamRecoil = Vector3.SmoothDamp(targetCamRecoil, Vector3.zero,ref targetCamRecoilVelocity, camRecoilSpeed);
        currentCamRecoil = Vector3.SmoothDamp(currentCamRecoil,targetCamRecoil,ref currentCamRecoilVelocity,camRecoilSmoothness);

        //targetCamRecoil = Vector3.Lerp(targetCamRecoil, Vector3.zero, Time.deltaTime * camRecoilSpeed);
        //currentCamRecoil = Vector3.Lerp(currentCamRecoil, targetCamRecoil, Time.deltaTime * camRecoilSmoothness);

       // Cache.cameraRotationOffsetManager.UpdateOffset(transform.name + "Recoil", new CameraRotationOffset { offset = new Vector4(currentCamRecoil.x, currentCamRecoil.y, currentCamRecoil.z, currentWCamRecoil), forceBob = true });
        Cache.cameraRotationOffsetManager.UpdateOffset(transform.name + "Recoil", new CameraRotationOffset { offset = new Vector4(currentCamRecoil.x, currentCamRecoil.y, currentCamRecoil.z, currentWCamRecoil), forceBob = true });

        shotsThisFrame = 0;

        if (timeAfterLastShot > onMainActionCd + 0.02f){
            currentShootSpread -= shootSpreadIncreaseDecaySpeed * Time.deltaTime;
            currentShootSpread = Mathf.Clamp(currentShootSpread, startShootSpread, 1f);

            spreadNoisePointer -= shootSpreadIncreaseDecaySpeed * Time.deltaTime;
            spreadNoisePointer = Mathf.Clamp01(spreadNoisePointer);
        }

        if (equip) // THIS IS FOR OLD SPREAD
        {
            float movementMultiplier = 1 + ((FragMovementListener.hSpeedPercentage / 1.5f) * movementSpreadMultiplier);
            float midAirMultiplier = FragMovementListener.grounded ? 1f : 2f;
            float crouchMultiplier = Cache.crouch.crouching ? 0.75f : 1f;
            globalCurrentSpread = ((spreadAmount * movementMultiplier) * midAirMultiplier) * crouchMultiplier * currentShootSpread;

            globalCurrentCamRecoilAmount = currentCamRecoil;

            globalCurrentSpreadCrosshairSizeMultiplier = spreadCrosshairSizeMultiplier;
        }
        globalCurrentInaccuracy = GetInaccuracy();

        timeAfterLastShot += Time.deltaTime;

        if (InputManager.GetReloadKeyDown() && equip && !globalCurrentlyReloading)
        {
            if (currentClipAmount < ammoClipSize && Cache.ammo.amount > 0) { OnReloadStart(); }
            else { Cache.ammo.NoAmmoAvailableIndication(); }
            /*if (currentReloadTime > 0) { OnReloadCancel(); }
            else {
                if (currentClipAmount < ammoClipSize && Cache.ammo.amount > 0){ OnReloadStart(); }
                else { Cache.ammo.NoAmmoAvailableIndication(); }
            }*/
        }
        if (currentReloadTime > 0f)
        {
            currentReloadTime -= Time.deltaTime;
            if (currentReloadTime <= (reloadTime - reloadDisableShootTime) && !reloadRequiredComplete){
                OnReloadComplete();
            }
            if (currentReloadTime <= 0f && globalCurrentlyReloading){
                globalCurrentlyReloading = false;
            }
        }
        if (currentFixedRecoilPointerDecayTime > 0f && fixedRecoilPointer > 0)
        {
            currentFixedRecoilPointerDecayTime -= Time.deltaTime;
            if (currentFixedRecoilPointerDecayTime <= 0)
            {
                fixedRecoilPointer--;
                currentFixedRecoilPointerDecaysUntilMultiplier++;
                currentFixedRecoilPointerDecayTime = fixedRecoilPointerDecayTime * (currentFixedRecoilPointerDecaysUntilMultiplier >= fixedRecoilPointerDecaysUntilMultiplier ? fixedRecoilPointerSpeedupMultiplier : 1f);
            }
        }

        //if (Input.GetKeyDown(KeyCode.J)) { GenerateFixedRecoil(); }
    }
}
