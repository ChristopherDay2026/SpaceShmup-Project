using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public class BlinkColorOnHit : MonoBehaviour {
    public static float blinkDuration = 0.1f;
    public static Color blinkColor = Color.red;

    [Header("Dynamic")]
    public bool showingColor = false;
    public float blinkCompleteTime;
    public bool ignoreOnClossionEnter = false;

    private Material[] materials;
    private Color[] originalColors;
    private BoundsCheck bndCheck;

    void Awake() {
        bndCheck = GetComponentInParent<BoundsCheck>();

        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for (int i = 0; i < materials.Length; i++) {
            originalColors[i] = materials[i].color;
        }
    }

    void Update() {
        if (showingColor && Time.time > blinkCompleteTime) {
            RevertColors();
        }
    }

    void OnCollisionEnter(Collision coll) {

        if (ignoreOnClossionEnter) {
            return;
        }

        ProjectileHero p = coll.gameObject.GetComponent<ProjectileHero>();
        if (p != null) {
            if (bndCheck != null && !bndCheck.isOnScreen) {
                return;
            }
            SetColors();
        }
    }

    public void SetColors() {
        foreach (Material mat in materials) {
            mat.color = blinkColor;
        }

        showingColor = true;
        blinkCompleteTime = Time.time + blinkDuration;
    }

    void RevertColors() {
        for (int i = 0; i < materials.Length; i++) {
            materials[i].color = originalColors[i];
        }

        showingColor = false;
    }
}
