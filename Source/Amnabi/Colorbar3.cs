using System;
using UnityEngine;
using Verse;

namespace Amnabi
{
    public class Colorbar3
    {
        private static bool SameAs(Color a, Color b)
        {
            return Math.Abs(a.r - b.r) + Math.Abs(a.g - b.g) + Math.Abs(a.b - b.b) < 0.001f;
        }

        public bool SelectColor(Rect rect, ref Color color)
        {
            GUI.BeginGroup(rect);
            var rect2 = new Rect(0f, 0f, rect.height, rect.height);
            GUI.color = Color.white;
            GUI.DrawTexture(rect2, BaseContent.WhiteTex);
            GUI.color = color;
            GUI.DrawTexture(rect2.ContractedBy(1f), BaseContent.WhiteTex);
            var color2 = new Color(1f, 1f, 1f, 1f);
            GUI.color = Color.red;
            color2.r = GUI.HorizontalSlider(new Rect(rect.height, 0f, rect.width - rect.height, rect.height / 3f),
                color.r, 0f, 1f);
            GUI.color = Color.green;
            color2.g = GUI.HorizontalSlider(
                new Rect(rect.height, rect.height / 3f, rect.width - rect.height, rect.height / 3f), color.g, 0f, 1f);
            GUI.color = Color.blue;
            color2.b = GUI.HorizontalSlider(
                new Rect(rect.height, rect.height * 2f / 3f, rect.width - rect.height, rect.height / 3f), color.b, 0f,
                1f);
            GUI.color = Color.white;
            GUI.EndGroup();
            if (SameAs(color2, color))
            {
                return false;
            }

            color = color2;
            return true;
        }
    }
}