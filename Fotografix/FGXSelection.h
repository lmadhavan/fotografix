#pragma once

#include "FGXChannel.h"

class FGXLayer;

enum SelectType {
	SelectNormal,
	SelectAdd,
	SelectSubtract
};

class FGXSelection : public FGXChannel
{
public:
	// Selects the inverse of the current selection
	void SelectInverse(FGXChannel &undo, const CRect &bounds);

	// Selects a rectangular region
	void SelectRectangle(FGXChannel &undo, const CRect &rect, SelectType type = SelectNormal);

	// Selects an elliptical region
	void SelectEllipse(FGXChannel &undo, const CRect &rect, SelectType type = SelectNormal);

	// Selects contiguous regions of similar color
	void SelectWand(FGXChannel &undo, const FGXLayer &layer, CPoint pt, int tolerance, SelectType type = SelectNormal);

	// Selects all regions of similar color
	void SelectRange(FGXChannel &undo, const FGXLayer &layer, CPoint pt, int tolerance, SelectType type = SelectNormal);

	// Prepares the selection outline for animation
	void InitAnimate(FGXBitmap &bitmap, const CRect &rect) const;

	// Animates the selection on a previously prepared bitmap
	void Animate(FGXBitmap &bitmap, const CRect &rect) const;

	// Loads a layer's transparency as the selection
	void LoadLayer(FGXChannel &undo, const FGXLayer &layer);

	// Returns true if the specified pixel is a border pixel of the selection
	bool IsBorderPixel(int i, int j) const {
		return PixelAt(i-1, j) == 0 || PixelAt(i+1, j) == 0 || PixelAt(i, j-1) == 0 || PixelAt(i, j+1) == 0;
	}
};
