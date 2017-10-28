// DragListCtrl.cpp : implementation file
//

#include "stdafx.h"
#include "Fotografix.h"
#include "DragListCtrl.h"


// CDragListCtrl

IMPLEMENT_DYNAMIC(CDragListCtrl, CListCtrl)

CDragListCtrl::CDragListCtrl()
{
	fromItem = toItem = -1;
	dragCursor = AfxGetApp()->LoadStandardCursor(MAKEINTRESOURCE(IDC_SIZEALL));
}

CDragListCtrl::~CDragListCtrl()
{
}


BEGIN_MESSAGE_MAP(CDragListCtrl, CListCtrl)
	ON_WM_MOUSEMOVE()
	ON_WM_LBUTTONUP()
	ON_NOTIFY_REFLECT(LVN_BEGINDRAG, &CDragListCtrl::OnLvnBegindrag)
END_MESSAGE_MAP()



// CDragListCtrl message handlers


void CDragListCtrl::OnMouseMove(UINT nFlags, CPoint point)
{
	if (fromItem > -1) {
		UINT flags;
		toItem = HitTest(point, &flags);
		SetItemState(toItem, LVIS_FOCUSED, LVIS_FOCUSED);

		if ((flags & LVHT_ABOVE) > 0)
			EnsureVisible(GetTopIndex() - 1, false);
		else if ((flags & LVHT_BELOW) > 0)
			EnsureVisible(GetTopIndex() + GetCountPerPage(), false);
	} else
		CListCtrl::OnMouseMove(nFlags, point);
}

void CDragListCtrl::OnLButtonUp(UINT nFlags, CPoint point)
{
	if (fromItem > -1) {
		ReleaseCapture();

		if (toItem > -1 && toItem != fromItem) {
			NMDRAGINFO DI = { { m_hWnd, GetDlgCtrlID(), LVNEX_FINISHDRAG }, fromItem, toItem };
			GetParent()->SendMessage(WM_NOTIFY, GetDlgCtrlID(), (LPARAM)&DI);
		}

		fromItem = -1;
	}

	CListCtrl::OnLButtonUp(nFlags, point);
}

void CDragListCtrl::OnLvnBegindrag(NMHDR *pNMHDR, LRESULT *pResult)
{
	NMLISTVIEW *pLV = reinterpret_cast<LPNMLISTVIEW>(pNMHDR);

	fromItem = pLV->iItem;
	SetCapture();
	SetCursor(dragCursor);
	SetFocus();
}
