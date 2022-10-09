using UnityEngine;

namespace Amnabi;

public static class PatternRecursiveStatic
{
    public static FlagPattern createFlagPatternMinScaled(this PatternLayer layer, FlagPatternDef fpd, Color c,
        double x, double y, double angleDeg, PositioningType xyType = PositioningType.Percentage)
    {
        return layer.createFlagPattern(fpd, c, layer.MinDims(), x, y, angleDeg, xyType);
    }

    public static FlagPattern createFlagPattern(this PatternLayer layer, FlagPatternDef fpd, Color c, float scale,
        double x, double y, double angleDeg, PositioningType xyType = PositioningType.Percentage)
    {
        return layer.createFlagPattern(fpd, c, scale, scale, x, y, angleDeg, xyType);
    }

    public static FlagPattern createFlagPattern(this PatternLayer layer, FlagPatternDef fpd, Color c, float scalX,
        float scalY, double x, double y, double angleDeg, PositioningType xyType = PositioningType.Percentage)
    {
        switch (xyType)
        {
            case PositioningType.Percentage:
                x = ((layer.innerrect.width - scalX) * x) + layer.innerrect.x + (scalX / 2f);
                y = ((layer.innerrect.height - scalY) * y) + layer.innerrect.y + (scalY / 2f);
                break;
            case PositioningType.CorneredCenterPercentage:
                x = (layer.innerrect.width * x) + layer.innerrect.x;
                y = (layer.innerrect.height * y) + layer.innerrect.y;
                break;
            case PositioningType.Relative:
                x += layer.innerrect.x;
                y += layer.innerrect.y;
                break;
        }

        return PatternRecursive.createFlagPattern(fpd, c, x, y, scalX * fpd.generateScale.x,
            scalY * fpd.generateScale.y, fpd.getRandomAngle() + angleDeg);
    }

    public static FlagPattern createFlagPatternMaxScaled(this PatternLayer layer, FlagPatternDef fpd, Color c,
        double x, double y, double angleDeg, PositioningType xyType = PositioningType.Percentage)
    {
        return layer.createFlagPattern(fpd, c, layer.MaxDims(), x, y, angleDeg, xyType);
    }
}