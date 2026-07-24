using UnityEngine;
using UnityEngine.UI;

namespace CoreKit.UI
{
    [RequireComponent(typeof(CanvasRenderer))]
    public class InvisibleRaycastTarget : Graphic
    {
        protected override void UpdateGeometry() { }
    }
}
