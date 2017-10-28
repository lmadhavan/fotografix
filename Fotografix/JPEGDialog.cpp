// JPEGDialog.cpp : implementation file
//

#include "stdafx.h"
#include "Fotografix.h"
#include "JPEGDialog.h"


// CJPEGDialog dialog

IMPLEMENT_DYNAMIC(CJPEGDialog, CDialog)

CJPEGDialog::CJPEGDialog(CWnd* pParent /*=NULL*/)
	: CDialog(CJPEGDialog::IDD, pParent)
	, quality(85)
{

}

CJPEGDialog::~CJPEGDialog()
{
}

void CJPEGDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_EDIT1, quality);
	DDX_Control(pDX, IDC_SLIDER1, slider);
	DDV_MinMaxInt(pDX, quality, 0, 100);
}


BEGIN_MESSAGE_MAP(CJPEGDialog, CDialog)
	ON_WM_HSCROLL()
	ON_EN_CHANGE(IDC_EDIT1, &CJPEGDialog::OnEnChangeEdit1)
END_MESSAGE_MAP()


// CJPEGDialog message handlers

BOOL CJPEGDialog::OnInitDialog()
{
	CDialog::OnInitDialog();
	TranslateDialog(this);

	slider.SetPos(quality);
	slider.SetRange(0, 100);
	slider.SetTicFreq(10);

	return true;
}

void CJPEGDialog::OnHScroll(UINT nSBCode, UINT nPos, CScrollBar* pScrollBar)
{
	quality = slider.GetPos();
	UpdateData(false);
}

void CJPEGDialog::OnEnChangeEdit1()
{
	if (UpdateData())
		slider.SetPos(quality);
}
