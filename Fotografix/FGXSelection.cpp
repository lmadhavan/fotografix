#include "StdAfx.h"
#include "FGXSelection.h"
#include "FGXLayer.h"

#include <cmath>

#include <queue>
using std::queue;

inline float sqr(float x) {
	return x * x;
}

void FGXSelection::SelectInverse(FGXChannel &undo, const CRect &bounds) {
	ExpandTo(undo, bounds);
	Adjust(FGXChannel(), GetPosition(), AdjustInvert, 0, 0, 0);
	ExpandTo(FGXChannel(), CalcBounds());
}

void FGXSelection::SelectRectangle(FGXChannel &undo, const CRect &rect, SelectType type) {
	switch (type) {
		case SelectNormal:
		case SelectAdd:
			if (type == SelectNormal) {
				Transfer(undo);
				SetPosition(rect);
				Allocate();
			} else
				ExpandTo(undo, rect | position);

			for (int j = rect.top; j < rect.bottom; j++)
				for (int i = rect.left; i < rect.right; i++)
					PixelAt(i, j) = 0xff;

			break;

		case SelectSubtract:
			{
				Clone(undo);

				CRect r = position & rect;

				for (int j = r.top; j < r.bottom; j++)
					for (int i = r.left; i < r.right; i++)
						PixelAt(i, j) = 0;

				ExpandTo(FGXChannel(), CalcBounds());
			}

			break;
	}
}

void FGXSelection::SelectEllipse(FGXChannel &undo, const CRect &rect, SelectType type) {
	float aa = sqr(rect.Width()) / 4.0,
		  bb = sqr(rect.Height()) / 4.0,
		  aa2 = sqr(rect.Width() + 1) / 4.0,
		  bb2 = sqr(rect.Height() + 1) / 4.0;
	int   xx = rect.CenterPoint().x,
		  yy = rect.CenterPoint().y;

	switch (type) {
		case SelectNormal:
		case SelectAdd:
			if (type == SelectNormal) {
				Transfer(undo);
				SetPosition(rect);
				Allocate();
			} else
				ExpandTo(undo, rect | position);

			for (int j = rect.top; j < rect.bottom; j++)
				for (int i = rect.left; i < rect.right; i++)
					if (sqr(i - xx) / aa + sqr(j - yy) / bb <= 1.0)
						PixelAt(i, j) = 0xff;
					else if (sqr(i - xx) / aa2 + sqr(j - yy) / bb2 <= 1.0)
						PixelAt(i, j) = max(PixelAt(i, j), 0x80);

			break;

		case SelectSubtract:
			{
				Clone(undo);

				CRect r = position & rect;

				for (int j = r.top; j < r.bottom; j++)
					for (int i = r.left; i < r.right; i++)
						if (sqr(i - xx) / aa + sqr(j - yy) / bb <= 1.0)
							PixelAt(i, j) = 0;
						else if (sqr(i - xx) / aa2 + sqr(j - yy) / bb2 <= 1.0)
							PixelAt(i, j) = min(PixelAt(i, j), 0x80);

				ExpandTo(FGXChannel(), CalcBounds());
			}

			break;
	}
}

BYTE colorDistance(short r1, short g1, short b1, short a1, short r2, short g2, short b2, short a2) {
	return short(sqrt(sqr(r1 - r2) + sqr(g1 - g2) + sqr(b1 - b2) + sqr(a1 - a2))) / 2;
}

