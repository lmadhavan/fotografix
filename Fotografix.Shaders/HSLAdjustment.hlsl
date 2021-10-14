#define D2D_INPUT_COUNT 1
#define D2D_INPUT0_SIMPLE

#include "d2d1effecthelpers.hlsli"

float vibrance;
float saturation;

D2D_PS_ENTRY(main)
{
    // input is assumed to be in HSL color space
    float4 color = D2DGetInput(0);
    color.y *= 1 + saturation + (vibrance * (1 - color.y));
    return color;
}