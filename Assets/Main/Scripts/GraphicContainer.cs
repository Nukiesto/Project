using UnityEngine;
using UnityEngine.UI;

namespace Main
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class GraphicContainer : Graphic
    {
        [SerializeField] private Graphic[] graphics;

        public override void CrossFadeColor(Color targetColor, float duration, bool ignoreTimeScale, bool useAlpha, bool useRGB)
        {
            foreach (var graphic in graphics)
                if (graphic != null)
                    graphic.CrossFadeColor(targetColor, duration, ignoreTimeScale, useAlpha, useRGB);
        }

        public override void CrossFadeAlpha(float alpha, float duration, bool ignoreTimeScale)
        {
            foreach (var graphic in graphics)
                if (graphic != null)
                    graphic.CrossFadeAlpha(alpha, duration, ignoreTimeScale);
        }
    }
}