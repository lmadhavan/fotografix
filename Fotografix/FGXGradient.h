#pragma once

#include "FGXChannel.h"
#include "FGXLayer.h"

enum {
	GradientLinear,
	GradientRadial,
	GradientReflected
};

class FGXGradient {
public:
	FGXGradient(int type, const CRect &rect, const CPoint &start, const CPoint &end);

	void Render(FGXLayer &layer, const FGXColor &fg, const FGXColor &bg, int channelMask = ChannelAll) const;
	void Render(FGXLayer &layer, const FGXChannel &selection, const FGXColor &fg, const FGXColor &bg, int channelMask = ChannelAll) const;
	void Render(FGXLayer &layer, const FGXColor &fg) const;
	void Render(FGXLayer &layer, const FGXChannel &selection, const FGXColor &fg) const;

private:
	void RenderChannel(FGXChannel &channel, BYTE fg, BYTE bg) const;
	void RenderChannel(FGXChannel &channel, const FGXChannel &selection, BYTE fg, BYTE bg) const;

private:
	FGXChannel blend;
};
