// TransformTracker.cpp : implementation file
//

#include "stdafx.h"
#include "Fotografix.h"
#include "TransformTracker.h"


// CTransformTracker

bool CTransformTracker::Track(CWnd *pWnd, CPoint point) {
	aspectRatio = float(m_rect.Width()) / m_rect.Height();
	return CRectTracker::Track(pWnd, point, true, NULL);
}

void CTransformTracker::AdjustRect(int nHandle, LPRECT lpRect) {
	CRectTracker::AdjustRect(nHandle, lpRect);

	if ((GetKeyState(VK_SHIFT) & 0x8000) > 0)
		switch (nHandle) {
		case hitTopLeft:
		case hitTopRight:
			lpRect->top = lpRect->bottom - (lpRect->right - lpRect->left) / aspectRatio;
			break;

		case hitBottomLeft:
		case hitBottomRight:
			lpRect->bottom = lpRect->top + (lpRect->right - lpRect->left) / aspectRatio;
			break;
		}
}
