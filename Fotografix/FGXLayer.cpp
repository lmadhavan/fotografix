#include "StdAfx.h"
#include "Fotografix.h"
#include "FGXLayer.h"
#include "FGXImage.h"

#include "FGX.h"
#include "FotografixDoc.h"

void FGXLayer::SetType(int type) {
	switch (this->type = type) {
	case LayerNormal:
	case LayerBrush:
	case LayerText:
		numChannels = 4;
		break;

	case LayerAdjust:
		numChannels = 1;
		break;

	default:
		numChannels = 0;
		break;
	}
}

void FGXLayer::SetPosition(const CRect &newPosition) {
	position = newPosition;

	for (int i = 0; i < numChannels; i++) {
		channels[i].Free();
		channels[i].SetPosition(position);
		channels[i].Allocate();
	}

	if (type == LayerAdjust)
		channels[0].Fill(255);
}

void FGXLayer::MoveTo(long left, long top) {
	for (int i = 0; i < numChannels; i++)
		channels[i].MoveTo(left, top);

	position = channels[0].GetPosition();
}

void FGXLayer::ExpandTo(FGXLayer &undo, const CRect &newPosition) {
	Transfer(undo);

	position = newPosition;

	if (numChannels == 1)
		channels[0].ExpandTo(undo.channels[0], newPosition, true);
	else if (HasMask()) {
		for (int i = 0; i < 4; i++)
			channels[i].ExpandTo(undo.channels[i], newPosition);

		channels[4].ExpandTo(undo.channels[4], newPosition, true);
	} else
		for (int i = 0; i < numChannels; i++)
			channels[i].ExpandTo(undo.channels[i], newPosition);
}

void FGXLayer::ResizeTo(FGXLayer &undo, const CRect &newPosition) {
	Transfer(undo);

	position = newPosition;

	for (int i = 0; i < numChannels; i++)
		channels[i].ResizeTo(undo.channels[i], newPosition);
}

void FGXLayer::Flip(bool horizontal, bool vertical) {
	CWaitCursor wait;

	for (int i = 0; i < numChannels; i++)
		channels[i].Flip(horizontal, vertical);
}

void FGXLayer::Rotate(FGXLayer &undo, int angle) {
	CWaitCursor wait;

	Transfer(undo);

	for (int i = 0; i < numChannels; i++)
		channels[i].Rotate(undo.channels[i], angle);

	position = channels[0].position;
}

void FGXLayer::Render(FGXBitmap &bitmap, const CRect &rect, int channelMask) const {
	if (type == LayerNormal)
		for (int i = 1; i < 4; i++)
			if (channelMask & (1 << i))
				channels[i].Render(bitmap, 3 - i, channels[0], opacity, rect);
}

void FGXLayer::AlphaRender(FGXLayer &layer, const CRect &rect, int channelMask) const {
	if (type == LayerAdjust)
		layer.Adjust(FGXLayer(), channels[0], rect, a, a1, a2, a3, channelMask);
	else {
		if (layer.numChannels == 1)
			channels[1].AlphaRenderMask(layer.channels[0], channels[0], opacity, rect);
		else if (HasMask() && IsMaskEnabled()) {
			for (int i = 1; i < 4; i++)
				if (channelMask & (1 << i))
					channels[i].AlphaRender(layer.channels[i], layer.channels[0], channels[0], channels[4], opacity, mode, rect);

			channels[0].AlphaSquare(layer.channels[0], channels[4], opacity, rect);
		} else if (channelMask == ChannelMask)
			channels[1].AlphaRenderMask(layer.channels[4], channels[0], opacity, rect);
		else {
			for (int i = 1; i < 4; i++)
				if (channelMask & (1 << i))
					channels[i].AlphaRender(layer.channels[i], layer.channels[0], channels[0], opacity, mode, rect);

			channels[0].AlphaSquare(layer.channels[0], opacity, rect);
		}
	}
}

void FGXLayer::Erase(FGXLayer &layer, const CRect &rect) const {
	channels[0].Erase(layer.channels[0], opacity, rect);
}

