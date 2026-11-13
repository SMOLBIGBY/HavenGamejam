private Coroutine rechargeCoroutine;

void StartRecharge()
{
    rechargeCoroutine = StartCoroutine(LaserRechargeDelayCoroutine(laserManager.LaserRechargeDelay));
}

void StopRecharge()
{
    if (rechargeCoroutine != null)
    {
        StopCoroutine(rechargeCoroutine);
        rechargeCoroutine = null;
    }
}