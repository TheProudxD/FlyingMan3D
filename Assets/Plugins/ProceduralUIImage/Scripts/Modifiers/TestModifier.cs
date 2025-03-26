using UnityEngine;
using UnityEngine.UI.ProceduralImage;


[ModifierID("Sin Modifier")]
public class TestModifier : ProceduralImageModifier
{
    public override Vector4 CalculateRadius(Rect imageRect)
    {
        float r = Mathf.Exp(imageRect.width + imageRect.height);
        return new Vector4(r, r, r, r);
    }
}