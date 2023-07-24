using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    
    public static string ConvertBigValue(float value)
    {
        var finalText = Convert.ToInt32(value).ToString();

        if (value >= 10000 && value < 100000)
        {
            finalText = $"{(Convert.ToInt32(value / 1000))}K";
        }
        else if (value >= 100000 && value < 1000000)
        {
            finalText = $"{Convert.ToInt32(value / 1000)}K";
        }
        else if (value >= 1000000)
        {
            finalText = $"{Convert.ToInt32(value / 1000000)}M";
        }

        return finalText;
    }
}
    