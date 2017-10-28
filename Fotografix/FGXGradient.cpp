#include "StdAfx.h"
#include "FGXGradient.h"

#include "FGX.h"

#include <cmath>

FGXGradient::FGXGradient(int type, const CRect &rect, const CPoint &start, const CPoint &end) {
	blend.SetPosition(rect);
	blend.Allocate();

	FGXAccessor p(blend, rect);

	float dx = end.x - start.x,
		  dy = start.y - end.y;

	switch (type) {
	case GradientLinear:
		{
			float d = sqrt(dx*dx + dy*dy);
			if (start.y < end.y || start.y == end.y && start.x > end.x) d = -d;

			float m = dx / dy,
				  c = sqrt(m*m + 1) * d;

			if (start.y == end.y) {
				BeginLoopRect(rect, p++) {
					float f = 1.0f - (i - start.x) / d;
					if (f < 0.0f) f = 0.0f; else if (f > 1.0f) f = 1.0f;
					*p = 255 * f;
				} EndLoopRect(p.next())
			} else {
				BeginLoopRect(rect, p++) {
					float f = 1.0f - (m * (i - start.x) - (j - start.y)) / c;
					if (f < 0.0f) f = 0.0f; else if (f > 1.0f) f = 1.0f;
					*p = 255 * f;
				} EndLoopRect(p.next())
			}
		}
		break;

	case GradientRadial:
		{
			float r2 = dx*dx + dy*dy;

			BeginLoopRect(rect, p++)
				*p = 255 * (1.0f - min(1.0f, ((i - start.x) * (i - start.x) + (j - start.y) * (j - start.y)) / r2));
			EndLoopRect(p.next())
		}
		break;

	case GradientReflected:
		{
			float d = sqrt(dx*dx + dy*dy);
			if (start.y < end.y || start.y == end.y && start.x > end.x) d = -d;

			float m = dx / dy,
				  c = sqrt(m*m + 1) * d;

			if (start.y == end.y) {
				BeginLoopRect(rect, p++) {
					float f = 2.0f * fabs(0.5f - (i - start.x) / d);
					if (f < 0.0f) f = 0.0f; else if (f > 1.0f) f = 1.0f;
					*p = 255 * f;
				} EndLoopRect(p.next())
			} else {
				BeginLoopRect(rect, p++) {
					float f = 2.0f * fabs(0.5f - (m * (i - start.x) - (j - start.y)) / c);
					if (f < 0.0f) f = 0.0f; else if (f > 1.0f) f = 1.0f;
					*p = 255 * f;
				} EndLoopRect(p.next())
			}
		}
		break;
	}
}

void FGXGradient::Render(FGXLayer &layer, const FGXColor &fg, const FGXColor &bg, int channelMask) const {
	layer.EnsureRect(blend.GetPosition());

	if (layer.numChannels == 1)
		RenderChannel(layer.channels[0], fg.r, bg.r);
	else if (channelMask == ChannelMask)
		RenderChannel(layer.channels[4], fg.r, bg.r);
	else {
		if (channelMask & ChannelRed)
			RenderChannel(layer.channels[1], fg.r, bg.r);
		if (channelMask & ChannelGreen)
			RenderChannel(layer.channels[2], fg.g, bg.g);
		if (channelMask & ChannelBlue)
			RenderChannel(layer.channels[3], fg.b, bg.b);

		if (channelMask == ChannelAll) {
			CRect common = blend.GetPosition() & layer.GetPosition();

			FGXAccessor a(layer.channels[0], common);

			BeginLoopRect(common, a++)
				*a = 255;
			EndLoopRect(a.next())
		}
	}
}

void FGXGradient::Render(FGXLayer &layer, const FGXChannel &selection, const FGXColor &fg, const FGXColor &bg, int channelMask) const {
	layer.EnsureRect(blend.GetPosition() & selection.GetPosition());

	if (layer.numChannels == 1)
		RenderChannel(layer.channels[0], selection, fg.r, bg.r);
	else if (channelMask == ChannelMask)
		RenderChannel(layer.channels[4], selection, fg.r, bg.r);
	else {
		if (channelMask & ChannelRed)
			RenderChannel(layer.channels[1], selection, fg.r, bg.r);
		if (channelMask & ChannelGreen)
			RenderChannel(layer.channels[2], selection, fg.g, bg.g);
		if (channelMask & ChannelBlue)
			RenderChannel(layer.channels[3], selection, fg.b, bg.b);

		if (channelMask == ChannelAll) {
			CRect common = blend.GetPosition() & selection.GetPosition() & layer.GetPosition();

			FGXAccessorC s(selection, common);
			FGXAccessor  a(layer.channels[0], common);

			BeginLoopRect(common, (s++, a++))
				*a = FGXScreen(*a, *s);
			EndLoopRect((s.next(), a.next()))
		}
	}
}

void FGXGradient::Render(FGXLayer &layer, const FGXColor &fg) const {
	layer.EnsureRect(blend.GetPosition());

	layer.channels[1].Fill(fg.r);
	layer.channels[2].Fill(fg.g);
	layer.channels[3].Fill(fg.b);
	blend.CopyRect(layer.channels[0], 255, blend.GetPosition());
}

void FGXGradient::Render(FGXLayer &layer, const FGXChannel &selection, const FGXColor &fg) const {
	layer.EnsureRect(blend.GetPosition() & selection.GetPosition());

	layer.channels[1].Clear(selection, fg.r);
	layer.channels[2].Clear(selection, fg.g);
	layer.channels[3].Clear(selection, fg.b);

	CRect common = blend.GetPosition() & selection.GetPosition() & layer.GetPosition();

	FGXAccessorC b(blend, common),
				 s(selection, common);
	FGXAccessor  a(layer.channels[0], common);

	BeginLoopRect(common, (b++, s++, a++))
		*a = FGXBlend(*b, *s, *a);
	EndLoopRect((b.next(), s.next(), a.next()))
}

void FGXGradient::RenderChannel(FGXChannel &channel, BYTE fg, BYTE bg) const {
	CRect common = blend.GetPosition() & channel.GetPosition();

	FGXAccessorC b(blend, common);
	FGXAccessor  c(channel, common);

	BeginLoopRect(common, (b++, c++))
		*c = FGXBlend(fg, *b, bg);
	EndLoopRect((b.next(), c.next()))
}

void FGXGradient::RenderChannel(FGXChannel &channel, const FGXChannel &selection, BYTE fg, BYTE bg) const {
	CRect common = blend.GetPosition() & selection.GetPosition() & channel.GetPosition();

	FGXAccessorC b(blend, common),
				 s(selection, common);
	FGXAccessor  c(channel, common);

	BeginLoopRect(common, (b++, s++, c++))
		*c = FGXBlend(FGXBlend(fg, *b, bg), *s, *c);
	EndLoopRect((b.next(), s.next(), c.next()))
}
