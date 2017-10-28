// FillDialog.cpp : implementation file
//

#include "stdafx.h"
#include "Fotografix.h"
#include "FillDialog.h"

// CFillDialog dialog

IMPLEMENT_DYNAMIC(CFillDialog, CDialog)

CFillDialog::CFillDialog(CWnd* pParent /*=NULL*/)
	: CDialog(CFillDialog::IDD, pParent)
	, colour(0)
	, opacity(100)
{
	CWinApp *app = AfxGetApp();
	colour = app->GetProfileInt(TEXT("Fill"), TEXT("Colour"), 0);
	opacity = app->GetProfileInt(TEXT("Fill"), TEXT("Opacity"), 100);
}

CFillDialog::~CFillDialog()
{
	CWinApp *app = AfxGetApp();
	app->WriteProfileInt(TEXT("Fill"), TEXT("Colour"), colour);
	app->WriteProfileInt(TEXT("Fill"), TEXT("Opacity"), opacity);
}

void CFillDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_CBIndex(pDX, IDC_COLOUR, colour);
	DDX_Text(pDX, IDC_OPACITY, opacity);
	DDV_MinMaxInt(pDX, opacity, 0, 100);
}


BEGIN_MESSAGE_MAP(CFillDialog, CDialog)
END_MESSAGE_MAP()


// CFillDialog message handlers

BOOL CFillDialog::OnInitDialog()
{
	CDialog::OnInitDialog();
	TranslateDialog(this);

	return TRUE;
}
