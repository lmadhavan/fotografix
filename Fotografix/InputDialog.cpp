// InputDialog.cpp : implementation file
//

#include "stdafx.h"
#include "Fotografix.h"
#include "InputDialog.h"


// CInputDialog dialog

IMPLEMENT_DYNAMIC(CInputDialog, CDialog)

CInputDialog::CInputDialog(CWnd* pParent /*=NULL*/)
	: CDialog(CInputDialog::IDD, pParent)
	, number(false)
{

}

CInputDialog::~CInputDialog()
{
}

void CInputDialog::DoDataExchange(CDataExchange* pDX)
{
	CDialog::DoDataExchange(pDX);
	DDX_Text(pDX, IDC_PROMPT, prompt);
	DDX_Text(pDX, IDC_VALUE, value);
}


BEGIN_MESSAGE_MAP(CInputDialog, CDialog)
END_MESSAGE_MAP()


// CInputDialog message handlers

BOOL CInputDialog::OnInitDialog()
{
	CDialog::OnInitDialog();
	TranslateDialog(this);

	SetWindowText(title);
	if (number == true) GetDlgItem(IDC_VALUE)->ModifyStyle(0, ES_NUMBER);

	return true;
}
