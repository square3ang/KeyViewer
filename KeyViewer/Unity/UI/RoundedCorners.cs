﻿using KeyViewer.Core;
using UnityEngine;
using UnityEngine.UI;

namespace KeyViewer.Unity.UI
{
    [ExecuteInEditMode]								//Required to check the OnEnable function
    [DisallowMultipleComponent]                     //You can only have one of these in every object.
    [RequireComponent(typeof(RectTransform))]
    public class RoundedCorners : MonoBehaviour
    {
        private static readonly int Props = Shader.PropertyToID("_WidthHeightRadius");

        public float radius = 40f;
        public Material material;

        [HideInInspector, SerializeField] private MaskableGraphic image;

        private void OnValidate()
        {
            Validate();
            Refresh();
        }

        private void OnDestroy()
        {
            image.material = null;      //This makes so that when the component is removed, the UI material returns to null

            Destroy(material);
            image = null;
            material = null;
        }

        private void OnEnable()
        {
            Validate();
            Refresh();
        }

        private void OnRectTransformDimensionsChange()
        {
            if (enabled && material != null)
            {
                Refresh();
            }
        }

        public void Validate()
        {
            if (material == null)
            {
                material = new Material(AssetManager.RoundedCorners);
            }

            if (image == null)
            {
                TryGetComponent(out image);
            }

            if (image != null)
            {
                image.material = material;
            }
        }

        public void Refresh()
        {
            var rect = ((RectTransform)transform).rect;

            //Multiply radius value by 2 to make the radius value appear consistent with ImageWithIndependentRoundedCorners script.
            //Right now, the ImageWithIndependentRoundedCorners appears to have double the radius than this.
            material.SetVector(Props, new Vector4(rect.width, rect.height, radius * 2, 0));
        }
    }
}