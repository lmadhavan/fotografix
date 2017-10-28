#include "StdAfx.h"
#include "FGXImage.h"

#include "FGX.h"

#include <cmath>

__int64 gpt() {
	FILETIME a,b,c,d;
	GetProcessTimes(GetCurrentProcess(), &a,&b,&c,&d);
	return *(__int64*)&d;
}

void FGXImage::Render(FGXLayer &layer, const CRect &rect, bool fastCopy, int channelMask) const {
	CWaitCursor wait;

	CRect r = rect & CRect(0, 0, width, height);

	if (channelMask == ChannelMask) channelMask = ChannelAll;

	for (int i = 0; i < layers.size(); i++)
		if (layers[i]->IsVisible())
			if (fastCopy == true) {
				// First layer can be directly copied onto target layer, provided it uses Normal blending mode
				if (layers[i]->GetType() != LayerAdjust) {
					fastCopy = false;

					if (layers[i]->GetMode() == LayerNormal)
						layers[i]->CopyRect(layer, r, channelMask | ChannelAlpha);
					else
						layers[i]->AlphaRender(layer, r, channelMask);
				}
			} else
				layers[i]->AlphaRender(layer, r, channelMask);
}

void FGXImage::Render(FGXBitmap &bitmap, const CRect &rect, int channelMask) const {
	CRect r = rect & CRect(0, 0, width, height);

	FGXLayer layer(r);
	Render(layer, rect, true, channelMask);

	bitmap.InitializeGrid(r);
	//layer.ConvertToGray(channelMask);
	layer.Render(bitmap, r);
}

void FGXImage::Crop(FGXImage &undo, const CRect &rect) {
	undo.Reset();

	undo.width = width;
	undo.height = height;
	undo.res = res;
	undo.unit = unit;

	for (int i = 0; i < layers.size(); i++) {
		CRect r = rect & layers[i]->GetPosition();
		layers[i]->ExpandTo(*undo.AddLayer(), r);
		layers[i]->MoveTo(r.left - rect.left, r.top - rect.top);
	}

	width = rect.Width();
	height = rect.Height();
}

void FGXImage::RevealAll() {
	CRect rect(0, 0, width, height);

	for (int i = 0; i < layers.size(); i++)
		rect |= layers[i]->GetPosition();

	for (int i = 0; i < layers.size(); i++) {
		// If this is an adjustment layer, expand the mask
		if (layers[i]->GetType() == LayerAdjust)
			layers[i]->EnsureRect(rect);
		// Else just move the layer to the new position
		else {
			const CRect &r = layers[i]->GetPosition();
			layers[i]->MoveTo(r.left - rect.left, r.top - rect.top);
		}
	}

	width = rect.Width();
	height = rect.Height();
}

void FGXImage::ResizeCanvas(int newWidth, int newHeight, int anchor) {
	int dx, dy;

	switch (anchor) {
	case 0:
	case 3:
	case 6:
		dx = 0;
		break;

	case 1:
	case 4:
	case 7:
		dx = (newWidth - width) / 2;
		break;

	case 2:
	case 5:
	case 8:
		dx = newWidth - width;
		break;
	}

	switch (anchor) {
	case 0:
	case 1:
	case 2:
		dy = 0;
		break;

	case 3:
	case 4:
	case 5:
		dy = (newHeight - height) / 2;
		break;

	case 6:
	case 7:
	case 8:
		dy = newHeight - height;
		break;
	}

	for (int i = 0; i < layers.size(); i++) {
		// If this is an adjustment layer, expand the mask
		if (layers[i]->GetType() == LayerAdjust)
			layers[i]->EnsureRect(CRect(0, 0, newWidth, newHeight));
		// Else just move the layer to the new position
		else {
			const CRect &r = layers[i]->GetPosition();
			layers[i]->MoveTo(r.left + dx, r.top + dy);
		}
	}

	width = newWidth;
	height = newHeight;
}

void FGXImage::ResizeImage(FGXImage &undo, int newWidth, int newHeight) {
	undo.width = width;
	undo.height = height;
	undo.res = res;
	undo.unit = unit;

	CPoint oldCenter(width / 2, height / 2),
		   newCenter(newWidth / 2, newHeight / 2);

	for (int i = 0; i < layers.size(); i++) {
		CRect position = layers[i]->GetPosition();

		CPoint center = position.CenterPoint() - oldCenter;
		center.x = center.x * newWidth / width;
		center.y = center.y * newHeight / height;
		center += newCenter;

		int w = position.Width() * newWidth / width,
			h = position.Height() * newHeight / height;
		position = CRect(CPoint(center.x - w / 2, center.y - h / 2), CSize(w, h));

		// If this is a text layer, change the font size
		if (layers[i]->GetType() == LayerText) {
			layers[i]->Clone(*undo.AddLayer());
			layers[i]->MoveTo(position.left, position.top);
			layers[i]->a1 = layers[i]->a1 * newHeight / height;
			layers[i]->RenderText();
		} else
		// Else just resample the pixels
			layers[i]->ResizeTo(*undo.AddLayer(), position);
	}

	width = newWidth;
	height = newHeight;
}

void FGXImage::Flip(bool horizontal, bool vertical) {
	for (int i = 0; i < layers.size(); i++) {
		CRect position = layers[i]->GetPosition();

		if (horizontal == true)
			position.left = width - position.right;

		if (vertical == true)
			position.top = height - position.bottom;

		layers[i]->MoveTo(position.left, position.top);
		layers[i]->Flip(horizontal, vertical);
	}
}

void FGXImage::Rotate(FGXImage &undo, int angle) {
	undo.width = width;
	undo.height = height;
	undo.res = res;
	undo.unit = unit;

	float a = angle * 3.14159 / 180;
	CPoint center(width / 2, height / 2);

	for (int i = 0; i < layers.size(); i++) {
		CPoint oldCenter = layers[i]->GetPosition().CenterPoint() - center;
		CPoint newCenter(center.x + oldCenter.x * cos(a) - oldCenter.y * sin(a), center.y + oldCenter.x * sin(a) + oldCenter.y * cos(a));

		layers[i]->Rotate(*undo.AddLayer(), angle);

		CRect rect = layers[i]->GetPosition();
		rect.OffsetRect(newCenter - rect.CenterPoint());
		layers[i]->MoveTo(rect.left, rect.top);
	}

	Compact();

	width = height = 0;
	RevealAll();
}

void FGXImage::Clone(FGXImage &image) const {
	image.Reset();

	image.width = width;
	image.height = height;
	image.res = res;
	image.unit = unit;

	for (int i = 0; i < layers.size(); i++)
		layers[i]->Clone(*image.AddLayer());
}

void FGXImage::Swap(FGXImage &other) {
	swap(other.width, width);
	swap(other.height, height);
	swap(other.res, res);
	swap(other.unit, unit);

	other.layers.swap(layers);
}

void FGXImage::Reset() {
	// Free the layers within this image
	for (int i = 0; i < layers.size(); i++)
		delete layers[i];
}

void FGXImage::Flatten(FGXImage &undo) {
	CWaitCursor wait;

	Swap(undo);

	width = undo.width;
	height = undo.height;
	res = undo.res;
	unit = undo.unit;

	FGXLayer *layer = new FGXLayer(CRect(0, 0, width, height));
	undo.Render(*layer, layer->GetPosition(), true);
	layer->SetName(TEXT("Flattened Image"));
	layer->Compact();
	AddLayer(layer);
}
