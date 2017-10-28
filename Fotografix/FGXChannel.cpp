#include "StdAfx.h"
#include "FGXChannel.h"
#include "FGXSelection.h"

#include "FGX.h"
#include "Language.h"

LPCTSTR channelNames[8] = {
	TEXT("RGB"),
	TEXT("Mask"),
	TEXT("Red"),
	TEXT("Green"),
	TEXT("Blue"),
	TEXT("Red-Green"),
	TEXT("Green-Blue"),
	TEXT("Blue-Red")
};

CString channelText[8];

int channelMasks[8] = {
	ChannelAll,
	ChannelMask,
	ChannelRed,
	ChannelGreen,
	ChannelBlue,
	ChannelRed | ChannelGreen,
	ChannelGreen | ChannelBlue,
	ChannelBlue | ChannelRed
};

void FGXChannel::Allocate() {
	if (data != NULL) Free();
	data = reinterpret_cast<BYTE *>(::VirtualAlloc(NULL, position.Width() * position.Height(), MEM_COMMIT, PAGE_READWRITE));
}

void FGXChannel::Free() {
	if (data != NULL) {
		::VirtualFree(data, 0, MEM_RELEASE);
		data = NULL;
		position.SetRectEmpty();
	}
}

void FGXChannel::MoveTo(long left, long top) {
	//position.right += left - position.left;
	//position.bottom += top - position.top;
	//position.left = left;
	//position.top = top;
	position.MoveToXY(left, top);
	pixelOffset = - position.left - position.top * position.Width();
}

void FGXChannel::ExpandTo(FGXChannel &undo, const CRect &newPosition, bool mask) {
	Transfer(undo);

	SetPosition(newPosition);
	Allocate();

	// If this is a mask channel, initialize to white

	if (mask == true) Fill(255);

	// Find the intersecting region and copy common contents to the new data block

	CRect common = undo.position & position;

	if (undo.IsAllocated() && common.IsRectEmpty() == false) {
		FGXAccessor  p(*this, common);
		FGXAccessorC u(undo, common);

		BeginLoopRect(common, (p++, u++))
			*p = *u;
		EndLoopRect((p.next(), u.next()))
	}
}

CRect FGXChannel::CalcBounds() {
	CRect rect(position.right, position.bottom, position.left, position.top);

	for (int j = position.top; j < position.bottom; j++)
		for (int i = position.left; i < position.right; i++)
			if (PixelAt(i, j) > 0) {
				if (i < rect.left)
					rect.left = i;
				else if (i >= rect.right)
					rect.right = i + 1;

				if (j < rect.top)
					rect.top = j;
				else if (j >= rect.bottom)
					rect.bottom = j + 1;
			}

	return rect;
}

void FGXChannel::ResizeTo(FGXChannel &undo, const CRect &newPosition) {
	Transfer(undo);

	SetPosition(newPosition);
	Allocate();

	if (undo.IsAllocated()) {
		int w = position.Width(),
			h = position.Height(),
			ow = undo.position.Width(),
			oh = undo.position.Height();

		float fx = float(ow) / w,
			  fy = float(oh) / h;

		int rx = int(fx),
			ry = int(fy);

		rx = (rx << 12) | int((fx - rx) * 0x1000);
		ry = (ry << 12) | int((fy - ry) * 0x1000);

		int uw = ow - 2,
			uh = oh - 2;

		BYTE *p = data;
		for (int j = 0; j < h; j++)
			for (int i = 0; i < w; i++, p++) {
				int x = i * rx,
					y = j * ry;
				int dx, dy;

				dx = x & 0xFFF, x = x >> 12; if (x >= uw) x = uw;
				dy = y & 0xFFF, y = y >> 12; if (y >= uh) y = uh;

				int a = (0x1000 - dx) * (0x1000 - dy),
					b = (         dx) * (0x1000 - dy),
					c = (         dx) * (         dy),
					d = 0x1000000 - (a + b + c);

				BYTE *o = undo.data + y * ow + x;
				*p = ((*o * a) + (*(o+1) * b) + (*(o+ow+1) * c) + (*(o+ow) * d)) >> 24;
			}
	}
}

