using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UIElements.Image;

namespace Magar
{
    public class UIStripeMovement : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private float _x, _y;

        private void Update()
        {
        //     image.uvRect = new Rect(
        //         image.uvRect.position + new Vector2(_x, _y) * Time.unscaledDeltaTime,
        //         image.uvRect.size
        //     );
        }
    }
}
