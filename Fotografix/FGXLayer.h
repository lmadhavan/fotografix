#pragma once

#include "FGXChannel.h"
#include "FGXSelection.h"
#include "Language.h"

class FGXImage;

enum {
	LayerNormal,
	LayerBrush,
	LayerAdjust,
	LayerText
};

class FGXLayer
{
public:
	FGXLayer(int type = LayerNormal) : image(NULL), opacity(255), mode(ModeNormal), visible(true), mask(false) {
		SetType(type);
		_tcscpy(name, GetLangItemEarly(TEXT("NewLayer")));
		SetPosition(CRect(0, 0, 0, 0));
		a = a1 = a2 = a3 = 0;
	}

	FGXLayer(const CRect &position, int type = LayerNormal) : image(NULL), opacity(255), mode(ModeNormal), visible(true), mask(false) {
		SetType(type);
		_tcscpy(name, GetLangItemEarly(TEXT("NewLayer")));
		SetPosition(position);
		a = a1 = a2 = a3 = 0;
	}

	/* ----- Pixel access ----- */

	FGXColor PixelAt(int i, int j) const {
		return FGXColor(channels[1].PixelAt(i, j),
						channels[2].PixelAt(i, j),
						channels[3].PixelAt(i, j),
						255);
	}

	/* ----- Position and dimensions ----- */

	// Returns the current position of the layer
	const CRect &GetPosition() const {
		return position;
	}

	// Sets a new position for the layer (this deletes the layer contents)
	void SetPosition(const CRect &newPosition);

	// Moves the layer to a new position
	void MoveTo(long left, long top);

	// Expands the layer to a new bounding rectangle
	void ExpandTo(FGXLayer &undo, const CRect &newPosition);

	// Compacts the layer by contracting it to the correct dimensions
	void Compact() {
		CRect rect = channels[0].CalcBounds();
		if (rect != position) ExpandTo(FGXLayer(), rect);
	}

	// Ensures that the layer includes a given rectangle, expanding it if necessary
	void EnsureRect(const CRect &rect) {
		if ((position | rect) != position)
			ExpandTo(FGXLayer(), position | rect);
	}

	// Resizes the layer to the specified dimensions
	void ResizeTo(FGXLayer &undo, const CRect &newPosition);

	// Flips the layer horizontally and/or vertically
	void Flip(bool horizontal, bool vertical);

	// Rotates the layer by a given angle
	void Rotate(FGXLayer &undo, int angle);

	/* ----- Layer properties ----- */

	// Returns the name of the layer
	const TCHAR *GetName() const {
		return name;
	}

	// Sets a new name for the layer
	void SetName(const TCHAR *newName) {
		_tcsncpy(name, newName, 255);
		name[255] = 0;
	}

	// Returns the opacity of the layer
	BYTE GetOpacity() const {
		return opacity;
	}

	// Sets a new opacity for the layer
	void SetOpacity(BYTE opacity) {
		this->opacity = opacity;
	}

	// Returns the mode of the layer
	BYTE GetMode() const {
		return mode;
	}

	// Sets a new mode for the layer
	void SetMode(BYTE mode) {
		this->mode = mode;
	}

	// Returns the visibility of the layer
	bool IsVisible() const {
		return visible;
	}

	// Sets a new visibility for the layer
	void SetVisible(bool visible) {
		this->visible = visible;
	}

	// Gets the type of this layer
	int GetType() const {
		return type;
	}

	// Returns true if the layer has a mask
	bool HasMask() const {
		return numChannels == 5;
	}

	void AddMask() {
		if (numChannels == 4) {
			numChannels++;
			mask = true;
			
			channels[4].SetPosition(position);
			channels[4].Allocate();
			channels[4].Fill(255);
		}
	}

	void DelMask(bool apply) {
		if (numChannels == 5) {
			if (apply == true)
				channels[4].MultiplyChannel(channels[0], channels[0]);

			channels[4].Free();
			numChannels--;
			mask = false;
		}
	}

	// Returns true if the layer mask is enabled
	bool IsMaskEnabled() const {
		return mask;
	}

