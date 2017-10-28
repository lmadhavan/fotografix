// RulerDialog.cpp : implementation file
//

#include "stdafx.h"
#include "Fotografix.h"
#include "RulerDialog.h"


// CRulerDialog dialog

IMPLEMENT_DYNAMIC(CRulerDialog, CDialog)

int CRulerDialog::count = 0;

CRulerDialog::CRulerDialog(CWnd* pParent /*=NULL*/)
	: CDialog(CRulerDialog::IDD, pParent)
{
	name.Format(TEXT("%s %d"), LangItem(Ruler), ++count);
}

CRulerDialog::~CRulerDialog()
{
}

void CRulerDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_NAME, name);
	DDX_Text(pDX, IDC_PT1, point1);
	DDX_Text(pDX, IDC_PT2, point2);
	DDX_Text(pDX, IDC_DISTANCE, distance);
	DDX_Text(pDX, IDC_ANGLE, angle);
}


BEGIN_MESSAGE_MAP(CRulerDialog, CDialog)
	ON_BN_CLICKED(IDOK, &CRulerDialog::OnBnClickedOk)
	ON_BN_CLICKED(IDCANCEL, &CRulerDialog::OnBnClickedCancel)
END_MESSAGE_MAP()


// CRulerDialog message handlers

void CRulerDialog::OnBnClickedOk()
{
}

void CRulerDialog::OnBnClickedCancel()
{
	DestroyWindow();
}

void CRulerDialog::PostNcDestroy()
{
	delete this;
}

BOOL CRulerDialog::OnInitDialog()
{
	CDialog::OnInitDialog();
	TranslateDialog(this);

	return TRUE;
}