void FGXSelection::SelectWand(FGXChannel &undo, const FGXLayer &layer, CPoint pt, int tolerance, SelectType type) {
	const CRect &rect = layer.GetPosition();

	switch (type) {
		case SelectNormal:
		case SelectAdd:
			if (type == SelectNormal) {
				Transfer(undo);
				SetPosition(rect);
				Allocate();
			} else
				ExpandTo(undo, rect | position);

			if (rect.PtInRect(pt) == true) {
				const FGXChannel *c = layer.channels;
				short r, g, b, a;
				queue<CPoint> q;

				r = c[1].PixelAt(pt.x, pt.y);
				g = c[2].PixelAt(pt.x, pt.y);
				b = c[3].PixelAt(pt.x, pt.y);
				a = c[0].PixelAt(pt.x, pt.y);

				q.push(pt);

				while (q.empty() == false) {
					CPoint pt = q.front();
					q.pop();

					if (PixelAt(pt.x, pt.y) < 0xff && colorDistance(r, g, b, a, c[1].PixelAt(pt.x, pt.y), c[2].PixelAt(pt.x, pt.y), c[3].PixelAt(pt.x, pt.y), c[0].PixelAt(pt.x, pt.y)) <= tolerance) {
							PixelAt(pt.x, pt.y) = 0xff;

							int i;

							i = 1;
							while (pt.x - i >= rect.left && colorDistance(r, g, b, a, c[1].PixelAt(pt.x - i, pt.y), c[2].PixelAt(pt.x - i, pt.y), c[3].PixelAt(pt.x - i, pt.y), c[0].PixelAt(pt.x - i, pt.y)) <= tolerance) {
								PixelAt(pt.x - i, pt.y) = 0xff;
								if (pt.y > rect.top)
									q.push(CPoint(pt.x - i, pt.y - 1));
								if (pt.y < rect.bottom - 1)
									q.push(CPoint(pt.x - i, pt.y + 1));
								i++;
							}

							i = 1;
							while (pt.x + i < rect.right && colorDistance(r, g, b, a, c[1].PixelAt(pt.x + i, pt.y), c[2].PixelAt(pt.x + i, pt.y), c[3].PixelAt(pt.x + i, pt.y), c[0].PixelAt(pt.x + i, pt.y)) <= tolerance) {
								PixelAt(pt.x + i, pt.y) = 0xff;
								if (pt.y > rect.top)
									q.push(CPoint(pt.x + i, pt.y - 1));
								if (pt.y < rect.bottom - 1)
									q.push(CPoint(pt.x + i, pt.y + 1));
								i++;
							}
					}
				}
			}

			ExpandTo(FGXChannel(), CalcBounds());

			break;
	}
}

void FGXSelection::SelectRange(FGXChannel &undo, const FGXLayer &layer, CPoint pt, int tolerance, SelectType type) {
	const CRect &rect = layer.GetPosition();

	switch (type) {
		case SelectNormal:
		case SelectAdd:
			if (type == SelectNormal) {
				Transfer(undo);
				SetPosition(rect);
				Allocate();
			} else
				ExpandTo(undo, rect | position);

			if (rect.PtInRect(pt) == true) {
				const FGXChannel *c = layer.channels;
				short r, g, b, a;

				r = c[1].PixelAt(pt.x, pt.y);
				g = c[2].PixelAt(pt.x, pt.y);
				b = c[3].PixelAt(pt.x, pt.y);
				a = c[0].PixelAt(pt.x, pt.y);

				for (int j = rect.top; j < rect.bottom; j++)
					for (int i = rect.left; i < rect.right; i++)
						if (colorDistance(r, g, b, a, c[1].PixelAt(i, j), c[2].PixelAt(i, j), c[3].PixelAt(i, j), c[0].PixelAt(i, j)) <= tolerance)
								PixelAt(i, j) = 0xff;
			}

			ExpandTo(FGXChannel(), CalcBounds());

			break;
	}
}

void FGXSelection::InitAnimate(FGXBitmap &bitmap, const CRect &rect) const {
	CRect common = position & rect;

	// Test border pixels

	for (int j = common.top; j < common.bottom; j++) {
		if (PixelAt(common.left, j) > 0)
			bitmap.PixelAt(common.left, j) = ((common.left + j) % 8 + 1) << 24;
		if (PixelAt(common.right - 1, j) > 0)
			bitmap.PixelAt(common.right - 1, j) = ((common.right - 1 + j) % 8 + 1) << 24;
	}

	for (int i = common.left; i < common.right; i++) {
		if (PixelAt(i, common.top) > 0)
			bitmap.PixelAt(i, common.top) = ((i + common.top) % 8 + 1) << 24;
		if (PixelAt(i, common.bottom - 1) > 0)
			bitmap.PixelAt(i, common.bottom - 1) = ((i + common.bottom - 1) % 8 + 1) << 24;
	}

	// Test inner pixels

	common.DeflateRect(1, 1);

	for (int j = common.top; j < common.bottom; j++)
		for (int i = common.left; i < common.right; i++)
			if (PixelAt(i, j) > 0 && IsBorderPixel(i, j) == true)
				bitmap.PixelAt(i, j) = ((i + j) % 8 + 1) << 24;
}

void FGXSelection::Animate(FGXBitmap &bitmap, const CRect &rect) const {
	CRect common = position & rect;

	for (int j = common.top; j < common.bottom; j++)
		for (int i = common.left; i < common.right; i++) {
			BYTE b = bitmap.PixelAt(i, j) >> 24;

			if (b > 0) {
				b = b % 8 + 1;
				bitmap.PixelAt(i, j) = (b << 24) | ((b < 5) ? 0x000000 : 0xffffff);
			}
		}
}

void FGXSelection::LoadLayer(FGXChannel &undo, const FGXLayer &layer) {
	Clone(undo);
	const_cast<FGXLayer &>(layer).Compact();
	layer.channels[0].Clone(*this);
}
