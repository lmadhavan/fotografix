// TextDialog.cpp : implementation file
//

#include "stdafx.h"
#include "Fotografix.h"
#include "TextDialog.h"

// CTextDialog dialog

IMPLEMENT_DYNAMIC(CTextDialog, CDialog)

CTextDialog::CTextDialog(CWnd* pParent /*=NULL*/)
	: CDialog(CTextDialog::IDD, pParent)
	, aa(false)
	, mode(0)
{
	ZeroMemory(&logFont, sizeof(logFont));
}

CTextDialog::~CTextDialog()
{
}

void CTextDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_OPACITY, opacity);
	DDV_MinMaxInt(pDX, opacity, 0, 100);
	DDX_Text(pDX, IDC_TEXT, text);
	DDX_Control(pDX, IDC_TEXT, edit);
	DDX_Control(pDX, IDC_COLOR, colorPicker);
	DDX_Check(pDX, IDC_AA, aa);
	DDX_CBIndex(pDX, IDC_MODE, mode);
}


BEGIN_MESSAGE_MAP(CTextDialog, CDialog)
	ON_BN_CLICKED(IDC_CHOOSE, &CTextDialog::OnBnClickedChoose)
	ON_COMMAND(IDC_COLOR, &CTextDialog::OnStnClickedColor)
	ON_WM_CTLCOLOR()
END_MESSAGE_MAP()


// CTextDialog message handlers

void CTextDialog::OnBnClickedChoose()
{
	CFontDialog dlg(&logFont);
	if (dlg.DoModal() == IDOK) {
		font.DeleteObject();
		font.CreateFontIndirect(&logFont);

		edit.SetFont(&font);
	}
}

BOOL CTextDialog::OnInitDialog()
{
	CDialog::OnInitDialog();
	TranslateDialog(this);

	font.CreateFontIndirect(&logFont);
	edit.SetFont(&font);
	colorPicker.SetColor(color);

	edit.SetFocus();
	return false;
}

void CTextDialog::OnStnClickedColor()
{
	color = colorPicker.GetColor();
	edit.Invalidate();
}

HBRUSH CTextDialog::OnCtlColor(CDC* pDC, CWnd* pWnd, UINT nCtlColor)
{
	HBRUSH hBrush = CDialog::OnCtlColor(pDC, pWnd, nCtlColor);

	if (pWnd == &edit)
		pDC->SetTextColor(color);

	return hBrush;
}
