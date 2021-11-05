#define D2D_INPUT_COUNT 2
#define D2D_INPUT0_SIMPLE // source image
#define D2D_INPUT1_SIMPLE // blurred source image

#include "d2d1effecthelpers.hlsli"

float amount;
float threshold;

D2D_PS_ENTRY(main)
{
    float4 source = D2DGetInput(0);
    float4 blurred = D2DGetInput(1);
    
    float4 delta = source - blurred;
    float4 gate = step(float4(threshold, threshold, threshold, 1), delta);
    return source + dot(amount * delta, gate);
}
