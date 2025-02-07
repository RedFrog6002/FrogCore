using System;
using UnityEngine;

namespace FrogCore.Unity;

public class UnitySpriteFlash : MonoBehaviour
{
    private SpriteRenderer rend;

    private Color flashColour;

    private float amount;

    private float timeUp;

    private float stayTime;

    private float timeDown;

    private int flashingState;

    private float flashTimer;

    private float amountCurrent;

    private float t;

    private bool repeatFlash;

    private bool cancelFlash;

    private float geoTimer;

    private bool geoFlash;

    private bool sendToChildren = true;

    private void Start()
    {
        if (!rend)
            rend = GetComponent<SpriteRenderer>();
    }

    private void OnDisable()
    {
        if (!rend)
            rend = GetComponent<SpriteRenderer>();
        rend.color = Color.white;
        flashTimer = 0f;
        flashingState = 0;
        repeatFlash = false;
        cancelFlash = false;
        geoFlash = false;
    }

    private void Update()
    {
        if (cancelFlash)
        {
            rend.color = Color.white;
            flashingState = 0;
            cancelFlash = false;
        }
        if (flashingState == 1)
        {
            if (flashTimer < timeUp)
            {
                flashTimer += Time.deltaTime;
                t = flashTimer / timeUp;
                amountCurrent = Mathf.Lerp(0f, amount, t);
                rend.color = Color.Lerp(Color.white, flashColour, amountCurrent);
            }
            else
            {
                rend.color = Color.Lerp(Color.white, flashColour, amount);
                flashTimer = 0f;
                flashingState = 2;
            }
        }
        if (flashingState == 2)
        {
            if (flashTimer < stayTime)
            {
                flashTimer += Time.deltaTime;
            }
            else
            {
                flashTimer = 0f;
                flashingState = 3;
            }
        }
        if (flashingState == 3)
        {
            if (flashTimer < timeDown)
            {
                flashTimer += Time.deltaTime;
                t = flashTimer / timeDown;
                amountCurrent = Mathf.Lerp(amount, 0f, t);
                rend.color = Color.Lerp(Color.white, flashColour, amountCurrent);
            }
            else
            {
                rend.color = Color.white;
                flashTimer = 0f;
                if (repeatFlash)
                    flashingState = 1;
                else
                    flashingState = 0;
            }
        }
        if (geoFlash)
        {
            if (geoTimer > 0f)
            {
                geoTimer -= Time.deltaTime;
                return;
            }
            FlashingSuperDash();
            geoFlash = false;
        }
    }

    public void GeoFlash()
    {
        geoFlash = true;
        geoTimer = 0.25f;
    }

    public void flash(Color flashColour_var, float amount_var, float timeUp_var, float stayTime_var, float timeDown_var)
    {
        flashColour = flashColour_var;
        amount = amount_var;
        timeUp = timeUp_var;
        stayTime = stayTime_var;
        timeDown = timeDown_var;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
    }

    public void CancelFlash()
    {
        cancelFlash = true;
    }

    public void FlashingSuperDash()
    {
        flashColour = new Color(1f, 1f, 1f);
        amount = 0.7f;
        timeUp = 0.1f;
        stayTime = 0.01f;
        timeDown = 0.1f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = true;
        SendToChildren(FlashingSuperDash);
    }

    public void FlashingGhostWounded()
    {
        flashColour = new Color(1f, 1f, 1f);
        amount = 0.7f;
        timeUp = 0.5f;
        stayTime = 0.01f;
        timeDown = 0.5f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = true;
        SendToChildren(FlashingGhostWounded);
    }

    public void FlashingWhiteStay()
    {
        flashColour = new Color(1f, 1f, 1f);
        amount = 0.6f;
        timeUp = 0.01f;
        stayTime = 999f;
        timeDown = 0.01f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = true;
        SendToChildren(FlashingWhiteStay);
    }

    public void FlashingWhiteStayMoth()
    {
        flashColour = new Color(1f, 1f, 1f);
        amount = 0.6f;
        timeUp = 2f;
        stayTime = 9999f;
        timeDown = 2f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = true;
        SendToChildren(FlashingWhiteStayMoth);
    }

    public void FlashingFury()
    {
        Start();
        flashColour = new Color(0.71f, 0.18f, 0.18f);
        amount = 0.75f;
        timeUp = 0.25f;
        stayTime = 0.01f;
        timeDown = 0.25f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = true;
        SendToChildren(FlashingFury);
    }

    public void FlashingOrange()
    {
        flashColour = new Color(1f, 0.31f, 0f);
        amount = 0.7f;
        timeUp = 0.1f;
        stayTime = 0.01f;
        timeDown = 0.1f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = true;
        SendToChildren(FlashingOrange);
    }

