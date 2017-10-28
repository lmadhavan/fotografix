// ColorPicker.cpp : implementation file
//

#include "stdafx.h"
#include "Fotografix.h"
#include "ColorPicker.h"


// CColorPicker

IMPLEMENT_DYNAMIC(CColorPicker, CStatic)

CColorPicker::CColorPicker()
{
	color = 0;
	brush = NULL;
}

CColorPicker::~CColorPicker()
{
	if (brush != NULL) ::DeleteObject(brush);
}

void CColorPicker::SetColor(COLORREF newColor) {
	if (brush != NULL) ::DeleteObject(brush);
	brush = ::CreateSolidBrush(color = newColor);
	GetParent()->Invalidate();
}

BEGIN_MESSAGE_MAP(CColorPicker, CStatic)
	ON_WM_CTLCOLOR_REFLECT()
	ON_CONTROL_REFLECT(STN_CLICKED, &CColorPicker::OnStnClicked)
END_MESSAGE_MAP()



// CColorPicker message handlers

HBRUSH CColorPicker::CtlColor(CDC* /*pDC*/, UINT /*nCtlColor*/)
{
	return brush;
}

void CColorPicker::OnStnClicked()
{
	CColorDialog dlg(color, CC_FULLOPEN, this);
	if (dlg.DoModal() == IDOK) {
		SetColor(dlg.GetColor());
		GetParent()->SendMessage(WM_COMMAND, GetDlgCtrlID(), NULL);
	}
}