void FGXLayer::ConvertToGray(int channelMask) {
	if (type != LayerAdjust) {
		switch (channelMask) {
		case ChannelRed:
			channels[1].Clone(channels[2]);
			channels[1].Clone(channels[3]);
			break;

		case ChannelBlue:
			channels[2].Clone(channels[1]);
			channels[2].Clone(channels[3]);
			break;

		case ChannelGreen:
			channels[3].Clone(channels[1]);
			channels[3].Clone(channels[2]);
			break;
		}
	}
}

void FGXLayer::LoadFromMemory(const void *data, int numChannels) {
	switch (numChannels) {
	case 3:
		channels[0].Fill(255);
		for (int i = 1; i < 4; i++)
			channels[i].LoadFromMemory(reinterpret_cast<const BYTE *>(data) + i - 1, 3);
		break;

	case 4:
		for (int i = 0; i < 4; i++)
			channels[i].LoadFromMemory(reinterpret_cast<const BYTE *>(data) + 3 - i, 4);
		break;
	}
}

void FGXLayer::SaveToMemory(void *data, int numChannels) const {
	switch (numChannels) {
	case 3:
		for (int i = 1; i < 4; i++)
			channels[i].SaveToMemory(reinterpret_cast<BYTE *>(data) + i - 1, 3);
		break;

	case 4:
		for (int i = 0; i < 4; i++)
			channels[i].SaveToMemory(reinterpret_cast<BYTE *>(data) + 3 - i, 4);
		break;
	}
}

void FGXLayer::Clone(FGXLayer &result) const {
	CopyProps(result);

	for (int i = 0; i < numChannels; i++)
		channels[i].Clone(result.channels[i]);
}

void FGXLayer::CopyProps(FGXLayer &result) const {
	result.image = image;
	result.position = position;
	result.numChannels = numChannels;
	result.SetName(name);
	result.SetOpacity(opacity);
	result.SetVisible(visible);

	result.mask = mask;
	result.type = type;
	result.a = a;
	result.a1 = a1;
	result.a2 = a2;
	result.a3 = a3;
	result.a4 = a4;
	result.a5 = a5;
}

void FGXLayer::CopyRect(FGXLayer &result, const CRect &rect, int channelMask) const {
	if (HasMask() && IsMaskEnabled()) {
		for (int i = 1; i < 4; i++)
			if (channelMask & (1 << i))
				channels[i].CopyRect(result.channels[i], channels[4], 255, rect);

		if (channelMask & ChannelAlpha)
			channels[0].CopyRect(result.channels[0], channels[4], opacity, rect);
	} else {
		for (int i = 1; i < 4; i++)
			if (channelMask & (1 << i))
				channels[i].CopyRect(result.channels[i], 255, rect);

		if (channelMask & ChannelAlpha)
			channels[0].CopyRect(result.channels[0], opacity, rect);
	}
}

void FGXLayer::CopySelection(FGXLayer &result, const FGXSelection &selection, int channelMask) const {
	for (int i = 1; i < 4; i++)
		if (channelMask & (1 << i))
			channels[i].CopyRect(result.channels[i], 255, selection.GetPosition());

	if (channelMask == ChannelAll)
		channels[0].MultiplyChannel(result.channels[0], selection);
	else
		result.channels[0].Clear(selection, 255);
}

void FGXLayer::Clear(FGXLayer &undo, const FGXChannel &selection, FGXColor color, int channelMask) {
	Clone(undo);

	if (numChannels == 1)
		channels[0].Clear(selection, color.r);
	else {
		if (channelMask == ChannelAll)
			channels[0].Clear(selection, color.a);
		if (channelMask & ChannelRed)
			channels[1].Clear(selection, color.r);
		if (channelMask & ChannelGreen)
			channels[2].Clear(selection, color.g);
		if (channelMask & ChannelBlue)
			channels[3].Clear(selection, color.b);
		if ((channelMask & ChannelMask) && HasMask())
			channels[4].Clear(selection, color.a);
	}
}

void FGXLayer::Fill(FGXColor color, int channelMask) {
	switch (type) {
	case LayerNormal:
		if (channelMask == ChannelAll) channels[0].Fill(color.a);
		if (channelMask & ChannelRed) channels[1].Fill(color.r);
		if (channelMask & ChannelGreen) channels[2].Fill(color.g);
		if (channelMask & ChannelBlue) channels[3].Fill(color.b);
		break;

	case LayerBrush:
		channels[1].Fill(color.r);
		channels[2].Fill(color.g);
		channels[3].Fill(color.b);
		break;

	case LayerAdjust:
		channels[0].Fill(color.r);
		break;
	}
}

