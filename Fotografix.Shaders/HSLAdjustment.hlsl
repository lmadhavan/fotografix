#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_SIMPLE

#include "d2d1effecthelpers.hlsli"

float vibrance;     // global vibrance adjustment
float saturation;   // global saturation adjustment
float3 hsl[6];      // HSL adjustment for each hue band

float midtoneBandpass(float luminance)
{
    float delta = 0.5 - luminance;
    return 1 - 4 * delta * delta;
}

float chromaticity(float4 color)
{
    /*
     * Hue values can be unreliable in areas of low chromaticity
     * (colors with low saturation and colors that are very dark/bright).
     * 
     * Luminance adjustments are scaled by a chromaticity factor
     * so that we don't incorrectly adjust achromatic colors.
     */
    return color.y * midtoneBandpass(color.z);
}

D2D_PS_ENTRY(main)
{
    // input is assumed to be in HSL color space
    float4 color = D2DGetInput(0);

    // interpolate neighboring hue bands to obtain adjustment for input hue
    float hueBand = color.x * 6;
    uint hueBandL = (uint)hueBand % 6;
    uint hueBandR = (uint)(hueBand + 1) % 6;
    float3 adjustment = lerp(hsl[hueBandL], hsl[hueBandR], frac(hueBand));

    color.z *= 1 + adjustment.z * chromaticity(color);
    color.x = frac(color.x + adjustment.x / 2 + 1);
    color.y *= 1 + saturation + (vibrance * (1 - color.y)) + adjustment.y;

    return color;
}
