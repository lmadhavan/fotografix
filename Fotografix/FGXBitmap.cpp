#include "StdAfx.h"
#include "FGXBitmap.h"
#include "FGXChannel.h"

FGXBitmap::FGXBitmap() : data(NULL)
{
	// Initialize bitmap header
	ZeroMemory(&bi, sizeof(bi));
	bi.bmiHeader.biSize = sizeof(bi.bmiHeader);
	bi.bmiHeader.biPlanes = 1;
	bi.bmiHeader.biBitCount = 32;
	bi.bmiHeader.biCompression = BI_RGB;
}

void FGXBitmap::Allocate(long width, long height) {
	if (data != NULL) Free();

	bi.bmiHeader.biWidth = width;
	bi.bmiHeader.biHeight = -height;

	data = reinterpret_cast<DWORD *>(::VirtualAlloc(NULL, width * height * 4, MEM_COMMIT, PAGE_READWRITE));
}

void FGXBitmap::Free() {
	if (data != NULL) {
		::VirtualFree(data, 0, MEM_RELEASE);
		data = NULL;
	}
}

void FGXBitmap::InitializeGrid(const CRect &rect) {
	if (data != NULL)
		for (int j = rect.top; j < rect.bottom; j++)
			for (int i = rect.left; i < rect.right; i++)
				PixelAt(i, j) = (j / 8) % 2 - (i / 8) % 2 ? 0x00cccccc : 0x00ffffff;
}

void FGXBitmap::Draw(CDC *pDC, int x, int y, int width, int height) const {
	if (data != NULL)
		::StretchDIBits(pDC->m_hDC, x, y, width, height, 0, 0, bi.bmiHeader.biWidth, -bi.bmiHeader.biHeight, data, &bi, DIB_RGB_COLORS, SRCCOPY);
}
