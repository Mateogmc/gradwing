using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostProcessingManager : MonoBehaviour
{
    public static PostProcessingManager instance;

    [SerializeField] Volume volume;
    LensDistortion ld;
    ChromaticAberration ca;
    MotionBlur mb;
    Vignette v;

    [SerializeField] float lensDistortionDivider;
    [SerializeField] float lensDistortionDuration;
    [SerializeField] float lensDistortionScale;
    float lensDistortionIntensity;
    float ldt;

    [SerializeField] float chromaticAberrationDivider;
    [SerializeField] float chromaticAberrationDuration;
    float chromaticAberrationIntensity;
    float cat;

    [SerializeField] float motionBlurIntensity;
    [SerializeField] float motionBlurDuration;
    float mbt;

    [SerializeField] float vignetteIntensity;
    [SerializeField] float vignetteDuration;
    bool vFlag = false;
    float vt;

    void Start()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        volume.profile.TryGet(out ld);
        volume.profile.TryGet(out ca);
        volume.profile.TryGet(out mb);
        volume.profile.TryGet(out v);
    }

    private void Update()
    {
        if (!DataManager.GetInstance().visualEffects)
        {
            ld.active = false;
            ca.active = false;
            mb.active = false;
            v.active = false;
        }
        else
        {
            ld.active = true;
            ca.active = true;
            mb.active = true;
            v.active = true;
        }
        LensDistortionManager();
        ChromaticAberrationManager();
        MotionBlurManager();
        VignetteManager();
    }

    private void LensDistortionManager()
    {
        if  (Time.time < ldt)
        {
            ld.intensity.value = Mathf.Lerp(0, lensDistortionIntensity, (ldt - Time.time) / lensDistortionDuration);
            ld.scale.value = Mathf.Lerp(1, lensDistortionScale, (ldt - Time.time) / lensDistortionDuration);
        }
    }

    private void ChromaticAberrationManager()
    {
        if (Time.time < cat)
        {
            ca.intensity.value = Mathf.Lerp(0, chromaticAberrationIntensity, (cat - Time.time) / chromaticAberrationDuration);
        }
    }

    private void MotionBlurManager()
    {
        if (Time.time < mbt)
        {
            mb.intensity.value = Mathf.Lerp(0, motionBlurIntensity, (mbt - Time.time) / motionBlurDuration);
        }
    }

    private void VignetteManager()
    {
        if (Time.time < vt)
        {
            if (vFlag)
            {
                v.intensity.value = Mathf.Lerp(vignetteIntensity, 0, (vt - Time.time) / vignetteDuration);
            }
            else
            {
                v.intensity.value = Mathf.Lerp(0, vignetteIntensity, (vt - Time.time) / vignetteDuration);
            }
        }
    }

    public void ClearPostProcess()
    {
        ld.intensity.value = 0;
        ld.scale.value = 1;
        ca.intensity.value = 0;
        mb.intensity.value = 0;
        v.intensity.value = 0;
    }

    public void Boost(float velocity)
    {
        lensDistortionIntensity = ld.intensity.value < velocity / lensDistortionDivider ? ld.intensity.value : velocity / lensDistortionDivider;
        ld.intensity.value = lensDistortionIntensity;   
        ld.scale.value = lensDistortionScale;
        ldt = Time.time + lensDistortionDuration;
    }

    public void Hit(float health)
    {
        chromaticAberrationIntensity = health == 0 ? 1 : Mathf.Lerp(0.6f, 0.2f, health);
        ca.intensity.value = chromaticAberrationIntensity;
        cat = Time.time + ((1 - health + 0.2f) * chromaticAberrationDuration);

        mb.intensity.value = motionBlurIntensity;
        mbt = Time.time + ((1 - health + 0.2f) * motionBlurDuration);
    }

    public void Gravel()
    {
        mb.intensity.value = mb.intensity.value > motionBlurIntensity / 2 ? mb.intensity.value : motionBlurIntensity / 2;
        mbt = Time.time + motionBlurDuration / 2;
    }

    public void Roll(bool start)
    {
        vFlag = start;
        if (start)
        {
            v.intensity.value = 0;
        }
        else
        {
            v.intensity.value = vignetteIntensity;
        }

        vt = Time.time + vignetteDuration;
    }
}