void FGXChannel::Flip(bool horizontal, bool vertical) {
	if (horizontal == true) {
		BYTE *p = data;
		int w = position.Width(),
			h = position.Height(),
			w2 = w / 2;

		for (int j = 0; j < h; j++) {
			BYTE *q = p + w - 1;

			for (int i = 0; i < w2; i++, p++, q--)
				swap(*p, *q);

			p += w - w2;
		}
	}

	if (vertical == true) {
		BYTE *p = data;
		int w = position.Width(),
			h = position.Height(),
			h2 = h / 2;

		for (int j = 0; j < h2; j++) {
			BYTE *q = data + (h - j - 1) * w;

			for (int i = 0; i < w; i++, p++, q++)
				swap(*p, *q);
		}
	}
}

#include <cmath>

void FGXChannel::Rotate(FGXChannel &undo, int angle) {
	Transfer(undo);

	angle = angle % 360;

	switch (angle) {
	case 0:
		break;

	case 90:
		SetPosition(CRect(position.top, position.left, position.bottom, position.right));
		Allocate();
		{
			FGXAccessor p(*this, position);
			int d = position.right + position.left - 1;
			BeginLoopRect(position, p++)
				*p = undo.PixelAt(j, d - i);
			EndLoopRect(p.next())

			CRect rect = position;
			rect.OffsetRect(undo.position.CenterPoint() - position.CenterPoint());
			MoveTo(rect.left, rect.top);
		}
		break;

	case 180:
		undo.Clone(*this);
		Flip(true, true);
		break;

	case 270:
		SetPosition(CRect(position.top, position.left, position.bottom, position.right));
		Allocate();
		{
			FGXAccessor p(*this, position);
			int d = position.bottom + position.top - 1;
			BeginLoopRect(position, p++)
				*p = undo.PixelAt(d - j, i);
			EndLoopRect(p.next())

			CRect rect = position;
			rect.OffsetRect(undo.position.CenterPoint() - position.CenterPoint());
			MoveTo(rect.left, rect.top);
		}
		break;

	default:
		{
			float a = angle * 3.14159 / 180;
			int m = (position.Width() + position.Height()) * max(cos(a), sin(a));

			CPoint oc = position.CenterPoint();
			SetPosition(CRect(CPoint(oc.x - m / 2, oc.y - m / 2), CSize(m, m)));
			CPoint nc = position.CenterPoint();

			Allocate();

			FGXAccessor p(*this, position);
			BeginLoopRect(position, p++) {
				int x = i - nc.x,
					y = j - nc.y;
				CPoint pt(oc.x + x * cos(a) + y * sin(a), oc.y - x * sin(a) + y * cos(a));

				if (undo.position.PtInRect(pt))
					*p = undo.PixelAt(pt.x, pt.y);
			} EndLoopRect(p.next())
		}
		break;
	}
}

void FGXChannel::Render(FGXBitmap &bitmap, int channel, const FGXChannel &alpha, BYTE opacity, const CRect &rect) const {
	CRect common = position & rect;

	FGXAccessorC p(*this, common),
				 a(alpha, common);
	FGXAccessorB b(bitmap, channel, common);

	//// For opacity < 255:

	//if (opacity < 255) {
	//	BeginLoopRect(common, (p++, a++, b++))
	//		if (*a > 0) *b = FGXBlend(*p, FGXMultiply(opacity, *a), *b);
	//	EndLoopRect((p.next(), a.next(), b.next()))
	//}

	//// For opacity = 255:

	//else {
		BeginLoopRect(common, (p++, a++, b++))
			if (*a > 0) *b = FGXBlend(*p, *a, *b);
		EndLoopRect((p.next(), a.next(), b.next()))
	//}
}

