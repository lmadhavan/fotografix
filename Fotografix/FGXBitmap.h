#pragma once

class FGXBitmap
{
public:
	FGXBitmap();

	~FGXBitmap() {
		Free();
	}

	/* ----- Pixel access ----- */

	DWORD &PixelAt(int i, int j) {
		return *(data + j * bi.bmiHeader.biWidth + i);
	}

	const DWORD &PixelAt(int i, int j) const {
		return *(data + j * bi.bmiHeader.biWidth + i);
	}

	/* ----- Memory allocation ----- */

	// Allocates memory for the bitmap
	void Allocate(long width, long height);

	// Frees allocated memory
	void Free();

	// Initializes the bitmap with a transparency grid
	void InitializeGrid(const CRect &rect);

	// Draws the bitmap onto a device
	void Draw(CDC *pDC, int x, int y, int width, int height) const;

private:
	// Structure describing the DIB
	BITMAPINFO bi;

	// DIB pixel data
	DWORD *data;

	friend class FGXChannel;
	friend class FGXSelection;
	friend class FGXAccessorB;
};

class FGXAccessorB {
public:
	FGXAccessorB(FGXBitmap &bitmap, int channel, const CRect &rect) {
		p = bitmap.data + rect.top * bitmap.bi.bmiHeader.biWidth + rect.left;
		this->channel = channel;
		delta = bitmap.bi.bmiHeader.biWidth - rect.Width();
	}

	BYTE &operator *() {
		return *((BYTE *)p + channel);
	}

	void operator ++(int) {
		p++;
	}

	void next() {
		p += delta;
	}

private:
	DWORD *p;
	int channel;
	int delta;
};
