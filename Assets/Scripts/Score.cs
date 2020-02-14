using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Score
{
    private static float score = 0;

    public static float GetScore()
    {
        return score;
    }

    public static void AddScore(float value)
    {
        score += value;
    }

    public static void ResetScore()
    {
        score = 0;
    }
}
