using UnityEngine;

namespace Amnabi;

public static class ToyBoxRotation
{
    public static Vector3 crossVector(Vector3 v1, Vector3 v2)
    {
        v1 = v1.normalized;
        v2 = v2.normalized;
        var x = v1.x;
        var y = v1.y;
        var z = v1.z;
        var x2 = v2.x;
        var y2 = v2.y;
        var z2 = v2.z;
        var num = (y * z2) - (y2 * z);
        var num2 = (z * x2) - (z2 * x);
        var num3 = (x * y2) - (x2 * y);
        if (num == 0f && num2 == 0f && num3 == 0f)
        {
            return default;
        }

        return new Vector3(num, num2, num3);
    }

    public static float getVectorValue(Vector3 pos, Vector3 pole)
    {
        return ((pole.x * pos.x) + (pole.y * pos.y) + (pole.z * pos.z)) / pole.magnitude;
    }

    public static float angleToUp(Vector3 posCent0, Vector3 pole)
    {
        var up = Vector3.up;
        var pole2 = crossVector(up, pole);
        var vectorValue = getVectorValue(posCent0, up);
        var vectorValue2 = getVectorValue(posCent0, pole2);
        return Mathf.Atan2(vectorValue2, vectorValue);
    }
}