#pragma once

#include <cmath>
#include <utility>

#define ForeachPixel(c) for (BYTE *p = (c).data, *_p = (c).data + position.Width() * position.Height(); p < _p; p++)
#define ForeachPixel2(c1, c2) for (BYTE *p = (c1).data, *_p = (c1).data + position.Width() * position.Height(), *o = (c2).data; p < _p; p++, o++)
//#define ForeachRectPixel for (int j = common.top; j < common.bottom; j++) for (int i = common.left; i < common.right; i++)
#define ForeachPixelRGB for (BYTE *r = channels[1].data, *g = channels[2].data, *b = channels[3].data, *_r = channels[1].data + position.Width() * position.Height(); r < _r; r++, g++, b++)

#define BeginLoopRect(rect, perPixel) \
	for (int j = rect.top; j < rect.bottom; j++) {\
		for (int i = rect.left; i < rect.right; i++, perPixel)

#define EndLoopRect(perLine) \
		perLine;\
	}

using std::swap;

template <class T>
__forceinline T clamp(T value, T min, T max) {
	return value < min ? min : (value > max ? max : value);
}

__forceinline BYTE FGXMultiply(BYTE value1, BYTE value2) { __asm {
	//return (WORD(value1) * WORD(value2)) / 255;
	mov al, value1
	mul value2
	mov al, ah
} }

__forceinline BYTE FGXDivide(BYTE value1, BYTE value2) { __asm {
	//return (WORD(value1) * 255) / WORD(value2);
	mov al, value1
	mov cl, 255
	mul cl
	div value2
} }

__forceinline BYTE FGXScreen(BYTE value1, BYTE value2) {
	//return 255 - WORD(255 - value1) * WORD(255 - value2) / 255;
__asm {
	mov al, 255
	sub al, value1
	mov cl, 255
	sub cl, value2
	mul cl
	mov cl, 255
	div cl
	mov ah, 255
	sub ah, al
	mov al, ah
}
}

__forceinline BYTE FGXScreen2(BYTE value1, BYTE value2) {
	//return 255 - 2 * WORD(255 - value1) * WORD(255 - value2) / 255;
__asm {
	mov al, 255
	sub al, value1
	mov cl, 255
	sub cl, value2
	mul cl
	mov cl, 255
	div cl
	shl al, 1
	mov ah, 255
	sub ah, al
	mov al, ah
}
}

__forceinline BYTE FGXOverlay(BYTE value1, BYTE value2) {
	return (value2 & 0x80) == 0 ? (FGXMultiply(value1, value2) << 1) : FGXScreen2(value1, value2);
}

__forceinline BYTE FGXBlend(BYTE src, BYTE srcAlpha, BYTE dest) {
	return (WORD(src) * WORD(srcAlpha) + WORD(dest) * WORD(255 - srcAlpha)) / 255;
}

__forceinline BYTE FGXAlphaBlend(BYTE src, BYTE srcAlpha, BYTE dest, BYTE destAlpha) {
	//register BYTE a = FGXScreen(srcAlpha, destAlpha);
	//return a == 0 ? 0 : (WORD(src) * WORD(srcAlpha) + WORD(dest) * (WORD(destAlpha) * WORD(255 - srcAlpha) / 255)) / a;
__asm {
	mov al, 255
	sub al, destAlpha
	mov ch, 255
	sub ch, srcAlpha
	mul ch
	mov cl, 255
	div cl
	mov dl, al
	sub cl, al
	jz end
	mov al, ch
	sub al, dl
	mul dest
	mov si, ax
	mov al, srcAlpha
	mul src
	add ax, si
	div cl
end:
}
}

__forceinline BYTE FGXInvert(BYTE value) {
	return 255 - value;
//__asm {
//	mov al, 255
//	sub al, value
//}
}

__forceinline BYTE FGXBrightnessPlus(BYTE value, BYTE d) {
__asm {
	mov al, value
	add al, d
	jnc end
	mov al, 255
end:
}
}

__forceinline BYTE FGXBrightnessMinus(BYTE value, BYTE d) {
__asm {
	mov al, value
	sub al, d
	jnc end
	xor al, al
end:
}
}

__forceinline BYTE FGXContrastPlus(BYTE value, BYTE d) {
	short r = (short(-128) * d + short(value) * (255 + d)) / 255;
	return clamp<short>(r, 0, 255);
}

__forceinline BYTE FGXContrastMinus(BYTE value, BYTE d) {
	return (WORD(128) * d + WORD(value) * (255 - d)) / 255;
}

