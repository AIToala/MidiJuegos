using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShaderCode : MonoBehaviour
{
    Image image;
    Material m;
    CardVisual visual;
    void Start()
    {
        image = GetComponent<Image>();
        m = new Material(image.material);
        image.material = m;
        visual = GetComponent<CardVisual>();

        string[] editions = new string[4];
        editions[0] = "REGULAR";
        /* editions[1] = "GOLD";
        editions[2] = "DIAMOND";
        editions[3] = "PLATINUM"; */

        image.material.DisableKeyword(image.material.enabledKeywords[0]);
        image.material.EnableKeyword("_EDITION_" + editions[0]);
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion currentRotation = transform.parent.localRotation;
        Vector3 eulerAngles = currentRotation.eulerAngles;

        float xAngle = eulerAngles.x;
        float yAngle = eulerAngles.y;

        xAngle = ClampAngle(xAngle, -90f, 90f);
        yAngle = ClampAngle(yAngle, -90f, 90);

        m.SetVector("_Rotation_", new Vector2(ExtensionMethods.Remap(xAngle, -20, 20, -.5f, .5f), ExtensionMethods.Remap(yAngle, -20, 20, -.5f, .5f)));
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -180f)
            angle += 360f;
        if (angle > 180f)
            angle -= 360f;
        return Mathf.Clamp(angle, min, max);
    }
}
