// Fotografix.h : main header file for the Fotografix application
//
#pragma once

#ifndef __AFXWIN_H__
	#error "include 'stdafx.h' before including this file for PCH"
#endif

#include "resource.h"       // main symbols
#include "ColorPicker.h"
#include "FGXLayer.h"
#include "DragListCtrl.h"
#include "Language.h"

// CFotografixApp:
// See Fotografix.cpp for the implementation of this class
//

class CFotografixApp : public CWinApp
{
public:
	CFotografixApp();

// Overrides
public:
	virtual BOOL InitInstance();
	virtual int ExitInstance();

// Implementation
	afx_msg void OnFileOpen();
	afx_msg void OnAppAbout();
	DECLARE_MESSAGE_MAP()

private:
	ULONG_PTR gdiToken;
	afx_msg void OnFileExtract();
	virtual BOOL OnIdle(LONG lCount);
	afx_msg void OnAppLanguage();
};

struct Globals {
	CString appPath;
	bool openExtract;

	int curTool;
	HCURSOR curCursor;

	CColorPicker fgColor;
	CColorPicker bgColor;

	int selStyle;
	int selW;
	int selH;

	int wandTolerance;
	bool wandContiguous;

	int gradColor;
	int gradType;

	int textSize;
	COLORREF textColor;
	int textStyle;
	CString textFace;

	CDragListCtrl layers;
	CComboBox channel;

	CTreeCtrl scripts;

	CImageList imageList;

	FGXLayer clipboard;
	FGXLayer brush;
};

extern CFotografixApp theApp;
extern Globals globals;
