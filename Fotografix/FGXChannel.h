#pragma once

#include "FGXBitmap.h"

enum {
	AdjustBrightnessContrast,
	AdjustColorBalance,
	AdjustDesaturate,
	AdjustBlackWhite,
	AdjustGradientMap,
	AdjustInvert,
	AdjustThreshold,
	AdjustPosterize,
	AdjustLevels,
	//AdjustHueSaturation,
	FilterBlur = 20,
	FilterMotionBlur,
	FilterGaussianBlur,
	FilterAddNoise,
	FilterEmboss,
	FilterShear,
	FilterSolarize,
	FilterOffset,
	FilterPixelate,
	FilterNightVision,
	FilterHalftone,
	FilterSharpen,
	FilterUnsharpMask,
	FilterEdgesAll,
	FilterEdgesHorz,
	FilterEdgesVert,
	FilterEdgesDiag
};

enum {
	ModeNormal,
	ModeMultiply,
	ModeScreen,
	ModeOverlay,
	ModeHardLight,
	ModeLinearDodge,
	ModeLinearBurn,
	ModeDarken,
	ModeLighten,
	ModeDifference,
	ModeExclusion,
	ModePinLight,
	ModeHardMix
};

enum {
	ChannelAlpha = 1,
	ChannelRed = 2,
	ChannelGreen = 4,
	ChannelBlue = 8,
	ChannelMask = 16
};

const int ChannelAll = ChannelRed | ChannelGreen | ChannelBlue;

extern LPCTSTR channelNames[];
extern CString channelText[];
extern int channelMasks[];

#pragma pack(push, 1)

struct FGXColor {
	BYTE r, g, b, a;

	FGXColor() {
		r = g = b = a = 0;
	}

	FGXColor(BYTE r, BYTE g, BYTE b, BYTE a) : r(r), g(g), b(b), a(a) {
	}

	FGXColor(COLORREF color, BYTE a) : r(GetRValue(color)), g(GetGValue(color)), b(GetBValue(color)), a(a) {
	}

	operator COLORREF() {
		return RGB(r, g, b);
	}
};

struct FGXTriple {
	BYTE r, g, b;
};

#pragma pack(pop)

typedef FGXTriple FGXPalette[256];
typedef BYTE FGXTable[256];

class FGXChannel
{
public:
	FGXChannel() : data(NULL), position(0, 0, 0, 0) {
	}

	virtual ~FGXChannel() {
		Free();
	}

	/* ----- Pixel access ----- */

	BYTE &PixelAt(int i, int j) {
		return *(data + j * position.Width() + i + pixelOffset);
	}

	const BYTE &PixelAt(int i, int j) const {
		return *(data + j * position.Width() + i + pixelOffset);
		//return *(data + (j - position.top) * position.Width() + (i - position.left));
	}

	/* ----- Memory allocation ----- */

	// Allocates memory for the channel data
	void Allocate();

	// Frees memory allocated for the channel data (the data is lost)
	void Free();

	// Returns true if memory has been allocated for the channel data
	bool IsAllocated() const {
		return data != NULL;
	}

	/* ----- Position and dimensions ----- */

	// Returns the current position of the layer
	const CRect &GetPosition() const {
		return position;
	}

	// Sets the position of this channel (this must be called before Allocate)
	void SetPosition(const CRect &newPosition) {
		if (IsAllocated() == false) {
			position = newPosition;
			pixelOffset = - position.left - position.top * position.Width();
		}
	}

	// Moves the channel to a new position
	void MoveTo(long left, long top);

	// Expands the channel to a new bounding rectangle
	void ExpandTo(FGXChannel &undo, const CRect &newPosition, bool mask = false);

	// Calculates the channel bounds based on its contents
	CRect CalcBounds();

	// Resizes the channel to the specified dimensions
	void ResizeTo(FGXChannel &undo, const CRect &newPosition);

	// Flips the channel horizontally and/or vertically
	void Flip(bool horizontal, bool vertical);

	// Rotates the channel by a given angle
	void Rotate(FGXChannel &undo, int angle);

	/* ----- Render operations ----- */

	// Renders the channel onto a bitmap
	void Render(FGXBitmap &bitmap, int channel, const FGXChannel &alpha, BYTE opacity, const CRect &rect) const;

	// Renders the channel (with alpha channel) onto another channel
	void AlphaRender(FGXChannel &channel, const FGXChannel &channelAlpha, const FGXChannel &alpha, BYTE opacity, BYTE mode, const CRect &rect) const;

	// Renders the channel (with alpha channel and mask) onto another channel
	void AlphaRender(FGXChannel &channel, const FGXChannel &channelAlpha, const FGXChannel &alpha, const FGXChannel &mask, BYTE opacity, BYTE mode, const CRect &rect) const;

	// Computes the alpha channel resulting from combining this channel and the given channel
	void AlphaSquare(FGXChannel &alpha, BYTE opacity, const CRect &rect) const;

	// Computes the alpha channel resulting from combining this channel (with mask) and the given channel
	void AlphaSquare(FGXChannel &alpha, const FGXChannel &mask, BYTE opacity, const CRect &rect) const;

