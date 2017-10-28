// ChildFrm.cpp : implementation of the CChildFrame class
//
#include "stdafx.h"
#include "Fotografix.h"

#include "ChildFrm.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// CChildFrame

IMPLEMENT_DYNCREATE(CChildFrame, CMDIChildWnd)

BEGIN_MESSAGE_MAP(CChildFrame, CMDIChildWnd)
	ON_WM_GETMINMAXINFO()
	ON_WM_MDIACTIVATE()
END_MESSAGE_MAP()


// CChildFrame construction/destruction

CChildFrame::CChildFrame()
{
	// TODO: add member initialization code here
}

CChildFrame::~CChildFrame()
{
}


BOOL CChildFrame::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: Modify the Window class or styles here by modifying the CREATESTRUCT cs
	if( !CMDIChildWnd::PreCreateWindow(cs) )
		return FALSE;

	return TRUE;
}


// CChildFrame diagnostics

#ifdef _DEBUG
void CChildFrame::AssertValid() const
{
	CMDIChildWnd::AssertValid();
}

void CChildFrame::Dump(CDumpContext& dc) const
{
	CMDIChildWnd::Dump(dc);
}

#endif //_DEBUG


// CChildFrame message handlers

void CChildFrame::OnGetMinMaxInfo(MINMAXINFO *pmmi)
{
	if (pmmi->ptMaxTrackSize.x < pmmi->ptMaxSize.x || pmmi->ptMaxTrackSize.y < pmmi->ptMaxSize.y) {
		// Should only hit here on Vista Aero
		pmmi->ptMaxTrackSize.x = max(pmmi->ptMaxTrackSize.x, pmmi->ptMaxSize.x);
		pmmi->ptMaxTrackSize.y = max(pmmi->ptMaxTrackSize.y, pmmi->ptMaxSize.y);

		if (IsZoomed())
			ModifyStyle(WS_CAPTION, 0);
		else
			ModifyStyle(0, WS_CAPTION);
	} else {
		// Needed when the test above fails on Aero; w/o this
		// you'll eventually end up with non-maximized child windows
		// that do NOT have a caption after forcing the caption off above
		if (IsZoomed())
			ModifyStyle(WS_CAPTION, 0);
		else
			ModifyStyle(0, WS_CAPTION);

		CMDIChildWnd::OnGetMinMaxInfo(pmmi);
	}
}

void CChildFrame::OnMDIActivate(BOOL bActivate, CWnd* pActivateWnd, CWnd* pDeactivateWnd)
{
	CMDIChildWnd::OnMDIActivate(bActivate, pActivateWnd, pDeactivateWnd);

	if (bActivate) {
		GetWindow(GW_CHILD)->SetTimer(1, 100, NULL);
		GetWindow(GW_CHILD)->SendMessage(WM_APP, 0, 0);
		globals.channel.EnableWindow(true);
	} else {
		GetWindow(GW_CHILD)->KillTimer(1);
		globals.layers.DeleteAllItems();
		globals.channel.EnableWindow(false);
	}
}