	void EnableMask(bool enable) {
		mask = enable;
	}

	void Rasterize() {
		type = LayerNormal;
	}

	/* ----- Render operations ----- */

	// Renders the layer onto a bitmap
	void Render(FGXBitmap &bitmap, const CRect &rect, int channelMask = ChannelRed | ChannelGreen | ChannelBlue) const;

	// Renders the layer (with alpha channel) onto another layer
	void AlphaRender(FGXLayer &layer, const CRect &rect, int channelMask = ChannelRed | ChannelGreen | ChannelBlue) const;

	// Selectively erases another layer's contents based on this layer's alpha channel
	void Erase(FGXLayer &layer, const CRect &rect) const;

	void ConvertToGray(int channelMask);

	void RenderText();

	/* ----- Load and save operations ----- */

	// Loads the layer data from pixel data in memory
	void LoadFromMemory(const void *data, int numChannels);

	// Saves the layer data as pixel data in memory
	void SaveToMemory(void *data, int numChannels) const;

	/* ----- Copy and clear operations ----- */

	// Clones the contents of this layer into another layer
	void Clone(FGXLayer &result) const;

	// Copies this layer's properties to another layer
	void CopyProps(FGXLayer &result) const;

	// Copies a rectangular portion of the layer into another layer
	void CopyRect(FGXLayer &result, const CRect &rect, int channelMask = ChannelRed | ChannelGreen | ChannelBlue | ChannelAlpha) const;

	// Copies a selected region of the layer into another layer
	void CopySelection(FGXLayer &result, const FGXSelection &selection, int channelMask = ChannelRed | ChannelGreen | ChannelBlue) const;

	// Clears the selection with the specified color
	void Clear(FGXLayer &undo, const FGXChannel &selection, FGXColor color, int channelMask = ChannelRed | ChannelGreen | ChannelBlue);

	// Fills the layer with a single color
	void Fill(FGXColor color, int channelMask = ChannelAll);

	// Transfers the contents of this layer into another layer
	void Transfer(FGXLayer &result);

	// Swaps the contents of this layer with another layer
	void Swap(FGXLayer &other);

	// Copies a channel into the mask for this layer
	void SetMask(const FGXChannel &mask, const CRect &rect);

	/* ----- Image adjustment operations ----- */

	void Adjust(FGXLayer &undo, const CRect &rect, int type, int a1, int a2, int a3, int channelMask = ChannelRed | ChannelGreen | ChannelBlue);

	void Adjust(FGXLayer &undo, int type, int a1, int a2, int a3, int channelMask = ChannelRed | ChannelGreen | ChannelBlue) {
		Adjust(undo, position, type, a1, a2, a3, channelMask);
	}

	void Adjust(FGXLayer &undo, const FGXChannel &selection, const CRect &rect, int type, int a1, int a2, int a3, int channelMask = ChannelRed | ChannelGreen | ChannelBlue);

	void Adjust(FGXLayer &undo, const FGXChannel &selection, int type, int a1, int a2, int a3, int channelMask = ChannelRed | ChannelGreen | ChannelBlue) {
		Adjust(undo, selection, selection.GetPosition(), type, a1, a2, a3, channelMask);
	}

	/* ----- Brush operations ----- */

	void ConvertToBrush();

private:
	void SetType(int type);

private:
	// Relative position of this layer in the image
	CRect position;

	// Channels contained in this layer
	FGXChannel channels[5];

	// Number of channels used
	int numChannels;

	// Opacity of this layer
	BYTE opacity;

	// Blending mode of this layer
	BYTE mode;

	// User-supplied name for this layer
	TCHAR name[256];

	// Specifies if this layer is visible
	bool visible;

	// Specifies if the layer mask is enabled
	bool mask;

	// Type of layer
	int type;

public:
	// Pointer to image containing this layer
	FGXImage *image;

	// Additional parameters for certain layer types
	int a, a1, a2, a3;
	CString a4, a5;

	friend class FGXSelection;
	friend class FGXGradient;
	friend class CFotografixDoc;
};
