using System;
using System.Collections.Generic;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Amnabi;

public class CompFlag : ThingComp, ISettlementAssignableFlag
{
    public static readonly Dictionary<BodyTypeDef, OffsetRotation> eastReJigger;

    public static readonly Dictionary<BodyTypeDef, OffsetRotation> southReJigger;

    public static readonly Dictionary<BodyTypeDef, OffsetRotation> northReJigger;

    public static readonly Dictionary<BodyTypeDef, OffsetRotation> westReJigger;

    public static readonly List<Mesh> a239n7a9;

    public static readonly List<Mesh> a239n7b9;

    public static readonly List<Mesh> a239n7a7;

    public static readonly List<Mesh> a239n7b7;

    public static readonly List<Mesh> a239n7a8;

    public static readonly List<Mesh> a239n7b8;

    public static Mesh FlagMesh1_5O;

    public static Mesh FlagMesh1_5RO;

    public static Mesh FlagMesh1_5;

    public static Mesh FlagMesh1_5R;

    public static Mesh YFLIPFlagMesh1_5;

    public static Mesh YFLIPFlagMesh1_5R;

    public static Mesh TrueFlipped10;

    public static Mesh Wall_North;

    public static Mesh Wall_East;

    public static Mesh Wall_South;

    public static Mesh Wall_West;

    public static Vector3 offsetOutlineVector;

    private float angleSmooth;

    public Graphic_Single cachedGraphicStick;

    public Graphic_Single cachedGraphicTop;

    public Flag flag;

    public string flagID = "TILEID-1";

    public Flag flagSaveCompatability;

    static CompFlag()
    {
        eastReJigger = new Dictionary<BodyTypeDef, OffsetRotation>();
        southReJigger = new Dictionary<BodyTypeDef, OffsetRotation>();
        northReJigger = new Dictionary<BodyTypeDef, OffsetRotation>();
        westReJigger = new Dictionary<BodyTypeDef, OffsetRotation>();
        a239n7a9 = [];
        a239n7b9 = [];
        a239n7a7 = [];
        a239n7b7 = [];
        a239n7a8 = [];
        a239n7b8 = [];
        offsetOutlineVector = new Vector3(0f, -0.1f, 0f);
        eastReJigger.Add(BodyTypeDefOf.Female, new OffsetRotation
        {
            offset = new Vector3(-0.28f, 0f, 0.1f),
            rotate = Quaternion.AngleAxis(10f, Vector3.up)
        });
        southReJigger.Add(BodyTypeDefOf.Female, new OffsetRotation
        {
            offset = new Vector3(0f, 0f, 0.1f),
            rotate = Quaternion.identity
        });
        northReJigger.Add(BodyTypeDefOf.Female, new OffsetRotation
        {
            offset = new Vector3(0f, 0f, 0.1f),
            rotate = Quaternion.identity
        });
        foreach (var key in eastReJigger.Keys)
        {
            var offsetRotation = new OffsetRotation
            {
                offset = eastReJigger[key].offset
            };
            offsetRotation.offset.x = 0f - offsetRotation.offset.x;
            offsetRotation.rotate = eastReJigger[key].rotate;
            offsetRotation.rotate.ToAngleAxis(out var axis, out var angle);
            offsetRotation.rotate = Quaternion.AngleAxis(axis, Vector3.zero - angle);
            westReJigger.Add(key, offsetRotation);
        }
    }

    public CompProperties_Flag Props => (CompProperties_Flag)props;

    public IEnumerable<object> AssignablePlayerFaction
    {
        get { yield return Faction.OfPlayer; }
    }

    public IEnumerable<object> AssignableOtherFaction
    {
        get
        {
            foreach (var ff in Find.FactionManager.AllFactions)
            {
                if (ff != Faction.OfPlayer)
                {
                    yield return ff;
                }
            }
        }
    }

    public IEnumerable<object> AssignablePlayerSettlement
    {
        get
        {
            foreach (var settlement in Find.WorldObjects.Settlements)
            {
                if (settlement.Faction == Faction.OfPlayer)
                {
                    yield return settlement;
                }
            }
        }
    }

    public IEnumerable<object> AssignableOtherSettlement
    {
        get
        {
            foreach (var settlement in Find.WorldObjects.Settlements)
            {
                if (settlement.Faction != Faction.OfPlayer)
                {
                    yield return settlement;
                }
            }
        }
    }

    public Flag getFlag()
    {
        return flag;
    }

    public string currentFlagID()
    {
        return flagID;
    }

    public void unassign()
    {
        flagID = "TILEID-1";
        flag = null;
    }

