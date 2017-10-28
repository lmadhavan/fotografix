// RawDialog.cpp : implementation file
//

#include "stdafx.h"
#include "Fotografix.h"
#include "RawDialog.h"


// CRawDialog dialog

IMPLEMENT_DYNAMIC(CRawDialog, CDialog)

CRawDialog::CRawDialog(CWnd* pParent /*=NULL*/)
	: CDialog(CRawDialog::IDD, pParent)
	, size(_T(""))
	, w(1)
	, h(1)
	, header(0)
{

}

CRawDialog::~CRawDialog()
{
}

void CRawDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_SIZE, size);
	DDX_Text(pDX, IDC_W, w);
	DDX_Text(pDX, IDC_H, h);
	DDV_MinMaxInt(pDX, w, 1, 32000);
	DDV_MinMaxInt(pDX, h, 1, 32000);
	DDX_Text(pDX, IDC_HEADER, header);
}


BEGIN_MESSAGE_MAP(CRawDialog, CDialog)
	ON_BN_CLICKED(IDOK, &CRawDialog::OnBnClickedOk)
END_MESSAGE_MAP()


// CRawDialog message handlers

BOOL CRawDialog::OnInitDialog()
{
	CDialog::OnInitDialog();
	TranslateDialog(this);

	size.Format(TEXT("%I64d %s"), length, LangItem(bytes));
	UpdateData(false);

	return true;
}

void CRawDialog::OnBnClickedOk()
{
	if (UpdateData() == true) {
		if (w * h * 3 + header > length)
			MessageBox(LangMessage(RAWError1), NULL, MB_ICONSTOP | MB_OK);
		else
			EndDialog(IDOK);
	}
}
