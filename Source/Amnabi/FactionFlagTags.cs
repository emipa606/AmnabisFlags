using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Amnabi
{
    public class FactionFlagTags : IExposable
    {
        public static Dictionary<string, ThemeTag> stringToTheme;

        public static List<string> uehf8742;

        public Dictionary<string, ListColor> colorThemes = new Dictionary<string, ListColor>();
        public Flag nationalFlag;

        public Dictionary<string, double> themeTags = new Dictionary<string, double>();

        static FactionFlagTags()
        {
            stringToTheme = new Dictionary<string, ThemeTag>();
            uehf8742 = new List<string>();
            stringToTheme.Add("Colonial", new Theme_Colonial());
            stringToTheme.Add("Imperial", new Theme_Imperial());
            stringToTheme.Add("Global", new Theme_Global());
        }

        public virtual void ExposeData()
        {
            Scribe_Collections.Look(ref themeTags, "themeTags", LookMode.Value, LookMode.Value);
        }

        public Color getBestColor(PatternLayer backGroundContrast, string extraTag)
        {
            if (!colorThemes.ContainsKey(extraTag))
            {
                extraTag = "Background";
            }

            if (!colorThemes.ContainsKey(extraTag))
            {
                return Color.white;
            }

            return colorThemes[extraTag].internalList.MaxBy(x =>
                (backGroundContrast.currentBackgroundColor.a *
                 ColorGeneratorT.GetContrast(backGroundContrast.currentBackgroundColor, x)) +
                (backGroundContrast.currentBackgroundColorN1.a *
                 ColorGeneratorT.GetContrast(backGroundContrast.currentBackgroundColorN1, x)));
        }

        public Color getRandomColor(string extraTag, int preferedIndex = -1)
        {
            if (!colorThemes.ContainsKey(extraTag))
            {
                extraTag = "Background";
            }

            if (!colorThemes.ContainsKey(extraTag))
            {
                return Color.white;
            }

            return preferedIndex == -1 || colorThemes[extraTag].internalList.Count <= preferedIndex
                ? colorThemes[extraTag].internalList.RandomElement()
                : colorThemes[extraTag].internalList[preferedIndex];
        }

        public void addColorTheme(string str, Color col)
        {
            if (!colorThemes.ContainsKey(str))
            {
                colorThemes.Add(str, new ListColor());
            }

            colorThemes[str].internalList.Add(col);
        }

        public void addColorTheme(string str, IEnumerable<Color> col)
        {
            if (!colorThemes.ContainsKey(str))
            {
                colorThemes.Add(str, new ListColor());
            }

            colorThemes[str].internalList.AddRange(col);
        }

        public void initialize(Faction faction)
        {
            Rand.PushState(((faction.loadID ^ 0x457982EC) * 4000) + (faction.loadID ^ 0x21FDC9C0));
            normalizeTheme();
            var key = themeTags.Count == 0
                ? "Imperial"
                : themeTags.Keys.RandomElementByWeight(x => (float)themeTags[x]);
            stringToTheme[key].generateColours(this);
            var patternLayer = new PatternLayer
            {
                innerrect = new Rect(0f, 0f, 1.5f, 1f)
            };
            patternLayer.setBackground(getRandomColor("Background"));
            patternLayer.iterateLayer(this, -1);
            var list = new List<FlagPattern>();
            patternLayer.compile(list);
            var flag = FlagsCore.CreateFlagFaction(faction);
            flag.patternStack.AddRange(list);
            flag.patternMax = list.Count;
            Rand.PopState();
        }

        public Flag initialize(Settlement stt)
        {
            Rand.PushState((((stt.Tile ^ 0x98C2772) * 20000) + stt.Tile) ^ 0x268AFC12);
            var patternLayer = new PatternLayer
            {
                innerrect = new Rect(0f, 0f, 1.5f, 1f)
            };
            patternLayer.setBackground(getRandomColor("Background"));
            patternLayer.iterateLayer(this, -1);
            var list = new List<FlagPattern>();
            patternLayer.compile(list);
            var flag = FlagsCore.CreateFlagSettlement(stt);
            flag.patternStack.AddRange(list);
            flag.patternMax = list.Count;
            Rand.PopState();
            return flag;
        }

        public void normalizeTheme()
        {
            uehf8742.Clear();
            var num = 0.0;
            foreach (var key in themeTags.Keys)
            {
                num += themeTags[key];
                uehf8742.Add(key);
            }

            foreach (var item in uehf8742)
            {
                var num2 = themeTags[item];
                themeTags.Remove(item);
                themeTags.Add(item, num2 / num);
            }

            uehf8742.Clear();
        }
    }
}