	// Renders the channel (with alpha channel) onto a mask channel
	void AlphaRenderMask(FGXChannel &channel, const FGXChannel &alpha, BYTE opacity, const CRect &rect) const;

	void Erase(FGXChannel &alpha, BYTE opacity, const CRect &rect) const;

	/* ----- Load and save operations ----- */

	// Loads the channel data from pixel data in memory
	void LoadFromMemory(const void *bits, int skip);

	// Saves the channel data as pixel data in memory
	void SaveToMemory(void *bits, int skip) const;

	/* ----- Copy and clear operations ----- */

	// Clones the channel data into another channel
	void Clone(FGXChannel &result) const;

	// Copies a portion of the channel data into another channel
	void CopyRect(FGXChannel &result, BYTE opacity, const CRect &rect) const;

	// Copies a portion of the channel data (with mask) into another channel
	void CopyRect(FGXChannel &result, const FGXChannel &mask, BYTE opacity, const CRect &rect) const;

	// Multiplies the contents of this channel with another channel
	void MultiplyChannel(FGXChannel &result, const FGXChannel &source) const;

	// Transfers the channel data into another channel
	void Transfer(FGXChannel &result);

	// Swaps the channel data with another channel
	void Swap(FGXChannel &other);

	// Clears the selection with the specified color
	void Clear(const FGXChannel &selection, BYTE color);

	// Fills the channel with a color
	void Fill(BYTE color);

	/* ----- Image adjustment operations ----- */

	void ApplyLUT(FGXChannel &undo, const CRect &rect, const FGXTable &table);

	void ApplyMatrix(FGXChannel &undo, const CRect &rect, short a11, short a12, short a13, short a21, short a22, short a23, short a31, short a32, short a33, short factor, short bias);

	void Adjust(FGXChannel &undo, const CRect &rect, int type, int a1, int a2, int a3);

	void Adjust(FGXChannel &undo, const FGXChannel &selection, const CRect &rect, int type, int a1, int a2, int a3);

protected:
	// Relative position of this channel in the image (same as position of parent layer)
	CRect position;

	// Channel data
	BYTE *data;
	int pixelOffset;

	friend class FGXLayer;
	friend class FGXAccessor;
	friend class FGXAccessorC;
	friend class FGXVAccessor;
	friend class FGXVAccessorC;
	friend class CFotografixDoc;
};

class FGXAccessor {
public:
	FGXAccessor(FGXChannel &channel, const CRect &rect) {
		p = channel.data + (rect.top - channel.position.top) * channel.position.Width() + (rect.left - channel.position.left);
		delta = channel.position.Width() - rect.Width();
	}

	BYTE &operator *() {
		return *p;
	}

	void operator ++(int) {
		p++;
	}

	void operator --(int) {
		p--;
	}

	void operator +=(int i) {
		p += i;
	}

	void operator -=(int i) {
		p -= i;
	}

	void next() {
		p += delta;
	}

private:
	BYTE *p;
	int delta;
};

class FGXAccessorC {
public:
	FGXAccessorC(const FGXChannel &channel, const CRect &rect) {
		p = channel.data + (rect.top - channel.position.top) * channel.position.Width() + (rect.left - channel.position.left);
		delta = channel.position.Width() - rect.Width();
	}

	const BYTE &operator *() {
		return *p;
	}

	void operator ++(int) {
		p++;
	}

	void operator --(int) {
		p--;
	}

	void operator +=(int i) {
		p += i;
	}

	void operator -=(int i) {
		p -= i;
	}

	void next() {
		p += delta;
	}

private:
	const BYTE *p;
	int delta;
};

class FGXVAccessor {
public:
	FGXVAccessor(FGXChannel &channel, const CRect &rect) {
		p = channel.data + (rect.top - channel.position.top) * channel.position.Width() + (rect.left - channel.position.left);
		plus = channel.position.Width();
		delta = - channel.position.Width() * rect.Height() + 1;
	}

	BYTE &operator *() {
		return *p;
	}

	void operator ++(int) {
		p += plus;
	}

	void operator --(int) {
		p -= plus;
	}

	void operator +=(int i) {
		p += i * plus;
	}

	void operator -=(int i) {
		p -= i * plus;
	}

	void next() {
		p += delta;
	}

private:
	BYTE *p;
	int plus;
	int delta;
};

class FGXVAccessorC {
public:
	FGXVAccessorC(FGXChannel &channel, const CRect &rect) {
		p = channel.data + (rect.top - channel.position.top) * channel.position.Width() + (rect.left - channel.position.left);
		plus = channel.position.Width();
		delta = - channel.position.Width() * rect.Height() + 1;
	}

	const BYTE &operator *() {
		return *p;
	}

	void operator ++(int) {
		p += plus;
	}

	void operator --(int) {
		p -= plus;
	}

	void operator +=(int i) {
		p += i * plus;
	}

	void operator -=(int i) {
		p -= i * plus;
	}

	void next() {
		p += delta;
	}

private:
	const BYTE *p;
	int plus;
	int delta;
};
