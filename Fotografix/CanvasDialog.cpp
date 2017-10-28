// CanvasDialog.cpp : implementation file
//

#include "stdafx.h"
#include "Fotografix.h"
#include "CanvasDialog.h"

// CCanvasDialog dialog

IMPLEMENT_DYNAMIC(CCanvasDialog, CDialog)

CCanvasDialog::CCanvasDialog(CWnd* pParent /*=NULL*/)
	: CDialog(CCanvasDialog::IDD, pParent)
	, unit(0)
	, anchor(4)
	, relative(false)
{

}

CCanvasDialog::~CCanvasDialog()
{
}

void CCanvasDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_W1, w1);
	DDX_Text(pDX, IDC_H1, h1);
	DDX_Text(pDX, IDC_W2, w2);
	DDX_Text(pDX, IDC_H2, h2);
	DDX_CBIndex(pDX, IDC_UNIT, unit);
	DDX_CBIndex(pDX, IDC_ANCHOR, anchor);
	DDX_Check(pDX, IDC_RELATIVE, relative);
	DDV_MinMaxFloat(pDX, w2, 0.001f, 32000.0f);
	DDV_MinMaxFloat(pDX, h2, 0.001f, 32000.0f);
	DDX_Control(pDX, IDC_UNIT, unitBox);
}


BEGIN_MESSAGE_MAP(CCanvasDialog, CDialog)
	ON_CBN_SELCHANGE(IDC_UNIT, &CCanvasDialog::OnCbnSelchangeUnits)
	ON_BN_CLICKED(IDOK, &CCanvasDialog::OnBnClickedOk)
END_MESSAGE_MAP()


// CCanvasDialog message handlers

BOOL CCanvasDialog::OnInitDialog()
{
	CDialog::OnInitDialog();
	TranslateDialog(this);

	w2 = ow;
	h2 = oh;

	w1.Format(TEXT("%d %s"), ow, unitName);
	h1.Format(TEXT("%d %s"), oh, unitName);

	unitBox.InsertString(0, unitName);
	unitBox.SetCurSel(0);

	UpdateData(false);

	return true;
}

void CCanvasDialog::OnCbnSelchangeUnits()
{
	int oldUnit = unit;

	UpdateData(true);

	if (oldUnit == 0 && unit == 1) {
		w2 = w2 / ow * 100;
		h2 = h2 / oh * 100;
	} else if (oldUnit == 1 && unit == 0) {
		w2 = w2 * ow / 100;
		h2 = h2 * oh / 100;
	}

	UpdateData(false);
}

void CCanvasDialog::OnBnClickedOk()
{
	UpdateData(true);

	if (unit == 1) {
		w2 = w2 * ow / 100;
		h2 = h2 * oh / 100;
	}

	if (relative) {
		w2 += ow;
		h2 += oh;
	}

	w2 = max(1, w2);
	h2 = max(1, h2);

	UpdateData(false);
	OnOK();
}