    public void flashInfected()
    {
        flashColour = new Color(1f, 0.31f, 0f);
        amount = 0.9f;
        timeUp = 0.01f;
        stayTime = 0.01f;
        timeDown = 0.25f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashInfected);
    }

    public void flashDung()
    {
        flashColour = new Color(0.45f, 0.27f, 0f);
        amount = 0.9f;
        timeUp = 0.01f;
        stayTime = 0.01f;
        timeDown = 0.25f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashDung);
    }

    public void flashDungQuick()
    {
        flashColour = new Color(0.45f, 0.27f, 0f);
        amount = 0.75f;
        timeUp = 0.001f;
        stayTime = 0.05f;
        timeDown = 0.1f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashDungQuick);
    }

    public void flashSporeQuick()
    {
        flashColour = new Color(0.95f, 0.9f, 0.15f);
        amount = 0.75f;
        timeUp = 0.001f;
        stayTime = 0.05f;
        timeDown = 0.1f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashSporeQuick);
    }

    public void flashWhiteQuick()
    {
        flashColour = new Color(1f, 1f, 1f);
        amount = 1f;
        timeUp = 0.001f;
        stayTime = 0.05f;
        timeDown = 0.001f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashWhiteQuick);
    }

    public void flashInfectedLong()
    {
        flashColour = new Color(1f, 0.31f, 0f);
        amount = 0.9f;
        timeUp = 0.01f;
        stayTime = 0.25f;
        timeDown = 0.35f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashInfectedLong);
    }

    public void flashArmoured()
    {
        flashColour = new Color(1f, 1f, 1f);
        amount = 0.9f;
        timeUp = 0.01f;
        stayTime = 0.01f;
        timeDown = 0.25f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashArmoured);
    }

    public void flashBenchRest()
    {
        flashColour = new Color(1f, 1f, 1f);
        amount = 0.7f;
        timeUp = 0.01f;
        stayTime = 0.1f;
        timeDown = 0.75f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashBenchRest);
    }

    public void flashDreamImpact()
    {
        flashColour = new Color(1f, 1f, 1f);
        amount = 0.9f;
        timeUp = 0.01f;
        stayTime = 0.25f;
        timeDown = 0.75f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashDreamImpact);
    }

    public void flashMothDepart()
    {
        flashColour = new Color(1f, 1f, 1f);
        amount = 0.75f;
        timeUp = 1.9f;
        stayTime = 1f;
        timeDown = 0.25f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashMothDepart);
    }

    public void flashSoulGet()
    {
        flashColour = new Color(1f, 1f, 1f);
        amount = 0.5f;
        timeUp = 0.01f;
        stayTime = 0.01f;
        timeDown = 0.25f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashSoulGet);
    }

    public void flashShadeGet()
    {
        flashColour = new Color(0f, 0f, 0f);
        amount = 0.5f;
        timeUp = 0.01f;
        stayTime = 0.01f;
        timeDown = 0.25f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashShadeGet);
    }

    public void flashWhiteLong()
    {
        flashColour = new Color(1f, 1f, 1f);
        amount = 1f;
        timeUp = 0.01f;
        stayTime = 0.01f;
        timeDown = 2f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashWhiteLong);
    }

    public void flashOvercharmed()
    {
        flashColour = new Color(0.72f, 0.376f, 0.72f);
        amount = 0.75f;
        timeUp = 0.2f;
        stayTime = 0.01f;
        timeDown = 0.5f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashOvercharmed);
    }

    public void flashFocusHeal()
    {
        Start();
        flashColour = new Color(1f, 1f, 1f);
        amount = 0.85f;
        timeUp = 0.01f;
        stayTime = 0.01f;
        timeDown = 0.35f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashFocusHeal);
    }

    public void flashFocusGet()
    {
        Start();
        flashColour = new Color(1f, 1f, 1f);
        amount = 0.5f;
        timeUp = 0.01f;
        stayTime = 0.01f;
        timeDown = 0.35f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashFocusGet);
    }

    public void flashWhitePulse()
    {
        Start();
        flashColour = new Color(1f, 1f, 1f);
        amount = 0.35f;
        timeUp = 0.5f;
        stayTime = 0.01f;
        timeDown = 0.75f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashWhitePulse);
    }

    public void flashHealBlue()
    {
        flashColour = new Color(0f, 0.584f, 1f);
        amount = 0.75f;
        timeUp = 0.01f;
        stayTime = 0.01f;
        timeDown = 0.5f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(flashHealBlue);
    }

    public void FlashShadowRecharge()
    {
        Start();
        flashColour = new Color(0f, 0f, 0f);
        amount = 0.75f;
        timeUp = 0.01f;
        stayTime = 0.01f;
        timeDown = 0.35f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(FlashShadowRecharge);
    }

    public void flashInfectedLoop()
    {
        flashColour = new Color(1f, 0.31f, 0f);
        amount = 0.9f;
        timeUp = 0.2f;
        stayTime = 0.01f;
        timeDown = 0.2f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = true;
        SendToChildren(flashInfectedLoop);
    }

    public void FlashGrimmflame()
    {
        Start();
        flashColour = new Color(1f, 0.25f, 0.25f);
        amount = 0.75f;
        timeUp = 0.01f;
        stayTime = 0.01f;
        timeDown = 1f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(FlashGrimmflame);
    }

    public void FlashGrimmHit()
    {
        Start();
        flashColour = new Color(1f, 0.25f, 0.25f);
        amount = 0.75f;
        timeUp = 0.01f;
        stayTime = 0.01f;
        timeDown = 0.25f;
        flashingState = 1;
        flashTimer = 0f;
        repeatFlash = false;
        SendToChildren(FlashGrimmHit);
    }

    private void SendToChildren(Action function)
    {
        if (!sendToChildren)
        {
            return;
        }
        UnitySpriteFlash[] componentsInChildren = GetComponentsInChildren<UnitySpriteFlash>();
        foreach (UnitySpriteFlash spriteFlash in componentsInChildren)
        {
            if (!(spriteFlash == this))
            {
                spriteFlash.sendToChildren = false;
                spriteFlash.GetType().GetMethod(function.Method.Name).Invoke(spriteFlash, null);
            }
        }
    }
}
