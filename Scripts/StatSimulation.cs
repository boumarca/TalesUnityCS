#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;

public class StatSimulation
{
    public enum Stats
    {
        HP,
        TP,
        STR,
        INT,
        DEF,
        ACC,
        EVA
    }

    [MenuItem("Tools/Simulate Stats")]
    public static void RunSimulation()
    {
        float[] currentGrowth = new float[7];
        float[] growthFactors = { 62.0f, 5.0f, 4.4f, 1.6f, 2.0f, 1.0f, 1.0f };
        const float minVariance = 0.95f;
        const float maxVariance = 1.05f;
        float[] minStat = { float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue };
        float[] maxStat = { float.MinValue, float.MinValue, float.MinValue, float.MinValue, float.MinValue, float.MinValue, float.MinValue };
        double[] averageSums = new double[7];
        const int iterations = 1000000;
        const int maxLevel = 250;

        for (int i = 0; i < iterations; i++)
        {
            for (int level = 0; level < maxLevel; level++)
            {
                for (int k = 0; k < currentGrowth.Length; k++)
                {
                    currentGrowth[k] += growthFactors[k] * Random.Range(minVariance, maxVariance);

                }
            }

            for (int stat = 0; stat < currentGrowth.Length; stat++)
            {
                float statValue = currentGrowth[stat];
                if (statValue > maxStat[stat])
                    maxStat[stat] = statValue;

                if (statValue < minStat[stat])
                    minStat[stat] = statValue;

                averageSums[stat] += statValue;
            }

            for (int j = 0; j < currentGrowth.Length; j++)
            {
                currentGrowth[j] = 0;
            }
        }

        StringBuilder sb = new();
        sb.AppendLine($"Pour {iterations} itérations au LV{maxLevel}:");
        for (int i = 0; i < currentGrowth.Length; i++)
        {
            sb.AppendLine($"{(Stats)i}: Min = {minStat[i]} Max = {maxStat[i]} Average = {averageSums[i] / iterations} Expected = {growthFactors[i] * maxLevel}");
        }

        Debug.Log(sb.ToString());
    }
}
#endif
