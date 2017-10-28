#pragma once


// CTransformTracker

class CTransformTracker : public CRectTracker
{
public:
	bool Track(CWnd *pWnd, CPoint point);
	void AdjustRect(int nHandle, LPRECT lpRect);

private:
	float aspectRatio;
};
