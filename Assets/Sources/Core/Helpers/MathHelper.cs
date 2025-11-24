using System;

public static class MathHelper
{
    public static void AddPercentage(ref float value, int percentage)
    {
        value = GetWithAddedPercentage(value, percentage);
    }

    public static float GetWithAddedPercentage(float value, int percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentOutOfRangeException(nameof(percentage));
        
        return value + GetPercentage(value, percentage);
    }

    public static float GetPercentage(float value, int percentage)
    {
        return percentage / 100f * value;
    }
}