__forceinline BYTE FGXDifference(BYTE value1, BYTE value2) {
	return value1 < value2 ? value2 - value1 : value1 - value2;
}

__forceinline BYTE FGXExclusion(BYTE value1, BYTE value2) {
	return value1 ^ value2;
}

__forceinline BYTE FGXDarken(BYTE value1, BYTE value2) {
	return value1 < value2 ? value1 : value2;
}

__forceinline BYTE FGXLighten(BYTE value1, BYTE value2) {
	return value1 < value2 ? value2 : value1;
}

__forceinline BYTE FGXPinLight(BYTE value1, BYTE value2) {
	if ((value1 & 0x80) == 0)
		return value1 < value2 ? value1 : value2;
	else
		return value1 < value2 ? value2 : value1;
}

__forceinline BYTE FGXHardMix(BYTE value1, BYTE value2) {
	return (WORD(value1) + value2 > 255) ? 255 : 0;
}

__forceinline BYTE FGXLevels(BYTE value, BYTE s, BYTE h) {
__asm {
	mov al, value
	sub al, s
	jnc next
	xor al, al
next:
	mov cl, 255
	mul cl
	xor dx, dx
	mov cl, h
	sub cl, s
	xor ch, ch
	div cx
	test ah, ah
	jz end
	mov al, 255
end:
}
}

__forceinline BYTE FGXGamma(BYTE value, short gamma) {
	return clamp<short>(pow(value, gamma / 100.0f), 0, 255);
}

__forceinline BYTE FGXPosterize(BYTE value, BYTE level) {
	return value & ((255 >> level) << level);
}

__forceinline BYTE FGXGrayscale(BYTE r, BYTE g, BYTE b) {
	return ((WORD)r + (WORD)g + (WORD)b) / 3;
}

__forceinline BYTE FGXGrayscaleEx(BYTE r, BYTE g, BYTE b, short rp, short gp, short bp) {
	register short p = rp + gp + bp;
	return p == 0 ? 0 : (short(r) * rp + short(g) * gp + short(b) * bp) / p;
}

__forceinline BYTE FGXSaturation(BYTE r, BYTE g, BYTE b) {
	return max(max(r, g), b) - min(min(r, g), b);
}

__forceinline BYTE FGXEmboss(BYTE c1, BYTE c2) {
__asm {
	mov ax, 128
	xor ch, ch
	mov cl, c1
	add ax, cx
	mov cl, c2
	sub ax, cx
}
}

__forceinline int bswap(int i) {
	__asm {
		mov eax, i
		bswap eax
	}
}

__forceinline unsigned short bswap(unsigned short i) {
	__asm {
		mov bx, i
		mov ah, bl
		mov al, bh
	}
}

__forceinline float noise(int x, int y) {
	int n = x + y * 57;
	n = (n << 13) ^ n;
	return 1.0 - ( (n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0;
}

__forceinline float smooth_noise(int x, int y) {
	return (noise(x-1, y-1) + noise(x+1, y-1) + noise(x-1, y+1) + noise(x+1, y+1)) / 16
		 + (noise(x-1, y) + noise(x+1, y) + noise(x, y-1) + noise(x, y+1)) / 8
		 + noise(x, y) / 4;
}

__forceinline float interpolate(float a, float b, float x) {
	return a * x + b * (1.0 - x);
}

__forceinline float interpolated_noise(float x, float y) {
	int ix = x,
		iy = y;
	
	float fx = x - ix,
		  fy = y - iy;

	float v1 = smooth_noise(ix, iy),
		  v2 = smooth_noise(ix+1, iy),
		  v3 = smooth_noise(ix, iy+1),
		  v4 = smooth_noise(ix+1, iy+1);

	float i1 = interpolate(v1, v2, fx),
		  i2 = interpolate(v3, v4, fx);

	return interpolate(i1, i2, fy);
}

__forceinline BYTE FGXPerlin(int x, int y, float p, int n) {
	float total = 0.0;

	for (int i = 0; i < n; i++) {
		float freq = 1 << i,
			  amp = pow(p, i);

		total += interpolated_noise(x * freq, y * freq) * amp;
	}

	return total * 255.0;
}

__forceinline BYTE interpolate_color(float x1, BYTE c1, float x2, BYTE c2, float x) {
	return (x - x1) * c2 / (x2 - x1) + (x2 - x) * c1 / (x2 - x1);
}