    public void TryAssignObject(object obj, bool update = true)
    {
        switch (obj)
        {
            case Settlement settlement:
                flagID = $"TILEID{settlement.Tile}";
                break;
            case Faction faction:
                flagID = $"FACTIONID{faction.loadID}";
                break;
            case string text:
                flagID = text;
                break;
        }

        if (flagID.Equals("TILEID-1"))
        {
            unassign();
            return;
        }

        flag = FlagsCore.GetFlag(flagID);
        if (update)
        {
            updateTexture();
        }
    }

    public static OffsetRotation getOffsetRotation(Pawn p)
    {
        var bodyTypeDef = p.story?.bodyType;
        if (bodyTypeDef == null || !eastReJigger.ContainsKey(bodyTypeDef))
        {
            bodyTypeDef = BodyTypeDefOf.Female;
        }

        return p.Rotation.AsInt switch
        {
            0 => northReJigger[bodyTypeDef],
            1 => eastReJigger[bodyTypeDef],
            2 => southReJigger[bodyTypeDef],
            3 => westReJigger[bodyTypeDef],
            _ => null
        };
    }

    public static float flipYFunc(float Y, bool doit)
    {
        if (doit)
        {
            Y = 1f - Y;
        }

        return Y;
    }

    public static Mesh WaveIndexedMesh(Vector2 size, int index, int iTotal, bool backSide, float expand = 0f,
        float depth = 0f, bool flipY = false)
    {
        var num = size.x / iTotal;
        var num3 = 1f / iTotal;
        var array = new Vector3[4];
        var array2 = new Vector2[4];
        var array3 = new int[6];
        array[0] = new Vector3(0f - expand, depth, backSide ? size.y + expand : 0f - expand);
        array[1] = new Vector3(0f - expand, depth, backSide ? 0f - expand : size.y + expand);
        array[2] = new Vector3(num + expand, depth, backSide ? 0f : size.y + expand);
        array[3] = new Vector3(num + expand, depth, backSide ? size.y : 0f - expand);
        array2[0] = new Vector2(num3 * index, flipYFunc(backSide ? 1f : 0f, flipY));
        array2[1] = new Vector2(num3 * index, flipYFunc(backSide ? 0f : 1f, flipY));
        array2[2] = new Vector2(num3 * (index + 1), flipYFunc(backSide ? 0f : 1f, flipY));
        array2[3] = new Vector2(num3 * (index + 1), flipYFunc(backSide ? 1f : 0f, flipY));
        array3[0] = 0;
        array3[1] = 1;
        array3[2] = 2;
        array3[3] = 0;
        array3[4] = 2;
        array3[5] = 3;
        var mesh = new Mesh
        {
            name = "NewPlaneMesh()",
            vertices = array,
            uv = array2
        };
        mesh.SetTriangles(array3, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static Mesh WaveIndexedMesh(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
    {
        var array = new Vector3[4];
        var array2 = new Vector2[4];
        var array3 = new int[6];
        array[0] = a;
        array[1] = b;
        array[2] = c;
        array[3] = d;
        array2[0] = new Vector2(0f, 0f);
        array2[1] = new Vector2(0f, 1f);
        array2[2] = new Vector2(1f, 1f);
        array2[3] = new Vector2(1f, 0f);
        array3[0] = 0;
        array3[1] = 1;
        array3[2] = 2;
        array3[3] = 0;
        array3[4] = 2;
        array3[5] = 3;
        var mesh = new Mesh
        {
            name = "NewPlaneMesh()",
            vertices = array,
            uv = array2
        };
        mesh.SetTriangles(array3, 0);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static List<Mesh> YFLIPmeshArrayFor(int mLength)
    {
        return a239n7a7;
    }

    public static List<Mesh> YFLIPmeshArrayForR(int mLength)
    {
        return a239n7b7;
    }

    public static List<Mesh> meshArrayFor(int mLength)
    {
        return a239n7a9;
    }

    public static List<Mesh> meshArrayForR(int mLength)
    {
        return a239n7b9;
    }

    public static List<Mesh> outlineMeshArrayFor(int mLength)
    {
        return a239n7a8;
    }

    public static List<Mesh> outlineMeshArrayForR(int mLength)
    {
        return a239n7b8;
    }

    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        if (!respawningAfterLoad)
        {
            AssignFlagFromWearer();
        }
    }

    public void AssignFlagFromWearer()
    {
        var apparel = default(Apparel);
        int num;
        if (parent != null)
        {
            apparel = parent as Apparel;
            if (apparel != null)
            {
                num = apparel.Wearer != null ? 1 : 0;
                goto IL_0024;
            }
        }

        num = 0;
        IL_0024:
        if (num == 0)
        {
            return;
        }

        var faction = apparel?.Wearer?.Faction;
        if (faction == null || faction == Faction.OfPlayer)
        {
            return;
        }

        var flagFaction = FlagsCore.GetFlagFaction(faction);
        if (flagFaction != null)
        {
            TryAssignObject(faction);
        }
    }

    public override void PostDraw()
    {
        base.PostDraw();
        if (flag == null || flagID.Equals("TILEID-1"))
        {
            return;
        }

        if (parent is not Building building)
        {
            return;
        }

        if (Props.drawOnWall)
        {
            var q = Quaternion.AngleAxis(0f, Vector3.up);
            var drawPos = parent.DrawPos;
            drawPos.y += 1.01f;
            drawPos.x += (Props.offsetX - 0.5f) * Props.scaleX;
            drawPos.z += (Props.offsetY - 0.5f) * Props.scaleY;
            var s = new Vector3(Props.scaleX, 1f, Props.scaleY);
            var matrix = default(Matrix4x4);
            matrix.SetTRS(drawPos, q, s);
            switch (parent.Rotation.AsInt)
            {
                case 0:
                    Graphics.DrawMesh(Wall_North, matrix, flag.getFlagMaterial(), 0);
                    break;
                case 1:
                    Graphics.DrawMesh(Wall_East, matrix, flag.getFlagMaterial(), 0);
                    break;
                case 2:
                    Graphics.DrawMesh(Wall_South, matrix, flag.getFlagMaterial(), 0);
                    break;
                case 3:
                    Graphics.DrawMesh(Wall_West, matrix, flag.getFlagMaterial(), 0);
                    break;
            }

            return;
        }

        var num = 1f;
        var room = building.GetRoom(RegionType.Normal | RegionType.Portal);
        if (room is { UsesOutdoorTemperature: false })
        {
            num = 0f;
        }
        else if (building.Map is { weatherManager: not null })
        {
            num += building.Map.weatherManager.CurWindSpeedOffset;
            num *= building.Map.weatherManager.CurWindSpeedFactor;
            num = Mathf.Max(num, 0.66f);
        }

        switch (Props.FlagDisplayType)
        {
            case FlagDisplayType.Default:
                if (FlagSettings.renderFancyWave)
                {
                    var quaternion2 = Quaternion.AngleAxis(20f, Vector3.left);
                    var y2 = parent.DrawPos.y + 1.01f;
                    var drawPos4 = parent.DrawPos;
                    var vector7 = parent.DrawPos + (quaternion2 * new Vector3(0f, 0f, 1f * Props.scaleY));
                    drawPos4.x += Props.offsetX * Props.scaleX;
                    drawPos4.z += Props.offsetY * Props.scaleY;
                    vector7.x += Props.offsetX * Props.scaleX;
                    vector7.z += Props.offsetY * Props.scaleY;
                    for (var j = 0; j < FlagSettings.renderFancyWaveInt; j++)
                    {
                        drawPos4.y = 0f;
                        vector7.y = 0f;
                        var angle3 = num *
                                     Mathf.Cos((((Find.TickManager.TicksGame * 4f) + (parent.thingIDNumber * 89)) /
                                                90f) +
                                               (j / (float)FlagSettings.renderFancyWaveInt)) * 9f /
                                     FlagSettings.renderFancyWaveInt;
                        var angle4 = num *
                                     Mathf.Cos((((Find.TickManager.TicksGame * 4f) + (parent.thingIDNumber * 89)) /
                                                70f) +
                                               (j / (float)FlagSettings.renderFancyWaveInt)) * 9f /
                                     FlagSettings.renderFancyWaveInt;
                        if (j != 0)
                        {
                            quaternion2 = quaternion2 * Quaternion.AngleAxis(angle3, Vector3.up) *
                                          Quaternion.AngleAxis(angle4, Vector3.left);
                        }

                        var vector8 = quaternion2 *
                                      new Vector3(1.5f * Props.scaleX / FlagSettings.renderFancyWaveInt, 0f, 0f);
                        var vector9 = quaternion2 *
                                      new Vector3(1.5f * Props.scaleX / FlagSettings.renderFancyWaveInt, 0f,
                                          1f * Props.scaleY);
                        vector9 += drawPos4 - vector7;
                        vector8.y = 0f;
                        vector9.y = 0f;
                        var vector10 = Vector3.zero;
                        if (vector8.magnitude < vector9.magnitude)
                        {
                            vector10 = vector8 - vector9;
                            vector9 += vector10;
                            vector8 += vector10;
                        }

                        var vector11 = drawPos4 + vector10;
                        vector11.y = y2;
                        var vector12 = new Vector3(Props.scaleX, 1f, Props.scaleY);
                        var matrix6 = Matrix4x4.identity * Matrix4x4.Translate(vector11) *
                                      Matrix4x4.Scale(vector12) * Matrix4x4.Rotate(quaternion2);
                        if (FlagSettings.useFlagOutline)
                        {
                            var matrix7 = Matrix4x4.identity * Matrix4x4.Translate(vector11) *
                                          Matrix4x4.Translate(offsetOutlineVector) * Matrix4x4.Scale(vector12) *
                                          Matrix4x4.Rotate(quaternion2);
                            Graphics.DrawMesh(outlineMeshArrayFor(FlagSettings.renderFancyWaveInt)[j], matrix7,
                                AmnabiFlagTextures.getFlagShapeOutline(flag.flagShape), 0);
                        }

                        Graphics.DrawMesh(meshArrayFor(FlagSettings.renderFancyWaveInt)[j], matrix6,
                            flag.getFlagMaterial(), 0);
                        drawPos4 += vector8;
                        vector7 += vector9;
                    }
                }
                else
                {
                    var q3 = Quaternion.AngleAxis(0f, Vector3.up);
                    var drawPos5 = parent.DrawPos;
                    drawPos5.y += 1.01f;
                    drawPos5.x += Props.offsetX * Props.scaleX;
                    drawPos5.z += Props.offsetY * Props.scaleY;
                    var s3 = new Vector3(Props.scaleX, 1f, Props.scaleY);
                    var matrix8 = default(Matrix4x4);
                    matrix8.SetTRS(drawPos5, q3, s3);
                    if (FlagSettings.useFlagOutline)
                    {
                        var matrix9 = default(Matrix4x4);
                        matrix9.SetTRS(drawPos5 + offsetOutlineVector, q3, s3);
                        Graphics.DrawMesh(FlagMesh1_5O, matrix9,
                            AmnabiFlagTextures.getFlagShapeOutline(flag.flagShape), 0);
                    }

                    Graphics.DrawMesh(FlagMesh1_5, matrix8, flag.getFlagMaterial(), 0);
                }

                break;
            case FlagDisplayType.Vertical:
                if (FlagSettings.renderFancyWave)
                {
                    var quaternion = Quaternion.AngleAxis(20f, Vector3.left) *
                                     Quaternion.AngleAxis(90f, Vector3.up);
                    var y = parent.DrawPos.y + 1.01f;
                    var drawPos2 = parent.DrawPos;
                    var vector = parent.DrawPos + (quaternion * new Vector3(0f, 0f, 1f * Props.scaleY));
                    drawPos2.x += Props.offsetX * Props.scaleX;
                    drawPos2.z += Props.offsetY * Props.scaleY;
                    vector.x += Props.offsetX * Props.scaleX;
                    vector.z += Props.offsetY * Props.scaleY;
                    for (var i = 0; i < FlagSettings.renderFancyWaveInt; i++)
                    {
                        drawPos2.y = 0f;
                        vector.y = 0f;
                        var angle = num *
                                    Mathf.Cos((((Find.TickManager.TicksGame * 4f) + (parent.thingIDNumber * 89)) /
                                               90f) +
                                              (i / (float)FlagSettings.renderFancyWaveInt)) * 9f /
                                    FlagSettings.renderFancyWaveInt;
                        var angle2 = num *
                                     Mathf.Cos((((Find.TickManager.TicksGame * 4f) + (parent.thingIDNumber * 89)) /
                                                70f) +
                                               (i / (float)FlagSettings.renderFancyWaveInt)) * 9f /
                                     FlagSettings.renderFancyWaveInt;
                        if (i != 0)
                        {
                            quaternion = quaternion * Quaternion.AngleAxis(angle, Vector3.up) *
                                         Quaternion.AngleAxis(angle2, Vector3.left);
                        }

                        var vector2 = quaternion *
                                      new Vector3(1.5f * Props.scaleX / FlagSettings.renderFancyWaveInt, 0f, 0f);
                        var vector3 = quaternion *
                                      new Vector3(1.5f * Props.scaleX / FlagSettings.renderFancyWaveInt, 0f,
                                          1f * Props.scaleY);
                        vector3 += drawPos2 - vector;
                        vector2.y = 0f;
                        vector3.y = 0f;
                        var vector4 = Vector3.zero;
                        if (vector2.magnitude < vector3.magnitude)
                        {
                            vector4 = vector2 - vector3;
                            vector3 += vector4;
                            vector2 += vector4;
                        }

                        var vector5 = drawPos2 + vector4;
                        vector5.y = y;
                        var vector6 = new Vector3(Props.scaleX, 1f, Props.scaleY);
                        var matrix2 = Matrix4x4.identity * Matrix4x4.Translate(vector5) * Matrix4x4.Scale(vector6) *
                                      Matrix4x4.Rotate(quaternion);
                        if (FlagSettings.useFlagOutline)
                        {
                            var matrix3 = Matrix4x4.identity * Matrix4x4.Translate(vector5) *
                                          Matrix4x4.Translate(offsetOutlineVector) * Matrix4x4.Scale(vector6) *
                                          Matrix4x4.Rotate(quaternion);
                            Graphics.DrawMesh(outlineMeshArrayFor(FlagSettings.renderFancyWaveInt)[i], matrix3,
                                AmnabiFlagTextures.getFlagShapeOutline(flag.flagShape), 0);
                        }

                        Graphics.DrawMesh(meshArrayFor(FlagSettings.renderFancyWaveInt)[i], matrix2,
                            flag.getFlagMaterial(), 0);
                        drawPos2 += vector2;
                        vector += vector3;
                    }
                }
                else
                {
                    var q2 = Quaternion.AngleAxis(0f, Vector3.up);
                    var drawPos3 = parent.DrawPos;
                    drawPos3.y += 1.01f;
                    drawPos3.x += Props.offsetX * Props.scaleX;
                    drawPos3.z += Props.offsetY * Props.scaleY;
                    var s2 = new Vector3(Props.scaleX, 1f, Props.scaleY);
                    var matrix4 = default(Matrix4x4);
                    matrix4.SetTRS(drawPos3, q2, s2);
                    if (FlagSettings.useFlagOutline)
                    {
                        var matrix5 = default(Matrix4x4);
                        matrix5.SetTRS(drawPos3 + offsetOutlineVector, q2, s2);
                        Graphics.DrawMesh(FlagMesh1_5O, matrix5,
                            AmnabiFlagTextures.getFlagShapeOutline(flag.flagShape), 0);
                    }

                    Graphics.DrawMesh(FlagMesh1_5, matrix4, flag.getFlagMaterial(), 0);
                }

                break;
        }
    }

    public void PawnWearDraw()
    {
        if (parent is not Apparel apparel)
        {
            return;
        }

        var wearer = apparel.Wearer;
        if (!(wearer?.Spawned ?? false))
        {
            return;
        }

        var num = FlagSettings.use3DBeltBanner ? 1f : 0f;
        var asAngle = wearer.Rotation.AsAngle;
        if (FlagSettings.use3DBeltBanner)
        {
            var num2 = Mathf.Abs(angleSmooth - asAngle);
            if (num2 < Mathf.Abs(((angleSmooth + 180f) % 360f) - ((asAngle + 180f) % 360f)))
            {
                angleSmooth = (angleSmooth * 0.9f) + (0.100000024f * asAngle);
            }
            else
            {
                angleSmooth = ((angleSmooth + 180f) % 360f * 0.9f) + (0.100000024f * ((asAngle + 180f) % 360f));
                angleSmooth -= 180f;
            }
        }
        else
        {
            angleSmooth = asAngle + 10f;
        }

        var num3 = angleSmooth;
        var offsetRotation = getOffsetRotation(wearer);
        var num4 = Props.scaleX * 5f;
        var num5 = Props.scaleY * 5f;
        var drawPos = wearer.DrawPos;
        var quaternion = Quaternion.AngleAxis(num * -60f, Vector3.left) * Quaternion.AngleAxis(0f, Vector3.up) *
                         offsetRotation.rotate;
        var vector = drawPos;
        vector.y = drawPos.y + (wearer.Rotation == Rot4.South ? -0.4f : 1f);
        vector.x += Props.offsetX;
        vector.z += Props.offsetY;
        vector += offsetRotation.offset;
        var s = new Vector3(num4, 1f, num5);
        var matrix = default(Matrix4x4);
        matrix.SetTRS(vector, quaternion, s);
        Graphics.DrawMesh(MeshPool.plane10, matrix, pawnBackGraphicStick().MatSingle, 0);
        Graphics.DrawMesh(TrueFlipped10, matrix, pawnBackGraphicStick().MatSingle, 0);
        var vector2 = new Vector3(s.x / 2f * 0f, s.y / 2f * 0f, s.z / 2f * 1f);
        var quaternion2 = quaternion;
        var vector3 = default(Vector3);
        var s2 = default(Vector3);
        switch (Props.FlagDisplayType)
        {
            case FlagDisplayType.Default:
                vector2.z -= num4 * 0.2f;
                quaternion2 *= Quaternion.AngleAxis(0f - num3, Vector3.forward);
                vector3 = vector + (quaternion * vector2);
                vector3.y = drawPos.y + (wearer.Rotation == Rot4.South ? -0.4f : 1f);
                s2 = new Vector3(num4 * 0.2f, 1f, num5 * 1f / 2f);
                break;
            case FlagDisplayType.Corner:
            {
                quaternion2 *= Quaternion.AngleAxis(0f - num3 + 90f, Vector3.forward);
                var posCent2 = quaternion2 * Vector3.up;
                var vector5 = quaternion2 * Vector3.left;
                var angle2 = (float)(ToyBoxRotation.angleToUp(posCent2, vector5) * 180f / Math.PI);
                vector3 = vector + (quaternion * vector2);
                vector3 += quaternion2 * Vector3.left * 0.98f / 2f;
                vector3.y = drawPos.y + (wearer.Rotation == Rot4.South ? -0.4f : 1f);
                s2 = new Vector3(num4 * 0.2f, 1f, num5 * 1f / 2f);
                var matrix3 = default(Matrix4x4);
                var q2 = Quaternion.AngleAxis(angle2, vector5) * quaternion2;
                matrix3.SetTRS(vector3, q2, s2);
                Graphics.DrawMesh(MeshPool.plane10, matrix3, pawnBackGraphicTop().MatSingle, 0);
                Graphics.DrawMesh(TrueFlipped10, matrix3, pawnBackGraphicTop().MatSingle, 0);
                break;
            }
            case FlagDisplayType.Vertical:
            {
                quaternion2 *= Quaternion.AngleAxis(0f - num3, Vector3.forward);
                var posCent = quaternion2 * Vector3.up;
                var vector4 = quaternion2 * Vector3.left;
                var angle = (float)(ToyBoxRotation.angleToUp(posCent, vector4) * 180f / Math.PI);
                vector3 = vector + (quaternion * vector2);
                vector3.y = drawPos.y + (wearer.Rotation == Rot4.South ? -0.4f : 1f);
                s2 = new Vector3(num4 * 0.2f, 1f, num5 * 1f / 2f);
                var matrix2 = default(Matrix4x4);
                var q = Quaternion.AngleAxis(angle, vector4) * quaternion2;
                matrix2.SetTRS(vector3, q, s2);
                Graphics.DrawMesh(MeshPool.plane10, matrix2, pawnBackGraphicTop().MatSingle, 0);
                Graphics.DrawMesh(TrueFlipped10, matrix2, pawnBackGraphicTop().MatSingle, 0);
                break;
            }
        }

        if (flag == null || flagID.Equals("TILEID-1"))
        {
            return;
        }

        var vector6 = default(Vector3);
        var vector7 = default(Vector3);
        var quaternion3 = quaternion2;
        switch (Props.FlagDisplayType)
        {
            case FlagDisplayType.Default:
                quaternion3 = quaternion3 * Quaternion.AngleAxis(-180f, Vector3.up) *
                              Quaternion.AngleAxis(-90f, Vector3.forward);
                vector6 = vector3 + (quaternion2 * new Vector3(0f, 0f, num4 * 0.2f));
                vector7 = vector3 + (quaternion2 * new Vector3(0f, 0f, num4 * -0.2f));
                break;
            case FlagDisplayType.Corner:
            {
                quaternion3 = quaternion3 * Quaternion.AngleAxis(-90f, Vector3.up) *
                              Quaternion.AngleAxis(-180f, Vector3.forward);
                var vector9 = new Vector3(s2.x / 2f * 1f, s2.y / 2f * 0f, s2.z / 2f * 0f);
                vector6 = vector3 + (quaternion2 * vector9);
                vector9 = new Vector3((0f - s2.x) / 2f * -1f, s2.y / 2f * 0f, s2.z / 2f * 0f);
                vector7 = vector3 + (quaternion2 * vector9);
                break;
            }
            case FlagDisplayType.Vertical:
            {
                quaternion3 = quaternion3 * Quaternion.AngleAxis(-90f, Vector3.up) *
                              Quaternion.AngleAxis(-170f, Vector3.forward);
                var vector8 = new Vector3(s2.x / 2f * 1f, s2.y / 2f * 0f, s2.z / 2f * 0f);
                vector6 = vector3 + (quaternion2 * vector8);
                vector8 = new Vector3((0f - s2.x) / 2f * 1f, s2.y / 2f * 0f, s2.z / 2f * 0f);
                vector7 = vector3 + (quaternion2 * vector8);
                break;
            }
        }

        var quaternion4 = quaternion3;
        vector6.y = drawPos.y + 1f;
        vector7.y = drawPos.y + 1f;
        var vector10 = new Vector3(num4, 1f, num5) / 5f;
        var vector11 = vector6;
        var vector12 = vector7;
        var y = drawPos.y;
        if (FlagSettings.swayBeltBanners)
        {
            for (var i = 0; i < FlagSettings.renderFancyWaveInt; i++)
            {
                vector11.y = 0f;
                vector12.y = 0f;
                var angle3 =
                    Mathf.Cos((((Find.TickManager.TicksGame * 4f) + (parent.thingIDNumber * 89)) / 90f) +
                              (i / (float)FlagSettings.renderFancyWaveInt)) * 9f / FlagSettings.renderFancyWaveInt;
                var angle4 =
                    Mathf.Cos((((Find.TickManager.TicksGame * 4f) + (parent.thingIDNumber * 89)) / 70f) +
                              (i / (float)FlagSettings.renderFancyWaveInt)) * 9f / FlagSettings.renderFancyWaveInt;
                if (i != 0)
                {
                    quaternion4 = quaternion4 * Quaternion.AngleAxis(angle3, Vector3.up) *
                                  Quaternion.AngleAxis(angle4, Vector3.left);
                }

                var vector13 = quaternion4 *
                               new Vector3(1.5f * vector10.x / FlagSettings.renderFancyWaveInt, 0f, 0f);
                var vector14 = quaternion4 * new Vector3(1.5f * vector10.x / FlagSettings.renderFancyWaveInt, 0f,
                    1f * vector10.y);
                vector14 += vector11 - vector12;
                vector13.y = 0f;
                vector14.y = 0f;
                var vector15 = Vector3.zero;
                if (i != 0 && vector13.magnitude < vector14.magnitude)
                {
                    vector15 = vector13 - vector14;
                    vector14 += vector15;
                    vector13 += vector15;
                }

                var vector16 = vector11 + vector15;
                vector16.y = y + (wearer.Rotation == Rot4.South ? -0.4f : 1f);
                var matrix4 = Matrix4x4.identity * Matrix4x4.Translate(vector16) * Matrix4x4.Scale(vector10) *
                              Matrix4x4.Rotate(quaternion4);
                if (FlagSettings.useFlagOutlineBelt)
                {
                    var matrix5 = Matrix4x4.identity * Matrix4x4.Translate(vector16 + offsetOutlineVector) *
                                  Matrix4x4.Scale(vector10) * Matrix4x4.Rotate(quaternion4);
                    Graphics.DrawMesh(outlineMeshArrayFor(FlagSettings.renderFancyWaveInt)[i], matrix5,
                        AmnabiFlagTextures.getFlagShapeOutline(flag.flagShape), 0);
                    Graphics.DrawMesh(outlineMeshArrayForR(FlagSettings.renderFancyWaveInt)[i], matrix5,
                        AmnabiFlagTextures.getFlagShapeOutline(flag.flagShape), 0);
                }

                Graphics.DrawMesh(YFLIPmeshArrayFor(FlagSettings.renderFancyWaveInt)[i], matrix4,
                    flag.getFlagMaterial(), 0);
                Graphics.DrawMesh(YFLIPmeshArrayForR(FlagSettings.renderFancyWaveInt)[i], matrix4,
                    flag.getFlagMaterial(), 0);
                vector11 += vector13;
                vector12 += vector14;
            }
        }
        else
        {
            var vector17 = vector11;
            vector17.y = y + (wearer.Rotation == Rot4.South ? -0.4f : 1f);
            var matrix6 = Matrix4x4.identity * Matrix4x4.Translate(vector17) * Matrix4x4.Scale(vector10) *
                          Matrix4x4.Rotate(quaternion4);
            if (FlagSettings.useFlagOutlineBelt)
            {
                var matrix7 = Matrix4x4.identity * Matrix4x4.Translate(vector17 + offsetOutlineVector) *
                              Matrix4x4.Scale(vector10) * Matrix4x4.Rotate(quaternion4);
                Graphics.DrawMesh(FlagMesh1_5O, matrix7, AmnabiFlagTextures.getFlagShapeOutline(flag.flagShape), 0);
                Graphics.DrawMesh(FlagMesh1_5RO, matrix7, AmnabiFlagTextures.getFlagShapeOutline(flag.flagShape),
                    0);
            }

            Graphics.DrawMesh(YFLIPFlagMesh1_5, matrix6, flag.getFlagMaterial(), 0);
            Graphics.DrawMesh(YFLIPFlagMesh1_5R, matrix6, flag.getFlagMaterial(), 0);
        }
    }

    public override IEnumerable<Gizmo> CompGetGizmosExtra()
    {
        foreach (var item in base.CompGetGizmosExtra())
        {
            yield return item;
        }

        yield return new Command_Action
        {
            defaultLabel = "Amnabi.ChooseProvince".Translate(),
            icon = SettleUtility.SettleCommandTex,
            defaultDesc = "Amnabi.ChooseProvince.Desc".Translate(),
            action = delegate { Find.WindowStack.Add(new Dialog_AssignPurposeToFlag(this)); },
            hotKey = KeyBindingDefOf.Misc3
        };
        yield return new Command_Action
        {
            defaultLabel = "Amnabi.DesignFlag".Translate(),
            icon = AmnabiFlagTextures.Copy,
            defaultDesc = "Amnabi.DesignFlag.Desc".Translate(),
            Disabled = flagID.Equals("TILEID-1"),
            action = delegate { Find.WindowStack.Add(new Dialog_ModifyFlag(this, new Vector2(1.5f, 1f))); },
            hotKey = KeyBindingDefOf.Misc3
        };
        yield return new Command_Action
        {
            defaultLabel = "Amnabi.Preset".Translate(),
            icon = AmnabiFlagTextures.Paste,
            defaultDesc = "Amnabi.Preset.Desc".Translate(),
            Disabled = flagID.Equals("TILEID-1"),
            action = delegate { Find.WindowStack.Add(new Dialog_SaveLoadFlag(this)); },
            hotKey = KeyBindingDefOf.Misc3
        };
    }

    public override IEnumerable<Gizmo> CompGetWornGizmosExtra()
    {
        foreach (var item in base.CompGetWornGizmosExtra())
        {
            yield return item;
        }

        yield return new Command_Action
        {
            defaultLabel = "Amnabi.ChooseProvince".Translate(),
            icon = SettleUtility.SettleCommandTex,
            defaultDesc = "Amnabi.ChooseProvince.Desc".Translate(),
            action = delegate { Find.WindowStack.Add(new Dialog_AssignPurposeToFlag(this)); },
            hotKey = KeyBindingDefOf.Misc3
        };
        yield return new Command_Action
        {
            defaultLabel = "Amnabi.DesignFlag".Translate(),
            icon = AmnabiFlagTextures.Copy,
            defaultDesc = "Amnabi.DesignFlag.Desc".Translate(),
            Disabled = flagID.Equals("TILEID-1"),
            action = delegate { Find.WindowStack.Add(new Dialog_ModifyFlag(this, new Vector2(1.5f, 1f))); },
            hotKey = KeyBindingDefOf.Misc3
        };
        yield return new Command_Action
        {
            defaultLabel = "Amnabi.Preset".Translate(),
            icon = AmnabiFlagTextures.Paste,
            defaultDesc = "Amnabi.Preset.Desc".Translate(),
            Disabled = flagID.Equals("TILEID-1"),
            action = delegate { Find.WindowStack.Add(new Dialog_SaveLoadFlag(this)); },
            hotKey = KeyBindingDefOf.Misc3
        };
    }

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref flagID, "RimFlag_flagID", "TILEID-1");
        if (Scribe.mode != LoadSaveMode.Saving &&
            (flagID == null || flagID.Equals("TILEID-1") || FlagsCore.GetFlag(flagID) == null))
        {
            Scribe_Deep.Look(ref flagSaveCompatability, "RimFlag_Flag", null);
            if (flagSaveCompatability != null)
            {
                if (FlagsCore.GetFlag(flagID) == null)
                {
                    FlagsCore.CreateFlag(flagID).inheritFlag(flagSaveCompatability, true);
                }
            }
        }

        Scribe_References.Look(ref flag, "RimFlag_FlagTrue");
        if (Scribe.mode != LoadSaveMode.PostLoadInit)
        {
            return;
        }

        if (flag == null && flagID != null)
        {
            flag = FlagsCore.GetFlag(flagID, true);
        }

        if (flag == null)
        {
            return;
        }

        if (FlagsCore.GetFlag(flagID, true) == null)
        {
            if (flagID != null)
            {
                FlagsCore.flagIDToFlag.Add(flagID, flag);
            }
        }
        else if (FlagsCore.GetFlag(flagID, true) != flag)
        {
            FlagsCore.GetFlag(flagID, true).recycle();
            if (flagID == null)
            {
                return;
            }

            FlagsCore.flagIDToFlag.Remove(flagID);
            FlagsCore.flagIDToFlag.Add(flagID, flag);
        }
    }

    public void updateTexture()
    {
        flag?.compileFlag();
    }

    public Graphic_Single pawnBackGraphicStick()
    {
        if (cachedGraphicStick == null)
        {
            cachedGraphicStick = (Graphic_Single)GraphicDatabase.Get<Graphic_Single>(Props.graphicPathStick,
                ShaderDatabase.Cutout, Vector2.one, parent.def.GetColorForStuff(parent.Stuff));
        }

        return cachedGraphicStick;
    }

    public Graphic_Single pawnBackGraphicTop()
    {
        if (cachedGraphicTop == null)
        {
            cachedGraphicTop = (Graphic_Single)GraphicDatabase.Get<Graphic_Single>(Props.graphicPathTop,
                ShaderDatabase.Cutout, Vector2.one, parent.def.GetColorForStuff(parent.Stuff));
        }

        return cachedGraphicTop;
    }
}