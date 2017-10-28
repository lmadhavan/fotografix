#pragma once

#include "FGXLayer.h"

#include <vector>
using std::vector;

class FGXImage
{
public:
	FGXImage() {
		width = height = -1;
		res = 72;
		unit = 0;
	}

	~FGXImage() {
		Reset();
	}

	long GetWidth() const {
		return width;
	}

	long GetHeight() const {
		return height;
	}

	unsigned short GetResolution() const {
		return res;
	}

	void SetResolution(unsigned short newRes) {
		res = newRes;
	}

	unsigned char GetUnit() const {
		return unit;
	}

	void SetUnit(unsigned char newUnit) {
		unit = newUnit;
	}

	// Initializes the image dimensions
	void Initialize(long initWidth, long initHeight) {
		width = initWidth;
		height = initHeight;
	}

	// Adds a new layer to the image
	int AddLayer(FGXLayer *layer) {
		layer->image = this;
		layers.push_back(layer);
		return layers.size() - 1;
	}

	// Adds a new layer to the image
	FGXLayer *AddLayer() {
		FGXLayer *layer = new FGXLayer;
		layer->image = this;
		layers.push_back(layer);
		return layer;
	}

	// Inserts a new layer into the image
	void InsertLayer(FGXLayer *layer, int position) {
		layer->image = this;
		layers.insert(layers.begin() + position, layer);
	}

	// Inserts a new layer into the image
	FGXLayer *InsertLayer(int position) {
		FGXLayer *layer = new FGXLayer;
		layer->image = this;
		layers.insert(layers.begin() + position, layer);
		return layer;
	}

	// Deletes a layer from the image
	void DeleteLayer(int position) {
		delete layers[position];
		layers.erase(layers.begin() + position);
	}

	// Returns a pointer to a layer in the image
	FGXLayer *GetLayer(int i) {
		return layers[i];
	}

	// Returns a pointer to a layer in the image
	const FGXLayer *GetLayer(int i) const {
		return layers[i];
	}

	// Moves a layer to a different position in the layer stack
	void MoveLayer(int from, int to) {
		FGXLayer *layer = layers[from];
		layers.erase(layers.begin() + from);
		layers.insert(layers.begin() + to, layer);
	}

	// Returns the number of layers in the image
	int GetLayerCount() const {
		return layers.size();
	}

	// Renders the image onto a layer
	void Render(FGXLayer &layer, const CRect &rect, bool fastCopy, int channelMask = ChannelRed | ChannelGreen | ChannelBlue) const;

	// Renders the image onto a bitmap
	void Render(FGXBitmap &bitmap, const CRect &rect, int channelMask = ChannelRed | ChannelGreen | ChannelBlue) const;

	// Crops the image to a specified rectangle
	void Crop(FGXImage &undo, const CRect &rect);

	// Reveals the entire image (including out-of-bounds layers)
	void RevealAll();

	// Resizes the canvas
	void ResizeCanvas(int newWidth, int newHeight, int anchor);

	// Resizes the image
	void ResizeImage(FGXImage &undo, int newWidth, int newHeight);

	// Flips the image horizontally and/or vertically
	void Flip(bool horizontal, bool vertical);

	// Rotates the image by a given angle
	void Rotate(FGXImage &undo, int angle);

	// Clones this image into another image
	void Clone(FGXImage &image) const;

	// Swaps the contents of this image with another image
	void Swap(FGXImage &other);

	// Deletes all the layers in this image
	void Reset();

	// Compacts the layers in this image
	void Compact() {
		for (int i = 0; i < layers.size(); i++)
			layers[i]->Compact();
	}

	// Flattens the layers in this image
	void Flatten(FGXImage &undo);

private:
	// Dimensions of the image
	long width, height;

	// Resolution of the image
	unsigned short res;

	// Dimension units (0 = pixels; 1 = inches; 2 = centimetres)
	unsigned char unit;

	// Layers contained in this image
	vector<FGXLayer *> layers;
};