void FGXLayer::Transfer(FGXLayer &result) {
	CopyProps(result);
}

void FGXLayer::Swap(FGXLayer &other) {
	swap(image, other.image);
	swap(position, other.position);
	swap(numChannels, other.numChannels);
	swap(opacity, other.opacity);
	swap(visible, other.visible);
	swap(mask, other.mask);

	swap(type, other.type);
	swap(a, other.a);
	swap(a1, other.a1);
	swap(a2, other.a2);
	swap(a3, other.a3);
	swap(a4, other.a4);
	swap(a5, other.a5);

	TCHAR temp[32];
	_tcscpy(temp, name);
	_tcscpy(name, other.name);
	_tcscpy(other.name, temp);

	for (int i = 0; i < 5; i++)
		channels[i].Swap(other.channels[i]);
}

void FGXLayer::SetMask(const FGXChannel &mask, const CRect &rect) {
	EnsureRect(rect);

	if (type == LayerAdjust) {
		channels[0].Fill(0);
		mask.CopyRect(channels[0], 255, rect);
	} else if (HasMask()) {
		channels[4].Fill(0);
		mask.CopyRect(channels[4], 255, rect);
	}
}

void FGXLayer::Adjust(FGXLayer &undo, const CRect &rect, int type, int a1, int a2, int a3, int channelMask) {
	CWaitCursor wait;

	Transfer(undo);

	switch (type) {
	case AdjustColorBalance:
		if (numChannels >= 4 && channelMask == ChannelAll) {
			channels[0].Clone(undo.channels[0]);
			channels[1].Adjust(undo.channels[1], rect, AdjustBrightnessContrast, a1, 0, 0);
			channels[2].Adjust(undo.channels[2], rect, AdjustBrightnessContrast, a2, 0, 0);
			channels[3].Adjust(undo.channels[3], rect, AdjustBrightnessContrast, a3, 0, 0);
			channels[4].Clone(undo.channels[4]);
		}
		return;

	case AdjustThreshold:
		if (numChannels >= 4 && channelMask == ChannelAll) {
			for (int i = 0; i < numChannels; i++)
				channels[i].Clone(undo.channels[i]);

			CRect common = position & rect;

			FGXAccessor r(channels[1], common);
			FGXAccessor g(channels[2], common);
			FGXAccessor b(channels[3], common);

			BYTE a = a1;

			BeginLoopRect(common, (r++, g++, b++))
				*r = *g = *b = FGXGrayscale(*r, *g, *b) >= a ? 255 : 0;
			EndLoopRect((r.next(), g.next(), b.next()))
		}
		return;

	case AdjustDesaturate:
		if (numChannels >= 4 && channelMask == ChannelAll) {
			for (int i = 0; i < numChannels; i++)
				channels[i].Clone(undo.channels[i]);

			CRect common = position & rect;

			FGXAccessor r(channels[1], common);
			FGXAccessor g(channels[2], common);
			FGXAccessor b(channels[3], common);

			BeginLoopRect(common, (r++, g++, b++))
				*r = *g = *b = FGXGrayscale(*r, *g, *b);
			EndLoopRect((r.next(), g.next(), b.next()))
		}
		return;

	case AdjustBlackWhite:
		if (numChannels >= 4 && channelMask == ChannelAll) {
			for (int i = 0; i < numChannels; i++)
				channels[i].Clone(undo.channels[i]);

			CRect common = position & rect;

			FGXAccessor r(channels[1], common);
			FGXAccessor g(channels[2], common);
			FGXAccessor b(channels[3], common);

			BeginLoopRect(common, (r++, g++, b++))
				*r = *g = *b = FGXGrayscaleEx(*r, *g, *b, a1, a2, a3);
			EndLoopRect((r.next(), g.next(), b.next()))
		}
		return;

	case AdjustGradientMap:
		if (numChannels >= 4 && channelMask == ChannelAll) {
			for (int i = 0; i < numChannels; i++)
				channels[i].Clone(undo.channels[i]);

			CRect common = position & rect;

			FGXAccessor r(channels[1], common);
			FGXAccessor g(channels[2], common);
			FGXAccessor b(channels[3], common);

			FGXColor c1(a1, 255),
					 c2(a2, 255);

			BeginLoopRect(common, (r++, g++, b++)) {
				register BYTE l = FGXGrayscale(*r, *g, *b);
				*r = FGXBlend(c1.r, l, c2.r);
				*g = FGXBlend(c1.g, l, c2.g);
				*b = FGXBlend(c1.b, l, c2.b);
			} EndLoopRect((r.next(), g.next(), b.next()))
		}
		return;

//	case AdjustHueSaturation:
//		if (numChannels >= 4 && channelMask == ChannelAll) {
//			for (int i = 0; i < numChannels; i++)
//				channels[i].Clone(undo.channels[i]);
//
//			CRect common = position & rect;
//			short sat = short(a2),
//				  hue = short(a1);
//
//			FGXAccessor r(channels[1], common);
//			FGXAccessor g(channels[2], common);
//			FGXAccessor b(channels[3], common);
//
//			BeginLoopRect(common, (r++, g++, b++)) {
//				BYTE M = *r, m = *b, C, S, F;
//				short H;
//				if (*g > M) M = *g;
//				if (*b > M) M = *b;
//				if (*g < m) m = *g;
//				if (*r < m) m = *r;
//				C = M - m;
//
//				if (C > 0) {
//					S = BYTE(clamp<short>(FGXDivide(C, M) + sat, 0, 255));
//
//					if (M == *r) {
//						H = (short(*g) - *b) * 60 / C;
//						if (H < 0) H += 360;
//					} else if (M == *g)
//						H = 120 + (short(*b) - *r) * 60 / C;
//					else
//						H = 240 + (short(*r) - *g) * 60 / C;
//					H += hue;
//					if (H < 0) H += 360;
//					else if (H >= 360) H -= 360;
//
//					F = H % 60;
//
//#define V	M
//#define P	FGXMultiply(V, 255 - S)
//#define Q	FGXMultiply(V, 255 - FGXMultiply(S, F))
//#define T	FGXMultiply(V, 255 - FGXMultiply(S, 255 - F))
//#define SET(rv, gv, bv)	*r = rv, *g = gv, *b = bv
//					switch (H / 60) {
//					case 0:
//						SET(V, T, P);
//						break;
//					case 1:
//						SET(Q, V, P);
//						break;
//					case 2:
//						SET(P, V, T);
//						break;
//					case 3:
//						SET(P, Q, V);
//						break;
//					case 4:
//						SET(T, P, V);
//						break;
//					default:
//						SET(V, P, Q);
//						break;
//					}
//#undef SET
//#undef T
//#undef Q
//#undef P
//#undef V
//				}
//			} EndLoopRect((r.next(), g.next(), b.next()))
//		}
//		return;

	case FilterNightVision:
		if (numChannels >= 4 && channelMask == ChannelAll) {
			for (int i = 0; i < numChannels; i++)
				channels[i].Clone(undo.channels[i]);

			CRect common = position & rect;

			FGXAccessor r(channels[1], common);
			FGXAccessor g(channels[2], common);
			FGXAccessor b(channels[3], common);

			BeginLoopRect(common, (r++, g++, b++))
				*g = FGXSaturation(*r, *g, *b), *r = *b = 0;
			EndLoopRect((r.next(), g.next(), b.next()))
		}
		return;
	}

	if (numChannels == 1)
		channels[0].Adjust(undo.channels[0], rect, type, a1, a2, a3);
	else {
		if (type == FilterBlur || type == FilterMotionBlur || type == FilterGaussianBlur || type == FilterShear) {
			if (image == NULL)
				return;

			Clone(undo);
			EnsureRect(CRect(0, 0, image->GetWidth(), image->GetHeight()));

			for (int i = 1; i < numChannels; i++)
				if (channelMask & (1 << i))
					channels[i].Adjust(FGXChannel(), rect, type, a1, a2, a3);

			channels[0].Adjust(FGXChannel(), rect, type, a1, a2, a3);
		} else {
			for (int i = 1; i < numChannels; i++)
				if (channelMask & (1 << i))
					channels[i].Adjust(undo.channels[i], rect, type, a1, a2, a3);
				else
					channels[i].Clone(undo.channels[i]);

			if (type == FilterOffset)
				channels[0].Adjust(undo.channels[0], rect, type, a1, a2, a3);
			else
				channels[0].Clone(undo.channels[0]);
		}
	}
}