void FGXChannel::AlphaRender(FGXChannel &channel, const FGXChannel &channelAlpha, const FGXChannel &alpha, BYTE opacity, BYTE mode, const CRect &rect) const {
	CRect common = position & channel.position & rect;

	FGXAccessor  c(channel, common);
	FGXAccessorC a(alpha, common),
				 p(*this, common),
				 ca(channelAlpha, common);

	// For opacity < 255:

	if (opacity < 255) switch (mode) {
	case ModeNormal:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(*p, FGXMultiply(opacity, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeMultiply:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXMultiply(*p, *c), FGXMultiply(opacity, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeScreen:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXScreen(*p, *c), FGXMultiply(opacity, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeOverlay:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXOverlay(*p, *c), FGXMultiply(opacity, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeHardLight:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXOverlay(*c, *p), FGXMultiply(opacity, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeLinearDodge:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXBrightnessPlus(*c, *p), FGXMultiply(opacity, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeLinearBurn:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXBrightnessMinus(*c, *p), FGXMultiply(opacity, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeDarken:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXDarken(*c, *p), FGXMultiply(opacity, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeLighten:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXLighten(*c, *p), FGXMultiply(opacity, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeDifference:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXDifference(*c, *p), FGXMultiply(opacity, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeExclusion:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXExclusion(*c, *p), FGXMultiply(opacity, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModePinLight:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXPinLight(*p, *c), FGXMultiply(opacity, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeHardMix:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXHardMix(*p, *c), FGXMultiply(opacity, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;
	}

	// For opacity = 255:

	else switch (mode) {
	case ModeNormal:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(*p, *a, *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeMultiply:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXMultiply(*p, *c), *a, *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeScreen:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXScreen(*p, *c), *a, *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeOverlay:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXOverlay(*p, *c), *a, *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeHardLight:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXOverlay(*c, *p), *a, *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeLinearDodge:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXBrightnessPlus(*c, *p), *a, *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeLinearBurn:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXBrightnessMinus(*c, *p), *a, *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeDarken:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXDarken(*c, *p), *a, *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeLighten:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXLighten(*c, *p), *a, *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeDifference:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXDifference(*c, *p), *a, *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeExclusion:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXExclusion(*c, *p), *a, *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModePinLight:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXPinLight(*p, *c), *a, *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeHardMix:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXHardMix(*p, *c), *a, *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;
	}
}

void FGXChannel::AlphaRender(FGXChannel &channel, const FGXChannel &channelAlpha, const FGXChannel &alpha, const FGXChannel &mask, BYTE opacity, BYTE mode, const CRect &rect) const {
	CRect common = position & channel.position & rect;

	FGXAccessor  c(channel, common);
	FGXAccessorC a(alpha, common),
				 m(mask, common),
				 p(*this, common),
				 ca(channelAlpha, common);

	// For opacity < 255:

	if (opacity < 255) switch (mode) {
	case ModeNormal:
		BeginLoopRect(common, (a++, c++, m++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(*p, FGXMultiply(opacity, FGXMultiply(*m, *a)), *c, *ca);
		EndLoopRect((a.next(), c.next(), m.next(), p.next(), ca.next()))
		break;

	case ModeMultiply:
		BeginLoopRect(common, (a++, c++, m++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXMultiply(*p, *c), FGXMultiply(opacity, FGXMultiply(*m, *a)), *c, *ca);
		EndLoopRect((a.next(), c.next(), m.next(), p.next(), ca.next()))
		break;

	case ModeScreen:
		BeginLoopRect(common, (a++, c++, m++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXScreen(*p, *c), FGXMultiply(opacity, FGXMultiply(*m, *a)), *c, *ca);
		EndLoopRect((a.next(), c.next(), m.next(), p.next(), ca.next()))
		break;

	case ModeOverlay:
		BeginLoopRect(common, (a++, c++, m++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXOverlay(*p, *c), FGXMultiply(opacity, FGXMultiply(*m, *a)), *c, *ca);
		EndLoopRect((a.next(), c.next(), m.next(), p.next(), ca.next()))
		break;

	case ModeHardLight:
		BeginLoopRect(common, (a++, c++, m++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXOverlay(*c, *p), FGXMultiply(opacity, FGXMultiply(*m, *a)), *c, *ca);
		EndLoopRect((a.next(), c.next(), m.next(), p.next(), ca.next()))
		break;

	case ModeLinearDodge:
		BeginLoopRect(common, (a++, c++, m++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXBrightnessPlus(*c, *p), FGXMultiply(opacity, FGXMultiply(*m, *a)), *c, *ca);
		EndLoopRect((a.next(), c.next(), m.next(), p.next(), ca.next()))
		break;

	case ModeLinearBurn:
		BeginLoopRect(common, (a++, c++, m++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXBrightnessMinus(*c, *p), FGXMultiply(opacity, FGXMultiply(*m, *a)), *c, *ca);
		EndLoopRect((a.next(), c.next(), m.next(), p.next(), ca.next()))
		break;

	case ModeDarken:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXDarken(*c, *p), FGXMultiply(opacity, FGXMultiply(*m, *a)), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeLighten:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXLighten(*c, *p), FGXMultiply(opacity, FGXMultiply(*m, *a)), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeDifference:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXDifference(*c, *p), FGXMultiply(opacity, FGXMultiply(*m, *a)), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeExclusion:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXExclusion(*c, *p), FGXMultiply(opacity, FGXMultiply(*m, *a)), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModePinLight:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXPinLight(*p, *c), FGXMultiply(opacity, FGXMultiply(*m, *a)), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeHardMix:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXHardMix(*p, *c), FGXMultiply(opacity, FGXMultiply(*m, *a)), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;
	}

	// For opacity = 255:

	else switch (mode) {
	case ModeNormal:
		BeginLoopRect(common, (a++, c++, m++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(*p, FGXMultiply(*m, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), m.next(), p.next(), ca.next()))
		break;

	case ModeMultiply:
		BeginLoopRect(common, (a++, c++, m++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXMultiply(*p, *c), FGXMultiply(*m, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), m.next(), p.next(), ca.next()))
		break;

	case ModeScreen:
		BeginLoopRect(common, (a++, c++, m++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXScreen(*p, *c), FGXMultiply(*m, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), m.next(), p.next(), ca.next()))
		break;

	case ModeOverlay:
		BeginLoopRect(common, (a++, c++, m++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXOverlay(*p, *c), FGXMultiply(*m, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), m.next(), p.next(), ca.next()))
		break;

	case ModeHardLight:
		BeginLoopRect(common, (a++, c++, m++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXOverlay(*c, *p), FGXMultiply(*m, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), m.next(), p.next(), ca.next()))
		break;

	case ModeLinearDodge:
		BeginLoopRect(common, (a++, c++, m++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXBrightnessPlus(*c, *p), FGXMultiply(*m, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), m.next(), p.next(), ca.next()))
		break;

	case ModeLinearBurn:
		BeginLoopRect(common, (a++, c++, m++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXBrightnessMinus(*c, *p), FGXMultiply(*m, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), m.next(), p.next(), ca.next()))
		break;

	case ModeDarken:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXDarken(*c, *p), FGXMultiply(*m, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeLighten:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXLighten(*c, *p), FGXMultiply(*m, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeDifference:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXDifference(*c, *p), FGXMultiply(*m, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeExclusion:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXExclusion(*c, *p), FGXMultiply(*m, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModePinLight:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXPinLight(*p, *c), FGXMultiply(*m, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;

	case ModeHardMix:
		BeginLoopRect(common, (a++, c++, p++, ca++))
			if (*a > 0) *c = FGXAlphaBlend(FGXHardMix(*p, *c), FGXMultiply(*m, *a), *c, *ca);
		EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
		break;
	}
}

void FGXChannel::AlphaRenderMask(FGXChannel &channel, const FGXChannel &alpha, BYTE opacity, const CRect &rect) const {
	CRect common = position & channel.position & rect;

	FGXAccessor  c(channel, common);
	FGXAccessorC a(alpha, common),
				 p(*this, common);

	// For opacity < 255:

	if (opacity < 255) {
		//BeginLoopRect(common, (a++, c++, p++, ca++))
		//	if (*a > 0) *c = FGXAlphaBlend(*p, FGXMultiply(opacity, *a), *c, *ca);
		//EndLoopRect((a.next(), c.next(), p.next(), ca.next()))
	}

	// For opacity = 255:

	else {
		BeginLoopRect(common, (a++, c++, p++))
			if (*a > 0) *c = FGXBlend(*p, *a, *c);
		EndLoopRect((a.next(), c.next(), p.next()))
	}
}

void FGXChannel::AlphaSquare(FGXChannel &alpha, BYTE opacity, const CRect &rect) const {
	CRect common = position & alpha.position & rect;

	FGXAccessor  a(alpha, common);
	FGXAccessorC p(*this, common);

	// For opacity < 255:

	if (opacity < 255) {
		BeginLoopRect(common, (a++, p++))
			*a = FGXScreen(*a, FGXMultiply(opacity, *p));
		EndLoopRect((a.next(), p.next()))
	}

	// For opacity = 255:

	else {
		BeginLoopRect(common, (a++, p++))
			*a = FGXScreen(*a, *p);
		EndLoopRect((a.next(), p.next()))
	}
}

void FGXChannel::AlphaSquare(FGXChannel &alpha, const FGXChannel &mask, BYTE opacity, const CRect &rect) const {
	CRect common = position & alpha.position & rect;

	FGXAccessor  a(alpha, common);
	FGXAccessorC m(mask, common),
				 p(*this, common);

	// For opacity < 255:

	if (opacity < 255) {
		BeginLoopRect(common, (a++, m++, p++))
			*a = FGXScreen(*a, FGXMultiply(opacity, FGXMultiply(*m, *p)));
		EndLoopRect((a.next(), m.next(), p.next()))
	}

	// For opacity = 255:

	else {
		BeginLoopRect(common, (a++, m++, p++))
			*a = FGXScreen(*a, FGXMultiply(*m, *p));
		EndLoopRect((a.next(), m.next(), p.next()))
	}
}

void FGXChannel::Erase(FGXChannel &alpha, BYTE opacity, const CRect &rect) const {
	CRect common = position & alpha.position & rect;

	FGXAccessor  a(alpha, common);
	FGXAccessorC p(*this, common);

	BeginLoopRect(common, (a++, p++))
		*a = FGXMultiply(*a, FGXMultiply(FGXInvert(*p), opacity));
	EndLoopRect((a.next(), p.next()))
}

void FGXChannel::LoadFromMemory(const void *bits, int skip) {
	BYTE *p = data;
	const BYTE *q = reinterpret_cast<const BYTE *>(bits);

	for (int i = 0, n = position.Width() * position.Height(); i < n; i++, p++, q += skip)
		*p = *q;
}

void FGXChannel::SaveToMemory(void *bits, int skip) const {
	BYTE *p = data,
		 *q = reinterpret_cast<BYTE *>(bits);

	for (int i = 0, n = position.Width() * position.Height(); i < n; i++, p++, q += skip)
		*q = *p;
}

void FGXChannel::Clone(FGXChannel &result) const {
	if (result.IsAllocated() == false || result.position.Width() != position.Width() || result.position.Height() != position.Height()) {
		result.Free();
		result.SetPosition(position);
		if (IsAllocated()) result.Allocate();
	}

	if (IsAllocated()) memcpy(result.data, data, position.Width() * position.Height());
}

void FGXChannel::CopyRect(FGXChannel &result, BYTE opacity, const CRect &rect) const {
	CRect common = position & result.position & rect;

	FGXAccessor  r(result, common);
	FGXAccessorC p(*this, common);

	if (opacity < 255) {
		BeginLoopRect(common, (r++, p++))
			*r = FGXMultiply(opacity, *p);
		EndLoopRect((r.next(), p.next()))
	} else {
		BeginLoopRect(common, (r++, p++))
			*r = *p;
		EndLoopRect((r.next(), p.next()))
	}
}

void FGXChannel::CopyRect(FGXChannel &result, const FGXChannel &mask, BYTE opacity, const CRect &rect) const {
	CRect common = position & result.position & rect;

	FGXAccessor  r(result, common);
	FGXAccessorC m(mask, common),
				 p(*this, common);

	if (opacity < 255) {
		BeginLoopRect(common, (r++, m++, p++))
			*r = FGXMultiply(*m, FGXMultiply(opacity, *p));
		EndLoopRect((r.next(), m.next(), p.next()))
	} else {
		BeginLoopRect(common, (r++, m++, p++))
			*r = FGXMultiply(*m, *p);
		EndLoopRect((r.next(), m.next(), p.next()))
	}
}

void FGXChannel::MultiplyChannel(FGXChannel &result, const FGXChannel &source) const {
	CRect common = position & result.position & source.position;

	FGXAccessor  r(result, common);
	FGXAccessorC p(*this, common),
				 s(source, common);

	BeginLoopRect(common, (r++, p++, s++))
		*r = FGXMultiply(*p, *s);
	EndLoopRect((r.next(), p.next(), s.next()))
}

void FGXChannel::Clear(const FGXChannel &selection, BYTE color) {
	CRect common = position & selection.position;

	FGXAccessor  p(*this, common);
	FGXAccessorC s(selection, common);

	BeginLoopRect(common, (p++, s++))
		*p = FGXBlend(color, *s, *p);
	EndLoopRect((p.next(), s.next()))
}

void FGXChannel::Fill(BYTE color) {
	memset(data, color, position.Width() * position.Height());
}

void FGXChannel::Transfer(FGXChannel &result) {
	result.Free();

	result.SetPosition(position);
	
	result.data = data;
	data = NULL;
}

void FGXChannel::Swap(FGXChannel &other) {
	swap(data, other.data);
	swap(position, other.position);
	swap(pixelOffset, other.pixelOffset);
}

void FGXChannel::ApplyLUT(FGXChannel &undo, const CRect &rect, const FGXTable &table) {
	Clone(undo);

	CRect common = position & rect;

	FGXAccessor p(*this, common);

	BeginLoopRect(common, p++)
		*p = table[*p];
	EndLoopRect(p.next())
}

void FGXChannel::ApplyMatrix(FGXChannel &undo, const CRect &rect, short a11, short a12, short a13, short a21, short a22, short a23, short a31, short a32, short a33, short factor, short bias) {
	if (undo.IsAllocated() == false)
		Clone(undo);

	CRect common = position & rect;
	common.DeflateRect(1, 1);

	BYTE *p = data + (common.top - position.top) * position.Width() + (common.left - position.left);
	BYTE *u = undo.data + (common.top - position.top) * position.Width() + (common.left - position.left);
	int w = position.Width(), wa = w - 1, wb = w + 1;
	int delta = w - common.Width();

	BeginLoopRect(common, (p++, u++))
		*p = clamp<short>(
			 (a11 * *(u - wb)	+ a12 * *(u - w)	+ a13 * *(u - wa) +
			  a21 * *(u - 1)	+ a22 * *u			+ a23 * *(u + 1)  +
			  a31 * *(u + wa)	+ a32 * *(u + w)	+ a33 * *(u + wb)) / factor
			  + bias, 0, 255);
	EndLoopRect((p += delta, u += delta))
}

void FGXChannel::Adjust(FGXChannel &undo, const CRect &rect, int type, int a1, int a2, int a3) {
	// Adjustments: use lookup table
	if (type < FilterBlur) {
		FGXTable table;

		switch (type) {
		case AdjustInvert:
			for (int i = 0; i < 256; i++)
				table[i] = 255 - i;
			break;

		case AdjustBrightnessContrast:
			for (int i = 0; i < 256; i++) {
				table[i] = i;

				if (a1 > 0) table[i] = FGXBrightnessPlus(table[i], a1);
				else if (a1 < 0) table[i] = FGXBrightnessMinus(table[i], -a1);

				if (a2 > 0) table[i] = FGXContrastPlus(table[i], a2);
				else if (a2 < 0) table[i] = FGXContrastMinus(table[i], -a2);
			}
			break;

		case AdjustLevels:
			if (a2 == 100) {
				for (int i = 0; i < 256; i++) {
					table[i] = FGXLevels(i, a1, a3);
				}
			} else {
				for (int i = 0; i < 256; i++) {
					table[i] = FGXLevels(i, a1, a3);
					table[i] = FGXGamma(table[i], a2);
				}
			}
			break;

		case AdjustPosterize:
			for (int i = 0; i < 256; i++)
				table[i] = FGXPosterize(i, a1);
			break;
		}

		ApplyLUT(undo, rect, table);
	}

	// Filters
	else {
		Clone(undo);

		CRect common = position & rect;

		switch (type) {
		case FilterEmboss:
			ApplyMatrix(undo, rect, -1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 128);
			break;

		case FilterShear:
			{
				if (a1 == 0) break;

				int w = position.right - 1,
					h = position.bottom - 1,
					a = a1 / 2;

				srand(w * h);

				FGXAccessor p(*this, common);

				BeginLoopRect(common, p++)
					*p = undo.PixelAt(clamp<int>(i + rand() % a1 - a, position.left, w), clamp<int>(j + rand() % a1 - a, position.top, h));
				EndLoopRect(p.next())
			}
			break;

		case FilterSolarize:
			{
				FGXAccessor  p(*this, common);
				FGXAccessorC o(undo, common);
				int w = common.Width();

				BeginLoopRect(common, (p++, o++))
					*p = (*o > (i << 7) / w) ? *o : FGXInvert(*o);
				EndLoopRect((p.next(), o.next()))
			}
			break;

		case FilterAddNoise:
			{
				FGXAccessor  p(*this, common);
				FGXAccessorC o(undo, common);

				BYTE a = a1;
				srand(common.right * common.bottom);

				BeginLoopRect(common, (p++, o++))
					*p = (rand() % 100 < a) ? FGXBrightnessPlus(*o, (rand() + int(this)) % 255) : *o;
				EndLoopRect((p.next(), o.next()))
			}
			break;

		case FilterMotionBlur:
			{
				CRect rect = position;
				rect.InflateRect(a1, 0);

				FGXChannel temp;
				undo.ExpandTo(temp, rect);
				undo.Swap(temp);

				FGXAccessor  p(*this, position);
				FGXAccessorC o1(temp, rect),
							 o2(temp, rect);

				int r = 2 * a1 + 1;
				int sum;

				for (int j = position.top; j < position.bottom; j++) {
					sum = 0;

					o2 += a1;
					for (int i = 0; i < a1 + 1; i++, o2++) sum += *o2;

					for (int i = position.left; i < position.right - 1; i++, p++, o1++, o2++) {
						*p = sum / (1 + min(a1, i - position.left) + min(a1, position.right - 1 - i));
						sum = sum + *o2 - *o1;
					}
					*p = sum / (1 + min(a1, position.Width() - 1)), p++, o1 += r;

					p.next(), o1.next(), o2.next();
				}
			}
			break;

		case FilterBlur:
			{
				CRect rect = position;
				rect.InflateRect(a1, 0);

				FGXChannel temp;
				undo.ExpandTo(temp, rect);
				undo.Swap(temp);

				FGXAccessor  p(*this, position);
				FGXAccessorC o1(temp, rect),
							 o2(temp, rect);

				int r = 2 * a1 + 1;
				int sum;

				for (int j = position.top; j < position.bottom; j++) {
					sum = 0;

					o2 += a1;
					for (int i = 0; i < a1 + 1; i++, o2++) sum += *o2;

					for (int i = position.left; i < position.right - 1; i++, p++, o1++, o2++) {
						*p = sum / (1 + min(a1, i - position.left) + min(a1, position.right - 1 - i));
						sum = sum + *o2 - *o1;
					}
					*p = sum / (1 + min(a1, position.Width() - 1)), p++, o1 += r;

					p.next(), o1.next(), o2.next();
				}
			}
			{
				CRect rect = position;
				rect.InflateRect(0, a1);

				FGXChannel temp;
				Transfer(temp);
				temp.ExpandTo(FGXChannel(), rect);
				Allocate();

				FGXVAccessor  p(*this, position);
				FGXVAccessorC o1(temp, rect),
							  o2(temp, rect);

				int r = 2 * a1 + 1;
				int sum;

				for (int i = position.left; i < position.right; i++) {
					sum = 0;

					o2 += a1;
					for (int j = 0; j < a1 + 1; j++, o2++) sum += *o2;

					for (int j = position.top; j < position.bottom - 1; j++, p++, o1++, o2++) {
						*p = sum / (1 + min(a1, j - position.top) + min(a1, position.bottom - 1 - j));
						sum = sum + *o2 - *o1;
					}
					*p = sum / (1 + min(a1, position.Height() - 1)), p++, o1 += r;

					p.next(), o1.next(), o2.next();
				}
			}
			break;

		case FilterGaussianBlur:
			if (a1 == 1)
				ApplyMatrix(undo, rect, 1, 2, 1, 2, 4, 2, 1, 2, 1, 16, 0);
			else {
				int r[3];
				FGXChannel temp;

				r[0] = r[1] = (a1 + 2) / 3;
				r[2] = a1 - r[0] - r[1];

				for (int i = 0; i < 3; i++)
					Adjust(temp, rect, FilterBlur, r[i], 0, 0);
			}
			break;

		case FilterOffset:
			{
				a1 = position.Width() - a1 % position.Width();
				a2 = position.Height() - a2 % position.Height();

				FGXAccessor p(*this, common);

				BeginLoopRect(common, p++)
					*p = undo.PixelAt(position.left + (i + a1) % position.Width(), position.top + (j + a2) % position.Height());
				EndLoopRect(p.next())
			}
			break;

		case FilterPixelate:
			{
				FGXAccessor p(*this, common);

				BeginLoopRect(common, p++)
					*p = undo.PixelAt((i >> a1) << a1, (j >> a1) << a1);
				EndLoopRect(p.next())
			}
			break;

		case FilterHalftone:
			{
				if (a1 == 0) break;

				FGXAccessor  p(*this, common);
				FGXAccessorC o(undo, common);

				float f = 3141.59f / (180 * a1);

				BeginLoopRect(common, (p++, o++))
					*p = *o > (sin(i * f) * cos(j * f) + 1.0f) * 127.5f ? 255 : 0;
				EndLoopRect((p.next(), o.next()))
			}
			break;

		case FilterSharpen:
			ApplyMatrix(undo, rect, 0, -1, 0, -1, 5, -1, 0, -1, 0, 1, 0);
			break;

		case FilterUnsharpMask:
			ApplyMatrix(undo, rect, -a1, -a1, -a1, -a1, 1000 - a1, -a1, -a1, -a1, -a1, 1000 - 9 * a1, 0);
			break;

		case FilterEdgesAll:
			ApplyMatrix(undo, rect, -1, -1, -1, -1, 8, -1, -1, -1, -1, 1, 0);
			break;

		case FilterEdgesHorz:
			ApplyMatrix(undo, rect, -1, 0, 1, -1, 0, 1, -1, 0, 1, 1, 0);
			break;

		case FilterEdgesVert:
			ApplyMatrix(undo, rect, -1, -1, -1, 0, 0, 0, 1, 1, 1, 1, 0);
			break;

		case FilterEdgesDiag:
			ApplyMatrix(undo, rect, -1, 0, 0, 0, 0, 0, 0, 0, 1, 1, 0);
			//{
			//	FGXChannel edges;
			//	ApplyMatrix(edges, rect, -1, -1, -1, -1, 8, -1, -1, -1, -1, 1, 0);
			//	Swap(edges);
			//	ApplyMatrix(undo, rect, 1, 1, 1, 1, 1, 1, 1, 1, 1, 9, 0);

			//	FGXAccessor  p(*this, rect);
			//	FGXAccessorC e(edges, rect),
			//				 u(undo, rect);

			//	BeginLoopRect(rect, (p++, e++, u++))
			//		//*p = FGXBlend(*u, *e, *p);
			//		if (*e > 127) *p = *u;
			//	EndLoopRect((p.next(), e.next(), u.next()))
			//}
			break;
		}
	}
}

void FGXChannel::Adjust(FGXChannel &undo, const FGXChannel &selection, const CRect &rect, int type, int a1, int a2, int a3) {
	CRect common = position & selection.position & rect;

	Adjust(undo, common, type, a1, a2, a3);

	FGXAccessor  p(*this, common);
	FGXAccessorC u(undo, common);
	FGXAccessorC s(selection, common);

	BeginLoopRect(common, (p++, u++, s++))
		*p = FGXBlend(*p, *s, *u);
	EndLoopRect((p.next(), u.next(), s.next()))
}
