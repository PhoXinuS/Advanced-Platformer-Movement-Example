public static class Smoothers
{
    public static float SmoothStartStop(float progress, int mathPower = 3, float startToStopBlend = 0.5f)
    {
        float smoothStartBlend = SmoothStart(progress, mathPower) * startToStopBlend;
        float smoothStopBlend = SmoothStop(progress, mathPower) * (1 - startToStopBlend);
        return (smoothStartBlend + smoothStopBlend);
    }

    public static float SmoothStart(float progress, int mathPower = 3)
    {
        float smoother = progress;

        for (int i = 1; i < mathPower; i++)
        {
            smoother *= progress;
        }

        return smoother;
    }

    public static float SmoothStop(float progress, int mathPower = 3)
    {
        float smoother = (1 - progress);

        for (int i = 1; i < mathPower; i++)
        {
            smoother *= (1 - progress);
        }

        return 1 - smoother;
    }
}
