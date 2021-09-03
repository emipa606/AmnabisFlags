using UnityEngine;
using Verse;

namespace Amnabi
{
    [StaticConstructorOnStartup]
    public static class StaticMeshHelper
    {
        public static float outlineExtra;

        static StaticMeshHelper()
        {
            outlineExtra = 0.045f;
            for (var i = 0; i < FlagSettings.renderFancyWaveInt; i++)
            {
                CompFlag.a239n7a7.Add(CompFlag.WaveIndexedMesh(new Vector2(1.5f, 1f), i,
                    FlagSettings.renderFancyWaveInt, false, 0f, 0f, true));
                CompFlag.a239n7b7.Add(CompFlag.WaveIndexedMesh(new Vector2(1.5f, 1f), i,
                    FlagSettings.renderFancyWaveInt, true, 0f, 0f, true));
                CompFlag.a239n7a9.Add(CompFlag.WaveIndexedMesh(new Vector2(1.5f, 1f), i,
                    FlagSettings.renderFancyWaveInt, false));
                CompFlag.a239n7b9.Add(CompFlag.WaveIndexedMesh(new Vector2(1.5f, 1f), i,
                    FlagSettings.renderFancyWaveInt, true));
                CompFlag.a239n7a8.Add(CompFlag.WaveIndexedMesh(new Vector2(1.5f, 1f), i,
                    FlagSettings.renderFancyWaveInt, false, outlineExtra));
                CompFlag.a239n7b8.Add(CompFlag.WaveIndexedMesh(new Vector2(1.5f, 1f), i,
                    FlagSettings.renderFancyWaveInt, true, outlineExtra));
            }

            CompFlag.FlagMesh1_5 = CompFlag.WaveIndexedMesh(new Vector2(1.5f, 1f), 0, 1, false);
            CompFlag.FlagMesh1_5R = CompFlag.WaveIndexedMesh(new Vector2(1.5f, 1f), 0, 1, true);
            CompFlag.FlagMesh1_5O = CompFlag.WaveIndexedMesh(new Vector2(1.5f, 1f), 0, 1, false, outlineExtra);
            CompFlag.FlagMesh1_5RO = CompFlag.WaveIndexedMesh(new Vector2(1.5f, 1f), 0, 1, true, outlineExtra);
            CompFlag.TrueFlipped10 = MeshMakerPlanes.NewPlaneMesh(new Vector2(1f, -1f), false, false, false);
            CompFlag.YFLIPFlagMesh1_5 = CompFlag.WaveIndexedMesh(new Vector2(1.5f, 1f), 0, 1, false, 0f, 0f, true);
            CompFlag.YFLIPFlagMesh1_5R = CompFlag.WaveIndexedMesh(new Vector2(1.5f, 1f), 0, 1, true, 0f, 0f, true);
            CompFlag.Wall_South = CompFlag.WaveIndexedMesh(new Vector3(0.25f, 0f, 0f), new Vector3(0.25f, 0f, 0.4375f),
                new Vector3(0.75f, 0f, 0.4375f), new Vector3(0.75f, 0f, 0f));
            CompFlag.Wall_North = CompFlag.WaveIndexedMesh(new Vector3(25f / 32f, 0f, 1f),
                new Vector3(25f / 32f, 0f, 31f / 32f), new Vector3(13f / 64f, 0f, 31f / 32f),
                new Vector3(13f / 64f, 0f, 1f));
            CompFlag.Wall_East = CompFlag.WaveIndexedMesh(new Vector3(1f, 0f, 5f / 32f),
                new Vector3(0.75f, 0f, 13f / 32f), new Vector3(0.75f, 0f, 61f / 64f), new Vector3(1f, 0f, 0.75f));
            CompFlag.Wall_West = CompFlag.WaveIndexedMesh(new Vector3(0f, 0f, 0.75f), new Vector3(0.25f, 0f, 61f / 64f),
                new Vector3(0.25f, 0f, 13f / 32f), new Vector3(0f, 0f, 5f / 32f));
        }
    }
}