void FGXLayer::Adjust(FGXLayer &undo, const FGXChannel &selection, const CRect &rect, int type, int a1, int a2, int a3, int channelMask) {
	CWaitCursor wait;

	CRect common = position & selection.position & rect;

	switch (type) {
	case AdjustColorBalance:
	case AdjustThreshold:
	case AdjustDesaturate:
	case AdjustBlackWhite:
	case AdjustGradientMap:
	case FilterNightVision:
		if (numChannels >= 4 && channelMask == ChannelAll) {
			Adjust(undo, common, type, a1, a2, a3, ChannelAll);

			FGXAccessor  pr(channels[1], common);
			FGXAccessor  pg(channels[2], common);
			FGXAccessor  pb(channels[3], common);

			FGXAccessorC ur(undo.channels[1], common);
			FGXAccessorC ug(undo.channels[2], common);
			FGXAccessorC ub(undo.channels[3], common);

			FGXAccessorC s(selection, common);

			BeginLoopRect(common, (pr++, pg++, pb++, ur++, ug++, ub++, s++)) {
				*pr = FGXBlend(*pr, *s, *ur);
				*pg = FGXBlend(*pg, *s, *ug);
				*pb = FGXBlend(*pb, *s, *ub);
			} EndLoopRect((pr.next(), pg.next(), pb.next(), ur.next(), ug.next(), ub.next(), s.next()))
		}
		return;
	}

	Transfer(undo);

	if (numChannels == 1)
		channels[0].Adjust(undo.channels[0], selection, rect, type, a1, a2, a3);
	else {
		if (type == FilterBlur || type == FilterMotionBlur || type == FilterShear) {
			if (image == NULL)
				return;

			Clone(undo);
			EnsureRect(CRect(0, 0, image->GetWidth(), image->GetHeight()));

			for (int i = 1; i < numChannels; i++)
				if (channelMask & (1 << i))
					channels[i].Adjust(FGXChannel(), selection, rect, type, a1, a2, a3);

			channels[0].Adjust(FGXChannel(), selection, rect, type, a1, a2, a3);
		} else {
			for (int i = 1; i < numChannels; i++)
				if (channelMask & (1 << i))
					channels[i].Adjust(undo.channels[i], selection, rect, type, a1, a2, a3);
				else
					channels[i].Clone(undo.channels[i]);

			if (type == FilterOffset)
				channels[0].Adjust(undo.channels[0], selection, rect, type, a1, a2, a3);
			else
				channels[0].Clone(undo.channels[0]);
		}
	}
}

void FGXLayer::ConvertToBrush() {
	channels[1].Clone(channels[0]);
	channels[0].Adjust(FGXChannel(), GetPosition(), AdjustInvert, 0, 0, 0);
	type = LayerBrush;
}

void FGXLayer::RenderText() {
	if (type == LayerText) {
		Font font(a4, a1, a3);
		RectF rect;

		// Measure the string dimensions
		{
			Graphics g(::GetDC(NULL));
			g.MeasureString(a5, -1, &font, PointF(0, 0), &rect);
		}

		// Render the string onto a bitmap and load it into the layer
		{
			Bitmap bitmap(rect.Width, rect.Height, PixelFormat32bppARGB);
			Graphics g(&bitmap);
			if (a3 & 128) g.SetTextRenderingHint(Gdiplus::TextRenderingHint::TextRenderingHintAntiAliasGridFit);
			g.DrawString(a5, -1, &font, PointF(0, 0), &Gdiplus::SolidBrush(Gdiplus::Color(GetRValue(a2), GetGValue(a2), GetBValue(a2))));

			CPoint pt = position.TopLeft();
			CFotografixDoc::LoadImage_Bitmap(bitmap, *this);
			Compact();
			MoveTo(pt.x, pt.y);
		}
	}
}
